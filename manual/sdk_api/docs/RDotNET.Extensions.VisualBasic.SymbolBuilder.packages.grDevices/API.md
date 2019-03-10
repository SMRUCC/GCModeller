﻿# API
_namespace: [RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.grDevices](./index.md)_





### Methods

#### rgb
```csharp
RDotNET.Extensions.VisualBasic.SymbolBuilder.packages.grDevices.API.rgb(System.Double,System.Double,System.Double,System.Double,System.String,System.Int32)
```
RGB Color Specification
 
 This function creates colors corresponding to the given intensities (between 0 and max) of the red, green and blue primaries. The colour specification refers to the standard sRGB colorspace (IEC standard 61966).
 An alpha transparency value can also be specified (As an opacity, so 0 means fully transparent And max means opaque). If alpha Is Not specified, an opaque colour Is generated.
 The names argument may be used To provide names For the colors.
 The values returned by these functions can be used With a col= specification In graphics functions Or In par.

|Parameter Name|Remarks|
|--------------|-------|
|red|numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.|
|green|numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.|
|blue|numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.|
|alpha|numeric vectors with values in [0, M] where M is maxColorValue. When this is 255, the red, blue, green, and alpha values are coerced to integers in 0:255 and the result is computed most efficiently.|
|names|character vector. The names for the resulting vector.|
|maxColorValue|number giving the maximum of the color values range, see above.|


_returns: A character vector with elements of 7 or 9 characters, "#" followed by the red, blue, green and optionally alpha values in hexadecimal (after rescaling to 0 ... 255). The optional alpha values range from 0 (fully transparent) to 255 (opaque).
 R does Not use 'premultiplied alpha’._
> 
>  The colors may be specified by passing a matrix or data frame as argument red, and leaving blue and green missing. In this case the first three columns of red are taken to be the red, green and blue values.
>  Semi-transparent colors (0 < alpha < 1) are supported only on some devices: at the time Of writing On the pdf, windows, quartz And X11(type = "cairo") devices And associated bitmap devices (jpeg, png, bmp, tiff And bitmap). They are supported by several third-party devices such As those In packages Cairo, cairoDevice And JavaGD. Only some Of these devices support semi-transparent backgrounds.
>  Most other graphics devices plot semi-transparent colors As fully transparent, usually With a warning When first encountered.
>  NA values are Not allowed For any Of red, blue, green Or alpha.
>  


