Imports System.Runtime.CompilerServices
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Topologically
Imports LANS.SystemsBiology.SequenceModel
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Topologically.SimilarityMatches

    ''' <summary>
    ''' 模糊相等的
    ''' </summary>
    Public Module MirrorPalindrome

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="l">目标片段的长度</param>
        ''' <param name="Loci">位点的位置</param>
        ''' <param name="Mirror">进行模糊比较的参考序列</param>
        ''' <param name="Sequence">基因组序列</param>
        ''' <returns></returns>
        <Extension> Private Function __haveMirror(l As Integer,
                                                  Loci As Integer,
                                                  Mirror As String,
                                                  Sequence As String,
                                                  cut As Double,
                                                  maxDist As Integer) As NamedValue(Of Integer)

            Dim mrStart As Integer = Loci + l  ' 左端的起始位置
            Dim ref As Integer() = Mirror.ToArray(AddressOf Asc)

            For i As Integer = 0 To maxDist
                Dim mMirr As String = Mid(Sequence, mrStart, l)
                Dim edits = LevenshteinDistance.ComputeDistance(ref, mMirr)   ' 和参考序列做模糊匹配
                Dim score As Double = If(edits Is Nothing, -1, edits.MatchSimilarity)

                If score >= cut Then
                    Return New NamedValue(Of Integer)(mMirr, mrStart + l)  ' 右端的结束位置
                Else
                    l += 1
                End If
            Next

            Return New NamedValue(Of Integer)(Nothing, -1)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Segment"></param>
        ''' <param name="Sequence"></param>
        ''' <param name="maxDist">两个片段之间的最大的距离</param>
        ''' <param name="cut"></param>
        ''' <returns></returns>
        <ExportAPI("Mirrors.Locis.Get")>
        Public Function CreateMirrors(Segment As String, Sequence As String, maxDist As Integer, Optional cut As Double = 0.6) As PalindromeLoci()
            Dim Locations As Integer() = Pattern.Extensions.FindLocation(Sequence, Segment)

            If Locations.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim Mirror As String = New String(Segment.Reverse.ToArray)  ' 这个是目标片段的镜像回文部分，也是需要进行比较的参考序列
            Dim l As Integer = Len(Segment)
            Dim Result = (From loci As Integer
                          In Locations
                          Let ml As NamedValue(Of Integer) =
                              __haveMirror(l, loci, Mirror, Sequence, cut, maxDist)
                          Where ml.x > -1
                          Select loci,
                              ml).ToArray
            Return Result.ToArray(
                Function(site) New PalindromeLoci With {
                    .Loci = Segment,
                    .Start = site.loci,
                    .PalEnd = site.ml.x,
                    .Palindrome = site.ml.Name,
                    .MirrorSite = Mirror
                })
        End Function
    End Module

    Public Class FuzzyMirrors : Inherits MirrorSearchs

        ReadOnly _maxDist As Integer, cut As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        Sub New(Sequence As I_PolymerSequenceModel,
                <Parameter("Min.Len", "The minimum length of the repeat sequence loci.")> Min As Integer,
                <Parameter("Max.Len", "The maximum length of the repeat sequence loci.")> Max As Integer,
                maxDist As Integer,
                cut As Double)
            Call MyBase.New(Sequence, Min, Max)

            Me.cut = cut
            Me._maxDist = maxDist
        End Sub

        Protected Overrides Sub __postResult(currentRemoves() As String, currentStat As Microsoft.VisualBasic.List(Of String), currLen As Integer)
            Dim Sites As PalindromeLoci() =
                currentStat.ToArray(
                    Function(loci) CreateMirrors(loci,
                                                 SequenceData,
                                                 _maxDist,
                                                 cut),
                                   Parallel:=True).MatrixAsIterator.TrimNull
            Call _ResultSet.Add(Sites)
        End Sub
    End Class
End Namespace