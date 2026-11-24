Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace Intervention

    <HideModuleName>
    Public Module Extensions

        ''' <summary>
        ''' 将干预分析结果写入 TSV 文件
        ''' </summary>
        ''' 
        <Extension>
        Public Sub WriteInterventionResult(result As InterventionResult, filePath As String)
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
        ''' 
        <Extension>
        Public Sub WriteBatchInterventionResults(results As IEnumerable(Of InterventionResult), filePath As String)
            Dim sb As New StringBuilder()
            sb.AppendLine("InterventionGene" & vbTab & "Mode" & vbTab & "TargetGene" & vbTab &
                          "FoldChange" & vbTab & "PercentChange" & vbTab & "Significant")

            For Each result As InterventionResult In results.SafeQuery
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