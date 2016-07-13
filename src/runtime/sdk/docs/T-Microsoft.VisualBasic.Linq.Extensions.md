---
title: Extensions
---

# Extensions
_namespace: [Microsoft.VisualBasic.Linq](N-Microsoft.VisualBasic.Linq.html)_

Linq Helpers.(为了方便编写Linq代码而构建的一个拓展模块)



### Methods

#### CopyVector``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.CopyVector``1(``0,System.Int32)
```
Copy **source** **n** times to construct a new vector.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|n|-|

_returns: An array consist of source with n elements._

#### MatrixAsIterator``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.MatrixAsIterator``1(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{``0}})
```
Iterates all of the elements in a two dimension collection as the data source for the linq expression or ForEach statement.
 (适用于二维的集合做为linq的数据源，不像@"M:Microsoft.VisualBasic.Extensions.MatrixToList``1(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{``0}})"是进行转换，这个是返回迭代器的，推荐使用这个函数)

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### MaxInd``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.MaxInd``1(System.Collections.Generic.IEnumerable{``0})
```
Gets the max element its index in the collection

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### Read``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.Read``1(``0[],System.Int32@)
```
Read source at element position **i** and returns its value, 
 and then this function makes position **i** offset +1

|Parameter Name|Remarks|
|--------------|-------|
|array|-|
|i|-|


#### RemoveLeft``2
```csharp
Microsoft.VisualBasic.Linq.Extensions.RemoveLeft``2(System.Collections.Generic.Dictionary{``0,``1}@,``0)
```
删除制定的键之后返回剩下的数据

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|key|-|


#### Removes``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.Removes``1(System.Collections.Generic.IEnumerable{``0},System.Func{``0,System.Boolean},System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|match|符合这个条件的所有的元素都将会被移除|


#### SafeQuery``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.SafeQuery``1(System.Collections.Generic.IEnumerable{``0})
```
A query proxy function makes your linq not so easily crashed due to the unexpected null reference collection as linq source.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|


#### Sequence
```csharp
Microsoft.VisualBasic.Linq.Extensions.Sequence(System.UInt32)
```
产生指定数目的一个递增序列(所生成序列的数值就是生成的数组的元素的个数)

|Parameter Name|Remarks|
|--------------|-------|
|n|-|


#### ToArray``1
```csharp
Microsoft.VisualBasic.Linq.Extensions.ToArray``1(System.Int32,System.Func{System.Int32,``0})
```
(所生成序列的数值就是生成的数组的元素的个数)

|Parameter Name|Remarks|
|--------------|-------|
|len|-|
|elementAt|-|


#### ToArray``2
```csharp
Microsoft.VisualBasic.Linq.Extensions.ToArray``2(System.Collections.Generic.IEnumerable{``0},System.Func{``0,System.Int32,``1},System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|__ctype|第二个参数是index|
|Parallel|-|



