using System;

namespace DimonSmart.StringTrimmerGenerator
{
    [Flags]
    public enum TrimType
    {
        None,
        Left = 1,
        Right = 2,
        LeftAndRignt = Left | Right,
        Seq = 4,
        All = LeftAndRignt | Seq,
    }
}
