#Region "Microsoft.VisualBasic::b510c6e823d3a50abfa4907912d9fbb0, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Exactly\Repeats\RepeatsSearchAPI.vb"

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

    '   Total Lines: 184
    '    Code Lines: 125
    ' Comment Lines: 35
    '   Blank Lines: 24
    '     File Size: 9.23 KB


    '     Module RepeatsSearchAPI
    ' 
    '         Function: CreateRepeatLocis, (+2 Overloads) SaveDocument, SaveRepeatsViews, SaveRevViews, SearchRepeats
    '                   SearchReversedRepeats, Trim, (+2 Overloads) WriteSearchResult
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
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
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
        Public Function WriteSearchResult(data As IEnumerable(Of ReverseRepeats), SaveTo As String) As Boolean
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
        Public Function SaveDocument(data As IEnumerable(Of ReverseRepeats), <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Return ReverseRepeats.CreateDocument(RevData:=data).SaveTo(SaveTo)
        End Function

        <ExportAPI("Save.Rev.Views")>
        Public Function SaveRevViews(data As IEnumerable(Of ReverseRepeats),
                                     <Parameter("Path.Csv")> SaveTo As String) As Boolean
            Dim view = ReverseRepeatsView.TrimView(data)
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
                .loci = segment,
                .locations = locis
            }
        End Function

        <ExportAPI("Invoke.Search.Reversed")>
        Public Function SearchReversedRepeats(SequenceData As IPolymerSequenceModel,
                                              Min As Integer,
                                              Max As Integer,
                                              Optional MinAppeared As Integer = 2) As ReverseRepeats()
            Dim revSearchs As New ReversedRepeatSeacher(SequenceData, Min, Max, MinAppeared)
            Call revSearchs.DoSearch()
            Return revSearchs.ResultSet.ToArray
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
            Call BatchTrim(Of ReverseRepeatsView)(Dir, min, max, minAppear, ExportDir)
        End Sub

        ''' <summary>
        ''' Batch search for the repeats and reversed repeats sequence feature sites. ``<see cref="RepeatsView"/>`` and ``<see cref="ReverseRepeatsView"/>``
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
                Dim revViews = ReverseRepeatsView.TrimView(rev).Trim(Min, Max, MinAppeared)

                Call repeatsViews.SaveTo(saveDIR & $"/views.repeats/{genome.Title.NormalizePathString(True)}.csv")
                Call revViews.SaveTo(saveDIR & $"/views.rev-repeats/{genome.Title.NormalizePathString(True)}.csv")
            Next
        End Sub
    End Module
End Namespace
