﻿#Region "Microsoft.VisualBasic::4474bcdac0d95ff2138daa4416aec95a, data\GO_gene-ontology\GeneOntology\DAG\Builder.vb"

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

    '     Module Builder
    ' 
    '         Function: (+2 Overloads) BuildTree, ConstructNode, xrefParser
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
            Dim xrefValues = term.xref _
                .SafeQuery _
                .Select(AddressOf xrefParser) _
                .ToArray

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
