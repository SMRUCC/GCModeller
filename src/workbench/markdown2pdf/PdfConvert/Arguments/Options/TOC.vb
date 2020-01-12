Imports Microsoft.VisualBasic.CommandLine.Reflection

Namespace Arguments

    ''' <summary>
    ''' 目录设置参数
    ''' </summary>
    Public Class TOC

        ''' <summary>
        ''' The header text of the toc (default Table of Contents)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--toc-header-text", CLITypes.String)>
        Public Property tocheadertext As String

        ''' <summary>
        ''' For each level of headings in the toc indent by this length (Default 1em)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--toc-level-indentation", CLITypes.String)>
        Public Property toclevelindentation As String

        ''' <summary>
        ''' Do not use dotted lines in the toc
        ''' </summary>
        ''' <returns></returns>
        <Argv("--disable-dotted-lines", CLITypes.Boolean)>
        Public Property disabledottedlines As Boolean

        ''' <summary>
        ''' Do not link from toc to sections
        ''' </summary>
        ''' <returns></returns>
        <Argv("--disable-toc-links", CLITypes.Boolean)>
        Public Property disabletoclinks As Boolean

        ''' <summary>
        ''' For each level of headings in the toc the font Is scaled by this factor (default 0.8)
        ''' </summary>
        ''' <returns></returns>
        <Argv("--toc-text-size-shrink", CLITypes.Double)>
        Public Property toctextsizeshrink As Double?

        ''' <summary>
        ''' Use the supplied xsl style sheet for printing the table Of content
        ''' </summary>
        ''' <returns></returns>
        <Argv("--xsl-style-sheet", CLITypes.File)>
        Public Property xslstylesheet As String

    End Class
End Namespace