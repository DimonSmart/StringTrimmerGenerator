using DimonSmart.StringTrimmer;
using System;
using System.Diagnostics;

namespace DimonSmart.StringTrimmerGeneratorDemo
{
    [GenerateStringTrimmer]
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Nickname { get; set; }
        public UserPhone Phone { get; set; }

        public override string ToString() => $"Name:'{Name}', Surname:'{Surname}', Nickname:'{Nickname}',{Environment.NewLine}Phone:'{Phone}'";
    }
}
