Imports System.Runtime.CompilerServices

Namespace Core

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 从 CSV/TSV 读取先验调控网络
        ''' 格式：TF, TargetGene, RegulationType, Confidence, Evidence
        ''' </summary>
        ''' 
        <Extension>
        Public Function AssemblePriorNetwork(TRN As IEnumerable(Of RegulatoryEdge)) As PriorNetwork
            Dim prior As New PriorNetwork()

            If Not TRN Is Nothing Then
                For Each edge As RegulatoryEdge In TRN
                    Call prior.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
                Next
            End If

            Return prior
        End Function
    End Module
End Namespace