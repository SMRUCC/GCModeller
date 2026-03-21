Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MemeWriter

    ''' <summary>
    ''' 将PWM模型保存为MEME格式的文本文件
    ''' </summary>
    ''' <param name="motif">PWM模型对象</param>
    ''' <param name="outputPath">输出文件路径</param>
    ''' <param name="backgroundFreq">背景碱基频率（可选）</param>
    ''' <param name="nsites">位点数量（可选，默认为100）</param>
    ''' <param name="motifId">Motif ID（可选）</param>
    ''' <param name="url">URL链接（可选）</param>
    Public Shared Sub WriteMemeFormat(motif As Probability,
                                      outputPath As String,
                                      Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
                                      Optional nsites As Integer = 100,
                                      Optional motifId As String = Nothing,
                                      Optional url As String = Nothing)

        ' 设置默认背景频率
        If backgroundFreq Is Nothing Then
            backgroundFreq = New Dictionary(Of String, Double) From {
                {"A", 0.25},
                {"C", 0.25},
                {"G", 0.25},
                {"T", 0.25}
            }
        End If

        ' 获取motif ID
        If String.IsNullOrEmpty(motifId) Then
            motifId = $"MP{motif.name.GetHashCode().ToString("000000")}"
        End If

        Dim sb As New StringBuilder()

        ' 1. 写入文件头
        sb.AppendLine("MEME version 4.4")
        sb.AppendLine("ALPHABET= ACGT")
        sb.AppendLine("strands: + -")
        sb.AppendLine("Background letter frequencies (from file `background.bg'):")

        ' 2. 写入背景频率
        Dim bgFreqStr As String = String.Join(" ", backgroundFreq.Select(
            Function(kv) $"{kv.Key} {kv.Value.ToString("0.00000")}"))
        sb.AppendLine(bgFreqStr)

        ' 3. 写入MOTIF行
        sb.AppendLine($"MOTIF {motif.name} {motifId}")

        ' 4. 写入letter-probability matrix
        sb.AppendLine("letter-probability matrix:")

        ' 5. 写入矩阵参数
        Dim eValue As String = If(motif.pvalue > 0, motif.pvalue.ToString("0.0e-000"), "1.0e-999")
        sb.AppendLine($"alength= 4 w= {motif.width} nsites= {nsites} E= {eValue}")

        ' 6. 写入PWM矩阵数据
        ' 定义碱基顺序：A, C, G, T
        Dim baseOrder As String() = {"A", "C", "G", "T"}

        For Each residue As Residue In motif.region
            Dim values As New List(Of String)

            For Each baseChar As String In baseOrder
                Dim freq As Double = 0.0
                If residue.frequency IsNot Nothing AndAlso residue.frequency.ContainsKey(baseChar) Then
                    freq = residue.frequency(baseChar)
                End If
                values.Add(freq.ToString("0.000000"))
            Next

            sb.AppendLine(String.Join(" ", values))
        Next

        ' 7. 写入URL（如果有）
        If Not String.IsNullOrEmpty(url) Then
            sb.AppendLine($"URL {url}")
        End If

        ' 8. 保存到文件
        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8)
    End Sub

    ''' <summary>
    ''' 重载方法：使用默认参数保存
    ''' </summary>
    Public Shared Sub WriteMemeFormat(motif As Probability, outputPath As String)
        WriteMemeFormat(motif, outputPath, Nothing, 100, Nothing, Nothing)
    End Sub

End Class

''' <summary>
''' 扩展方法模块
''' </summary>
Public Module ProbabilityExtensions

    ''' <summary>
    ''' 扩展方法：直接保存PWM模型为MEME格式文件
    ''' </summary>
    <Runtime.CompilerServices.Extension>
    Public Sub SaveToMeme(motif As Probability,
                          outputPath As String,
                          Optional backgroundFreq As Dictionary(Of String, Double) = Nothing,
                          Optional nsites As Integer = 100,
                          Optional motifId As String = Nothing,
                          Optional url As String = Nothing)
        MemeWriter.WriteMemeFormat(motif, outputPath, backgroundFreq, nsites, motifId, url)
    End Sub

End Module
