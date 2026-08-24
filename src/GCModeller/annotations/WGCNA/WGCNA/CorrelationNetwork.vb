Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.Framework.IO.CSVFile
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports std = System.Math

Public Module CorrelationNetwork

    Public Function LoadAdjacencyMatrix(file As String) As DataMatrix
        Dim rows As IEnumerable(Of String()) = From line As String In file.IterateAllLines(tqdm_wrap:=True) Skip 1 Select Tokenizer.CharsParser(line).ToArray
        Dim data As IEnumerable(Of (String, Double())) = From row As String() In rows Select (row(0), row.Skip(1).AsDouble)
        Dim adj As New DataMatrix(data)
        Return adj
    End Function

    <Extension>
    Public Function ExportGraph(adj As DataMatrix, modules As IEnumerable(Of ModuleMembershipResult), Optional adj_thres As Double = 0.8) As NetworkGraph
        Dim g As New NetworkGraph With {
            .id = "adjacency_matrix",
            .name = "WGCNA correlation network"
        }

        For Each gene As ModuleMembershipResult In modules
            Call g.CreateNode(gene.GeneId, New NodeData With {
                .color = New SolidBrush(gene.ModuleName.TranslateColor),
                .label = gene.ToString,
                .mass = gene.Correlation,
                .origID = gene.GeneId,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, gene.ModuleName},
                    {"kME", gene.Correlation},
                    {"pvalue", gene.PValue}
                }
            })
        Next

        For Each gene_id As String In TqdmWrapper.Wrap(g.vertex.Select(Function(a) a.label).ToArray)
            Dim u = g.GetElementByID(gene_id)

            ' 20260824 removes the selfloop node
            For Each v As Node In From vi As Node
                                  In g.vertex
                                  Where vi.label <> gene_id

                Dim cor As Double = adj(gene_id, v.label)

                If std.Abs(cor) > adj_thres Then
                    Call g.CreateEdge(u, v, weight:=cor)
                End If
            Next
        Next

        Return g
    End Function
End Module
