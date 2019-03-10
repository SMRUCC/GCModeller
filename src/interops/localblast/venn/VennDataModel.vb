﻿#Region "Microsoft.VisualBasic::aa6ac6e1daa6b0e299e7a8f2a4b32a2f, venn\VennDataModel.vb"

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

    '     Module VennDataModel
    ' 
    '         Function: __parserIndex, BeginLoadCdsDumpInfo, Copy, DeltaMove, ExportBidirectionalBesthit
    '                   LoadCdsDumpInfo, NullHash, OutputConservedCluster, ReadBesthitXML, ReadXml
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Parallel.Threads
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Analysis
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.VennDataBuilder
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports PathEntry = System.Collections.Generic.KeyValuePair(Of String, String)

Namespace BlastAPI

    ''' <summary>
    ''' Generates the Venn diagram data model using the bbh orthology method.(模块之中的方法可以应用于使用直系同源来创建文氏图)
    ''' </summary>
    ''' <remarks>
    ''' 生成Venn表格所需要的步骤：
    ''' 1. 按照基因组进行导出序列数据
    ''' 2. 两两组合式的双向比对
    ''' 3.
    ''' </remarks>
    <Package("VennDiagram.LDM.BBH",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gcmodeller.org",
                        Description:="Package for generate the data model for creates the Venn diagram using the completely combination blastp based bbh protein homologous method result.
                        BBH based method is the widely used algorithm for the cell pathway system automatically annotation, such as the kaas system in the KEGG database.")>
    Public Module VennDataModel

        ''' <summary>
        ''' 批量导出最佳比对匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="Source">单项最佳的两两比对的结果数据文件夹</param>
        ''' <param name="EXPORT">双向最佳的导出文件夹</param>
        ''' <param name="CDSAll">从GBK文件列表之中所导出来的蛋白质信息的汇总表</param>
        '''
        <ExportAPI("Export.Besthits")>
        Public Function ExportBidirectionalBesthit(Source As IEnumerable(Of AlignEntry),
                                                   <Parameter("Path.CDS.All.Dump")> CDSAll As String,
                                                   <Parameter("DIR.EXPORT")> EXPORT As String,
                                                   <Parameter("Null.Trim")> Optional TrimNull As Boolean = False) As BestHit()
            Return SMRUCC.genomics.Interops.NCBI.Extensions.Analysis.ExportBidirectionalBesthit(Source, EXPORT, LoadCdsDumpInfo(CDSAll), TrimNull)
        End Function

        <ExportAPI("Orf.Dump.Load.As.Hash")>
        Public Function LoadCdsDumpInfo(Path As String) As Dictionary(Of String, GeneDumpInfo)
            Dim dumpInfo As IEnumerable(Of GeneDumpInfo) = Path.LoadCsv(Of GeneDumpInfo)(False)
            Dim GroupData = (From ORF As GeneDumpInfo
                             In dumpInfo.AsParallel
                             Select ORF
                             Group By ORF.LocusID Into Group)
            Dim hash = GroupData.ToDictionary(Function(x) x.LocusID,
                                              Function(x) x.Group.First)
            Return hash
        End Function

        <ExportAPI("Orf.Dump.Begin.Load.As.Hash")>
        Public Function BeginLoadCdsDumpInfo(Path As String) As Task(Of String, Dictionary(Of String, GeneDumpInfo))
            Return New Task(Of String, Dictionary(Of String, GeneDumpInfo))(Path, AddressOf LoadCdsDumpInfo).Start
        End Function

        ''' <summary>
        ''' If you don't want the export bbh data contains the protein description information or just don't know how the create the information, using this function to leave it blank.
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("Orf.Hash.Null", Info:="If you don't want the export bbh data contains the protein description information or just don't know how the create the information, using this function to leave it blank.")>
        Public Function NullHash() As Dictionary(Of String, GeneDumpInfo)
            Return New Dictionary(Of String, GeneDumpInfo)
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="mainIndex">
        ''' 进化比较的标尺
        ''' 假若为空字符串或者数字0以及first，都表示使用集合之中的第一个元素对象作为标尺
        ''' 假若参数值为某一个菌株的名称<see cref="BestHit.sp"></see>，则会以该菌株的数据作为比对数据
        ''' 假若为last，则使用集合之中的最后一个
        ''' 对于其他的处于0-集合元素上限的数字，可以认识使用该集合之中的第i-1个元素对象
        ''' 还可以选择longest或者shortest参数值来作为最长或者最短的元素作为主标尺
        ''' 对于其他的任何无效的字符串，则默认使用第一个
        '''
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Generate.Venn.LDM",
                   Info:="The trim_null parameter is TRUE, and the function will filtering all of the data which have more than one hits.")>
        Public Function DeltaMove(data As IEnumerable(Of BestHit),
                                  <Parameter("Index.Main",
                                             "The file name without the extension name of the target query fasta data.")> Optional mainIndex As String = "",
                                  <Parameter("Null.Trim")> Optional TrimNull As Boolean = False) As IO.File

            Dim dataHash As Dictionary(Of String, BestHit) = data.ToDictionary(Function(item) item.sp)
            Dim IndexKey As String = dataHash.Keys(__parserIndex(dataHash, mainIndex))
            Dim indexQuery As BestHit = dataHash(IndexKey)

            Call dataHash.Remove(IndexKey)

            If indexQuery.hits.IsNullOrEmpty Then
                Call $"The profile data of your key ""{mainIndex}"" ---> ""{indexQuery.sp}"" is null!".__DEBUG_ECHO
                Call "Thread exists...".__DEBUG_ECHO
                Return New IO.File
            End If

            Dim dataframe As IO.File = indexQuery.ExportCsv(TrimNull)
            Dim sps As String() = indexQuery.hits.First.Hits.Select(Function(x) x.tag).ToArray

            For deltaIndex As Integer = 0 To dataHash.Count - 1
                Dim subMain As BestHit = dataHash.Values(deltaIndex)

                If subMain.hits.IsNullOrEmpty Then
                    Call $"Profile data {subMain.sp} is null!".__DEBUG_ECHO
                    Continue For
                End If

                Dim di As Integer = deltaIndex
                Dim subHits As String() =
                    LinqAPI.Exec(Of String) <= From row As IO.RowObject
                                               In dataframe
                                               Let d As Integer = 2 + 4 * di + 1
                                               Let id As String = row(d)
                                               Where Not String.IsNullOrEmpty(id)
                                               Select id

                For Each subMainNotHit In From hits As HitCollection
                                          In subMain.hits
                                          Where Array.IndexOf(subHits, hits.QueryName) = -1
                                          Select hits.QueryName,
                                              hits.Description,
                                              speciesProfile = hits.Hits.ToDictionary(Function(hit) hit.tag)  '竖直方向遍历第n列的基因号

                    Dim row As IO.RowObject =
                        New IO.RowObject From {subMainNotHit.Description, subMainNotHit.QueryName} +
                            From nnn As Integer In (4 * deltaIndex).Sequence Select ""

                    For Each sid As String In sps.Skip(deltaIndex)
                        Dim matched As Hit = subMainNotHit.speciesProfile(sid)
                        Call row.Add("")
                        Call row.Add(matched.HitName)
                        Call row.Add(matched.Identities)
                        Call row.Add(matched.Positive)
                    Next

                    dataframe += row
                Next
            Next

            Return dataframe
        End Function

        <ExportAPI("Venn.Source.Copy")>
        Public Function Copy(besthits As BestHit, source As String, copyTo As String) As String()
            Return besthits.SelectSourceFromHits(source, copyTo)
        End Function

        <ExportAPI("Read.Xml.Besthit")>
        Public Function ReadXml(path As String) As BestHit
            Return path.LoadXml(Of BestHit)()
        End Function

        <ExportAPI("Load.Xmls.Besthit")>
        Public Function ReadBesthitXML(DIR As String) As BestHit()
            Dim files As BestHit() =
                LinqAPI.Exec(Of BestHit) <= From path As String
                                            In (ls - l - wildcards("*.xml") <= DIR).AsParallel
                                            Select path.LoadXml(Of BestHit)()
            Return files
        End Function

        ''' <summary>
        ''' 计算出可能的保守区域
        ''' </summary>
        ''' <param name="bh"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Export.Conserved.GenomeRegion", Info:="Calculate of the conserved genome region based on the multiple genome bbh comparison result.")>
        Public Function OutputConservedCluster(bh As BestHit) As String()()
            Dim regions As IReadOnlyCollection(Of String()) = bh.GetConservedRegions
            Dim i As Integer = 1

            Call Console.WriteLine(New String("=", 200))
            Call Console.WriteLine("Conserved region on ""{0}"":", bh.sp)
            Call Console.WriteLine()

            For Each line As String() In regions
                Call Console.WriteLine(i & "   ----> " & String.Join(", ", line))
                i += 1
            Next

            Return regions.ToArray
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="index">
        ''' 进化比较的标尺
        ''' 假若为空字符串或者数字0以及first，都表示使用集合之中的第一个元素对象作为标尺
        ''' 假若参数值为某一个菌株的名称<see cref="BestHit.sp"></see>，则会以该菌株的数据作为比对数据
        ''' 假若为last，则使用集合之中的最后一个
        ''' 对于其他的处于0-集合元素上限的数字，可以认识使用该集合之中的第i-1个元素对象
        ''' 还可以选择longest或者shortest参数值来作为最长或者最短的元素作为主标尺
        ''' 对于其他的任何无效的字符串，则默认使用第一个
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function __parserIndex(data As Dictionary(Of String, BestHit), index As String) As Integer
            If String.Equals(index, "first", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(index, "0") Then
                Return 0
            ElseIf data.ContainsKey(index) Then
                Return Array.IndexOf(data.Keys.ToArray, index)
            ElseIf String.Equals(index, "last", StringComparison.OrdinalIgnoreCase) Then
                Return data.Count - 1
            ElseIf String.Equals(index, "longest", StringComparison.OrdinalIgnoreCase) Then
                Dim sid As String = (From item In data Select item Order By Len(item.Key) Descending).First.Key
                Return Array.IndexOf(data.Keys.ToArray, sid)
            ElseIf String.Equals(index, "shortest", StringComparison.OrdinalIgnoreCase) Then
                Dim sid As String = (From item In data Select item Order By Len(item.Key) Ascending).First.Key
                Return Array.IndexOf(data.Keys.ToArray, sid)
            End If

            Return 0
        End Function
    End Module
End Namespace
