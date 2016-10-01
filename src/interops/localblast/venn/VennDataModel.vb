#Region "Microsoft.VisualBasic::cc39716e16595e52d268656d33c029ad, ..\interops\localblast\Localblast.Extensions.VennDiagram\VennDataModel.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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
    <[PackageNamespace]("VennDiagram.LDM.BBH",
                        Category:=APICategories.ResearchTools,
                        Publisher:="xie.guigang@gcmodeller.org",
                        Description:="Package for generate the data model for creates the Venn diagram using the completely combination blastp based bbh protein homologous method result.
                        BBH based method is the widely used algorithm for the cell pathway system automatically annotation, such as the kaas system in the KEGG database.")>
    Public Module VennDataModel

        Sub New()
            Call Settings.Initialize(GetType(VennDataModel))
        End Sub

#Region "Creates Handle"

        <ExportAPI("Blast_Plus.Handle.Creates", Info:="Creates the blastp+ program handle automaticaly from the environment variable.")>
        Public Function CreateHandle() As BLASTPlus
            Return New BLASTPlus(GCModeller.FileSystem.GetLocalBlast)
        End Function

        <ExportAPI("Blast_Plus.Session.New()")>
        Public Function NewBlastPlusSession(<Parameter("Blast.Bin", "The program group of the local blast program group.")>
                                            DIR As String) As BLASTPlus
            Return New BLASTPlus(DIR)
        End Function

        <Extension>
        <ExportAPI("Blast_Handle.Create()")>
        Public Function CreateInvokeHandle(<Parameter("Session.Handle")> SessionHandle As BLASTPlus,
                                           <Parameter("Invoke.Blastp",
                                                      "If using this parameter and specific FALSE value, then the function will " &
                                                      "create the blastn handle or create the blastp handle as default.")>
                                           Optional Blastp As Boolean = True) As INVOKE_BLAST_HANDLE

            If Blastp Then
                Return BuildBLASTP_InvokeHandle(SessionHandle)
            Else
                Return BuildBLASTN_InvokeHandle(SessionHandle)
            End If
        End Function

        <ExportAPI("BlastpHandle.From.Blastbin", Info:="Creates the blastp invoke handle from the installed location of the blast program group.")>
        Public Function InvokeCreateBlastpHandle(<Parameter("Blast.Bin", "The program group of the local blast program group.")> DIR As String) As INVOKE_BLAST_HANDLE
            Return BuildBLASTP_InvokeHandle(New BLASTPlus(DIR))
        End Function
#End Region

        ''' <summary>
        ''' {Queries} -> Subject
        ''' 
        ''' .(这个方法是与<see cref="BatchBlastp"></see>相反的，即使用多个Query来查询一个Subject)
        ''' </summary>
        ''' <remarks></remarks>
        <ExportAPI("Blastp.Invoke_Batch", Info:="Batch parallel task scheduler.")>
        Public Sub BatchBlastpRev(<Parameter("Handle.Blastp", "This handle value is the blastp handle, not blastn handle.")> Handle As BLASTPlus,
                                  <Parameter("Dir.Query", "The data directory which contains the query protein fasta data.")> Query As String,
                                  <Parameter("Path.Subject", "The file path value of the subject protein fasta data.")> Subject As String,
                                  <Parameter("Dir.Export", "The data directory for export the blastp data between the query and subject.")> EXPORT As String,
                                  <Parameter("E-Value", "The blastp except value.")> Optional Evalue As String = "1e-3",
                                  <Parameter("Exists.Overriding", "Overrides the exists blastp result if the file length is not ZERO length.")> Optional [Overrides] As Boolean = False,
                                  <Parameter("Using.Parallel")> Optional Parallel As Boolean = False,
                                  Optional numThreads As Integer = -1)

            Dim Queries As Dictionary(Of String, String) = Query.LoadSourceEntryList({"*.fasta", "*.fsa", "*.txt", "*.faa"})

            If Not FileIO.FileSystem.FileExists(Subject) Then
                Dim msg As String = String.Format(SubjectNotFound, Subject.ToFileURL)
                Throw New Exception(msg)
            End If

            Call FileIO.FileSystem.CreateDirectory(EXPORT)
            Call Handle.FormatDb(Subject, Handle.MolTypeProtein).Start(WaitForExit:=True)

            Dim tasks As IORedirectFile() =
                LinqAPI.Exec(Of IORedirectFile) <= From Path As PathEntry
                                                   In Queries
                                                   Let log As String = EXPORT & "/" & Path.Key & ".txt"
                                                   Let invoke As IORedirectFile =
                                                       Handle.Blastp(
                                                       Input:=Path.Value,
                                                       TargetDb:=Subject,
                                                       e:=Evalue,
                                                       Output:=log)
                                                   Select invoke
            Dim runTask As Func(Of IORedirectFile, Integer) = Function(x) x.Start(WaitForExit:=True)

            If Parallel Then
                Call BatchTask(tasks, runTask, numThreads)
            Else
                Handle.NumThreads = Environment.ProcessorCount / 2
                Call BatchTask(tasks, runTask, numThreads:=1)
            End If
        End Sub

        Const SubjectNotFound As String = "Could not found the subject protein database fasta file ""{0}""!"
        Const QueryNotFound As String = "Could not found the query protein fasta file ""{0}""!"

        ''' <summary>
        ''' Query -> {Subjects}
        ''' </summary>
        ''' <param name="Handle"></param>
        ''' <param name="Query"></param>
        ''' <param name="Subject"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="[Overrides]"></param>
        ''' <returns></returns>
        <ExportAPI("Blastp.Invoke_Batch")>
        Public Function BatchBlastp(<Parameter("Handle.Blastp", "This handle value is the blastp handle, not blastn handle.")> Handle As INVOKE_BLAST_HANDLE,
                                    <Parameter("Path.Query", "The file path value of the query protein fasta data.")> Query As String,
                                    <Parameter("Dir.Subject", "The data directory which contains the subject protein fasta data.")> Subject As String,
                                    <Parameter("Dir.Export", "The data directory for export the blastp data between the query and subject.")> EXPORT As String,
                                    <Parameter("E-Value", "The blastp except value.")> Optional Evalue As String = "1e-3",
                                    <Parameter("Exists.Overriding", "Overrides the exists blastp result if the file length is not ZERO length.")>
                                    Optional [Overrides] As Boolean = False,
                                    Optional numThreads As Integer = -1) As String()

            Dim Subjects As Dictionary(Of String, String) =
                Subject.LoadSourceEntryList({"*.fasta", "*.fsa", "*.txt", "*.faa"})

            If Not FileIO.FileSystem.FileExists(Query) Then
                Dim msg As String = String.Format(QueryNotFound, Query.ToFileURL)
                Throw New Exception(msg)
            End If

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim task As Func(Of String, String) = Function(x) Handle(Query,
                                                                     Subject:=x,
                                                                     Evalue:=Evalue,
                                                                     EXPORT:=EXPORT,
                                                                     num_threads:=Environment.ProcessorCount / 2,
                                                                     [Overrides]:=[Overrides])
            Dim LQuery As String() = BatchTask(Of String, String)(
                Subjects.Values,
                task,
                numThreads:=numThreads)
            Return LQuery
        End Function

        <ExportAPI("BBH.Start()", Info:="Only perfermence the bbh analysis for the query protein fasta, the subject source parameter is the fasta data dir path of the subject proteins.")>
        Public Function BBH(<Parameter("Handle.Blastp", "This handle value is the blastp handle, not blastn handle.")> Handle As INVOKE_BLAST_HANDLE,
                            <Parameter("Path.Query", "The file path value of the query protein fasta data.")> Query As String,
                            <Parameter("DIR.Subject", "The data directory which contains the subject protein fasta data.")> Subject As String,
                            <Parameter("DIR.Export", "The data directory for export the blastp data between the query and subject.")> EXPORT As String,
                            <Parameter("E-Value", "The blastp except value.")> Optional Evalue As String = "1e-3",
                            <Parameter("Exists.Overriding", "Overrides the exists blastp result if the file length is not ZERO length.")>
                            Optional [Overrides] As Boolean = False) As AlignEntry()

            If Not FileIO.FileSystem.FileExists(Query) Then
                Throw New Exception($"Could not found the query protein fasta file ""{Query.ToFileURL}""!")
            End If

            Dim Subjects = Subject.LoadSourceEntryList({"*.fasta", "*.fsa", "*.txt"})

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim LQuery As IEnumerable(Of String) = (From Path As PathEntry
                                                    In Subjects.AsParallel
                                                    Select __bbh(Path, Query, Evalue, EXPORT, Handle, [Overrides])).MatrixAsIterator
            Return LQuery.ToArray(AddressOf LogNameParser)
        End Function

        ''' <summary>
        ''' query -> hits;   hits -> query
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <param name="Query"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Handle"></param>
        ''' <param name="[Overrides]"></param>
        ''' <returns></returns>
        Private Function __bbh(Path As PathEntry,
                               Query As String,
                               Evalue As String,
                               EXPORT As String,
                               Handle As INVOKE_BLAST_HANDLE,
                               [Overrides] As Boolean) As String()
            Dim Files As List(Of String) = New List(Of String) +
                Handle(Query, Path.Value, 1, Evalue, EXPORT, [Overrides]) +
                Handle(Path.Value, Query, 1, Evalue, EXPORT, [Overrides])
            Return Files.ToArray
        End Function

        <ExportAPI("Integrity.Checks")>
        Public Function CheckIntegrity(<Parameter("Blastp.Handle")> Handle As INVOKE_BLAST_HANDLE,
                                       <Parameter("Dir.Source.Input", "The data directory which contains the protein sequence fasta files.")> Input As String,
                                       <Parameter("Dir.Blastp.Export", "The data directory for export the blastp result.")> EXPORT As String,
                                       <Parameter("E-value")> Optional Evalue As String = "1e-3") _
                                    As <FunctionReturns("The file log path which is not integrity.")> String()

            Dim Files As IEnumerable(Of String) = ls - l - r - wildcards("*.fasta", "*.fsa", "*.fa") <= Input
            Dim ComboList = Comb(Of String).CreateCompleteObjectPairs(Files).MatrixAsIterator

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim LQuery As String() = (From paired As KeyValuePair(Of String, String)
                                      In ComboList.AsParallel
                                      Let PathLog As String = BuildFileName(paired.Key, paired.Value, EXPORT)
                                      Let InternalInvoke = paired.__invokeInner(PathLog, Handle, Evalue, EXPORT)
                                      Where Not String.IsNullOrEmpty(InternalInvoke)
                                      Select InternalInvoke).ToArray
            Return LQuery
        End Function

        <Extension>
        Private Function __invokeInner(paired As KeyValuePair(Of String, String),
                                   PathLog As String,
                                   Handle As INVOKE_BLAST_HANDLE,
                                   Evalue As String,
                                   EXPORT As String) As String
            If NCBILocalBlast.FastCheckIntegrityProvider(FastaFile.Read(paired.Key), PathLog) Then
                Call Console.Write(".")
                Return ""
            Else
                Call VBDebugger.Warning($"File ""{PathLog.ToFileURL}"" is incorrect!")
                Return Handle(Query:=paired.Key, Subject:=paired.Value, Evalue:=Evalue, EXPORT:=EXPORT, num_threads:=Environment.ProcessorCount / 2, [Overrides]:=True)
            End If
        End Function

        ''' <summary>
        ''' 两两组合的双向比对用来创建文氏图所需要的数据
        ''' </summary>
        ''' <param name="Handle"></param>
        ''' <param name="Input"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="Parallel"></param>
        ''' <param name="Overrides"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("_Start_Task()", Info:="Completely combination of the blastp search result for creating the venn diagram data model.")>
        Public Function StartTask(<Parameter("Blastp.Handle")> Handle As INVOKE_BLAST_HANDLE,
                                  <Parameter("Dir.Source.Input", "The data directory which contains the protein sequence fasta files.")> Input As String,
                                  <Parameter("Dir.Blastp.Export", "The data directory for export the blastp result.")> EXPORT As String,
                                  <Parameter("E-value")> Optional Evalue As String = "1e-3",
                                  <Parameter("Task.Parallel", "The task is parallelize? Default is yes!")> Optional Parallel As Boolean = True,
                                  <Parameter("Exists.Overrides", "If the target blastp output log data is not a empty file, " &
                                  "then if overrides then the blastp will be invoke again orelse function will skip this not null file.")>
                                  Optional [Overrides] As Boolean = False) As AlignEntry()

            If Parallel Then
                Return TaskBuilder_p(Input, EXPORT, Evalue, Handle, [Overrides])
            Else
                Return TaskBuilder(Input, EXPORT, Evalue, Handle, [Overrides])
            End If
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
                                  <Parameter("Null.Trim")> Optional TrimNull As Boolean = False) As DocumentStream.File

            Dim dataHash As Dictionary(Of String, BestHit) = data.ToDictionary(Function(item) item.sp)
            Dim IndexKey As String = dataHash.Keys(__parserIndex(dataHash, mainIndex))
            Dim indexQuery As BestHit = dataHash(IndexKey)

            Call dataHash.Remove(IndexKey)

            If indexQuery.hits.IsNullOrEmpty Then
                Call $"The profile data of your key ""{mainIndex}"" ---> ""{indexQuery.sp}"" is null!".__DEBUG_ECHO
                Call "Thread exists...".__DEBUG_ECHO
                Return New DocumentStream.File
            End If

            Dim dataframe As DocumentStream.File = indexQuery.ExportCsv(TrimNull)
            Dim sps As String() = indexQuery.hits.First.Hits.ToArray(Function(x) x.tag)

            For deltaIndex As Integer = 0 To dataHash.Count - 1
                Dim subMain As BestHit = dataHash.Values(deltaIndex)

                If subMain.hits.IsNullOrEmpty Then
                    Call $"Profile data {subMain.sp} is null!".__DEBUG_ECHO
                    Continue For
                End If

                Dim di As Integer = deltaIndex
                Dim subHits As String() =
                    LinqAPI.Exec(Of String) <= From row As DocumentStream.RowObject
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

                    Dim row As DocumentStream.RowObject =
                        New DocumentStream.RowObject From {subMainNotHit.Description, subMainNotHit.QueryName} +
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
