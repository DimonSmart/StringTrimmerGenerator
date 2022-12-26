using System;

namespace DimonSmart.StringTrimmerGeneratorDemo
{
    class Program
    {
        static void Main(string[] _)
        {
            var model = new CreateUserRequest
            {
                Name = "John  Doe ",
                Nickname = " JohnDoe",
                Surname = "Junior",
                Phone = new UserPhone
                {
                    PhoneNumber = "+7  555 55 555 ",
                    Comment = "Whatsapp  please",
                    Tags = "Whatsapp, Personal"
                }
            };

            Console.WriteLine("Check useless spaces, before and after");
            Console.WriteLine(model);

            // Just all methods demo
            // model.TrimExtraSpaces();
            // model.TrimAll();
            // model.TrimStart();
            // model.TrimEnd();

            // Recommended method in most cases
            model.TrimAll();
            Console.WriteLine(model);
        }
    }
}
