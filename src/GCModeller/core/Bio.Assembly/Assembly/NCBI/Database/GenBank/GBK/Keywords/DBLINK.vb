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

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' 数据库之间的相互链接的列表
    ''' </summary>
    Public Class DBLINK : Inherits KeyWord

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        Public Property Links As NamedValue(Of String)()

        Public ReadOnly Property assembly_id As String
            Get
                Return GetLink("Assembly")
            End Get
        End Property

        Public ReadOnly Property bioproject As String
            Get
                Return GetLink("BioProject")
            End Get
        End Property

        Public ReadOnly Property biosample As String
            Get
                Return GetLink("BioSample")
            End Get
        End Property

        ''' <summary>
        ''' 按照名称获取链接的值(名称的比较是忽略大小写的)
        ''' </summary>
        Public Function GetLink(name As String) As String
            For Each link As NamedValue(Of String) In Links.SafeQuery
                If String.Equals(link.Name, name, StringComparison.OrdinalIgnoreCase) Then
                    Return link.Value
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>
        ''' 返回的是genbank的这部分的文本数据，用于生成genbank文件
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            If Links.IsNullOrEmpty Then
                Return ""
            End If

            Dim sb As New StringBuilder("DBLINK      " & $"{Links(Scan0).Name}: {Links(Scan0).Value}")

            For i As Integer = 1 To Links.Length - 1
                Call sb.AppendLine()
                Call sb.Append($"            {Links(i).Name}: {Links(i).Value}")
            Next

            Return sb.ToString()
        End Function

        Friend Shared Function Parser(list As String()) As DBLINK
            Dim links As New List(Of NamedValue(Of String))

            If Not list.IsNullOrEmpty Then
                For Each line As String In list
                    Dim text As String = ACCESSION.__fieldValue(line)

                    If String.IsNullOrEmpty(text) Then
                        Continue For
                    End If

                    Dim i As Integer = text.IndexOf(":"c)

                    If i < 0 Then
                        Call links.Add(New NamedValue(Of String)(text, ""))
                    Else
                        Call links.Add(New NamedValue(Of String)(
                            text.Substring(0, i).Trim(),
                            text.Substring(i + 1).Trim()))
                    End If
                Next
            End If

            Return New DBLINK With {
                .Links = links.ToArray
            }
        End Function
    End Class
End Namespace
