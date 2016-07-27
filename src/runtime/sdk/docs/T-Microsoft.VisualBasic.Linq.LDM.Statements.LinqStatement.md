---
title: LinqStatement
---

# LinqStatement
_namespace: [Microsoft.VisualBasic.Linq.LDM.Statements](N-Microsoft.VisualBasic.Linq.LDM.Statements.html)_

A linq statement object model.

> 
>  From [Object [As TypeId]] 
>  In [Collection] 
>  Let [Declaration1, Declaration2, ...]
>  Where [Condition Test] 
>  Select [Object/Object Constrctor] 
>  [Distinct] 
>  [Order Statement]


### Methods

#### TryParse
```csharp
Microsoft.VisualBasic.Linq.LDM.Statements.LinqStatement.TryParse(System.String,Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry)
```
Try to parsing a linq query script into a statement object model and compile the model into a assembly dynamic.
 (尝试着从所输入的命令语句中解析出一个LINQ查询命令对象，并完成动态编译过程)

|Parameter Name|Remarks|
|--------------|-------|
|source|-|



### Properties

#### PreDeclare
A read only object collection which were construct by the LET statement token in the LINQ statement.
 (使用Let语句所构造出来的只读对象类型的对象申明集合)
#### SelectClosure
A expression for return the query result.(用于生成查询数据返回的语句)
#### source
Target query collection expression, this can be a file path or a database connection string.
 (目标待查询集合，值可以为一个文件路径或者数据库连接字符串)
#### Text
Original statement text of this linq expression
#### TypeId
获取目标LINQCollection待查询集合中的元素对象的类型标识符，以进行外部模块的动态加载
 与RegistryItem中的Name属性值相对应
#### var
An object element in the target query collection.(目标待查询集合之中的一个元素)
#### Where
Where test condition for the query.(查询所使用的Where条件测试语句)
