using System;
using System.Collections.Generic;
using System.Text;

namespace Korzh.NLP
{
    /// <summary>
    /// This class allows an application to break a string into tokens.
    /// </summary>
    public class TextTokenizer {

        private string _source;
        private TextFormats _formats;
        private ITextFilterMapper _filterMapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextTokenizer"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public TextTokenizer(string source, TextFormats formats = null, ITextFilterMapper filterMapper = null) {
            _source = source;
            _formats = formats ?? TextFormats.PlainText;
            _filterMapper = filterMapper;
        }

        private TokenType _tokenType;

        /// <summary>
        /// Gets the current token type.
        /// </summary>
        /// <value>
        /// A TokenType enum value
        /// </value>
        public TokenType TokType {
            get { return _tokenType; }
        }


        private TokenType _lastBreak;

        /// <summary>
        /// Returns the type of the last scan stop. It can be either space or separator.
        /// </summary>
        public TokenType LastBreak {
            get { return _lastBreak; }
        }

        private int _position = 0;

        /// <summary>
        /// Returns current position in the scanned string.
        /// </summary>
        public int Position => _position; 

        private int _tokenStart = 0;

        /// <summary>
        /// Returns start position of the current token.
        /// </summary>
        public int TokenStart {
            get { return _tokenStart; }

        }

        private bool IsSpace(char c) {
            return _formats.Spaces.IndexOf(c) >= 0;
        }

        private void SkipSpaces() {
            while (_position < _source.Length && IsSpace(_source[_position]))
                _position++;
        }

        private void SkipSeparators() {
            while (_position < _source.Length && IsSeparator(_source[_position]))
                _position++;
        }

        private bool IsSeparator(char c) {
            return _formats.Separators.IndexOf(c) >= 0;
        }

        private bool IsCQuote(char c) {
            return c == '"';
        }


        private bool IsPascalQuote(char c) {
            return c == '\'';
        }

        private bool IsAQuote1(char c) {
            return c == '[';
        }

        private bool IsAQuote2(char c) {
            return c == ']';
        }

        /// <summary>
        /// Get first token.
        /// </summary>
        /// <returns></returns>
        public string FirstToken() {
            _position = 0;
            return NextToken();
        }

        /// <summary>
        /// Returns next token.
        /// </summary>
        /// <returns></returns>
        public string NextToken() {
            string result = null;
            bool accepted = false;
            while (!accepted) {
                SkipSpaces();
                if (_position < _source.Length) {
                    int start = _position;
                    if (_formats.HasCStrings && IsCQuote(_source[_position])) {
                        result = RunCString();
                        _tokenType = TokenType.CString;
                        _lastBreak = TokenType.Separator;
                    }
                    else if (_formats.HasPascalStrings && IsPascalQuote(_source[_position])) {
                        result = RunPascalString();
                        _tokenType = TokenType.PascalString;
                        _lastBreak = TokenType.Separator;
                    }
                    else if (_formats.HasIdentsInSquareBrackets && IsAQuote1(_source[_position])) {
                        result = RunAString();
                        _tokenType = TokenType.AIdent;
                        _lastBreak = TokenType.Separator;
                    }
                    else {
                        if (IsSeparator(_source[_position])) {
                            _position++;
                            _tokenType = TokenType.Separator;
                            result = _source.Substring(start, _position - start);
                            _lastBreak = TokenType.Separator;
                        }
                        else {
                            while (_position < _source.Length) {
                                if (IsSpace(_source[_position]) ||
                                    IsSeparator(_source[_position]) ||
                                    IsCQuote(_source[_position]))
                                    break;
                                _position++;
                            }
                            result = _source.Substring(start, _position - start);
                            _tokenType = TokenType.Identifier;
                            if (_position < _source.Length)
                                _lastBreak = IsSpace(_source[_position]) ? TokenType.Space : TokenType.Separator;
                            else
                                _lastBreak = TokenType.Space;
                        }
                    }
                    _tokenStart = start;
                }
                else
                    return result;

                if (_filterMapper != null) {
                    accepted = _filterMapper.Filter(result);
                    if (accepted) {
                        result = _filterMapper.Map(result);
                    }
                }
                else
                    accepted = true;
            }

            return result;
        }


        public IList<string> ToList() {
            var result = new List<string>();
            string token = FirstToken();
            while (token != null) {
                result.Add(token);
                token = NextToken();
            }
            return result;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            string token = FirstToken();
            while (token != null) {
                builder.Append(token);
                builder.Append(" ");
                token = NextToken();
            }

            return builder.ToString();
        }


