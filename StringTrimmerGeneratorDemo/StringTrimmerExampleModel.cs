using DimonSmart.StringTrimmer;

namespace DimonSmart.StringTrimmerGeneratorDemo
{
    [GenerateStringTrimmer]
    public class StringTrimmerExampleModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        private string PrivateProperty { get; set; }
        //  public string PrivateSetterProperty { get; private set; }
    }
}






