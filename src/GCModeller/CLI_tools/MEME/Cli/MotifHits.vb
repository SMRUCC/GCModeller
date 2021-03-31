#Region "Microsoft.VisualBasic::8e9bd4d81f337cfea937b22fb954cf27, CLI_tools\MEME\Cli\MotifHits.vb"

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
'     Function: __buildRegulates, __siteToFootprint, (+2 Overloads) __siteToRegulation, Expand, HitContext
'               HitsRegulation, MotifInfo, MotifInfoBatch, MotifMatch, MotifMatch2
'               SiteHitsToFootprints, SiteMASTScan, SiteMASTScanBatch, SiteRegexCommon, SiteRegexScan
'               ToFootprints
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel.ThreadTask
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Data.Regprecise.WebServices
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.FootprintTraceAPI
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.Similarity.TOMQuery
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Partial Module CLI

    <ExportAPI("/MotifHits.Regulation",
               Usage:="/MotifHits.Regulation /hits <motifHits.Csv> /source <meme.txt.DIR> /PTT <genome.PTT> /correlates <sp/DIR> /bbh <bbhh.csv> [/out <out.footprints.Csv>]")>
    Public Function HitsRegulation(args As CommandLine) As Integer
        Dim hitFile As String = args("/hits")
        Dim out As String = args.GetValue("/out", hitFile.TrimSuffix & ".VirtualFootprints.Csv")
        Dim PTTFile As String = args("/PTT")
        Dim Correlates As String = args("/correlates")
        Dim bbh As String = args("/bbh")
        Dim hitsData = hitFile.LoadCsv(Of MotifHit)
        Dim PTT As PTT = TabularFormat.PTT.Load(PTTFile)
        Dim setName = New SetValue(Of bbhMappings) <= NameOf(bbhMappings.query_name)
        Dim bbhMaps = bbh.LoadCsv(Of bbhMappings).Select(Function(x) setName(x, x.query_name.Split(":"c).Last))
        Dim LDM As Dictionary(Of AnnotationModel) = AnnotationModel.LoadLDM
        Dim RegDb As Regulations = RegpreciseAPI.LoadRegulationDb
        Dim Regulates = (From x In LDM.AsParallel
                         Let sites As Site() = x.Value.Sites
                         Let regs As String() = (From site As Site
                                                 In sites
                                                 Select RegDb.GetRegulators(site.Name)).IteratesALL.Distinct.ToArray
                         Select x, regs).ToArray
        Dim mapsRegulates = (From x In Regulates
                             Let mapped As bbhMappings() = (From map As bbhMappings In bbhMaps
                                                            Where Array.IndexOf(x.regs, map.query_name) > -1
                                                            Select map).ToArray
                             Where Not mapped.IsNullOrEmpty
                             Select x.x,
                                 mapped).ToDictionary(Function(x) x.x.Value.Uid,
                                                      Function(x) x.mapped.Select(Function(xx) xx.hit_name).Distinct.ToArray)
        Dim sourceLDM = AnnotationModel.LoadMEMEOUT(args("/source"))
        ' Dim correlations As ICorrelations = Correlation2.LoadAuto(Correlates)
        'Dim results = (From hit As MotifHit In hitsData.AsParallel
        '               Where sourceLDM.ContainsKey(hit.Query) AndAlso
        '                   LDM.ContainsKey(hit.Subject)
        '               Let query = sourceLDM(hit.Query), subject = LDM(hit.Subject)
        '               Select __buildRegulates(query, subject, PTT, correlations, mapsRegulates)).IteratesALL.TrimNull
        ' Return results.SaveTo(out).CLICode
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="correlates"></param>
    ''' <param name="mapsRegulates">subjects -> uid</param>
    ''' <returns></returns>
    Private Function __buildRegulates(query As AnnotationModel,
                                      subject As AnnotationModel,
                                      PTT As PTT,
                                      correlates As ICorrelations,
                                      mapsRegulates As Dictionary(Of String, String())) As PredictedRegulationFootprint()

        Dim motif As String = query.Motif
        If Not mapsRegulates.ContainsKey(subject.Uid) Then
            ' 没有被Mapping到的调控因子，则只返回位点数据
            Return query.Sites.Select(Function(x) __siteToFootprint(x, query.Uid, motif, subject, PTT))
        Else
            Return query.Sites.Select(Function(x) __siteToRegulation(x, query.Uid, motif, subject, PTT, correlates, mapsRegulates)).ToVector
        End If
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="site">被调控的基因</param>
    ''' <param name="subject"></param>
    ''' <param name="PTT"></param>
    ''' <param name="correlates"></param>
    ''' <param name="mapsRegulates"></param>
    ''' <returns></returns>
    Private Function __siteToRegulation(site As Site,
                                        query As String,
                                        motif As String,
                                        subject As AnnotationModel,
                                        PTT As PTT,
                                        correlates As ICorrelations,
                                        mapsRegulates As Dictionary(Of String, String())) As PredictedRegulationFootprint()
        Dim footprint As PredictedRegulationFootprint = __siteToFootprint(site, query, motif, subject, PTT)

        If footprint Is Nothing Then
            Return Nothing
        End If

        ' 生成调控信息
        Dim regulators As String() = mapsRegulates(subject.Uid)
        Dim LQuery = (From sId As String In regulators
                      Let copy = Serialization.ShadowsCopy.ShadowsCopy(Of PredictedRegulationFootprint)(footprint)
                      Select __siteToRegulation(copy, sId, correlates)).ToArray
        Return LQuery
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="site">copy</param>
    ''' <param name="TF"></param>
    ''' <param name="correlations"></param>
    ''' <returns></returns>
    Private Function __siteToRegulation(site As PredictedRegulationFootprint, TF As String, correlations As ICorrelations) As PredictedRegulationFootprint
        site.Regulator = TF
        site.Pcc = correlations.GetPcc(TF, site.ORF)
        site.sPcc = correlations.GetSPcc(TF, site.ORF)
        site.WGCNA = correlations.GetWGCNAWeight(TF, site.ORF)
        Return site
    End Function

    ''' <summary>
    ''' 基本的位点信息
    ''' </summary>
    ''' <param name="site"></param>
    ''' <param name="subject"></param>
    ''' <param name="PTT"></param>
    ''' <returns></returns>
    Private Function __siteToFootprint(site As Site, query As String, motif As String, subject As AnnotationModel, PTT As PTT) As PredictedRegulationFootprint
        Dim loci = site.GetLoci(PTT)

        If loci Is Nothing Then
            Return Nothing
        End If

        Dim footprint As New PredictedRegulationFootprint With {
            .Distance = site.GetDist(loci.Strand),
            .Strand = loci.Strand.GetBriefCode,
            .Ends = loci.right,
            .MotifFamily = subject.Uid.Split("."c).First,
            .MotifId = subject.Uid,
            .MotifTrace = query,
            .ORF = site.Name,
            .ORFDirection = loci.Strand.GetBriefCode,
            .Sequence = site.Site,
            .Signature = motif,
            .Starts = loci.left
        }
        Return footprint
    End Function

    ''' <summary>
    ''' 1
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/MAST.MotifMatchs.Family",
               Info:="1",
               Usage:="/MAST.MotifMatchs.Family /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.Xml>]")>
    Public Function MotifMatch(args As CommandLine) As Integer
        Dim MEME_OUT As String = args("/meme")
        Dim MAST_OUT As String = args("/mast")
        Dim out As String = args.GetValue("/out", MEME_OUT & ".MotifMatchs.FROM_MAST.Xml")
        Dim res As Dictionary(Of String, String) = MotifMatchMast.PreCompile(MEME_OUT, MAST_OUT, False)
        Dim result As FootprintTrace = MotifMatchMast.BatchCompile(res)
        Return result.SaveAsXml(out).CLICode
    End Function

    <ExportAPI("/MAST.MotifMatches",
               Info:="",
               Usage:="/MAST.MotifMatches /meme <meme.txt.DIR> /mast <MAST_OUT.DIR> [/out <out.csv>]")>
    Public Function MotifMatch2(args As CommandLine) As Integer
        Dim MEME_OUT As String = args("/meme")
        Dim MAST_OUT As String = args("/mast")
        Dim out As String = args.GetValue("/out", MEME_OUT & ".MotifMatchs.FROM_MAST.Csv")
        Dim res As Dictionary(Of String, String) = MotifMatchMast.PreCompile(MEME_OUT, MAST_OUT, True)
        Dim result As MotifSiteHit() = BatchCompileDirectly(res)
        Return result.SaveTo(out).CLICode
    End Function

    <ExportAPI("/SiteHits.Footprints",
               Info:="Generates the regulation information.",
               Usage:="/SiteHits.Footprints /sites <MotifSiteHits.Csv> /bbh <bbh.Csv> /meme <meme.txt_DIR> /PTT <genome.PTT> /DOOR <DOOR.opr> [/queryHash /out <out.csv>]")>
    Public Function SiteHitsToFootprints(args As CommandLine) As Integer
        Dim [in] As String = args("/sites")
        Dim bbh As String = args("/bbh")
        Dim out As String = args.GetValue("/out", [in].TrimSuffix & ".VirtualFootprints.Csv")
        Dim sites As IEnumerable(Of MotifSiteHit) = [in].LoadCsv(Of MotifSiteHit)
        Dim bbhIndex As IEnumerable(Of BBHIndex) = bbh.LoadCsv(Of BBHIndex)
        Dim hitHash As Boolean = Not args.GetBoolean("/queryHash")
        Dim PTT As String = args - "/PTT"
        Dim DOOR As String = args - "/DOOR"
        Dim meme As String = args - "/meme"
        Return sites.BuildVirtualFootprints(bbhIndex,
                                            hitHash,
                                            TabularFormat.PTT.Load(PTT),
                                            DOOR_API.Load(DOOR),
                                            meme).SaveTo(out).CLICode
    End Function

    ''' <summary>
    ''' 2
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Hits.Context",
               Info:="2",
               Usage:="/Hits.Context /footprints <footprints.Xml> /PTT <genome.PTT> [/out <out.Xml> /RegPrecise <RegPrecise.Regulations.Xml>]")>
    Public Function HitContext(args As CommandLine) As Integer
        Dim footprint As String = args("/footprints")
        Dim genome As String = args("/PTT")
        Dim out As String = args.GetValue("/out", footprint.TrimSuffix & "-" & genome.BaseName & ".Xml")
        Dim RegPrecise As String = args.GetValue("/regprecise", GCModeller.FileSystem.Regulations)
        Dim result = MotifMatchMast.AssignContext(footprint.LoadXml(Of FootprintTrace), PTT.Load(genome), RegPrecise.LoadXml(Of Regulations))
        Return result.SaveAsXml(out).CLICode
    End Function

    'Public Iterator Function ToFootprints(footprints As FootprintTrace,
    '                                      coors As Correlation2,
    '                                      DOOR As DOOR,
    '                                      maps As IEnumerable(Of bbhMappings)) As IEnumerable(Of PredictedRegulationFootprint)
    '    Dim source As IEnumerable(Of PredictedRegulationFootprint) = footprints.ToFootprints(DOOR, maps)

    '    For Each x As PredictedRegulationFootprint In source
    '        x.Pcc = coors.GetPcc(x.ORF, x.Regulator)
    '        x.sPcc = coors.GetSPcc(x.ORF, x.Regulator)
    '        x.WGCNA = coors.GetWGCNAWeight(x.ORF, x.Regulator)

    '        Yield x
    '    Next
    'End Function

    ''' <summary>
    ''' 3
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Footprints",
               Info:="3 - Generates the regulation footprints.",
               Usage:="/Footprints /footprints <footprints.xml> /coor <name/DIR> /DOOR <genome.opr> /maps <bbhMappings.Csv> [/out <out.csv> /cuts <0.65> /extract]")>
    <ArgumentAttribute("/extract", True,
                   Description:="Extract the DOOR operon when the regulated gene is the first gene of the operon.")>
    Public Function ToFootprints(args As CommandLine) As Integer
        Dim footprintXml As String = args("/footprints")
        Dim coor As String = args("/coor")
        Dim DOOR As String = args("/door")
        Dim maps As String = args("/maps")
        Dim cut As Double = Math.Abs(args.GetValue("/cuts", 0.65))
        Dim out As String = args.GetValue("/out", footprintXml.TrimSuffix & "-" & DOOR.BaseName & $"{cut}.Csv")
        Dim oprDOOR As DOOR = DOOR_API.Load(DOOR)
        ' Dim coors As Correlation2 = Correlation2.LoadAuto(coor)
        'Dim source = ToFootprints(footprintXml.LoadXml(Of FootprintTrace),
        '                          coors,
        '                          oprDOOR,
        '                          maps.LoadCsv(Of bbhMappings))
        Dim tag As String = footprintXml.BaseName
        'Dim Cuts = (From x As PredictedRegulationFootprint In source
        '            Where Math.Abs(x.Pcc) >= cut OrElse
        '                Math.Abs(x.sPcc) >= cut
        '            Select x).ToArray

        'For Each x As PredictedRegulationFootprint In Cuts
        '    x.tag = tag
        'Next

        If args.GetBoolean("/extract") Then
            ' Return Cuts.ExpandDOOR(oprDOOR, coors, cut).SaveTo(out).CLICode
        Else
            ' Return Cuts.SaveTo(out).CLICode
        End If
    End Function

    <ExportAPI("/Site.MAST_Scan", Info:="[MAST.Xml] -> [SimpleSegment]",
               Usage:="/Site.MAST_Scan /mast <mast.xml/DIR> [/batch /out <out.csv>]")>
    <ArgumentAttribute("/batch", True,
                   Description:="If this parameter presented in the CLI, then the parameter /mast will be used as a DIR.")>
    Public Function SiteMASTScan(args As CommandLine) As Integer
        Dim batch As Boolean = args.GetBoolean("/batch")

        If Not batch Then
            Dim mast As String = args - "/mast"
            Dim out As String = args.GetValue("/out", mast.TrimSuffix & "-" & NameOf(SiteMASTScan) & ".csv")
            Dim result As New List(Of SimpleSegment)(mast.LoadXml(Of XmlOutput.MAST.MAST).MASTSites)
            Return result > out
        Else
            Dim mastDIR As String = args("/mast")
            Dim out As String = args.GetValue("/out", mastDIR & $"-{NameOf(SiteMASTScan)}.Csv")
            Dim result As New List(Of SimpleSegment)

            For Each mast As String In ls - l - r - wildcards("*.Xml") <= mastDIR
                Try
                    result += mast.LoadXml(Of XmlOutput.MAST.MAST).MASTSites
                    Call mast.__DEBUG_ECHO
                Catch ex As Exception
                    ex = New Exception(mast.ToFileURL, ex)
                    Call App.LogException(ex)
                End Try
            Next

            Return result > out
        End If
    End Function

    <ExportAPI("/Site.MAST_Scan.Batch", Info:="[MAST.Xml] -> [SimpleSegment]",
               Usage:="/Site.MAST_Scan /mast <mast.xml.DIR> [/out <out.csv.DIR> /num_threads <-1>]")>
    Public Function SiteMASTScanBatch(args As CommandLine) As Integer
        Dim [in] As String = args - "/mast"
        Dim out As String = args.GetValue("/out", [in].TrimDIR & ".SiteMASTScan/")
        Dim DIRs As IEnumerable(Of String) = ls - l - lsDIR <= [in]
        Dim task As Func(Of String, String) =
            Function(DIR) _
                $"{GetType(CLI).API(NameOf(SiteMASTScan))} /mast {DIR.CLIPath} /batch /out {(out & "/" & DIR.BaseName & ".Csv").CLIPath}"
        Dim CLI As String() = DIRs.Select(task).ToArray
        Dim num As Integer = args.GetValue("/num_threads", -1)

        num = LQuerySchedule.AutoConfig(num)

        Return BatchTasks.SelfFolks(CLI, parallel:=num)
    End Function

    ''' <summary>
    ''' 使用正则表达式来扫描可能的位点
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Site.RegexScan", Usage:="/Site.RegexScan /meme <meme.txt> /nt <nt.fasta> [/batch /out <out.csv>]")>
    Public Function SiteRegexScan(args As CommandLine) As Integer
        Dim batch As Boolean = args.GetBoolean("/batch")
        Dim nt As String = args("/nt")
        Dim ntFa As New FastaSeq(nt)
        Dim scanner As New Scanner(ntFa)

        If Not batch Then
            Dim meme As String = args("/meme")
            Dim out As String = args.GetValue("/out", meme.TrimSuffix & "-" & nt.BaseName & ".csv")
            Dim result As List(Of SimpleSegment) = SiteRegexCommon(meme, scanner)

            Return result > out
        Else
            Dim memeDIR As String = args("/meme")
            Dim out As String = args.GetValue("/out", memeDIR.ParentPath & "/" & memeDIR.BaseName & "-" & nt.BaseName & ".csv")
            Dim result As New List(Of SimpleSegment)

            For Each meme As String In FileIO.FileSystem.GetFiles(memeDIR, FileIO.SearchOption.SearchAllSubDirectories, "*.txt")
                result += SiteRegexCommon(meme, scanner).ToArray
                Call meme.__DEBUG_ECHO
            Next

            Return result > out
        End If
    End Function

    Public Function SiteRegexCommon(meme As String, scanner As Scanner) As List(Of SimpleSegment)
        Dim result As New List(Of SimpleSegment)
        Dim models As AnnotationModel() =
            AnnotationModel.LoadDocument(meme, AnnotationModel.MEME_UID(meme))
        Dim setId = New SetValue(Of SimpleSegment) <= NameOf(SimpleSegment.ID)

        For Each model As AnnotationModel In models
            result += scanner.Scan(model.Expression) _
                .Select(Function(x) setId(x, model.Uid))
        Next

        Return result
    End Function

    <ExportAPI("/Motif.Info.Batch",
               Info:="[SimpleSegment] -> [MotifLog]",
               Usage:="/Motif.Info.Batch /in <sites.csv.inDIR> /gffs <gff.DIR> [/motifs <regulogs.motiflogs.MEME.DIR> /num_threads -1 /atg-dist 350 /out <out.DIR>]")>
    <ArgumentAttribute("/motifs", False, Description:="Regulogs.Xml source directory")>
    <ArgumentAttribute("/num_threads", True,
                   Description:="Default Is -1, means auto config of the threads number.")>
    Public Function MotifInfoBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim gffs As String = args("/gffs")
        Dim motifs As String = args("/motifs")
        Dim dist As Integer = args.GetValue("/atg-dist", 350)
        Dim out As String = args.GetValue("/out", inDIR & "-" & gffs.BaseName & "/")
        Dim sites As IEnumerable(Of String) = ls - l - wildcards("*.csv") <= inDIR
        Dim gffFiles As IEnumerable(Of String) = ls - l - wildcards("*.gff") <= gffs
        Dim task As Func(Of String, String, String) =
            Function(site, gff) _
                $"{GetType(CLI).API(NameOf(MotifInfo))} /loci {site.CLIPath} /motifs {motifs.CLIPath} /gff {gff.CLIPath} /atg-dist {dist} /out {(out & "-" & site.BaseName & ".genomics_context.csv").CLIPath}"
        Dim CLI As String() = PathMatch.Pairs(sites, gffFiles).Select(Function(x) task(x.Pair1, x.Pair2))
        Dim n As Integer = args.GetInt32("/num_threads")

        If n = 0 Then
            n = 1
        ElseIf n < 0 Then
            n = LQuerySchedule.CPU_NUMBER
        End If

        Return BatchTasks.SelfFolks(CLI, n)
    End Function

    <ExportAPI("/Motif.Info",
               Info:="Assign the phenotype information And genomic context Info for the motif sites. [SimpleSegment] -> [MotifLog]",
               Usage:="/Motif.Info /loci <loci.csv> [/motifs <motifs.DIR> /gff <genome.gff> /atg-dist 250 /out <out.csv>]")>
    <ArgumentAttribute("/loci", False,
                   Description:="The motif site info data set, type Is simple segment.")>
    <ArgumentAttribute("/motifs", False,
                   Description:="A directory which contains the motifsitelog data in the xml file format. Regulogs.Xml source directory")>
    Public Function MotifInfo(args As CommandLine) As Integer
        Dim loci As String = args("/loci")
        Dim motifs As String = args("/motifs")
        Dim result As List(Of MotifLog) =
            If(motifs.DirectoryExists,
            Expand(loci.LoadCsv(Of SimpleSegment), motifs),
            loci.LoadCsv(Of SimpleSegment).ToList(Function(x) New MotifLog(x)))

        Dim out As String =
            args.GetValue("/out", loci.TrimSuffix & If(String.IsNullOrEmpty(motifs), "", "-" & motifs.BaseName) & ".Csv")
        Dim gffFile As String = args("/gff")

        If gffFile.FileExists Then
            Dim dist As Integer = args.GetValue("/atg-dist", 250)
            Dim gff As GFFTable = GFFTable.LoadDocument(gffFile)
            Dim list As New List(Of MotifLog)

            gff = New GFFTable(gff, Features.CDS)

            For Each x As MotifLog In result
                Dim rel = gff.GetRelatedGenes(x.MappingLocation,, dist)
                If rel.Length = 0 Then
                    x.ID = ""
                    x.Location = "intergenic"
                    list += x
                Else
                    Dim temp As New List(Of MotifLog)

                    For Each g In rel
                        If g.Relation = SegmentRelationships.DownStream OrElse
                            g.Relation = SegmentRelationships.DownStreamOverlap Then
                            Continue For
                        End If

                        Dim c As New MotifLog(x)
                        c.ID = g.Gene.synonym
                        c.Location = g.ToString
                        c.ATGDist = ContextModel.GetATGDistance(c.MappingLocation, g.Gene)
                        temp += c
                    Next

                    If temp.Count = 0 Then
                        x.ID = ""
                        x.Location = "intergenic"
                        list += x
                    Else
                        list += DirectCast(temp, IEnumerable(Of MotifLog))
                    End If
                End If
            Next

            result = list
        End If

        Return result > out
    End Function

    ''' <summary>
    ''' 拓展简单位点的信息为Motif位点信息
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="motifs">MotifSiteLog.Xml 文件夹</param>
    ''' <returns></returns>
    Public Function Expand(source As IEnumerable(Of SimpleSegment), motifs As String) As List(Of MotifLog)
        Dim list As New List(Of MotifLog)
        Dim hash As Dictionary(Of String, MotifSitelog) =
            (ls - l - r - wildcards("*.xml") <= motifs) _
            .Select(AddressOf LoadXml(Of MotifSitelog)) _
            .ToDictionary(AddressOf MotifSitelog.Name)

        For Each x As SimpleSegment In source
            Dim tokens As String() = Strings.Split(x.ID, "::")
            Dim k As String = tokens(0)

            If Not hash.ContainsKey(k) Then
                Dim keys As String() = hash.Keys.Take(20).ToArray
                Dim ex As New KeyNotFoundException(keys.GetJson)
                ex = New KeyNotFoundException("Key: " & k, ex)
                Throw ex
            End If

            Dim log As MotifSitelog = hash(k)
            Dim loci As New MotifLog(x)

            loci.BiologicalProcess = log.BiologicalProcess
            loci.Family = log.Family
            loci.Regulog = log.Regulog.Key
            loci.Taxonomy = log.Taxonomy.Key

            list += loci
        Next

        Return list
    End Function
End Module
