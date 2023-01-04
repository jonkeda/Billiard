using System.Text;
using System.Text.RegularExpressions;

namespace Billiards.Base.Extensions
{
    public static class StringExtension
    {
        public static bool NumbersOnly(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }
            foreach (char c in text)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }

            return true;
        }

        public static string SplitWords(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            bool lastIsLower = false;
            foreach (char c in text)
            {
                if (lastIsLower
                    && char.IsUpper(c))
                {
                    sb.Append(' ');
                    
                }

                sb.Append(c);
                lastIsLower = char.IsLower(c);
            }

            return sb.ToString();
        }

        public static string CleanLabel(this string text)
        {
            return text.AsLabel();
        }

        public static string AsLabel(this string text)
        {
            if (text == null)
            {
                return null;
            }

            if (text.EndsWith(" *"))
            {
                text = text.Trim(' ', '*', '%');
            }

            text = text.Replace("(%)", "");
            text = RemoveBrackets(text);

            return text?.Replace("\n", "").Replace("\r", "");
        }

        public static string CleanId(this string text)
        {
            text = RemoveId(text, "ctl00_");
            text = RemoveId(text, "PageContent_");
            return text;
        }

        public static string NameFromId(this string text)
        {
            if (text == null)
            {
                return null;
            }

            int i = text.LastIndexOf("_", StringComparison.Ordinal);
            if (i < 1)
            {
                return text;
            }

            string id = text.Substring(0, i);

            i = id.LastIndexOf("_", StringComparison.Ordinal);
            if (i < 0)
            {
                return text;
            }

            id = id.Substring(i + 1);

            id = id.RemoveId("tb");
            id = id.RemoveId("ddl");
            id = id.RemoveId("btn");

            return id;
        }

        private static string RemoveId(this string text, string id)
        {
            while (text.StartsWith(id))
            {
                text = text.Substring(id.Length);
            }
            return text;
        }

        public static string AsId(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            text = RemoveBrackets(text);


            text = text.TrimEnd().TrimStart();

            bool first = true;
            bool upper = false;

            StringBuilder builder = new StringBuilder();
            foreach (char t in text)
            {
                char c = t.Normalize();
                if (c == '%')
                {
                    builder.Append("Percentage");
                }
                else if (char.IsLetterOrDigit(c))
                {
                    if (first)
                    {
                        first = false;
                        if (char.IsDigit(c))
                        {
                            builder.Append("e");
                        }
                        builder.Append(char.ToLower(c));
                    }
                    else
                    {
                        if (upper)
                        {
                            upper = false;
                            builder.Append(char.ToUpper(c));
                        }
                        else
                        {
                            builder.Append(c);
                        }
                    }
                }
                else
                {
                    upper = true;
                }
            }

            return builder.ToString();
        }

        public static bool IsNumbers(this string text)
        {
            foreach (char c in text)
            {
                if (!char.IsNumber(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static string AsPackageName(this string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return "";
            }

            int i = filename.IndexOf("com");
            if (i > 0)
            {
                filename = filename.Substring(i);

                string packageName = Path.GetDirectoryName(filename).Replace('\\', '.').ToLower();

                return $"{packageName}";
            }

            return filename.Replace('\\', '.').ToLower();
        }

        public static string Left(this string text, int length)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (text.Length > length)
            {
                return text.Substring(0, length);
            }

            return text;
        }

        public static string AsName(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            bool upper = true;
            StringBuilder builder = new StringBuilder();

            text = RemoveBrackets(text);

            bool first = true;
            foreach (char t in text)
            {
                char c = t.Normalize();
                if (c == '%')
                {
                    builder.Append("Percentage");
                }
                else if (char.IsLetterOrDigit(c))
                {
                    if (first)
                    {
                        if (char.IsDigit(c))
                        {
                            builder.Append("e");
                        }
                    }
                    if (upper)
                    {
                        upper = false;
                        builder.Append(char.ToUpper(c));
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                else
                {
                    upper = true;
                }

                first = false;
            }

            string name = builder.ToString();

            string trimmedName = name.Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');

            if (name.Length - trimmedName.Length >= 3)
            {
                return trimmedName;
            }
            return name;
        }

        private static string RemoveBrackets(string text)
        {
            int i = text.LastIndexOf("(");
            if (i > 0)
            {
                int j = text.LastIndexOf(")");
                if (j > 0)
                {
                    string part = text.Substring(i + 1, (j - i) - 1);
                    if (part.IsNumbers())
                    {
                        text = text.Substring(0, i);
                    }
                }
                else
                {
                    text = text.Substring(0, i);
                }
            }

            return text;
        }

        public static string Escape(this string text)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    sb.Append("\\n");
                }
                else if (c == '\r')
                {
                    //sb.Append("\\r");
                }
                else if (c == '\t')
                {
                    sb.Append("\\t");
                }
                else if (c == '"')
                {
                    sb.Append("\\\"");
                }
                else if (c == '\'')
                {
                    sb.Append("\\'");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string UpperAndUnderline(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            bool underline = false;
            StringBuilder builder = new StringBuilder();

            //int i = text.LastIndexOf("(");
            //if (i > 0)
            //{
            //    text = text.Substring(0, i);
            //}

            bool first = true;
            bool lastIsUpper = false;
            foreach (char t in text)
            {
                char c = t.Normalize();
                if (c == '%')
                {
                    builder.Append("_PERCENTAGE");
                    lastIsUpper = true;
                }
                else if (char.IsLetterOrDigit(c))
                {
                    if (first)
                    {
                        if (char.IsDigit(c))
                        {
                            builder.Append("E");
                        }
                    }

                    if (builder.Length > 0
                        && char.IsUpper(c))
                    {
                        if (!lastIsUpper)
                        {
                            builder.Append('_');
                            underline = false;
                        }
                    }
                    if (underline)
                    {
                        underline = false;
                        builder.Append('_');
                    }
                    builder.Append(char.ToUpper(c));
                    lastIsUpper = char.IsUpper(c);
                }
                else
                {
                    underline = true;
                    lastIsUpper = false;
                }

                first = false;
            }

            return builder.ToString().TrimStart('_');
        }

        public static char Normalize(this char aChar)
        {
            string text = ""; 
            text += aChar;
            string output = text.Normalize(NormalizationForm.FormD);
            return output[0];
        }

        public static string AsPath(this string text)
        {
            return text?.Replace(".", "\\");
        }

        public static string AsUnixPath(this string text)
        {
            return text?.Replace(".", "/");
        }

        public static string PathToUnixPath(this string text)
        {
            return text?.Replace("\\", "/");
        }

        public static string AsCode(this string text)
        {
            return text?.Replace("\\", ".");
        }

        public static string AsPathLabel(this string text)
        {
            return text?.Replace("\\", " ").Replace(".", " ");
        }


        public static bool UrlEquals(this string url1, string url2)
        {
            if (url1 == null
                && url2 == null)
            {
                return true;
            }
            if (url1 == null)
            {
                return false;
            }
            if (url2 == null)
            {
                return false;
            }

            return url1.CleanUrl().ToLower() == url2.CleanUrl().ToLower();
        }

        public static string CleanUrl(this string url)
        {
            if (url == null)
            {
                return null;
            }

            int i = url.IndexOf('?');
            if (i < 0)
            {
                return url;
            }
            return url.Substring(0, i);
        }

        public static string AbsolutePath(this string url)
        {
            try
            {
                Uri uri = new Uri(url);

                return uri.AbsolutePath;
            }
            catch (Exception)
            {
                return url;
            }
        }

        public static string CleanXml(this string text)
        {
            text = text.Replace("&lt;", "<");
            text = text.Replace("&gt;", ">");
            text = text.Replace("<b>", "");
            text = text.Replace("<br>", " ");
            text = text.Replace("</b>", "");

            return text;
        }

        public static string CleanHtml(this string text)
        {
            text = text.Replace("&nbsp;", " ");
            return text;
        }

        public static string PadNumbers(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
        }

        public static string LastName(this string name)
        {
            if (name == null)
            {
                return name;
            }

            int i = name.LastIndexOf('.');
            if (i > 0)
            {
                return name.Substring(i + 1);
            }

            return name;
        }

        public static IEnumerable<string> Lines(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            using StringReader reader = new(text);
            while (reader.ReadLine() is { } line)
            {
                yield return line;
            }
        }
        
        public static IEnumerable<string> LinesOrComma(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }
            string[] lines = text.Split('\n', '\r', ',');
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    yield return line;
                }
            }
        }

        public static string TrimNonLetters(this string text)
        {
            int i = 0;
            foreach (char c in text)
            {
                if (char.IsLetter(c))
                {
                    break;
                }

                i++;
            }

            return text.Substring(i);
        }

        public static string FromDutchNumber(this string text)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsNumber(c))
                {
                    sb.Append(c);
                }
                else if (c == ',')
                {
                    sb.Append('.');
                }
                else if (c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }
}
