Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Arguments

    Public Class GlobalOptions

        ''' <summary>
        ''' Do not collate when printing multiple copies
        ''' </summary>
        ''' <returns></returns>
        <Argv("--no-collate", CLITypes.Boolean)>
        Public Property nocollate As Boolean

        ''' <summary>
        ''' Read and write cookies from and to the supplied cookie jar file
        ''' </summary>
        ''' <returns></returns>
        <Argv("--cookie-jar", CLITypes.File)>
        Public Property cookiejar As String

        ''' <summary>
        ''' Number of copies to print into the pdf file (default 1)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--copies", CLITypes.Integer)>
        Public Property copies As Integer?

        ''' <summary>
        ''' Change the dpi explicitly (this has no effect on X11 based systems) 
        ''' (default 96)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--dpi", CLITypes.Integer)>
        Public Property dpi As Integer?

        ''' <summary>
        ''' PDF will be generated in grayscale
        ''' </summary>
        ''' <returns></returns>
        <Argv("--grayscale", CLITypes.Boolean)>
        Public Property grayscale As Boolean = False

        ''' <summary>
        ''' When embedding images scale them down to this dpi(default 600)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--image-dpi", CLITypes.Integer)>
        Public Property imagedpi As Integer?

        ''' <summary>
        ''' When jpeg compressing images use this quality (default 94)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--image-quality", CLITypes.Integer)>
        Public Property imagequality As Integer?

        ''' <summary>
        ''' Generates lower quality pdf/ps. Useful to shrink the result document space
        ''' </summary>
        ''' <returns></returns>
        <Argv("--lowquality", CLITypes.Boolean)>
        Public Property lowquality As Boolean = False

        ''' <summary>
        ''' Set the page bottom margin
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-bottom", CLITypes.String)>
        Public Property marginbottom As String

        ''' <summary>
        ''' Set the page left margin (default 10mm)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-left", CLITypes.String)>
        Public Property marginleft As String

        ''' <summary>
        ''' Set the page right margin (default 10mm)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-right", CLITypes.String)>
        Public Property marginright As String

        ''' <summary>
        ''' Set the page top margin
        ''' </summary>
        ''' <returns></returns>
        <Argv("--margin-top", CLITypes.String)>
        Public Property margintop As String

        ''' <summary>
        ''' Set orientation to Landscape or Portrait (default Portrait)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--orientation", CLITypes.String)>
        Public Property orientation As Orientations?

        ''' <summary>
        ''' Do not use lossless compression on pdf objects
        ''' </summary>
        ''' <returns></returns>
        <Argv("--no-pdf-compression", CLITypes.Boolean)>
        Public Property nopdfcompression As Boolean

        ''' <summary>
        ''' The title of the generated pdf file (The title of the first document 
        ''' Is used if Not specified)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--title", CLITypes.String)>
        Public Property title As String
    End Class

    Public Enum Orientations
        Portrait
        Landscape
    End Enum
End Namespace