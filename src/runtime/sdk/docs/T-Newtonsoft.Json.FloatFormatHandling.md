---
title: FloatFormatHandling
---

# FloatFormatHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies float format handling options when writing special floating point numbers, e.g. @"F:System.Double.NaN",
 @"F:System.Double.PositiveInfinity" and @"F:System.Double.NegativeInfinity" with @"T:Newtonsoft.Json.JsonWriter".




### Properties

#### DefaultValue
Write special floating point values as the property's default value in JSON, e.g. 0.0 for a @"T:System.Double" property, null for a @"T:System.Nullable`1" property.
#### String
Write special floating point values as strings in JSON, e.g. "NaN", "Infinity", "-Infinity".
#### Symbol
Write special floating point values as symbols in JSON, e.g. NaN, Infinity, -Infinity.
 Note that this will produce non-valid JSON.
