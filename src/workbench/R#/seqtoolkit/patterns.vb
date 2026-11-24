#Region "Microsoft.VisualBasic::6fce360df57b686ef6f95ec2ec4089bd, R#\seqtoolkit\patterns.vb"

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

    '   Total Lines: 730
    '    Code Lines: 505 (69.18%)
    ' Comment Lines: 131 (17.95%)
    '    - Xml Docs: 89.31%
    ' 
    '   Blank Lines: 94 (12.88%)
    '     File Size: 30.30 KB


    ' Module patterns
    ' 
    '     Function: createSeeds, DrawLogo, FindMirrorPalindromes, GetMotifs, GetSeeds
    '               gibbs_scans, gibbs_table, matchSites, matchTableOutput, MotifString
    '               openSeedFile, PalindromeToString, plotMotif, pullAllSeeds, read_memexml
    '               readMotifs, readSites, ScaffoldOrthogonality, seqGraph, seqgraph_df
    '               SplitMatchesSource, top_sites, toPWM, viewSites
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
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.Analysis.SequenceTools
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
        Call REnv.Internal.generic.add("plot", GetType(Probability), AddressOf plotMotif)

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

    ''' <summary>
    ''' open the sequence seed scan data file
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the seed scan data file, or a file stream object of 
    ''' the target seed scan data file.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="ScanFile"/> sequence seed data file object for read or 
    ''' save the seed data in a stream manner, which can be used by the 
    ''' ``pull.all_seeds`` api or the ``create.seeds`` api;
    ''' 
    ''' this function returns a R# error message object if the given file can 
    ''' not be opened for read/write.
    ''' </returns>
    <ExportAPI("open.seedFile")>
    Public Function openSeedFile(<RRawVectorArgument> file As Object, Optional env As Environment = Nothing) As Object
        Dim filesave = SMRUCC.Rsharp.GetFileStream(file, FileAccess.ReadWrite, env)

        If filesave Like GetType(Message) Then
            Return filesave.TryCast(Of Message)
        End If

        Return New ScanFile(filesave.TryCast(Of Stream))
    End Function

    ''' <summary>
    ''' pull all of the sequence seed data from the given seed scan data file
    ''' </summary>
    ''' <param name="seed">
    ''' a <see cref="ScanFile"/> sequence seed data file object that is opened 
    ''' by the ``open.seedFile`` api.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="HSP"/> sequence seed object that loaded from 
    ''' the given seed data file.
    ''' </returns>
    <ExportAPI("pull.all_seeds")>
    Public Function pullAllSeeds(seed As ScanFile) As HSP()
        Return seed.LoadAllSeeds.ToArray
    End Function

    ''' <summary>
    ''' read the xml motif data model output from the meme program
    ''' </summary>
    ''' <param name="file">
    ''' the file path of the MEME suite xml format motif discovery output 
    ''' document(``meme.xml``).
    ''' </param>
    ''' <returns>
    ''' a <see cref="MEMEXml"/> meme document object model, which can be used 
    ''' for extract the motif PWM model via the ``toPWM`` api.
    ''' </returns>
    <ExportAPI("read.meme_xml")>
    Public Function read_memexml(file As String) As MEMEXml
        Return MEMEXml.LoadDocument(file)
    End Function

    ''' <summary>
    ''' convert the meme document to motif PWM model object
    ''' </summary>
    ''' <param name="meme">
    ''' a <see cref="MEMEXml"/> meme document object that is read by the 
    ''' ``read.meme_xml`` api.
    ''' </param>
    ''' <returns>a vector of the PWM clr object.</returns>
    <ExportAPI("toPWM")>
    <RApiReturn(GetType(Probability))>
    Public Function toPWM(meme As MEMEXml) As Object
        Return meme.GetMotifs.ToArray
    End Function

    ''' <summary>
    ''' make a motif scan from the given sequence collection
    ''' </summary>
    ''' <param name="seqs">
    ''' a fasta sequence collection for run the gibbs sampler motif discovery, 
    ''' which can be a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="width">
    ''' the motif width of the gibbs sampler. If this parameter is not 
    ''' specified, then the motif width will be evaluated from the input 
    ''' sequence data automatically as the 60% of the average sequence length.
    ''' </param>
    ''' <param name="maxitr">
    ''' the maximum iteration number of the gibbs sampler.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="MSAMotif"/> motif object that discovered from the given 
    ''' sequence collection by the gibbs sampler;
    ''' 
    ''' this function returns NULL if the input sequence data can not be cast 
    ''' to a fasta sequence collection.
    ''' </returns>
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
    ''' display the motif match sites on the given target sequence
    ''' </summary>
    ''' <param name="sites">
    ''' a collection of the motif match site data(<see cref="Site"/>) for 
    ''' display on the target sequence.
    ''' </param>
    ''' <param name="seq">
    ''' the target sequence data for display the motif match sites, which can 
    ''' be a raw sequence text or a fasta sequence object.
    ''' </param>
    ''' <param name="deli">
    ''' the delimiter string for display the sequence fragment of each match 
    ''' site.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a character value of the formatted motif match site display text.
    ''' </returns>
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
    ''' <param name="file">
    ''' the file path of the sequence motif json data document.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="SequenceMotif"/> motif object that loaded 
    ''' from the given json data file.
    ''' </returns>
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
    ''' <param name="file">
    ''' a file path to a csv format motif match scan result table file, which 
    ''' is generated by the ``motif.find_sites`` api via the ``write.csv`` api.
    ''' </param>
    ''' <param name="tqdm">
    ''' read the table data in a lazy stream mode? if this parameter is TRUE, 
    ''' then a lazy pipeline collection will be returned, which is helpful for 
    ''' read a huge csv table file without loading all of the data rows into 
    ''' the memory at once.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="MotifMatch"/> motif match scan result, or a 
    ''' lazy pipeline collection of the <see cref="MotifMatch"/> object when 
    ''' the ``tqdm`` parameter is TRUE.
    ''' </returns>
    <ExportAPI("read.scans")>
    <RApiReturn(GetType(MotifMatch))>
    Public Function readSites(file As String, Optional tqdm As Boolean = False) As Object
        If tqdm Then
            Return file.OpenHandle.AsLinq(Of MotifMatch).as_iterator
        Else
            Return file.LoadCsv(Of MotifMatch)(mute:=True).ToArray
        End If
    End Function

    ''' <summary>
    ''' takes the top motif match sites by a set of the given filter threshold 
    ''' values
    ''' </summary>
    ''' <param name="sites">
    ''' a vector of the <see cref="MotifMatch"/> motif match scan result for 
    ''' make the filter.
    ''' </param>
    ''' <param name="identities">
    ''' the minimum identity threshold value of the accepted motif matches.
    ''' </param>
    ''' <param name="pvalue">
    ''' the maximum p-value threshold of the accepted motif matches.
    ''' </param>
    ''' <param name="minW">
    ''' the minimum motif width of the accepted motif matches.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="MotifMatch"/> object that contains the 
    ''' accepted motif match sites: the input motif matches will be passed 
    ''' through each of the specified filter thresholds in turn, and only the 
    ''' matches that satisfies all of the specified threshold conditions will 
    ''' be kept in the returned result.
    ''' </returns>
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
    ''' make the sequence graph embedding data of the given sequence collection
    ''' </summary>
    ''' <param name="fasta">
    ''' a fasta sequence collection for make the sequence graph embedding, 
    ''' which can be a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="mol_type">
    ''' the molecule type of the input sequence data for select the graph 
    ''' embedding algorithm.
    ''' </param>
    ''' <param name="parallel">
    ''' run the graph embedding task in parallel on multiple cpu cores?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the sequence graph embedding vector data is generates from different method 
    ''' based on the <paramref name="mol_type"/> data:
    ''' 
    ''' + <see cref="SeqTypes.DNA"/>: <see cref="SMRUCC.genomics.Model.MotifGraph.Builder.DNAGraph"/>
    ''' + <see cref="SeqTypes.Protein"/>: <see cref="SMRUCC.genomics.Model.MotifGraph.Builder.PolypeptideGraph"/>
    ''' + <see cref="SeqTypes.RNA"/>: <see cref="SMRUCC.genomics.Model.MotifGraph.Builder.RNAGraph"/>
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
            Case SeqTypes.DNA : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf SMRUCC.genomics.Model.MotifGraph.DNAGraph, parallel:=parallel)
            Case SeqTypes.Protein : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf SMRUCC.genomics.Model.MotifGraph.PolypeptideGraph, parallel:=parallel)
            Case SeqTypes.RNA : Return env.EvaluateFramework(Of FastaSeq, SequenceGraph)(seqList, AddressOf SMRUCC.genomics.Model.MotifGraph.RNAGraph, parallel:=parallel)
            Case Else
                Return RInternal.debug.stop("general is not allowed!", env)
        End Select
    End Function

    ''' <summary>
    ''' find the target loci match sites of the given sequence data based on 
    ''' the motif PWM model
    ''' </summary>
    ''' <param name="motif">
    ''' the motif PWM model for make the site scan, which could be a 
    ''' <see cref="SequenceMotif"/> or a <see cref="MSAMotif"/> object.
    ''' </param>
    ''' <param name="target">
    ''' a fasta sequence collection for make the motif site scan, which can be 
    ''' a single <see cref="FastaSeq"/> object, a <see cref="FastaFile"/> 
    ''' object, or a character vector of the raw sequence data.
    ''' </param>
    ''' <param name="cutoff#">
    ''' the minimum similarity score threshold value between the candidate 
    ''' sequence region and the motif PWM model.
    ''' </param>
    ''' <param name="minW#">
    ''' the minimum width of the candidate match site region.
    ''' </param>
    ''' <param name="identities">
    ''' the minimum identity threshold value of the accepted match sites.
    ''' </param>
    ''' <param name="pvalue">
    ''' the maximum p-value threshold of the accepted match sites.
    ''' </param>
    ''' <param name="parallel">
    ''' run the site scan task in parallel on multiple cpu cores?
    ''' </param>
    ''' <param name="motif_name">
    ''' the motif name that will be recorded in the ``seeds`` property of the 
    ''' match result. If this parameter is not specified, then the motif name 
    ''' of the given PWM model object will be used.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="MotifMatch"/> motif match result;
    ''' 
    ''' this function returns a R# error message object if the given motif 
    ''' model object or the target sequence data is not a supported data 
    ''' model.
    ''' </returns>
    <ExportAPI("motif.find_sites")>
    <RApiReturn(GetType(MotifMatch))>
    Public Function matchSites(motif As Object,
                               <RRawVectorArgument>
                               target As Object,
                               Optional cutoff# = 0.6,
                               Optional minW# = 8,
                               Optional identities As Double = 0.85,
                               Optional pvalue As Double = 0.05,
                               Optional parallel As Boolean = False,
                               Optional motif_name As String = Nothing,
                               Optional env As Environment = Nothing) As Object

        Dim PWM As SequencePatterns.Residue()
        Dim seedName As String = motif_name

        If motif Is Nothing Then
            Call "the required motif PWM model is nothing".warning
            Return Nothing
        End If

        If TypeOf motif Is SequenceMotif Then
            PWM = DirectCast(motif, SequenceMotif).region

            If motif_name Is Nothing Then
                seedName = DirectCast(motif, SequenceMotif).name
            End If
        ElseIf TypeOf motif Is MSAMotif Then
            PWM = DirectCast(motif, MSAMotif).PWM.ToArray
        Else
            Return Message.InCompatibleType(GetType(SequenceMotif), motif.GetType, env)
        End If

        If target Is Nothing Then
            Return RInternal.debug.stop("sequence target can not be nothing!", env)
        ElseIf TypeOf target Is FastaSeq Then
            ' scan a simple single sequence
            Return PWM _
                .ScanSites(DirectCast(target, FastaSeq), cutoff, minW,
                           pvalue_cut:=pvalue,
                           identities:=identities) _
                .Select(Function(s)
                            s.seeds = {seedName}
                            Return s
                        End Function) _
                .ToArray
        Else
            Dim seqs As IEnumerable(Of FastaSeq) = GetFastaSeq(target, env)

            ' scan multiple sequence
            If seqs Is Nothing Then
                Return RInternal.debug.stop($"invalid sequence collection type: {target.GetType.FullName}", env)
            Else
                Return seqs.ToArray _
                    .Populate(parallel, App.CPUCoreNumbers) _
                    .Select(Function(seq)
                                Return PWM.ScanSites(seq, cutoff, minW,
                                                     identities:=identities,
                                                     pvalue_cut:=pvalue)
                            End Function) _
                    .IteratesALL _
                    .Select(Function(s)
                                s.seeds = {seedName}
                                Return s
                            End Function) _
                    .ToArray
            End If
        End If
    End Function

    ''' <summary>
    ''' Search mirror palindrome sites for a given seed sequence
    ''' </summary>
    ''' <param name="sequence">
    ''' the raw nucleotide sequence text for search the mirror palindrome 
    ''' sites.
    ''' </param>
    ''' <param name="seed">
    ''' the seed sequence fragment text for search its mirror palindrome sites.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="PalindromeLoci"/> mirror palindrome site 
    ''' loci object that found in the given sequence data.
    ''' </returns>
    <ExportAPI("palindrome.mirror")>
    Public Function FindMirrorPalindromes(sequence$, seed$) As PalindromeLoci()
        Return Palindrome.FindMirrorPalindromes(seed, sequence)
    End Function

    ''' <summary>
    ''' create all of the possible seed sequence fragments of the given 
    ''' alphabet base letters
    ''' </summary>
    ''' <param name="size">
    ''' the seed length in chars, i.e. the ``k`` value of the k-mer seeds.
    ''' </param>
    ''' <param name="base">
    ''' a character value of the sequence alphabet letters, example as 
    ''' ``ACGT`` for the nucleotide sequence.
    ''' </param>
    ''' <returns>
    ''' a character vector of all of the possible combination of the seed 
    ''' sequence fragments, and the size of the generated seed collection is 
    ''' ``len(base) ^ size``.
    ''' </returns>
    <ExportAPI("seeds")>
    Public Function GetSeeds(size As Integer, base As String) As String()
        Return Seeds.InitializeSeeds(base.ToArray, size)
    End Function

    ''' <summary>
    ''' convert the given sequence motif object as a regexp liked format 
    ''' pattern string for do motif matches
    ''' </summary>
    ''' <param name="motif">
    ''' a <see cref="SequenceMotif"/> motif object for convert as the pattern 
    ''' string text.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>the regexp liked format string for do motif matches</returns>
    <ExportAPI("motifString")>
    <RApiReturn(GetType(String))>
    Public Function MotifString(<RRawVectorArgument> motif As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of SequenceMotif, String)(motif, Function(m) m.patternString())
    End Function

    ''' <summary>
    ''' create the sequence seeds data from the given fasta sequence 
    ''' collection, and then save the generated seed data into the target seed 
    ''' data file
    ''' </summary>
    ''' <param name="fasta">
    ''' a fasta sequence collection for make the sequence seeds, which can be 
    ''' a <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="saveto">
    ''' a <see cref="ScanFile"/> sequence seed data file object for save the 
    ''' generated seed data, which is created by the ``open.seedFile`` api.
    ''' </param>
    ''' <param name="minw%">
    ''' the minimum seed width in chars.
    ''' </param>
    ''' <param name="maxw%">
    ''' the maximum seed width in chars.
    ''' </param>
    ''' <param name="seedingCutoff">
    ''' the similarity cutoff threshold value for build the seed clusters.
    ''' </param>
    ''' <param name="scanMinW">
    ''' the minimum width of the seed scan region.
    ''' </param>
    ''' <param name="scanCutoff">
    ''' the similarity score cutoff threshold value of the seed scan.
    ''' </param>
    ''' <param name="significant_sites">
    ''' the minimum number of the significant sites for keep a seed.
    ''' </param>
    ''' <param name="debug">
    ''' print the debug log message of the seed scan progress?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the input <see cref="ScanFile"/> sequence seed data file object with 
    ''' the generated seed data has been written into it.
    ''' </returns>
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
    ''' <param name="fasta">
    ''' a fasta sequence collection for make the motif discovery, which should 
    ''' contains multiple sequence.
    ''' </param>
    ''' <param name="minw%">
    ''' the minimum motif width in chars.
    ''' </param>
    ''' <param name="maxw%">
    ''' the maximum motif width in chars.
    ''' </param>
    ''' <param name="nmotifs">
    ''' A number for limit the number of motif outputs:
    ''' 
    ''' + negative integer/zero: no limits[default]
    ''' + positive value: top motifs with score desc
    ''' </param>
    ''' <param name="noccurs%">
    ''' the minimum occurrence number of the motif sites in the input sequence 
    ''' collection for keep a motif.
    ''' </param>
    ''' <param name="seedingCutoff">
    ''' the similarity cutoff threshold value for build the seed clusters.
    ''' </param>
    ''' <param name="scanMinW">
    ''' the minimum width of the seed scan region.
    ''' </param>
    ''' <param name="scanCutoff">
    ''' the similarity score cutoff threshold value of the seed scan.
    ''' </param>
    ''' <param name="cleanMotif">
    ''' the motif cleaning threshold value for remove the low score motif 
    ''' sites from the generated motif model.
    ''' </param>
    ''' <param name="significant_sites">
    ''' the minimum number of the significant sites for keep a seed.
    ''' </param>
    ''' <param name="seeds">
    ''' the pre-computed seed data for make the motifs, which can be a 
    ''' <see cref="ScanFile"/> object that is created by the ``create.seeds`` 
    ''' api, or a collection of the <see cref="HSP"/> seed object. If this 
    ''' parameter is not specified, then the seeds will be discovered from the 
    ''' input sequence data automatically.
    ''' </param>
    ''' <param name="debug">
    ''' print the debug log message of the motif discovery progress?
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="SequenceMotif"/> motif object that 
    ''' discovered from the given sequence collection, which is sorted by the 
    ''' average motif site score in descending order.
    ''' </returns>
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
            Dim clusters = GraphSeedTool.Cluster(seedList, 0.5).ToArray

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
    ''' <param name="MSA">
    ''' the multiple sequence alignment data for drawing the sequence logo, 
    ''' which can be a <see cref="MSAOutput"/> alignment result object, a 
    ''' <see cref="SequenceMotif"/> motif object, a <see cref="MSAMotif"/> 
    ''' gibbs sampler result, a <see cref="Probability"/> PWM model object, or 
    ''' a fasta sequence collection.
    ''' </param>
    ''' <param name="title">
    ''' the title text of the generated sequence logo graphics.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a <see cref="GraphicsData"/> graphics data object of the sequence 
    ''' logo, which can be saved as a image file via the ``bitmap`` or 
    ''' ``svg`` api.
    ''' </returns>
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
        Dim pwm As MotifPWM

        If data Is Nothing Then
            Dim type As Type = MSA.GetType

            Select Case type
                Case GetType(SequenceMotif)
                    pwm = DirectCast(MSA, SequenceMotif).CreateModel
                Case GetType(MSAOutput)
                    data = DirectCast(MSA, MSAOutput).PopulateAlignment
                    pwm = SequencePatterns.Motif.PWM.FromMla(New FastaFile(data))
                Case GetType(MSAMotif)
                    pwm = DirectCast(MSA, MSAMotif).CreateMotif
                Case GetType(Probability)
                    pwm = DirectCast(MSA, Probability).CreateModel
                Case Else
                    Return REnv.Internal.debug.stop(New InvalidProgramException($"un-supported clr object type for extract MSA data: {type.FullName}!"), env)
            End Select
        Else
            pwm = SequencePatterns.Motif.PWM.FromMla(New FastaFile(data))
        End If

        Return DrawingDevice.DrawFrequency(pwm, title, driver:=driver)
    End Function

    ''' <summary>
    ''' analyses orthogonality of two DNA-Origami scaffold strands.
    ''' Multiple criteria For orthogonality Of the two sequences can be specified
    ''' to determine the level of orthogonality.
    ''' </summary>
    ''' <param name="scaffolds">
    ''' a collection of the DNA-Origami scaffold nucleotide sequences for 
    ''' evaluate the pairwise orthogonality, which can be a 
    ''' <see cref="FastaFile"/> object, a collection of the 
    ''' <see cref="FastaSeq"/> object, or a character vector of the raw 
    ''' sequence data.
    ''' </param>
    ''' <param name="segment_len">segment length</param>
    ''' <param name="is_linear">scaffolds are not circular</param>
    ''' <param name="rev_compl">also count reverse complementary sequences</param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a vector of the <see cref="DNAOrigami.Output"/> orthogonality 
    ''' evaluation result: one result element for each of the scaffold 
    ''' sequence pair in the input sequence collection.
    ''' </returns>
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
    ''' <param name="matches">
    ''' the motif match result data for split, which can be a vector of the 
    ''' <see cref="MotifMatch"/> object or a file path of the csv format motif 
    ''' match result table.
    ''' </param>
    ''' <param name="gff">
    ''' a <see cref="GFFTable"/> genomics feature annotation table for map the 
    ''' match result to its source gene feature. If this parameter is not 
    ''' specified, then the match result will be grouped by the first token of 
    ''' the match title.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' a named list of the <see cref="MotifMatch"/> match result group: the 
    ''' name of each list element is the gene source id, and the element value 
    ''' is a vector of the <see cref="MotifMatch"/> object that belongs to the 
    ''' corresponding gene source.
    ''' </returns>
    <ExportAPI("split_match_source")>
    Public Function SplitMatchesSource(<RRawVectorArgument>
                                       matches As Object,
                                       Optional gff As GFFTable = Nothing,
                                       Optional env As Environment = Nothing) As Object

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
