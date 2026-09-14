using System;
using System.Collections.Generic;
using System.Text;

namespace SafeStringGenerator.Editor.Generation
{
    internal static class IdentifierUtility
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "add", "alias", "and", "args", "as", "ascending", "async", "await",
            "base", "bool", "break", "by", "byte", "case", "catch", "char", "checked", "class",
            "const", "continue", "decimal", "default", "delegate", "descending", "do", "double",
            "dynamic", "else", "enum", "equals", "event", "explicit", "extern", "false", "file",
            "finally", "fixed", "float", "for", "foreach", "from", "get", "global", "goto", "group",
            "if", "implicit", "in", "init", "int", "interface", "internal", "into", "is", "join",
            "let", "lock", "long", "managed", "nameof", "namespace", "new", "nint", "not", "notnull",
            "nuint", "null", "object", "on", "operator", "or", "orderby", "out", "override", "params",
            "partial", "private", "protected", "public", "readonly", "record", "ref", "remove", "required",
            "return", "sbyte", "scoped", "sealed", "select", "set", "short", "sizeof", "stackalloc",
            "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint",
            "ulong", "unchecked", "unmanaged", "unsafe", "ushort", "using", "value", "var", "virtual",
            "void", "volatile", "when", "where", "while", "with", "yield"
        };

        internal static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "Unnamed";
            }

            StringBuilder builder = new StringBuilder(value.Length + 1);
            bool previousWasSeparator = false;

            for (int i = 0; i < value.Length; i++)
            {
                char character = value[i];
                bool isAsciiLetter = character >= 'A' && character <= 'Z' || character >= 'a' && character <= 'z';
                bool isDigit = character >= '0' && character <= '9';

                if (isAsciiLetter || isDigit || character == '_')
                {
                    builder.Append(character);
                    previousWasSeparator = false;
                }
                else if (!previousWasSeparator)
                {
                    builder.Append('_');
                    previousWasSeparator = true;
                }
            }

            string identifier = builder.ToString();
            if (identifier.Trim('_').Length == 0)
            {
                identifier = "Unnamed";
            }

            if (identifier[0] >= '0' && identifier[0] <= '9' || Keywords.Contains(identifier))
            {
                identifier = "_" + identifier;
            }

            return identifier;
        }

        internal static IReadOnlyDictionary<string, string> Allocate(
            IEnumerable<string> values,
            IEnumerable<string> reservedIdentifiers = null)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            HashSet<string> used = new HashSet<string>(StringComparer.Ordinal);
            if (reservedIdentifiers != null)
            {
                foreach (string reserved in reservedIdentifiers)
                {
                    if (!string.IsNullOrEmpty(reserved))
                    {
                        used.Add(reserved);
                    }
                }
            }

            SortedSet<string> orderedValues = new SortedSet<string>(StringComparer.Ordinal);
            foreach (string value in values)
            {
                orderedValues.Add(value ?? string.Empty);
            }

            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string value in orderedValues)
            {
                string baseIdentifier = Sanitize(value);
                string identifier = baseIdentifier;
                int suffix = 2;

                while (!used.Add(identifier))
                {
                    identifier = baseIdentifier + "_" + suffix;
                    suffix++;
                }

                result.Add(value, identifier);
            }

            return result;
        }
    }
}
