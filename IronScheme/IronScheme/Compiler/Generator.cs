#region License
/* ****************************************************************************
 * Copyright (c) Llewellyn Pritchard. 
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 * ***************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Scripting.Ast;
using IronScheme.Runtime;
using Microsoft.Scripting;
using IronScheme.Hosting;
using Microsoft.Scripting.Actions;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Utils;
using System.Diagnostics;
using Microsoft.Scripting.Math;

[assembly: Extension(GeneratorType=typeof(Generator), BuiltinsType=typeof(Builtins))]

namespace IronScheme.Compiler
{

  public partial class Generator
  {
    static Generator()
    {
      Initialize();
    }

    [ThreadStatic]
    static SourceSpan spanhint;

    protected static SourceSpan SpanHint
    {
      get { return Generator.spanhint; }
      set { Generator.spanhint = value; }
    }

    // this is probably not very threadsafe....
    protected static Dictionary<SymbolId, CodeBlockExpression> references = new Dictionary<SymbolId, CodeBlockExpression>();

    protected internal readonly static FieldInfo Unspecified = typeof(Builtins).GetField("Unspecified");

    protected static Expression GetCons(object args, CodeBlock cb)
    {
      Cons c = args as Cons;
      if (c != null)
      {
        //if (!IsSimpleCons(c))
        {
          //return Ast.Constant(new IronSchemeConstant(c));
        }
        return GetConsList(c, cb);
      }
      object[] v = args as object[];
      if (v != null)
      {
        return GetConsVector(v, cb);
      }
      else if (args is Fraction)
      {
        Fraction f = (Fraction) args;
        return Ast.New(Fraction_New, Ast.Constant(f.Numerator), Ast.Constant(f.Denominator));
      }
      else
      {
        if (args is long)
        {
          args = (BigInteger)(long)args;
        }
        return Ast.Constant(args);
      }
    }

    static bool IsSimpleCons(Cons c)
    {
      if (c == null)
      {
        return true;
      }
      return !(c.car is Cons) && (c.cdr == null || IsSimpleCons(c.cdr as Cons));
    }

    protected readonly static Dictionary<SymbolId, bool> assigns = new Dictionary<SymbolId, bool>();

    protected internal static Expression GetAst(object args, CodeBlock cb)
    {
      Cons c = args as Cons;
      if (c != null)
      {
        if (Builtins.IsTrue(Builtins.IsSymbol(c.car)))
        {
          SymbolId f = (SymbolId)c.car;

          Variable var = cb.Lookup(f);

          if (var != null && !assigns.ContainsKey(f))
          {
            var = null;
          }

          object m;
          CodeBlockExpression cbe;

          // needs to do the same for overloads...
          if (SimpleGenerator.libraryglobals.TryGetValue(f, out cbe))
          {
            Expression[] ppp = GetAstList(c.cdr as Cons, cb);

            if (ppp.Length < 6)
            {
              return CallNormal(cbe, ppp);
            }
          }

          // varargs
          if (SimpleGenerator.libraryglobalsX.TryGetValue(f, out cbe))
          {
            Expression[] ppp = GetAstList(c.cdr as Cons, cb);

            if (ppp.Length < 6)
            {
              return CallVarArgs(cbe, ppp);
            }
          }

          // overloads
          CodeBlockDescriptor[] cbd;
          if (SimpleGenerator.libraryglobalsN.TryGetValue(f, out cbd))
          {
            Expression[] ppp = GetAstList(c.cdr as Cons, cb);

            foreach (CodeBlockDescriptor d in cbd)
            {
              if (ppp.Length == d.arity || (d.varargs && ppp.Length > d.arity))
              {
                if (d.varargs)
                {
                  return CallVarArgs(d.codeblock, ppp);
                }
                else
                {
                  return CallNormal(d.codeblock, ppp);
                }
              }
            }
          }

          if (Context.Scope.TryLookupName(f, out m))
          {
            if (var == null)
            {
              IGenerator gh = m as IGenerator;
              if (gh != null)
              {
                if (!Parser.sourcemap.TryGetValue(c, out spanhint))
                {
                  spanhint = SourceSpan.None;
                }
                return gh.Generate(c.cdr, cb);
              }

              BuiltinMethod bf = m as BuiltinMethod;
              if (bf != null)
              {
                // check for inline emitter
                InlineEmitter ie;
                if (inlineemitters.TryGetValue(f, out ie))
                {
                  Expression result = ie(GetAstList(c.cdr as Cons, cb));
                  // if null is returned, the method cannot be inlined
                  if (result != null)
                  {
                    if (result.Type.IsValueType)
                    {
                      result = Ast.Convert(result, typeof(object));
                    }
                    return result;
                  }
                }

                MethodBinder mb = bf.Binder;
                Expression[] pars = GetAstList(c.cdr as Cons, cb);

                Type[] types = GetExpressionTypes(pars);
                MethodCandidate mc = mb.MakeBindingTarget(CallType.None, types);
                if (mc != null)
                {
                  if (mc.Target.NeedsContext)
                  {
                    pars = ArrayUtils.Insert<Expression>(Ast.CodeContext(), pars);
                  }
                  MethodBase meth = mc.Target.Method;

                  return Ast.ComplexCallHelper(meth as MethodInfo, pars);
                }
              }
              Closure clos = m as Closure;
              if (clos != null)
              {
                // no provision for varargs
                MethodInfo[] mis = clos.Targets;
                if (mis.Length > 0)
                {

                  MethodBinder mb = MethodBinder.MakeBinder(binder, SymbolTable.IdToString(f), mis, BinderType.Normal);

                  Expression[] pars = GetAstList(c.cdr as Cons, cb);

                  Type[] types = GetExpressionTypes(pars);
                  MethodCandidate mc = mb.MakeBindingTarget(CallType.None, types);
                  if (mc != null)
                  {
                    if (mc.Target.NeedsContext)
                    {
                      pars = ArrayUtils.Insert<Expression>(Ast.CodeContext(), pars);
                    }
                    MethodBase meth = mc.Target.Method;

                    return Ast.ComplexCallHelper(meth as MethodInfo, pars);
                  }
                }
                // check for overload thing
              }
            }
          }
        }

        Expression[] pp = GetAstList(c.cdr as Cons, cb);

        Expression ex = Unwrap(GetAst(c.car, cb));
        if (ex is MethodCallExpression)
        {
          MethodCallExpression mcexpr = (MethodCallExpression)ex;
          if (mcexpr.Method == Closure_Make)
          {
            CodeBlockExpression cbe = mcexpr.Arguments[1] as CodeBlockExpression;

            bool needscontext = true;
            MethodInfo dc = GetDirectCallable(needscontext, pp.Length);
            if (needscontext)
            {
              pp = ArrayUtils.Insert<Expression>(mcexpr.Arguments[0], pp);
            }
            return Ast.ComplexCallHelper(cbe, dc, pp);
          }
          if (mcexpr.Instance is MethodCallExpression && mcexpr.Method.Name == "Call")
          {
            MethodCallExpression mcei = mcexpr.Instance as MethodCallExpression;
            if (mcei.Method == Closure_Make)
            {
              CodeBlockExpression cbe = mcei.Arguments[1] as CodeBlockExpression;
              Debug.Assert(mcexpr.Arguments.Count == 0);
              bool needscontext = true;
              MethodInfo dc = GetDirectCallable(needscontext, 0);
              ex = Ast.ComplexCallHelper(cbe, dc, mcei.Arguments[0]);
            }
          }
        }

        if (ex is ConstantExpression)
        {
          Builtins.SyntaxError(SymbolTable.StringToId("generator"), "expecting a procedure", c.car, c);
        }

        ex = Ast.ConvertHelper(ex, typeof(ICallable));
        
        MethodInfo call = GetCallable(pp.Length);

        Expression r = pp.Length > 5 ?
          Ast.Call(ex, call, Ast.NewArray(typeof(object[]), pp)) :
          Ast.Call(ex, call, pp);

        if (spanhint != SourceSpan.Invalid || spanhint != SourceSpan.None)
        {
          r.SetLoc(spanhint);
        }

        return r;
      }
      object[] v = args as object[];
      if (v != null)
      {
        return GetConsVector(v, cb);
      }
      else
      {
        if (args is SymbolId)
        {
          return Read((SymbolId)args, cb, typeof(object));
        }
        if (args == Builtins.Unspecified)
        {
          return Ast.ReadField(null, Unspecified);
        }
        if (args is Fraction)
        {
          Fraction f = (Fraction)args;
          return Ast.New(Fraction_New, Ast.Constant(f.Numerator), Ast.Constant(f.Denominator));
        }
        return Ast.Constant(args);
      }
    }

    static Expression CallNormal(CodeBlockExpression cbe, Expression[] ppp)
    {
      bool needscontext = true;
      MethodInfo dc = GetDirectCallable(needscontext, ppp.Length);
      if (needscontext)
      {
        ppp = ArrayUtils.Insert<Expression>(Ast.CodeContext(), ppp);
      }

      cbe = Ast.CodeBlockReference(cbe.Block, CallTargets.GetTargetType(true, ppp.Length - 1, false));

      return Ast.ComplexCallHelper(cbe, dc, ppp);
    }

    static Expression CallVarArgs(CodeBlockExpression cbe, Expression[] ppp)
    {
      bool needscontext = true;

      int pc = cbe.Block.ParameterCount;

      Expression[] tail = new Expression[ppp.Length - (pc - 1)];

      Array.Copy(ppp, ppp.Length - tail.Length, tail, 0, tail.Length);

      Expression[] nppp = new Expression[pc];

      Array.Copy(ppp, nppp, ppp.Length - tail.Length);

      if (tail.Length > 0)
      {
        nppp[nppp.Length - 1] = Ast.Call(MakeList(tail, true), tail);
      }
      else
      {
        nppp[nppp.Length - 1] = Ast.Null();
      }

      ppp = nppp;

      MethodInfo dc = GetDirectCallable(needscontext, pc);
      if (needscontext)
      {
        ppp = ArrayUtils.Insert<Expression>(Ast.CodeContext(), ppp);
      }

      cbe = Ast.CodeBlockReference(cbe.Block, CallTargets.GetTargetType(true, ppp.Length - 1, false));

      return Ast.ComplexCallHelper(cbe, dc, ppp);
    }

    protected static Expression Unwrap(Expression ex)
    {
      while (ex is UnaryExpression && ((UnaryExpression)ex).NodeType == AstNodeType.Convert)
      {
        ex = ((UnaryExpression)ex).Operand;
      }

      return ex;
    }

  }

}
