---
title: MemberSerialization
---

# MemberSerialization
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies the member serialization options for the @"T:Newtonsoft.Json.JsonSerializer".




### Properties

#### Fields
All public and private fields are serialized. Members can be excluded using @"T:Newtonsoft.Json.JsonIgnoreAttribute" or @"T:System.NonSerializedAttribute".
 This member serialization mode can also be set by marking the class with @"T:System.SerializableAttribute"
 and setting IgnoreSerializableAttribute on @"T:Newtonsoft.Json.Serialization.DefaultContractResolver" to false.
#### OptIn
Only members marked with @"T:Newtonsoft.Json.JsonPropertyAttribute" or @"T:System.Runtime.Serialization.DataMemberAttribute" are serialized.
 This member serialization mode can also be set by marking the class with @"T:System.Runtime.Serialization.DataContractAttribute".
#### OptOut
All public members are serialized by default. Members can be excluded using @"T:Newtonsoft.Json.JsonIgnoreAttribute" or @"T:System.NonSerializedAttribute".
 This is the default member serialization mode.
