﻿#Region "Microsoft.VisualBasic::e4c4c4a28d572aac267560e859bdd2ee, LocalBLAST\Analysis\BBHLogs.vb"

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

    '     Module BBHLogs
    ' 
    '         Function: (+3 Overloads) __export, __getDirectionary, __operation, (+2 Overloads) BuildBBHEntry, (+2 Overloads) ExportBidirectionalBesthit
    '                   (+2 Overloads) ExportLogData, ExportLogDataUltraLargeSize, LoadEntries, LoadSBHEntry
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.CsvExports
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports Entry = System.Collections.Generic.KeyValuePair(Of
    SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.AlignEntry,
    SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BatchParallel.AlignEntry)

Namespace Analysis

    <Package("NCBI.LocalBlast.BBH",
                      Publisher:="amethyst.asuka@gcmodeller.org",
                      Category:=APICategories.ResearchTools,
                      Url:="http://gcmodeller.org")>
    Public Module BBHLogs

        ''' <summary>
        ''' 从文件系统之中加载比对的文件的列表
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <param name="ext"></param>
        ''' <returns></returns>
        <ExportAPI("LoadEntries")>
        <Extension>
        Public Function LoadEntries(DIR As String, Optional ext As String = "*.txt") As AlignEntry()
            Dim Logs As AlignEntry() = (ls - l - wildcards(ext) <= DIR).Select(AddressOf LogNameParser).ToArray
            Return Logs
        End Function

        <ExportAPI("BBH_Entry.Build")>
        <Extension>
        Public Function BuildBBHEntry(source As List(Of AlignEntry)) As Entry()
            Dim lstPairs As New List(Of Entry)

            Do While source.Count > 0
                Dim First = source.First
                Call source.RemoveAt(Scan0)

                Dim paired As AlignEntry =
                    LinqAPI.DefaultFirst(Of AlignEntry) <= From entry As AlignEntry
                                                           In source
                                                           Where entry.BiDirEquals(First)
                                                           Select entry
                If Not paired Is Nothing Then
                    Call source.Remove(paired)
                    Call lstPairs.Add(New Entry(First, paired))
                    Call Console.Write(".")
                End If
            Loop

            If lstPairs.Count = 0 Then
                Call $"null bbh paires was found, please check you file name rule is in format like <query>_vs__<subject>!".__DEBUG_ECHO
            End If

            Return lstPairs.ToArray
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="DIR">Is a Directory which contains the text file output of the blastp searches.</param>
        ''' <returns></returns>
        <ExportAPI("BBH_Entry.Build")>
        <Extension>
        Public Function BuildBBHEntry(DIR As String, Optional ext As String = "*.txt") As Entry()
            Dim Source As List(Of AlignEntry) = LoadEntries(DIR, ext).AsList
            Return Source.BuildBBHEntry
        End Function

        ''' <summary>
        ''' 只单独加载单向比对的数据入口点列表
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <param name="query"></param>
        ''' <returns></returns>
        <ExportAPI("Load.SBHEntry")>
        Public Function LoadSBHEntry(DIR As String, query As String) As String()
            Dim LQuery As AlignEntry() = (ls - l - wildcards("*.*") <= DIR).Select(AddressOf LogNameParser).ToArray
            Dim Paths$() = LinqAPI.Exec(Of String) _
 _
                () <= From entry As AlignEntry
                      In LQuery.AsParallel
                      Where String.Equals(query, entry.QueryName, StringComparison.OrdinalIgnoreCase)
                      Select entry.FilePath

            Return Paths
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="Source"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="QueryGrep"></param>
        ''' <param name="SubjectGrep"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Export.LogData", Info:="Batch export the log data into the besthit data from the batch blastp operation.")>
        Public Function ExportLogData(<Parameter("Dir.Source")> Source As String,
                                      <Parameter("Dir.Export")> EXPORT As String,
                                      <Parameter("Grep.Query", "Default action is:  Tokens ' ' First")>
                                      Optional QueryGrep As TextGrepScriptEngine = Nothing,
                                      <Parameter("Grep.Subject", "Default action is:  Tokens ' ' First")>
                                      Optional SubjectGrep As TextGrepScriptEngine = Nothing,
                                      <Parameter("Using.UltraLarge.Mode")>
                                      Optional UltraLargeSize As Boolean = False) _
                                      As <FunctionReturns("")> AlignEntry()

            If QueryGrep Is Nothing Then
                QueryGrep = TextGrepScriptEngine.Compile("Tokens ' ' First")
            End If

            If SubjectGrep Is Nothing Then
                SubjectGrep = TextGrepScriptEngine.Compile("Tokens ' ' First")
            End If

            Dim Logs As AlignEntry() = LoadEntries(Source)

            If UltraLargeSize Then
                Return ExportLogDataUltraLargeSize(Logs, EXPORT, QueryGrep, SubjectGrep)
            Else
                Return ExportLogData(Logs, EXPORT, QueryGrep, SubjectGrep)
            End If
        End Function

        <ExportAPI("Export.LogData.UltraLargeSize", Info:="Batch export the log data into the besthit data from the batch blastp operation.")>
        Public Function ExportLogDataUltraLargeSize(<Parameter("DataList.Logs.Entry")> source As IEnumerable(Of AlignEntry),
                                                    <Parameter("Dir.Export")> EXPORT As String,
                                                    <Parameter("Grep.Query")> Optional QueryGrep As TextGrepScriptEngine = Nothing,
                                                    <Parameter("Grep.Subject")> Optional SubjectGrep As TextGrepScriptEngine = Nothing) As <FunctionReturns("")> AlignEntry()

            If QueryGrep Is Nothing Then QueryGrep = TextGrepScriptEngine.Compile("tokens | first")
            If SubjectGrep Is Nothing Then SubjectGrep = TextGrepScriptEngine.Compile("tokens | first")

            Dim LQuery As AlignEntry() =
                LinqAPI.Exec(Of AlignEntry) <= From path As AlignEntry
                                               In source.AsParallel
                                               Let reuslt As AlignEntry =
                                                   __operation(EXPORT, path, QueryGrep, SubjectGrep)
                                               Where Not reuslt Is Nothing
                                               Select reuslt

            Call "All of the available besthit data was exported!".__DEBUG_ECHO

            Return LQuery
        End Function

        Private Function __operation(EXPORT As String, path As AlignEntry, qgr As TextGrepScriptEngine, sgr As TextGrepScriptEngine) As AlignEntry
            Dim filePath As String = $"{EXPORT}/{path.FilePath.BaseName}.besthit.csv"
            If filePath.FileExists Then
                GoTo RETURN_VALUE
            End If

            Dim OutputLog As BlastPlus.v228 =
                BlastPlus.Parser.TryParse(path.FilePath)
            If OutputLog Is Nothing Then
                Return Nothing
            Else
                Call OutputLog.Grep(Query:=qgr.PipelinePointer, Hits:=sgr.PipelinePointer)
            End If

            Dim besthitsData As BBH.BestHit() = OutputLog.ExportBestHit
            Call besthitsData.SaveTo(filePath, False, System.Text.Encoding.ASCII)

