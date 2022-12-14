🚧 Work In Progress 
========

I'd love to have a quick and easy way to clean up user input, such cutting whitespace from the beginning and end of strings. In addition, I'd like to remove space sequences from within strings (optionally). The StringTrimmer software does this flawlessly with just one speed restriction. Many users choose to manually sanitize user input or perhaps not to sanitize it at all because reflection slows it down. As a result, numerous people have email addresses and user names that are nearly identical. And as a result, there are "duplicate" users with inconsistent access privileges, duplicate records in the database, and trim and distinct functions all over the place in the codebase. Deterioration of overall performance, demotivation of developers, and several other issues are the outcome.

I'd want to provide some quick fixes for common early-stage issues.

With a single function call, you can trim every string in a class.
 
For more infomation on the Source Generators feature, see the [design document](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.md).

Prerequisites
-----

Visual Studio 16.6 or higher required

NOTE!
Wellknown bug in visual strudio!
You may need to close and reopen the solution in Visual Studio after first source generator usage.
