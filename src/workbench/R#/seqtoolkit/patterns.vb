#Region "Microsoft.VisualBasic::2b28612c6ea57b9ce50a2b6ee7158d63, R#\seqtoolkit\patterns.vb"

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


    ' Code Statistics:

    '   Total Lines: 533
    '    Code Lines: 367 (68.86%)
    ' Comment Lines: 101 (18.95%)
    '    - Xml Docs: 86.14%
    ' 
    '   Blank Lines: 65 (12.20%)
    '     File Size: 22.20 KB


    ' Module patterns
    ' 
    '     Function: createSeeds, DrawLogo, FindMirrorPalindromes, GetMotifs, GetSeeds
    '               gibbs_scans, gibbs_table, matchSites, matchTableOutput, MotifString
    '               openSeedFile, PalindromeToString, plotMotif, pullAllSeeds, readMotifs
    '               readSites, ScaffoldOrthogonality, seqGraph, seqgraph_df, viewSites
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO.Linq
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.DNAOrigami
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.Annotation.Assembly.NCBI.GenBank.TabularFormat.GFF
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.DocumentFormat.XmlOutput.MEME
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports dataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' Tools for sequence patterns
''' </summary>
<Package("bioseq.patterns", Category:=APICategories.ResearchTools)>
<RTypeExport("motif_match", GetType(MotifMatch))>
Module patterns

    Friend Sub Main()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of PalindromeLoci)(AddressOf PalindromeToString)
        Call REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(MotifMatch()), AddressOf matchTableOutput)
        Call REnv.Internal.generic.add("plot", GetType(SequenceMotif), AddressOf plotMotif)
        Call REnv.Internal.generic.add("plot", GetType(MSAOutput), AddressOf plotMotif)
        Call REnv.Internal.generic.add("plot", GetType(MSAMotif), AddressOf plotMotif)
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of SequenceMotif)(Function(m) DirectCast(m, SequenceMotif).patternString)
        Call REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(MSAMotif), AddressOf gibbs_table)
        Call REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(SequenceGraph()), AddressOf seqgraph_df)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function seqgraph_df(graphs As SequenceGraph(), args As list, env As Environment) As dataframe
        Dim type As String = args.getValue({"seq_type", "type", "mol_type"}, env, [default]:="DNA")
        Dim norm As Boolean = args.getValue({"norm"}, env, [default]:=False)
        Dim charset As Char() = SequenceModel.GetVector(SequenceModel.ParseSeqType(type)).ToArray
        Dim matrix = graphs.Select(Function(si) si.GetVector(charset, norm)).ToArray
        Dim df As New dataframe With {
            .rownames = graphs.Keys.ToArray,
            .columns = New Dictionary(Of String, Array)
        }
        Dim size As Integer = matrix(0).Length

        For i As Integer = 0 To size - 1
#Disable Warning
            Call df.add($"v{i + 1}", matrix.Select(Function(a) a(i)))
#Enable Warning
        Next

        Return df
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function gibbs_table(score As MSAMotif, args As list, env As Environment) As dataframe
        Dim df As New dataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = score.names
        }

        df.add("motif", score.MSA)
        df.add("p", score.p)
        df.add("q", score.q)
        df.add("score", score.score)
        df.add("site", score.start)

        Return df
    End Function

    <RGenericOverloads("plot")>
    Private Function plotMotif(motif As Object, args As list, env As Environment) As Object
        Dim title As String = args.getValue("title", env, [default]:="")
        Return DrawLogo(motif, title, env)
    End Function

    <RGenericOverloads("as.data.frame")>
    Private Function matchTableOutput(scans As MotifMatch(), args As list, env As Environment) As dataframe
        Dim table As New dataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        table.columns(NameOf(MotifMatch.title)) = scans.Select(Function(m) m.title).ToArray
        table.columns(NameOf(MotifMatch.segment)) = scans.Select(Function(m) m.segment).ToArray
        table.columns(NameOf(MotifMatch.identities)) = scans.Select(Function(m) m.identities).ToArray
        table.columns(NameOf(MotifMatch.score1)) = scans.Select(Function(m) m.score1).ToArray
        table.columns(NameOf(MotifMatch.score2)) = scans.Select(Function(m) m.score2).ToArray
        table.columns(NameOf(MotifMatch.motif)) = scans.Select(Function(m) m.motif).ToArray
        table.columns(NameOf(MotifMatch.start)) = scans.Select(Function(m) m.start).ToArray
        table.columns(NameOf(MotifMatch.ends)) = scans.Select(Function(m) m.ends).ToArray
        table.columns(NameOf(MotifMatch.seeds)) = scans.Select(Function(m) m.seeds.JoinBy("; ")).ToArray

        Return table
    End Function

    Private Function PalindromeToString(obj As Object) As String
        If obj Is Nothing Then
            Return "n/a"
        ElseIf obj.GetType Is GetType(PalindromeLoci) Then
            With DirectCast(obj, PalindromeLoci)
                Return $"""{ .Start} { .Loci}|{ .MirrorSite} { .PalEnd}"""
            End With
        Else
            Throw New NotImplementedException(obj.GetType.FullName)
        End If
    End Function

    <ExportAPI("open.seedFile")>
    Public Function openSeedFile(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim filesave = SMRUCC.Rsharp.GetFileStream(file, FileAccess.ReadWrite, env)

        If filesave Like GetType(Message) Then
            Return filesave.TryCast(Of Message)
        End If

        Return New ScanFile(filesave.TryCast(Of Stream))
    End Function

    <ExportAPI("pull.all_seeds")>
    Public Function pullAllSeeds(seed As ScanFile) As HSP()
        Return seed.LoadAllSeeds.ToArray
    End Function

    ''' <summary>
    ''' read the xml motif data model output from the meme program
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.meme_xml")>
    Public Function read_memexml(file As String) As MEMEXml
        Return MEMEXml.LoadDocument(file)
    End Function

    ''' <summary>
    ''' convert the meme document to motif PWM model object
    ''' </summary>
    ''' <param name="meme"></param>
    ''' <returns></returns>
    <ExportAPI("toPWM")>
    <RApiReturn(GetType(Probability))>
    Public Function toPWM(meme As MEMEXml) As Object
        Return meme.GetMotifs.ToArray
    End Function

    ''' <summary>
    ''' make a motif scan from the given sequence collection
    ''' </summary>
    ''' <param name="seqs"></param>
    ''' <param name="width"></param>
    ''' <param name="maxitr"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("gibbs_scan")>
    <RApiReturn(GetType(MSAMotif))>
    Public Function gibbs_scans(<RRawVectorArgument>
                                seqs As Object,
                                Optional width As Integer? = Nothing,
                                Optional maxitr As Integer = 1000,
                                Optional env As Environment = Nothing) As Object

        Dim fa As FastaSeq() = GetFastaSeq(seqs, env).ToArray

        If fa.IsNullOrEmpty Then
            Call "could not extract sequence source fasta data!".warning
            Return Nothing
        End If

        Dim gibbs As New GibbsSampler(fa, If(width, CInt(fa.Average(Function(s) s.Length) * 0.6)))
        Dim motif As MSAMotif = gibbs.find(maxIterations:=maxitr)

        Return motif
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="seq"></param>
    ''' <param name="deli"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("view.sites")>
    <RApiReturn(GetType(String))>
    Public Function viewSites(<RRawVectorArgument>
                              sites As Object,
                              seq As Object,
                              Optional deli$ = ", ",
                              Optional env As Environment = Nothing) As Object

        Dim siteData As pipeline = pipeline.TryCreatePipeline(Of Site)(sites, env)

        If siteData.isError Then
            Return siteData.getError
        End If

        Dim fa As FastaSeq

        If TypeOf seq Is String Then
            fa = New FastaSeq With {
                .Headers = {"seq"},
                .SequenceData = DirectCast(seq, String)
            }
        Else
            fa = GetFastaSeq(seq, env).FirstOrDefault
        End If

        With New StringBuilder
            Call siteData _
                .populates(Of Site)(env) _
                .DisplayOn(fa.SequenceData, New StringWriter(.ByRef), deli)

            Return .ToString
        End With
    End Function

    ''' <summary>
    ''' read sequence motif json file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' apply for search by <see cref="matchSites"/>
    ''' </remarks>
    <ExportAPI("read.motifs")>
    Public Function readMotifs(file As String) As SequenceMotif()
        Return file.LoadJSON(Of SequenceMotif())
    End Function

    ''' <summary>
    ''' read the motif match scan result table file
    ''' </summary>
    ''' <param name="file">should be a file path to a csv table file.</param>
    ''' <returns></returns>
    <ExportAPI("read.scans")>
    Public Function readSites(file As String) As MotifMatch()
        Return file.LoadCsv(Of MotifMatch)(mute:=True).ToArray
    End Function

    <ExportAPI("top_sites")>
    Public Function top_sites(sites As MotifMatch(),
                              Optional identities As Double? = Nothing,
                              Optional pvalue As Double? = Nothing,
                              Optional minW As Integer? = Nothing) As MotifMatch()

        If identities IsNot Nothing Then
            Dim identitiesVal As Double = CDbl(identities)

            sites = (From site As MotifMatch
                     In sites
                     Where site.identities > identitiesVal).ToArray
        End If
        If pvalue IsNot Nothing Then
            Dim pvalue_cut As Double = CDbl(pvalue)

            sites = (From site As MotifMatch
                     In sites
                     Where site.pvalue < pvalue_cut).ToArray
        End If
        If minW IsNot Nothing Then
            Dim width As Integer = CInt(minW)

            sites = (From site As MotifMatch
                     In sites
                     Where (site.ends - site.start) >= minW).ToArray
        End If

        Return sites
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <param name="mol_type"></param>
    ''' <param name="parallel"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' the sequence graph embedding vector data is generates from different method 
    ''' based on the <paramref name="mol_type"/> data:
    ''' 
    ''' + <see cref="SeqTypes.DNA"/>: <see cref="Builder.DNAGraph"/>
    ''' + <see cref="SeqTypes.Protein"/>: <see cref="Builder.PolypeptideGraph"/>
    ''' + <see cref="SeqTypes.RNA"/>: <see cref="Builder.RNAGraph"/>
    ''' </returns>
    <ExportAPI("as.seq_graph")>
    <RApiReturn(GetType(SequenceGraph))>
    Public Function seqGraph(<RRawVectorArgument>
                             fasta As Object,
                             Optional mol_type As SeqTypes = SeqTypes.DNA,
                             Optional parallel As Boolean = False,
                             Optional env As Environment = Nothing) As Object

        Dim seqList = GetFastaSeq(fasta, env).ToArray

        Select Case mol_type
            Case SeqTypes.DNA : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf Builder.DNAGraph, parallel:=parallel)
            Case SeqTypes.Protein : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf Builder.PolypeptideGraph, parallel:=parallel)
            Case SeqTypes.RNA : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf Builder.RNAGraph, parallel:=parallel)
            Case Else
                Return RInternal.debug.stop("general is not allowed!", env)
        End Select
    End Function

    ''' <summary>
    ''' Find target loci site based on the given motif model
    ''' </summary>
    ''' <param name="motif"></param>
    ''' <param name="target">a collection of fasta sequence</param>
    ''' <param name="cutoff#"></param>
    ''' <param name="minW#"></param>
    ''' <returns></returns>
    <ExportAPI("motif.find_sites")>
    <RApiReturn(GetType(MotifMatch))>
    Public Function matchSites(motif As SequenceMotif,
                               <RRawVectorArgument>
                               target As Object,
                               Optional cutoff# = 0.6,
                               Optional minW# = 8,
                               Optional identities As Double = 0.85,
                               Optional pvalue As Double = 0.05,
                               Optional parallel As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        If target Is Nothing Then
            Return RInternal.debug.stop("sequence target can not be nothing!", env)
        ElseIf TypeOf target Is FastaSeq Then
            ' scan a simple single sequence
            Return motif.region _
                .ScanSites(DirectCast(target, FastaSeq), cutoff, minW,
                           pvalue_cut:=pvalue,
                           identities:=identities) _
                .ToArray
        Else
            Dim seqs = GetFastaSeq(target, env)

            ' scan multiple sequence
            If seqs Is Nothing Then
                Return RInternal.debug.stop($"invalid sequence collection type: {target.GetType.FullName}", env)
            Else
                Return seqs.ToArray _
                    .Populate(parallel, App.CPUCoreNumbers) _
                    .Select(Function(seq)
                                Return motif.ScanSites(seq, cutoff, minW, identities, pvalue:=pvalue)
                            End Function) _
                    .IteratesALL _
                    .ToArray
            End If
        End If
    End Function

    ''' <summary>
    ''' Search mirror palindrome sites for a given seed sequence
    ''' </summary>
    ''' <param name="sequence"></param>
    ''' <param name="seed"></param>
    ''' <returns></returns>
    <ExportAPI("palindrome.mirror")>
    Public Function FindMirrorPalindromes(sequence$, seed$) As PalindromeLoci()
        Return Palindrome.FindMirrorPalindromes(seed, sequence)
    End Function

    ''' <summary>
    ''' Create seeds
    ''' </summary>
    ''' <param name="size"></param>
    ''' <param name="base"></param>
    ''' <returns></returns>
    <ExportAPI("seeds")>
    Public Function GetSeeds(size As Integer, base As String) As String()
        Return Seeds.InitializeSeeds(base.ToArray, size)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="motif"></param>
    ''' <param name="env"></param>
    ''' <returns>the regexp liked format string for do motif matches</returns>
    <ExportAPI("motifString")>
    <RApiReturn(GetType(String))>
    Public Function MotifString(<RRawVectorArgument> motif As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of SequenceMotif, String)(motif, Function(m) m.patternString())
    End Function

    <ExportAPI("create.seeds")>
    Public Function createSeeds(<RRawVectorArgument> fasta As Object, saveto As ScanFile,
                                Optional minw% = 8,
                                Optional maxw% = 20,
                                Optional seedingCutoff As Double = 0.95,
                                Optional scanMinW As Integer = 6,
                                Optional scanCutoff As Double = 0.8,
                                Optional significant_sites As Integer = 4,
                                Optional debug As Boolean = False,
                                Optional env As Environment = Nothing) As Object

        Dim param As New PopulatorParameter With {
           .maxW = maxw,
           .minW = minw,
           .seedingCutoff = seedingCutoff,
           .ScanMinW = scanMinW,
           .ScanCutoff = scanCutoff,
           .log = env.WriteLineHandler,
           .seedScanner = Scanners.GraphScan,
           .significant_sites = significant_sites,
           .seedOccurances = 6
        }
        Dim scan As SeedScanner = Activator.CreateInstance(param.GetScanner, param, debug)

        For Each seed As HSP In scan.GetSeeds(GetFastaSeq(fasta, env))
            Call saveto.AddSeed($"{seed.Query}+{seed.Subject}".MD5, seed)
        Next

        Return saveto
    End Function

    ''' <summary>
    ''' find possible motifs of the given sequence collection
    ''' </summary>
    ''' <param name="fasta">should contains multiple sequence</param>
    ''' <param name="minw%"></param>
    ''' <param name="maxw%"></param>
    ''' <param name="nmotifs">
    ''' A number for limit the number of motif outputs:
    ''' 
    ''' + negative integer/zero: no limits[default]
    ''' + positive value: top motifs with score desc
    ''' </param>
    ''' <param name="noccurs%"></param>
    ''' <returns></returns>
    <ExportAPI("find_motifs")>
    <RApiReturn(GetType(SequenceMotif))>
    Public Function GetMotifs(<RRawVectorArgument> fasta As Object,
                              Optional minw% = 8,
                              Optional maxw% = 20,
                              Optional nmotifs% = -1,
                              Optional noccurs% = 12,
                              Optional seedingCutoff As Double = 0.65,
                              Optional scanMinW As Integer = 6,
                              Optional scanCutoff As Double = 0.8,
                              Optional cleanMotif As Double = 0.5,
                              Optional significant_sites As Integer = 4,
                              <RRawVectorArgument>
                              Optional seeds As Object = Nothing,
                              Optional debug As Boolean = False,
                              Optional env As Environment = Nothing) As Object

        Dim param As New PopulatorParameter With {
            .maxW = maxw,
            .minW = minw,
            .seedingCutoff = seedingCutoff,
            .ScanMinW = scanMinW,
            .ScanCutoff = scanCutoff,
            .log = env.WriteLineHandler,
            .seedScanner = Scanners.GraphScan,
            .significant_sites = significant_sites,
            .seedOccurances = 6
        }
        Dim seqInputs = GetFastaSeq(fasta, env).ToArray
        Dim motifs As SequenceMotif()

        If seeds Is Nothing Then
            'motifs = seqInputs.PopulateMotifs(
            '    leastN:=noccurs,
            '    param:=param,
            '    cleanMotif:=cleanMotif,
            '    debug:=debug
            ').ToArray
            Dim seedList = seqInputs.RandomSeed(100, New IntRange(6, 20)).ToArray
            ' seedList = GraphSeed.UMAP(seedList, 30).ToArray
            Dim clusters = FileName.Cluster(seedList, 0.5).ToArray

            Call VBDebugger.EchoLine($"create motifs for {clusters.Length} seeds clusters!")

            motifs = clusters _
                .Select(Function(c) c.CreateMotifs(param)) _
                .Where(Function(m) Not m Is Nothing) _
                .ToArray
        Else
            Dim seedsList As HSP()

            If TypeOf seeds Is ScanFile Then
                seedsList = DirectCast(seeds, ScanFile).LoadAllSeeds.ToArray
            Else
                Dim pop = pipeline.TryCreatePipeline(Of HSP)(seeds, env)

                If pop.isError Then
                    Return pop.getError
                Else
                    seedsList = pop.populates(Of HSP)(env).ToArray
                End If
            End If

            motifs = seedsList.PopulateMotifs(
                param:=param,
                leastN:=noccurs,
                cleanMotif:=cleanMotif,
                debug:=debug
            ).ToArray
        End If

        motifs = motifs _
            .OrderByDescending(Function(m) m.AverageScore) _
            .ToArray

        If nmotifs > 0 Then
            Return motifs.Take(nmotifs).ToArray
        Else
            Return motifs
        End If
    End Function

    ''' <summary>
    ''' Drawing the sequence logo just simply modelling this motif site 
    ''' from the clustal multiple sequence alignment.
    ''' </summary>
    ''' <param name="MSA"></param>
    ''' <param name="title"></param>
    ''' <returns></returns>
    <ExportAPI("plot.seqLogo")>
    <RApiReturn(GetType(GraphicsData))>
    Public Function DrawLogo(<RRawVectorArgument> MSA As Object,
                             Optional title$ = "",
                             Optional env As Environment = Nothing) As Object

        Dim driver As Drivers = env.getDriver

        If MSA Is Nothing Then
            Return REnv.Internal.debug.stop("MSA is nothing!", env)
        End If

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(MSA, env)

        If data Is Nothing Then
            Dim type As Type = MSA.GetType

            Select Case type
                Case GetType(SequenceMotif)
                    data = DirectCast(MSA, SequenceMotif).seeds.ToFasta
                Case GetType(MSAOutput), GetType(MSAMotif)
                    data = DirectCast(MSA, MSAOutput).PopulateAlignment
                Case Else
                    Return REnv.Internal.debug.stop(New InvalidProgramException($"un-supported clr object type for extract MSA data: {type.FullName}!"), env)
            End Select
        End If

        Return DrawingDevice.DrawFrequency(New FastaFile(data), title, driver:=driver)
    End Function

    ''' <summary>
    ''' analyses orthogonality of two DNA-Origami scaffold strands.
    ''' Multiple criteria For orthogonality Of the two sequences can be specified
    ''' to determine the level of orthogonality.
    ''' </summary>
    ''' <param name="scaffolds"></param>
    ''' <param name="segment_len">segment length</param>
    ''' <param name="is_linear">scaffolds are not circular</param>
    ''' <param name="rev_compl">also count reverse complementary sequences</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("scaffold.orthogonality")>
    <RApiReturn(GetType(DNAOrigami.Output))>
    Public Function ScaffoldOrthogonality(<RRawVectorArgument>
                                          scaffolds As Object,
                                          Optional segment_len% = 7,
                                          Optional is_linear As Boolean = False,
                                          Optional rev_compl As Boolean = False,
                                          Optional env As Environment = Nothing) As Object

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(scaffolds, env)

        If data Is Nothing Then
            Return RInternal.debug.stop({
                "invalid data type for sequence data input!",
               $"required: fasta",
               $"given: {scaffolds.GetType.FullName}"
            }, env)
        End If

        Dim seqs As FastaSeq() = data.ToArray
        Dim outputs As New List(Of DNAOrigami.Output)
        Dim args As New Project With {
            .n = segment_len,
            .is_linear = is_linear,
            .get_rev_compl = rev_compl
        }

        For Each x As FastaSeq In seqs
            For Each y As FastaSeq In seqs.Where(Function(a) Not a Is x)
                Call CheckOrthogonality(x, y, project:=args).DoCall(AddressOf outputs.Add)
            Next
        Next

        Return outputs.ToArray
    End Function

    ''' <summary>
    ''' split the motif matches result in parts by its gene source
    ''' </summary>
    ''' <param name="matches"></param>
    ''' <param name="gff"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("split_match_source")>
    Public Function SplitMatchesSource(<RRawVectorArgument> matches As Object, Optional gff As GFFTable = Nothing, Optional env As Environment = Nothing) As Object
        Dim matchList = pipeline.TryCreatePipeline(Of MotifMatch)(matches, env)

        If matchList.isError Then
            Dim filepath As String = CLRVector.asScalarCharacter(matches)

            If Not filepath.FileExists Then
                Return matchList.getError
            End If

            matchList = filepath _
                .OpenHandle() _
                .AsLinq(Of MotifMatch) _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If

        Dim sourceList As New Dictionary(Of String, List(Of MotifMatch))
        Dim hashContextData As Boolean = Not gff Is Nothing

        If hashContextData Then
            Dim context = gff.features _
                .GroupBy(Function(a) a.ID) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.First
                              End Function)

            For Each match As MotifMatch In matchList.populates(Of MotifMatch)(env)
                Dim feature = context(match.title)
                Dim source As String = feature.seqname

                If Not sourceList.ContainsKey(source) Then
                    Call sourceList.Add(source, New List(Of MotifMatch))
                End If

                Call sourceList(source).Add(match)
            Next
        Else
            For Each match As MotifMatch In matchList.populates(Of MotifMatch)(env)
                Dim title As String() = match.title.Split("|"c)
                Dim source As String = title(0)

                If Not sourceList.ContainsKey(source) Then
                    Call sourceList.Add(source, New List(Of MotifMatch))
                End If

                Call sourceList(source).Add(match)
            Next
        End If

        Return New list(sourceList.ToDictionary(Function(a) a.Key, Function(a) a.Value.ToArray))
    End Function
End Module
