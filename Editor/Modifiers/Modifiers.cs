using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SafeStringGenerator
{
    public class Modifiers
    {
        private HashSet<string> _usedNames = new HashSet<string>();

        private static readonly HashSet<string> CSharpKeywords = new HashSet<string>
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock",
            "long", "namespace", "new", "null", "object", "operator", "out", "override", "params",
            "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short",
            "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true",
            "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while"
        };

        public void Reset()
        {
            _usedNames.Clear();
        }

        public string GetSanitizedName(string originalName)
        {
            string safeName = Regex.Replace(originalName, @"[^a-zA-Z0-9_]", "");

            if (string.IsNullOrEmpty(safeName)) safeName = "Unknown";

            if (Regex.IsMatch(safeName, @"^\d") || CSharpKeywords.Contains(safeName))
            {
                safeName = "_" + safeName;
            }

            return safeName;
        }

        public string GetSafeNameWithDeduplication(string originalName)
        {
            string safeName = GetSanitizedName(originalName);

            string finalName = safeName;
            int counter = 1;
            while (_usedNames.Contains(finalName))
            {
                finalName = safeName + "_" + counter;
                counter++;
            }

            _usedNames.Add(finalName);
            return finalName;
        }
    }
}
