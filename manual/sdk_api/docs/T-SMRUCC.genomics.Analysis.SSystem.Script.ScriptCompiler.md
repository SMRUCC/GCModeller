---
title: ScriptCompiler
---

# ScriptCompiler
_namespace: [SMRUCC.genomics.Analysis.SSystem.Script](N-SMRUCC.genomics.Analysis.SSystem.Script.html)_

Modeling script compiler



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptCompiler.#ctor(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|path|The file path of the PLAS script.|


#### CheckConsist
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptCompiler.CheckConsist(SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var[],SMRUCC.genomics.Analysis.SSystem.Script.SEquation[])
```
Check the consist of the metabolites and the reactions.(检查代谢物和反应通路之间的一致性，确认是否有缺失的部分)

#### Compile
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptCompiler.Compile(System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|Path|The file path of the target compile script.(目标脚本的文件路径)|
|AutoFix|
 Optional，when error occur in the procedure of the script compiled, then if TRUE, the program was 
 trying to fix the error automatically, if FALSE, then the program throw an exception and then 
 exit the compile procedure.
 (可选参数，当在脚本文件的编译的过程之中出现错误的话，假若本参数为真，则程序会尝试着自己修复这个错误并给出
 警告，假若不为真，则程序会抛出一个错误并退出整个编译过程。请注意，即使本参数为真，当遭遇重大错误程序无法
 处理的时候，整个编译过程还是会被意外终止的。)
 |



