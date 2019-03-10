# ScriptParser
_namespace: [SMRUCC.genomics.Analysis.SSystem.Script](./index.md)_





### Methods

#### ConstantParser
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptParser.ConstantParser(Microsoft.VisualBasic.Scripting.TokenIcer.Token{SMRUCC.genomics.Analysis.SSystem.Script.Tokens})
```
这里只是进行解析，并没有立即进行求值

|Parameter Name|Remarks|
|--------------|-------|
|expr|-|


#### ExperimentParser
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptParser.ExperimentParser(System.String)
```
解析出系统的状态扰动实验表达式

|Parameter Name|Remarks|
|--------------|-------|
|line|-|


#### ParseScript
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptParser.ParseScript(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|scriptText|脚本的文本内容|


#### ParseStream
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptParser.ParseStream(System.IO.Stream)
```
从文件指针或者网络数据之中解析出脚本模型

|Parameter Name|Remarks|
|--------------|-------|
|s|-|


#### sEquationParser
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.ScriptParser.sEquationParser(Microsoft.VisualBasic.Scripting.TokenIcer.Token{SMRUCC.genomics.Analysis.SSystem.Script.Tokens})
```
解析系统方程表达式

|Parameter Name|Remarks|
|--------------|-------|
|x|-|



