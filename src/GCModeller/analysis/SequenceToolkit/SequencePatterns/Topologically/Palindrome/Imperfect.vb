Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Pattern
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically

    ''' <summary>
    ''' 在互补链部分的回文，由于Mirror其实就是简单重复序列，所以在这里不再编写了
    ''' </summary>
    Public Class Imperfect : Inherits MirrorSearchs

        ReadOnly _index As TextIndexing
        ReadOnly _cutoff As Double
        Shadows ReadOnly _resultSet As New List(Of ImperfectPalindrome)

        Public Shadows ReadOnly Property ResultSet As ImperfectPalindrome()
            Get
                Return _resultSet.ToArray
            End Get
        End Property

        ''' <summary>
        ''' 种子位点和回文位点之间的最大的距离
        ''' </summary>
        ReadOnly _maxDist As Integer
        ReadOnly _partitions As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                cutoff As Double,
                maxDist As Integer,
                partitions As Integer)
            Call MyBase.New(Sequence, Min, Max)
            _index = New TextIndexing(Sequence.SequenceData, Min, Max)
            _cutoff = cutoff
            _maxDist = maxDist
            _partitions = partitions
        End Sub

        ''' <summary>
        ''' 做blastn？？
        ''' </summary>
        ''' <param name="currentRemoves"></param>
        ''' <param name="currentStat"></param>
        ''' <param name="currLen"></param>
        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As List(Of String), currLen As Integer)
            Dim LQuery = (From segment As String
                          In currentRemoves.AsParallel
                          Let seg As String = Mid(segment, 1, Len(segment) - 1)
                          Let palLoci As String = PalindromeLoci.GetPalindrome(seg)
                          Select seg
                          Distinct).ToArray(
                                Function(segment) New With {
                                    .seg = segment,
                                    .pali = _index.Found(segment, _cutoff, NumPartitions:=_partitions)
                          }, Parallel:=True)

            For Each segment In LQuery
                Dim locis = FindLocation(Me.SequenceData, segment.seg)
                Dim seg As String = segment.seg
                Dim lstSet As ImperfectPalindrome() =
                    LinqAPI.Exec(Of ImperfectPalindrome) <=
                    From loci
                    In segment.pali.AsParallel
                    Let palLeft As Integer = loci.Key.Index
                    Select From left As Integer
                           In locis
                           Let d As Integer = palLeft - left
                           Where d > 0 AndAlso d <= Me._maxDist
                           Select New ImperfectPalindrome With {
                               .Site = segment.seg,
                               .Left = left,
                               .Palindrome = loci.Key.Segment,
                               .Paloci = loci.Key.Index,
                               .Distance = loci.Value.Distance,
                               .Evolr = loci.Value.DistEdits,
                               .Matches = loci.Value.Matches,
                               .Score = loci.Value.Score
                          }

                Call _resultSet.AddRange(lstSet)
            Next

            Call FlushMemory()
        End Sub
    End Class
End Namespace