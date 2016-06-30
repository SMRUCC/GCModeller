Imports System.Text
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace grDevices

    ''' <summary>
    ''' Graphics devices for BMP, JPEG, PNG and TIFF format bitmap files.
    ''' </summary>
    ''' <remarks>
    ''' Plots in PNG and JPEG format can easily be converted to many other bitmap formats, and both can be displayed in modern web browsers. The PNG format is lossless and is best for line diagrams and blocks of colour. The JPEG format is lossy, but may be useful for image plots, for example. The BMP format is standard on Windows, and supported by most viewers elsewhere. TIFF is a meta-format: the default format written by tiff is lossless and stores RGB values uncompressed—such files are widely accepted, which is their main virtue over PNG.
    ''' Windows GDI imposes limits On the size Of bitmaps: these are Not documented In the SDK And may depend On the version Of Windows. It seems that width And height are Each limited To 2^15-1. In addition, there are limits On the total number Of pixels which depend On the graphics hardware.
    ''' By Default no resolution Is recorded In the file (except For BMP). Viewers will often assume a nominal resolution Of 72 ppi When none Is recorded. As resolutions In PNG files are recorded In pixels/metre, the reported ppi value will be changed slightly.
    ''' For graphics parameters that make use of dimensions in inches, res ppi (default 72) Is assumed.
    ''' Both bmp And png will use a palette If there are fewer than 256 colours On the page, And record a 24-bit RGB file otherwise. For the png device, type = "cairo" does the PNG output In the driver And so Is compatible With the "windows" type. type = "cairo-png" uses cairographics' PNG backend which will never use a palette and normally creates a larger 32-bit ARGB file—this may work better for specialist uses with semi-transparent colours.
    ''' png(type = "windows") supports transparent backgrounds on 16-bit ('High Color’) or better screens: use bg = "transparent". There is also support for semi-transparent colours of lines, fills and text. However, as there is only partial support for transparency in the graphics toolkit used: if there is a transparent background semi-transparent colours are painted onto a slightly off-white background and hence the pixels are opaque.
    ''' Not all PNG viewers render files with transparency correctly.
    ''' tiff compression types "lzw+p" And "zip+p" use horizontal differencing ('differencing predictor’, section 14 of the TIFF specification) in combination with the compression method, which is effective for continuous-tone images, especially colour ones.
    ''' Prior to R 3.0.3 unknown resolutions in BMP files were sometimes recorded incorrectly: they are now recorded As 72 ppi.
    ''' </remarks>
    Public MustInherit Class grImage : Inherits grDevice

        ''' <summary>
        ''' the name of the output file, up to 511 characters. The page number is substituted if a C integer format is included in the character string, as in the default, and tilde-expansion is performed (see path.expand). (The result must be less than 600 characters long. See postscript for further details.)
        ''' </summary>
        ''' <returns></returns>
        <Parameter("filename", ValueTypes.Path)> Public Property filename As String = "Rplot%03d.bmp"

        ''' <summary>
        ''' The units in which height and width are given. Can be px (pixels, the default), in (inches), cm or mm.
        ''' </summary>
        ''' <returns></returns>
        Public Property units As String = "px"
        ''' <summary>
        ''' the default pointsize of plotted text, interpreted as big points (1/72 inch) at res ppi.
        ''' </summary>
        ''' <returns></returns>
        Public Property pointsize As Integer = 12
        ''' <summary>
        ''' The nominal resolution in ppi which will be recorded in the bitmap file, if a positive integer. Also used for units other than the default. If not specified, taken as 72 ppi to set the size of text and line widths.
        ''' </summary>
        ''' <returns></returns>
        Public Property res As RExpression = NA

        ''' <summary>
        ''' See the ‘Details’ section of windows. For type == "windows" only.
        ''' </summary>
        ''' <returns></returns>
        Public Property restoreConsole As Boolean = True
        ''' <summary>
        ''' Should be plotting be done using Windows GDI or cairographics?
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Property type As RExpression = c("windows", "cairo")
        ''' <summary>
        ''' Length-one character vector.
        ''' For allowed values And their effect on fonts with type = "windows" see windows: For that type if the argument Is missing the default Is taken from windows.options()$bitmap.aa.win.
        ''' For allowed values And their effect (on fonts And lines, but Not fills) with type = "cairo" see svg.
        ''' </summary>
        ''' <returns></returns>
        Public Property antialias As RExpression
    End Class

    <RFunc("bmp")> Public Class bmp : Inherits grImage

        Sub New(file As String, Optional width As Integer = 1024, Optional height As Integer = 800)
            Me.filename = file
            Me.width = width
            Me.height = height
        End Sub

        Sub New()
        End Sub
    End Class

    <RFunc("jpeg")> Public Class jpeg : Inherits grImage

        ''' <summary>
        ''' the ‘quality’ of the JPEG image, as a percentage. Smaller values will give more compression but also more degradation of the image.
        ''' </summary>
        ''' <returns></returns>
        Public Property quality As Integer = 75

        Sub New(file As String, Optional width As Integer = 1024, Optional height As Integer = 800)
            Me.filename = file
            Me.width = width
            Me.height = height
        End Sub

        Sub New()
        End Sub
    End Class

    <RFunc("png")> Public Class png : Inherits grImage

        ''' <summary>
        ''' Should be plotting be done using Windows GDI or cairographics?
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property type As RExpression = c("windows", "cairo", "cairo-png")

        Sub New(file As String, Optional width As Integer = 1024, Optional height As Integer = 800)
            Me.filename = file
            Me.width = width
            Me.height = height
        End Sub

        Sub New()
        End Sub
    End Class

    <RFunc("tiff")> Public Class tiff : Inherits grImage

        ''' <summary>
        ''' the type Of compression To be used.
        ''' </summary>
        ''' <returns></returns>
        Public Property compression As RExpression = c("none", "rle", "lzw", "jpeg", "zip", "lzw+p", "zip+p")

        Sub New(file As String, Optional width As Integer = 1024, Optional height As Integer = 800)
            Me.filename = file
            Me.width = width
            Me.height = height
        End Sub

        Sub New()
        End Sub
    End Class
End Namespace
