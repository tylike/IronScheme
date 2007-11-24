//
//  This CSharp output file generated by Gardens Point LEX
//  Version:  0.5.1.126 (2007-03-28)
//  Machine:  UBER-VISTA
//  DateTime: 2007/11/04 01:20:11 AM
//  UserName: leppie
//  GPLEX input file <IronScheme.lex>
//  GPLEX frame file <C:\dev\IronScheme\tools\gplexx.frame>
//
//  Option settings: noverbose, parser, stack, minimize, compress
//

#define BACKUP
#define STACK
//
// gplexx.frame
// Version 0.5.1 of 11 March 2007
// Derived from gplex.frame version of 2-September-2006. 
// Left and Right Anchored state support.
// Start condition stack. Two generic params.
// Using fixed length context handling for right anchors
//
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Scripting;
#if !STANDALONE
using gppg;
#endif


namespace IronScheme.Compiler
{   
    /// <summary>
    /// Summary Canonical example of GPLEX automaton
    /// </summary>
    
#if STANDALONE
    //
    // These are the dummy declarations for stand-alone GPLEX applications
    // normally these declarations would come from the parser.
    // If you declare /noparser, or %option noparser then you get this.
    //

    public enum Tokens
    { 
      EOF = 0, maxParseToken = int.MaxValue 
      // must have at least these two, values are almost arbitrary
    }
    
    [CLSCompliant(false)]
    public abstract class ScanBase
    {
        public abstract int yylex();
#if BABEL
        protected abstract int CurrentSc { get; set; }
        // EolState is the 32-bit of state data persisted at 
        // the end of each line for Visual Studio colorization.  
        // The default is to return CurrentSc.  You must override
        // this if you want more complicated behavior.
        public virtual int EolState { get { return CurrentSc; set { CurrentSc = value; } }
    }
    
    public interface IColorScan
    {
        void SetSource(string source, int offset);
        int GetNext(ref int state, out int start, out int end);
#endif // BABEL
    }

#endif // STANDALONE

    public abstract class ScanBuff
    {
        public const char EOF = (char)256;
        public abstract int Pos { get; set; }
        public abstract int Read();
        public abstract int Peek();
        public abstract string GetString(int b, int e);
    }
    
    // If the compiler can't find ScanBase maybe you need to run
    // GPPG with the /gplex option, or GPLEX with /noparser
#if BABEL
    public sealed class Scanner : ScanBase, IColorScan
    {
        public ScanBuff buffer;
        int currentScOrd;  // start condition ordinal
        
        protected override int CurrentSc 
        {
             // The current start state is a property
             // to try to avoid the user error of setting
             // scState but forgetting to update the FSA
             // start state "currentStart"
             //
             get { return currentScOrd; }  // i.e. return YY_START;
             set { currentScOrd = value;   // i.e. BEGIN(value);
                   currentStart = startState[value]; }
        }
#else  // BABEL
    public sealed class Scanner : ScanBase
    {
        public ScanBuff buffer;
        int currentScOrd;  // start condition ordinal
#endif // BABEL
        
        private static int GetMaxParseToken() {
            System.Reflection.FieldInfo f = typeof(Tokens).GetField("maxParseToken");
            return (f == null ? int.MaxValue : (int)f.GetValue(null));
        }
        
        static int parserMax = GetMaxParseToken();
        
        enum Result {accept, noMatch, contextFound};

        const int maxAccept = 41;
        const int initial = 42;
        const int eofNum = 0;
        const int goStart = -1;
        const int INITIAL = 0;
        const int ML_COMMENT = 1;

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
public override void yyerror(string format, params object[] args)
{
  if (!format.Contains("EOF"))
  {
    Console.Error.WriteLine(format, args);
  }
}

public int MakeChar()
{
  switch (yytext)
  {
    case "#\\":
    case "#\\space":
      yylval.text = " ";
      break;
    case "#\\newline":
      yylval.text = "\n";
      break;
    default:
      yylval.text = yytext[2].ToString();
      break;
  }
  yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
  return (int)Tokens.CHARACTER;
}

public int Make(Tokens token)
{
  yylval.text = yytext;
  yylloc = new LexLocation(tokLin,tokCol,tokELin,tokECol);
  return (int)token;
}
/*
Identifiers.  Identifiers may denote variables, keywords, or symbols, depending upon context. They are formed from sequences of letters, 
digits, and special characters. With three exceptions, identifiers cannot begin with a character that can also begin a number, i.e., 
they cannot begin with ., +, -, or a digit. The three exceptions are the identifiers ..., +, and -. Case is insignificant in identifiers 
so that, for example, newspaper, NewsPaper, and NEWSPAPER all represent the same identifier. 

<identifier>  <initial> <subsequent>* | + | - | ... 
<initial>  <letter> | ! | $ | % | & | * | / | : | < | = | > | ? | ~ | _ | ^ 
<subsequent>  <initial> | <digit> | . | + | - | @ 
<letter>  a | b | ... | z 
<digit>  0 | 1 | ... | 9  


*/
        int state;
        int currentStart = initial;
        int chr;           // last character read
        int cPos;          // position of chr
        int lNum = 0;      // current line number
        int lineStart;     // start of line

