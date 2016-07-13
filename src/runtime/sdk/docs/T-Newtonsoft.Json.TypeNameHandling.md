---
title: TypeNameHandling
---

# TypeNameHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies type name handling options for the @"T:Newtonsoft.Json.JsonSerializer".

> 
>             @"T:Newtonsoft.Json.TypeNameHandling" should be used with caution when your application deserializes JSON from an external source.
>             Incoming types should be validated with a custom @"T:System.Runtime.Serialization.SerializationBinder"
>             when deserializing with a value other than TypeNameHandling.None.
>             



### Properties

#### All
Always include the .NET type name when serializing.
#### Arrays
Include the .NET type name when serializing into a JSON array structure.
#### Auto
Include the .NET type name when the type of the object being serialized is not the same as its declared type.
#### None
Do not include the .NET type name when serializing types.
#### Objects
Include the .NET type name when serializing into a JSON object structure.
