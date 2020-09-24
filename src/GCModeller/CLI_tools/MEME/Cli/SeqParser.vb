#Region "Microsoft.VisualBasic::75178854a0c9c78cca24d05efa523b1e, CLI_tools\MEME\Cli\SeqParser.vb"

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
    '     Function: __cornParser, __firstLocus, __TFoperonsParser, ModuleParser, ParserDEGs
    '               ParserLocus, ParserLog2, ParserMAST, ParserNextIterator, ParserRegPreciseOperon
    '               PathwayParser, PathwayParserBatch, RegulonParser, RegulonParser2, RegulonParser3
    ' 
    '     Sub: __save
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools
Imports SMRUCC.genomics.Analysis.RNA_Seq.RTools.DESeq2
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ContextModel.Promoter
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.NCBI
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Workflows.PromoterParser
Imports SMRUCC.genomics.Model.Network
Imports SMRUCC.genomics.Model.Network.VirtualFootprint
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Partial Module CLI

    ''' <summary>
    ''' 这个函数里面默认是按照TF进行分组输出的，假若需要做操纵子的分析，可以添加/corn标记
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Parser.RegPrecise.Operons")>
    <Usage("/Parser.RegPrecise.Operons /operon <operons.Csv> /PTT <PTT_DIR> [/corn /DOOR <genome.opr> /id <null> /locus <union/initx/locus, default:=union> /out <outDIR>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserRegPreciseOperon(args As CommandLine) As Integer
        Dim [in] As String = args - "/operon"
        Dim PTT_DIR As String = args - "/PTT"
        Dim type As String = args.GetValue("/locus", "union")
        Dim id As String = args - "/id"
        Dim out As String = ("/out" <= args) ^ $"{[in].TrimSuffix}-{type}{If(Not String.IsNullOrEmpty(id), $"-{id}", "")}.fasta/"
        Dim operons As RegPreciseOperon() = [in].LoadCsv(Of RegPreciseOperon)
        Dim PTTDb As New PTTDbLoader(PTT_DIR, True)
        Dim PTT As PTT = PTTDb.ORF_PTT
        Dim fa As FastaSeq = PTTDb.GenomeFasta
        Dim opr As String = args("/door")
        Dim Parser As New PromoterRegionParser(fa, PTTDb.ORF_PTT)

        If args.GetBoolean("/corn") Then
            Return __cornParser(operons, Parser, PTT, out)
        Else
            Return __TFoperonsParser(operons, Parser, id, type, opr, PTT, out)
        End If
    End Function

    Private Function __TFoperonsParser(operons As IEnumerable(Of RegPreciseOperon),
                                       Parser As PromoterRegionParser,
                                       id As String,
                                       type As String,
                                       opr As String,
                                       PTT As PTT,
                                       out As String) As Integer
        Dim DOOR As DOOR
        Dim method As GetLocusTags = Workflows.PromoterParser.ParserLocus.GetType(type)
        Dim regulons = (From x As RegPreciseOperon
                        In operons
                        Where Not x.Regulators.IsNullOrEmpty
                        Let locus As String = x.Operon.__firstLocus(PTT)  ' 得到当前的这个operon的第一个基因
                        Select (From TF As String
                                In x.Regulators
                                Select TF,
                                    sid = locus)).IteratesALL
        Dim Groups = (From x In regulons
                      Select x  ' 对每一个operon的第一个基因按照TF进行分组
                      Group x By x.TF Into Group) _
                           .ToDictionary(Function(x) x.TF,
                                         Function(x) x.Group.Select(Function(g) g.sid).Distinct.OrderBy(Function(sid) sid).ToArray)

        If Not method = GetLocusTags.locus Then
            DOOR = DOOR_API.Load(opr)
        Else
            DOOR = Nothing
        End If

        If Not String.IsNullOrEmpty(id) Then
            Dim locus As String() = Groups(id)
            Call GenePromoterRegions.ParsingList(Parser, DOOR, locus, EXPORT:=out, tag:="", method:=method)
        Else
            For Each Group In Groups
                Dim locus As String() = Group.Value
                Call GenePromoterRegions.ParsingList(Parser, DOOR, locus, EXPORT:=out, tag:=Group.Key, method:=method)
            Next
        End If

        Return 0
    End Function

    Private Function __cornParser(operons As IEnumerable(Of RegPreciseOperon),
                                  Parser As PromoterRegionParser,
                                  PTT As PTT,
                                  EXPORT As String) As Integer
        Dim fasta As New FastaFile

        For Each operon As RegPreciseOperon In operons
            Dim locus As String = operon.Operon.__firstLocus(PTT)  ' 得到当前的这个operon的第一个基因
            Dim uid As String = $"{locus}|{operon.TF_trace}|{operon.source}"
            Dim fa As New FastaSeq({uid}, Parser.GetRegionCollectionByLength(500)(locus).SequenceData)
            Call fasta.Add(fa)
        Next

        fasta = New FastaFile(From fa As FastaSeq
                              In fasta
                              Select fa
                              Order By fa.Headers.First Ascending)
        Return fasta.Save((EXPORT.ParentPath & "/" & EXPORT.BaseName).TrimSuffix & ".CRON.fasta")
    End Function

    <Extension>
    Private Sub __save(source As Dictionary(Of String, FastaSeq), locus As String(), EXPORT As String)
        Dim fasta As New FastaFile(From sid As String In locus Where source.ContainsKey(sid) Select source(sid))
        Call fasta.Save(EXPORT, Encodings.ASCII)
    End Sub

    <Extension>
    Private Function __firstLocus(operon As String(), PTT As PTT) As String
        Dim LQuery As ComponentModels.GeneBrief() = (From x As String In operon Select PTT(x)).ToArray

        If LQuery.First.Location.Strand = Strands.Forward Then
            Return (From x In LQuery Select x Order By x.Location.Normalization.Left Ascending).First.Synonym
        Else
            Return (From x In LQuery Select x Order By x.Location.Normalization.Right Descending).First.Synonym
        End If
    End Function

    <ExportAPI("/Parser.Operon")>
    <Usage("/Parser.Operon /in <footprint.csv> /PTT <PTTDIR> [/out <outDIR> /family /offset <50> /all]")>
    <Argument("/family", True,
                   Description:="Group the source by family? Or output the source in one fasta set")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserNextIterator(args As CommandLine) As Integer
        Dim inCsv As String = args("/in")
        Dim PTT_DIR As String = args("/PTT")
        Dim out As String = args.GetValue("/out", inCsv.TrimSuffix & ".Operon-regulons.fa/")
        Dim gbFamily As Boolean = args.GetBoolean("/family")
        Dim offset As Integer = args.GetValue("/offset", 50)
        Dim footprints As IEnumerable(Of PredictedRegulationFootprint) =
            inCsv.LoadCsv(Of PredictedRegulationFootprint)
        Dim PTTDb As New PTTDbLoader(PTT_DIR)
        Dim all As Boolean = args.GetBoolean("/all")

        footprints = (From x As PredictedRegulationFootprint In footprints Where InStr(x.MotifTrace, "@") = 0 Select x).ToArray
        footprints = If(all, footprints, (From x As PredictedRegulationFootprint
                                          In footprints
                                          Where x.InitX <> "0"c   ' 筛选出操纵子里面的第一个基因
                                          Select x).ToArray)
        Dim GroupHash As Dictionary(Of String, PredictedRegulationFootprint())
        If gbFamily Then
            Dim Groups = (From x As PredictedRegulationFootprint In footprints Select x Group x By x.Regulator Into Group)
            Dim FamilyGroup = (From x In Groups
                               Let Family = (From reg As PredictedRegulationFootprint
                                             In x.Group
                                             Select reg
                                             Group reg By reg.MotifFamily Into Group)
                               Select x.Regulator,
                                   Family)
            GroupHash = (From x In FamilyGroup
                         Select (From family In x.Family
                                 Let uid As String = $"{x.Regulator}-{x.Family}"
                                 Select uid,
                                     family.Group.ToArray)).IteratesALL _
                                        .ToDictionary(Function(x) x.uid,
                                                      Function(x) x.ToArray)
        Else
            GroupHash = (From x As PredictedRegulationFootprint
                         In footprints
                         Select x
                         Group x By x.Regulator Into Group) _
                              .ToDictionary(Function(x) x.Regulator,
                                            Function(x) x.Group.ToArray)
        End If

        Dim PTT As PTT = PTTDb.ORF_PTT
        Dim parser As IPolymerSequenceModel = PTTDb.GenomeFasta

        For Each siteGroup In GroupHash
            Dim path As String = out & "/" & siteGroup.Key.NormalizePathString(False) & ".fa"
            Dim locis = (From x In siteGroup.Value
                         Let gene As ComponentModels.GeneBrief = PTT.GeneObject(x.ORF)
                         Let loci = {x.Starts, x.Ends}
                         Let left As Integer = loci.Min - offset
                         Let right As Integer = loci.Max + offset
                         Select x,
                             motifPos = New NucleotideLocation(left, right, gene.Location.Strand))
            Dim fa = (From loci In locis
                      Let attrs As String() = {loci.x.ORF, loci.x.MotifFamily, loci.x.Sequence, loci.motifPos.ToString}
                      Let site As String = parser.CutSequenceLinear(loci.motifPos).SequenceData
                      Let fasta = New FastaSeq(attrs, site)
                      Select uid = fasta.Title,
                          fasta
                      Group By uid Into Group) _
                         .Select(Function(x) x.Group.First.fasta)
            Call New FastaFile(fa).Save(path, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/Parser.MAST")>
    <Usage("/Parser.MAST /sites <mastsites.csv> /ptt <genome-context.pttDIR> /door <genome.opr> [/out <outDIR>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserMAST(args As CommandLine) As Integer
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", sites.TrimSuffix & ".fa/")
        Dim mastSites = sites.LoadCsv(Of MastSites)
        Dim LQuery = (From x As MastSites In mastSites
                      Where Not String.IsNullOrEmpty(x.Gene)
                      Select x
                      Group x By x.Trace Into Group)
        Dim PTTDb As New PTTDbLoader(PTT)
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)

        For Each Group In LQuery
            Dim locus As String() = Group.Group.Select(Function(x) x.Gene).Distinct.ToArray
            Call GenePromoterRegions.ParsingList(Parser, door, locus, out & "/" & Group.Trace.NormalizePathString)
        Next

        Return 0
    End Function

    <ExportAPI("/Parser.DEGs", Usage:="/Parser.DEGs /degs <deseq2.csv> /PTT <genomePTT.DIR> /door <genome.opr> /out <out.DIR> [/log-fold 2]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserDEGs(args As CommandLine) As Integer
        Dim diff As String = args("/degs")
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim out As String = args("/out")
        Dim logFold As Double = args.GetValue("/log-fold", 2)
        Dim DEGs = diff.LoadCsv(Of DESeq2.DESeq2Diff)
        DEGs = (From x In DEGs Where Math.Abs(x.log2FoldChange) >= logFold Select x).AsList
        Dim locus As String() = DEGs.Select(Function(x) x.locus_tag)
        Dim PTTDb As New TabularFormat.PTTDbLoader(PTT)
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterRegions.ParsingList(Parser, door, locus, out)
        Return 0
    End Function

    <ExportAPI("/Parser.Log2", Usage:="/Parser.Log2 /in <log2.csv> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/factor 1 /out <outDIR>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserLog2(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim DOOR As String = args("/DOOR")
        Dim factor As Double = args.GetValue("/factor", 1.0R)
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & $".Log2Seq-{factor}.fa/")
        Dim data As IEnumerable(Of ResultData) = inFile.LoadCsv(Of ResultData)(maps:={{GetLocusMapName(inFile), NameOf(ResultData.locus_tag)}})
        Dim DEGs As String() = (From x As ResultData
                                In data.AsParallel
                                Where Not String.IsNullOrEmpty((From exp In x.dataExpr0
                                                                Where Math.Abs(exp.Value) >= factor
                                                                Select exp.Key).FirstOrDefault)
                                Select x.locus_tag).ToArray
        Dim PTTDb As New PTTDbLoader(PTT)
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterRegions.ParsingList(Parser, DOOR, DEGs, out)
        Return 0
    End Function

    <ExportAPI("/Parser.Locus", Usage:="/Parser.Locus /locus <locus.txt> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/out <out.DIR>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ParserLocus(args As CommandLine) As Integer
        Dim locus As String = args("/locus")
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", locus.TrimSuffix & ".fa")
        Dim PTTDb As New TabularFormat.PTTDbLoader(PTT)
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterRegions.ParsingList(Parser, door, locus.ReadAllLines, out)
        Return 0
    End Function

    '' 这一部分是生成MEME分析所需要的上游序列的程序

    <ExportAPI("/Parser.Regulon", Usage:="/Parser.Regulon /inDIR <regulons.inDIR> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/door <genome.opr>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function RegulonParser(args As CommandLine) As Integer
        Dim inDIR As String = args("/inDIR")
        Dim out As String = args("/out")
        Dim PTT As String = args("/PTT")
        Dim PTTDb As New TabularFormat.PTTDbLoader(PTT)
        Dim door As String = args("/door")
        Dim opr As DOOR
        If door.FileExists Then
            opr = DOOR_API.Load(door)
        Else
            opr = PTT2DOOR(PTTDb.ORF_PTT)
        End If
        Dim Parser As New RegulonParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT, opr)
        Return Parser.RegulonParser(inDIR, out).CLICode
    End Function

    <ExportAPI("/Parser.Regulon.Merged", Usage:="/Parser.Regulon.Merged /in <merged.Csv> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/DOOR <genome.opr>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function RegulonParser3(args As CommandLine) As Integer
        Dim inCsv As String = args("/in")
        Dim out As String = args.GetValue("/out", inCsv.TrimSuffix & ".MEME.fa/")
        Dim PTT As String = args("/PTT")
        Dim PTTDb As New TabularFormat.PTTDbLoader(PTT)
        Dim DOOR As String = args("/door")
        Dim opr As DOOR
        If DOOR.FileExists Then
            opr = DOOR_API.Load(DOOR)
        Else
            opr = DOOR_API.PTT2DOOR(PTTDb.ORF_PTT)
        End If
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        For Each regulon As VirtualFootprint.RegPreciseRegulon In inCsv.LoadCsv(Of VirtualFootprint.RegPreciseRegulon)
            Call GenePromoterRegions.ParsingList(Parser, opr, regulon.Members, out & "/" & regulon.uid)
        Next
        Return 0
    End Function

    <ExportAPI("/Parser.Regulon.gb", Usage:="/Parser.Regulon.gb /inDIR <regulons.inDIR> /out <fasta.outDIR> /gb <genbank.gbk> [/door <genome.opr>]")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function RegulonParser2(args As CommandLine) As Integer
        Dim inDIR As String = args("/inDIR")
        Dim out As String = args("/out")
        Dim gb As GBFF.File = GBFF.File.Load(args("/gb"))
        Dim PTT As PTT = gb.GbffToPTT(ORF:=True)
        Dim DOOR As String = args("/door")
        Dim opr As DOOR
        If DOOR.FileExists Then
            opr = DOOR_API.Load(DOOR)
        Else
            opr = PTT2DOOR(PTT)
        End If
        Dim Parser As New RegulonParser(gb.Origin.ToFasta, PTT, opr)
        Return Parser.RegulonParser(inDIR, out).CLICode
    End Function

    <ExportAPI("/Parser.Pathway")>
    <Usage("/Parser.Pathway /KEGG.Pathways <KEGG.pathways.DIR/organismModel.Xml> /src <genomePTT.DIR/gbff.txt> [/DOOR <genome.opr> /locus <union/initx/locus, default:=union> /out <fasta.outDIR>]")>
    <Description("Parsing promoter sequence region for genes in pathways.")>
    <Argument("/kegg.pathways", False, CLITypes.File, Description:="DBget fetch result from ``kegg_tools``.")>
    <Argument("/src", False, CLITypes.File, Description:="The genome proteins gene coordination data file. It can be download from NCBI web site.")>
    <Argument("/locus", True, CLITypes.String, Description:="Only works when ``/DOOR`` file was presented.")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function PathwayParser(args As CommandLine) As Integer
        Dim pathwayDIR As String = args("/KEGG.Pathways")
        Dim src$ = args("/src")
        Dim DOOR As String = args("/door")
        Dim locusParser As String = args("/locus") Or "union"
        Dim out As String = args("/out") Or (App.CurrentDirectory & $"/Pathways.{locusParser}.fa")
        Dim parser As PromoterRegionParser
        Dim method As GetLocusTags

        If src.FileExists Then
            Dim gb As GBFF.File

            If Not DOOR.FileExists Then
                If locusParser.TextEquals("union") Then
                    gb = GBFF.File.Load(src)
                Else
                    gb = GBFF.File _
                        .LoadDatabase(src) _
                        .Where(Function(g)
                                   Return g.Locus.AccessionID.TextEquals(locusParser)
                               End Function) _
                        .First
                End If

                method = GetLocusTags.locus
            Else
                gb = GBFF.File.Load(src)
                method = Workflows.PromoterParser.ParserLocus.GetType(locusParser)
            End If

            parser = New PromoterRegionParser(gb)
        Else
            Dim PTTDb As New PTTDbLoader(src)
            parser = New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
            method = Workflows.PromoterParser.ParserLocus.GetType(locusParser)
        End If

        Call GenePromoterRegions.ParsingKEGGPathways(parser, DOOR, pathwayDIR, out, method)

        Return 0
    End Function

    <ExportAPI("/Parser.Pathway.Batch")>
    <Usage("/Parser.Pathway.Batch /in <pathway.directory> /assembly <NCBI_assembly.directory> [/out <out.directory>]")>
    Public Function PathwayParserBatch(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim assembly$ = args <= "/assembly"
        Dim out$ = args("/out") Or $"{[in].ParentPath}/meme/promoters/"
        Dim err As New List(Of String)

        For Each Xml As String In ls - l - "*.xml" <= [in]
            Dim genome As OrganismModel = Xml.LoadXml(Of OrganismModel)
            Dim name$ = genome.GetGenbankSource

            If name.StringEmpty Then
                Call Xml.PrintException
                Call err.Add(Xml)
                Continue For
            End If

            Dim search$ = RepositoryExtensions.GetAssemblyPath(assembly, name)
            Dim gb As GBFF.File = RepositoryExtensions.GetGenomeData(gb:=search)
            Dim locus$ = gb.Locus.AccessionID
            Dim EXPORT$ = $"{out}/{genome.organism.FullName.NormalizePathString}/"

            Call Apps.MEME.PathwayParser(Xml, search, locus:=locus, out:=EXPORT)
        Next

        If err > 0 Then
            Call err.ToArray.GetJson(indent:=True).PrintException
        End If

        Return 0
    End Function

    <ExportAPI("/Parser.Modules")>
    <Usage("/Parser.Modules /KEGG.Modules <KEGG.modules.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]")>
    <Description("Parsing promoter sequence region for genes in kegg reaction modules")>
    <Group(CLIGrouping.MEMESeqParser)>
    Public Function ModuleParser(args As CommandLine) As Integer
        Dim pathwayDIR As String = args("/KEGG.Modules")
        Dim PTT_DIR As String = args("/PTT")
        Dim DOOR As String = args("/door")
        Dim locusParser As String = args.GetValue("/locus", "union")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/Modules.{locusParser}.fa/")
        Dim PTTDb As New PTTDbLoader(PTT_DIR)
        Dim method As GetLocusTags = Workflows.PromoterParser.ParserLocus.GetType(locusParser)
        Dim Parser As New PromoterRegionParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterRegions.ParsingKEGGModules(Parser, DOOR, pathwayDIR, out, method)
        Return 0
    End Function
End Module
