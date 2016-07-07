---
title: CodeDOMExpressions
---

# CodeDOMExpressions
_namespace: [Microsoft.VisualBasic.CodeDOM_VBC](N-Microsoft.VisualBasic.CodeDOM_VBC.html)_





### Methods

#### Call
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.Call(System.Type,System.String,System.Object[])
```
Call a statics function from a specific type with a known function name

|Parameter Name|Remarks|
|--------------|-------|
|type|-|
|Name|-|
|parametersValue|-|


#### DeclareFunc
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.DeclareFunc(System.String,System.Collections.Generic.Dictionary{System.String,System.Type},System.Type,System.CodeDom.MemberAttributes)
```
声明一个函数

|Parameter Name|Remarks|
|--------------|-------|
|name|-|
|args|-|
|returns|-|
|control|-|


#### FieldRef
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.FieldRef(System.String)
```
Reference of Me.Field

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|


#### GetType
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.GetType(System.Type)
```
System.Type.GetType(TypeName)

|Parameter Name|Remarks|
|--------------|-------|
|Type|-|


#### GetValue
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.GetValue(System.CodeDom.CodeExpression,System.Int32)
```
Gets the element value in a array object.

|Parameter Name|Remarks|
|--------------|-------|
|Array|-|
|index|-|


#### LocalsInit
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.LocalsInit(System.String,System.Type,System.Object)
```
Declare a local variable.

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|
|Type|-|
|init|-|


#### LocalVariable
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.LocalVariable(System.String)
```
Reference to a local variable in a function body.(引用局部变量)

|Parameter Name|Remarks|
|--------------|-------|
|Name|-|


#### New
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.New(System.Type,System.CodeDom.CodeExpression[])
```
Class object instance constructor

|Parameter Name|Remarks|
|--------------|-------|
|Type|-|
|parameters|-|


#### New``1
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.New``1(System.Object[])
```
Class object instance constructor.

|Parameter Name|Remarks|
|--------------|-------|
|parameters|-|


#### Reference
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.Reference(System.CodeDom.CodeExpression,System.String)
```
Reference to a instance field in the specific object instance.

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|
|Name|-|


#### Return
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.Return(System.CodeDom.CodeExpression)
```
Returns value in a function body

|Parameter Name|Remarks|
|--------------|-------|
|expression|-|


#### Value
```csharp
Microsoft.VisualBasic.CodeDOM_VBC.CodeDOMExpressions.Value(System.Object)
```
Variable value initializer

|Parameter Name|Remarks|
|--------------|-------|
|obj|-|



### Properties

#### EntryPoint
Public Shared Function Main(Argvs As String()) As Integer
