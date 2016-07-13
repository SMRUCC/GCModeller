---
title: Color
---

# Color
_namespace: [RDotNET.Graphics](N-RDotNET.Graphics.html)_

32-bit color of ABGR model.



### Methods

#### FromArgb
```csharp
RDotNET.Graphics.Color.FromArgb(System.Byte,System.Byte,System.Byte,System.Byte)
```
Gets a color from bytes.

|Parameter Name|Remarks|
|--------------|-------|
|alpha|Alpha.|
|red|Red.|
|green|Green.|
|blue|Blue.|

_returns: The color._

#### FromRgb
```csharp
RDotNET.Graphics.Color.FromRgb(System.Byte,System.Byte,System.Byte)
```
Gets a color from bytes.

|Parameter Name|Remarks|
|--------------|-------|
|red|Red.|
|green|Green.|
|blue|Blue.|

_returns: The color._

#### FromUInt32
```csharp
RDotNET.Graphics.Color.FromUInt32(System.UInt32)
```
Gets a color from 32-bit value.

|Parameter Name|Remarks|
|--------------|-------|
|rgba|UInt32.|

_returns: The color._


### Properties

#### Alpha
Gets and sets the alpha value.
#### Blue
Gets and sets the blue value.
#### Green
Gets and sets the green value.
#### IsTransparent
Gets whether the point is transparent.
#### Opaque
Gets the opaque value.
#### Red
Gets and sets the red value.
