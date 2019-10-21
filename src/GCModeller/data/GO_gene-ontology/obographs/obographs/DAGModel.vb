Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Public Module DAGModel

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
        Next

        Return g
    End Function

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
                .U = g.GetNode(term.id),
                .V = g.GetNode(rel.parent.Name),
                .isDirected = True,
                .weight = rel.type,
                .data = New EdgeData With {
                    .weight = rel.type,
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
