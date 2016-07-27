---
title: LinqProvider
---

# LinqProvider
_namespace: [Microsoft.VisualBasic.ComputingServices.TaskHost](N-Microsoft.VisualBasic.ComputingServices.TaskHost.html)_

执行得到数据集合然后分独传输数据元素



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.LinqProvider.#ctor(System.Collections.IEnumerable,System.Type)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|type|Element's @"T:System.Type"[type] in the **source**|


#### Moves
```csharp
Microsoft.VisualBasic.ComputingServices.TaskHost.LinqProvider.Moves(System.Int32,System.Boolean@)
```


|Parameter Name|Remarks|
|--------------|-------|
|n|
 个数小于或者等于1，就只会返回一个对象；
 个数大于1的，会读取相应数量的元素然后返回一个集合类型
 |
|readDone|-|



### Properties

#### __svrThread
使用线程可能会在出现错误的时候导致应用程序崩溃，所以在这里使用begineInvoke好了
#### BaseType
Linq数据源的集合的类型信息
#### IsOpen
当前的这个数据源服务是否已经正确的开启了？
