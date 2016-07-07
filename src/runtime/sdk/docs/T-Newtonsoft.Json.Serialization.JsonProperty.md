---
title: JsonProperty
---

# JsonProperty
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Maps a JSON property to a .NET member or constructor parameter.



### Methods

#### ToString
```csharp
Newtonsoft.Json.Serialization.JsonProperty.ToString
```
Returns a @"T:System.String" that represents this instance.
_returns: 
            A @"T:System.String" that represents this instance.
            _


### Properties

#### AttributeProvider
Gets or sets the @"T:Newtonsoft.Json.Serialization.IAttributeProvider" for this property.
#### Converter
Gets or sets the for the property.
 If set this converter takes presidence over the contract converter for the property type.
#### DeclaringType
Gets or sets the type that declared this property.
#### DefaultValue
Gets the default value.
#### DefaultValueHandling
Gets or sets the property default value handling.
#### GetIsSpecified
Gets or sets a predicate used to determine whether the property should be serialized.
#### HasMemberAttribute
Gets or sets a value indicating whether this @"T:Newtonsoft.Json.Serialization.JsonProperty" has a member attribute.
#### Ignored
Gets or sets a value indicating whether this @"T:Newtonsoft.Json.Serialization.JsonProperty" is ignored.
#### IsReference
Gets or sets a value indicating whether this property preserves object references.
#### ItemConverter
Gets or sets the converter used when serializing the property's collection items.
#### ItemIsReference
Gets or sets whether this property's collection items are serialized as a reference.
#### ItemReferenceLoopHandling
Gets or sets the the reference loop handling used when serializing the property's collection items.
#### ItemTypeNameHandling
Gets or sets the the type name handling used when serializing the property's collection items.
#### MemberConverter
Gets or sets the member converter.
#### NullValueHandling
Gets or sets the property null value handling.
#### ObjectCreationHandling
Gets or sets the property object creation handling.
#### Order
Gets or sets the order of serialization of a member.
#### PropertyName
Gets or sets the name of the property.
#### PropertyType
Gets or sets the type of the property.
#### Readable
Gets or sets a value indicating whether this @"T:Newtonsoft.Json.Serialization.JsonProperty" is readable.
#### ReferenceLoopHandling
Gets or sets the property reference loop handling.
#### Required
Gets or sets a value indicating whether this @"T:Newtonsoft.Json.Serialization.JsonProperty" is required.
#### SetIsSpecified
Gets or sets an action used to set whether the property has been deserialized.
#### ShouldDeserialize
Gets or sets a predicate used to determine whether the property should be deserialized.
#### ShouldSerialize
Gets or sets a predicate used to determine whether the property should be serialize.
#### TypeNameHandling
Gets or sets or sets the type name handling.
#### UnderlyingName
Gets or sets the name of the underlying member or parameter.
#### ValueProvider
Gets the @"T:Newtonsoft.Json.Serialization.IValueProvider" that will get and set the @"T:Newtonsoft.Json.Serialization.JsonProperty" during serialization.
#### Writable
Gets or sets a value indicating whether this @"T:Newtonsoft.Json.Serialization.JsonProperty" is writable.
