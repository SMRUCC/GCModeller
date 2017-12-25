#Region "Microsoft.VisualBasic::d6910088abba0b348e245e9bfa26dda8, ..\repository\HMP_client\manifest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File
Imports Microsoft.VisualBasic.Data.csv

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

    ''' <summary>
    ''' 从tsv表格文件之中加载资源数据
    ''' </summary>
    ''' <param name="tsv"></param>
    ''' <returns></returns>
    Public Shared Function LoadTable(tsv As String) As IEnumerable(Of manifest)
        Return csv.LoadTsv(tsv, Encodings.UTF8).AsDataSource(Of manifest)()
    End Function
End Class

