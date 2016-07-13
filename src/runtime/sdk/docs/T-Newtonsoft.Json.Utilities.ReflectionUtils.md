---
title: ReflectionUtils
---

# ReflectionUtils
_namespace: [Newtonsoft.Json.Utilities](N-Newtonsoft.Json.Utilities.html)_





### Methods

#### CanReadMemberValue
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.CanReadMemberValue(System.Reflection.MemberInfo,System.Boolean)
```
Determines whether the specified MemberInfo can be read.

|Parameter Name|Remarks|
|--------------|-------|
|member|The MemberInfo to determine whether can be read.|
|nonPublic|if set to true then allow the member to be gotten non-publicly.|

_returns: true if the specified MemberInfo can be read; otherwise, false.
            _

#### CanSetMemberValue
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.CanSetMemberValue(System.Reflection.MemberInfo,System.Boolean,System.Boolean)
```
Determines whether the specified MemberInfo can be set.

|Parameter Name|Remarks|
|--------------|-------|
|member|The MemberInfo to determine whether can be set.|
|nonPublic|if set to true then allow the member to be set non-publicly.|
|canSetReadOnly|if set to true then allow the member to be set if read-only.|

_returns: true if the specified MemberInfo can be set; otherwise, false.
            _

#### GetCollectionItemType
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.GetCollectionItemType(System.Type)
```
Gets the type of the typed collection's items.

|Parameter Name|Remarks|
|--------------|-------|
|type|The type.|

_returns: The type of the typed collection's items._

#### GetMemberUnderlyingType
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.GetMemberUnderlyingType(System.Reflection.MemberInfo)
```
Gets the member's underlying type.

|Parameter Name|Remarks|
|--------------|-------|
|member|The member.|

_returns: The underlying type of the member._

#### GetMemberValue
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.GetMemberValue(System.Reflection.MemberInfo,System.Object)
```
Gets the member's value on the object.

|Parameter Name|Remarks|
|--------------|-------|
|member|The member.|
|target|The target object.|

_returns: The member's value on the object._

#### IsIndexedProperty
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.IsIndexedProperty(System.Reflection.PropertyInfo)
```
Determines whether the property is an indexed property.

|Parameter Name|Remarks|
|--------------|-------|
|property|The property.|

_returns: true if the property is an indexed property; otherwise, false.
            _

#### SetMemberValue
```csharp
Newtonsoft.Json.Utilities.ReflectionUtils.SetMemberValue(System.Reflection.MemberInfo,System.Object,System.Object)
```
Sets the member's value on the target object.

|Parameter Name|Remarks|
|--------------|-------|
|member|The member.|
|target|The target.|
|value|The value.|