        //
        // The following instance variables are used, among other
        // things, for constructing the yylloc location objects.
        //
        int tokPos;        // buffer position at start of token
        int tokCol;        // zero-based column number at start of token
        int tokLin;        // line number at start of token
        int tokEPos;       // buffer position at end of token
        int tokECol;       // column number at end of token
        int tokELin;       // line number at end of token
        string tokTxt;     // lazily constructed text of token
#if STACK          
        private Stack<int> scStack = new Stack<int>();
#endif // STACK

#region ScannerTables
   struct Table {
        public int min; public int rng; public int dflt;
        public sbyte[] nxt;
        public Table(int m, int x, int d, sbyte[] n) {
            min = m; rng = x; dflt = d; nxt = n;
        }
    };

   static int[] startState = {42, 57, 0};

    static Table[] NxS = new Table[58];

    static Scanner() {
    NxS[0] = new Table(0, 0, 0, null);
    NxS[1] = new Table(0, 0, -1, null);
    NxS[2] = new Table(9, 24, -1, new sbyte[] {2, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, 2});
    NxS[3] = new Table(0, 0, -1, null);
    NxS[4] = new Table(10, 1, -1, new sbyte[] {3});
    NxS[5] = new Table(33, 94, -1, new sbyte[] {5, -1, -1, 5, 5, 5, 
        -1, -1, -1, 5, 5, -1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, -1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, -1, -1, -1, 5, 5, -1, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, -1, -1, -1, 5});
    NxS[6] = new Table(10, 83, 55, new sbyte[] {-1, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 38, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 56});
    NxS[7] = new Table(40, 85, -1, new sbyte[] {31, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 32, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, 33, -1, -1, -1, -1, -1, -1, -1, 34});
    NxS[8] = new Table(0, 0, -1, null);
    NxS[9] = new Table(0, 0, -1, null);
    NxS[10] = new Table(0, 0, -1, null);
    NxS[11] = new Table(48, 10, -1, new sbyte[] {28, 29, 29, 29, 29, 29, 
        29, 29, 29, 29});
    NxS[12] = new Table(64, 1, -1, new sbyte[] {30});
    NxS[13] = new Table(46, 12, -1, new sbyte[] {47, -1, 25, 25, 25, 25, 
        25, 25, 25, 25, 25, 25});
    NxS[14] = new Table(46, 75, -1, new sbyte[] {43, -1, 15, 15, 15, 15, 
        15, 15, 15, 15, 15, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        20, 44, 20, -1, -1, -1, -1, -1, 21, 20, -1, -1, -1, -1, -1, -1, 
        -1, 22, -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        20, 44, 20, -1, -1, -1, -1, -1, 21, 20, -1, -1, -1, -1, -1, -1, 
        -1, 22, -1, -1, 46});
    NxS[15] = new Table(46, 72, -1, new sbyte[] {43, -1, 15, 15, 15, 15, 
        15, 15, 15, 15, 15, 15, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        20, 44, 20, -1, -1, -1, -1, -1, 21, 20, -1, -1, -1, -1, -1, -1, 
        -1, 22, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        20, 44, 20, -1, -1, -1, -1, -1, 21, 20, -1, -1, -1, -1, -1, -1, 
        -1, 22});
    NxS[16] = new Table(10, 1, 16, new sbyte[] {-1});
    NxS[17] = new Table(0, 0, -1, null);
    NxS[18] = new Table(0, 0, -1, null);
    NxS[19] = new Table(0, 0, -1, null);
    NxS[20] = new Table(0, 0, -1, null);
    NxS[21] = new Table(85, 33, -1, new sbyte[] {23, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 23});
    NxS[22] = new Table(76, 33, -1, new sbyte[] {23, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 23});
    NxS[23] = new Table(0, 0, -1, null);
    NxS[24] = new Table(48, 62, -1, new sbyte[] {24, 24, 24, 24, 24, 24, 
        24, 24, 24, 24, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 20, -1, 
        20, -1, -1, -1, -1, -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 20, -1, 
        20, -1, -1, -1, -1, -1, -1, 20});
    NxS[25] = new Table(48, 62, -1, new sbyte[] {25, 25, 25, 25, 25, 25, 
        25, 25, 25, 25, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 20, 44, 
        20, -1, -1, -1, -1, -1, -1, 20, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 20, 44, 
        20, -1, -1, -1, -1, -1, -1, 20});
    NxS[26] = new Table(48, 70, -1, new sbyte[] {26, 26, 26, 26, 26, 26, 
        26, 26, 26, 26, -1, -1, -1, -1, -1, -1, -1, 26, 26, 26, 26, 26, 
        26, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 26, 26, 26, 26, 
        26, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22});
    NxS[27] = new Table(0, 0, -1, null);
    NxS[28] = new Table(48, 73, -1, new sbyte[] {29, 29, 29, 29, 29, 29, 
        29, 29, 29, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22, 
        -1, -1, 46, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22, 
        -1, -1, 46});
    NxS[29] = new Table(48, 70, -1, new sbyte[] {29, 29, 29, 29, 29, 29, 
        29, 29, 29, 29, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, 21, -1, -1, -1, -1, -1, -1, -1, -1, 22});
    NxS[30] = new Table(0, 0, -1, null);
    NxS[31] = new Table(0, 0, -1, null);
    NxS[32] = new Table(10, 106, 35, new sbyte[] {-1, 35, 35, 35, 35, 35, 
        35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
        -1, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
        35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
        35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
        35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 
        35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 36, 35, 
        35, 35, 35, 37});
    NxS[33] = new Table(0, 0, -1, null);
    NxS[34] = new Table(0, 0, -1, null);
    NxS[35] = new Table(0, 0, -1, null);
    NxS[36] = new Table(101, 1, -1, new sbyte[] {51});
    NxS[37] = new Table(112, 1, -1, new sbyte[] {48});
    NxS[38] = new Table(0, 0, -1, null);
    NxS[39] = new Table(10, 115, 39, new sbyte[] {-1, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, -1});
    NxS[40] = new Table(35, 1, -1, new sbyte[] {41});
    NxS[41] = new Table(0, 0, -1, null);
    NxS[42] = new Table(9, 118, 1, new sbyte[] {2, 3, 1, 1, 4, 1, 
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 
        1, 2, 5, 6, 7, 5, 5, 5, 8, 9, 10, 5, 11, 12, 11, 13, 
        5, 14, 15, 15, 15, 15, 15, 15, 15, 15, 15, 5, 16, 5, 5, 5, 
        5, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 17, 1, 18, 5, 
        5, 19, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 
        5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, 1, 1, 5});
    NxS[43] = new Table(48, 10, -1, new sbyte[] {25, 25, 25, 25, 25, 25, 
        25, 25, 25, 25});
    NxS[44] = new Table(43, 15, -1, new sbyte[] {45, -1, 45, -1, -1, 24, 
        24, 24, 24, 24, 24, 24, 24, 24, 24});
    NxS[45] = new Table(48, 10, -1, new sbyte[] {24, 24, 24, 24, 24, 24, 
        24, 24, 24, 24});
    NxS[46] = new Table(48, 55, -1, new sbyte[] {26, 26, 26, 26, 26, 26, 
        26, 26, 26, 26, -1, -1, -1, -1, -1, -1, -1, 26, 26, 26, 26, 26, 
        26, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 26, 26, 26, 26, 26, 
        26});
    NxS[47] = new Table(46, 1, -1, new sbyte[] {27});
    NxS[48] = new Table(97, 1, -1, new sbyte[] {49});
    NxS[49] = new Table(99, 1, -1, new sbyte[] {50});
    NxS[50] = new Table(101, 1, -1, new sbyte[] {35});
    NxS[51] = new Table(119, 1, -1, new sbyte[] {52});
    NxS[52] = new Table(108, 1, -1, new sbyte[] {53});
    NxS[53] = new Table(105, 1, -1, new sbyte[] {54});
    NxS[54] = new Table(110, 1, -1, new sbyte[] {50});
    NxS[55] = new Table(10, 83, 55, new sbyte[] {-1, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 38, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 
        55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 55, 56});
    NxS[56] = new Table(34, 85, -1, new sbyte[] {55, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
        -1, -1, -1, -1, 55, -1, -1, -1, -1, 55, 55, -1, -1, -1, 55, -1, 
        -1, -1, -1, -1, -1, -1, 55, -1, -1, -1, 55, -1, 55, -1, 55});
    NxS[57] = new Table(10, 115, 39, new sbyte[] {-1, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 
        39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 39, 40});
    }

int NextState(int qStat) {
    if (chr == ScanBuff.EOF)
        return (qStat <= maxAccept && qStat != currentStart ? currentStart : eofNum);
    else {
        int rslt;
        uint idx = (byte)(chr - NxS[qStat].min);
        if (idx >= (uint)NxS[qStat].rng) rslt = NxS[qStat].dflt;
        else rslt = NxS[qStat].nxt[idx];
        return (rslt == goStart ? currentStart : rslt);
    }
}

int NextState() {
    if (chr == ScanBuff.EOF)
        return (state <= maxAccept && state != currentStart ? currentStart : eofNum);
    else {
        int rslt;
        uint idx = (byte)(chr - NxS[state].min);
        if (idx >= (uint)NxS[state].rng) rslt = NxS[state].dflt;
        else rslt = NxS[state].nxt[idx];
        return (rslt == goStart ? currentStart : rslt);
    }
}
#endregion


#if BACKUP
        // ====================== Nested class ==========================

