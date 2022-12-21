using System.Text;

namespace DimonSmart.StringTrimmerGenerator
{
    public class TabbedStringBuilder
    {
        private readonly StringBuilder _stringBuilder = new();
        public int Indent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public TabbedStringBuilder AppendLine(string text)
        {
            _stringBuilder.Append(new string(' ', Indent));
            _stringBuilder.AppendLine(text);
            return this;
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

    }
}
