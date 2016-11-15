using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lynicon.Utility
{
    /// <summary>
    /// Extension methods for lexing a string into tokens
    /// </summary>
    public static class Tokeniser
    {
        /// <summary>
        /// Break a string into tokens
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Enumerable of tokens</returns>
        public static IEnumerable<Token> Tokenise(this string s)
        {
            return s.Tokenise(CultureInfo.InvariantCulture, false, null, null);
        }
        /// <summary>
        /// Break a string into tokens
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="quoteCharacter">Quote character found around quoted literal token</param>
        /// <returns>Enumerable of tokens</returns>
        public static IEnumerable<Token> Tokenise(this string s, char quoteCharacter)
        {
            return s.Tokenise(CultureInfo.InvariantCulture, false, quoteCharacter, null);
        }
        /// <summary>
        /// Break a string into tokens
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="excludedSymbols">List of characters which are not to be considered symbols</param>
        /// <returns>Enumerable of tokens</returns>
        public static IEnumerable<Token> Tokenise(this string s, List<char> excludedSymbols)
        {
            return s.Tokenise(CultureInfo.InvariantCulture, false, null, excludedSymbols);
        }
        /// <summary>
        /// Break a string into tokens
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="culture">The Culture to use for this</param>
        /// <returns>Enumerable of tokens</returns>
        public static IEnumerable<Token> Tokenise(this string s, CultureInfo culture)
        {
            return s.Tokenise(culture, false, null, null);
        }
        /// <summary>
        /// Break a string into tokens
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="culture">The Culture to use for this</param>
        /// <param name="symbolIsOneChar">Whether to consider every symbol character a separate symbol token</param>
        /// <param name="quoteCharacter">Quote character found around quoted literal token</param>
        /// <param name="excludedSymbols">List of characters which are not to be considered symbols</param>
        /// <returns>Enumerable of tokens</returns>
        public static IEnumerable<Token> Tokenise(this string s, CultureInfo culture, bool symbolIsOneChar, char? quoteCharacter, List<char> excludedSymbols)
        {
            int pos = 0;
            Token token = null;
            Token lastToken = null;
            bool hasWhitespaceBefore = false;
            string decimalPoint = culture.NumberFormat.NumberDecimalSeparator;
            if (excludedSymbols == null)
                excludedSymbols = new List<char>();

            while (pos < s.Length)
            {
                hasWhitespaceBefore = false;
                while (pos < s.Length && char.IsWhiteSpace(s[pos]))
                {
                    pos++;
                    hasWhitespaceBefore = true;
                }
                    
                if (pos >= s.Length)
                    yield break;
                if (quoteCharacter.HasValue && s[pos] == quoteCharacter)
                    token = new QuotedLiteralToken(pos + 1, pos + 1);
                else if (char.IsNumber(s[pos]))
                    token = new NumberToken(pos, pos + 1);
                else if (IsSymbolChar(s[pos], excludedSymbols))
                    token = new SymbolToken(pos, pos + 1);
                else
                    token = new WordToken(pos, pos + 1);
                pos++;
                bool hasDP = false;
                while (pos < s.Length && (token is QuotedLiteralToken || !char.IsWhiteSpace(s[pos])))
                {
                    if ((token is NumberToken && char.IsNumber(s[pos]))
                        || (token is SymbolToken && IsSymbolChar(s[pos], excludedSymbols) && !symbolIsOneChar)
                        || (token is WordToken && !char.IsNumber(s[pos]) && !IsSymbolChar(s[pos], excludedSymbols) && !char.IsWhiteSpace(s[pos]))
                        || (token is QuotedLiteralToken && s[pos] != quoteCharacter))
                        token.ExtendRight();
                    else if (token is NumberToken
                        && s[pos] == decimalPoint[0]
                        && pos + 1 < s.Length
                        && char.IsNumber(s[pos+1])
                        && !hasDP)
                    {
                        hasDP = true;
                        token.ExtendRight();
                    }
                    else
                        break;

                    pos++;
                }

                token.Previous = lastToken;
                token.SetValue(s);
                token.HasWhitespaceBefore = hasWhitespaceBefore;
                if (token is QuotedLiteralToken)    // skip final quote
                    pos++;
                yield return token;
                lastToken = token;
            }
        }

        /// <summary>
        /// Whether a character is a symbol or punctuation, taking into account a list of characters to consider
        /// not to be symbols
        /// </summary>
        /// <param name="c">The character to test</param>
        /// <param name="excludedSymbols">Characters not to consider to be symbols</param>
        /// <returns>Whether the character is a symbol</returns>
        public static bool IsSymbolChar(char c, List<char> excludedSymbols)
        {
            return (char.IsPunctuation(c) || char.IsSymbol(c)) && !excludedSymbols.Contains(c);
        }

        /// <summary>
        /// Test whether a list of tokens matches a string which lists the Codes of the successive tokens
        /// </summary>
        /// <param name="tokens">The list of tokens</param>
        /// <param name="pattern">The pattern</param>
        /// <returns>True if matches</returns>
        public static bool MatchPattern(this List<Token> tokens, string pattern)
        {
            int pos = 0;
            int tPos = 0;
            while (pos >= 0 && pos < pattern.Length)
            {
                if (tPos >= tokens.Count) return false;
                if (pattern[pos] == '"')
                {
                    string val = pattern.GetHead(ref pos, "\"");
                    if (tokens[tPos].ToString() != val) return false;
                    tPos++;
                    continue;
                }
                else if (pattern[pos] != tokens[tPos].Code && (new string(pattern[pos], 1)) != tokens[tPos].ToString())
                    return false;

                pos++;
                tPos++;
            }

            return true;
        }

        /// <summary>
        /// Rejoin a list of tokens into the original string (although spaces will be normalised)
        /// </summary>
        /// <param name="tokens">List of tokens</param>
        /// <returns>string version of the list</returns>
        public static string Rejoin(this IEnumerable<Token> tokens)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tok in tokens)
            {
                if (tok.HasWhitespaceBefore)
                    sb.Append(" ");
                sb.Append(tok.ToString());
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Base class for a token
    /// </summary>
    public abstract class Token
    {
        protected int fromPos;
        protected int toPos;
        protected string innerString;

        /// <summary>
        /// The previously parsed token
        /// </summary>
        public Token Previous { get; set; }

        /// <summary>
        /// A character which represents this type of token in a token list match string
        /// </summary>
        public abstract char Code { get; }

        /// <summary>
        /// Create a new Token with the region of the underlying string of which it consists
        /// </summary>
        /// <param name="fromPos">Start position</param>
        /// <param name="toPos">End position</param>
        public Token(int fromPos, int toPos)
        {
            this.fromPos = fromPos;
            this.toPos = toPos;
        }

        /// <summary>
        /// Extend the covered region to the right by one character
        /// </summary>
        public void ExtendRight()
        {
            toPos++;
        }

        bool hasWhitespaceBefore = false;
        /// <summary>
        /// Whether there is whitespace before this token in the original string
        /// </summary>
        public bool HasWhitespaceBefore
        {
            get { return hasWhitespaceBefore; }
            set { hasWhitespaceBefore = value; }
        }

        /// <summary>
        /// Get the part of the underlying string before this token
        /// </summary>
        /// <param name="s">The underlying string</param>
        /// <returns>Part of string before token</returns>
        public string BeforePart(string s)
        {
            return s.Substring(0, fromPos);
        }

        /// <summary>
        /// Get the part of the underlying string after this token
        /// </summary>
        /// <param name="s">The underlying string</param>
        /// <returns>Part of string after token</returns>
        public string AfterPart(string s)
        {
            return s.Substring(toPos);
        }

        /// <summary>
        /// Set the value of the token from the underlying string
        /// </summary>
        /// <param name="s">The underlying string</param>
        public abstract void SetValue(string s);

        /// <summary>
        /// The part of the underlying string which this token covers
        /// </summary>
        /// <param name="s">The underlying string</param>
        /// <returns>The part covered by this token</returns>
        public string SourceString(string s)
        {
            return s.Substring(fromPos, toPos - fromPos);
        }
    }

    /// <summary>
    /// A token representing a number (decimal or integer)
    /// </summary>
    public class NumberToken : Token
    {
        /// <summary>
        /// Create a number token setting the range of the underlying string it covers
        /// </summary>
        /// <param name="fromPos">Start position</param>
        /// <param name="toPos">End position</param>
        public NumberToken(int fromPos, int toPos)
            : base(fromPos, toPos)
        { }

        /// <summary>
        /// The numeric value of the token
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// The code for a NumberToken
        /// </summary>
        public override char Code
        {
            get { return 'N'; }
        }

        /// <summary>
        /// Set the value of the token from the underlying string
        /// </summary>
        /// <param name="s">The underlying string</param>
        public override void SetValue(string s)
        {
            string sVal = SourceString(s);
            // deal with evil fraction characters
            sVal = sVal.Replace("½", ".5").Replace("¼", ".25").Replace("¾", ".75").Replace(" ", "");
            Value = decimal.Parse(sVal);
        }

        /// <summary>
        /// The value of the token as a string
        /// </summary>
        /// <returns>Value of token as string</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A token representing a word
    /// </summary>
    public class WordToken : Token
    {
        /// <summary>
        /// Create a word token setting the range of the underlying token it covers
        /// </summary>
        /// <param name="fromPos">The start position of the word token</param>
        /// <param name="toPos">The end position of the word token</param>
        public WordToken(int fromPos, int toPos)
            : base(fromPos, toPos)
        { }

        /// <summary>
        /// The value of the word token
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The code for a word token
        /// </summary>
        public override char Code
        {
            get { return 'W'; }
        }

        /// <summary>
        /// Set the value of a word token from the underlying string
        /// </summary>
        /// <param name="s">The underlying string</param>
        public override void SetValue(string s)
        {
            Value = SourceString(s);
        }

        /// <summary>
        /// The value as a string
        /// </summary>
        /// <returns>The value as a string</returns>
        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// Token representing symbols/punctuation
    /// </summary>
    public class SymbolToken : Token
    {
        /// <summary>
        /// Create a symbol token setting the range of the underlying token it covers
        /// </summary>
        /// <param name="fromPos">The start position of the word token</param>
        /// <param name="toPos">The end position of the word token</param>
        public SymbolToken(int fromPos, int toPos)
            : base(fromPos, toPos)
        { }

        /// <summary>
        /// The value of the token
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The code for a SymbolToken
        /// </summary>
        public override char Code
        {
            get { return 'S'; }
        }

        /// <summary>
        /// Set the value from the underlying string
        /// </summary>
        /// <param name="s">The underlying string</param>
        public override void SetValue(string s)
        {
            Value = SourceString(s);
        }

        /// <summary>
        /// The value as a string
        /// </summary>
        /// <returns>The value as a string</returns>
        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>
    /// A token representing a quoted string
    /// </summary>
    public class QuotedLiteralToken : WordToken
    {
        public QuotedLiteralToken(int fromPos, int toPos)
            : base(fromPos, toPos)
        { }

        public override char Code
        {
            get
            {
                return 'Q';
            }
        }
    }

    //public class StartTagToken : Token, ISyntaxTreeNode
    //{
    //    public StartTagToken(int fromPos, int toPos)
    //        : base(fromPos, toPos)
    //    { }

    //    public string Value { get; set; }

    //    private Dictionary<string, string> Attributes = new Dictionary<string, string>();

    //    public override void SetValue(string s)
    //    {
    //        string tagContent = s.Substring(fromPos + 1, toPos - fromPos - 2);
    //        string[] words = tagContent.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //        Value = words[0];
    //        if (words.Length > 1)
    //            Attributes = words
    //                .Skip(1)
    //                .Select(w => w.Tokenise(CultureInfo.InvariantCulture, true).ToArray())
    //                .Where(ts => ts.Length >= 5
    //                             && ts[0] is WordToken
    //                             && ts[1].ToString() == "="
    //                             && ts[2].ToString() == "\""
    //                             && ts[ts.Length - 1].ToString() == "\"")
    //                .ToDictionary(ts => ts[0].ToString(),
    //                              ts => string.Concat(ts.Skip(3).Take(ts.Length - 4).ToString()));
    //    }
    //}
}
