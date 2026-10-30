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

        ''' <summary>
        ''' 把文本形式的调控类型映射为 <see cref="Effector"/> 枚举。
        ''' </summary>
        ''' <param name="text">
        ''' 原始文本，支持 activation/activate/activator/up/positive 与
        ''' repression/repress/repressor/inhibit/inhibitor/down/negative
        ''' 及 1 / -1 数值写法。
        ''' </param>
        Public Function ParseRegulationType(text As String) As Effector
            If String.IsNullOrWhiteSpace(text) Then Return Effector.Unknown

            Select Case text.Trim().ToLower()
                Case "activation", "activate", "activator", "activating", "up", "positive", "1", "+", "↑"
                    Return Effector.Activator
                Case "repression", "repress", "repressor", "inhibition", "inhibit", "inhibitor", "suppress", "down", "negative", "-1", "-", "↓"
                    Return Effector.Inhibitor
                Case "1" : Return Effector.Activator
                Case "2" : Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
        End Function

        ''' <summary>
        ''' 根据相关系数符号推断调控类型：正相关为激活因子，负相关为抑制因子。
        ''' </summary>
        ''' <param name="weight">WGCNA 网络边的相关系数（通常为 Pearson / Spearman 相关系数）。</param>
        ''' <returns>激活或抑制 effector 类型。</returns>
        ''' 
        <Extension>
        Public Function InferEffector(weight As Double) As Effector
            If weight >= 0 Then
                Return Effector.Activator
            Else
                Return Effector.Inhibitor
            End If
        End Function
    End Module
End Namespace