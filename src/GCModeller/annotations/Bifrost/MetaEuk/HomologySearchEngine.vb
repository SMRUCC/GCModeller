
' ========================================================================
' MODULE 5: HOMOLOGY SEARCH ENGINE
' ========================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Class HomologySearchEngine : Inherits VectorTask

    ReadOnly references As FastaSeq()
    ReadOnly hits As New List(Of HomologyHit)
    ReadOnly config As MetaEukConfig

    Dim frag As CandidateFragment

    Public ReadOnly Property nsize As Integer
        Get
            Return hits.Count
        End Get
    End Property

    Public Sub New(references As IReadOnlyCollection(Of FastaSeq), config As MetaEukConfig)
        MyBase.New(references.Count)
        Me.references = references.ToArray()
        Me.config = config
    End Sub

    Shared Sub New()
        VectorTask.n_threads = 12
    End Sub

    Public Sub Search(frag As CandidateFragment)
        Me.frag = frag
        Call Run()
    End Sub

    Public Function GetResult() As IEnumerable(Of HomologyHit)
        Return hits
    End Function

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        Dim hits As New List(Of HomologyHit)
        Dim gapOpen As Integer = 11
        Dim gapExtend As Integer = 1

        For i As Integer = start To ends
            Dim refSeq As FastaSeq = references(i)
            Dim hit = SmithWatermanAligner.Align(
                  frag.Peptide, refSeq.SequenceData, gapOpen, gapExtend, config)

            If hit IsNot Nothing Then
                hit.Fragment = frag
                hit.TargetID = refSeq.locus_tag
                hits.Add(hit)
            End If
        Next

        If hits.Any Then
            SyncLock Me.hits
                Call Me.hits.AddRange(hits)
            End SyncLock
        End If
    End Sub

    ''' <summary>
    ''' Search all candidate fragments against reference protein database.
    ''' Returns list of significant homology hits.
    ''' </summary>
    Public Shared Function SearchAll(
        fragments As List(Of CandidateFragment),
        references As List(Of FastaSeq),
        config As MetaEukConfig) As List(Of HomologyHit)

        Dim align As New HomologySearchEngine(references, config)

        Console.WriteLine($"[INFO] Searching {fragments.Count} fragments against {references.Count} reference proteins...")

        For Each frag As CandidateFragment In TqdmWrapper.Wrap(fragments)
            Call align.Search(frag)
        Next

        Console.WriteLine($"[INFO] Found {align.nsize} significant homology hits")

        Return align.GetResult
    End Function

End Class
