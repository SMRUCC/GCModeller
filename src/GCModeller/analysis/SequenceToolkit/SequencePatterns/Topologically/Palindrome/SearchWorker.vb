Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language

Namespace Topologically

    ''' <summary>
    ''' 片段在反向链找得到自己的反向片段
    ''' </summary>
    Public Class PalindromeSearchs : Inherits MirrorSearchs

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)
            Call MyBase.New(Sequence, Min, Max)
        End Sub

        ''' <summary>
        ''' 片段在反向链找得到自己的反向片段
        ''' </summary>
        ''' <param name="currentRemoves"></param>
        ''' <param name="currentStat"></param>
        ''' <param name="currLen"></param>
        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As List(Of String), currLen As Integer)
            Dim Sites As PalindromeLoci() = currentStat.ToArray(
                Function(loci) Palindrome.CreatePalindrome(
                    loci, SequenceData),
                    Parallel:=True).MatrixAsIterator.TrimNull
            Call _ResultSet.Add(Sites)
        End Sub
    End Class

    ''' <summary>
    ''' 片段在正向链找得到自己的反向片段
    ''' </summary>
    Public Class MirrorSearchs : Inherits SearchWorker

        Public ReadOnly Property ResultSet As IEnumerable(Of PalindromeLoci)
            Get
                Dim LQuery = (From site As PalindromeLoci
                              In _ResultSet
                              Select site
                              Group site By site.Mirror Into Group).ToArray
                Dim rs As PalindromeLoci() =
                    LinqAPI.Exec(Of PalindromeLoci) <=
                        From site
                        In LQuery
                        Select PalindromeLoci.SelectSite(site.Group.ToArray)
                Return rs
            End Get
        End Property

        Protected _ResultSet As New List(Of PalindromeLoci)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer)
            Call MyBase.New(Sequence, Min, Max)
            If Me.Max > Len(Sequence.SequenceData) / 2 Then Me._max = Len(Sequence.SequenceData) / 2  '因为是回文重复，所以到一半长度的时候肯定是没有了的
        End Sub

        ''' <summary>
        ''' 目标片段过后
        ''' </summary>
        ''' <param name="currentRemoves"></param>
        ''' <param name="currentStat"></param>
        ''' <param name="currLen"></param>
        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As List(Of String), currLen As Integer)
            Dim Sites As PalindromeLoci() = currentStat.ToArray(
                Function(loci) _
                    Palindrome.CreateMirrors(
                        loci,
                        SequenceData), Parallel:=True).MatrixAsIterator.TrimNull

            Call _ResultSet.Add(Sites)
        End Sub

        Protected Overrides Sub __beginInit(ByRef seeds As List(Of String))
            seeds = LinqAPI.MakeList(Of String) <= From fragment As String
                                                   In seeds
                                                   Where SequenceData.IndexOf(fragment) > -1
                                                   Select fragment
        End Sub

        Protected Overrides Function __getRemovedList(ByRef currentStat As List(Of String)) As String()
            Return LinqAPI.Exec(Of String) <=
                From Segment As String
                In currentStat.AsParallel
                Where SequenceData.IndexOf(Segment) = -1  ' 只需要判断存不存在就行了，因为是连在一起的，所以现在短片段可能不会有回文，但是长片段却会出现回文，所以不能够在这里移除
                Select Segment
        End Function
    End Class
End Namespace