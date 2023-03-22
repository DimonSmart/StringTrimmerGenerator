String Trimmer Generator - trim all class string propertines at once!
========

I'd love to have a quick and easy way to clean up user input, such cutting whitespace from the beginning and end of strings. In addition, I'd like to remove space sequences from within strings (optionally).
The StringTrimmer nuget does this flawlessly with just one speed restriction. Many users choose to manually sanitize user input or perhaps not to sanitize it at all because reflection slows it down. As a result, numerous people have email addresses and user names that are nearly identical. And next result, there are "duplicate" users with inconsistent access privileges, duplicate records in the database, and trim and distinct functions all over the place in the codebase. Deterioration of overall performance, demotivation of developers, and several other issues are the outcome.

# StringTrimmerGenerator
Don't ever rely on user input!
This package offers the ability to trim all class strings at once.
Sanitizing input data before processing is beneficial.


# Usage example
Typical request for user creation
```csharp
[GenerateStringTrimmer]
public class CreateUserRequest
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Nickname { get; set; }
    public UserPhone Phone { get; set; }
}

[GenerateStringTrimmer]
public class UserPhone
{
    public string PhoneNumber { get; set; }
    public string Comment { get; set; }
    public string Tags { get; set; }
    public override string ToString() => $"PhoneNumber:'{PhoneNumber}', Comment:'{Comment}', Tags:'{Tags}'";
}
```

## Fields trimming Before this nuget
```csharp 
public void CreateUser (CreateUserRequest request)
{
   request.Name = request.Name.Trim();
   request.Surname = request.Surname.Trim();
   request.NickName = request.NickName.Trim();
   request.Phone.PhoneNumber = request.Phone.PhoneNumber.Trim();
   request.Phone.Comment = request.Phone.Comment.Trim();
   request.Phone.Tags = request.Phone.Tags.Trim();
   // Send to db logic
}
```

## Fields trimming WITH this nuget
```csharp 
public void CreateUser (CreateUserRequest request)
{
   request.TrimAll();
   // Send to db logic
}
```

## Why is trimming user import is important?
Imaging you simply forgot trimming the UserName field
and send exa ctly the same users to the database many times.

As a result you have user(s)
* "John Doe "
* " John Doe"
* " John Doe "
* "John Doe"

Having the same user registered many times irritates the support personnel!

Note: It is usual for users to not merely input a UserName during
registration, but to copy it from other sources with different
spaces surrounding (and inside) the name.

Note: There is a StringTrimmer package with provide similar functionality
The difference is:
* StringTrimmer - reflection based.
* StringTrimmerGenerator - use Source Generator to provide best performance

For more infomation on the Source Generators feature, see the [design document](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md).

Prerequisites
-----

Visual Studio 16.6 or higher required

NOTE!
Wellknown bug in visual strudio!
You may need to close and reopen the solution in Visual Studio after first source generator usage.
