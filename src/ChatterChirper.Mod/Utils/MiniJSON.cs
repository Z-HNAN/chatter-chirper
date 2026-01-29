/*
 * Copyright (c) 2013 Calvin Rien
 *
 * Based on the JSON parser by Patrick van Bergen
 * http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-a-JSON-parser-in-C%23.html
 *
 * Simplified for use in ChatterChirper
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatterChirper.Utils
{
    public static class MiniJSON
    {
        public static object Deserialize(string json)
        {
            if (json == null) return null;
            return Parser.Parse(json);
        }

        sealed class Parser : IDisposable
        {
            const string WORD_BREAK = "{}[],:\"";
            public static bool IsWordBreak(char c) { return char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1; }

            StringReader json;
            Parser(string jsonString) { json = new StringReader(jsonString); }
            public void Dispose() { json.Dispose(); json = null; }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString)) { return instance.ParseValue(); }
            }

            object ParseValue()
            {
                EatWhitespace();
                char nextChar = PeekChar();
                if (nextChar == '"') return ParseString();
                if (nextChar == '[') return ParseArray();
                if (nextChar == '{') return ParseObject();
                if (char.IsDigit(nextChar) || nextChar == '-' || nextChar == 'b' || nextChar == 'f' || nextChar == 'n' || nextChar == 't') return ParseNumberOrKeyword(); // simplified
                return null;
            }

            object ParseObject()
            {
                Dictionary<string, object> table = new Dictionary<string, object>();
                json.Read(); // Consume {
                while (true)
                {
                    switch (NextToken())
                    {
                        case TOKEN.NONE: return null;
                        case TOKEN.CURLY_CLOSE: return table;
                        default:
                            string name = ParseString();
                            if (name == null) return null;
                            if (NextToken() != TOKEN.COLON) return null;
                            json.Read(); // discard :
                            table[name] = ParseValue();
                            break;
                    }
                }
            }

            List<object> ParseArray()
            {
                List<object> array = new List<object>();
                json.Read(); // Consume [
                var parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = NextToken();
                    switch (nextToken)
                    {
                        case TOKEN.NONE: return null;
                        case TOKEN.SQUARED_CLOSE: parsing = false; break;
                        default:
                            object value = ParseByToken(nextToken);
                            array.Add(value);
                            break;
                    }
                }
                return array;
            }

            object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING: return ParseString();
                    case TOKEN.NUMBER: return ParseNumberOrKeyword();
                    case TOKEN.CURLY_OPEN: return ParseObject();
                    case TOKEN.SQUARED_OPEN: return ParseArray();
                    case TOKEN.TRUE: return true;
                    case TOKEN.FALSE: return false;
                    case TOKEN.NULL: return null;
                    default: return null;
                }
            }

            string ParseString()
            {
                StringBuilder s = new StringBuilder();
                json.Read(); // consume opening "
                bool parsing = true;
                while (parsing)
                {
                    if (json.Peek() == -1) break;
                    char c = NextChar();
                    if (c == '"') parsing = false;
                    else if (c == '\\')
                    {
                        if (json.Peek() == -1) break;
                        char next = NextChar();
                        switch (next)
                        {
                            case '"': s.Append('"'); break;
                            case '\\': s.Append('\\'); break;
                            case '/': s.Append('/'); break;
                            case 'b': s.Append('\b'); break;
                            case 'f': s.Append('\f'); break;
                            case 'n': s.Append('\n'); break;
                            case 'r': s.Append('\r'); break;
                            case 't': s.Append('\t'); break;
                            case 'u':
                                char[] hex = new char[4];
                                for (int i = 0; i < 4; i++) hex[i] = NextChar();
                                s.Append((char)Convert.ToInt32(new string(hex), 16));
                                break;
                        }
                    }
                    else s.Append(c);
                }
                return s.ToString();
            }

            object ParseNumberOrKeyword()
            {
                // Quick hack for numbers/bools
                char next = PeekChar();
                if(next == '"') return ParseString();
                // Accumulate until break
                StringBuilder sb = new StringBuilder();
                while(true) {
                    char c = PeekChar();
                    if(IsWordBreak(c)) break;
                    sb.Append(NextChar());
                    if(json.Peek() == -1) break;
                }
                string word = sb.ToString();
                if(word == "true") return true;
                if(word == "false") return false;
                if(word == "null") return null;
                if(double.TryParse(word, out double d)) return d;
                return word;
            }
            
            enum TOKEN { NONE, CURLY_OPEN, CURLY_CLOSE, SQUARED_OPEN, SQUARED_CLOSE, COLON, COMMA, STRING, NUMBER, TRUE, FALSE, NULL };

            TOKEN NextToken()
            {
                EatWhitespace();
                if (json.Peek() == -1) return TOKEN.NONE;
                char c = PeekChar();
                switch (c)
                {
                    case '{': return TOKEN.CURLY_OPEN;
                    case '}': json.Read(); return TOKEN.CURLY_CLOSE;
                    case '[': return TOKEN.SQUARED_OPEN;
                    case ']': json.Read(); return TOKEN.SQUARED_CLOSE;
                    case ',': json.Read(); return NextToken(); 
                    case '"': return TOKEN.STRING;
                    case ':': return TOKEN.COLON;
                    case '0': case '1': case '2': case '3': case '4': case '5': case '6': case '7': case '8': case '9': case '-': 
                        return TOKEN.NUMBER;
                    case 't': return TOKEN.TRUE;
                    case 'f': return TOKEN.FALSE;
                    case 'n': return TOKEN.NULL;
                    default: return TOKEN.NONE;
                }
            }

            void EatWhitespace()
            {
                while (char.IsWhiteSpace(PeekChar())) json.Read();
            }

            char PeekChar() { return Convert.ToChar(json.Peek()); }
            char NextChar() { return Convert.ToChar(json.Read()); }
        }
    }
}
