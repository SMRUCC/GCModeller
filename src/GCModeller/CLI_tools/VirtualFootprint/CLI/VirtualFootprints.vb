#Region "Microsoft.VisualBasic::b701ae2373e545e9b8a24059bac8f6e3, CLI_tools\VirtualFootprint\CLI\VirtualFootprints.vb"

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
    '     Function: __copy, __createFootprints, BuildFootprints, CastLogAsFootprints, ExportFasta
    '               Intersection, KEGGRegulons, MergeFootprints, PromoterSites, PromoterSitesBatch
    '               RegulateFootprintNetwork, RegulationFootprints, ScanBlastnMapMotifSites, Scanner, SiteScreens
    '               SiteScreens2, TestFootprints, TestFootprints2, TFMotifSites, TrimRegulates
    '     Class __testWorker
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: __buildHash, __isValid
    ' 
    '         Sub: TestBlock
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.AnalysisAPI
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports Parallel.ThreadTask
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.MEME
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Model.Network.VirtualFootprint
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    ''' <summary>
    ''' 假若tag数据里面是调控因子的话，可以用这个函数进行转换
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Logs.Cast.Footprints",
               Usage:="/Logs.Cast.Footprints /in <motifLogs.Csv> [/out <out.csv>]")>
    Public Function CastLogAsFootprints(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".Footprints.Csv")
        Dim logs As IEnumerable(Of MotifLog) = [in].LoadCsv(Of MotifLog)
        Dim footprints As IEnumerable(Of RegulatesFootprints) =
            LinqAPI.Exec(Of RegulatesFootprints) <= From site As MotifLog
                                                    In logs
                                                    Let tfs As String() = Strings.Split(site.tag, "; ")
                                                    Select tfs.Select(Function(tf) __copy(site, site.Regulog, tf, site.Family))
        Return footprints.SaveTo(out)
    End Function

    <ExportAPI("/Export.Footprints.Sites",
               Info:="Exports the motif sites from the virtual footprints sites.",
               Usage:="/Export.Footprints.Sites /in <virtualfootprints> [/TF <locus_tag> /offset <group-offset> /out <outDIR/fasta>]")>
    Public Function ExportFasta(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim TF As String = args("/TF")
        Dim out As String = ("/out" <= args) ^ $"{[in].TrimSuffix}{If(String.IsNullOrEmpty(TF), "", "." & TF)}"
        Dim source As RegulatesFootprints() = [in].LoadCsv(Of RegulatesFootprints)

        If Not String.IsNullOrEmpty(TF) Then
            Dim LQuery = (From x As RegulatesFootprints
                          In source.AsParallel
                          Where String.Equals(TF, x.Regulator)
                          Select x).ToArray
            Dim offset As Integer = args.GetValue("/offset", CInt(LQuery.Average(Function(x) x.Length) * (2 / 3)))
            Dim Grouping = LQuery.Groups(offset)
            Dim Fasta As New FastaFile(
                Grouping.Select(Function(x) New FastaSeq({$"{x.Tag} {x.First.MotifFamily}"}, x.First.Sequence)))
            Return Fasta.Save(out & ".fasta", Encodings.ASCII)
        Else
            Dim EXPORT As String = out
            Dim Groups = (From x As RegulatesFootprints
                          In source.AsParallel
                          Select x
                          Group x By x.Regulator Into Group)
            Dim offset As Integer = args.GetInt32("/offset")

            For Each value In Groups
                source = value.Group.ToArray
                Dim Grouping = source.Groups(If(offset = 0, source.Average(Function(site) site.Length) * (2 / 3), offset))
                Dim Fasta As New FastaFile(
                    Grouping.Select(Function(o) New FastaSeq({$"{o.Tag} {o.First.MotifFamily}"}, o.First.Sequence)) _
                            .OrderBy(Function(o) o.Headers.First))
                Call Fasta.Save(EXPORT & $"/{value.Regulator}.fasta", Encodings.ASCII)
            Next
        End If

        Return 0
    End Function

    <ExportAPI("/Test.Footprints.2",
               Usage:="/Test.Footprints.2 /in <virtualfootprints.csv> [/out <out.csv> /n 2]")>
    Public Function TestFootprints2(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim n As Integer = args.GetValue("/n", 2)
        Dim footprints As IEnumerable(Of RegulatesFootprints) =
            [in].LoadCsv(Of RegulatesFootprints)
        Dim out As String =
            args.GetValue("/out", [in].TrimSuffix & $".TestFootprints2,{n}.csv")
        Dim modGroups = (From x In footprints Select x Group x By x.MotifId Into Group)
        Dim result As New List(Of RegulatesFootprints)

        For Each mgr In modGroups
            Dim Familys = (From x As RegulatesFootprints
                           In mgr.Group
                           Select x
                           Group x By x.MotifFamily Into Group)
            For Each Family In Familys
                Dim data As RegulatesFootprints() = Family.Group.ToArray
                Dim regs As String() = (From x As RegulatesFootprints
                                        In data
                                        Select x.RegulatorTrace
                                        Distinct).ToArray

                If regs.Length >= n Then
                    result += data
                End If
            Next
        Next

        Return result.SaveTo(out)
    End Function

    ''' <summary>
    ''' Footprint文件可能会比较大
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Test.Footprints",
               Usage:="/Test.Footprints /in <virtualfootprints.csv> /opr <regulon-operons.csv> [/out <out.csv>]")>
    Public Function TestFootprints(args As CommandLine) As Integer
        Dim [in] As String = args <= "/in"
        Dim opr As String = args <= "/opr"
        Dim out As String = ("/out" <= args) ^ $"{[in].TrimSuffix}-{opr.BaseName}.Tested.csv"
        Dim regulons As RegPreciseOperon() = opr.LoadCsv(Of RegPreciseOperon)
        Dim regHash = (From o In (From x As RegPreciseOperon
                                  In regulons
                                  Where Not x.Regulators.IsNullOrEmpty
                                  Select (From n As String
                                          In x.Regulators
                                          Select n, operon = x)).IteratesALL
                       Select o
                       Group o By o.n Into Group) _
                            .ToDictionary(Function(x) x.n,
                                          Function(x) x.Group.Select(Function(o) o.operon).ToArray)

        Using writer As New WriteStream(Of RegulatesFootprints)(out)
            Call New DataStream([in]).ForEachBlock(Of RegulatesFootprints)(
                AddressOf New __testWorker(writer, regHash).TestBlock)
        End Using

        Return 0
    End Function

    Private Class __testWorker

        Private __writer As WriteStream(Of RegulatesFootprints)
        Private __regHash As Dictionary(Of String, Dictionary(Of String, String()))

        Sub New(writer As WriteStream(Of RegulatesFootprints), regHash As Dictionary(Of String, RegPreciseOperon()))
            __writer = writer
            __regHash = regHash.ToDictionary(Function(x) x.Key, Function(x) __buildHash(x.Value))
        End Sub

        Private Function __buildHash(source As RegPreciseOperon()) As Dictionary(Of String, String())
            Dim LQuery = (From x As RegPreciseOperon
                          In source
                          Select (From sid As String
                                  In x.Operon
                                  Select sid,
                                      opr = x)).IteratesALL
            Dim hash = (From x In LQuery
                        Select x
                        Order By x.sid Ascending
                        Group x By x.sid Into Group) _
                             .ToDictionary(Function(x) x.sid,
                                           Function(x) x.Group.Select(Function(o) o.opr.source).ToArray)
            Return hash
        End Function

        ''' <summary>
        ''' 从比对的Regulon数据之中检测这个调控关系是否成立
        ''' </summary>
        ''' <returns></returns>
        Private Function __isValid(footprint As RegulatesFootprints) As Double
            Dim TF As String = footprint.Regulator
            Dim ORF As String = footprint.ORF

            If String.IsNullOrEmpty(TF) OrElse String.IsNullOrEmpty(ORF) Then
                Return 0
            End If

            If Not __regHash.ContainsKey(TF) Then
                Return 0
            End If

            Dim ORFs As Dictionary(Of String, String()) = __regHash(TF)

            If Not ORFs.ContainsKey(ORF) Then
                Return 0
            Else
                Dim regulons As String() = ORFs(ORF)
                Dim log As String = footprint.MotifTrace

                If Array.IndexOf(regulons, log) > -1 Then
                    Return 1
                Else
                    Return 0.5
                End If
            End If
        End Function

        Public Sub TestBlock(source As RegulatesFootprints())
            Dim setValue = New SetValue(Of RegulatesFootprints) <= NameOf(RegulatesFootprints.Pcc)
            Dim LQuery As RegulatesFootprints() =
                LinqAPI.Exec(Of RegulatesFootprints) <= From x As RegulatesFootprints
                                                        In source.AsParallel
                                                        Let c As Double = __isValid(x)
                                                        Where c <> 0R
                                                        Select setValue(x, c)
            Call __writer.Flush(LQuery)
        End Sub
    End Class

    ''' <summary>
    ''' 调控位点分析
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Build.Footprints",
               Info:="Build regulations from motif log site.",
               Usage:="/Build.Footprints /motifs <motifLogs.csv> /bbh <queryHits.csv> [/hitshash /sites <motifLogSites.Xml.DIR> /out <out.csv>]")>
    <ArgumentAttribute("/bbh", False,
                   Description:="The bbh hit result between the RegPrecise database and annotated genome proteins. query should be the RegPrecise TF and hits should be the annotated proteins.")>
    <ArgumentAttribute("/sites", True,
                   Description:="If this parameter not presented, then using GCModeller repository data as default.")>
    <ArgumentAttribute("/hitshash", True,
                   Description:="Using hit name as the bbh hash index key? default is using query name.")>
    Public Function BuildFootprints(args As CommandLine) As Integer
        Dim xmls As IEnumerable(Of String) =
            ls - l - r - wildcards("*.xml") <= ("/sites" <= args) ^ GCModeller.FileSystem.RegPrecise.Directories.Motif_PWM
        Dim motifIn As String = args - "/motifs"
        Dim bbh As String = args - "/bbh"
        Dim hits As List(Of BBHIndex) = bbh.LoadCsv(Of BBHIndex)
        Dim sitesHash As Dictionary(Of MotifSitelog) = (From xml As String In xmls Select xml.LoadXml(Of MotifSitelog)).ToDictionary
        Dim out As String = ("/out" <= args) ^ $"{motifIn.TrimSuffix}-{bbh.BaseName}-virtualFootprints.Csv"
        Dim hitsHash As Dictionary(Of String, String()) = BBHIndex.BuildHitsTable(hits, args.GetBoolean("/hitshash"))
        Dim RegPrecise As TranscriptionFactors =
            GCModeller.FileSystem.RegPrecise.RegPreciseRegulations.LoadXml(Of TranscriptionFactors)
        Dim regHash As Dictionary(Of String, String()) = RegPrecise.BuildRegulatesTable

        Using writeFootprints As New WriteStream(Of RegulatesFootprints)(out)
            Call New DataStream(motifIn) _
                .ForEachBlock(Of MotifLog)(
                 Sub(source)  ' Regulog -> motifsitelog -> TF -> bbh-> TF' -> footprints
                     Dim result As RegulatesFootprints() = source.__createFootprints(sitesHash, regHash, hitsHash)
                     Call writeFootprints.Flush(result)
                 End Sub, blockSize:=10240)
        End Using

        Return 0
    End Function

    <Extension>
    Private Function __createFootprints(source As MotifLog(),
                                        sitesHash As Dictionary(Of MotifSitelog),
                                        RegPrecise As Dictionary(Of String, String()),
                                        hitsHash As Dictionary(Of String, String())) As RegulatesFootprints()

        Dim LQuery = (From x As MotifLog In source.AsParallel
                      Let siteLog As MotifSitelog = sitesHash(x.Regulog)
                      Let TFs As IEnumerable(Of String) =
                          (From site As Regtransbase.WebServices.MotifFasta
                           In siteLog.Sites
                           Let uid As String = $"{site.locus_tag}:{site.position}"
                           Where RegPrecise.ContainsKey(uid)
                           Select RegPrecise(uid)).IteratesALL.Distinct
                      Select (From TF As String
                              In TFs
                              Where hitsHash.ContainsKey(TF)   ' bbh 没有比对上去的记录，则跳过这个位点
                              Let maps As String() = hitsHash(TF)
                              Select (From TF_locus As String
                                      In maps   ' 生成footprint位点
                                      Select __copy(x, TF, TF_locus, siteLog.Family)).ToArray).IteratesALL).IteratesALL
        Return LQuery.ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="TF">RegulatorTrace</param>
    ''' <param name="locus">Regulator</param>
    ''' <param name="family"></param>
    ''' <returns></returns>
    Private Function __copy(x As MotifLog, TF As String, locus As String, family As String) As RegulatesFootprints
        Return New RegulatesFootprints With {
            .Distance = x.ATGDist,
            .Ends = x.Ends,
            .Starts = x.Start,
            .ORF = x.ID,
            .Strand = x.Strand,
            .MotifFamily = family,
            .MotifTrace = x.Regulog,
            .Regulator = locus,
            .RegulatorTrace = TF,
            .Sequence = x.SequenceData,
            .tag = x.Taxonomy,
            .Type = x.BiologicalProcess,
            .Signature = x.Location
        }
    End Function

    <ExportAPI("/Trim.Regulates", Usage:="/Trim.Regulates /in <virtualfootprint.csv> [/out <out.csv> /cut 0.65]")>
    Public Function TrimRegulates(args As CommandLine) As Integer
        Dim inRegulates As String = args("/in")
        Dim out As String = args.GetValue("/out", inRegulates.TrimSuffix & ".Trim.Csv")
        Dim cut As Double = args.GetValue("/cut", 0.65)
        Dim source = inRegulates.LoadCsv(Of RegulatesFootprints)
        Dim outResult As RegulatesFootprints() = (From x As RegulatesFootprints
                                                  In source.AsParallel
                                                  Where Not String.IsNullOrEmpty(x.Regulator)
                                                  Select x).ToArray
        outResult = (From x As RegulatesFootprints
                     In outResult.AsParallel
                     Where Math.Abs(x.Pcc) >= cut OrElse
                         Math.Abs(x.sPcc) >= cut
                     Select x).ToArray

        Return outResult.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Merge.Footprints",
               Usage:="/Merge.Footprints /in <inDIR> [/out <out.csv> /trim]")>
    Public Function MergeFootprints(args As CommandLine) As Integer
        Dim DIR As String = args("/in")
        Dim out As String = args.GetValue("/out", DIR & "/Mergs.Footprints.Csv")
        Dim result As IEnumerable(Of RegulatesFootprints) =
            LinqAPI.MakeList(Of RegulatesFootprints) <=
                From file As String
                In ls - l - wildcards("*.csv") <= DIR
                Select file.LoadCsv(Of RegulatesFootprints)

        If args.GetBoolean("/trim") Then
            result = (From x As RegulatesFootprints
                      In result.AsParallel
                      Where Not String.IsNullOrEmpty(x.Regulator)
                      Select x).ToArray
        End If

        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Intersect",
               Usage:="/Intersect /s1 <footprints.csv> /s2 <footprints.csv> [/out <out.csv> /strict]")>
    Public Function Intersection(args As CommandLine) As Integer
        Dim s1 As String = args("/s1")
        Dim s2 As String = args("/s2")
        Dim out As String = args.GetValue("/out", s1.TrimSuffix & "-" & s2.BaseName & ".csv")
        Dim strict As Boolean = args.GetBoolean("/strict")
        Dim result As RegulatesFootprints() =
            IEqualsAPI.Intersection(s1.LoadCsv(Of RegulatesFootprints), s2.LoadCsv(Of RegulatesFootprints), strict)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/KEGG.Regulons",
               Usage:="/KEGG.Regulons /in <footprints.csv> /mods <KEGG.mods.DIR> [/pathway /out <out.csv>]")>
    Public Function KEGGRegulons(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim mods As String = args("/mods")
        Dim isPathway As Boolean = args.GetBoolean("/pathway")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & "-" & mods.BaseName & ".KEGG.Regulons.Csv")
        Dim cats As ModuleClassAPI =
            If(isPathway,
            ModuleClassAPI.FromPathway(mods),
            ModuleClassAPI.FromModules(mods))
        Dim footprints As List(Of RegulatesFootprints) =
            inFile.LoadCsv(Of RegulatesFootprints)

        Return footprints.KEGGRegulons(cats).SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 使用一个特定的motif信息来扫描指定的基因组的序列
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/scan",
               Info:="Sanning genome sequence with a specific motif meme model.",
               Usage:="/scan /motif <meme.txt> /nt <genome.fasta> [/PTT <genome.ptt> /atg-dist <250> /out <out.csv>]")>
    Public Function Scanner(args As CommandLine) As Integer
        Dim meme As String = args("/motif")
        Dim ntFa As String = args("/nt")
        Dim PTT As String = args("/ptt")
        Dim atgDist As Integer = args.GetValue("/atg-dist", 250)
        Dim out As String = args.GetValue("/out", meme.TrimSuffix & $"-{ntFa.BaseName},cut={atgDist}.csv")
        Dim motifs = Text.Load(meme)
        Dim scan As New Scanner(New FastaSeq(ntFa))
        Dim list As New List(Of SimpleSegment)

        For Each motif As LDM.Motif In motifs
            list += scan.Scan(motif.Signature)
        Next

        If PTT.FileExists Then
            Dim PTTBriefs As PTT = TabularFormat.PTT.Load(PTT)
            Dim result As New List(Of MotifLog)

            For Each x As MotifLog In result
                Dim rel = PTTBriefs.GetRelatedGenes(x.MappingLocation,, atgDist)
                If rel.Length = 0 Then
                    x.ID = ""
                    x.Location = "intergenic"
                    result += x
                Else
                    For Each g In rel
                        If g.Relation = SegmentRelationships.DownStream OrElse
                            g.Relation = SegmentRelationships.DownStreamOverlap Then
                            Continue For
                        End If

                        Dim c As New MotifLog(x)
                        c.ID = g.Gene.Synonym
                        c.Location = g.ToString
                        c.ATGDist = ContextModel.GetATGDistance(c.MappingLocation, g.Gene)
                        result += c
                    Next
                End If
            Next

            Return result > out
        Else
            Return list > out
        End If
    End Function

    ''' <summary>
    ''' 将调控因子预测和调控位点预测组装为调控关系网络
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/regulation.footprints")>
    <Usage("/regulation.footprints /regulator <regprecise.regulator.csv> /footprint <motif_context.csv> [/out <regulation.csv>]")>
    <Description("Build the TF regulation predicted network.")>
    <ArgumentAttribute("/regulator", False, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(RegPreciseRegulatorMatch)})>
    <ArgumentAttribute("/footprint", False, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(FootprintSite)})>
    <ArgumentAttribute("/out", True, CLITypes.File, PipelineTypes.undefined, AcceptTypes:={GetType(RegulationFootprint)})>
    Public Function RegulationFootprints(args As CommandLine) As Integer
        Dim regulator$ = args <= "/regulator"
        Dim footprint$ = args <= "/footprint"
        Dim out$ = args("/out") Or $"{regulator.TrimSuffix}_{footprint.BaseName}.regulation-footprints.csv"
        Dim regulators = regulator.LoadCsv(Of RegPreciseRegulatorMatch)
        Dim footprints = footprint.LoadCsv(Of FootprintSite) _
            .Select(Function(gene)
                        Return gene.src.Select(Function(tag) (tag, gene))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(t) t.Item1) _
            .ToDictionary(Function(tag) tag.Key,
                          Function(genes)
                              Return genes _
                                  .Select(Function(site) site.Item2) _
                                  .ToArray
                          End Function)

        Using regulations As New WriteStream(Of RegulationFootprint)(out)
            Dim regulation As RegulationFootprint

            For Each predict As RegPreciseRegulatorMatch In regulators
                For Each site As String In predict.RegulonSites
                    Dim genes As FootprintSite() = footprints.TryGetValue(site)

                    If genes Is Nothing Then
                        Continue For
                    End If

                    For Each gene As FootprintSite In genes
                        regulation = New RegulationFootprint With {
                            .biological_process = predict.biological_process,
                            .distance = gene.distance,
                            .effector = predict.effector,
                            .family = predict.Family,
                            .identities = predict.Identities,
                            .mode = predict.mode,
                            .motif = New NucleotideLocation(gene.left, gene.right, gene.strand),
                            .product = gene.product,
                            .regprecise = predict.Regulator,
                            .regulated = gene.gene,
                            .regulator = predict.Query,
                            .regulog = predict.Regulog,
                            .species = predict.species,
                            .site = site,
                            .sequenceData = gene.sequenceData,
                            .replicon = gene.replicon
                        }
                        regulations.Flush(regulation)
                    Next
                Next
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/regulation.footprints.network")>
    <Usage("/regulation.footprints.network /in <regulations.csv/repository> [/degree.cutoff <default=0> /supports.cutoff <default=1> /out <network.dir>]")>
    Public Function RegulateFootprintNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim degreeCutoff% = args("/degree.cutoff") Or 0
        Dim out$
        Dim regulations As RegulationFootprint()
        Dim duplicatedCutoff% = args("/supports.cutoff") Or 1

        If [in].FileExists Then
            out = args("/out") Or $"{[in].TrimSuffix}.network/"
            regulations = [in].LoadCsv(Of RegulationFootprint)
        Else
            out = args("/out") Or $"{[in].TrimDIR}.network/"
            regulations = (ls - l - r - "*.csv" <= [in]) _
                .Select(Function(path)
                            Return path.LoadCsv(Of RegulationFootprint)
                        End Function) _
                .IteratesALL _
                .ToArray
        End If

        Dim genomeReplicon$ = regulations _
            .FirstOrDefault(Function(reg)
                                Return InStr(reg.replicon, "plasmid", CompareMethod.Text) = 0
                            End Function) _
           ?.replicon
        Dim regulators = regulations _
            .GroupBy(Function(reg) reg.regulator) _
            .ToDictionary(Function(reg) reg.Key,
                          Function(reg) reg.ToArray)
        Dim regulatedGenes = regulations _
            .GroupBy(Function(reg) reg.regulated) _
            .ToDictionary(Function(reg) reg.Key,
                          Function(reg) reg.First)
        Dim nodes = regulations _
            .Select(Function(reg) {reg.regulator, reg.regulated}) _
            .IteratesALL _
            .Distinct _
            .Select(Function(id)
                        Dim type$ = "gene" Or "TF".When(regulators.ContainsKey(id))
                        Dim data As New Dictionary(Of String, String)

                        If regulatedGenes.ContainsKey(id) Then
                            data(NameOf(RegulationFootprint.replicon)) = regulatedGenes(id).replicon
                        Else
                            data(NameOf(RegulationFootprint.replicon)) = genomeReplicon
                        End If

                        If type = "TF" Then
                            data(NameOf(RegulationFootprint.family)) = regulators(id) _
                                .Select(Function(r) r.family) _
                                .Distinct _
                                .JoinBy("|")
                        End If

                        Return New Node With {
                            .ID = id,
                            .NodeType = type,
                            .Properties = data
                        }
                    End Function) _
            .ToArray
        Dim edges = regulations _
            .GroupBy(Function(reg) $"{reg.regulator}__{reg.regulated}") _
            .Where(Function(reg) reg.Count > duplicatedCutoff) _
            .Select(Function(reg)
                        Dim regulation As RegulationFootprint = reg.First

                        Return New NetworkEdge With {
                            .fromNode = regulation.regulator,
                            .toNode = regulation.regulated,
                            .interaction = "transcript_regulation",
                            .value = 0,
                            .Properties = New Dictionary(Of String, String) From {
                                {"supports", reg.Count}
                            }
                        }
                    End Function) _
            .ToArray

        Return New NetworkTables With {
            .edges = edges,
            .nodes = nodes
        }.AnalysisDegrees _
         .RemovesByDegree(degreeCutoff) _
         .Save(out) _
         .CLICode
    End Function

    ''' <summary>
    ''' 位点数太少了
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/MAST_Sites.Screen",
               Usage:="/MAST_Sites.Screen /in <mast_sites.csv> /operons <regprecise.operons.csv> [/out <out.csv>]")>
    Public Function SiteScreens(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim operons As String = args - "/operons"
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $".{operons.BaseName}.siteScreen.Csv")
        Dim sites As IEnumerable(Of MotifLog) = [in].LoadCsv(Of MotifLog)
        Dim RegPreciseOperons As IEnumerable(Of RegPreciseOperon) =
            operons.LoadCsv(Of RegPreciseOperon)
        Dim rgHash = (From x As RegPreciseOperon
                      In RegPreciseOperons
                      Where Not String.IsNullOrEmpty(x.source)
                      Select x
                      Group x By x.source Into Group) _
                           .ToDictionary(Function(x) x.source,
                                         Function(x) x.Group.ToArray)
        Dim result As New List(Of MotifLog)

        For Each x As MotifLog In sites
            If String.IsNullOrEmpty(x.Regulog) Then
                Continue For
            End If

            If Not rgHash.ContainsKey(x.Regulog) Then
                Continue For
            End If

            For Each opr As RegPreciseOperon In rgHash(x.Regulog)
                If Array.IndexOf(opr.Operon, x.ID) > -1 Then
                    result += New MotifLog(x)
                End If
            Next
        Next

        Return result > out
    End Function

    <ExportAPI("/MAST_Sites.Screen2",
               Usage:="/MAST_Sites.Screen2 /in <mast_sites.csv> [/n <2> /offset <30> /out <out.csv>]")>
    Public Function SiteScreens2(args As CommandLine) As Integer
        Dim [in] As String = args - "/in"
        Dim sites As IEnumerable(Of MotifLog) = [in].LoadCsv(Of MotifLog)
        Dim ORFs = (From x As MotifLog
                    In sites
                    Select x
                    Group x By x.ID Into Group)
        Dim n As Integer = args.GetValue("/n", 2)
        Dim offset As Integer = args.GetValue("/offset", 30)
        Dim result As New List(Of MotifLog)
        Dim unpassed As New List(Of MotifLog)

        For Each ORF In ORFs
            Dim Familys = (From x As MotifLog
                           In ORF.Group
                           Where Not String.IsNullOrEmpty(x.BiologicalProcess)
                           Let bp As String() = Strings.Split(x.BiologicalProcess, ";")
                           Select From proc As String
                                  In bp
                                  Where Not proc.StringEmpty
                                  Select bioProc = proc.Trim,
                                      site = x).IteratesALL
            For Each gr In (From x In Familys Select x Group x By x.bioProc Into Group)
                Dim source As MotifLog() = gr.Group.Select(Function(x) x.site)
                Dim Groups = source.Groups(offset)

                For Each dGr In Groups
                    If dGr.Group.Length >= n Then
                        result += dGr.Group
                    Else
                        unpassed += dGr.Group
                    End If
                Next
            Next
        Next

        Dim out As String =
            args.GetValue("/out", [in].TrimSuffix & $",{n},{offset}.SiteScreens2.Csv")

        Call unpassed.SaveTo(out.TrimSuffix & ".unpass.Csv")

        Return result > out
    End Function

    <ExportAPI("/TF.Sites",
               Usage:="/TF.Sites /bbh <bbh.Csv> /RegPrecise <RegPrecise.Xmls.DIR> [/hitHash /out <outDIR>]")>
    Public Function TFMotifSites(args As CommandLine) As Integer
        Dim [in] As String = args - "/bbh"
        Dim RegDIR As String = args - "/RegPrecise"
        Dim hitHash As Boolean = args.GetBoolean("/HitHash")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & $"{NameOf(TFMotifSites)}.fasta/")
        Dim bbhhash As Dictionary(Of String, String()) = BBHIndex.BuildHitsTable([in].LoadCsv(Of BBHIndex), hitHash)
        Dim regulators = (From xml As String
                          In ls - l - wildcards("*.xml") <= RegDIR
                          Let g As BacteriaRegulome = xml.LoadXml(Of BacteriaRegulome)
                          Where Not (g.regulome Is Nothing OrElse
                              g.regulome.regulators.IsNullOrEmpty)
                          Select tfs = g.regulome.regulators).IteratesALL
        Dim reghash = (From x As Regulator
                       In regulators
                       Select x
                       Group x By x.locus_tag.name Into Group) _
                            .ToDictionary(Function(x) x.name,
                                          Function(x) x.Group.ToArray)
        For Each TF As KeyValuePair(Of String, String()) In bbhhash
            Dim path As String = $"{out}/{TF.Key}.fasta"
            Dim fasta As New FastaFile

            For Each hit As String In TF.Value
                If Not reghash.ContainsKey(hit) Then
                    Continue For
                End If

                fasta += From rg As Regulator In reghash(hit) Select rg.ExportMotifs
            Next

            Call fasta.Save(path, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/Filter.Promoter.Sites", Usage:="/Filter.Promoter.Sites /in <motifLog.Csv> [/out <out.csv>]")>
    Public Function PromoterSites(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".PromoterSites.csv")
        Dim reader As New DataStream([in])

        Using writer As New WriteStream(Of MotifLog)(out)
            Call reader.ForEachBlock(Of MotifLog)(
                Sub(buf)
                    Dim LQuery As MotifLog() =
                        LinqAPI.Exec(Of MotifLog) <= From site As MotifLog
                                                     In buf.AsParallel
                                                     Where site.InPromoterRegion
                                                     Select site
                    Call writer.Flush(LQuery)
                End Sub)
        End Using

        Return 0
    End Function

    <ExportAPI("/Filter.PromoterSites.Batch",
               Usage:="/Filter.PromoterSites.Batch /in <motifLogs.DIR> [/num_threads <-1> /out <out.DIR>]")>
    Public Function PromoterSitesBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", inDIR & "-PromoterSites/")
        Dim files As IEnumerable(Of String) = ls - l - r - wildcards("*.csv") <= inDIR
        Dim task As Func(Of String, String) =
            Function(file) _
                $"{GetType(CLI).API(NameOf(PromoterSites))} /in {file.CLIPath} /out {$"{out}/{file.BaseName}.csv".CLIPath}"
        Dim CLI As String() = files.Select(task).ToArray
        Dim n As Integer = args.GetValue("/num_threads", -1)

        If n < 0 Then
            n = LQuerySchedule.CPU_NUMBER
        ElseIf n = 0 Then
            n = 1
        End If

        Return BatchTasks.SelfFolks(CLI, n)
    End Function

    <ExportAPI("/scan.blastn.map.motifsite")>
    <Usage("/scan.blastn.map.motifsite /in <blastn.mapping.csv> [/hits.base <default=2> /out <motifsite.csv>]")>
    <ArgumentAttribute("/hits.base", True, CLITypes.String,
              AcceptTypes:={GetType(String), GetType(Integer)},
              Description:="Quantile expression: ``quantile:[0,1]``")>
    Public Function ScanBlastnMapMotifSites(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim hitsBase$ = args("/hits.base") Or 2
        Dim out$

        If Not hitsBase.IsPattern("\d+") Then
            ' 是quantile表达式
            out = args("/out") Or $"{[in].TrimSuffix}.motif_sites={hitsBase.NormalizePathString}.csv"
        Else
            out = args("/out") Or $"{[in].TrimSuffix}.motif_sites={hitsBase}.csv"
        End If

        Dim tree As NaiveBinaryTree(Of Location, BlastnMapping) = [in].OpenHandle.AsLinq(Of BlastnMapping).BuildTree
        Dim motifSites As NamedValue(Of NucleotideLocation)() = tree _
            .ExtractSites _
            .FilterMotifs(Function(q) q.Split("|"c)(2),
                          Function(q)
                              Return q.Split("|"c) _
                                      .Take(2) _
                                      .JoinBy(":")
                          End Function, hitsExpression:=hitsBase) _
            .ToArray
        Dim output As MotifSiteMatch() = motifSites _
            .Select(Function(site)
                        Dim targets$() = site _
                            .Description _
                            .LoadJSON(Of String())

                        Return New MotifSiteMatch With {
                            .ID = site.Name,
                            .left = site.Value.left,
                            .right = site.Value.right,
                            .strand = site.Value.Strand.GetBriefCode,
                            .src = targets
                        }
                    End Function) _
            .ToArray

        Return output.SaveTo(out).CLICode
    End Function
End Module
