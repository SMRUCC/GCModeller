Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.SequenceModel.FASTA

Module MotifSeeds

    ''' <summary>
    ''' create seeds via pairwise alignment, use 
    ''' the smith-waterman HSP as motif seeds.
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="q"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    <Extension>
    Friend Function LocalSeeding(regions As IEnumerable(Of FastaSeq), q As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim seeds As New List(Of HSP)

        ' the object reference is breaked at parallel
        ' use the title for test equals instead of test equals via hashcode
        ' at here
        For Each s As FastaSeq In regions.Where(Function(seq) Not seq.Title = q.Title)
            seeds += PairwiseSeeding(q, s, param)
        Next

        Return seeds
    End Function

    Public Function PairwiseSeeding(q As FastaSeq, s As FastaSeq, param As PopulatorParameter) As IEnumerable(Of HSP)
        Dim smithWaterman As New SmithWaterman(q.SequenceData, s.SequenceData, New DNAMatrix)
        Call smithWaterman.BuildMatrix()
        Dim result = smithWaterman.GetOutput(param.seedingCutoff, param.minW)
        Return result.HSP.Where(Function(seed) seed.LengthHit <= param.maxW)
    End Function
End Module
