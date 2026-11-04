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

Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF
Imports IO = System.IO
Imports gbFeature = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Feature
Imports gbLocation = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location
Imports qualifiers = SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.FeatureQualifiers

''' <summary>
''' 大型 genbank(GBFF) 数据库文件读写的性能与正确性测试程序。
''' 
''' 测试数据默认为人基因组注释文件 ``human.gbff``(4.8GB / 6550 万行)，通过 <see cref="DebugAudit"/>
''' 记录每个阶段的耗时、内存峰值以及正确性断言，最后输出 JSON 格式的测试报告到 ``Test/report/`` 目录下。
''' </summary>
''' <remarks>
''' 用法:
''' <code>
''' gbffLargeGenomeTest.exe record  [gbff path] [accession]  ' 单条染色体记录的解析(随机访问)
''' gbffLargeGenomeTest.exe feature [gbff path] [accession]  ' feature 序列提取与区间查询
''' gbffLargeGenomeTest.exe write   [gbff path] [accession]  ' 单条记录写出与回读校验
''' gbffLargeGenomeTest.exe stream  [gbff path]              ' 全文件流式遍历
''' gbffLargeGenomeTest.exe lowmem  [gbff path] [accession]  ' 低内存模式(序列按需回读)
''' gbffLargeGenomeTest.exe index   [gbff path]              ' 记录偏移量索引的构建与随机访问
''' gbffLargeGenomeTest.exe all     [gbff path] [accession]  ' 依次运行上面的全部测试
''' </code>
''' </remarks>
Module gbffLargeGenomeTest

    ''' <summary>
    ''' 默认的人基因组 genbank 注释数据文件
    ''' </summary>
    Public Const DefaultGbff As String = "G:\VirtualViewer\data\human.gbff"

    ''' <summary>
    ''' 默认测试的染色体记录
    ''' </summary>
    Public Const DefaultAccession As String = "NC_000001"

    ''' <summary>
    ''' 测试报告的保存目录，相对于测试程序的输出目录
    ''' </summary>
    ReadOnly reportDir As String = IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory & "../../../report/")

    Sub Main(args As String())
        Dim mode As String = If(args.Length > 0, args(0).ToLower, "record")
        Dim gbff As String = If(args.Length > 1, args(1), DefaultGbff)
        Dim accession As String = If(args.Length > 2, args(2), DefaultAccession)

        Call Console.WriteLine($"gbff benchmark: mode={mode}")
        Call Console.WriteLine($"data: {gbff}")
        Call Console.WriteLine($"accession: {accession}")
        Call Console.WriteLine()

        If Not IO.File.Exists(gbff) Then
            Call Console.WriteLine($"[error] target genbank file '{gbff}' is not found!")
            Call Environment.Exit(1)
            Return
        End If

        Try
            Select Case mode
                Case "record"
                    Call RunRecord(gbff, accession)
                Case "feature"
                    Call RunFeature(gbff, accession)
                Case "write"
                    Call RunWrite(gbff, accession)
                Case "stream"
                    Call RunStream(gbff)
                Case "lowmem"
                    Call RunLowMemory(gbff, accession)
                Case "index"
                    Call RunIndex(gbff, accession)
                Case "all"
                    Call RunIndex(gbff, accession)
                    Call RunRecord(gbff, accession)
                    Call RunFeature(gbff, accession)
                    Call RunLowMemory(gbff, accession)
                    Call RunWrite(gbff, accession)
                    Call RunStream(gbff)
                Case Else
                    Call Console.WriteLine($"[error] unknown test mode '{mode}'")
            End Select
        Catch ex As Exception
            Call DebugAudit.Mark("fatal", $"{ex.GetType.Name}: {ex.Message}")
            Call Console.WriteLine()
            Call Console.WriteLine("[fatal] " & ex.ToString())
        End Try

        Call Flush()

        If Environment.UserInteractive AndAlso Not args.Contains("--nopause") Then
            Call Console.WriteLine()
            Call Console.WriteLine("press any key to exit...")
            Call Console.ReadKey()
        End If
    End Sub

    ''' <summary>
    ''' 将当前的审计信息打印到终端并且保存为JSON报告文件
    ''' </summary>
    Public Sub Flush()
        Call Console.WriteLine()
        Call Console.WriteLine(DebugAudit.Dump())

        Dim report As String = reportDir & $"gbff-{Now.ToString("yyyyMMdd-HHmmss")}.json"

        Call DebugAudit.SaveReport(report)
        Call Console.WriteLine($"report saved: {report}")
    End Sub

    ''' <summary>
    ''' 单条染色体记录的解析测试(通过记录偏移量索引进行随机访问)
    ''' </summary>
    Public Sub RunRecord(gbff As String, accession As String)
        Dim gb As File = Nothing

        Using DebugAudit.Watch($"record.load[{accession}]")
            gb = File.LoadRecord(gbff, accession)
        End Using

        Call DebugAudit.Require(gb IsNot Nothing, $"genbank record '{accession}' was not found in '{gbff}'!")

        Dim length As Integer = gb.Origin.Size
        Dim features As Integer = gb.Features.Count

        Call DebugAudit.AreEqual(accession, gb.Locus.AccessionID, "locus accession")
        Call DebugAudit.Mark("record.locus.length", gb.Locus.Length)
        Call DebugAudit.Mark("record.origin.length", length)
        Call DebugAudit.Mark("record.features", features)
        Call DebugAudit.Mark("record.topology", gb.Topology)
        Call DebugAudit.Mark("record.contig", If(gb.Contig Is Nothing, "<none>", $"{gb.Contig.Segments.Length} segments, {gb.Contig.TotalLength} bp"))
        Call DebugAudit.Mark("record.source", If(gb.Features.source Is Nothing, "<none>", gb.Features.source.KeyName))
        Call DebugAudit.Mark("record.species", gb.Species.Trim())
        Call DebugAudit.Mark("record.peakWorkingSet", $"{Process.GetCurrentProcess().PeakWorkingSet64 / 1048576.0:F1}MB")
        Call DebugAudit.Mark("record.managedMemory", $"{GC.GetTotalMemory(False) / 1048576.0:F1}MB")

        Call DebugAudit.Check(gb.Locus.Length > 0, "locus length should be parsed")
        Call DebugAudit.Check(features > 0, "features should not be empty")
        Call DebugAudit.Check(length > 1000000, "chromosome sequence length should be large than 1M bp")
        Call DebugAudit.AreEqual(gb.Locus.Length, length, "the ORIGIN sequence length should be equals to the LOCUS length")
    End Sub

    ''' <summary>
    ''' feature 的序列提取以及区间查询测试
    ''' </summary>
    Public Sub RunFeature(gbff As String, accession As String)
        Dim gb As File = File.LoadRecord(gbff, accession)
        Dim cds As gbFeature() = gb.Features.ByKey("CDS")

        Call DebugAudit.Mark("feature.cds.count", cds.Length)
        Call DebugAudit.Require(cds.Length > 0, "no CDS feature was found!")

        Dim failed As Integer = 0
        Dim totalBases As Long = 0
        Dim sample As Integer = Math.Min(10000, cds.Length)
        Dim multiExon As Integer = 0
        Dim multiExonOk As Integer = 0

        Using DebugAudit.Watch("feature.cds.sequence")
            For i As Integer = 0 To sample - 1
                Dim feature As gbFeature = cds(i)
                Dim seq As String = gb.ReadSequence(feature)

                If String.IsNullOrEmpty(seq) Then
                    failed += 1
                Else
                    totalBases += seq.Length
                End If

                If feature.Location.SegmentCount > 1 Then
                    Dim expected As Long = 0

                    For Each seg In feature.Location.SequenceSegments
                        expected += seg.RegionLength
                    Next

                    multiExon += 1

                    If expected = seq.Length Then
                        multiExonOk += 1
                    End If
                End If
            Next
        End Using

        Call DebugAudit.Mark("feature.sequence.samples", sample)
        Call DebugAudit.Mark("feature.sequence.bases", totalBases)
        Call DebugAudit.Mark("feature.multiexon.count", multiExon)
        Call DebugAudit.Mark("feature.multiexon.matched", multiExonOk)
        Call DebugAudit.AreEqual(0, failed, "features with empty sequence data")
        Call DebugAudit.AreEqual(multiExon, multiExonOk, "the multi-exon feature sequence length should be the sum of all segments")

        ' 反向互补校验: CDS 的序列应该是其互补链上面正序序列的反向互补
        Dim reversed As gbFeature = cds.FirstOrDefault(Function(f) f.Location.Complement)

        If reversed IsNot Nothing Then
            Dim forward As String = gb.Origin.Slice(reversed.Location.Left, reversed.Location.Right)
            Dim reverse As String = gb.ReadSequence(reversed)

            Call DebugAudit.Check(reverse.Length = forward.Length, "reverse complement sequence length")
            Call DebugAudit.Check(reverse.Length > 0 AndAlso reverse.ReverseString().ToUpper = SMRUCC.genomics.SequenceModel.NucleotideModels.NucleicAcid.Complement(reverse).ToUpper,
                                  "the reverse complement of the feature sequence should be equal to the complement of the reversed sequence")
        End If

        ' 区间查询
        Dim loci As gbLocation = cds(Scan0).Location

        Using DebugAudit.Watch("feature.getByLocation")
            Dim hits As gbFeature() = gb.Features.GetByLocation(loci.Left, loci.Right)

            Call DebugAudit.Mark("feature.getByLocation.hits", hits.Length)
            Call DebugAudit.Check(hits.Length > 0, "GetByLocation should found at least one feature")
        End Using

        Using DebugAudit.Watch("feature.query")
            Dim hits As gbFeature() = gb.Features.Query("kinase", qKey:=qualifiers.product)

            Call DebugAudit.Mark("feature.query.hits", hits.Length)
        End Using

        Using DebugAudit.Watch("feature.query.bykey")
            Dim hits As gbFeature() = gb.Features.Query("kinase", qKey:=qualifiers.product, Key:="CDS")
            Dim genes As gbFeature() = gb.Features.ByKey("gene")

            Call DebugAudit.Mark("feature.query.bykey.hits", hits.Length)
            Call DebugAudit.Mark("feature.gene.count", genes.Length)
        End Using
    End Sub

    ''' <summary>
    ''' 低内存模式测试: 序列数据不会常驻内存，在需要的时候从文件之中按需回读
    ''' </summary>
    Public Sub RunLowMemory(gbff As String, accession As String)
        Dim gb As File = Nothing
        Dim options As GbkReadOptions = GbkReadOptions.LowMemory

        Using DebugAudit.Watch("lowmem.load")
            gb = File.LoadRecord(gbff, accession, options)
        End Using

        Call DebugAudit.Require(gb IsNot Nothing, "failed to load the record in low memory mode!")
        Call DebugAudit.Mark("lowmem.sequence.materialized", gb.Origin.IsMaterialized)
        Call DebugAudit.Mark("lowmem.origin.size", gb.Origin.Size)
        Call DebugAudit.Check(Not gb.Origin.IsMaterialized, "the sequence data should not be loaded into memory")

        Dim cds As gbFeature() = gb.Features.ByKey("CDS")

        Using DebugAudit.Watch("lowmem.cds.sequence")
            Dim total As Long = 0
            Dim failed As Integer = 0

            For i As Integer = 0 To Math.Min(1000, cds.Length) - 1
                Dim seq As String = gb.ReadSequence(cds(i))

                If String.IsNullOrEmpty(seq) Then
                    failed += 1
                Else
                    total += seq.Length
                End If
            Next

            Call DebugAudit.Mark("lowmem.cds.bases", total)
            Call DebugAudit.AreEqual(0, failed, "features with empty sequence data in low memory mode")
        End Using

        ' 与内存模式的序列数据比对
        Dim memory As File = File.LoadRecord(gbff, accession)
        Dim same As Integer = 0
        Dim diff As Integer = 0

        For i As Integer = 0 To Math.Min(1000, cds.Length) - 1
            If gb.ReadSequence(cds(i)) = memory.ReadSequence(memory.Features.ByKey("CDS")(i)) Then
                same += 1
            Else
                diff += 1
            End If
        Next

        Call DebugAudit.Mark("lowmem.compare.same", same)
        Call DebugAudit.AreEqual(0, diff, "the sequence data from the file backed provider should be equal to the in-memory provider")
    End Sub

    ''' <summary>
    ''' 记录偏移量索引的构建以及随机访问测试
    ''' </summary>
    Public Sub RunIndex(gbff As String, accession As String)
        Dim cache As String = gbff & ".gbff.idx"

        If IO.File.Exists(cache) Then
            Call IO.File.Delete(cache)
        End If

        Dim index As GbkRecordIndex = Nothing

        Using DebugAudit.Watch("index.scan")
            index = GbkRecordIndex.Scan(gbff)
        End Using

        Call DebugAudit.Mark("index.records", index.Count)
        Call DebugAudit.Check(index.Count > 0, "the record index should not be empty")
        Call DebugAudit.Check(IO.File.Exists(cache), "the record index cache file should be created")
    End Sub

    ''' <summary>
    ''' 全文件流式遍历测试: 验证单遍扫描能够在有限的峰值内存之下读完整个文件
    ''' </summary>
    Public Sub RunStream(gbff As String)
        Dim records As Integer = 0
        Dim totalBases As Long = 0
        Dim totalFeatures As Long = 0
        Dim mismatch As Integer = 0

        Using DebugAudit.Watch("stream.loadDatabase")
            For Each gb As File In File.LoadDatabase(gbff)
                records += 1
                totalBases += gb.Origin.Size
                totalFeatures += gb.Features.Count

                If gb.Locus IsNot Nothing AndAlso gb.Locus.Length > gb.Origin.Size Then
                    mismatch += 1
                End If

                If records Mod 100 = 0 Then
                    Call Console.WriteLine($"  ... {records} records, {totalBases} bp, {totalFeatures} features")
                End If
            Next
        End Using

        Call DebugAudit.Mark("stream.records", records)
        Call DebugAudit.Mark("stream.bases", totalBases)
        Call DebugAudit.Mark("stream.features", totalFeatures)
        Call DebugAudit.Mark("stream.truncated", mismatch)
        Call DebugAudit.Check(records > 0, "records should not be empty")
        Call DebugAudit.AreEqual(0, mismatch, "the sequence data of every record should not be truncated")
    End Sub

    ''' <summary>
    ''' 写出测试: 将解析出来的记录重新写出到文件，然后再回读校验语义是否保真
    ''' </summary>
    Public Sub RunWrite(gbff As String, accession As String)
        Dim gb As File = File.LoadRecord(gbff, accession)
        Dim target As String = IO.Path.Combine(IO.Path.GetTempPath(), $"gbff-write-test-{accession}.gbff")
        Dim originLength As Integer = gb.Origin.Size
        Dim features As Integer = gb.Features.Count

        Using DebugAudit.Watch("write.record")
            Call gb.Save(target, Encoding.ASCII.CodePage)
        End Using

        Dim fileSize As Long = New IO.FileInfo(target).Length

        Call DebugAudit.Mark("write.file.size", fileSize)
        Call DebugAudit.Mark("write.origin.length", originLength)
        Call DebugAudit.Check(fileSize > 0, "written file should not be empty")

        Using DebugAudit.Watch("write.reload")
            Dim reload As File = File.Load(target)

            Call DebugAudit.AreEqual(accession, reload.Locus.AccessionID, "round-trip locus accession")
            Call DebugAudit.AreEqual(originLength, reload.Origin.Size, "round-trip sequence length")
            Call DebugAudit.AreEqual(features, reload.Features.Count, "round-trip feature count")
            Call DebugAudit.AreEqual(gb.Contig.Segments.Length, reload.Contig.Segments.Length, "round-trip contig segments")

            ' 抽查若干个feature的序列是否一致
            Dim cds As gbFeature() = gb.Features.ByKey("CDS")
            Dim reloadCds As gbFeature() = reload.Features.ByKey("CDS")

            Call DebugAudit.AreEqual(cds.Length, reloadCds.Length, "round-trip CDS count")

            Dim diff As Integer = 0

            For i As Integer = 0 To Math.Min(200, cds.Length) - 1
                If gb.ReadSequence(cds(i)) <> reload.ReadSequence(reloadCds(i)) Then
                    diff += 1
                End If
            Next

            Call DebugAudit.AreEqual(0, diff, "round-trip feature sequence data")
        End Using

        ' 校验写出的文本是否保持严格的列格式
        Dim lines As String() = IO.File.ReadAllLines(target)

        Call DebugAudit.Check(lines(0).StartsWith("LOCUS       "), "LOCUS line format")
        Call DebugAudit.AreEqual(142, Array.IndexOf(lines, "FEATURES             Location/Qualifiers"), "the header lines should be kept in the same order")
        Call DebugAudit.Check(lines.Any(Function(l) l.StartsWith("ORIGIN")), "ORIGIN line should be written")
        Call DebugAudit.Check(lines.Last() = "//", "the record should be end with //")

        Call IO.File.Delete(target)
    End Sub
End Module

Module StringExtensions
    <Runtime.CompilerServices.Extension>
    Public Function ReverseString(s As String) As String
        Dim out As Char() = s.ToCharArray()

        Call Array.Reverse(out)

        Return New String(out)
    End Function
End Module
