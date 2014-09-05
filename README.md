ReReflection - ReSharper plugin for .NET Reflection API
-------------------------------------------------------

The plugin is aimed to simplify work with .NET reflection API by providing quick-fixes, problem, highlights and auto-completion items. 

##[Download](http://resharper-plugins.jetbrains.com/api/v2/package/ReReflection/0.0.1)

## Articles

1. [Writing a ReSharper Plugin: Quick Fixes](http://elekslabs.com/2013/10/writing-resharper-plugin-quick-fixes.html)
2. [Writing a ReSharper Plugin: Problem Analyzers](http://elekslabs.com/2014/05/writing-a-resharper-plugin-problem-analyzers.html)
3. [Writing a ReSharper Plugin: Auto-completion] (http://elekslabs.com/2014/06/writing-a-resharper-plugin-auto-completion.html)
4. [Writing a ReSharper Plugin: Search and Navigation] (http://elekslabs.com/2014/08/writing-a-resharper-plugin-search-and-navigation.html)
5. And many more...

## Usages

This section describes basic plugin usages with examples.

### Search and Navigation

![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/search_navigation.gif)

### Quick-fixes

#### "Use reflection"
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/use_reflection.gif)
#### "Did you mean?"
Provides possible choices for 
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/did_you_mean.gif)
#### "Correct binding flags"
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/correct_binding_flag.gif)
#### "Remove binding flags"
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/remove_binding_flag.gif)


### Problem highlights

1. AmbigiousMemberMatchError – For cases when there are several method overloads with the same name in the reflected type.
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/ambigious_member.png)
2. BindingFlagsCanBeSkippedWarning – If BindingFlags specified as argument exactly matches the default value used by Reflection.
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/binding_flags_can_be_skipped.png)
3. IncorrectBindingFlagsError – BindingFlags specified for the current type member are incorrect. For example, BindingFlag.Static is missed for a static member.
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/incorrect_flags.png)
4. IncorrectMakeGenericTypeHighlighting – Highlighting for MakeGenericType misuse.
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/make_generic.png)
5. ReflectionMemberNotFoundError – Member with the specified name cannot be found in the reflected type.
![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/member_not_found.png)

### Auto-completion

![](https://github.com/YuriyZanichkovskyy/ReReflection/raw/master/Images/auto_completion.gif)

