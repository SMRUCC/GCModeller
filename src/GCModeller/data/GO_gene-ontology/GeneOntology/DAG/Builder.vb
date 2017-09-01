#Region "Microsoft.VisualBasic::2b7597ba523f6aaa2721b6f81391e048, ..\GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Builder.vb"

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

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    Public Module Builder

        <Extension>
        Public Function BuildTree(file As IEnumerable(Of Term)) As Dictionary(Of TermNode)
            Dim tree As New Dictionary(Of TermNode)

            For Each x As Term In file
                tree += x.ConstructNode
            Next

            Return tree
        End Function

        <Extension> Public Function ConstructNode(term As Term) As TermNode
            Dim is_a = term.is_a.ToArray(Function(s) New is_a(s$))
            Dim rels = term.relationship.ToArray(Function(s) New Relationship(s$))
            Dim synonym = term.synonym.ToArray(Function(s) New synonym(s$))

            Return New TermNode With {
                .id = term.id,
                .is_a = is_a,
                .relationship = rels,
                .synonym = synonym,
                .xref = term.xref.ToArray(AddressOf xrefParser),
                .namespace = term.namespace
            }
        End Function

        Private Function xrefParser(s$) As NamedValue(Of String)
            Dim tokens$() = CommandLine.GetTokens(s$)
            Dim id$() = tokens(Scan0).Split(":"c)

            Return New NamedValue(Of String) With {
                .Name = id(Scan0),
                .Value = id(1%),
                .Description = tokens.ElementAtOrDefault(1%)
            }
        End Function

        Public Function BuildTree(path$) As Dictionary(Of TermNode)
            Return GO_OBO.Open(path).BuildTree
        End Function
    End Module
End Namespace
