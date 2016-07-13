---
title: grImage
---

# grImage
_namespace: [RDotNET.Extensions.VisualBasic.grDevices](N-RDotNET.Extensions.VisualBasic.grDevices.html)_

Graphics devices for BMP, JPEG, PNG and TIFF format bitmap files.

> 
>  Plots in PNG and JPEG format can easily be converted to many other bitmap formats, and both can be displayed in modern web browsers. The PNG format is lossless and is best for line diagrams and blocks of colour. The JPEG format is lossy, but may be useful for image plots, for example. The BMP format is standard on Windows, and supported by most viewers elsewhere. TIFF is a meta-format: the default format written by tiff is lossless and stores RGB values uncompressed—such files are widely accepted, which is their main virtue over PNG.
>  Windows GDI imposes limits On the size Of bitmaps: these are Not documented In the SDK And may depend On the version Of Windows. It seems that width And height are Each limited To 2^15-1. In addition, there are limits On the total number Of pixels which depend On the graphics hardware.
>  By Default no resolution Is recorded In the file (except For BMP). Viewers will often assume a nominal resolution Of 72 ppi When none Is recorded. As resolutions In PNG files are recorded In pixels/metre, the reported ppi value will be changed slightly.
>  For graphics parameters that make use of dimensions in inches, res ppi (default 72) Is assumed.
>  Both bmp And png will use a palette If there are fewer than 256 colours On the page, And record a 24-bit RGB file otherwise. For the png device, type = "cairo" does the PNG output In the driver And so Is compatible With the "windows" type. type = "cairo-png" uses cairographics' PNG backend which will never use a palette and normally creates a larger 32-bit ARGB file—this may work better for specialist uses with semi-transparent colours.
>  png(type = "windows") supports transparent backgrounds on 16-bit ('High Color’) or better screens: use bg = "transparent". There is also support for semi-transparent colours of lines, fills and text. However, as there is only partial support for transparency in the graphics toolkit used: if there is a transparent background semi-transparent colours are painted onto a slightly off-white background and hence the pixels are opaque.
>  Not all PNG viewers render files with transparency correctly.
>  tiff compression types "lzw+p" And "zip+p" use horizontal differencing ('differencing predictor’, section 14 of the TIFF specification) in combination with the compression method, which is effective for continuous-tone images, especially colour ones.
>  Prior to R 3.0.3 unknown resolutions in BMP files were sometimes recorded incorrectly: they are now recorded As 72 ppi.
>  



### Properties

#### antialias
Length-one character vector.
 For allowed values And their effect on fonts with type = "windows" see windows: For that type if the argument Is missing the default Is taken from windows.options()$bitmap.aa.win.
 For allowed values And their effect (on fonts And lines, but Not fills) with type = "cairo" see svg.
#### filename
the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). (The result must be less than 600 characters long. See postscript for further details.)
#### pointsize
the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.
#### res
The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. If not specified, taken as 72 ppi to set the size of text and line widths.
#### restoreConsole
See the ‘Details’ section of windows. For type == "windows" only.
#### type
Should be plotting be done using Windows GDI or cairographics?
#### units
The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.
