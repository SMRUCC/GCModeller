---
title: WriteState
---

# WriteState
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies the state of the @"T:Newtonsoft.Json.JsonWriter".




### Properties

#### Array
A array is being written.
#### Closed
The @"M:Newtonsoft.Json.JsonWriter.Close" method has been called.
#### Constructor
A constructor is being written.
#### Error
An exception has been thrown, which has left the @"T:Newtonsoft.Json.JsonWriter" in an invalid state.
 You may call the @"M:Newtonsoft.Json.JsonWriter.Close" method to put the @"T:Newtonsoft.Json.JsonWriter" in the Closed state.
 Any other @"T:Newtonsoft.Json.JsonWriter" method calls results in an @"T:System.InvalidOperationException" being thrown.
#### Object
An object is being written.
#### Property
A property is being written.
#### Start
A write method has not been called.
