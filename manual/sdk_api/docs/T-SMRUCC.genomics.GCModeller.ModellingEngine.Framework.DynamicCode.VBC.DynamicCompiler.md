---
title: DynamicCompiler
---

# DynamicCompiler
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC](N-SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC.html)_

编译整个LINQ语句的动态代码编译器



### Methods

#### #ctor
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC.DynamicCompiler.#ctor(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|SDK|.NET Framework Reference Assembly文件夹的位置|


#### Compile
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC.DynamicCompiler.Compile(System.CodeDom.CodeCompileUnit,System.String[],System.String,System.String,Microsoft.VisualBasic.Logging.LogFile)
```
Compile the codedom object model into a binary assembly module file.(将CodeDOM对象模型编译为二进制应用程序文件)

|Parameter Name|Remarks|
|--------------|-------|
|ObjectModel|CodeDom dynamic code object model.(目标动态代码的对象模型)|
|Reference|Reference assemby file path collection.(用户代码的引用DLL文件列表)|
|DotNETReferenceAssembliesDir|.NET Framework SDK|
|CodeStyle|VisualBasic, C#|


#### DeclareFunction
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC.DynamicCompiler.DeclareFunction(System.String,System.String,System.CodeDom.CodeStatementCollection)
```
Declare a function with a specific function name and return type. please notice that in this newly 
 declare function there is always a local variable name rval using for return the value.
 (申明一个方法，返回指定类型的数据并且具有一个特定的函数名，请注意，在这个新申明的函数之中，
 固定含有一个rval的局部变量用于返回数据)

|Parameter Name|Remarks|
|--------------|-------|
|Name|Function name.(函数名)|
|Type|Function return value type.(该函数的返回值类型)|

_returns: A codeDOM object model of the target function.(一个函数的CodeDom对象模型)_

#### GenerateCode
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.Framework.DynamicCode.VBC.DynamicCompiler.GenerateCode(System.CodeDom.CodeNamespace,System.String)
```
Generate the source code from the CodeDOM object model.(根据对象模型生成源代码以方便调试程序)

|Parameter Name|Remarks|
|--------------|-------|
|NameSpace|-|
|CodeStyle|VisualBasic, C#|

> 
>  You can easily convert the source code between VisualBasic and C# using this function just by makes change in statement: 
>  CodeDomProvider.GetCompilerInfo("VisualBasic").CreateProvider().GenerateCodeFromNamespace([NameSpace], sWriter, Options)
>  Modify the VisualBasic in to C#
>  


### Properties

#### DotNETReferenceAssembliesDir

