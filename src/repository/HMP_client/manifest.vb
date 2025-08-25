#Region "Microsoft.VisualBasic::479c7bab930799dacc768c29bed7ef63, HMP_client\manifest.vb"

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


' Code Statistics:

'   Total Lines: 62
'    Code Lines: 46 (74.19%)
' Comment Lines: 8 (12.90%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 8 (12.90%)
'     File Size: 2.03 KB


' Class manifest
' 
'     Properties: AsperaURL, file_id, HttpURL, md5, sample_id
'                 size, urls
' 
'     Function: LoadTable, ToString
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Darwinism.OSSUtil
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Text
Imports csv = Microsoft.VisualBasic.Data.Framework.IO.File

''' <summary>
''' 数据文件的下载信息
''' </summary>
Public Class manifest

    Public Property file_id As String
    Public Property md5 As String
    Public Property size As Long
    Public Property urls As String
    Public Property sample_id As String

    Public ReadOnly Property HttpURL As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return urls _
                .Split(","c) _
                .Where(Function(url)
                           Return InStr(url, "http://", CompareMethod.Text) = 1 OrElse
                                  InStr(url, "https://", CompareMethod.Text) = 1
                       End Function) _
                .FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property AsperaURL As String
        Get
            Return urls _
                .Split(","c) _
                .Where(AddressOf Aspera.IsAsperaProtocol) _
                .FirstOrDefault
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim basename$

        If HttpURL.StringEmpty Then
            basename = AsperaURL.FileName
        Else
            basename = HttpURL.FileName
        End If

        Return $"{basename} ({size.Unit(ByteSize.B).ScaleTo(ByteSize.MB)})"
    End Function

    ''' <summary>
    ''' 从tsv表格文件之中加载资源数据
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function LoadTable(tsv As String) As IEnumerable(Of manifest)
        Return csv.LoadTsv(tsv, Encodings.UTF8).AsDataSource(Of manifest)()
    End Function
End Class
