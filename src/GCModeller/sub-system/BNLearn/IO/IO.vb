#Region "Microsoft.VisualBasic::7af0dbd4fb93c4bc64b91de5a6955e68, sub-system\BNLearn\IO\IO.vb"

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

    '   Total Lines: 178
    '    Code Lines: 113 (63.48%)
    ' Comment Lines: 31 (17.42%)
    '    - Xml Docs: 77.42%
    ' 
    '   Blank Lines: 34 (19.10%)
    '     File Size: 7.43 KB


    '     Module BnIO
    ' 
    '         Function: ReadGeneExpressionMatrix, ReadPriorNetwork
    ' 
    '         Sub: WriteBatchInterventionResults, WriteCPDParameters, WriteInterventionResult, WriteNetworkStructure
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
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
        Public Function ReadGeneExpressionMatrix(expr As Matrix,
                                                 Optional sampleinfo As SampleInfo() = Nothing,
                                                 Optional time_label As String = "time") As Core.GeneExpressionData

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

            Dim t As Double() = Enumerable.Repeat(0.0, nSamples).ToArray()

            If Not sampleinfo.IsNullOrEmpty Then
                Dim sampleSet = sampleinfo.ToDictionary(Function(a) a.ID)
                Dim sort = sampleNames.Select(Function(id) sampleSet(id)).ToArray

                t = sort _
                    .Select(Function(sample) Val(sample(time_label))) _
                    .ToArray
            End If

            Return New Core.GeneExpressionData() With {
                .GeneNames = geneNames.ToArray(),
                .SampleNames = sampleNames,
                .Matrix = matrix,
                .TimePoints = t
            }
        End Function

        ' ==================== 读取单样本外部转录组向量 ====================

        ''' <summary>
        ''' 从外部转录组向量文件读取单样本（或一组均值）的基因表达水平
        ''' 文件格式：两列基因名与表达值，支持 CSV/TSV，首行可为表头（gene / expression，大小写不敏感）
        ''' 例如：
        '''   gene,expression
        '''   codY,12.3
        '''   comK,4.5
        ''' 返回 基因名 → 表达值 的键值对字典。基因名按 OrdinalIgnoreCase 比较去重（后者覆盖前者）。
        ''' 若文件不存在或解析不到任何记录则抛出友好异常。
        ''' </summary>
        <Extension>
        Public Function ReadExpressionVector(path As String) As Dictionary(Of String, Double)
            If Not File.Exists(path) Then
                Throw New Exception(String.Format("外部转录组向量文件不存在: {0}", path))
            End If

            Dim lines As String() = File.ReadAllLines(path)
            If lines.Length = 0 Then
                Throw New Exception(String.Format("外部转录组向量文件为空: {0}", path))
            End If

            Dim delim As Char() = New Char() {","c, vbTab, ";"c}
            Dim result As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            ' 判断首行是否为表头
            Dim startRow As Integer = 0
            Dim firstTokens As String() = lines(0).Split(delim, StringSplitOptions.RemoveEmptyEntries)
            If firstTokens.Length >= 2 Then
                Dim col0 As String = firstTokens(0).Trim()
                Dim col1 As String = firstTokens(1).Trim()
                If col0.Equals("gene", StringComparison.OrdinalIgnoreCase) OrElse
                   col0.Equals("genes", StringComparison.OrdinalIgnoreCase) Then
                    startRow = 1
                End If
            End If

            For i = startRow To lines.Length - 1
                Dim line As String = lines(i).Trim()
                If String.IsNullOrEmpty(line) Then Continue For

                Dim tokens As String() = line.Split(delim, StringSplitOptions.RemoveEmptyEntries)
                If tokens.Length < 2 Then Continue For

                Dim gene As String = tokens(0).Trim()
                Dim value As Double
                If Not Double.TryParse(tokens(1).Trim(), value) Then Continue For

                ' 按大小写不敏感去重：后者覆盖前者
                result(gene) = value
            Next

            If result.Count = 0 Then
                Throw New Exception(String.Format("外部转录组向量文件未解析到任何有效记录: {0}", path))
            End If

            Return result
        End Function

        ' ==================== 读取先验调控网络 ====================

        ''' <summary>
        ''' 从 CSV/TSV 读取先验调控网络
        ''' 格式：TF, TargetGene, RegulationType, Confidence, Evidence
        ''' </summary>
        Public Function ReadPriorNetwork(TRN As IEnumerable(Of RegulatoryEdge)) As Core.PriorNetwork
            Dim prior As New Core.PriorNetwork()

            For Each edge As RegulatoryEdge In TRN.SafeQuery
                Call prior.AddEdge(edge.TF, edge.TargetGene, edge.RegulationType, edge.Confidence, edge.Evidence)
            Next

            Return prior
        End Function

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

