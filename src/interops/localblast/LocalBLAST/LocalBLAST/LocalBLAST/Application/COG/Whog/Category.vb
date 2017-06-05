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
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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
                _locus_tags = New Index(Of String)((From item In LQuery Let IdList As String() = item.Value Select IdList).IteratesALL)
            End Set
        End Property

        Dim _IdList As NamedValue()
        Dim IdTokens As NamedCollection(Of String)()

        <XmlIgnore>
        Public ReadOnly Property locus_tags As Index(Of String)

        Const REGX_CATAGORY As String = "\[[^]]+\]"
        Const REGX_COG_ID As String = "COG\d+"

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1} --> {2}", Category, COG_id, Description)
        End Function

        Public Function ContainsGene(id As String) As Boolean
            Return locus_tags.IndexOf(id) > -1
        End Function

        Protected Friend Shared Function Parse(srcText$()) As Category
            Dim list As NamedValue() = __parseList(srcText.Skip(1).ToArray)
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

        Private Shared Function __parseList(lines As IEnumerable(Of String)) As NamedValue()
            Dim list As New List(Of NamedValue)

            For Each line As String In lines
                Dim nid As NamedValue(Of String) = line.GetTagValue(":", trim:=True)
                Dim genome$ = nid.Name.Trim

                If Not String.IsNullOrEmpty(genome) Then
                    list += New NamedValue With {
                        .name = genome,
                        .text = nid.Value
                    }
                Else
                    list.Last = New NamedValue With {
                        .name = list.Last.name,
                        .text = list.Last.text & " " & Trim(nid.Value)
                    }
                End If
            Next

            Return list
        End Function

        Public Function Find(Id$) As String
            If IdTokens Is Nothing Then
                Return ""
            End If

            Dim LQuery$ = LinqAPI.DefaultFirst(Of String) <= From item
                                                             In IdTokens
                                                             Where Array.IndexOf(item.Value, Id) > -1
                                                             Select item.Name
            Return LQuery
        End Function
    End Class
End Namespace