        /// <summary>
        /// Gets the full token string including quotes, brackets, etc.
        /// </summary>
        /// <value>The full token.</value>
        public string FullToken {
            get { return _source.Substring(_tokenStart, _position - _tokenStart); }
        }

        /// <summary>
        /// Represents the types of tokens.  Used in <see cref="TextTokenizer"/> class
        /// </summary>
        public enum TokenType {
            /// <summary>
            /// Space. It's not actually the type of a token but it's used for LastBreak property of <see cref="TextTokenizer"/> class
            /// </summary>
            Space,

            /// <summary>
            /// Separator
            /// </summary>
            Separator,

            /// <summary>
            /// Word
            /// </summary>
            Identifier,

            /// <summary>
            /// C-style string (some text placed in double quotes)
            /// </summary>
            CString,

            /// <summary>
            /// Pascal-style string (some text placed in single quotes)
            /// </summary>
            PascalString,

            /// <summary>
            /// Access SQL identifier (some text placed in [ ] brackets)
            /// </summary>
            AIdent
        }

        private string RunCString() {
            StringBuilder result = new StringBuilder(100);

            if (IsCQuote(_source[_position])) {
                _position++;
                while (_position < _source.Length) {
                    if (IsCQuote(_source[_position])) {
                        if (_position < _source.Length - 1 && IsCQuote(_source[_position + 1])) {
                            result.Append(_source[_position]);
                            _position += 2;
                            continue;
                        }
                        else {
                            _position++;
                            break;
                        }
                    }
                    result.Append(_source[_position]);
                    _position++;
                }
            }
            return result.ToString();
        }

        private string RunPascalString() {
            StringBuilder result = new StringBuilder(100);

            if (IsPascalQuote(_source[_position])) {
                _position++;
                while (_position < _source.Length) {
                    if (IsPascalQuote(_source[_position])) {
                        if (_position < _source.Length - 1 && IsPascalQuote(_source[_position + 1])) {
                            result.Append(_source[_position]);
                            _position += 2;
                            continue;
                        }
                        else {
                            _position++;
                            break;
                        }
                    }
                    result.Append(_source[_position]);
                    _position++;
                }
            }
            return result.ToString();
        }

        private string RunAString() {
            StringBuilder result = new StringBuilder(100);

            if (IsAQuote1(_source[_position])) {
                _position++;
                while (_position < _source.Length) {
                    if (IsAQuote2(_source[_position])) {
                        _position++;
                        break;
                    }
                    result.Append(_source[_position]);
                    _position++;
                }
            }
            return result.ToString();
        }

    }

    public class TextFormats {
        public const string DefaultSpaces = " \t\n\r";

        public const string DefaultSeparators = "()[]{}<>.,;:=@+-*/\"\'\\`~#%^";

        public string Spaces { get; set; } = DefaultSpaces;

        public string Separators { get; set; } = DefaultSeparators;

        public bool HasIdentsInSquareBrackets { get; set; } = false;

        public bool HasPascalStrings { get; set; } = false;

        public bool HasCStrings { get; set; } = true;

        private static TextFormats _sql = null;

        public static TextFormats SQL {
            get {
                if (_sql == null) {
                    _sql = new TextFormats {
                        HasIdentsInSquareBrackets = true,
                        HasPascalStrings = true,
                        HasCStrings = true
                    };
                }
                return _sql;
            }
        }

        private static TextFormats _ccode = null;

        public static TextFormats CCode {
            get {
                if (_ccode == null) {
                    _ccode = new TextFormats {
                        HasIdentsInSquareBrackets = false,
                        HasPascalStrings = true,
                        HasCStrings = true
                    };
                }
                return _ccode;
            }
        }

        private static TextFormats _plainText = null;

        public static TextFormats PlainText {
            get {
                if (_plainText == null) {
                    _plainText = new TextFormats {
                        HasIdentsInSquareBrackets = false,
                        HasPascalStrings = false,
                        HasCStrings = false,
                    };
                }
                return _plainText;
            }
        }

        private static TextFormats _wordsOnly = null;

        public static TextFormats WordsOnly {
            get {
                if (_wordsOnly == null) {
                    _wordsOnly = new TextFormats {
                        Spaces = DefaultSpaces + DefaultSeparators + "!?",
                        HasIdentsInSquareBrackets = false,
                        HasPascalStrings = false,
                        HasCStrings = false,
                    };
                }
                return _wordsOnly;
            }
        }
    }

    public interface ITextFilterMapper {
        Func<string, bool> Filter { get; }
            
        Func<string, string> Map { get; }
    }
}