        internal class Context // class used for automaton backup.
        {
            public int bPos;
            public int cPos;
            public int state;
            public int cChr;
        }
#endif // BACKUP


        // ====================== Nested class ==========================

        public sealed class StringBuff : ScanBuff
        {
            string str;        // input buffer
            int bPos;          // current position in buffer
            int sLen;

            public StringBuff(string str)
            {
                this.str = str;
                this.sLen = str.Length;
            }

            public override int Read()
            {
                if (bPos < sLen) return str[bPos++];
#if BABEL
                else if (bPos == sLen) { bPos++; return '\n'; }   // one strike, see newline
#endif // BABEL
                else return EOF;                                  // two strikes and you're out!
            }

            public override int Peek()
            {
                if (bPos < sLen) return str[bPos];
                else return '\n';
            }

            public override string GetString(int beg, int end)
            {
                //  "end" can be greater than sLen with the BABEL
                //  option set.  Read returns a "virtual" EOL if
                //  an attempt is made to read past the end of the
                //  string buffer.  Without the guard any attempt 
                //  to fetch yytext for a token that includes the 
                //  EOL will throw an index exception.
                if (end > sLen) end = sLen;
                if (end <= beg) return ""; 
                else return str.Substring(beg, end - beg);
            }

            public override int Pos
            {
                get { return bPos; }
                set { bPos = value; }
            }
        }

