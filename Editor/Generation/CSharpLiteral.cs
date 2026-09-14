using System;
using System.Globalization;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class CSharpLiteral
    {
        internal static string Quote(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            StringBuilder builder = new StringBuilder(value.Length + 2);
            builder.Append('"');

            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                switch (character)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\0': builder.Append("\\0"); break;
                    case '\a': builder.Append("\\a"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\r': builder.Append("\\r"); break;
                    case '\t': builder.Append("\\t"); break;
                    case '\v': builder.Append("\\v"); break;
                    default:
                        if (char.IsControl(character) || char.IsSurrogate(character) ||
                            character == '\u2028' || character == '\u2029')
                        {
                            builder.Append("\\u");
                            builder.Append(((int)character).ToString("X4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            builder.Append(character);
                        }
                        break;
                }
            }

            builder.Append('"');
            return builder.ToString();
        }
    }
}
