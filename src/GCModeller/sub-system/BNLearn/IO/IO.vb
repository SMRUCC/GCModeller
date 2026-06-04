Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BNLearn.Core
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace IO

    ''' <summary>
    ''' 数据读写工具
    ''' </summary>
    Public Module BnIO

        ' ==================== 读取基因表达矩阵 ====================

        ''' <summary>
        ''' 从 CSV/TSV 文件读取基因表达矩阵
        ''' 格式：第一列为基因名，第一行为样本名，其余为表达值
        ''' </summary>
        ''' 
        <Extension>
        Public Function ReadGeneExpressionMatrix(expr As Matrix, Optional sampleinfo As SampleInfo() = Nothing) As Core.GeneExpressionData
            Dim matrixRows As New List(Of Double())()
            Dim geneNames As String() = expr.rownames
            Dim sampleNames As String() = expr.sampleID
            Dim nSamples As Integer = sampleNames.Length

            For Each gene As DataFrameRow In expr.expression
                Call matrixRows.Add(gene.experiments)
            Next

            ' 转置为 [gene, sample]
            Dim matrix As Double(,) = New Double(geneNames.Count - 1, nSamples - 1) {}
            For i = 0 To geneNames.Count - 1
                For j = 0 To nSamples - 1
                    matrix(i, j) = matrixRows(i)(j)
                Next
            Next

            Return New Core.GeneExpressionData() With {
                .GeneNames = geneNames.ToArray(),
                .SampleNames = sampleNames,
                .Matrix = matrix,
                .TimePoints = Enumerable.Repeat(0.0, nSamples).ToArray()
            }
        End Function

        ' ==================== 读取先验调控网络 ====================

        ''' <summary>
        ''' 从 CSV/TSV 读取先验调控网络
        ''' 格式：TF, TargetGene, RegulationType, Confidence, Evidence
        ''' </summary>
        Public Function ReadPriorNetwork(TRN As IEnumerable(Of RegulatoryEdge)) As Core.PriorNetwork
            Dim prior As New Core.PriorNetwork()

            For Each edge As RegulatoryEdge In TRN
                Call prior.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            Next

            Return prior
        End Function

        ' ==================== 写入网络结构 ====================

        ''' <summary>
        ''' 将网络结构写入 TSV 文件
        ''' 格式：From, To, EdgeType
        ''' </summary>
        Public Sub WriteNetworkStructure(network As Core.BayesianNetwork, filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("From" & vbTab & "To" & vbTab & "EdgeType")

            For Each node In network.Nodes
                For Each parentIdx In node.Parents
                    sb.AppendLine(String.Format("{0}{1}{2}{3}regulation",
                        network.Nodes(parentIdx).Name, vbTab, node.Name, vbTab))
                Next
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 写入 CPD 参数 ====================

        ''' <summary>
        ''' 将 CPD 参数写入 TSV 文件
        ''' </summary>
        Public Sub WriteCPDParameters(network As Core.BayesianNetwork, filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("Gene" & vbTab & "Intercept" & vbTab & "Parents" & vbTab &
                          "Coefficients" & vbTab & "ResidualSD" & vbTab & "RSquared")

            For Each node In network.Nodes
                If node.CPD Is Nothing Then Continue For
                Dim cpd As Core.BnCPD = node.CPD

                Dim parentNames As String = ""
                Dim coeffStr As String = ""
                If cpd.ParentIndices IsNot Nothing AndAlso cpd.ParentIndices.Length > 0 Then
                    parentNames = String.Join(";", cpd.ParentIndices.Select(Function(p) network.Nodes(p).Name))
                    coeffStr = String.Join(";", cpd.Coeffs.Select(Function(c) c.ToString("F6")))
                End If

                sb.AppendLine(String.Format("{0}{1}{2:F6}{3}{4}{5}{6}{7}{8:F6}{9}{10:F4}",
                    node.Name, vbTab, cpd.Intercept, vbTab, parentNames, vbTab,
                    coeffStr, vbTab, cpd.ResidualSD, vbTab, cpd.RSquared))
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ' ==================== 写入干预分析结果 ====================

        ''' <summary>
        ''' 将干预分析结果写入 TSV 文件
        ''' </summary>
        Public Sub WriteInterventionResult(result As Intervention.InterventionResult, filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("# 干预分析: {0} ({1})", result.Spec.GeneName, result.Spec.Mode.ToString()))
            sb.AppendLine("Gene" & vbTab & "Wildtype" & vbTab & "Mutant" & vbTab &
                          "FoldChange" & vbTab & "PercentChange" & vbTab & "Significant")

            For i = 0 To result.GeneNames.Length - 1
                sb.AppendLine(String.Format("{0}{1}{2:F4}{3}{4:F4}{5}{6:F4}{7}{8:F1}%{9}{10}",
                    result.GeneNames(i), vbTab,
                    result.WildtypeMeans(i), vbTab,
                    result.MutantMeans(i), vbTab,
                    result.FoldChanges(i), vbTab,
                    result.PercentChanges(i), vbTab,
                    If(result.IsSignificant(i), "Yes", "No")))
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

        ''' <summary>
        ''' 将批量干预结果写入汇总 TSV
        ''' </summary>
        Public Sub WriteBatchInterventionResults(results As List(Of Intervention.InterventionResult), filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("InterventionGene" & vbTab & "Mode" & vbTab & "TargetGene" & vbTab &
                          "FoldChange" & vbTab & "PercentChange" & vbTab & "Significant")

            For Each result In results
                For i = 0 To result.GeneNames.Length - 1
                    sb.AppendLine(String.Format("{0}{1}{2}{3}{4}{5}{6:F4}{7}{8:F1}%{9}{10}",
                        result.Spec.GeneName, vbTab,
                        result.Spec.Mode.ToString(), vbTab,
                        result.GeneNames(i), vbTab,
                        result.FoldChanges(i), vbTab,
                        result.PercentChanges(i), vbTab,
                        If(result.IsSignificant(i), "Yes", "No")))
                Next
            Next

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        End Sub

    End Module

End Namespace