RETURN_VALUE:
            path.FilePath = filePath
            Return path
        End Function

        ''' <summary>
        ''' 使用这个函数批量导出sbh数据，假若数据量比较小的话
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="queryGrep">假若解析的方法为空，则会尝试使用默认的方法解析标题</param>
        ''' <param name="SubjectGrep"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <ExportAPI("Export.LogData.List", Info:="Batch export the log data into the besthit data from the batch blastp operation.")>
        Public Function ExportLogData(<Parameter("DataList.Logs.Entry")>
                                      Source As IEnumerable(Of AlignEntry),
                                      <Parameter("Dir.Export")> EXPORT As String,
                                      <Parameter("Grep.Query")> Optional QueryGrep As TextGrepScriptEngine = Nothing,
                                      <Parameter("Grep.Subject")> Optional SubjectGrep As TextGrepScriptEngine = Nothing) _
            As <FunctionReturns("")> AlignEntry()

            If QueryGrep Is Nothing Then QueryGrep = TextGrepScriptEngine.Compile("tokens | first")
            If SubjectGrep Is Nothing Then SubjectGrep = TextGrepScriptEngine.Compile("tokens | first")

            Dim GrepOperation As New GrepOperation(QueryGrep.PipelinePointer, SubjectGrep.PipelinePointer)
            Dim LQuery = (From path As AlignEntry  ' 从日志文件之中解析出比对结果的对象模型
                          In Source.AsParallel
                          Let OutputLog = BlastPlus.Parser.TryParse(path.FilePath)
                          Where OutputLog IsNot Nothing
                          Select path, OutputLog)
            Call "Load blast output log data internal operation job done!".__DEBUG_ECHO
            Dim LogDataChunk = (From OutputLog In LQuery.AsParallel Select logData = GrepOperation.Grep(OutputLog.OutputLog), OutputLog.path)  ' 进行蛋白质序列对象的标题的剪裁操作
            Call "Internal data trimming operation job done! start to writing data....".__DEBUG_ECHO

            For Each File In LogDataChunk
                Dim besthitsData As BBH.BestHit() = File.logData.ExportBestHit

                'If Not SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH.BestHit.IsNullOrEmpty(besthitsData, TrimSelfAligned:=True) Then
                Dim Path As String = EXPORT & "/" & File.path.FilePath.BaseName & ".besthit.csv"
                File.path.FilePath = Path
                Call besthitsData.SaveTo(Path, False, System.Text.Encoding.ASCII)
                'End If
                Call Console.Write(".")
            Next

            Call "All of the available besthit data was exported!".__DEBUG_ECHO

            Return (From PathEntry In LogDataChunk.AsParallel Select PathEntry.path).ToArray
        End Function

        ''' <summary>
        ''' 批量导出最佳比对匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="Source">单项最佳的两两比对的结果数据文件夹</param>
        ''' <param name="EXPORT">双向最佳的导出文件夹</param>
        ''' <param name="CDSAll">从GBK文件列表之中所导出来的蛋白质信息的汇总表</param>
        '''
        <ExportAPI("Export.Besthits", Info:="Batch export the bbh result")>
        Public Function ExportBidirectionalBesthit(Source As IEnumerable(Of AlignEntry),
                                                   <Parameter("CDS.All.Dump", "Lazy loading task.")>
                                                   CDSAll As Task(Of String, Dictionary(Of String, GeneDumpInfo)),
                                                   <Parameter("DIR.EXPORT")> EXPORT As String,
                                                   <Parameter("Null.Trim")> Optional TrimNull As Boolean = False) As BestHit()
            Return ExportBidirectionalBesthit(Source, EXPORT, CDSAll.GetValue, TrimNull)
        End Function

        ''' <summary>
        ''' 批量导出双向最佳比对匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' <param name="Source">单项最佳的两两比对的结果数据文件夹，里面的数据文件都是从blastp里面倒出来的besthit的csv文件</param>
        ''' <param name="EXPORT">双向最佳的导出文件夹</param>
        ''' <param name="CDSInfo">从GBK文件列表之中所导出来的蛋白质信息的汇总表</param>
        '''
        <ExportAPI("Export.Besthits", Info:="Batch export the bbh result")>
        Public Function ExportBidirectionalBesthit(Source As IEnumerable(Of AlignEntry),
                                                   <Parameter("Dir.Export")> EXPORT As String,
                                                   <Parameter("CDS.All.Dump")>
                                                   Optional CDSInfo As Dictionary(Of String, GeneDumpInfo) = Nothing,
                                                   <Parameter("Null.Trim")> Optional TrimNull As Boolean = False) As BestHit()

            Dim Files = (From path As AlignEntry
                         In Source
                         Let besthitData As BBH.BestHit() =
                             path.FilePath.LoadCsv(Of BBH.BestHit)(False).ToArray
                         Select path,
                             besthitData).ToDictionary(Function(x) x.path,
                                                       Function(x) x.besthitData)
            Dim CreateBestHit = (From Path As KeyValuePair(Of AlignEntry, BBH.BestHit())
                                 In Files
                                 Let Data As BiDirectionalBesthit() =
                                     __export(Source, Path.Key, Files, Path.Value)
                                 Select Path = Path.Key,
                                     Data)

            Dim getDescrib As BiDirectionalBesthit.GetDescriptionHandle

            If CDSInfo Is Nothing Then
                getDescrib = Function(null) ""
            Else
                getDescrib = Function(Id As String) If(CDSInfo.ContainsKey(Id), CDSInfo(Id).CommonName, "")
            End If

            Dim GetDescriptionResult = (From item
                                        In CreateBestHit
                                        Let descrMatches As BiDirectionalBesthit() =
                                            BiDirectionalBesthit.MatchDescription(item.Data, sourceDescription:=getDescrib)
                                        Select Path = item.Path,
                                            descrMatches)

            Dim result = GetDescriptionResult.CopyTypeDef

            For Each EntryHit In GetDescriptionResult '保存临时数据
                Dim FileName As String = EXPORT & "/" & EntryHit.Path.FilePath.BaseName & ".bibesthit.csv"
                EntryHit.Path.FilePath = FileName
                Call EntryHit.descrMatches.SaveTo(FileName, False)
                result += EntryHit
            Next

            Dim Grouped = (From item In result Select item Group By item.Path.QueryName Into Group)        '按照Query分组
            Dim EXPORTS = (From Data In Grouped.AsParallel
                           Let hitData = (From x
                                          In Data.Group
                                          Select New KeyValuePair(Of String, Dictionary(Of String, BiDirectionalBesthit))(x.Path.HitName, __getDirectionary(x.descrMatches))).ToArray
                           Select __export(Data.QueryName, hitData)).ToArray   '按照分组将数据导出

            '保存临时数据
            For Each item As BestHit In EXPORTS
                Dim path As String = EXPORT & "/CompiledBesthits/" & item.sp & ".xml"
                Call item.GetXml.SaveTo(path)
                path = EXPORT & "/CompiledCsvData/" & item.sp & ".csv"
                Call item.ExportCsv(TrimNull).Save(path, False)
            Next

            Return EXPORTS
        End Function

        Private Function __getDirectionary(data As BiDirectionalBesthit()) As Dictionary(Of String, BiDirectionalBesthit)
            Return (From x As BiDirectionalBesthit  ' 为什么在这里还是会存在重复的数据？？
                    In data
                    Select x
                    Group x By x.QueryName Into Group) _
                         .ToDictionary(Function(x) x.QueryName,
                                       Function(x) x.Group.First)
        End Function

        ''' <summary>
        ''' 得到最佳双向比对的结果, Top类型
        ''' </summary>
        ''' <param name="Source"></param>
        ''' <param name="Entry"></param>
        ''' <param name="Files"></param>
        ''' <param name="Query"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function __export(Source As IEnumerable(Of AlignEntry),
                                  Entry As AlignEntry,
                                  Files As Dictionary(Of AlignEntry, BBH.BestHit()),
                                  Query As BBH.BestHit()) As BiDirectionalBesthit()

            Dim ReverEntry = Entry.SelectEquals(Source)
            Dim Rever = Files(ReverEntry)
            Dim Result = GetBBHTop(qvs:=Query, svq:=Rever)

            Return Result
        End Function

        Private Function __export(QuerySpeciesName As String, data As KeyValuePair(Of String, Dictionary(Of String, BiDirectionalBesthit))()) As BestHit
            Dim Result As BestHit = New BestHit With {
                .sp = QuerySpeciesName
            }
            Dim QueryProteins As String() = data.First.Value.Keys.ToArray   '作为主键的蛋白质编号
            Dim LQuery As HitCollection() =
                LinqAPI.Exec(Of HitCollection) <=
                From query As String
                In QueryProteins
                Let hits = (From sp As KeyValuePair(Of String, Dictionary(Of String, BiDirectionalBesthit))
                            In data
                            Let hitttt As BiDirectionalBesthit =
                                __export(sp.Value, query)
                            Let hhh As Hit = New Hit With {
                                .tag = sp.Key,
                                .HitName = hitttt.HitName,
                                .Identities = hitttt.Identities,
                                .Positive = hitttt.Positive
                            }
                            Select desc = hitttt.Description,
                                hhh).ToArray
                Let hitCol As HitCollection =
                    New HitCollection With {
                        .QueryName = query,
                        .Description = hits.First.desc,
                        .Hits = hits.Select(Function(x) x.hhh).ToArray
                    }
                Select hitCol

            Return New BestHit With {
                .sp = QuerySpeciesName,
                .hits = LQuery
            }
        End Function

        Private Function __export(hitSpecies As Dictionary(Of String, BiDirectionalBesthit), queryProt As String) As BiDirectionalBesthit
            If hitSpecies.ContainsKey(queryProt) Then
                Return hitSpecies(queryProt)
            Else
                Call $"QueryProtein {queryProt} not found!".__DEBUG_ECHO
                Return BiDirectionalBesthit.NullValue
            End If
        End Function
    End Module
End Namespace
