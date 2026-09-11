Imports System.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.TSSAR.Models
Imports SMRUCC.genomics.ComponentModel.Loci

''' <summary>
''' 对连续的 TSS 位置执行聚类合并，只保留每一个簇之中最显著的位点。
''' </summary>
''' <remarks>
''' 对应 TSSAR 的 ``--cluster`` 选项：两个显著位置之间的距离不超过
''' <c>range</c>（缺省 3 nt）时被视为"连续"，合并为一个簇；
''' 簇内"最佳"位点由评分模式决定 —— ``p`` 模式取 p 值最小者，
''' ``d`` 模式取 [+] 与 [-] 峰值差最大者。
''' </remarks>
Public Module TssClustering

    ''' <summary>
    ''' 按链分别对显著 TSS 位置执行聚类。
    ''' </summary>
    ''' <param name="sites">未聚类的显著 TSS 位置（任意顺序）。</param>
    ''' <param name="range">合并为同一簇的最大间距（nt）。</param>
    ''' <param name="scoreMode">评分模式。</param>
    ''' <returns>聚类之后的 TSS 位置：先正链（按位置升序）、后负链（按位置升序）。</returns>
    Public Function Cluster(sites As IEnumerable(Of TssSite), range As Integer, scoreMode As ScoreModes) As TssSite()
        Dim result As New List(Of TssSite)

        result.AddRange(ClusterStrand(sites, Strands.Forward, range, scoreMode))
        result.AddRange(ClusterStrand(sites, Strands.Reverse, range, scoreMode))

        Return result.ToArray()
    End Function

    Private Function ClusterStrand(sites As IEnumerable(Of TssSite),
                                   strand As Strands,
                                   range As Integer,
                                   scoreMode As ScoreModes) As TssSite()

        Dim ordered As TssSite() = sites _
            .Where(Function(site) site.Strand = strand) _
            .OrderBy(Function(site) site.Position) _
            .ToArray()

        Dim result As New List(Of TssSite)

        If ordered.Length = 0 Then
            Return result.ToArray()
        End If

        Dim current As New List(Of TssSite) From {ordered(0)}

        For i As Integer = 1 To ordered.Length - 1
            If ordered(i).Position - current(current.Count - 1).Position <= range Then
                current.Add(ordered(i))
            Else
                result.Add(Best(current, scoreMode))
                current = New List(Of TssSite) From {ordered(i)}
            End If
        Next

        result.Add(Best(current, scoreMode))

        Return result.ToArray()
    End Function

    ''' <summary>
    ''' 从同一个簇之中选出最显著的位点。
    ''' </summary>
    Private Function Best(cluster As List(Of TssSite), scoreMode As ScoreModes) As TssSite
        Dim selected As TssSite = cluster(0)

        For i As Integer = 1 To cluster.Count - 1
            Dim candidate As TssSite = cluster(i)

            If scoreMode = ScoreModes.PValue Then
                If candidate.PValue < selected.PValue Then
                    selected = candidate
                End If
            Else
                If candidate.PeakDifference > selected.PeakDifference Then
                    selected = candidate
                End If
            End If
        Next

        Return selected
    End Function
End Module

''' <summary>
''' TSSAR 结果文件的输出（BED 格式）。
''' </summary>
Public Module BedWriter

    ''' <summary>
    ''' 写出被注释为 TSS 的位置（BED 格式）。
    ''' </summary>
    ''' <param name="sites">TSS 位置（顺序即为输出顺序）。</param>
    ''' <param name="path">输出文件路径。</param>
    ''' <param name="scoreMode">评分模式。</param>
    Public Sub WriteTss(sites As TssSite(), path As String, scoreMode As ScoreModes)
        Dim sb As New StringBuilder()
        Dim index As Integer = 1

        For Each site As TssSite In sites
            Dim score As String

            If scoreMode = ScoreModes.PValue Then
                score = FormatScientific(site.PValue)
            Else
                score = site.PeakDifference.ToString("F6", Globalization.CultureInfo.InvariantCulture)
            End If

            Dim strand As String = If(site.Strand = Strands.Forward, "+", "-")
            Dim id As String = $"TSS_{index.ToString("D5")}"

            site.TssId = id

            ' 与 TSSAR.pl 一致：chr, pos-1, pos, id, score, strand
            sb.AppendLine(String.Join(ControlChars.Tab,
                                      site.Chromosome,
                                      site.Position - 1,
                                      site.Position,
                                      id,
                                      score,
                                      strand))

            index += 1
        Next

        IO.File.WriteAllText(path, sb.ToString())
    End Sub

    ''' <summary>
    ''' 写出无法建模的基因组区间（``Dump.bed``）。
    ''' </summary>
    Public Sub WriteDump(regions As DumpRegion(), path As String)
        Dim sb As New StringBuilder()
        Dim index As Integer = 1

        For Each region As DumpRegion In regions
            Dim strand As String = If(region.Strand = Strands.Forward, "+", "-")
            Dim id As String = $"DumpRegion_{index.ToString("D4")}"

            sb.AppendLine(String.Join(ControlChars.Tab,
                                      region.Chromosome,
                                      region.Start - 1,
                                      region.Ends,
                                      id,
                                      ".",
                                      strand))

            index += 1
        Next

        IO.File.WriteAllText(path, sb.ToString())
    End Sub

    ''' <summary>
    ''' 按照 C 语言 ``printf("%e")`` 的格式输出数值（小写 e，指数至少 2 位）。
    ''' </summary>
    Public Function FormatScientific(value As Double) As String
        If value = 0.0 Then
            Return "0.000000e+00"
        End If

        Dim text As String = value.ToString("0.000000E+00", Globalization.CultureInfo.InvariantCulture)
        Dim index As Integer = text.IndexOf("E"c)

        If index < 0 Then
            Return text
        End If

        Dim mantissa As String = text.Substring(0, index)
        Dim exponent As String = text.Substring(index + 1)
        Dim sign As String = exponent.Substring(0, 1)
        Dim digits As String = exponent.Substring(1).TrimStart("0"c)

        If digits.Length < 2 Then
            digits = digits.PadLeft(2, "0"c)
        End If

        Return mantissa & "e" & sign & digits
    End Function
End Module
