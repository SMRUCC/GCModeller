#Region "Microsoft.VisualBasic::ea52479d60c212e329f3643567c9911d, ..\interops\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\COG\Whog\Category.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace LocalBLAST.Application.RpsBLAST.Whog

    Public Class Category

        <XmlAttribute> Public Property Category As String
        <XmlAttribute> Public Property COG_id As String
        <XmlElement("description")> Public Property Description As String
        <XmlElement("geneID")> Public Property IdList As NamedValue()
            Get
                Return _IdList
            End Get
            Set(value As NamedValue())
                If value Is Nothing Then
                    Return
                End If

                Dim LQuery = LinqAPI.Exec(Of NamedCollection(Of String)) <=
                    From item In value
                    Let list As String() = item.text.Split
                    Select New NamedCollection(Of String) With {
                        .Name = item.name,
                        .Value = list,
                        .Description = item.text
                    }

                _IdList = value
                IdTokens = LQuery
                _locus_tags = (From item In LQuery Let IdList As String() = item.Value Select IdList).ToVector
            End Set
        End Property

        Dim _IdList As NamedValue()
        Dim IdTokens As NamedCollection(Of String)()

        Public ReadOnly Property locus_tags As String()

        Const REGX_CATAGORY As String = "\[[^]]+\]"
        Const REGX_COG_ID As String = "COG\d+"

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1} --> {2}", Category, COG_id, Description)
        End Function

        Public Function ContainsGene(id As String) As Boolean
            Return Array.IndexOf(locus_tags, id) > -1
        End Function

        Protected Friend Shared Function Parse(srcText$()) As Category
            Dim list As NamedValue() = srcText _
                .Skip(1) _
                .Select(Function(line) line.GetTagValue(":", trim:=True)) _
                .Select(Function(l) New NamedValue With {
                    .name = l.Name.Trim,
                    .text = l.Value
                }).ToArray
            Dim description As String = srcText(Scan0)
            Dim cat$ = Regex.Match(description, REGX_CATAGORY).Value
            Dim item As New Category With {
                .Category = Mid(cat, 2, Len(cat) - 2),
                .COG_id = Regex.Match(description, REGX_COG_ID).Value,
                .Description = Mid(description, Len(.Category) + Len(.COG_id) + 4).Trim,
                .IdList = list
            }
            Return item
        End Function

        Public Function Find(Id As String) As String
            If IdTokens Is Nothing Then
                Return ""
            End If

            Dim LQuery As String =
                LinqAPI.DefaultFirst(Of String) <= From item
                                                   In IdTokens
                                                   Where Array.IndexOf(item.Value, Id) > -1
                                                   Select item.Name
            Return LQuery
        End Function
    End Class
End Namespace
