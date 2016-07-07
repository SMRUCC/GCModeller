---
title: GraphicsDevice
---

# GraphicsDevice
_namespace: [RDotNET.Extensions.VisualBasic.Graphics](N-RDotNET.Extensions.VisualBasic.Graphics.html)_

Graphics devices for BMP, JPEG, PNG and TIFF format bitmap files.



### Methods

#### bmp
```csharp
RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.bmp(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|plot|画图的语句|
|filename|the name Of the output file, up To 511 characters. The page number Is substituted If a C Integer format Is included In the character String, As In the Default, And tilde-expansion Is performed (see path.expand). (The result must be less than 600 characters Long. See postscript For further details.) |
|width|-|
|height|-|
|units|-|
|pointsize|-|
|bg|-|
|res|-|
|family|-|
|restoreConsole|-|


#### jpeg
```csharp
RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.jpeg(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|plot|画图的语句|
|filename|-|
|width|-|
|height|-|
|units|-|
|pointsize|-|
|quality|-|
|bg|-|
|res|-|
|family|-|
|restoreConsole|-|


#### png
```csharp
RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.png(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|plot|画图的语句|
|filename|-|
|width|-|
|height|-|
|units|-|
|pointsize|-|
|bg|-|
|res|-|
|family|-|
|restoreConsole|-|


#### tiff
```csharp
RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.tiff(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.String,System.Boolean,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|plot|画图的语句|
|filename|-|
|width|-|
|height|-|
|units|-|
|pointsize|-|
|bg|-|
|res|-|
|family|-|
|restoreConsole|-|


#### WriteImage
```csharp
RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.WriteImage(System.String)
```
Execute the statement that comes from the function @"M:RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.bmp(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)",
 @"M:RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.jpeg(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)",
 @"M:RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.png(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.Boolean,System.String)",
 @"M:RDotNET.Extensions.VisualBasic.Graphics.GraphicsDevice.tiff(System.String,System.String,System.Int32,System.Int32,System.String,System.Int32,System.String,System.String,System.String,System.String,System.Boolean,System.String)"

|Parameter Name|Remarks|
|--------------|-------|
|plot|-|



