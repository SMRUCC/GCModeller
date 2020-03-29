#Region "Microsoft.VisualBasic::010f16cbe3be0d5cda485287073bff1e, markdown2pdf\PdfConvert\Arguments\Options\Outline.vb"

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

    '     Class Outline
    ' 
    '         Properties: dumpdefaulttocxsl, dumpoutline, nooutline, outlinedepth
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
