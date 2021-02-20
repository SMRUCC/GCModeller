#Region "Microsoft.VisualBasic::d6f568b5b291b948f3a655f5b105b570, markdown2pdf\PdfConvert\PdfConvert.vb"

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

    ' Module PdfConvert
    ' 
    '     Function: BuildArguments, createPageArguments, (+2 Overloads) getRepeatParameters
    ' 
    '     Sub: (+4 Overloads) ConvertHtmlToPdf, PdfConvertFailure, RunProcess
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports WkHtmlToPdf.Arguments

''' <summary>
''' wkhtmltopdf is able to put several objects into the output file, an object is
''' either a single webpage, a cover webpage or a table of contents.  The objects
''' are put into the output document in the order they are specified on the
''' command line, options can be specified on a per object basis or in the global
''' options area. Options from the Global Options section can only be placed in
''' the global options area
''' </summary>
Public Module PdfConvert

    Public Const PdfPageBreak$ = "<div style='page-break-before:always;'></div>"

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub ConvertHtmlToPdf(document As PDFContent, output As PdfOutput)
        ConvertHtmlToPdf(document, output, Nothing)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="document"></param>
    ''' <param name="out$">PDF的保存的文件路径</param>
    <Extension>
    Public Sub ConvertHtmlToPdf(document As PDFContent, out$)
        Call ConvertHtmlToPdf(document, New PdfOutput With {
            .OutputFilePath = out
        })
    End Sub

    Const noHTML$ = "You must supply a HTML string, if you have enterd the url: '-'"

    Public Sub ConvertHtmlToPdf(document As PDFContent, woutput As PdfOutput, Optional environment As PdfConvertEnvironment = Nothing)
        Dim html$ = document.GetDocument
        Dim url$() = Nothing

        If TypeOf document Is PdfDocument Then
            url = DirectCast(document, PdfDocument).Url
        End If

        If url.IsNullOrEmpty Then
            If Not html.StringEmpty Then
                With App.GetAppSysTempFile(, App.PID)
                    html.SaveTo(.ByRef)
                    url = { .ByRef}
                End With
            Else
                Throw New PdfConvertException(noHTML)
            End If
        End If

        Dim outputPdfFilePath As String
        Dim delete As Boolean
        Dim argument$

        environment = environment Or InternalEnvironment.Environment

        If woutput.OutputFilePath IsNot Nothing Then
            outputPdfFilePath = woutput.OutputFilePath
            delete = False
        Else
            outputPdfFilePath = App.GetAppSysTempFile(".pdf", App.PID)
            delete = True
        End If

        If Not File.Exists(environment.WkHtmlToPdfPath) Then
            Throw New PdfConvertException($"File '{environment.WkHtmlToPdfPath}' not found. Check if wkhtmltopdf application is installed.")
        Else
            argument = document.BuildArguments(url, outputPdfFilePath)
        End If

        Try
            Call outputPdfFilePath.ParentPath.MkDIR
            Call environment.RunProcess(
                args:=argument,
                url:=url.JoinBy(ASCII.LF),
                document:=document,
                outputPdfFilePath:=outputPdfFilePath,
                woutput:=woutput
            )
        Finally
            If delete AndAlso File.Exists(outputPdfFilePath) Then
                File.Delete(outputPdfFilePath)
            End If
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="document"></param>
    ''' <param name="urls$"></param>
    ''' <param name="pdfOut$"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 这些命令部分之间是具有顺序的
    ''' </remarks>
    <Extension>
    Public Function BuildArguments(document As PDFContent, urls$(), pdfOut$) As String
        Dim paramsBuilder As New StringBuilder

        If Not document.globalOptions Is Nothing Then
            Call paramsBuilder.AppendLine(document.globalOptions.GetCLI)
        End If
        If Not document.pagesize Is Nothing Then
            Call paramsBuilder.AppendLine(document.pagesize.ToString)
        End If

        If Not document.TOC Is Nothing Then
            Call paramsBuilder.AppendLine("toc")
            Call paramsBuilder.AppendLine(document.TOC.GetCLI)
        End If
        If Not document.outline Is Nothing Then
            Call paramsBuilder.AppendLine(document.outline.GetCLI)
        End If

        If Not document.header Is Nothing Then
            Call paramsBuilder.AppendLine(document.header.GetCLI("--header"))
        End If
        If Not document.footer Is Nothing Then
            Call paramsBuilder.AppendLine(document.footer.GetCLI("--footer"))
        End If

        If Not document.page Is Nothing Then
            Dim pageArgument = document.createPageArguments

            If TypeOf document Is PdfDocument AndAlso TryCast(document, PdfDocument).LocalConfigMode Then
                For Each url As String In urls
                    Call paramsBuilder.AppendLine(pageArgument)
                    Call paramsBuilder.AppendLine(url.CLIPath)
                Next
            Else
                Call paramsBuilder.AppendLine(pageArgument)
                Call paramsBuilder.AppendLine($"""{urls.JoinBy(""" """)}""")
            End If
        Else
            Call paramsBuilder.AppendLine($"""{urls.JoinBy(""" """)}""")
        End If

        Call paramsBuilder.AppendLine(pdfOut.CLIPath)

        Return paramsBuilder.ToString
    End Function

    <Extension>
    Private Function createPageArguments(document As PDFContent) As String
        Dim paramsBuilder As New StringBuilder

        ' 2018-10-25 添加page标记会出bug
        ' Call paramsBuilder.AppendLine("page")
        Call paramsBuilder.AppendLine(document.page.GetCLI)

        If Not document.page.cookies.IsNullOrEmpty Then
            Call paramsBuilder.AppendLine(document.page.cookies.getRepeatParameters("--cookie"))
        End If
        If Not document.page.customheader.IsNullOrEmpty Then
            Call paramsBuilder.AppendLine(document.page.customheader.getRepeatParameters("--custom-header"))
        End If
        If Not document.page.runscript.IsNullOrEmpty Then
            Call paramsBuilder.AppendLine(document.page.runscript.getRepeatParameters("--run-script"))
        End If

        Return paramsBuilder.ToString
    End Function

    <Extension>
    Private Function getRepeatParameters(data$(), argName$) As String
        Dim sb As New StringBuilder

        For Each item In data
            Call sb.AppendLine($"{argName} {item.CLIToken}")
        Next

        Return sb.ToString
    End Function

    <Extension>
    Private Function getRepeatParameters(data As Dictionary(Of String, String), argName$) As String
        Dim sb As New StringBuilder

        For Each item In data
            Call sb.AppendLine($"{argName} {item.Key.CLIToken} {item.Value.CLIToken}")
        Next

        Return sb.ToString
    End Function

    <Extension>
    Private Sub RunProcess(environment As PdfConvertEnvironment,
                           args$,
                           url$, outputPdfFilePath$,
                           document As PDFContent,
                           woutput As PdfOutput)

        Using process As New IORedirect(environment.WkHtmlToPdfPath, args, IOredirect:=True)
            If environment.Debug Then
                Call $"Process running in debug mode...".__DEBUG_ECHO
                Call $"Current workspace: {App.CurrentDirectory}.".__DEBUG_ECHO
            End If

            Call process.Start(False)

            If process.WaitForExit(environment.Timeout) AndAlso
               process.WaitOutput(environment.Timeout) AndAlso
               process.WaitError(environment.Timeout) Then

                If process.ExitCode <> 0 AndAlso Not File.Exists(outputPdfFilePath) Then
                    Call process.GetError.PdfConvertFailure(url)
                End If
            Else
                If Not process.HasExited Then
                    Call process.Kill()
                End If

                Throw New PdfConvertTimeoutException()
            End If

            If environment.Debug Then
                Call Console.WriteLine(process.StandardOutput)
            End If
        End Using

        If woutput.OutputStream IsNot Nothing Then
            Using fs As Stream = New FileStream(outputPdfFilePath, FileMode.Open)
                Dim buffer As Byte() = New Byte(32 * 1024 - 1) {}
                Dim read As i32 = 0

                While (read = fs.Read(buffer, 0, buffer.Length)) > 0
                    Call woutput.OutputStream.Write(buffer, 0, read)
                End While
            End Using
        End If

        If woutput.OutputCallback IsNot Nothing Then
            Dim pdfFileBytes As Byte() = File.ReadAllBytes(outputPdfFilePath)
            Call woutput.OutputCallback()(document, pdfFileBytes)
        End If
    End Sub

    <Extension> Private Sub PdfConvertFailure(error$, url$)
        Dim msg$ = $"Html to PDF conversion of '{url}' failed. 
Wkhtmltopdf output:

{[error]}"
        Throw New PdfConvertException(msg)
    End Sub

    Public Sub ConvertHtmlToPdf(url As String, outputFilePath As String, Optional environment As PdfConvertEnvironment = Nothing)
        Dim [in] As New PdfDocument With {.Url = {url}}
        Dim out As New PdfOutput With {.OutputFilePath = outputFilePath}

        Call ConvertHtmlToPdf([in], out, environment)
    End Sub

    <Extension>
    Private Function localFileExists(file As String) As Boolean
        If file.isURL Then
            Return True
        Else
            Return file.FileExists
        End If
    End Function

    ''' <summary>
    ''' check pdf content source
    ''' </summary>
    ''' <param name="content"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CheckContentSource(content As PdfDocument) As Boolean
        Dim check As Boolean = True
        Dim valid As Boolean
        Dim message As String

        Call Console.WriteLine($"check for {content.Url.Length} content source urls...")

        For Each file As String In content.Url
            valid = file.localFileExists
            message = $"{file.GetFullPath} ... [{valid.ToString.ToLower}]"

            Console.WriteLine(message)

            If Not valid Then
                check = False
            End If
        Next

        Return check
    End Function
End Module

