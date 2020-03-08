
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.SequenceLogo
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' Tools for sequence patterns
''' </summary>
<Package("bioseq.patterns", Category:=APICategories.ResearchTools)>
Module patterns

    Sub New()
        Call REnv.Internal.ConsolePrinter.AttachConsoleFormatter(Of PalindromeLoci)(AddressOf PalindromeToString)
    End Sub

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

    <ExportAPI("read.motifs")>
    Public Function readMotifs(file As String) As SequenceMotif()
        Return file.LoadJSON(Of SequenceMotif())
    End Function

    ''' <summary>
    ''' Find target loci site based on the given motif model
    ''' </summary>
    ''' <param name="motif"></param>
    ''' <param name="target"></param>
    ''' <param name="cutoff#"></param>
    ''' <param name="minW#"></param>
    ''' <returns></returns>
    <ExportAPI("motif.find_sites")>
    <RApiReturn(GetType(SimpleSegment()))>
    Public Function matchSites(motif As SequenceMotif,
                               <RRawVectorArgument>
                               target As Object,
                               Optional cutoff# = 0.6,
                               Optional minW# = 6,
                               Optional identities As Double = 0.85,
                               Optional env As Environment = Nothing) As Object

        If target Is Nothing Then
            Return Internal.debug.stop("sequence target can not be nothing!", env)
        ElseIf TypeOf target Is FastaSeq Then
            Return motif.region.ScanSites(DirectCast(target, FastaSeq), cutoff, minW, identities)
        Else
            Dim seqs = GetFastaSeq(target)

            If seqs Is Nothing Then
                Return Internal.debug.stop($"invalid sequence collection type: {target.GetType.FullName}", env)
            Else
                Return seqs _
                    .AsParallel _
                    .Select(Function(seq)
                                Dim locis = motif.region.ScanSites(seq, cutoff, minW, identities)

                                For Each site As SimpleSegment In locis
                                    site.ID = seq.Title.Split.First
                                Next

                                Return locis
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

    <ExportAPI("seeds")>
    Public Function GetSeeds(size As Integer, base As String()) As String()
        Return Seeds.InitializeSeeds(base.Select(Function(s) CChar(s)).ToArray, size)
    End Function

    <ExportAPI("find_motifs")>
    Public Function GetMotifs(<RRawVectorArgument> fasta As Object,
                              Optional minw% = 6,
                              Optional maxw% = 20,
                              Optional nmotifs% = 25,
                              Optional noccurs% = 6) As SequenceMotif()

        Dim param As New PopulatorParameter With {
            .maxW = maxw,
            .minW = minw,
            .seedingCutoff = 0.95,
            .ScanMinW = 6,
            .ScanCutoff = 0.8
        }
        Dim motifs As SequenceMotif() = GetFastaSeq(fasta) _
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

        Dim data As IEnumerable(Of FastaSeq) = GetFastaSeq(MSA)

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
End Module
