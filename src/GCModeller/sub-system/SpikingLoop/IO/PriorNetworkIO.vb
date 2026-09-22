' ============================================================================
' PriorNetworkIO.vb — 先验调控网络（TF-Target）的 CSV 读写
'
' 期望的表头：TF,TargetGene,RegulationType,Confidence,Evidence
' 例如：
'   TF,TargetGene,RegulationType,Confidence,Evidence
'   lutR,lutP,repression,0.95,ChIP-seq and EMSA confirmed binding
'   abrB,spoA,activation,0.85,Regulator of sporulation initiation
'
' 解析要点（与 BNLearn 的通用 CSV 反序列化不兼容，故需显式解析）：
'   · RegulationType 在文件里是 activation / repression 这样的文本，
'     而 RegulatoryEdge.RegulationType 是 Effector 枚举（Activator/Inhibitor/Unknown），
'     因此需要一张文本 → 枚举的映射表；
'   · 分隔符同时支持逗号 / Tab / 分号（上游导出的表格式不统一）。
'
' 关系说明：本模块与 GEARS 项目中的同名模块功能等价（表头与映射规则一致），
' 这里有意在 SpikingLoop 内自带一份实现，而非把 GEARS 变成 SpikingLoop 的工程依赖，
' 以保持两个子系统的独立性（GEARS 对本项目仍是可选的外部工具）。
' ============================================================================

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO

Namespace IO

    ''' <summary>先验调控网络的 CSV/TSV 读写</summary>
    Public Module PriorNetworkIO

        ''' <summary>CSV/TSV 支持的分隔符</summary>
        ReadOnly delimiters As Char() = {","c, ControlChars.Tab, ";"c}

        ''' <summary>
        ''' 从 CSV/TSV 文件加载先验调控网络。
        ''' </summary>
        ''' <param name="path">文件路径，表头为 TF,TargetGene,RegulationType,Confidence,Evidence</param>
        Public Function LoadPriorNetwork(path As String) As PriorNetwork
            If String.IsNullOrWhiteSpace(path) Then
                Throw New ArgumentException("先验网络路径不能为空", NameOf(path))
            End If
            If Not File.Exists(path) Then
                Throw New FileNotFoundException($"先验调控网络文件不存在: {path}", path)
            End If

            Dim prior = BnIO.ReadPriorNetwork(ParseRegulatoryEdges(File.ReadAllLines(path)))

            If prior.Edges.Count = 0 Then
                Throw New InvalidOperationException(
                    $"先验调控网络未解析到任何调控边: {path}；" &
                    "请确认表头为 TF,TargetGene,RegulationType,Confidence,Evidence")
            End If

            Return prior
        End Function

        ''' <summary>
        ''' 解析内存中的 CSV 文本行所描述的所有调控边。
        ''' </summary>
        ''' <param name="lines">文本行集合，首行为表头</param>
        Public Iterator Function ParseRegulatoryEdges(lines As String()) As IEnumerable(Of RegulatoryEdge)
            If lines Is Nothing OrElse lines.Length = 0 Then
                Return
            End If

            Dim header = SplitCsvLine(lines(0))
            Dim colTF = IndexOfColumn(header, "TF", 0)
            Dim colTarget = IndexOfColumn(header, "TargetGene", 1)
            Dim colType = IndexOfColumn(header, "RegulationType", 2)
            Dim colConf = IndexOfColumn(header, "Confidence", 3)
            Dim colEvidence = IndexOfColumn(header, "Evidence", 4)

            For i = 1 To lines.Length - 1
                If String.IsNullOrWhiteSpace(lines(i)) Then Continue For

                Dim tokens = SplitCsvLine(lines(i))
                If tokens.Length <= colTarget Then Continue For

                Dim tf = tokens(colTF).Trim()
                Dim target = tokens(colTarget).Trim()
                If String.IsNullOrEmpty(tf) OrElse String.IsNullOrEmpty(target) Then Continue For

                Dim regType = Effector.Unknown
                Dim confidence = 1.0
                Dim evidence = ""

                If colType >= 0 AndAlso colType < tokens.Length Then
                    regType = ParseRegulationType(tokens(colType))
                End If
                If colConf >= 0 AndAlso colConf < tokens.Length Then
                    Dim parsed As Double
                    If Double.TryParse(tokens(colConf).Trim(), parsed) Then confidence = parsed
                End If
                If colEvidence >= 0 AndAlso colEvidence < tokens.Length Then
                    evidence = tokens(colEvidence).Trim()
                End If

                Yield New RegulatoryEdge With {
                    .TF = tf,
                    .TargetGene = target,
                    .RegulationType = regType,
                    .Confidence = confidence,
                    .Evidence = evidence
                }
            Next
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
                Case "activation", "activate", "activator", "activating", "up", "positive", "1"
                    Return Effector.Activator
                Case "repression", "repress", "repressor", "inhibition", "inhibit", "inhibitor", "down", "negative", "-1"
                    Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
        End Function

        ''' <summary>把先验调控网络写回 CSV（首行表头，便于人工检查与结果留档）</summary>
        Public Sub WritePriorNetwork(path As String, prior As PriorNetwork)
            If prior Is Nothing Then Return

            Dim sb As New StringBuilder()
            sb.AppendLine("TF,TargetGene,RegulationType,Confidence,Evidence")

            For Each edge As RegulatoryEdge In prior.Edges.SafeQuery()
                sb.AppendLine(String.Join(",",
                    CsvField(edge.TF),
                    CsvField(edge.TargetGene),
                    edge.RegulationType.ToString(),
                    edge.Confidence.ToString("G8", Globalization.CultureInfo.InvariantCulture),
                    CsvField(If(edge.Evidence, ""))))
            Next

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8)
        End Sub

        Private Function SplitCsvLine(line As String) As String()
            Return line.Split(delimiters, StringSplitOptions.None)
        End Function

        Private Function IndexOfColumn(header As String(), name As String, defaultIndex As Integer) As Integer
            For i = 0 To header.Length - 1
                If String.Equals(header(i).Trim(), name, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next
            Return defaultIndex
        End Function

        ''' <summary>最小化 CSV 转义：含分隔符或引号时用双引号包裹并转义内部引号</summary>
        Private Function CsvField(text As String) As String
            If String.IsNullOrEmpty(text) Then Return ""

            If text.IndexOfAny(delimiters) >= 0 OrElse text.Contains("""") Then
                Return """" & text.Replace("""", """""") & """"
            End If
            Return text
        End Function

    End Module

End Namespace
