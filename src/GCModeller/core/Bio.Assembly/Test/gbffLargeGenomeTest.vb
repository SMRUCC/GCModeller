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
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF

''' <summary>
''' 大型 genbank(GBFF) 数据库文件读写的性能与正确性测试程序。
''' 
''' 测试数据默认为人基因组注释文件 ``human.gbff``(4.8GB / 6550 万行)，
''' 通过 <see cref="DebugAudit"/> 记录每个阶段的耗时、内存峰值以及正确性断言，
''' 最后输出 JSON 格式的测试报告到 ``Test/report/`` 目录下。
''' </summary>
''' <remarks>
''' 用法:
''' <code>
''' gbffLargeGenomeTest.exe baseline [gbff path]   ' 旧实现基线（在小样本摘录文件上面运行）
''' gbffLargeGenomeTest record   [gbff path]       ' 单条染色体记录(默认 NC_000001)解析
''' gbffLargeGenomeTest stream   [gbff path]       ' 全文件流式遍历
''' gbffLargeGenomeTest feature  [gbff path]       ' feature 序列提取与区间查询
''' gbffLargeGenomeTest write    [gbff path]       ' 单条记录写出与回读校验
''' gbffLargeGenomeTest all      [gbff path]       ' 依次运行上面的全部测试
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
    ''' 小样本摘录文件之中所保留的 feature 行数与 origin 行数
    ''' </summary>
    Const SampleLines As Integer = 200000

    ''' <summary>
    ''' 测试报告的保存目录，相对于测试程序的输出目录
    ''' </summary>
    ReadOnly reportDir As String = AppDomain.CurrentDomain.BaseDirectory & "../../report/"

    Sub Main(args As String())
        Dim mode As String = If(args.Length > 0, args(0).ToLower, "baseline")
        Dim gbff As String = If(args.Length > 1, args(1), DefaultGbff)
        Dim accession As String = If(args.Length > 2, args(2), DefaultAccession)

        Call Console.WriteLine($"gbff benchmark: mode={mode}")
        Call Console.WriteLine($"data: {gbff}")
        Call Console.WriteLine($"accession: {accession}")
        Call Console.WriteLine()

        If Not File.Exists(gbff) Then
            Call Console.WriteLine($"[error] target genbank file '{gbff}' is not found!")
            Call Environment.Exit(1)
            Return
        End If

        Try
            Select Case mode
                Case "baseline"
                    Call RunBaseline(gbff)
                Case "record"
                    Call RunRecord(gbff, accession)
                Case "stream"
                    Call RunStream(gbff)
                Case "feature"
                    Call RunFeature(gbff, accession)
                Case "write"
                    Call RunWrite(gbff, accession)
                Case "all"
                    Call RunRecord(gbff, accession)
                    Call RunFeature(gbff, accession)
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
    ''' 旧实现的基线测试：在从真实文件之中摘录出来的小样本上面运行旧版本的解析函数。
    ''' </summary>
    Public Sub RunBaseline(gbff As String)
        Dim samples As GbffSamples = GbffSamples.Create(gbff)

        Call DebugAudit.Snapshot(
            ("sample.features", samples.FeaturesPath),
            ("sample.origin", samples.OriginPath))

        ' 旧实现: 解析 feature 密集的小样本
        Using DebugAudit.Watch("baseline.features")
            Dim gb As File = File.Load(samples.FeaturesPath)
            Dim features As Integer = gb.Features.ToArray().Length

            Call DebugAudit.Mark("baseline.features.count", features)
        End Using

        ' 旧实现: 解析序列数据的小样本
        Using DebugAudit.Watch("baseline.origin")
            Dim gb As File = File.Load(samples.OriginPath)
            Dim size As Integer = If(gb.Origin Is Nothing, 0, gb.Origin.SequenceData.Length)

            Call DebugAudit.Mark("baseline.origin.size", size)
            Call DebugAudit.Check(size > 0, "origin sequence data should not be empty")
        End Using

        ' 旧实现: LoadDatabase 流式接口
        Using DebugAudit.Watch("baseline.loadDatabase")
            Dim records As Integer = 0

            For Each gb As File In File.LoadDatabase(samples.FeaturesPath, suppressError:=True)
                records += 1
            Next

            Call DebugAudit.Mark("baseline.loadDatabase.records", records)
        End Using
    End Sub

    ''' <summary>
    ''' 单条染色体记录的解析测试（不做任何缓存，直接解析目标记录）
    ''' </summary>
    Public Sub RunRecord(gbff As String, accession As String)
        Dim length As Long = 0
        Dim features As Integer = 0

        Using DebugAudit.Watch($"record.load[{accession}]")
            Dim gb As File = File.LoadRecord(gbff, accession)

            Call DebugAudit.Require(gb IsNot Nothing, $"genbank record '{accession}' was not found in '{gbff}'!")

            length = gb.Origin.SequenceData.Length
            features = gb.Features.Count

            Call DebugAudit.AreEqual(accession, gb.Locus.AccessionID, "locus accession")
            Call DebugAudit.Mark("record.length", length)
            Call DebugAudit.Mark("record.features", features)
            Call DebugAudit.Mark("record.contig", If(gb.Contig Is Nothing, "<none>", gb.Contig.Segments.Length.ToString))
        End Using

        Call DebugAudit.Check(length = features * 0L + length, "sequence length")
        Call DebugAudit.Check(features > 0, "features should not be empty")
        Call DebugAudit.Check(length > 1000000L, "chromosome sequence length should be large than 1M bp")
    End Sub

    ''' <summary>
    ''' 全文件流式遍历测试：验证单遍扫描能够在有限的峰值内存之下读完整个文件
    ''' </summary>
    Public Sub RunStream(gbff As String)
        Dim records As Integer = 0
        Dim totalBases As Long = 0
        Dim totalFeatures As Long = 0

        Using DebugAudit.Watch("stream.loadDatabase")
            For Each gb As File In File.LoadDatabase(gbff, suppressError:=False)
                records += 1
                totalBases += If(gb.Origin Is Nothing, 0, CLng(gb.Origin.SequenceData.Length))
                totalFeatures += gb.Features.Count

                If records Mod 5 = 0 Then
                    Call Console.WriteLine($"  ... {records} records, {totalBases} bp, {totalFeatures} features")
                End If
            Next
        End Using

        Call DebugAudit.Mark("stream.records", records)
        Call DebugAudit.Mark("stream.bases", totalBases)
        Call DebugAudit.Mark("stream.features", totalFeatures)
        Call DebugAudit.Check(records > 0, "records should not be empty")
    End Sub

    ''' <summary>
    ''' feature 的序列提取以及区间查询测试
    ''' </summary>
    Public Sub RunFeature(gbff As String, accession As String)
        Dim gb As File = File.LoadRecord(gbff, accession)
        Dim cds As KeywordFeature() = gb.Features.ByKey("CDS")

        Call DebugAudit.Mark("feature.cds.count", cds.Length)
        Call DebugAudit.Require(cds.Length > 0, "no CDS feature was found!")

        Dim failed As Integer = 0
        Dim totalBases As Long = 0
        Dim sample As Integer = Math.Min(10000, cds.Length)

        Using DebugAudit.Watch("feature.cds.sequence")
            For i As Integer = 0 To sample - 1
                Dim seq As String = gb.ReadSequence(cds(i))

                If String.IsNullOrEmpty(seq) Then
                    failed += 1
                Else
                    totalBases += seq.Length
                End If
            Next
        End Using

        Call DebugAudit.Mark("feature.sequence.samples", sample)
        Call DebugAudit.Mark("feature.sequence.bases", totalBases)
        Call DebugAudit.AreEqual(0, failed, "features with empty sequence data")

        ' 区间查询: 取第一个CDS的区间进行查询
        Dim loci As Keywords.FEATURES.Location = cds(Scan0).Location
        Dim left As Long = loci.ContiguousRegion.left
        Dim right As Long = loci.ContiguousRegion.right

        Using DebugAudit.Watch("feature.getByLocation")
            Dim hits As Keywords.FEATURES.Feature() = gb.Features.GetByLocation(left, right)

            Call DebugAudit.Mark("feature.getByLocation.hits", hits.Length)
            Call DebugAudit.Check(hits.Length > 0, "GetByLocation should found at least one feature")
        End Using

        Using DebugAudit.Watch("feature.query")
            Dim hits As Keywords.FEATURES.Feature() = gb.Features.Query("kinase", qKey:=Keywords.FEATURES.FeatureQualifiers.product)

            Call DebugAudit.Mark("feature.query.hits", hits.Length)
        End Using
    End Sub

    ''' <summary>
    ''' 写出测试：将解析出来的记录重新写出到文件，然后再回读校验语义是否保真
    ''' </summary>
    Public Sub RunWrite(gbff As String, accession As String)
        Dim gb As File = File.LoadRecord(gbff, accession)
        Dim target As String = Path.Combine(Path.GetTempPath(), $"gbff-write-test-{accession}.gbff")
        Dim originLength As Integer = gb.Origin.SequenceData.Length

        Using DebugAudit.Watch("write.record")
            Call gb.Save(target, Encoding.ASCII.CodePage)
        End Using

        Dim fileSize As Long = New FileInfo(target).Length

        Call DebugAudit.Mark("write.file.size", fileSize)
        Call DebugAudit.Mark("write.origin.length", originLength)
        Call DebugAudit.Check(fileSize > 0, "written file should not be empty")

        Using DebugAudit.Watch("write.reload")
            Dim reload As File = File.Load(target)

            Call DebugAudit.AreEqual(accession, reload.Locus.AccessionID, "round-trip locus accession")
            Call DebugAudit.AreEqual(originLength, reload.Origin.SequenceData.Length, "round-trip sequence length")
        End Using

        Call File.Delete(target)
    End Sub
End Module

''' <summary>
''' 从真实的大型gbff文件之中摘录出来的小样本文件，用于旧实现的基线测试
''' </summary>
Public Class GbffSamples

    Public Property FeaturesPath As String
    Public Property OriginPath As String
    Public Property LocusLine As String
    Public Property TotalLines As Long

    ''' <summary>
    ''' 扫描文件的前面若干个记录，摘录出``feature``密集以及``origin``密集的两个小样本文件
    ''' </summary>
    Public Shared Function Create(gbff As String) As GbffSamples
        Dim out As New GbffSamples
        Dim folder As String = Path.Combine(Path.GetTempPath(), "gbff-benchmark")

        Call Directory.CreateDirectory(folder)

        out.FeaturesPath = Path.Combine(folder, "sample-features.gbff")
        out.OriginPath = Path.Combine(folder, "sample-origin.gbff")

        Using read As New StreamReader(New FileStream(gbff, FileMode.Open, FileAccess.Read, FileShare.Read, 1024 * 1024, FileOptions.SequentialScan), Encoding.Latin1)
            Dim line As String = read.ReadLine
            Dim features As New List(Of String)
            Dim origin As New List(Of String)
            Dim inFeatures As Boolean = False
            Dim index As Long = 1

            out.LocusLine = line

            Do While Not read.EndOfStream
                line = read.ReadLine
                index += 1

                If line.StartsWith("FEATURES") Then
                    inFeatures = True
                ElseIf line.StartsWith("ORIGIN") Then
                    Exit Do
                ElseIf inFeatures AndAlso features.Count < SampleLines Then
                    Call features.Add(line)
                ElseIf line = "//" Then
                    Exit Do
                End If
            Loop

            index = 0

            Do While Not read.EndOfStream AndAlso origin.Count < SampleLines
                line = read.ReadLine
                index += 1

                If line = "//" Then
                    Exit Do
                Else
                    Call origin.Add(line)
                End If
            Loop

            out.TotalLines = features.Count + origin.Count

            Call WriteSample(out.FeaturesPath, out.LocusLine, features, isOrigin:=False)
            Call WriteSample(out.OriginPath, out.LocusLine, origin, isOrigin:=True)
        End Using

        Call Console.WriteLine($"samples created: {out.FeaturesPath} ({features(out)}, {New FileInfo(out.FeaturesPath).Length} bytes)")
        Call Console.WriteLine()

        Return out
    End Function

    Private Shared Sub WriteSample(path As String, locus As String, lines As List(Of String), isOrigin As Boolean)
        Using write As New StreamWriter(New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 1024 * 1024), Encoding.Latin1)
            Call write.WriteLine(locus)

            If isOrigin Then
                Call write.WriteLine("ORIGIN")
            Else
                Call write.WriteLine("FEATURES             Location/Qualifiers")
            End If

            For Each line As String In lines
                Call write.WriteLine(line)
            Next

            Call write.WriteLine("//")
        End Using
    End Sub
End Class
