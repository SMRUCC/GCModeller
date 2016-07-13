---
title: CLIBuildMethod
---

# CLIBuildMethod
_namespace: [Microsoft.VisualBasic.CommandLine](N-Microsoft.VisualBasic.CommandLine.html)_





### Methods

#### __pathRule
```csharp
Microsoft.VisualBasic.CommandLine.CLIBuildMethod.__pathRule(System.Object,Microsoft.VisualBasic.CommandLine.Reflection.Optional,System.Reflection.PropertyInfo)
```


|Parameter Name|Remarks|
|--------------|-------|
|value|只能是@"T:System.String"类型的|
|attr|-|
|prop|-|


#### __stringRule
```csharp
Microsoft.VisualBasic.CommandLine.CLIBuildMethod.__stringRule(System.Object,Microsoft.VisualBasic.CommandLine.Reflection.Optional,System.Reflection.PropertyInfo)
```
可能包含有枚举值

|Parameter Name|Remarks|
|--------------|-------|
|value|-|
|attr|-|
|prop|-|


#### ClearParameters``1
```csharp
Microsoft.VisualBasic.CommandLine.CLIBuildMethod.ClearParameters``1(``0)
```


|Parameter Name|Remarks|
|--------------|-------|
|inst|-|

_returns: 返回所重置的参数的个数_

#### GetCLI``1
```csharp
Microsoft.VisualBasic.CommandLine.CLIBuildMethod.GetCLI``1(``0)
```
Generates the command line string value for the invoked target cli program using this interop services object instance.
 (生成命令行参数)

|Parameter Name|Remarks|
|--------------|-------|
|Instance|目标交互对象的实例|

> 
>  依照类型@"T:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes"来生成参数字符串
>  
>  @"F:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes.Boolean", True => 参数名；
>  @"F:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes.Double", @"F:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes.Integer", @"F:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes.String", => 参数名 + 参数值，假若字符串为空则不添加；
>  （假若是枚举值类型，可能还需要再枚举值之中添加@"T:System.ComponentModel.DescriptionAttribute"属性）
>  @"F:Microsoft.VisualBasic.CommandLine.Reflection.CLITypes.File", 假若字符串为空则不添加，有空格自动添加双引号，相对路径会自动转换为全路径。
>  


