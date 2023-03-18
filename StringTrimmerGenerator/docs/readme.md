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

Note: Visual studio have well known "bug" while using the source generators.
So you have to reopen the solution after adding source generator nuget package.

