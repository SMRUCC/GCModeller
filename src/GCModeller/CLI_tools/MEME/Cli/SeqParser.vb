Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic
Imports MEME.GCModeller.FileSystem.FileSystem
Imports MEME.Analysis
Imports SMRUCC.genomics.DatabaseServices.Regprecise
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Workflows.PromoterParser
Imports SMRUCC.genomics.Assembly.NCBI
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools
Imports SMRUCC.genomics.Assembly.DOOR
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.InteractionModel.Network
Imports SMRUCC.genomics.InteractionModel.Network.VirtualFootprint
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.GenomeMotifFootPrints
Imports LANS.SystemsBiology
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.Toolkits.RNA_Seq.RTools.DESeq2
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite

Partial Module CLI

    ''' <summary>
    ''' 这个函数里面默认是按照TF进行分组输出的，假若需要做操纵子的分析，可以添加/corn标记
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Parser.RegPrecise.Operons",
               Usage:="/Parser.RegPrecise.Operons /operon <operons.Csv> /PTT <PTT_DIR> [/corn /DOOR <genome.opr> /id <null> /locus <union/initx/locus, default:=union> /out <outDIR>]")>
    Public Function ParserRegPreciseOperon(args As CommandLine) As Integer
        Dim [in] As String = args - "/operon"
        Dim PTT_DIR As String = args - "/PTT"
        Dim type As String = args.GetValue("/locus", "union")
        Dim id As String = args - "/id"
        Dim out As String = ("/out" <= args) ^ $"{[in].TrimFileExt}-{type}{If(Not String.IsNullOrEmpty(id), $"-{id}", "")}.fasta/"
        Dim operons As RegPreciseOperon() = [in].LoadCsv(Of RegPreciseOperon)
        Dim PTTDb As New PTTDbLoader(PTT_DIR)
        Dim PTT As PTT = PTTDb.ORF_PTT
        Dim fa As FastaToken = PTTDb.GenomeFasta
        Dim opr As String = args("/door")
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)

        If args.GetBoolean("/corn") Then
            Return __cornParser(operons, Parser, PTT, out)
        Else
            Return __TFoperonsParser(operons, Parser, id, type, opr, PTT, out)
        End If
    End Function

    Private Function __TFoperonsParser(operons As IEnumerable(Of RegPreciseOperon),
                                       Parser As GenePromoterParser,
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
                                    sid = locus)).MatrixAsIterator
        Dim Groups = (From x In regulons
                      Select x  ' 对每一个operon的第一个基因按照TF进行分组
                      Group x By x.TF Into Group) _
                           .ToDictionary(Function(x) x.TF,
                                         Function(x) x.Group.ToArray(Function(g) g.sid).Distinct.OrderBy(Function(sid) sid).ToArray)

        If Not method = GetLocusTags.locus Then
            DOOR = DOOR_API.Load(opr)
        Else
            DOOR = Nothing
        End If

        If Not String.IsNullOrEmpty(id) Then
            Dim locus As String() = Groups(id)
            Call GenePromoterParser.ParsingList(Parser, DOOR, locus, EXPORT:=out, tag:="", method:=method)
        Else
            For Each Group In Groups
                Dim locus As String() = Group.Value
                Call GenePromoterParser.ParsingList(Parser, DOOR, locus, EXPORT:=out, tag:=Group.Key, method:=method)
            Next
        End If

        Return 0
    End Function

    Private Function __cornParser(operons As IEnumerable(Of RegPreciseOperon),
                                  Parser As GenePromoterParser,
                                  PTT As PTT,
                                  EXPORT As String) As Integer
        Dim fasta As New FastaFile

        For Each operon As RegPreciseOperon In operons
            Dim locus As String = operon.Operon.__firstLocus(PTT)  ' 得到当前的这个operon的第一个基因
            Dim uid As String = $"{locus}|{operon.TF_trace}|{operon.source}"
            Dim fa As New FastaToken({uid}, Parser.Promoter_500(locus).SequenceData)
            Call fasta.Add(fa)
        Next

        fasta = New FastaFile(From fa As FastaToken
                              In fasta
                              Select fa
                              Order By fa.Attributes.First Ascending)
        Return fasta.Save((EXPORT.ParentPath & "/" & EXPORT.BaseName).TrimFileExt & ".CRON.fasta")
    End Function

    <Extension>
    Private Sub __save(source As Dictionary(Of String, FastaToken), locus As String(), EXPORT As String)
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

    <ExportAPI("/Parser.Operon",
               Usage:="/Parser.Operon /in <footprint.csv> /PTT <PTTDIR> [/out <outDIR> /family /offset <50> /all]")>
    <ParameterInfo("/family", True,
                   Description:="Group the source by family? Or output the source in one fasta set")>
    Public Function ParserNextIterator(args As CommandLine) As Integer
        Dim inCsv As String = args("/in")
        Dim PTT_DIR As String = args("/PTT")
        Dim out As String = args.GetValue("/out", inCsv.TrimFileExt & ".Operon-regulons.fa/")
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
                                     family.Group.ToArray)).MatrixAsIterator _
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
        Dim parser As New SegmentReader(PTTDb.GenomeFasta)

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
                      Let site As String = parser.TryParse(loci.motifPos).SequenceData
                      Let fasta = New FastaToken(attrs, site)
                      Select uid = fasta.Title,
                          fasta
                      Group By uid Into Group) _
                         .ToArray(Function(x) x.Group.First.fasta)
            Call New FastaFile(fa).Save(path, Encodings.ASCII)
        Next

        Return 0
    End Function

    <ExportAPI("/Parser.MAST",
               Usage:="/Parser.MAST /sites <mastsites.csv> /ptt <genome-context.pttDIR> /door <genome.opr> [/out <outDIR>]")>
    Public Function ParserMAST(args As CommandLine) As Integer
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim sites As String = args("/sites")
        Dim out As String = args.GetValue("/out", sites.TrimFileExt & ".fa/")
        Dim mastSites = sites.LoadCsv(Of MastSites)
        Dim LQuery = (From x As MastSites In mastSites
                      Where Not String.IsNullOrEmpty(x.Gene)
                      Select x
                      Group x By x.Trace Into Group)
        Dim PTTDb As New PTTDbLoader(PTT)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)

        For Each Group In LQuery
            Dim locus As String() = Group.Group.ToArray(Function(x) x.Gene).Distinct.ToArray
            Call GenePromoterParser.ParsingList(Parser, door, locus, out & "/" & Group.Trace.NormalizePathString)
        Next

        Return 0
    End Function

    <ExportAPI("/Parser.DEGs", Usage:="/Parser.DEGs /degs <deseq2.csv> /PTT <genomePTT.DIR> /door <genome.opr> /out <out.DIR> [/log-fold 2]")>
    Public Function ParserDEGs(args As CommandLine) As Integer
        Dim diff As String = args("/degs")
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim out As String = args("/out")
        Dim logFold As Double = args.GetValue("/log-fold", 2)
        Dim DEGs = diff.LoadCsv(Of DESeq2.DESeq2Diff)
        DEGs = (From x In DEGs Where Math.Abs(x.log2FoldChange) >= logFold Select x).ToList
        Dim locus As String() = DEGs.ToArray(Function(x) x.locus_tag)
        Dim PTTDb As New GenBank.TabularFormat.PTTDbLoader(PTT)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterParser.ParsingList(Parser, door, locus, out)
        Return 0
    End Function

    <ExportAPI("/Parser.Log2", Usage:="/Parser.Log2 /in <log2.csv> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/factor 1 /out <outDIR>]")>
    Public Function ParserLog2(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim PTT As String = args("/PTT")
        Dim DOOR As String = args("/DOOR")
        Dim factor As Double = args.GetValue("/factor", 1.0R)
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & $".Log2Seq-{factor}.fa/")
        Dim map As Dictionary(Of String, String) = New Dictionary(Of String, String) From {{GetLocusMapName(inFile), NameOf(ResultData.locus_tag)}}
        Dim data As IEnumerable(Of ResultData) = inFile.LoadCsv(Of ResultData)(maps:=map)
        Dim DEGs As String() = (From x As ResultData
                                In data.AsParallel
                                Where Not String.IsNullOrEmpty((From exp In x.dataExpr0
                                                                Where Math.Abs(exp.Value) >= factor
                                                                Select exp.Key).FirstOrDefault)
                                Select x.locus_tag).ToArray
        Dim PTTDb As New PTTDbLoader(PTT)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterParser.ParsingList(Parser, DOOR, DEGs, out)
        Return 0
    End Function

    <ExportAPI("/Parser.Locus", Usage:="/Parser.Locus /locus <locus.txt> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/out <out.DIR>]")>
    Public Function ParserLocus(args As CommandLine) As Integer
        Dim locus As String = args("/locus")
        Dim PTT As String = args("/ptt")
        Dim door As String = args("/door")
        Dim out As String = args.GetValue("/out", locus.TrimFileExt & ".fa")
        Dim PTTDb As New GenBank.TabularFormat.PTTDbLoader(PTT)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterParser.ParsingList(Parser, door, locus.ReadAllLines, out)
        Return 0
    End Function

    '' 这一部分是生成MEME分析所需要的上游序列的程序

    <ExportAPI("/Parser.Regulon", Usage:="/Parser.Regulon /inDIR <regulons.inDIR> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/door <genome.opr>]")>
    Public Function RegulonParser(args As CommandLine) As Integer
        Dim inDIR As String = args("/inDIR")
        Dim out As String = args("/out")
        Dim PTT As String = args("/PTT")
        Dim PTTDb As New GenBank.TabularFormat.PTTDbLoader(PTT)
        Dim door As String = args("/door")
        Dim opr As Assembly.DOOR.DOOR
        If door.FileExists Then
            opr = DOOR_API.Load(door)
        Else
            opr = Assembly.DOOR.PTT2DOOR(PTTDb.ORF_PTT)
        End If
        Dim Parser As New RegulonParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT, opr)
        Return Parser.RegulonParser(inDIR, out).CLICode
    End Function

    <ExportAPI("/Parser.Regulon.Merged", Usage:="/Parser.Regulon.Merged /in <merged.Csv> /out <fasta.outDIR> /PTT <genomePTT.DIR> [/DOOR <genome.opr>]")>
    Public Function RegulonParser3(args As CommandLine) As Integer
        Dim inCsv As String = args("/in")
        Dim out As String = args.GetValue("/out", inCsv.TrimFileExt & ".MEME.fa/")
        Dim PTT As String = args("/PTT")
        Dim PTTDb As New GenBank.TabularFormat.PTTDbLoader(PTT)
        Dim DOOR As String = args("/door")
        Dim opr As DOOR
        If DOOR.FileExists Then
            opr = DOOR_API.Load(DOOR)
        Else
            opr = DOOR_API.PTT2DOOR(PTTDb.ORF_PTT)
        End If
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        For Each regulon As VirtualFootprint.RegPreciseRegulon In inCsv.LoadCsv(Of VirtualFootprint.RegPreciseRegulon)
            Call GenePromoterParser.ParsingList(Parser, opr, regulon.Members, out & "/" & regulon.uid)
        Next
        Return 0
    End Function

    <ExportAPI("/Parser.Regulon.gb", Usage:="/Parser.Regulon.gb /inDIR <regulons.inDIR> /out <fasta.outDIR> /gb <genbank.gbk> [/door <genome.opr>]")>
    Public Function RegulonParser2(args As CommandLine) As Integer
        Dim inDIR As String = args("/inDIR")
        Dim out As String = args("/out")
        Dim gb As GenBank.GBFF.File = GenBank.GBFF.File.Load(args("/gb"))
        Dim PTT As PTT = GenBank.GbffToORF_PTT(gb)
        Dim DOOR As String = args("/door")
        Dim opr As DOOR
        If DOOR.FileExists Then
            opr = Assembly.DOOR.Load(DOOR)
        Else
            opr = PTT2DOOR(PTT)
        End If
        Dim Parser As New RegulonParser(gb.Origin.ToFasta, PTT, opr)
        Return Parser.RegulonParser(inDIR, out).CLICode
    End Function

    <ExportAPI("/Parser.Pathway",
               Usage:="/Parser.Pathway /KEGG.Pathways <KEGG.pathways.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]")>
    Public Function PathwayParser(args As CommandLine) As Integer
        Dim pathwayDIR As String = args("/KEGG.Pathways")
        Dim PTT_DIR As String = args("/PTT")
        Dim DOOR As String = args("/door")
        Dim locusParser As String = args.GetValue("/locus", "union")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/Pathways.{locusParser}.fa")
        Dim PTTDb As New PTTDbLoader(PTT_DIR)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Dim method As GetLocusTags = Workflows.PromoterParser.ParserLocus.GetType(locusParser)
        Call GenePromoterParser.ParsingKEGGPathways(Parser, DOOR, pathwayDIR, out, method)
        Return 0
    End Function

    <ExportAPI("/Parser.Modules",
               Usage:="/Parser.Modules /KEGG.Modules <KEGG.modules.DIR> /PTT <genomePTT.DIR> /DOOR <genome.opr> [/locus <union/initx/locus, default:=union> /out <fasta.outDIR>]")>
    Public Function ModuleParser(args As CommandLine) As Integer
        Dim pathwayDIR As String = args("/KEGG.Modules")
        Dim PTT_DIR As String = args("/PTT")
        Dim DOOR As String = args("/door")
        Dim locusParser As String = args.GetValue("/locus", "union")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & $"/Modules.{locusParser}.fa/")
        Dim PTTDb As New PTTDbLoader(PTT_DIR)
        Dim method As GetLocusTags = Workflows.PromoterParser.ParserLocus.GetType(locusParser)
        Dim Parser As New GenePromoterParser(PTTDb.GenomeFasta, PTTDb.ORF_PTT)
        Call GenePromoterParser.ParsingKEGGModules(Parser, DOOR, pathwayDIR, out, method)
        Return 0
    End Function
End Module