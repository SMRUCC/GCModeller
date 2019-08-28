#Region "Microsoft.VisualBasic::c83794e45d061417c18f43d2d2db7649, LocalBLAST\Pipeline\COG\Whog\Category.vb"

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

    '     Class Category
    ' 
    '         Properties: category, COG_id, description, IdList, locus_tags
    ' 
    '         Function: ContainsGene, Find, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Pipeline.COG.Whog

    <XmlType("category", [Namespace]:=Category.NCBI_COG)>
    Public Class Category

        <XmlAttribute> Public Property category As String
        <XmlAttribute> Public Property COG_id As String

        <XmlElement("description")>
        Public Property description As String

        <XmlArray("geneID")> Public Property IdList As NamedValue()
            Get
                Return list
            End Get
            Set(value As NamedValue())
                If value Is Nothing Then
                    Return
                End If

                Dim LQuery = LinqAPI.Exec(Of NamedCollection(Of String)) _
 _
                    () <= From item In value
                          Let list As String() = item.text.Split
                          Select New NamedCollection(Of String) With {
                              .name = item.name,
                              .value = list,
                              .description = item.text
                          }

                list = value
                IdTokens = LQuery
                _locus_tags = New Index(Of String)((From item In LQuery Let IdList As String() = item.value Select IdList).IteratesALL)
            End Set
        End Property

        Dim list As NamedValue()
        Dim IdTokens As NamedCollection(Of String)()

        <XmlIgnore>
        Public ReadOnly Property locus_tags As Index(Of String)

        Public Const NCBI_COG$ = "https://www.ncbi.nlm.nih.gov/COG/"

        Public Overrides Function ToString() As String
            Return String.Format("[{0}] {1} --> {2}", category, COG_id, description)
        End Function

        Public Function ContainsGene(id As String) As Boolean
            Return locus_tags.IndexOf(id) > -1
        End Function

        Public Function Find(Id$) As String
            If IdTokens Is Nothing Then
                Return ""
            End If

            Dim LQuery$ = LinqAPI.DefaultFirst(Of String) _
                () <= From item
                      In IdTokens
                      Where Array.IndexOf(item.value, Id) > -1
                      Select item.name

            Return LQuery
        End Function
    End Class
End Namespace
