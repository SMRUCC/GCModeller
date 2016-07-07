---
title: DataFile`1
---

# DataFile`1
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.html)_

All of the data file object in the metacyc database will be inherits from this class object type.
 (在MetaCyc数据库之中的所有元素对象都必须要继承自此对象类型)



### Methods

#### Add
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Add(`0)
```
Adding the target element object instance into the current list object, if the target element is 
 already exists in the list object then not add the target object and return its current position 
 in the list or add the target object into the list when it is not appear in the list object and 
 then return the length of the current list object.
 (将目标元素对象添加至当前的列表之中，假若目标对象存在于列表之中，则进行添加并返回列表的最后一个元素的位置，
 否则不对目标元素进行添加并返回目标元素在列表中的当前位置)

|Parameter Name|Remarks|
|--------------|-------|
|x|
 The target element that want to added into the list object.(将要添加进入列表之中的目标元素对象)
 |


#### Append
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Append(`0)
```
Just add the element into the current list object and return the length of it, this method is fast than [Add(T) As Long] function, 
 but an element duplicate error may occur.
 (仅仅只是将目标元素添加进入当前的列表对象之中并返回添加了该元素的列表对象的新长度，本方法的速度要快于Add方法，但是可能会出现列表元素重复的错误)

|Parameter Name|Remarks|
|--------------|-------|
|e|
 The element that will be add into the current list object.(将要添加进入当前的列表对象的目标元素对象)
 |

_returns: The length of the current list object.(当前列表元素的长度)_

#### Clear
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Clear
```
Clear all of the data that exists in this list object.(将本列表对象中的所有的数据进行清除操作)

#### IndexOf
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.IndexOf(System.String)
```
Locate the target object using its unique id property, this function will return the location point of 
 the target object in the list if we found it or return -1 if the object was not found.
 (使用目标对象的唯一标识符属性对其进行在本列表对象中的定位操作，假若查找到了目标对象则返回其位置，反之则返回-1值)

|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|


#### Select
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Select(`0,System.Reflection.PropertyInfo[],SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection.MetaCycField[],System.Boolean)
```
本函数较[Select]([Object] As T, ParamArray Fields As String())函数有着更高的查询性能

|Parameter Name|Remarks|
|--------------|-------|
|Object|-|
|ItemProperties|所缓存的类型信息|
|FieldAttributes|所缓存的类型信息|


#### Select2
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Select2(System.String)
```
使用Index对象进行对象实例目标的查找操作

|Parameter Name|Remarks|
|--------------|-------|
|UniqueId|-|

> 请务必要确保Index的顺序和FrameObjects的顺序一致

#### Takes
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.DataFile`1.Takes(System.Collections.Generic.IEnumerable{System.String})
```
Takes a sub list of the elements that were pointed by the unique-id collection.
 (获取一个UniqueId集合所指向的对象元素列表，会自动过滤掉不存在的UniqueId值)

|Parameter Name|Remarks|
|--------------|-------|
|uids|
 The unique-id collection of the objects that wants to take from the list obejct.
 (将要从本列表对象获取的对象的唯一标识符的集合)
 |



### Properties

#### AttributeList
BaseType Attribute List is empty.
#### Item
Get a object from current list object using its @"P:SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object.Identifier"[unique-id] property.(根据一个对象的Unique-Id字段的值来获取该目标对象，查询失败则返回空值)
#### NumOfTokens
The length of the current list objetc.(当前的列表对象的长度)
