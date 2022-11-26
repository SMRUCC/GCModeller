#Region "Microsoft.VisualBasic::e6b7c752b2253c819384b8b7ab730bf4, GCModeller\core\Bio.Assembly\Assembly\NCBI\Database\GenBank\GBK\Keywords\DBLINK.vb"

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

    '   Total Lines: 52
    '    Code Lines: 31
    ' Comment Lines: 11
    '   Blank Lines: 10
    '     File Size: 1.75 KB


    '     Class DBLINK
    ' 
    '         Properties: Links
    ' 
    '         Function: Parser, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text

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

        ''' <summary>
        ''' 返回的是genbank的这部分的文本数据，用于生成genbank文件
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim links$() = Me.Links.Select(Function(x) $"{x.Name}: {x.Value}").ToArray
            Dim sb As New StringBuilder("DBLINK      " & links(Scan0))

            For Each l$ In links.Skip(1)
                Call sb.AppendLine("            " & l)
            Next

            Return sb.ToString.Trim(ASCII.LF, ASCII.CR)
        End Function

        Friend Shared Function Parser(list As String()) As DBLINK
            Dim links As New List(Of NamedValue(Of String))

            If Not list.IsNullOrEmpty Then
                Dim line As String = list(Scan0) ' 第一行会有些特殊

                links += Mid(line, 13).Trim.GetTagValue(":", trim:=True)
                For Each line In list.Skip(1)
                    links += Strings.Trim(line).GetTagValue(":", trim:=True)
                Next
            End If

            Return New DBLINK With {
                .Links = links
            }
        End Function
    End Class
End Namespace
