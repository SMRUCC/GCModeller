#Region "Microsoft.VisualBasic::de96a9b0831cc1094b98243a49a42632, markdown2pdf\reportKit\pdf.vb"

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

    ' Module pdf
    ' 
    '     Function: GetContentHtml, makePDF
    ' 
    ' /********************************************************************************/

#End Region


Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.text.markdown
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports WkHtmlToPdf
Imports WkHtmlToPdf.Arguments

<Package("pdf", Category:=APICategories.UtilityTools)>
Module pdf

    <Extension>
    Private Iterator Function GetContentHtml(files As IEnumerable(Of String), wwwroot$, style$, resolvedAsDataUri As Boolean, [strict] As Boolean) As IEnumerable(Of String)
        Dim render As New MarkdownHTML
        Dim dir As String = App.CurrentDirectory

        wwwroot = wwwroot.GetDirectoryFullPath

        If Not style.StringEmpty Then
            If style.FileExists Then
                style = style.GetFullPath
            ElseIf $"{wwwroot}/{style}".FileExists Then
                style = $"{wwwroot}/{style}".GetFullPath
            Else
                Dim tmp As String = App.SysTemp & $"/pdf_styles_{App.PID.ToHexString}-{Now.ToString.MD5.ToLower}.css"

                style.SaveTo(tmp)
                style = tmp
            End If

            If resolvedAsDataUri Then
                style = New DataURI(style).ToString
            End If
        End If

        For Each file As String In files.SafeQuery
            If Not file.FileExists Then

                If strict Then
                    Throw New EntryPointNotFoundException($"missing source file: {file}!")
                Else
                    Call $"missing source file: {file}!".Warning
                End If

            ElseIf file.ExtensionSuffix("html") Then
                ' Yield RelativePath(dir, file.GetFullPath)
                Yield file.GetFullPath
            Else
                Dim htmlfile As String = file.GetFullPath.ChangeSuffix("html")
                Dim html As String = file _
                    .ReadAllText _
                    .DoCall(AddressOf render.Transform) _
                    .ResolveLocalFileLinks(relativeTo:=wwwroot, asDataUri:=True)

                If Not style.StringEmpty Then
                    html = sprintf(<html>
                                       <head>

                                           <link rel="stylesheet" href=<%= style %>/>

                                       </head>
                                       <body>%s</body>
                                   </html>, html)
                End If

                Call html.SaveTo(htmlfile)

                ' Yield RelativePath(dir, htmlfile)
                Yield htmlfile
            End If
        Next
    End Function

    ''' <summary>
    ''' convert the local html documents to pdf document.
    ''' </summary>
    ''' <param name="files">
    ''' markdown files or html files
    ''' </param>
    ''' <param name="pdfout"></param>
    <ExportAPI("makePDF")>
    <RApiReturn(GetType(String))>
    Public Function makePDF(files As String(),
                            Optional pdfout As String = "out.pdf",
                            Optional wwwroot As String = "/",
                            Optional style As String = Nothing,
                            Optional resolvedAsDataUri As Boolean = False,
                            Optional logo As String = Nothing,
                            Optional footer As String = Nothing,
                            Optional env As Environment = Nothing) As Object

        Dim [strict] As Boolean = env.globalEnvironment.options.strict
        Dim contentUrls As String() = files _
            .GetContentHtml(
                wwwroot:=wwwroot,
                style:=style,
                resolvedAsDataUri:=resolvedAsDataUri,
                strict:=strict
            ) _
            .ToArray

        If contentUrls.IsNullOrEmpty Then
            Return Internal.debug.stop("no pdf content files was found!", env)
        End If

        If (Not logo.StringEmpty) Then
            If (Not logo.ExtensionSuffix("html", "htm")) Then
                Dim tmp As String = App.SysTemp & $"/{App.PID.ToHexString}_logo{App.GetNextUniqueName("image_")}.html"
                Dim logoHtml As String = sprintf(
                    <div>
                        <img style="height: 100%;" src=<%= New DataURI(logo).ToString %>/>
                    </div>)

                logoHtml.SaveTo(tmp)
                logo = tmp.GetFullPath
            End If
        End If

        Dim content As New PdfDocument With {
            .Url = contentUrls,
            .footer = New Decoration With {.right = "[page] / [toPage]"},
            .header = New Decoration With {.html = logo, .spacing = 10}
        }

        If footer.StringEmpty Then
            content.footer = New Decoration With {.center = "[page] / [toPage]"}
        ElseIf footer.ExtensionSuffix("html") Then
            content.footer = New Decoration With {.html = footer}
        Else
            content.footer = New Decoration With {.center = footer}
        End If

        Dim output As New PdfOutput With {.OutputFilePath = pdfout}
        Dim wkhtmltopdf As New PdfConvertEnvironment With {
            .TempFolderPath = TempFileSystem.GetAppSysTempFile("__pdf", App.PID.ToHexString, "wkhtmltopdf"),
            .Debug = env.globalEnvironment.Rscript.debug,
            .Timeout = 60000,
            .WkHtmlToPdfPath = env.globalEnvironment.options.getOption("wkhtmltopdf")
        }

        If wkhtmltopdf.WkHtmlToPdfPath.StringEmpty Then
            Return Internal.debug.stop("please config wkhtmltopdf program at first!", env)
        ElseIf Not wkhtmltopdf.WkHtmlToPdfPath.FileExists Then
            Return Internal.debug.stop($"wkhtmltopdf program Is Not exists at the given location: '{wkhtmltopdf.WkHtmlToPdfPath}'...", env)
        End If

        If wkhtmltopdf.Debug Then
            Call Console.WriteLine("wkhtmltopdf config:")
            Call Console.WriteLine(wkhtmltopdf.GetJson)
        End If

        If Not content.CheckContentSource Then
            Return Internal.debug.stop("part of the content is missing... break pdf conversion progress...", env)
        End If

        Call pdfout.ParentPath.MakeDir
        Call PdfConvert.ConvertHtmlToPdf(content, output, environment:=wkhtmltopdf)

        Return Nothing
    End Function
End Module

