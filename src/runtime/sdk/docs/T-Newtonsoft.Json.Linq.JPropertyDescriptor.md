---
title: JPropertyDescriptor
---

# JPropertyDescriptor
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a view of a @"T:Newtonsoft.Json.Linq.JProperty".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.#ctor(System.String)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JPropertyDescriptor" class.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|


#### CanResetValue
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.CanResetValue(System.Object)
```
When overridden in a derived class, returns whether resetting an object changes its value.

|Parameter Name|Remarks|
|--------------|-------|
|component|The component to test for reset capability.|

_returns: true if resetting the component changes its value; otherwise, false.
            _

#### GetValue
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.GetValue(System.Object)
```
When overridden in a derived class, gets the current value of the property on a component.

|Parameter Name|Remarks|
|--------------|-------|
|component|The component with the property for which to retrieve the value. 
                            |

_returns: 
            The value of a property for a given component.
            _

#### ResetValue
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.ResetValue(System.Object)
```
When overridden in a derived class, resets the value for this property of the component to the default value.

|Parameter Name|Remarks|
|--------------|-------|
|component|The component with the property value that is to be reset to the default value. 
                            |


#### SetValue
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.SetValue(System.Object,System.Object)
```
When overridden in a derived class, sets the value of the component to a different value.

|Parameter Name|Remarks|
|--------------|-------|
|component|The component with the property value that is to be set. 
                            |
|value|The new value. 
                            |


#### ShouldSerializeValue
```csharp
Newtonsoft.Json.Linq.JPropertyDescriptor.ShouldSerializeValue(System.Object)
```
When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.

|Parameter Name|Remarks|
|--------------|-------|
|component|The component with the property to be examined for persistence.|

_returns: true if the property should be persisted; otherwise, false.
            _


### Properties

#### ComponentType
When overridden in a derived class, gets the type of the component this property is bound to.
#### IsReadOnly
When overridden in a derived class, gets a value indicating whether this property is read-only.
#### NameHashCode
Gets the hash code for the name of the member.
#### PropertyType
When overridden in a derived class, gets the type of the property.
