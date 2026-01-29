// Minimal JSON parser (Unity-style MiniJSON) for .NET 3.5
// Source adapted from public-domain/MIT implementations
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

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

            public static object Parse(string json)
            {
                using (var instance = new Parser(json))
                {
                    return instance.ParseValue();
                }
            }

            StringReader json;

            Parser(string jsonString)
            {
                json = new StringReader(jsonString);
            }

            public void Dispose()
            {
                json.Dispose();
                json = null;
            }

            enum TOKEN { NONE, CURLY_OPEN, CURLY_CLOSE, SQUARE_OPEN, SQUARE_CLOSE, COLON, COMMA, STRING, NUMBER, TRUE, FALSE, NULL }

            object ParseValue()
            {
                TOKEN nextToken = NextToken;
                switch (nextToken)
                {
                    case TOKEN.STRING:
                        return ParseString();
                    case TOKEN.NUMBER:
                        return ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return ParseObject();
                    case TOKEN.SQUARE_OPEN:
                        return ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }

            Dictionary<string, object> ParseObject()
            {
                var table = new Dictionary<string, object>();
                json.Read(); // {

                while (true)
                {
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            // key
                            string name = ParseString();
                            if (NextToken != TOKEN.COLON) return null;
                            // val
                            table[name] = ParseValue();
                            break;
                    }

                    switch (NextToken)
                    {
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            return table;
                    }
                }
            }

            List<object> ParseArray()
            {
                var array = new List<object>();
                json.Read(); // [

                var parsing = true;
                while (parsing)
                {
                    TOKEN nextToken = NextToken;
                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.SQUARE_CLOSE:
                            return array;
                        default:
                            array.Add(ParseValue());
                            break;
                    }

                    switch (NextToken)
                    {
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARE_CLOSE:
                            parsing = false;
                            break;
                        default:
                            parsing = false;
                            break;
                    }
                }

                return array;
            }

            string ParseString()
            {
                var s = new StringBuilder();
                char c;
                json.Read(); // "

                bool parsing = true;
                while (parsing)
                {
                    if (json.Peek() == -1) break;
                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (json.Peek() == -1) { parsing = false; break; }
                            c = NextChar;
                            switch (c)
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
                                    var hex = new char[4];
                                    for (int i = 0; i < 4; i++) hex[i] = NextChar;
                                    s.Append((char)Convert.ToInt32(new string(hex), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            object ParseNumber()
            {
                var number = NextWord;
                if (number.IndexOf('.') == -1)
                {
                    long parsedInt;
                    long.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedInt);
                    return parsedInt;
                }

                double parsedDouble;
                double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedDouble);
                return parsedDouble;
            }

            void EatWhitespace()
            {
                while (char.IsWhiteSpace(PeekChar)) json.Read();
            }

            char PeekChar { get { return Convert.ToChar(json.Peek()); } }
            char NextChar { get { return Convert.ToChar(json.Read()); } }

            string NextWord
            {
                get
                {
                    var sb = new StringBuilder();
                    while (" \\r\\n\t".IndexOf(PeekChar) == -1 && WORD_BREAK.IndexOf(PeekChar) == -1)
                    {
                        sb.Append(NextChar);
                        if (json.Peek() == -1) break;
                    }
                    return sb.ToString();
                }
            }

            TOKEN NextToken
            {
                get
                {
                    EatWhitespace();
                    if (json.Peek() == -1) return TOKEN.NONE;

                    char c = PeekChar;
                    switch (c)
                    {
                        case '{': return TOKEN.CURLY_OPEN;
                        case '}': json.Read(); return TOKEN.CURLY_CLOSE;
                        case '[': return TOKEN.SQUARE_OPEN;
                        case ']': json.Read(); return TOKEN.SQUARE_CLOSE;
                        case ',': json.Read(); return TOKEN.COMMA;
                        case '"': return TOKEN.STRING;
                        case ':': json.Read(); return TOKEN.COLON;
                        case '-':
                        case '0': case '1': case '2': case '3': case '4':
                        case '5': case '6': case '7': case '8': case '9':
                            return TOKEN.NUMBER;
                    }

                    var word = NextWord;
                    switch (word)
                    {
                        case "false": return TOKEN.FALSE;
                        case "true": return TOKEN.TRUE;
                        case "null": return TOKEN.NULL;
                    }
                    return TOKEN.NONE;
                }
            }
        }

        sealed class StringReader : IDisposable
        {
            readonly string s;
            int pos;
            public StringReader(string str) { s = str; pos = 0; }
            public int Peek() { return pos >= s.Length ? -1 : s[pos]; }
            public int Read() { return pos >= s.Length ? -1 : s[pos++]; }
            public void Dispose() { }
        }

        public static string Serialize(object obj)
        {
            var builder = new StringBuilder();
            Serializer.SerializeValue(obj, builder);
            return builder.ToString();
        }

        class Serializer
        {
            public static void SerializeValue(object value, StringBuilder builder)
            {
                if (value == null)
                {
                    builder.Append("null");
                }
                else if (value is string)
                {
                    SerializeString((string)value, builder);
                }
                else if (value is bool)
                {
                    builder.Append((bool)value ? "true" : "false");
                }
                else if (value is IList)
                {
                    SerializeArray((IList)value, builder);
                }
                else if (value is IDictionary)
                {
                    SerializeObject((IDictionary)value, builder);
                }
                else if (value is char)
                {
                    SerializeString(new string((char)value, 1), builder);
                }
                else
                {
                    SerializeOther(value, builder);
                }
            }

            static void SerializeObject(IDictionary obj, StringBuilder builder)
            {
                bool first = true;
                builder.Append('{');
                foreach (object e in obj.Keys)
                {
                    if (!first) builder.Append(',');
                    SerializeString(e.ToString(), builder);
                    builder.Append(':');
                    SerializeValue(obj[e], builder);
                    first = false;
                }
                builder.Append('}');
            }

            static void SerializeArray(IList anArray, StringBuilder builder)
            {
                builder.Append('[');
                bool first = true;
                foreach (object obj in anArray)
                {
                    if (!first) builder.Append(',');
                    SerializeValue(obj, builder);
                    first = false;
                }
                builder.Append(']');
            }

            static void SerializeString(string str, StringBuilder builder)
            {
                builder.Append('"');
                foreach (var c in str)
                {
                    switch (c)
                    {
                        case '"': builder.Append("\\\""); break;
                        case '\\': builder.Append("\\\\"); break;
                        case '\b': builder.Append("\\b"); break;
                        case '\f': builder.Append("\\f"); break;
                        case '\n': builder.Append("\\n"); break;
                        case '\r': builder.Append("\\r"); break;
                        case '\t': builder.Append("\\t"); break;
                        default:
                            int codepoint = Convert.ToInt32(c);
                            if ((codepoint >= 32) && (codepoint <= 126)) builder.Append(c);
                            else builder.Append("\\u" + codepoint.ToString("x4"));
                            break;
                    }
                }
                builder.Append('"');
            }

            static void SerializeOther(object value, StringBuilder builder)
            {
                if (value is float || value is double || value is decimal)
                {
                    builder.Append(Convert.ToDouble(value, CultureInfo.InvariantCulture).ToString("R", CultureInfo.InvariantCulture));
                }
                else
                {
                    builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
