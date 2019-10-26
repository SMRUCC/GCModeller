#Region "Microsoft.VisualBasic::ca3a9b4ca944ab780380dec300406c56, analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\RepeatsSearchAPI.vb"

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

'     Module RepeatsSearchAPI
' 
'         Function: Density, GenerateRepeats, (+2 Overloads) RepeatsDensity, RevRepeatsDensity, SaveDensity
'                   (+2 Overloads) SaveDocument, SaveRepeatsViews, SaveRevViews, SearchRepeats, SearchReversedRepeats
'                   Trim, (+2 Overloads) WriteSearchResult
' 
'         Sub: BatchSearch, BatchTrim, TrimRepeats, TrimRevRepeats
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Abstract.Motif
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Topologically

    <Package("GCModeller.SeqTools.Repeats.SearchAPI",
                        Category:=APICategories.ResearchTools,
                        Description:="Repeats sites search tools.",
                        Publisher:="xie.guigang@gcmodeller.org")>
    Public Module RepeatsSearchAPI

        <ExportAPI("Write.Csv.RepeatsLoci")>
        Public Function WriteSearchResult(data As IEnumerable(Of Repeats), SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        <ExportAPI("Write.Csv.RepeatsLoci")>
        Public Function WriteSearchResult(data As IEnumerable(Of RevRepeats), SaveTo As String) As Boolean
            Return data.SaveTo(SaveTo, False)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="Min">The minimum length of the repeat sequence loci.</param>
        ''' <param name="Max">The maximum length of the repeat sequence loci.</param>
        ''' <param name="minOccurs">最少的重复出现次数为2，也可以将这个值设置得更高一些</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Invoke.Search")>
        Public Function SearchRepeats(seq As IPolymerSequenceModel, min%, max%, Optional minOccurs% = 2) As Repeats()
            Dim search As New RepeatsSearcher(seq, min, max, minOccurs)

            Call search.DoSearch()
            Call search.CountStatics _
                .AsMatrix _
                .Select(AddressOf ToArray(Of String)) _
                .PrintTable

            Return search.ResultSet.ToArray
        End Function

        <ExportAPI("Save")>
        Public Function SaveDocument(data As IEnumerable(Of Repeats), <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Return Repeats.CreateDocument(data).SaveTo(SaveTo)
        End Function

        <ExportAPI("Save")>
        Public Function SaveDocument(data As IEnumerable(Of RevRepeats), <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Return RevRepeats.CreateDocument(RevData:=data).SaveTo(SaveTo)
        End Function

        <ExportAPI("Save.Rev.Views")>
        Public Function SaveRevViews(data As IEnumerable(Of RevRepeats),
                                     <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Dim view = RevRepeatsView.TrimView(data)
            Return view.SaveTo(SaveTo)
        End Function

        <ExportAPI("Save.Rep.Views")>
        Public Function SaveRepeatsViews(data As IEnumerable(Of Repeats), <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Dim locis = Repeats.CreateDocument(data)
            Dim views = RepeatsView.TrimView(locis)
            Return views.SaveTo(SaveTo)
        End Function

        ''' <summary>
        ''' 请注意，这个函数是应用于生成最长的重复序列片段的方法，假若当前的序列片段在序列上已经没有重复出现了，
        ''' 则上一次迭代的序列可能为重复，所以<paramref name="Segment"></paramref>的长度减1的序列片段是会
        ''' 有重复的，故而会在函数之中将该目的片段缩短
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="Segment"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CreateRepeatLocis(seq$, segment$, minOccurs%, Optional reversed As Boolean = False) As Repeats
            Dim locis%()

            segment = Mid(segment, 1, Len(segment) - 1)
            locis = IScanner.FindLocation(seq, segment).ToArray

            If locis.Length < minOccurs Then
                If reversed Then
                    GoTo returns
                Else
                    Return Nothing
                End If
            End If

returns:    Return New Repeats With {
                .SequenceData = segment,
                .locations = locis
            }
        End Function

        <ExportAPI("Invoke.Search.Reversed")>
        Public Function SearchReversedRepeats(SequenceData As IPolymerSequenceModel,
                                              Min As Integer,
                                              Max As Integer,
                                              Optional MinAppeared As Integer = 2) As RevRepeats()
            Dim revSearchs As New ReversedRepeatSeacher(SequenceData, Min, Max, MinAppeared)
            Call revSearchs.DoSearch()
            Return revSearchs.ResultSet.ToArray
        End Function

        ''' <summary>
        ''' 通过计算每一个基因组上面的每一个位点的重复片段的出现频率来计算出每一个位点的重复片段的热度
        ''' </summary>
        ''' <returns></returns>
        ''' <param name="Mla">必须是经过多序列比对对齐的</param>
        ''' 
        <ExportAPI("Repeats.Density")>
        Public Function RepeatsDensity(Mla As FastaFile,
                                       Min As Integer,
                                       Max As Integer,
                                       Optional MinAppeared As Integer = 2) As KeyValuePair(Of Double(), Double())
            Dim LQuery = (From genome As FastaSeq
                          In Mla
                          Select repeats = RepeatsSearchAPI.SearchRepeats(genome, Min, Max, MinAppeared),
                              rev = RepeatsSearchAPI.SearchReversedRepeats(genome, Min, Max, MinAppeared)).ToArray
            Dim Vecotrs = (From genome In LQuery
                           Let repeatsViews = RepeatsView.TrimView(Repeats.CreateDocument(genome.repeats)),
                               revViews = RevRepeatsView.TrimView(genome.rev)
                           Select repeats = RepeatsView.ToVector(repeatsViews, Mla.First.Length),
                               revRepeats = RepeatsView.ToVector(revViews, Mla.First.Length)).ToArray

            ' 有重复序列的个数的百分比 * 热度的平均值
            Dim p_vectors As Double() =
                Mla.First.Length.Sequence.Select(Function(index As Integer) As Double
                                                     Dim site = Vecotrs.Select(Function(genome) genome.repeats(index)).ToArray
                                                     Dim hashRepeats = (From g In site Where g <> 0R Select g).ToArray
                                                     Dim pHas = hashRepeats.Length / site.Length
                                                     Dim hotAvg = hashRepeats.Average
                                                     Return pHas * hotAvg
                                                 End Function).ToArray
            ' 有重复序列的个数的百分比 * 热度的平均值
            Dim p_rev_vectors As Double() =
                Mla.First.Length.Sequence.Select(Function(index As Integer) As Double
                                                     Dim site = Vecotrs.Select(Function(genome) genome.revRepeats(index)).ToArray
                                                     Dim hashRepeats = (From g In site Where g <> 0R Select g).ToArray
                                                     Dim pHas = hashRepeats.Length / site.Length
                                                     Dim hotAvg = hashRepeats.Average
                                                     Return pHas * hotAvg
                                                 End Function).ToArray
            Return New KeyValuePair(Of Double(), Double())(p_vectors, p_rev_vectors)
        End Function

        <ExportAPI("Repeats.Density")>
        Public Function RepeatsDensity(DIR As String, size As Integer, ref As String, Optional cutoff As Double = 0R) As Double()
            Return Density(Of RepeatsView)(DIR, size, ref, cutoff)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <typeparam name="TView"></typeparam>
        ''' <param name="DIR"></param>
        ''' <param name="size"></param>
        ''' <param name="ref">作为参考的原始数据之中的csv文件名</param>
        ''' <param name="cutoff"></param>
        ''' <returns></returns>
        Public Function Density(Of TView As RepeatsView)(DIR As String, size As Integer, ref As String, cutoff As Double) As Double()
            Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv") _
                .Select(Function(file) New With {
                    .ID = BaseName(file),
                    .context = file.LoadCsv(Of TView)}).ToArray

            VBDebugger.Mute = True

            Dim Vecotrs = (From genome
                           In files.AsParallel
                           Select genome.ID,
                               vector = RepeatsView.ToVector(genome.context, size)) _
                              .ToDictionary(Function(genome) genome.ID,
                                            Function(genome) genome.vector)
            Dim refGenome As Double() = Vecotrs.TryGetValue(ref.NormalizePathString(True))

            VBDebugger.Mute = False

            If refGenome Is Nothing Then
                Call $"Reference `{ref}` is not exists in the dataset, using the first sequence as default!".__DEBUG_ECHO
                refGenome = Vecotrs.First.Value
            End If

            Call New String("="c, 120).__DEBUG_ECHO
            Call $"cutoff={cutoff}".__DEBUG_ECHO
            Call $"genomes={Vecotrs.Count}".__DEBUG_ECHO

            Dim p_vectors As Double() = size _
                .Sequence _
                .Select(Function(index As Integer) As Double
                            Dim refV As Double = refGenome(index)

                            If refV <= cutoff Then
                                Return 0
                            End If

                            Dim site = Vecotrs.Select(Function(genome) genome.Value(index)).ToArray
                            Dim hashRepeats = (From g As Double In site.AsParallel Where g >= cutoff Select g).ToArray
                            Dim pHas As Double = hashRepeats.Length / site.Length
                            Return pHas
                        End Function) _
                .ToArray

            Return p_vectors
        End Function

        <ExportAPI("rev-Repeats.Density")>
        Public Function RevRepeatsDensity(dir As String, size As Integer, ref As String, Optional cutoff As Double = 0R) As Double()
            Return Density(Of RevRepeatsView)(dir, size, ref, cutoff)
        End Function

        ''' <summary>
        ''' 原来的限定函数不起作用了？？可以使用这个函数进行剪裁，请注意剪裁是不可逆的，使用这个函数处理数据只能够收缩数据
        ''' </summary>
        ''' <typeparam name="TRepeats"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <param name="minappear"></param>
        ''' <returns></returns>
        <Extension> Public Function Trim(Of TRepeats As RepeatsView)(data As IEnumerable(Of TRepeats),
                                                                     min As Integer,
                                                                     max As Integer,
                                                                     minappear As Integer) As TRepeats()
            Dim LQuery = (From site As TRepeats
                          In data.AsParallel
                          Let lociLen As Integer = Len(site.SequenceData)
                          Where lociLen >= min AndAlso
                              lociLen <= max AndAlso
                              site.RepeatsNumber >= minappear
                          Select site).ToArray
            Return LQuery
        End Function

        Public Sub BatchTrim(Of TRepeats As RepeatsView)(DIR As String,
                                                         min As Integer,
                                                         max As Integer,
                                                         minapper As Integer,
                                                         ExportDir As String)
            For Each file As String In FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchTopLevelOnly, "*.csv")
                Dim data = file.LoadCsv(Of TRepeats)
                Dim Trimmed = data.Trim(min, max, minapper)
                Dim path As String = ExportDir & "/" & FileIO.FileSystem.GetFileInfo(file).Name
                Call Trimmed.SaveTo(path)
            Next
        End Sub

        <ExportAPI("Repeats.Trim")>
        Public Sub TrimRepeats(Dir As String, min As Integer, max As Integer, minAppear As Integer, Optional ExportDir As String = "./Trim/")
            Call BatchTrim(Of RepeatsView)(Dir, min, max, minAppear, ExportDir)
        End Sub

        <ExportAPI("rev-Repeats.Trim")>
        Public Sub TrimRevRepeats(Dir As String, min As Integer, max As Integer, minAppear As Integer, Optional ExportDir As String = "./Trim/")
            Call BatchTrim(Of RevRepeatsView)(Dir, min, max, minAppear, ExportDir)
        End Sub

        ''' <summary>
        ''' Batch search for the repeats and reversed repeats sequence feature sites. ``<see cref="RepeatsView"/>`` and ``<see cref="RevRepeatsView"/>``
        ''' </summary>
        ''' <param name="Mla"></param>
        ''' <param name="Min"></param>
        ''' <param name="Max"></param>
        ''' <param name="MinAppeared"></param>
        ''' <param name="saveDIR"></param>
        <ExportAPI("Search.Batch")>
        Public Sub BatchSearch(Mla As FastaFile, Min As Integer, Max As Integer, Optional MinAppeared As Integer = 2, Optional saveDIR As String = "./")
            For Each genome As FastaSeq In Mla
                Call genome.__DEBUG_ECHO

                Dim repeats = RepeatsSearchAPI.SearchRepeats(genome, Min, Max, MinAppeared)
                Dim rev = RepeatsSearchAPI.SearchReversedRepeats(genome, Min, Max, MinAppeared)

                Dim repeatsViews = RepeatsView.TrimView(Topologically.Repeats.CreateDocument(repeats)).Trim(Min, Max, MinAppeared)
                Dim revViews = RevRepeatsView.TrimView(rev).Trim(Min, Max, MinAppeared)

                Call repeatsViews.SaveTo(saveDIR & $"/views.repeats/{genome.Title.NormalizePathString(True)}.csv")
                Call revViews.SaveTo(saveDIR & $"/views.rev-repeats/{genome.Title.NormalizePathString(True)}.csv")
            Next
        End Sub

        <ExportAPI("Write.Density")>
        Public Function SaveDensity(data As KeyValuePair(Of Double(), Double()), SaveDIR As String) As Boolean
            Call IO.File.WriteAllLines(SaveDIR & "/repeats.density.txt", data.Key.Select(Function(d) CStr(d)).ToArray)
            Call IO.File.WriteAllLines(SaveDIR & "/rev-repeats.density.txt", data.Value.Select(Function(d) CStr(d)).ToArray)
            Return True
        End Function
    End Module
End Namespace
