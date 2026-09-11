Imports System.Drawing
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.Karyotype
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Module Program

    Sub New()
        ' register .net image driver for draw circos in .net via gdi+
        Call ImageDriver.Register()
    End Sub

    Sub Main(args As String())
        Dim outDIR As String = "Z:\circos-test\smoke\"

        Call Directory.CreateDirectory(outDIR)
        Call smokeTest(outDIR)

        Console.WriteLine("DONE.")
    End Sub

    Private Function writeSeq(len%, gc#) As String
        Dim rnd As New Random(len + CInt(gc * 100))
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

    Private Sub smokeTest(outDIR As String)
        Dim sizes As Integer() = {200000, 150000, 120000}
        Dim gcs As Double() = {0.42, 0.55, 0.36}
        Dim colors As String() = {"chr1", "chr2", "chr3"}
        Dim karyos As New List(Of Karyotype)
        Dim seqs As New Dictionary(Of String, String)

        For i As Integer = 0 To sizes.Length - 1
            Dim name$ = $"chr{i + 1}"
            Dim seq As String = writeSeq(sizes(i), gcs(i))

            seqs(name) = seq

            karyos.Add(New Karyotype With {
                .chrName = name,
                .chrLabel = $"Test {name}",
                .start = 0,
                .end = sizes(i),
                .color = colors(i)
            })
        Next

        Dim skeleton As New KaryotypeChromosomes(karyos)

        ' GC content sliding window
        Dim bins As New List(Of ValueTrackData)
        Dim winSize% = 2000
        Dim steps% = 2000

        For Each k As Karyotype In karyos
            Dim seq$ = seqs(k.chrName)

            For start As Integer = 0 To seq.Length - 1 Step steps
                Dim nt$ = seq.Substring(start, Math.Min(winSize, seq.Length - start))
                Dim gc# = nt.Count(Function(c) c = "G"c OrElse c = "C"c) / nt.Length

                bins.Add(New ValueTrackData With {
                    .chr = k.chrName,
                    .start = start,
                    .end = start + steps,
                    .value = Math.Round(gc, 4)
                })
            Next
        Next

        Dim circos As Circos = Circos.CreateObject()

        circos.skeletonKaryotype = skeleton
        circos.karyotype = "data/karyotype.txt"
        circos.chromosomes_units = "1000000"
        circos.includes.Add(New Ideogram(circos))
        circos.includes.Add(New Ticks(circos))

        circos.Ideogram.Ideogram.show_label = "yes"
        circos.Ideogram.Ideogram.Spacing.default = "0.01r"

        circos.AddTrack(New Histogram(New data(Of ValueTrackData)(bins)))
        circos.AddTrack(New ScatterPlot(New data(Of ValueTrackData)(
            bins.Where(Function(b, i) i Mod 12 = 0).ToArray)))

        Call circos.Save(outDIR)

        Dim result = CircosRender.Render($"{outDIR}/circos.conf", outputFile:="smoke.png", outputDir:=outDIR)

        Console.WriteLine(result.ToString)
        Console.WriteLine(result.StdErr)
    End Sub
End Module
