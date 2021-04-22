﻿#Region "Microsoft.VisualBasic::13e0d96ad447f80565af7e93d5a1061e, mime\application%vnd.openxmlformats-officedocument.spreadsheetml.sheet\Excel\IO.vb"

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

    ' Module IO
    ' 
    '     Function: CreateReader, SaveTo, ToXML
    ' 
    '     Sub: UnZipHandler
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Zip
Imports Microsoft.VisualBasic.MIME.Office.Excel.Model.Directory
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Xml
Imports Microsoft.VisualBasic.Text.Xml.OpenXml

Public Module IO

    ' /
    '  +------- <_rels>
    '           + .rels
    '  +------- <docProps>
    '  +------- <xl>
    '  +------- [Content_Types].xml
    Private Sub UnZipHandler(xlsx$, ROOT$)
        Dim success As Boolean = False
        Dim exception As Exception = Nothing

        ' 20190606 会随机性的出现本地文件头已损坏的错误？？
        For i As Integer = 1 To 3
            success = False

            Try
                If DataURI.IsWellFormedUriString(xlsx) Then
                    UnZip.ImprovedExtractToDirectory(DataURI.URIParser(xlsx), destinationDirectoryName:=ROOT, Overwrite.Always)
                Else
                    UnZip.ImprovedExtractToDirectory(xlsx, ROOT, Overwrite.Always)
                End If

                success = True
            Catch ex As Exception
                exception = ex
            End Try

            If success Then
                Exit For
            End If
        Next

        If Not success Then
            Throw exception
        End If
    End Sub

    ''' <summary>
    ''' 解压缩Excel文件然后读取其中的XML数据以构成DataFrame表格 
    ''' </summary>
    ''' <param name="xlsx"></param>
    ''' <returns></returns>
    Public Function CreateReader(xlsx As String) As File
        Dim ROOT$ = TempFileSystem.GetAppSysTempFile(
            ext:=RandomASCIIString(6, skipSymbols:=True),
            sessionID:=App.PID,
            prefix:="excel_xlsx_"
        )

        Call UnZipHandler(xlsx, ROOT)

        Dim contentType As ContentTypes = (ROOT & "/[Content_Types].xml").LoadXml(Of ContentTypes)
        Dim rels As New _rels(ROOT)
        Dim docProps As New docProps(ROOT)
        Dim xl As New xl(ROOT)
        Dim file As New File With {
            .ContentTypes = contentType,
            ._rels = rels,
            .docProps = docProps,
            .xl = xl,
            .FilePath = If(DataURI.IsWellFormedUriString(xlsx), "datauri://", xlsx),
            .ROOT = ROOT
        }

        Return file
    End Function

    ''' <summary>
    ''' Save the Xlsx file data to a specific <paramref name="path"/> location.
    ''' </summary>
    ''' <param name="xlsx"></param>
    ''' <param name="path$"></param>
    ''' <returns></returns>
    <Extension> Public Function SaveTo(xlsx As File, path$) As Boolean
        Dim workbook$ = xlsx.ROOT & "/xl/workbook.xml"
        Dim sharedStrings = xlsx.ROOT & "/xl/sharedStrings.xml"
        Dim ContentTypes$ = xlsx.ROOT & "/[Content_Types].xml"

        If xlsx.modify("worksheet.add") > -1 Then
            With xlsx.xl
                Call .worksheets.Save()
                Call .workbook _
                    .ToXML _
                    .SaveTo(workbook, UTF8WithoutBOM)
                Call .sharedStrings _
                    .ToXML _
                    .SaveTo(sharedStrings, UTF8WithoutBOM)

                Call xlsx.ContentTypes _
                    .ToXML _
                    .SaveTo(ContentTypes, UTF8WithoutBOM)
            End With
        ElseIf xlsx.modify("worksheet.update") > -1 Then
            Call xlsx.xl.worksheets.Save()
            Call xlsx.xl.sharedStrings _
                .ToXML _
                .SaveTo(sharedStrings, UTF8WithoutBOM)
        End If

        ' 重新进行zip打包
        Call ZipLib.DirectoryArchive(xlsx.ROOT, path, ArchiveAction.Replace, Overwrite.Always, CompressionLevel.Fastest)

        Return True
    End Function

    <Extension>
    Public Function ToXML(Of T)(obj As T) As String
        Dim xml As New XmlDoc(obj.GetXml(xmlEncoding:=XmlEncodings.UTF8))
        xml.xmlns.xsi = Nothing
        xml.xmlns.xsd = Nothing
        xml.standalone = True

        Dim out$ = ASCII.TrimNonPrintings(xml.ToString)
        Return out
    End Function
End Module
