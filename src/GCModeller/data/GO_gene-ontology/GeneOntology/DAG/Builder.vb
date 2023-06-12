#Region "Microsoft.VisualBasic::ee652e746c508318b8be0cdda2b00660, GCModeller\data\GO_gene-ontology\GeneOntology\DAG\Builder.vb"

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

    '   Total Lines: 107
    '    Code Lines: 86
    ' Comment Lines: 5
    '   Blank Lines: 16
    '     File Size: 3.81 KB


    '     Module Builder
    ' 
    '         Function: (+2 Overloads) BuildTree, ConstructNode, CreateClusterMembers, xrefParser
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace DAG

    Public Module Builder

        <Extension>
        Public Function CreateClusterMembers(tree As Graph) As Dictionary(Of String, List(Of TermNode))
            Dim clusters As New Dictionary(Of String, List(Of TermNode))
            Dim family As Graph.InheritsChain()

            For Each term As TermNode In tree.DAG.Values
                family = tree.Family(term.id).ToArray

                For Each node As Graph.InheritsChain In family
                    For Each parent In node.Route
                        If Not clusters.ContainsKey(parent.id) Then
                            clusters.Add(parent.id, New List(Of TermNode))
                        End If

                        clusters(parent.id).Add(term)
                    Next
                Next
            Next

            Return clusters
        End Function

        <Extension>
        Public Function BuildTree(file As IEnumerable(Of Term)) As Dictionary(Of TermNode)
            Dim tree As New Dictionary(Of TermNode)

            For Each x As Term In file
                tree += x.ConstructNode
            Next

            For Each node As TermNode In tree.Values
                node.is_a = node.is_a _
                    .SafeQuery _
                    .Select(Function(rel)
                                Return New is_a With {
                                    .name = rel.name,
                                    .term_id = rel.term_id,
                                    .term = tree(rel.term_id)
                                }
                            End Function) _
                    .ToArray
            Next

            Return tree
        End Function

        ''' <summary>
        ''' Creates a node in this DAG graph
        ''' </summary>
        ''' <param name="term"></param>
        ''' <returns></returns>
        <Extension> Public Function ConstructNode(term As Term) As TermNode
            Dim is_a = term.is_a _
                .SafeQuery _
                .Select(Function(s) New is_a(s$)) _
                .ToArray
            Dim rels = term.relationship _
                .SafeQuery _
                .Select(Function(s) New Relationship(s$)) _
                .ToArray
            Dim synonym = term.synonym _
                .SafeQuery _
                .Select(Function(s) New synonym(s$)) _
                .ToArray
            Dim xrefValues = term.GetTermXrefs

            Return New TermNode With {
                .id = term.id,
                .is_a = is_a,
                .relationship = rels,
                .synonym = synonym,
                .xref = xrefValues,
                .namespace = term.namespace,
                .GO_term = term
            }
        End Function

        Public Function TermXrefParser(s As String) As NamedValue(Of String)
            Dim tokens$() = CommandLine.GetTokens(s$)
            Dim id$() = tokens(Scan0).Split(":"c)

            Return New NamedValue(Of String) With {
                .Name = id(Scan0),
                .Value = id(1%),
                .Description = tokens.ElementAtOrDefault(1%)
            }
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function GetTermXrefs(term As Term) As NamedValue(Of String)()
            Return term.xref _
                .SafeQuery _
                .Select(AddressOf TermXrefParser) _
                .ToArray
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function BuildTree(path As String) As Dictionary(Of TermNode)
            Return GO_OBO.Open(path).BuildTree
        End Function
    End Module
End Namespace
