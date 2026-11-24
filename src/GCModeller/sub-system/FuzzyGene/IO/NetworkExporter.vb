Imports System.Runtime.CompilerServices
Imports System.Text

Namespace IO

    ''' <summary>
    ''' 模型的可解释性导出（把规则与边表写到普通目录）。
    ''' </summary>
    Public Module NetworkExporter

        ''' <summary>
        ''' 把网络的可解释内容导出到目录：<c>rules.txt</c>、<c>edges.sif</c>、<c>edges.csv</c>、<c>summary.txt</c>。
        ''' </summary>
        ''' <param name="network">已拟合的网络</param>
        ''' <param name="dir">输出目录（不存在时自动创建）</param>
        ''' <returns>实际写入的文件路径</returns>
        <Extension>
        Public Function Export(network As FuzzyGeneNetwork, dir As String) As String()
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If
            If String.IsNullOrWhiteSpace(dir) Then
                Throw New ArgumentException("the output directory is empty", NameOf(dir))
            End If

            Call dir.MakeDir

            Dim rules As String = System.IO.Path.Combine(dir, "rules.txt")
            Dim sif As String = System.IO.Path.Combine(dir, "edges.sif")
            Dim csv As String = System.IO.Path.Combine(dir, "edges.csv")
            Dim summary As String = System.IO.Path.Combine(dir, "summary.txt")

            Call network.RulesText().SaveTo(rules, New UTF8Encoding(False))
            Call network.EdgesToSif().SaveTo(sif, New UTF8Encoding(False))
            Call network.EdgesToCsv().SaveTo(csv, New UTF8Encoding(False))
            Call BuildSummary(network).SaveTo(summary, New UTF8Encoding(False))

            Return New String() {rules, sif, csv, summary}
        End Function

        Private Function BuildSummary(network As FuzzyGeneNetwork) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"FuzzyGene model, generated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
            Call sb.AppendLine($"mode      : {network.Mode}")
            Call sb.AppendLine($"genes     : {network.NGene} ({network.NModelled} modelled)")
            Call sb.AppendLine($"edges     : {network.Edges.Length}")
            Call sb.AppendLine($"activation: {network.Edges.Count(Function(e) e.IsActivation)}")
            Call sb.AppendLine($"repression: {network.Edges.Count(Function(e) Not e.IsActivation)}")
            Call sb.AppendLine($"prior edge: {network.Edges.Count(Function(e) e.IsPrior)}")
            Call sb.AppendLine($"config    : {network.Config}")
            Call sb.AppendLine()

            Call sb.AppendLine("top target genes by R2:")
            Call sb.AppendLine("  Gene,Regulators,R2,RMSE,Rules")

            For Each model In network.Models _
                    .Where(Function(m) m IsNot Nothing) _
                    .OrderByDescending(Function(m) m.R2) _
                    .Take(50)
                Call sb.AppendLine($"  {model.TargetGene},{String.Join(";", If(model.Regulators, New String() {}))}," &
                                   $"{model.R2:G4},{model.RMSE:G4},{model.RuleCount}")
            Next

            Call sb.AppendLine()
            Call sb.AppendLine("top edges by |weight|:")

            For Each edge In network.Edges _
                    .OrderByDescending(Function(e) System.Math.Abs(e.Weight)) _
                    .Take(50)
                Call sb.AppendLine("  " & edge.ToString())
            Next

            Return sb.ToString()
        End Function

    End Module
End Namespace