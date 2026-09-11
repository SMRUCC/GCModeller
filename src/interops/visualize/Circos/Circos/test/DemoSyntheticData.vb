Imports System.Text
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas.Highlights

''' <summary>
''' 虚构的测试数据生成器
''' </summary>
''' <remarks>
''' 原来的演示代码所依赖的数据文件(``H:\5.14.circos\6.22\Af293.fna`` 等)已经不可用了，
''' 所以在这里通过程序化的方式虚构出一套用于测试 circos 绘图的数据。
''' </remarks>
Public Module DemoSyntheticData

    ''' <summary>
    ''' 虚构出来的基因组的各个染色体的长度
    ''' </summary>
    Public ReadOnly ChrSizes As Integer() = {200000, 150000, 120000}
    ''' <summary>
    ''' 虚构出来的各个染色体的平均 GC 含量
    ''' </summary>
    Public ReadOnly ChrGC As Double() = {0.42, 0.55, 0.36}

    ''' <summary>
    ''' 第 i 条染色体的名称
    ''' </summary>
    ''' <param name="i"></param>
    ''' <returns></returns>
    Public Function ChrName(i%) As String
        Return $"chr{i + 1}"
    End Function

    ''' <summary>
    ''' 使用随机数生成一条长度为 <paramref name="len"/> 的虚构的碱基序列
    ''' </summary>
    Public Function RandomSequence(len%, gc#, seed%) As String
        Dim rnd As New Random(seed)
        Dim sb As New StringBuilder(len)

        For i As Integer = 0 To len - 1
            If rnd.NextDouble < gc Then
                sb.Append(If(rnd.NextDouble < 0.5, "G"c, "C"c))
            Else
                sb.Append(If(rnd.NextDouble < 0.5, "A"c, "T"c))
            End If
        Next

        Return sb.ToString
    End Function

    ''' <summary>
    ''' 生成一套虚构的基因组序列数据
    ''' </summary>
    ''' <returns>``[chrName => nt sequence]``</returns>
    Public Function Genome() As Dictionary(Of String, String)
        Dim genomes As New Dictionary(Of String, String)

        For i As Integer = 0 To ChrSizes.Length - 1
            genomes(ChrName(i)) = RandomSequence(ChrSizes(i), ChrGC(i), seed:=100 + i)
        Next

        Return genomes
    End Function

    ''' <summary>
    ''' 生成虚构的基因组的 karyotype 骨架信息
    ''' </summary>
    ''' <returns></returns>
    Public Function Karyotype() As GenomeKaryotype
        Dim entries As New List(Of KaryotypeEntry)

        For i As Integer = 0 To ChrSizes.Length - 1
            entries.Add(New KaryotypeEntry With {
                .chrName = ChrName(i),
                .chrLabel = $"TEST_{ChrName(i)}",
                .start = 0,
                .end = ChrSizes(i),
                .color = $"chr{i + 1}"
            })
        Next

        Return New GenomeKaryotype(entries)
    End Function

    ''' <summary>
    ''' 计算滑动窗口的 GC 含量
    ''' </summary>
    Public Function GCContent(genome As Dictionary(Of String, String),
                              Optional winSize% = 2000,
                              Optional steps% = 2000) As ValueTrackData()

        Return WindowValues(genome, winSize, steps, Function(nt) nt.Count(Function(c) c = "G"c OrElse c = "C"c) / nt.Length)
    End Function

    ''' <summary>
    ''' 计算滑动窗口的 GC skew: ``(G - C) / (G + C)``
    ''' </summary>
    Public Function GCSkew(genome As Dictionary(Of String, String),
                           Optional winSize% = 2000,
                           Optional steps% = 2000) As ValueTrackData()

        Return WindowValues(genome, winSize, steps,
            Function(nt)
                Dim g% = nt.Count(Function(c) c = "G"c)
                Dim c% = nt.Count(Function(c) c = "C"c)

                If g + c = 0 Then
                    Return 0R
                Else
                    Return (g - c) / (g + c)
                End If
            End Function)
    End Function

    ''' <summary>
    ''' 对虚构的基因组序列做滑窗扫描
    ''' </summary>
    Public Function WindowValues(genome As Dictionary(Of String, String),
                                 winSize%,
                                 steps%,
                                 getValue As Func(Of String, Double)) As ValueTrackData()

        Dim out As New List(Of ValueTrackData)

        For i As Integer = 0 To ChrSizes.Length - 1
            Dim name$ = ChrName(i)
            Dim seq$ = genome(name)

            For start As Integer = 0 To seq.Length - 1 Step steps
                Dim nt$ = seq.Substring(start, Math.Min(winSize, seq.Length - start))

                out.Add(New ValueTrackData With {
                    .chr = name,
                    .start = start,
                    .end = start + steps,
                    .value = Math.Round(getValue(nt), 4)
                })
            Next
        Next

        Return out.ToArray
    End Function

    ''' <summary>
    ''' 生成虚构的基因区间(用于 tile 以及 text 标签的绘制)
    ''' </summary>
    Public Function Genes(Optional countPerChr% = 30, Optional geneLen% = 3000) As RegionTrackData()
        Dim rnd As New Random(2026)
        Dim genes As New List(Of RegionTrackData)

        For i As Integer = 0 To ChrSizes.Length - 1
            Dim size% = ChrSizes(i)

            For j As Integer = 0 To countPerChr - 1
                Dim start% = rnd.Next(0, size - geneLen)

                genes.Add(New RegionTrackData With {
                    .chr = ChrName(i),
                    .start = start,
                    .end = start + geneLen
                })
            Next
        Next

        Return genes.OrderBy(Function(g) g.chr).ThenBy(Function(g) g.start).ToArray
    End Function

    ''' <summary>
    ''' 生成虚构的基因名称标签
    ''' </summary>
    Public Function GeneLabels(genes As RegionTrackData(), Optional take% = 18) As TextTrackData()
        Dim rnd As New Random(77)
        Dim stepN% = Math.Max(1, genes.Length \ take)
        Dim labels As New List(Of TextTrackData)

        For i As Integer = 0 To genes.Length - 1 Step stepN
            Dim gene = genes(i)

            labels.Add(New TextTrackData With {
                .chr = gene.chr,
                .start = gene.start,
                .end = gene.end,
                .text = $"{gene.chr}_gene{j + 1}"
            })
        Next

        Return labels.ToArray
    End Function

    ''' <summary>
    ''' 生成虚构的基因组区段之间的连接关系(用于 ``&lt;links>`` 块的绘制)
    ''' </summary>
    Public Function Links(Optional countPerPair% = 6, Optional linkLen% = 12000) As LinkData()
        Dim rnd As New Random(314)
        Dim links As New List(Of LinkData)

        For i As Integer = 0 To ChrSizes.Length - 1
            Dim j% = (i + 1) Mod ChrSizes.Length
            Dim aSize% = ChrSizes(i)
            Dim bSize% = ChrSizes(j)

            For k As Integer = 0 To countPerPair - 1
                Dim aStart% = rnd.Next(0, aSize - linkLen)
                Dim bStart% = rnd.Next(0, bSize - linkLen)

                links.Add(New LinkData(
                    New RegionTrackData With {.chr = ChrName(i), .start = aStart, .end = aStart + linkLen},
                    New RegionTrackData With {.chr = ChrName(j), .start = bStart, .end = bStart + linkLen}))
            Next
        Next

        Return links.ToArray
    End Function

    ''' <summary>
    ''' 生成虚构的高亮区域(用于 ``&lt;highlights>`` 块的绘制)
    ''' </summary>
    Public Function Highlights(Optional countPerChr% = 4, Optional regionLen% = 15000) As DemoHighlights
        Dim rnd As New Random(2718)
        Dim source As New List(Of ValueTrackData)

        For i As Integer = 0 To ChrSizes.Length - 1
            Dim size% = ChrSizes(i)

            For j As Integer = 0 To countPerChr - 1
                Dim start% = rnd.Next(0, size - regionLen)

                source.Add(New ValueTrackData With {
                    .chr = ChrName(i),
                    .start = start,
                    .end = start + regionLen,
                    .value = 1
                })
            Next
        Next

        Return New DemoHighlights(source)
    End Function

    ''' <summary>
    ''' 生成虚构的散点数据(从 GC 含量数据之中抽样)
    ''' </summary>
    Public Function Sample(values As ValueTrackData(), Optional n% = 40) As ValueTrackData()
        Dim stepN% = Math.Max(1, values.Length \ n)
        Dim out As New List(Of ValueTrackData)

        For i As Integer = 0 To values.Length - 1 Step stepN
            out.Add(values(i))
        Next

        Return out.ToArray
    End Function
End Module

''' <summary>
''' 用于演示的高亮数据文档
''' </summary>
Public Class DemoHighlights : Inherits Highlights

    Sub New(source As IEnumerable(Of ValueTrackData))
        Call MyBase.New(source)
    End Sub
End Class
