Imports LANS.SystemsBiology.SequenceModel
Imports LANS.SystemsBiology.SequenceModel.NucleotideModels
Imports LANS.SystemsBiology.AnalysisTools.SequenceTools.SequencePatterns.Pattern
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Similarity
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports System.Runtime.CompilerServices

Namespace Topologically.SimilarityMatches

    ''' <summary>
    ''' 模糊匹配重复的序列片段
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <[PackageNamespace]("Seqtools.Repeats.Search", Publisher:="xie.guigang@gmail.com", Url:="http://gcmodeller.org")>
    Public Module Repeats

        ''' <summary>
        ''' 模糊匹配相似的位点在目标序列之上的位置
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="loci"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Search.Loci.Repeats")>
        Public Function MatchLociLocations(Sequence As String,
                                           loci As String,
                                           Min As Integer,
                                           Max As Integer,
                                           Optional cutoff As Double = 0.65) As LociMatchedResult()

            Dim Chars As Char() =
                LinqAPI.Exec(Of Char) <= From c As Char
                                         In Sequence.ToUpper.ShadowCopy(Sequence)
                                         Select c
                                         Distinct
            Dim initSeeds = (From obj In __generateSeeds(Chars, loci, cutoff, Min, Max)
                             Select obj
                             Group By obj.Name Into Group)
            Dim SeedsData = (From seed In initSeeds
                             Select seed.Name,
                                 seed.Group.First.x) _
                                   .ToDictionary(Function(obj) obj.Name,
                                                 Function(obj) obj.x)
            Dim Seeds = (From obj In SeedsData Select obj.Key).ToArray
            Dim Repeats As LociMatchedResult() =
                LinqAPI.Exec(Of LociMatchedResult) <= From lociSegment As LociMatchedResult
                                                      In __matchLociLocation(Sequence, Seeds)
                                                      Let Score As Double = SeedsData(lociSegment.Matched)
                                                      Select lociSegment _
                                                          .InvokeSet(NameOf(lociSegment.Similarity), Score) _
                                                          .InvokeSet(NameOf(lociSegment.Loci), loci)
            Return Repeats
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Sequence"></param>
        ''' <param name="seeds">为了加快计算效率，事先所生成的种子缓存</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __matchLociLocation(Sequence As String, seeds As String()) As LociMatchedResult()
            Dim LQuery As LociMatchedResult() =
                LinqAPI.Exec(Of LociMatchedResult) <= From s As String
                                                      In seeds
                                                      Let Location = FindLocation(Sequence, s)
                                                      Select New LociMatchedResult With {
                                                          .Matched = s,
                                                          .Location = Location
                                                      }
            Return LQuery
        End Function

        <Extension>
        Private Function __generateSeeds(Chars As Char(),
                                         Loci As String,
                                         Cutoff As Double,
                                         Min As Integer,
                                         Max As Integer) As NamedValue(Of Double)()
            If Min < 6 Then
                Cutoff = 0.3
            End If

            Dim seeds As List(Of String) = Topologically.InitializeSeeds(Chars, Min)
            Dim source = (From s As String In seeds
                          Let Score As Double = New StringSimilarityMatchs(s, Loci).Score
                          Where Score >= Cutoff
                          Select s,
                              Score).ToList '生成初始长度的种子
            Dim buf As List(Of String)
            'Seeds = (From obj In SeedsCollection Select obj.s).ToList

            For i As Integer = Min + 1 To Max   '种子延伸至长度的上限
                buf = Topologically.ExtendSequence(seeds, Chars)
                Dim tmp = (From s As String
                           In buf.AsParallel
                           Let Score As Double = New StringSimilarityMatchs(Loci, s).Score
                           Where Score >= Cutoff
                           Select s,
                               Score).ToArray

                Call Console.Write(".")

                seeds += tmp.Select(Function(x) x.s)
                source += tmp
            Next

            Call $"Seeds generation thread for   {Loci}    job done!".__DEBUG_ECHO

            Return LinqAPI.Exec(Of NamedValue(Of Double)) <=
                From x
                In source.AsParallel
                Select New NamedValue(Of Double)(x.s, x.Score)
        End Function

        ''' <summary>
        ''' 生成和<paramref name="loci"></paramref>满足相似度匹配关系的序列的集合
        ''' </summary>
        ''' <param name="Chars"></param>
        ''' <param name="loci"></param>
        ''' <param name="Cutoff"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __generateSeeds(Chars As Char(), loci As String, Cutoff As Double) As NamedValue(Of Double)()
            Dim Min As Integer = Len(loci),
                Max As Integer = Len(loci) * 2
            Return __generateSeeds(Chars, loci, Cutoff, Min, Max)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SequenceData"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        ''' <remarks>为了加快计算，首先生成种子，然后再对种子进行模糊匹配</remarks>
        ''' 
        <ExportAPI("invoke.search.similarity")>
        Public Function InvokeSearch(SequenceData As String, Min As Integer, Max As Integer, Optional cutoff As Double = 0.65) As LociMatchedResult()
            SequenceData = SequenceData.ToUpper

            Call "Start to generate seeds....".__DEBUG_ECHO

            Dim Seeds As String() =
                LinqAPI.Exec(Of String) <= From rp As Topologically.Repeats
                                           In SearchRepeats(New SegmentObject(SequenceData, 1), Min, Max)
                                           Select seq = rp.SequenceData
                                           Distinct  '生成搜索所需要的种子

            Call $"Generate repeats search seeds, job done! {Seeds.Length} repeats sequence was export for seeds!".__DEBUG_ECHO

            Dim Chars As Char() = (From c As Char In SequenceData Select c Distinct).ToArray

            Call "Scanning the whole sequence for each repeats loci.....".__DEBUG_ECHO

            Dim Repeats = (From Loci As String
                           In Seeds
                           Let InternalSeeds = (From obj As NamedValue(Of Double)
                                                In __generateSeeds(Chars, Loci, cutoff, Min, Max:=Len(Loci) * 1.5)
                                                Select obj
                                                Group By obj.Name Into Group) _
                                                    .ToDictionary(Function(obj) obj.Name,
                                                                  Function(obj) obj.Group.First.x)
                           Let InternalSeedsSegment As String() = (From obj In InternalSeeds Select obj.Key).ToArray
                           Select InternalSeeds,
                               Loci,
                               repeatsCollection = __matchLociLocation(SequenceData, InternalSeedsSegment)).ToArray '遍历种子，进行全序列扫描

            Call $"{Repeats.Length} repeats loci!".__DEBUG_ECHO

            Dim LQuery As LociMatchedResult() =
                LinqAPI.Exec(Of LociMatchedResult) <= From Group
                                                      In Repeats.AsParallel
                                                      Let data = (From loci As LociMatchedResult
                                                                  In Group.repeatsCollection.AsParallel
                                                                  Let Score As Double = Group.InternalSeeds(loci.Matched)
                                                                  Select loci.InvokeSet(NameOf(loci.Similarity), Score) _
                                                                             .InvokeSet(NameOf(loci.Loci), Group.Loci)).ToArray
                                                      Select data

            Call $"Finally generate {LQuery.Length} repeats loci data!".__DEBUG_ECHO

            Return LQuery
        End Function

        <ExportAPI("invoke.search.similarity")>
        Public Function InvokeSearch(Sequence As I_PolymerSequenceModel, Min As Integer, Max As Integer, Optional cutoff As Double = 0.65) As LociMatchedResult()
            Return InvokeSearch(Sequence.SequenceData, Min, Max, cutoff)
        End Function

        <ExportAPI("write.csv.repeats_result")>
        Public Function SaveRepeatsResult(result As IEnumerable(Of LociMatchedResult), saveto As String) As Boolean
            Return result.SaveTo(saveto, False)
        End Function

        <ExportAPI("write.csv.repeats_result")>
        Public Function SaveRepeatsResult(result As IEnumerable(Of ReversedLociMatchedResult), saveto As String) As Boolean
            Return result.SaveTo(saveto, False)
        End Function

        <ExportAPI("invoke.search.similarity.reversed")>
        Public Function InvokeSearchReversed(Sequence As String, Min As Integer, Max As Integer, Optional cutoff As Double = 0.65) As ReversedLociMatchedResult()
            Dim Seeds = (From rp As RevRepeats
                         In RepeatsSearchAPI.SearchReversedRepeats(New SegmentObject(Sequence.ToUpper.ShadowCopy(Sequence), 1), Min, Max)
                         Select rp.RevSegment
                         Distinct).ToArray  '生成搜索所需要的反向重复序列的种子
            Dim Chars As Char() = (From c As Char
                                   In Sequence
                                   Select c
                                   Distinct).ToArray

            'Seeds是具有重复的反向序列
            Dim Repeats = (From Loci As String
                           In Seeds.AsParallel
                           Let InternalSeeds = (From obj In __generateSeeds(Chars, Loci, cutoff)
                                                Select obj
                                                Group By obj.Name Into Group) _
                                                      .ToDictionary(Function(obj) obj.Name,
                                                                    Function(obj) obj.Group.First.x)
                           Let InternalSeedsSegment As String() = (From obj In InternalSeeds Select obj.Key).ToArray
                           Select InternalSeeds,
                               Loci,
                               repeatsCollection = __matchLociLocation(Sequence, InternalSeedsSegment)).ToArray '遍历种子，进行全序列扫描

            '反向重复的
            Dim LQuery As ReversedLociMatchedResult() =
                LinqAPI.Exec(Of ReversedLociMatchedResult) <=
                    From Group
                    In Repeats.AsParallel
                    Select From Loci As LociMatchedResult
                           In Group.repeatsCollection.AsParallel
                           Let Score As Double = Group.InternalSeeds(Loci.Matched)
                           Let LociResult =
                               Loci.InvokeSet(NameOf(Loci.Similarity), Score) _
                                   .InvokeSet(NameOf(Loci.Loci), Group.Loci)
                           Select ReversedLociMatchedResult.GenerateFromBase(LociResult)
            Return LQuery
        End Function
    End Module
End Namespace