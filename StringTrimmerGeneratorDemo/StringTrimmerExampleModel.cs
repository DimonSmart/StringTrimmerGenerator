using DimonSmart.StringTrimmer;

namespace DimonSmart.StringTrimmerGeneratorDemo
{
    [GenerateStringTrimmer]
    public class StringTrimmerExampleModel
    {
        public string Name { get; set; } = " A ";
        public string Surname { get; set; } = " B ";
        private string PrivateProperty { get; set; } = " C ";
        //  public string PrivateSetterProperty { get; private set; }

        public SubClass Sub { get; set; } = new SubClass();
    }

    [GenerateStringTrimmer]
    public class SubClass
    {
        public string S1 { get; set; } = " S ";
    }


}






