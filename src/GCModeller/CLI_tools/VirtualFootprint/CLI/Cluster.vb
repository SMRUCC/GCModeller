#Region "Microsoft.VisualBasic::488e6354a534a4826312645c630fdb39, CLI_tools\VirtualFootprint\CLI\Cluster.vb"

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

    ' Module CLI
    ' 
    '     Function: alloacte, BinaryKmeans, BinaryKmeansSW, Cluster, PromoterPalindromeLociHist
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.KMeans
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.ClusterMatrix
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 自己和自己进行比对，然后进行聚类
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Binary.KMeans.SW",
               Usage:="/Binary.KMeans.SW /in <dataset.fasta> [/cut 0.65 /minw 6 /first.ID /parallel.depth <-1> /out <out.DIR>]")>
    <ArgumentAttribute("/first.ID",
              Description:="Using the first token in the fasta header as the output entity ID? Default is using the full title.")>
    Public Function BinaryKmeansSW(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim cut# = args.GetValue("/cut", 0.65)
        Dim minw% = args.GetValue("/minw", 6)
        Dim out As String =
            args.GetValue("/out", [in].TrimSuffix & $"-cut={cut},minw={minw}/")
        Dim fa As New FastaFile([in])
        Dim parallelDepth% = args.GetValue("/parallel.depth", -1)

        If args.GetBoolean("/first.ID") Then
            Call fa.FirstTokenID
        End If

        Dim clusters As EntityClusterModel() = BinaryKmeans(fa, cut, minw, parallelDepth)
        Dim net As NetworkTables = clusters.bTreeNET(removesProperty:=True)

        Call clusters.SaveTo(out & $"/{[in].BaseName}-kmeans.csv")
        Call net.Save(out & "/binary-net/", Encoding.UTF8)

        Return 0
    End Function

    <ExportAPI("/Promoter.Palindrome.loci.hist",
               Usage:="/Promoter.Palindrome.loci.hist /in <palindrome.csv> /len <length-bp> [/step <10> /out <out.csv>]")>
    Public Function PromoterPalindromeLociHist(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".promoter.Palindrome.loci.hist.png")
        Dim len# = args.GetDouble("/len")
        Dim step! = args.GetValue("/step", 10.0!)
        Dim data = [in].LoadCsv(Of PalindromeLoci)
        Dim histPlot As GraphicsData = data _
            .Select(Function(s) s.Start - len) _
            .HistogramPlot([step], serialsTitle:="Frequency(loci.start)", xLabel:="Palindrome loci")

        Return histPlot.Save(out).CLICode
    End Function

    Public Function BinaryKmeans(seq As FastaFile, Optional cutoff# = 0.65, Optional minW% = 6, Optional parallelDepth% = 5) As EntityClusterModel()
        Dim ms? As Boolean = App.IsMicrosoftPlatform ' optimization for linux
        Dim LQuery As EntityClusterModel() = LinqAPI.Exec(Of EntityClusterModel) <=
 _
            From a As KeyValuePair(Of FastaSeq, FastaFile)
            In alloacte(seq, clone:=Not ms).AsParallel  ' 在這裏使用clone而非直接使用原始的對象是爲了提升linux平臺上面的并行計算效率
            Let Name As String = a.Key.Title
            Select New EntityClusterModel With {
                .ID = Name,
                .Properties = a.Key.Cluster(a.Value, cutoff, minW)
            }

        Dim tree As EntityClusterModel() = LQuery.TreeCluster(True, parallelDepth:=parallelDepth)
        Return tree
    End Function

    ''' <summary>
    ''' 并行算法似乎會因爲内存資源的讀取問題而在linux平臺上面出現較高的系統CPU時間
    ''' 在這裏創建新對象來解決這個問題
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <returns></returns>
    Private Iterator Function alloacte(seq As FastaFile, clone? As Boolean) As IEnumerable(Of KeyValuePair(Of FastaSeq, FastaFile))
        Dim prog As New EventProc(seq.Count, "Allocate Memory")

        For Each x As FastaSeq In seq
            If clone Then
                Yield New KeyValuePair(Of FastaSeq, FastaFile)(x.Clone, TryCast(seq.Clone, FastaFile))
            Else
                Yield New KeyValuePair(Of FastaSeq, FastaFile)(
                    x, New FastaFile(seq))
            End If

            Call prog.Tick()
        Next
    End Function

    <Extension>
    Private Function Cluster(query As FastaSeq, source As FastaFile, cutoff#, minW%) As Dictionary(Of String, Double)
        Dim matrix As Blosum = Blosum.FromInnerBlosum62
        Dim LQuery = From b As FastaSeq
                     In source
                     Let sw As SmithWaterman = SmithWaterman.Align(query, b, matrix)
                     Let out As HSP = sw.GetOutput(cutoff, minW).Best
                     Select b.Title,
                         score = If(out Is Nothing, -100.0R, out.score)

        Dim output As Dictionary(Of String, Double) =
            LQuery.ToDictionary(Function(x) x.Title,
                                Function(x) x.score)
        Return output
    End Function
End Module
