#Region "Microsoft.VisualBasic::3d65178f89e4e7f817bbc9e0e3c10332, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Similarity\Repeats.vb"

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

    '   Total Lines: 261
    '    Code Lines: 181
    ' Comment Lines: 47
    '   Blank Lines: 33
    '     File Size: 13.12 KB


    '     Module Repeats
    ' 
    '         Function: (+2 Overloads) doGenerateSeeds, doMatchLociLocations, (+2 Overloads) InvokeSearch, InvokeSearchReversed, MatchLociLocations
    '                   (+2 Overloads) SaveRepeatsResult
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Similarity
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Topologically.SimilarityMatches

    ''' <summary>
    ''' 模糊匹配重复的序列片段
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("Seqtools.Repeats.Search", Publisher:="xie.guigang@gmail.com", Url:="http://gcmodeller.org")>
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

            Sequence = Sequence.ToUpper

            Dim Chars As Char() =
                LinqAPI.Exec(Of Char) <= From c As Char
                                         In Sequence
                                         Select c
                                         Distinct
            Dim initSeeds = (From obj In doGenerateSeeds(Chars, loci, cutoff, Min, Max)
                             Select obj
                             Group By obj.Name Into Group)
            Dim SeedsData = (From seed In initSeeds
                             Select seed.Name,
                                 seed.Group.First.Value) _
                                   .ToDictionary(Function(obj) obj.Name,
                                                 Function(obj) obj.Value)
            Dim Seeds = (From obj In SeedsData Select obj.Key).ToArray
            Dim setValue As New SetValue(Of LociMatchedResult)
            Dim Repeats As LociMatchedResult() =
                LinqAPI.Exec(Of LociMatchedResult) <= From lociSegment As LociMatchedResult
                                                      In Sequence.doMatchLociLocations(Seeds)
                                                      Let Score As Double = SeedsData(lociSegment.Matched)
                                                      Select setValue _
                                                          .InvokeSetValue(lociSegment, NameOf(lociSegment.Similarity), Score) _
                                                          .InvokeSet(NameOf(lociSegment.Loci), loci).obj
            Return Repeats
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sequence"></param>
        ''' <param name="seeds">为了加快计算效率，事先所生成的种子缓存</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Private Function doMatchLociLocations(sequence$, seeds$()) As IEnumerable(Of LociMatchedResult)
            Return From s As String
                   In seeds
                   Let Location = IScanner.FindLocation(sequence, s).ToArray
                   Select New LociMatchedResult With {
                       .Matched = s,
                       .Location = Location
                   }
        End Function

        <Extension>
        Private Function doGenerateSeeds(chars As Char(), loci$, cutoff#, min%, max%) As NamedValue(Of Double)()
            Dim seeds As List(Of String) = Seeding.InitializeSeeds(chars, min).AsList

            If min < 6 Then
                cutoff = 0.3
            End If

            ' 生成初始长度的种子
            Dim source = (From s As String
                          In seeds
                          Let score As Double = LevenshteinEvaluate(s, loci)
                          Where score >= cutoff
                          Select s, score).AsList
            Dim buf As List(Of String)
            'Seeds = (From obj In SeedsCollection Select obj.s).AsList

            ' 种子延伸至长度的上限
            For i As Integer = min + 1 To max
                buf = Seeding.ExtendSequence(seeds, chars).AsList
                Dim tmp = (From s As String
                           In buf.AsParallel
                           Let Score As Double = LevenshteinEvaluate(loci, s)
                           Where Score >= cutoff
                           Select s,
                               Score).ToArray

                Call Console.Write(".")

                seeds += tmp.Select(Function(x) x.s)
                source += tmp
            Next

            Call $"Seeds generation thread for   {loci}    job done!".__DEBUG_ECHO

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
        Private Function doGenerateSeeds(Chars As Char(), loci As String, Cutoff As Double) As NamedValue(Of Double)()
            Dim Min As Integer = Len(loci),
                Max As Integer = Len(loci) * 2
            Return doGenerateSeeds(Chars, loci, Cutoff, Min, Max)
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
                                           Select seq = rp.loci
                                           Distinct  '生成搜索所需要的种子

            Call $"Generate repeats search seeds, job done! {Seeds.Length} repeats sequence was export for seeds!".__DEBUG_ECHO

            Dim Chars As Char() = (From c As Char In SequenceData Select c Distinct).ToArray

            Call "Scanning the whole sequence for each repeats loci.....".__DEBUG_ECHO

            Dim Repeats = (From Loci As String
                           In Seeds
                           Let InternalSeeds = (From obj As NamedValue(Of Double)
                                                In doGenerateSeeds(Chars, Loci, cutoff, Min, max:=Len(Loci) * 1.5)
                                                Select obj
                                                Group By obj.Name Into Group) _
                                                    .ToDictionary(Function(obj) obj.Name,
                                                                  Function(obj) obj.Group.First.Value)
                           Let InternalSeedsSegment As String() = (From obj In InternalSeeds Select obj.Key).ToArray
                           Select InternalSeeds,
                               Loci,
                               repeatsCollection = doMatchLociLocations(SequenceData, InternalSeedsSegment)).ToArray '遍历种子，进行全序列扫描

            Call $"{Repeats.Length} repeats loci!".__DEBUG_ECHO

            Dim setValue As New SetValue(Of LociMatchedResult)
            Dim LQuery As LociMatchedResult() =
                LinqAPI.Exec(Of LociMatchedResult) <= From Group
                                                      In Repeats.AsParallel
                                                      Let data = (From loci As LociMatchedResult
                                                                  In Group.repeatsCollection.AsParallel
                                                                  Let Score As Double =
                                                                      Group.InternalSeeds(loci.Matched)
                                                                  Select setValue _
                                                                      .InvokeSetValue(loci, NameOf(loci.Similarity), Score) _
                                                                      .InvokeSet(NameOf(loci.Loci), Group.Loci).obj).ToArray
                                                      Select data

            Call $"Finally generate {LQuery.Length} repeats loci data!".__DEBUG_ECHO

            Return LQuery
        End Function

        <ExportAPI("invoke.search.similarity")>
        Public Function InvokeSearch(Sequence As IPolymerSequenceModel, Min As Integer, Max As Integer, Optional cutoff As Double = 0.65) As LociMatchedResult()
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
            Sequence = Sequence.ToUpper

            Dim Seeds = (From rp As ReverseRepeats
                         In RepeatsSearchAPI.SearchReversedRepeats(New SegmentObject(Sequence, 1), Min, Max)
                         Select rp.RevSegment
                         Distinct).ToArray  '生成搜索所需要的反向重复序列的种子
            Dim Chars As Char() = (From c As Char
                                   In Sequence
                                   Select c
                                   Distinct).ToArray

            'Seeds是具有重复的反向序列
            Dim Repeats = (From Loci As String
                           In Seeds.AsParallel
                           Let InternalSeeds = (From obj In doGenerateSeeds(Chars, Loci, cutoff)
                                                Select obj
                                                Group By obj.Name Into Group) _
                                                      .ToDictionary(Function(obj) obj.Name,
                                                                    Function(obj) obj.Group.First.Value)
                           Let InternalSeedsSegment As String() = (From obj In InternalSeeds Select obj.Key).ToArray
                           Select InternalSeeds,
                               Loci,
                               repeatsCollection = doMatchLociLocations(Sequence, InternalSeedsSegment)).ToArray '遍历种子，进行全序列扫描

            ' 反向重复的
            Dim setValue As New SetValue(Of LociMatchedResult)
            Dim LQuery As ReversedLociMatchedResult() =
                LinqAPI.Exec(Of ReversedLociMatchedResult) <=
                    From Group
                    In Repeats.AsParallel
                    Select From Loci As LociMatchedResult
                           In Group.repeatsCollection.AsParallel
                           Let Score As Double = Group.InternalSeeds(Loci.Matched)
                           Let LociResult =
                               setValue _
                                   .InvokeSetValue(Loci, NameOf(Loci.Similarity), Score) _
                                   .InvokeSet(NameOf(Loci.Loci), Group.Loci).obj
                           Select ReversedLociMatchedResult.GenerateFromBase(LociResult)
            Return LQuery
        End Function
    End Module
End Namespace
