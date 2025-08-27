#Region "Microsoft.VisualBasic::c0159267ba46eaceeb2921e3ac8902d8, data\GO_gene-ontology\obographs\obographs\DAGModel.vb"

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

    '   Total Lines: 166
    '    Code Lines: 109 (65.66%)
    ' Comment Lines: 37 (22.29%)
    '    - Xml Docs: 86.49%
    ' 
    '   Blank Lines: 20 (12.05%)
    '     File Size: 6.32 KB


    ' Module DAGModel
    ' 
    '     Function: (+2 Overloads) CreateGraph, tryInsertNode
    ' 
    '     Sub: addTerm
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module DAGModel

    Friend Const value_colors As String = "value_colors"

    ''' <summary>
    ''' Create a <see cref="NetworkGraph"/> based on a given go term id list.
    ''' </summary>
    ''' <param name="go"></param>
    ''' <param name="terms">
    ''' A go term <see cref="Term.id"/> collection.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGraph(go As GO_OBO, terms As IEnumerable(Of NamedValue(Of Double)), Optional colorSet$ = "Spectral:c11") As NetworkGraph
        Dim enrichment As Dictionary(Of String, Double) = terms _
            .ToDictionary(Function(term) term.Name,
                          Function(term)
                              Return term.Value
                          End Function)
        Dim dag As NetworkGraph = go.CreateGraph(enrichment.Keys)
        Dim colors As Color() = Designer.GetColors(colorSet, 30)
        Dim valueRange As DoubleRange = enrichment.Values.Range
        Dim indexRange As DoubleRange = {0#, colors.Length - 1}
        Dim value As Double
        Dim color As String
        Dim index As Integer
        Dim gray As String = Drawing.Color.Gray.ToHtmlColor

        For Each node As Node In dag.vertex
            value = enrichment.TryGetValue(node.label, [default]:=Double.NaN)

            If value.IsNaNImaginary Then
                node.data.Add(value_colors, gray)
            Else
                index = CInt(valueRange.ScaleMapping(value, indexRange))
                color = colors(index).ToHtmlColor
                node.data.Add(value_colors, color)
            End If
        Next

        Return dag
    End Function

    ''' <summary>
    ''' Create a <see cref="NetworkGraph"/> based on a given go term id list.
    ''' </summary>
    ''' <param name="go"></param>
    ''' <param name="terms">
    ''' A go term <see cref="Term.id"/> collection.
    ''' </param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateGraph(go As GO_OBO, terms As IEnumerable(Of String)) As NetworkGraph
        Dim g As New NetworkGraph
        Dim termsTable As Dictionary(Of String, Term) = go.CreateTermTable
        Dim relIndex As New Index(Of String)

        ' 每一个term都单独构建出一条通往base namespace的途径
        For Each termId As String In terms.SafeQuery
            Call termsTable(termId).addTerm(g, termsTable, relIndex)
            Call termsTable(termId).ToString.info
        Next

        Return g
    End Function

    ''' <summary>
    ''' 所插入的节点中，<see cref="Term.id"/>为节点的唯一标识符
    ''' </summary>
    ''' <param name="term"></param>
    ''' <param name="g"></param>
    ''' <param name="termsTable"></param>
    ''' <param name="relIndex"></param>
    <Extension>
    Private Sub addTerm(term As Term, g As NetworkGraph, termsTable As Dictionary(Of String, Term), relIndex As Index(Of String))
        Dim relations As New OBO.OntologyRelations(term)
        Dim relLink As Edge
        Dim relLabel$
        Dim baseTerm As Term

        ' add itself as node
        Call g.tryInsertNode(term)

        For Each rel In relations.AsEnumerable
            baseTerm = termsTable(rel.parent.Name)

            ' 边的连接是有方向的
            Call g.tryInsertNode(baseTerm)

            relLabel = $"{term.id} {rel.type.Description} {baseTerm.id}"

            ' 已经添加过了
            ' 则后面的都不需要再做添加了
            If relLabel Like relIndex Then
                Continue For
            Else
                relIndex += relLabel
            End If

            relLink = New Edge With {
                .U = g.GetElementByID(term.id),
                .V = g.GetElementByID(rel.parent.Name),
                .isDirected = True,
                .weight = rel.type,
                .data = New EdgeData With {
                    .label = relLabel,
                    .Properties = New Dictionary(Of String, String) From {
                        {"relationship", rel.type.Description},
                        {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, rel.type.Description}
                    }
                }
            }

            Call g.AddEdge(relLink)
            Call baseTerm.addTerm(g, termsTable, relIndex)
        Next
    End Sub

    ''' <summary>
    ''' 在这里会尝试处理重复出现的term对象
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="term"></param>
    ''' <returns>
    ''' 这个函数返回false说明对象已经被添加过了
    ''' 则其网络关系也已经添加过了？
    ''' </returns>
    <Extension>
    Private Function tryInsertNode(g As NetworkGraph, term As Term) As Boolean
        If Not g.ExistVertex(term.id) Then
            Dim def As Definition = Definition.Parse(term)
            Dim node As New Node With {
                .label = term.id,
                .data = New NodeData With {
                    .origID = term.id,
                    .label = term.name,
                    .Properties = New Dictionary(Of String, String) From {
                        {"definition", def.definition},
                        {"evidence", def.evidences.GetJson},
                        {"is_obsolete", def.isOBSOLETE.ToString.ToLower},
                        {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, term.namespace}
                    }
                }
            }

            Call g.AddNode(node)

            Return True
        Else
            Return False
        End If
    End Function
End Module
