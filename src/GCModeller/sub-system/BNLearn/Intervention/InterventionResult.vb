Namespace Intervention

    ''' <summary>
    ''' 干预分析结果
    ''' </summary>
    Public Class InterventionResult

        ''' <summary>干预定义</summary>
        Public Property Spec As InterventionSpec

        ''' <summary>野生型（对照）表达值</summary>
        Public Property WildtypeMeans As Double()

        ''' <summary>干扰后表达值</summary>
        Public Property MutantMeans As Double()

        ''' <summary>表达变化量（Mutant - Wildtype）</summary>
        Public Property FoldChanges As Double()

        ''' <summary>表达变化百分比</summary>
        Public Property PercentChanges As Double()

        ''' <summary>变化是否显著</summary>
        Public Property IsSignificant As Boolean()

        ''' <summary>基因名称列表</summary>
        Public Property GeneNames As String()

        ''' <summary>Z-Score（标准化效应量）</summary>
        Public Property ZScores As Double()

        ''' <summary>野生型标准差（每个基因）</summary>
        Public Property WildtypeSDs As Double()


        ''' <summary>受影响的基因数量（显著变化的）</summary>
        Public ReadOnly Property NAffected As Integer
            Get
                If IsSignificant Is Nothing Then Return 0
                Dim count As Integer = 0
                For Each s In IsSignificant
                    If s Then count += 1
                Next
                Return count
            End Get
        End Property

        ''' <summary>动态级联结果（多时间步）</summary>
        Public Property DynamicTrajectory As Double(,)()  ' [timeStep, geneIdx] = samples

        ''' <summary>获取变化最大的前N个基因</summary>
        Public Function GetTopChangedGenes(n As Integer) As List(Of (GeneName As String, FoldChange As Double, PercentChange As Double))
            Dim indexed As New List(Of (index As Integer, foldChange As Double, percentChange As Double))()
            For i = 0 To FoldChanges.Length - 1
                indexed.Add((i, FoldChanges(i), PercentChanges(i)))
            Next

            indexed.Sort(Function(a, b) Math.Abs(b.foldChange).CompareTo(Math.Abs(a.foldChange)))

            Dim result As New List(Of (String, Double, Double))()
            For i = 0 To Math.Min(n - 1, indexed.Count - 1)
                result.Add((GeneNames(indexed(i).Item1), indexed(i).foldChange, indexed(i).percentChange))
            Next
            Return result
        End Function

        ''' <summary>输出摘要字符串</summary>
        Public Overrides Function ToString() As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine(String.Format("=== 干预分析结果: {0} ({1}) ===", Spec.GeneName, Spec.Mode.ToString()))
            sb.AppendLine(String.Format("受影响基因数: {0}/{1}", NAffected, GeneNames.Length))
            sb.AppendLine()
            sb.AppendLine("Top 变化基因:")
            For Each item In GetTopChangedGenes(20)
                sb.AppendLine(String.Format("  {0}: FC={1:F4}  ({2:F1}%)", item.GeneName, item.FoldChange, item.PercentChange))
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace