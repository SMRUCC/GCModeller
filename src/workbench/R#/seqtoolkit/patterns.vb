#Region "Microsoft.VisualBasic::212f2f5ae3522fd320b63faeedcf8ad8, R#\seqtoolkit\patterns.vb"

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

' Module patterns
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: DrawLogo, FindMirrorPalindromes, GetMotifs, GetSeeds, matchSites
'               matchTableOutput, PalindromeToString, readMotifs, readSites, ScaffoldOrthogonality
'               viewSites
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.DNAOrigami
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.GCModeller.Workbench.SeqFeature
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.Rsharp
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Tools for sequence patterns
''' </summary>
<Package("bioseq.patterns", Category:=APICategories.ResearchTools)>
Module patterns

    Sub New()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of PalindromeLoci)(AddressOf PalindromeToString)
        Call REnv.Internal.Object.Converts.makeDataframe.addHandler(GetType(MotifMatch()), AddressOf matchTableOutput)
        Call REnv.Internal.generic.add("plot", GetType(SequenceMotif), AddressOf plotMotif)
    End Sub

    Private Function plotMotif(motif As SequenceMotif, args As list, env As Environment) As Object
        Dim data As IEnumerable(Of FastaSeq) = DirectCast(motif, SequenceMotif).seeds.ToFasta
        Dim title As String = args.getValue(Of String)("title", env, [default]:="")

        Return DrawingDevice.DrawFrequency(New FastaFile(data), title)
    End Function

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

    <ExportAPI("read.scans")>
    Public Function readSites(file As String) As MotifMatch()
        Return file.LoadCsv(Of MotifMatch).ToArray
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
                               Optional minW# = 6,
                               Optional identities As Double = 0.85,
                               Optional parallel As Boolean = False,
                               Optional env As Environment = Nothing) As Object

        If target Is Nothing Then
            Return Internal.debug.stop("sequence target can not be nothing!", env)
        ElseIf TypeOf target Is FastaSeq Then
            Return motif.region _
                .ScanSites(DirectCast(target, FastaSeq), cutoff, minW, identities) _
                .ToArray
        Else
            Dim seqs = GetFastaSeq(target, env)

            If seqs Is Nothing Then
                Return Internal.debug.stop($"invalid sequence collection type: {target.GetType.FullName}", env)
            Else
                Return seqs.ToArray _
                    .Populate(parallel, App.CPUCoreNumbers) _
                    .Select(Function(seq)
                                Return motif.ScanSites(seq, cutoff, minW, identities)
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

    <ExportAPI("motifString")>
    Public Function MotifString(<RRawVectorArgument> motif As Object, Optional env As Environment = Nothing) As Object
        Return env.EvaluateFramework(Of SequenceMotif, String)(motif, Function(m) m.patternString())
    End Function

    ''' <summary>
    ''' find possible motifs of the given sequence collection
    ''' </summary>
    ''' <param name="fasta"></param>
    ''' <param name="minw%"></param>
    ''' <param name="maxw%"></param>
    ''' <param name="nmotifs%"></param>
    ''' <param name="noccurs%"></param>
    ''' <returns></returns>
    <ExportAPI("find_motifs")>
    Public Function GetMotifs(<RRawVectorArgument> fasta As Object,
                              Optional minw% = 6,
                              Optional maxw% = 20,
                              Optional nmotifs% = 25,
                              Optional noccurs% = 6,
                              Optional seedingCutoff As Double = 0.95,
                              Optional scanMinW As Integer = 6,
                              Optional scanCutoff As Double = 0.8,
                              Optional env As Environment = Nothing) As SequenceMotif()

        Dim param As New PopulatorParameter With {
            .maxW = maxw,
            .minW = minw,
            .seedingCutoff = seedingCutoff,
            .ScanMinW = scanMinW,
            .ScanCutoff = scanCutoff
        }
        Dim motifs As SequenceMotif() = GetFastaSeq(fasta, env) _
            .PopulateMotifs(
                leastN:=noccurs,
                param:=param
            ) _
            .OrderByDescending(Function(m) m.score / m.seeds.MSA.Length) _
            .Take(nmotifs) _
            .ToArray

        Return motifs
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
    Public Function DrawLogo(<RRawVectorArgument> MSA As Object, Optional title$ = "", Optional env As Environment = Nothing) As Object
        If MSA Is Nothing Then
            Return REnv.Internal.debug.stop("MSA is nothing!", env)
        End If

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(MSA, env)

        If data Is Nothing Then
            Dim type As Type = MSA.GetType

            Select Case type
                Case GetType(SequenceMotif)
                    data = DirectCast(MSA, SequenceMotif).seeds.ToFasta
                Case Else
                    Return REnv.Internal.debug.stop(New InvalidProgramException, env)
            End Select
        End If

        Return DrawingDevice.DrawFrequency(New FastaFile(data), title)
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
    <RApiReturn(GetType(Output))>
    Public Function ScaffoldOrthogonality(<RRawVectorArgument>
                                          scaffolds As Object,
                                          Optional segment_len% = 7,
                                          Optional is_linear As Boolean = False,
                                          Optional rev_compl As Boolean = False,
                                          Optional env As Environment = Nothing) As Object

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(scaffolds, env)

        If data Is Nothing Then
            Return Internal.debug.stop({
                "invalid data type for sequence data input!",
               $"required: fasta",
               $"given: {scaffolds.GetType.FullName}"
            }, env)
        End If

        Dim seqs As FastaSeq() = data.ToArray
        Dim outputs As New List(Of Output)
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
End Module
