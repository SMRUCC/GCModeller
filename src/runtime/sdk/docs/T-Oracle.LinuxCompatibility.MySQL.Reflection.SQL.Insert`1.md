---
title: Insert`1
---

# Insert`1
_namespace: [Oracle.LinuxCompatibility.MySQL.Reflection.SQL](N-Oracle.LinuxCompatibility.MySQL.Reflection.SQL.html)_



> 
>  Example SQL:
>  
>  INSERT INTO `TableName` (`Field1`, `Field2`, `Field3`) VALUES ('1', '1', '1');
>  


### Methods

#### Generate
```csharp
Oracle.LinuxCompatibility.MySQL.Reflection.SQL.Insert`1.Generate(`0)
```
Generate the INSERT sql command of the instance of the specific type of 'Schema'.
 (生成特定的'Schema'数据类型实例的 'INSERT' sql命令)

|Parameter Name|Remarks|
|--------------|-------|
|value|The instance to generate this command of type 'Schema'|

_returns: INSERT sql text_


### Properties

#### InsertSQL
INSERT INTO `TableName` (`Field1`, `Field2`, `Field3`, ...) VALUES ('{0}', '{1}', '{2}', ...);
