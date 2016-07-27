---
title: TypeRegistry
---

# TypeRegistry
_namespace: [Microsoft.VisualBasic.Linq.Framework.Provider](N-Microsoft.VisualBasic.Linq.Framework.Provider.html)_

Type registry table for loading the external LINQ entity assembly module.
 (起始这个模块就是相当于一个类型缓存而已，因为程序可以直接读取dll文件里面的内容，但是直接读取的方法会造成性能下降，所以需要使用这个对象来缓存所需要的类型数据)



### Methods

#### Find
```csharp
Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry.Find(System.String)
```
Return a registry item in the table using its specific name property.
 (返回注册表中的一个指定名称的项目)

|Parameter Name|Remarks|
|--------------|-------|
|name|大小写不敏感的|


#### GetHandle
```csharp
Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry.GetHandle(System.String)
```
查找不成功会返回空值

|Parameter Name|Remarks|
|--------------|-------|
|name|-|


#### InstallCurrent
```csharp
Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry.InstallCurrent
```
扫描安装应用程序文件夹之中的所有插件

#### LoadAssembly
```csharp
Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry.LoadAssembly(System.String)
```
返回包含有该类型的目标模块的文件路径

|Parameter Name|Remarks|
|--------------|-------|
|name|LINQ Entity集合中的元素的简称或者别称，即Item中的Name属性|

_returns: If the key is not exists in this object, than the function will return a empty string._

#### Register
```csharp
Microsoft.VisualBasic.Linq.Framework.Provider.TypeRegistry.Register(System.String)
```
Registry the external LINQ entity assembly module in the LINQFramework

|Parameter Name|Remarks|
|--------------|-------|
|assmPath|DLL file path|

> 查询出目标元素的类型定义并获取信息


### Properties

#### SDK
.NET SDK directory
