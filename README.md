🚧 Work In Progress 
========

I'd love to have a fast and simple way to sanitize user input such as trimming spaces at the begining and at the end of strings. Additionaly I'd love to remove space sequences inside the string (optionally)
StringTrimmer package do it perfectly with only one limitation - speed. As it uses reflection it is not as fast as possible, so many users prefer to sanitize user input manually or even not sanitize it at all.
As a result multiple users with "same" user names and email addresses with only space difference. And as result "duplicate" users with inconsistent rights, database polluted with duplicate records, as a result
codebase polluted with trim and distinct function everywhere. As a result total performance degradation, developers demotivation and many other troubles.

I'd love to share simple solution for solve many problems in the earlies stage.

Trim all strings in a class with only one function call!
 
For more infomation on the Source Generators feature, see the [design document](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md).

Prerequisites
-----

Visual Studio 16.6 or higher required

NOTE!
Wellknown bug in visual strudio!
You may need to close and reopen the solution in Visual Studio after first source generator usage.
