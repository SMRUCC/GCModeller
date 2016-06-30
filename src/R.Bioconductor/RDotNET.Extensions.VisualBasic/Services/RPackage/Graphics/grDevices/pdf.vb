Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNET.Extensions.VisualBasic.base.Control

Namespace grDevices

    ''' <summary>
    ''' pdf starts the graphics device driver for producing PDF graphics.
    ''' </summary>
    ''' <remarks>
    ''' All arguments except file default to values given by pdf.options(). The ultimate defaults are quoted in the arguments section.
    ''' pdf() opens the file file And the PDF commands needed to plot any graphics requested are sent to that file.
    ''' The file argument Is interpreted As a C Integer format As used by sprintf, With Integer argument the page number. The Default gives files 'Rplot001.pdf’, ..., ‘Rplot999.pdf’, ‘Rplot1000.pdf’, ....
    ''' The family argument can be used To specify a PDF-specific font family As the initial/Default font For the device. If additional font families are To be used they should be included In the fonts argument.
    ''' If a device-independent R graphics font family Is specified (e.g., via par(family = ) In the graphics package), the PDF device makes use Of the PostScript font mappings To convert the R graphics font family To a PDF-specific font family description. (See the documentation For pdfFonts.)
    ''' This device does Not embed fonts In the PDF file, so it Is only straightforward To use mappings To the font families that can be assumed To be available In any PDF viewer: "Times" (equivalently "serif"), "Helvetica" (equivalently "sans") And "Courier" (equivalently "mono"). Other families may be specified, but it Is the user's responsibility to ensure that these fonts are available on the system and third-party software (e.g., Ghostscript) may be required to embed the fonts so that the PDF can be included in other documents (e.g., LaTeX): see embedFonts. The URW-based families described for postscript can be used with viewers such as GSView which utilise URW fonts. Since embedFonts makes use of Ghostscript, it should be able to embed the URW-based families for use with other viewers.
    ''' See postscript For details Of encodings, As the internal code Is Shared between the drivers. The native PDF encoding Is given In file 'PDFDoc.enc’.
    ''' The PDF produced Is fairly simple, With Each page being represented As a Single stream (by Default compressed And possibly With references To raster images). The R graphics model does Not distinguish graphics objects at the level Of the driver Interface.
    ''' The version argument declares the version Of PDF that gets produced. The version must be at least 1.2 When compression Is used, 1.4 For semi-transparent output To be understood, And at least 1.3 If CID fonts are To be used: If any Of these features are used the version number will be increased (With a warning). (PDF 1.4 was first supported by Acrobat 5 In 2001; it Is very unlikely Not To be supported In a current viewer.)
    ''' Line widths As controlled by par(lwd = ) are In multiples Of 1/96 inch. Multiples less than 1 are allowed. pch = "." With cex = 1 corresponds To a square Of side 1/72 inch, which Is also the 'pixel’ size assumed for graphics parameters such as "cra".
    ''' The paper argument sets the /MediaBox entry In the file, which defaults To width by height. If it Is Set To something other than "special", a device region Of the specified size Is (by Default) centred On the rectangle given by the paper size: If either width Or height Is less than 0.1 Or too large To give a total margin Of 0.5 inch, it Is reset To the corresponding paper dimension minus 0.5. Thus If you want the Default behaviour Of postscript use pdf(paper = "a4r", width = 0, height = 0) To centre the device region On a landscape A4 page With 0.25 inch margins.
    ''' When the background colour Is fully transparent (as Is the initial default value), the PDF produced does Not paint the background. Most PDF viewers will use a white canvas so the visual effect Is if the background were white. This will Not be the case when printing onto coloured paper, though.
    ''' </remarks>
    Public Class pdf : Inherits grDevice

        ''' <summary>
        ''' a character string giving the name of the file. If it is of the form "|cmd", the output is piped to the command given by cmd. If it is NULL, then no external file is created (effectively, no drawing occurs), but the device may still be queried (e.g., for size of text).
        ''' For use with onefile = FALSE give a C integer format such as "Rplot%03d.pdf" (the default in that case). (See postscript for further details.)
        ''' Tilde expansion(see path.expand) Is done.
        ''' </summary>
        ''' <returns></returns>
        Public Property file As RExpression = ifelse("onefile", Rstring("Rplots.pdf"), Rstring("Rplot%03d.pdf"))
        ''' <summary>
        ''' logical: if true (the default) allow multiple figures in one file. If false, generate a file with name containing the page number for each page. 
        ''' Defaults to TRUE, and forced to true if file is a pipe.
        ''' </summary>
        ''' <returns></returns>
        Public Property onefile As Boolean
        ''' <summary>
        ''' title string to embed as the /Title field in the file. Defaults to "R Graphics Output".
        ''' </summary>
        ''' <returns></returns>
        Public Property title As String
        ''' <summary>
        ''' a character vector specifying R graphics font family names for additional fonts which will be included in the PDF file. Defaults to NULL.
        ''' </summary>
        ''' <returns></returns>
        Public Property fonts As String
        ''' <summary>
        ''' a string describing the PDF version that will be required to view the output. 
        ''' This is a minimum, and will be increased (with a warning) if necessary. Defaults to "1.4", but see ‘Details’.
        ''' </summary>
        ''' <returns></returns>
        Public Property version As String
        ''' <summary>
        ''' the target paper size. The choices are "a4", "letter", "legal" (or "us") and "executive" (and these can be capitalized), or "a4r" and "USr" for rotated (‘landscape’). 
        ''' The default is "special", which means that the width and height specify the paper size. 
        ''' A further choice is "default"; if this is selected, the papersize is taken from the option "papersize" if that is set and as "a4" if it is unset or empty. 
        ''' Defaults to "special".
        ''' </summary>
        ''' <returns></returns>
        Public Property paper As String
        ''' <summary>
        ''' the name of an encoding file. See postscript for details. Defaults to "default".
        ''' </summary>
        ''' <returns></returns>
        Public Property encoding As String
        ''' <summary>
        ''' the initial foreground color to be used. Defaults to "black".
        ''' </summary>
        ''' <returns></returns>
        Public Property fg As String
        ''' <summary>
        ''' the default point size to be used. Strictly speaking, in bp, that is 1/72 of an inch, but approximately in points. Defaults to 12.
        ''' </summary>
        ''' <returns></returns>
        Public Property pointsize As Double
        ''' <summary>
        ''' logical: should the device region be centred on the page? – is only relevant for paper != "special". Defaults to TRUE.
        ''' </summary>
        ''' <returns></returns>
        Public Property pagecentre As Boolean
        ''' <summary>
        ''' a character string describing the color model: currently allowed values are "srgb", "gray" (or "grey") and "cmyk". Defaults to "srgb". See section ‘Color models’.
        ''' </summary>
        ''' <returns></returns>
        Public Property colormodel As String
        ''' <summary>
        ''' logical. Should small circles be rendered via the Dingbats font? Defaults to TRUE, which produces smaller and better output. 
        ''' Setting this to FALSE can work around font display problems in broken PDF viewers: although this font is one of the 14 guaranteed to be available in all PDF viewers, that guarantee is not always honoured.
        ''' </summary>
        ''' <returns></returns>
        Public Property useDingbats As Boolean
        ''' <summary>
        ''' logical. Should kerning corrections be included in setting text and calculating string widths? Defaults to TRUE.
        ''' </summary>
        ''' <returns></returns>
        Public Property useKerning As Boolean
        ''' <summary>
        ''' logical controlling the polygon fill mode: see polygon for details. Defaults to FALSE.
        ''' </summary>
        ''' <returns></returns>
        Public Property fillOddEven As Boolean
        ''' <summary>
        ''' logical. Should PDF streams be generated with Flate compression? Defaults to TRUE.
        ''' </summary>
        ''' <returns></returns>
        Public Property compress As Boolean
    End Class
End Namespace