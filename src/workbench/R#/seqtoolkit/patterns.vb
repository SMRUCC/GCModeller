
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' Tools for sequence patterns
''' </summary>
<Package("bioseq.patterns", Category:=APICategories.ResearchTools)>
Module patterns

    Sub New()
        Call REnv.AttachConsoleFormatter(Of PalindromeLoci)(AddressOf PalindromeToString)
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
                              Optional noccurs% = 6) As Motif()

        Dim param As New PopulatorParameter With {
            .maxW = maxw,
            .minW = minw,
            .seedingCutoff = 0.95,
            .ScanMinW = 6,
            .ScanCutoff = 0.8
        }
        Dim motifs As Motif() = GetFastaSeq(fasta) _
            .PopulateMotifs(
                leastN:=noccurs,
                param:=param
            ) _
            .OrderByDescending(Function(m) m.score / m.seeds.MSA.Length) _
            .Take(nmotifs) _
            .ToArray

        Return motifs
    End Function
End Module
