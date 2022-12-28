using DimonSmart.StringTrimmer;

namespace DimonSmart.StringTrimmerGeneratorDemo
{

    [GenerateStringTrimmer]
    public class UserPhone
    {
        public string PhoneNumber { get; set; }
        public string Comment { get; set; }
        public string Tags { get; set; }
        public override string ToString() => $"PhoneNumber:'{PhoneNumber}', Comment:'{Comment}', Tags:'{Tags}'";
    }
}
