using System;

namespace DimonSmart.StringTrimmerGenerator
{
    public class Indent : IDisposable
    {
        private readonly TabbedStringBuilder _tabbedStringBuilder;
        private readonly int _indent;

        public Indent(TabbedStringBuilder tabbedStringBuilder, int indent = 4)
        {
            _tabbedStringBuilder = tabbedStringBuilder;
            _indent = indent;
            _tabbedStringBuilder.Indent += _indent;
        }

        public void Dispose()
        {
            _tabbedStringBuilder.Indent -= _indent;
        }
    }
}
