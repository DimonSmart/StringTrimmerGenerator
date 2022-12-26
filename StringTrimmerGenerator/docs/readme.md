# StringTrimmerGenerator
Don't ever rely on user input!
This package offers the ability to trim every string in a class.
Sanitizing input data before processing is beneficial.

NOTE1: There is a StringTrimmer package with provide similar functionality
The difference is:
* StringTrimmer - reflection based
* StringTrimmerGenerator - use Source Generator to provide best performance

NOTE2: StringTrimmerGenerator support trim operation over all public strings
and public class properties for classes marked with [GenerateStringTrimmer] attribute

# Problem statement
Standard class for describing a user.
```csharp
public class User
{
   public string Name {get; set;}
   public string Surname {get; set;}
   public string Secondname {get; set;}
   public string UserName {get; set;}
   public string Email {get; set;}
   public string Phone {get; set;}
}
```

Usage of the User class to add a new User record to the database is typical.
## Can you find an error?
```csharp
Try 
public void CreateUser (User user)
{
   user.Name = user.Name.Trim();
   user.Surname = user.Surname.Trim();
   user.Secondname = user.Secondname.Trim();
   user.Email = user.Email.Trim();
   user.Phone = user.Phone.Trim();
}
```

Error: UserName Trim command is missing.
Due to the tiny difference in UserName, this allows for many users while having only one real user.

* "John Doe "
* " John Doe"
* " John Doe "
* "John Doe"

Having the same user registered many times irritates the support personnel!

Note: It is usual for users to not merely input a UserName during registration, but to copy it from other sources with different spaces surrounding (and inside) the name.


# Magic bullet! (Usage example)
```csharp
Try 
public void CreateUser (CreateUserRequest request)
{
   request.TrimAll(); // Just one line
}
```

