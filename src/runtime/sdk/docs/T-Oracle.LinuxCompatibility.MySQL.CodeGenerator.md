---
title: CodeGenerator
---

# CodeGenerator
_namespace: [Oracle.LinuxCompatibility.MySQL](N-Oracle.LinuxCompatibility.MySQL.html)_

Automatically generates visualbasic source code from the SQL schema dump document.(根据SQL文档生成Visual Basic源代码)



### Methods

#### __generateCode
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.__generateCode(System.Collections.Generic.IEnumerable{Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Table},System.String,System.String,System.Collections.Generic.Dictionary{System.String,System.String},System.String)
```
Generate the source code file from the table schema dumping

|Parameter Name|Remarks|
|--------------|-------|
|SqlDoc|-|
|Head|-|
|FileName|-|
|TableSql|-|


#### __generateCodeSplit
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.__generateCodeSplit(System.Collections.Generic.IEnumerable{Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Table},System.String,System.String,System.Collections.Generic.Dictionary{System.String,System.String},System.String)
```
Generate the source code file from the table schema dumping

|Parameter Name|Remarks|
|--------------|-------|
|SqlDoc|-|
|Head|-|
|FileName|-|
|TableSql|-|


#### __getExprInvoke
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.__getExprInvoke(Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Field,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|Field|-|


#### GenerateCode
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.GenerateCode(System.String,System.String)
```
Convert the sql definition into the visualbasic source code.

|Parameter Name|Remarks|
|--------------|-------|
|SqlDump|The SQL dumping file path.(Dump sql文件的文件路径)|

_returns: VisualBasic source code_

#### GenerateCodeSplit
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.GenerateCodeSplit(System.String,System.String)
```
返回 {类名, 类定义}

|Parameter Name|Remarks|
|--------------|-------|
|SqlDump|The SQL dumping file path.(Dump sql文件的文件路径)|


#### GenerateTableClass
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.GenerateTableClass(Oracle.LinuxCompatibility.MySQL.Reflection.Schema.Table,System.String,System.Boolean)
```
Generate the class object definition to mapping a table in the mysql database.

|Parameter Name|Remarks|
|--------------|-------|
|Table|-|
|DefSql|-|


#### TrimKeyword
```csharp
Oracle.LinuxCompatibility.MySQL.CodeGenerator.TrimKeyword(System.String)
```
Works with the conflicts of the VisualBasic keyword.(处理VB里面的关键词的冲突)

|Parameter Name|Remarks|
|--------------|-------|
|name|-|



