---
title: DefaultValueHandling
---

# DefaultValueHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies default value handling options for the @"T:Newtonsoft.Json.JsonSerializer".




### Properties

#### Ignore
Ignore members where the member value is the same as the member's default value when serializing objects
 so that is is not written to JSON.
 This option will ignore all default values (e.g. null for objects and nullable types; 0 for integers,
 decimals and floating point numbers; and false for booleans). The default value ignored can be changed by
 placing the @"T:System.ComponentModel.DefaultValueAttribute" on the property.
#### IgnoreAndPopulate
Ignore members where the member value is the same as the member's default value when serializing objects
 and sets members to their default value when deserializing.
#### Include
Include members where the member value is the same as the member's default value when serializing objects.
 Included members are written to JSON. Has no effect when deserializing.
#### Populate
Members with a default value but no JSON will be set to their default value when deserializing.