        // ====================== Nested class ==========================

        public sealed class StreamBuff : ScanBuff
        {
            BufferedStream bStrm;   // input buffer

            public StreamBuff(Stream str) { this.bStrm = new BufferedStream(str); }

            public override int Read()
            {
                int byt = bStrm.ReadByte();
                if (byt == -1) return EOF;
                else return byt;
            }

            public override int Peek()
            {
                int rslt = Read();
                bStrm.Seek(-1, SeekOrigin.Current);
                return rslt;
            }

            public override string GetString(int beg, int end)
            {
                if (end - beg <= 0) return "";
                long savePos = bStrm.Position;
                char[] arr = new char[end - beg];
                bStrm.Position = (long)beg;
                for (int i = 0; i < (end - beg); i++)
                    arr[i] = (char)Read();
                bStrm.Position = savePos;
                return new String(arr);
            }

            public override int Pos
            {
                get { return (int)bStrm.Position; }
                set { bStrm.Position = value; }
            }
        }

        // =================== End Nested classes =======================

        public Scanner(Stream file) {
            buffer = new StreamBuff(file);
            this.cPos = -1;
            this.chr = '\n'; // to initialize yyline, yycol and lineStart
            GetChr();
        }

        public Scanner() { }

        void GetChr()
        {
            if (chr == '\n') { lineStart = buffer.Pos; lNum++; }
            chr = buffer.Read(); cPos++;
        }

        void MarkToken()
        {
            tokPos = cPos;
            tokLin = lNum;
            tokCol = cPos - lineStart;
        }
        
        void MarkEnd()
        {
            tokTxt = null;
            tokEPos = cPos;
            tokELin = lNum;
            tokECol = cPos - lineStart;

        }

