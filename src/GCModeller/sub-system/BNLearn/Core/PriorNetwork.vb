' ============================================================
' PriorNetwork.vb - 先验调控网络
' ============================================================
' 从 TF 注释和 TFBS motif 扫描得到的转录调控网络
' 作为 bnlearn 结构学习的白名单先验知识
' ============================================================

Namespace Core

    ''' <summary>
    ''' 转录调控关系
    ''' </summary>
    Public Class RegulatoryEdge

        ''' <summary>转录因子名称</summary>
        Public Property TF As String = ""

        ''' <summary>靶基因名称</summary>
        Public Property TargetGene As String = ""

        ''' <summary>调控类型（激活/抑制）</summary>
        Public Property RegulationType As String = "activation"

        ''' <summary>置信度分数（0-1）</summary>
        Public Property Confidence As Double = 1.0

        ''' <summary>证据来源</summary>
        Public Property Evidence As String = ""

        Public Overrides Function ToString() As String
            Return String.Format("{0} → {1} ({2}, conf={3:F2})", TF, TargetGene, RegulationType, Confidence)
        End Function

    End Class

    ''' <summary>
    ''' 先验调控网络
    ''' </summary>
    Public Class PriorNetwork

        ''' <summary>调控关系列表</summary>
        Public Property Edges As New List(Of RegulatoryEdge)()

        ''' <summary>所有转录因子名称</summary>
        Public Property TFNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>所有靶基因名称</summary>
        Public Property TargetNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ''' <summary>添加调控关系</summary>
        Public Sub AddEdge(tf As String, target As String, regType As String, confidence As Double, evidence As String)
            Edges.Add(New RegulatoryEdge() With {
                .TF = tf, .TargetGene = target,
                .RegulationType = regType, .Confidence = confidence, .Evidence = evidence})
            TFNames.Add(tf)
            TargetNames.Add(target)
        End Sub

        ''' <summary>获取指定靶基因的所有上游TF</summary>
        Public Function GetRegulators(targetGene As String) As List(Of RegulatoryEdge)
            Return Edges.Where(Function(e) String.Equals(e.TargetGene, targetGene, StringComparison.OrdinalIgnoreCase)).ToList()
        End Function

        ''' <summary>获取指定TF的所有靶基因</summary>
        Public Function GetTargets(tf As String) As List(Of RegulatoryEdge)
            Return Edges.Where(Function(e) String.Equals(e.TF, tf, StringComparison.OrdinalIgnoreCase)).ToList()
        End Function

        ''' <summary>转换为白名单边列表（使用基因表达矩阵的索引）</summary>
        Public Function ToWhitelist(geneNames As String()) As List(Of (FromIdx As Integer, ToIdx As Integer))
            Dim nameMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To geneNames.Length - 1
                nameMap(geneNames(i)) = i
            Next

            Dim wl As New List(Of (Integer, Integer))()
            For Each edge In Edges
                Dim fromIdx As Integer = -1, toIdx As Integer = -1
                nameMap.TryGetValue(edge.TF, fromIdx)
                nameMap.TryGetValue(edge.TargetGene, toIdx)
                If fromIdx >= 0 AndAlso toIdx >= 0 AndAlso fromIdx <> toIdx Then
                    wl.Add((fromIdx, toIdx))
                End If
            Next
            Return wl
        End Function

        ''' <summary>获取所有涉及的基因名</summary>
        Public Function GetAllGeneNames() As String()
            Dim names As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            For Each e In Edges
                names.Add(e.TF)
                names.Add(e.TargetGene)
            Next
            Return names.ToArray()
        End Function

    End Class

End Namespace
