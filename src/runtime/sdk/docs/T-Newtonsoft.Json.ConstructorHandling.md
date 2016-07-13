---
title: ConstructorHandling
---

# ConstructorHandling
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies how constructors are used when initializing objects during deserialization by the @"T:Newtonsoft.Json.JsonSerializer".




### Properties

#### AllowNonPublicDefaultConstructor
Json.NET will use a non-public default constructor before falling back to a parameterized constructor.
#### Default
First attempt to use the public default constructor, then fall back to single parameterized constructor, then the non-public default constructor.