        // ================ StringBuffer Initialization ===================

        public void SetSource(string source, int offset)
        {
            this.buffer = new StringBuff(source);
            this.buffer.Pos = offset;
            this.cPos = offset - 1;
            this.chr = '\n'; // to initialize yyline, yycol and lineStart
            GetChr();
        }
        
#if BABEL
        //
        //  Get the next token for Visual Studio
        //
        //  "state" is the inout mode variable that maintains scanner
        //  state between calls, using the EolState property. In principle,
        //  if the calls of EolState are costly set could be called once
        //  only per line, at the start; and get called only at the end
        //  of the line. This needs more infrastructure ...
        public int GetNext(ref int state, out int start, out int end)
        {
            Tokens next;
            int s, e;
            s = state;        // state at start
            EolState = state;
            next = (Tokens)Scan();
            state = EolState;
            e = state;       // state at end;
            start = tokPos;
            end = tokEPos - 1; // end is the index of last char.
            // if (next > Tokens.maxParseToken)
            //     Trace.WriteLine(String.Format("({0}-{1}) {2}->{3} {4}", start, end, s, e, next.ToString()));
            return (int)next;
        }        
#endif // BABEL

        // ======== IScanner<> Implementation =========

        public override int yylex()
        {
            // parserMax is set by reflecting on the Tokens
            // enumeration.  If maxParseTokeen is defined
            // that is used, otherwise int.MaxValue is used.
            int next;
            do { next = Scan(); } while (next >= parserMax);
            return next;
        }
        
        int yyleng { get { return tokEPos - tokPos; } }
        int yypos { get { return tokPos; } }
        int yyline { get { return tokLin; } }
        int yycol { get { return tokCol; } }

        public string yytext
        {
            get 
            {
                if (tokTxt == null) 
                    tokTxt = buffer.GetString(tokPos, tokEPos);
                return tokTxt;
            }
        }

        void yyless(int n) { 
            int pos = tokPos + n;
            buffer.Pos = pos;
            cPos = pos - 1; 
            GetChr(); 
            MarkEnd(); 
        }

        // ============ methods available in actions ==============

        internal int YY_START {
            get { return currentScOrd; }
            set { currentScOrd = value; } 
        }
        
        internal void BEGIN(int next) {
            currentScOrd = next;
            currentStart = startState[next];
        }

        // ============== The main tokenizer code =================

