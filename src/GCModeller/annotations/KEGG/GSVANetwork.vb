Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.WGCNA.Network

Public Module GSVANetwork

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gsva">limma analysis result of gsva matrix</param>
    ''' <param name="diffExprs">limma analysis result of the different expression</param>
    ''' <param name="model">gsva/gsea background model</param>
    ''' <param name="cor">create network across other component that not in kegg pathway network</param>
    ''' <param name="names">mapping the molecule id to molecule name, molecule node will only display the molecule id if this mapping is missing.</param>
    ''' <returns></returns>
    Public Function AssemblingNetwork(gsva As LimmaTable(), diffExprs As LimmaTable(), model As Background,
                                      Optional cor As WGCNAWeight = Nothing,
                                      Optional modules As Dictionary(Of String, ClusterModuleResult) = Nothing,
                                      Optional names As Dictionary(Of String, String) = Nothing) As NetworkGraph
        Dim g As New NetworkGraph

        If names Is Nothing Then
            names = New Dictionary(Of String, String)
        End If
        If modules Is Nothing Then
            modules = New Dictionary(Of String, ClusterModuleResult)
        End If

        For Each node As LimmaTable In gsva
            Call g.CreateNode(node.id, New NodeData With {
                .label = model(node.id).names,
                .origID = node.id,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "pathway"}
                }
            })
        Next

        For Each node As LimmaTable In diffExprs
            Dim color As ClusterModuleResult = modules.TryGetValue(node.id)
            Dim color_str As String = If(color Is Nothing, "NA", color.color)
            Dim nodeName As String = names.TryGetValue(node.id, [default]:=node.id)

            Call g.CreateNode(node.id, New NodeData With {
                .label = If(nodeName.StringEmpty, node.id, nodeName),
                .origID = node.id,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "different expression molecule"},
                    {"module", color_str}
                }
            })
        Next

        For Each path As LimmaTable In gsva
            Dim cluster As Cluster = model(path.id)
            Dim pathway As Node = g.GetElementByID(path.id)

            For Each mol As LimmaTable In diffExprs
                If cluster.Intersect({mol.id}).Any Then
                    Call g.CreateEdge(g.GetElementByID(mol.id), pathway, 0, New EdgeData With {
                            .Properties = New Dictionary(Of String, String) From {
                                {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "pathway member"}
                            }
                        }
                    )
                End If
            Next
        Next

        If cor IsNot Nothing Then
            For Each a As LimmaTable In diffExprs
                For Each b As LimmaTable In diffExprs
                    If a.id = b.id Then
                        Continue For
                    End If

                    Dim corVal As Weight = cor(a.id, b.id)

                    If corVal IsNot Nothing Then
                        Call g.CreateEdge(
                            g.GetElementByID(a.id), g.GetElementByID(b.id), CDbl(corVal), New EdgeData With {
                                .Properties = New Dictionary(Of String, String) From {
                                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, "correlation network"}
                                }
                            }
                        )
                    End If
                Next
            Next
        End If

        Return g
    End Function

End Module
