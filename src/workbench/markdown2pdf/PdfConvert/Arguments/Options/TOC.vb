#Region "Microsoft.VisualBasic::a83042c91f66cfd4aec36c518fd5dc7a, markdown2pdf\PdfConvert\Arguments\Options\TOC.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class TOC
    ' 
    '         Properties: disabledottedlines, disabletoclinks, tocheadertext, toclevelindentation, toctextsizeshrink
    '                     xslstylesheet
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