        int Scan()
        {
                for (; ; )
                {
                    int next;              // next state to enter                   
#if BACKUP
                    bool inAccept = false; // inAccept ==> current state is an accept state
                    Result rslt = Result.noMatch;
                    // skip "idle" states
#if LEFTANCHORS
                    if (lineStart == cPos &&
                        NextState(anchorState[currentScOrd]) != currentStart)
                        state = anchorState[currentScOrd];
                    else {
                        state = currentStart;
                        while (NextState() == currentStart) {
                            GetChr();
                            if (lineStart == cPos &&
                                NextState(anchorState[currentScOrd]) != currentStart) {
                                state = anchorState[currentScOrd];
                                break;
                            }
                        }
                    }
#else // !LEFTANCHORS
                    state = currentStart;
                    while (NextState() == state) 
                        GetChr(); // skip "idle" transitions
#endif // LEFTANCHORS
                    MarkToken();
                    
                    while ((next = NextState()) != currentStart)
                        if (inAccept && next > maxAccept) // need to prepare backup data
                        {
                            Context ctx = new Context();
                            rslt = Recurse2(ctx, next);
                            if (rslt == Result.noMatch) RestoreStateAndPos(ctx);
                            // else if (rslt == Result.contextFound) RestorePos(ctx);
                            break;
                        }
                        else
                        {
                            state = next;
                            GetChr();
                            if (state <= maxAccept) inAccept = true;
                        }
#else // !BACKUP
#if LEFTANCHORS
                    if (lineStart == cPos &&
                        NextState(anchorState[currentScOrd]) != currentStart)
                        state = anchorState[currentScOrd];
                    else {
                        state = currentStart;
                        while (NextState() == currentStart) {
                            GetChr();
                            if (lineStart == cPos &&
                                NextState(anchorState[currentScOrd]) != currentStart) {
                                state = anchorState[currentScOrd];
                                break;
                            }
                        }
                    }
#else // !LEFTANCHORS
                    state = currentStart;
                    while (NextState() == state) 
                        GetChr(); // skip "idle" transitions
#endif // LEFTANCHORS
                    MarkToken();
                    // common code
                    while ((next = NextState()) != currentStart)
                    {
                        state = next;
                        GetChr();
                    }
#endif // BACKUP
                    if (state > maxAccept) 
                    {
                        state = currentStart;
                    }
                    else
                    {
                        MarkEnd();
#pragma warning disable 162												
#region ActionSwitch
    switch (state)
    {
        case eofNum:
            switch (currentStart) {
                case 42:
{ }
                    break;
            }
            return (int)Tokens.EOF;
        case 1:
        case 4:
        case 6:
        case 7:
Errors.Add(SourceUnit, string.Format("Bad input: '{0}'", yytext), 
                          new SourceSpan( new SourceLocation(1,tokLin,tokCol + 1) , new SourceLocation(1,tokLin,tokCol + yytext.Length + 1)), 2, Microsoft.Scripting.Hosting.Severity.Error);
            break;
        case 2:
;
            break;
        case 3:
;
            break;
        case 5:
        case 11:
        case 27:
return Make(Tokens.SYMBOL);
            break;
        case 8:
return Make(Tokens.QUOTE);
            break;
        case 9:
return Make(Tokens.LBRACE);
            break;
        case 10:
return Make(Tokens.RBRACE);
            break;
        case 12:
return Make(Tokens.UNQUOTE);
            break;
        case 13:
return Make(Tokens.DOT);
            break;
        case 14:
        case 15:
        case 21:
        case 22:
        case 23:
        case 26:
        case 28:
        case 29:
return Make(Tokens.INTEGER);
            break;
        case 16:
{  }
            break;
        case 17:
return Make(Tokens.LBRACK);
            break;
        case 18:
return Make(Tokens.RBRACK);
            break;
        case 19:
return Make(Tokens.QUASIQUOTE);
            break;
        case 20:
        case 24:
        case 25:
return Make(Tokens.REAL);
            break;
        case 30:
return Make(Tokens.UNQUOTESPLICING);
            break;
        case 31:
return Make(Tokens.VECTORLBRACE);
            break;
        case 32:
        case 35:
        case 36:
        case 37:
return MakeChar();
            break;
        case 33:
return Make(Tokens.LITERAL);
            break;
        case 34:
yy_push_state(ML_COMMENT);
            break;
        case 38:
return Make(Tokens.STRING);
            break;
        case 39:
;
            break;
        case 40:
;
            break;
        case 41:
yy_pop_state();
            break;
        default:
            break;
    }
#endregion
#pragma warning restore 162
                    }
                }
        }

#if BACKUP
        Result Recurse2(Context ctx, int next)
        {
            // Assert: at entry "state" is an accept state AND
            //         NextState(state, chr) != currentStart AND
            //         NextState(state, chr) is not an accept state.
            //
            bool inAccept;
            SaveStateAndPos(ctx);
            state = next;
            if (state == eofNum) return Result.accept;
            GetChr();
            inAccept = false;

            while ((next = NextState()) != currentStart)
            {
                if (inAccept && next > maxAccept) // need to prepare backup data
                    SaveStateAndPos(ctx);
                state = next;
                if (state == eofNum) return Result.accept;
                GetChr(); 
                inAccept = (state <= maxAccept);
            }
            if (inAccept) return Result.accept; else return Result.noMatch;
        }

        void SaveStateAndPos(Context ctx)
        {
            ctx.bPos  = buffer.Pos;
            ctx.cPos  = cPos;
            ctx.state = state;
            ctx.cChr = chr;
        }

        void RestoreStateAndPos(Context ctx)
        {
            buffer.Pos = ctx.bPos;
            cPos = ctx.cPos;
            state = ctx.state;
            chr = ctx.cChr;
        }

        void RestorePos(Context ctx) { buffer.Pos = ctx.bPos; cPos = ctx.cPos; }
#endif // BACKUP

        // ============= End of the tokenizer code ================

#if STACK        
        internal void yy_clear_stack() { scStack.Clear(); }
        internal int yy_top_state() { return scStack.Peek(); }
        
        internal void yy_push_state(int state)
        {
            scStack.Push(currentScOrd);
            BEGIN(state);
        }
        
        internal void yy_pop_state()
        {
            // Protect against input errors that pop too far ...
            if (scStack.Count > 0) {
				int newSc = scStack.Pop();
				BEGIN(newSc);
            } // Otherwise leave stack unchanged.
        }
 #endif // STACK

        internal void ECHO() { Console.Out.Write(yytext); }
        
    } // end class Scanner
} // end namespace