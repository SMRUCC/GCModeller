---
title: Function
---

# Function
_namespace: [Microsoft.VisualBasic.Mathematical.Helpers](N-Microsoft.VisualBasic.Mathematical.Helpers.html)_

Mathematics function calculation engine
 (数学函数计算引擎)



### Methods

#### Add
```csharp
Microsoft.VisualBasic.Mathematical.Helpers.Function.Add(System.String)
```
Parsing the use function definition from the user input value on the console 
 and then add it to the function dictionary.
 (从终端上面输入的用户函数的申明语句中解析出表达式，然后将其加入到用户字典中)

|Parameter Name|Remarks|
|--------------|-------|
|statement|[function name](args) expression|

> function [function name] expression

#### Evaluate
```csharp
Microsoft.VisualBasic.Mathematical.Helpers.Function.Evaluate(System.String,System.Double[])
```
大小写不敏感

|Parameter Name|Remarks|
|--------------|-------|
|name|-|
|args|-|


#### RND
```csharp
Microsoft.VisualBasic.Mathematical.Helpers.Function.RND(System.Double,System.Double)
```
This function return a random number, you can specific the boundary of the random number in the parameters.

|Parameter Name|Remarks|
|--------------|-------|
|UpBound|
 If this parameter is empty or value is zero, then return the randome number between 0 and 1.
 (如果这个参数为空或者其值为0，那么函数就会返回0和1之间的随机数)
 |
|LowBound|-|



### Properties

#### SystemPrefixFunctions
The mathematics calculation delegates collection with its specific name.
 (具有特定名称的数学计算委托方法的集合)
