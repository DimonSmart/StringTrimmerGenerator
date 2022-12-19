using DimonSmart.StringTrimmer;

namespace DimonSmart.StringTrimmerGeneratorDemo
{
    [GenerateStringTrimmer]
    public class StringTrimmerExampleModel
    {
        public string PublicName { get; set; } = " A ";
        internal string InternalName { get; set; } = " AI ";
        private string PrivateName { get; set; } = " C ";
        public string PublicNamePrivateSetter { get; private set; }

        public SubClass Sub { get; set; } = new SubClass();
    }

    [GenerateStringTrimmer]
    public class SubClass
    {
        public string S1 { get; set; } = " S ";
    }
}






