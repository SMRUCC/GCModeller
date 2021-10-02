#Region "Microsoft.VisualBasic::0dc0430bfb0c2a1b017b9363a1b3a89c, markdown2pdf\Markdown2PDF\CLI.vb"

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

    ' Module CLI
    ' 
    '     Function: glob
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.text.markdown
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

Module CLI

    <ExportAPI("/glob")>
    <Usage("/glob /src <dir of *.md> [/pdf <output.pdf>]")>
    Public Function glob(args As CommandLine) As Integer
        Dim src$ = args <= "/src"
        Dim pdf$ = args("/pdf") Or $"{src.TrimDIR}.pdf"
        Dim render As New MarkdownHTML()
        Dim tmp As String = TempFileSystem.GetAppSysTempFile

        Call src.FileCopy(tmp & "/")

        Dim mdlist As String() = tmp _
            .ListFiles("*.md") _
            .OrderBy(Function(path)
                         Return path.BaseName
                     End Function) _
            .ToArray

        For Each file As String In mdlist
            Call file _
                .ReadAllText _
                .DoCall(AddressOf render.Transform) _
                .SaveTo(file.ChangeSuffix("html"))
        Next

        Dim srcDoc As New PdfDocument With {
            .Url = mdlist _
                .Select(Function(file)
                            Return file.ChangeSuffix("html")
                        End Function) _
                .ToArray
        }

        Call PdfConvert.ConvertHtmlToPdf(
            document:=srcDoc,
            output:=New PdfOutput With {
                .OutputFilePath = pdf
            }
        )

        Return 0
    End Function
End Module

