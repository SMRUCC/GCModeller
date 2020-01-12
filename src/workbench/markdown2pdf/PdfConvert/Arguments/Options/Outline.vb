Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Arguments

    Public Class Outline

        ''' <summary>
        ''' Dump the default TOC xsl style sheet to stdout
        ''' </summary>
        ''' <returns></returns>
        <Argv("--dump-default-toc-xsl", CLITypes.Boolean)>
        Public Property dumpdefaulttocxsl As Boolean

        ''' <summary>
        ''' Dump the outline to a file
        ''' </summary>
        ''' <returns></returns>
        <Argv("--dump-outline", CLITypes.File)>
        Public Property dumpoutline As String

        ''' <summary>
        ''' Do not put an outline into the pdf
        ''' </summary>
        ''' <returns></returns>
        <Argv("--no-outline", CLITypes.Boolean)>
        Public Property nooutline As Boolean

        ''' <summary>
        ''' Set the depth of the outline (default 4)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--outline-depth", CLITypes.Integer)>
        Public Property outlinedepth As Integer?
    End Class
End Namespace