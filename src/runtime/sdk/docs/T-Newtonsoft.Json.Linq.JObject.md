---
title: JObject
---

# JObject
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a JSON object.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JObject.#ctor(System.Object)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JObject" class with the specified content.

|Parameter Name|Remarks|
|--------------|-------|
|content|The contents of the object.|


#### Add
```csharp
Newtonsoft.Json.Linq.JObject.Add(System.String,Newtonsoft.Json.Linq.JToken)
```
Adds the specified property name.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|
|value|The value.|


#### FromObject
```csharp
Newtonsoft.Json.Linq.JObject.FromObject(System.Object,Newtonsoft.Json.JsonSerializer)
```
Creates a @"T:Newtonsoft.Json.Linq.JObject" from an object.

|Parameter Name|Remarks|
|--------------|-------|
|o|The object that will be used to create @"T:Newtonsoft.Json.Linq.JObject".|
|jsonSerializer|The @"T:Newtonsoft.Json.JsonSerializer" that will be used to read the object.|

_returns: A @"T:Newtonsoft.Json.Linq.JObject" with the values of the specified object_

#### GetEnumerator
```csharp
Newtonsoft.Json.Linq.JObject.GetEnumerator
```
Returns an enumerator that iterates through the collection.
_returns: 
            A @"T:System.Collections.Generic.IEnumerator`1" that can be used to iterate through the collection.
            _

#### GetMetaObject
```csharp
Newtonsoft.Json.Linq.JObject.GetMetaObject(System.Linq.Expressions.Expression)
```
Returns the @"T:System.Dynamic.DynamicMetaObject" responsible for binding operations performed on this object.

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression tree representation of the runtime value.|

_returns: 
            The @"T:System.Dynamic.DynamicMetaObject" to bind this object.
            _

#### GetValue
```csharp
Newtonsoft.Json.Linq.JObject.GetValue(System.String,System.StringComparison)
```
Gets the @"T:Newtonsoft.Json.Linq.JToken" with the specified property name.
 The exact property name will be searched for first and if no matching property is found then
 the @"T:System.StringComparison" will be used to match a property.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|
|comparison|One of the enumeration values that specifies how the strings will be compared.|

_returns: The @"T:Newtonsoft.Json.Linq.JToken" with the specified property name._

#### Load
```csharp
Newtonsoft.Json.Linq.JObject.Load(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Loads an @"T:Newtonsoft.Json.Linq.JObject" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|A @"T:Newtonsoft.Json.JsonReader" that will be read for the content of the @"T:Newtonsoft.Json.Linq.JObject".|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JObject" that contains the JSON that was read from the specified @"T:Newtonsoft.Json.JsonReader"._

#### OnPropertyChanged
```csharp
Newtonsoft.Json.Linq.JObject.OnPropertyChanged(System.String)
```
Raises the @"E:Newtonsoft.Json.Linq.JObject.PropertyChanged" event with the provided arguments.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|


#### OnPropertyChanging
```csharp
Newtonsoft.Json.Linq.JObject.OnPropertyChanging(System.String)
```
Raises the @"E:Newtonsoft.Json.Linq.JObject.PropertyChanging" event with the provided arguments.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|


#### Parse
```csharp
Newtonsoft.Json.Linq.JObject.Parse(System.String,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Load a @"T:Newtonsoft.Json.Linq.JObject" from a string that contains JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|A @"T:System.String" that contains JSON.|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JObject" populated from the string that contains JSON._

#### Properties
```csharp
Newtonsoft.Json.Linq.JObject.Properties
```
Gets an @"T:System.Collections.Generic.IEnumerable`1" of this object's properties.
_returns: An @"T:System.Collections.Generic.IEnumerable`1" of this object's properties._

#### Property
```csharp
Newtonsoft.Json.Linq.JObject.Property(System.String)
```
Gets a @"T:Newtonsoft.Json.Linq.JProperty" the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The property name.|

_returns: A @"T:Newtonsoft.Json.Linq.JProperty" with the specified name or null._

#### PropertyValues
```csharp
Newtonsoft.Json.Linq.JObject.PropertyValues
```
Gets an @"T:Newtonsoft.Json.Linq.JEnumerable`1" of this object's property values.
_returns: An @"T:Newtonsoft.Json.Linq.JEnumerable`1" of this object's property values._

#### Remove
```csharp
Newtonsoft.Json.Linq.JObject.Remove(System.String)
```
Removes the property with the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|

_returns: true if item was successfully removed; otherwise, false._

#### System#ComponentModel#ICustomTypeDescriptor#GetAttributes
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetAttributes
```
Returns a collection of custom attributes for this instance of a component.
_returns: 
            An @"T:System.ComponentModel.AttributeCollection" containing the attributes for this object.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetClassName
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetClassName
```
Returns the class name of this instance of a component.
_returns: 
            The class name of the object, or null if the class does not have a name.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetComponentName
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetComponentName
```
Returns the name of this instance of a component.
_returns: 
            The name of the object, or null if the object does not have a name.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetConverter
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetConverter
```
Returns a type converter for this instance of a component.
_returns: 
            A @"T:System.ComponentModel.TypeConverter" that is the converter for this object, or null if there is no @"T:System.ComponentModel.TypeConverter" for this object.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetDefaultEvent
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetDefaultEvent
```
Returns the default event for this instance of a component.
_returns: 
            An @"T:System.ComponentModel.EventDescriptor" that represents the default event for this object, or null if this object does not have events.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetDefaultProperty
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetDefaultProperty
```
Returns the default property for this instance of a component.
_returns: 
            A @"T:System.ComponentModel.PropertyDescriptor" that represents the default property for this object, or null if this object does not have properties.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetEditor
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetEditor(System.Type)
```
Returns an editor of the specified type for this instance of a component.

|Parameter Name|Remarks|
|--------------|-------|
|editorBaseType|A @"T:System.Type" that represents the editor for this object.|

_returns: 
            An @"T:System.Object" of the specified type that is the editor for this object, or null if the editor cannot be found.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetEvents
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetEvents
```
Returns the events for this instance of a component.
_returns: 
            An @"T:System.ComponentModel.EventDescriptorCollection" that represents the events for this component instance.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetProperties
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetProperties(System.Attribute[])
```
Returns the properties for this instance of a component using the attribute array as a filter.

|Parameter Name|Remarks|
|--------------|-------|
|attributes|An array of type @"T:System.Attribute" that is used as a filter.|

_returns: 
            A @"T:System.ComponentModel.PropertyDescriptorCollection" that represents the filtered properties for this component instance.
            _

#### System#ComponentModel#ICustomTypeDescriptor#GetPropertyOwner
```csharp
Newtonsoft.Json.Linq.JObject.System#ComponentModel#ICustomTypeDescriptor#GetPropertyOwner(System.ComponentModel.PropertyDescriptor)
```
Returns an object that contains the property described by the specified property descriptor.

|Parameter Name|Remarks|
|--------------|-------|
|pd|A @"T:System.ComponentModel.PropertyDescriptor" that represents the property whose owner is to be found.|

_returns: 
            An @"T:System.Object" that represents the owner of the specified property.
            _

#### TryGetValue
```csharp
Newtonsoft.Json.Linq.JObject.TryGetValue(System.String,Newtonsoft.Json.Linq.JToken@)
```
Tries the get value.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|
|value|The value.|

_returns: true if a value was successfully retrieved; otherwise, false._

#### WriteTo
```csharp
Newtonsoft.Json.Linq.JObject.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### ChildrenTokens
Gets the container's children tokens.
#### Item
Gets or sets the @"T:Newtonsoft.Json.Linq.JToken" with the specified property name.
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
