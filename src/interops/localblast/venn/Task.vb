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
    ''' NCBI blast parallel task
    ''' </summary>
    Public Module ParallelTaskAPI

        Sub New()
            Call Settings.Initialize(GetType(ParallelTaskAPI))
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
                                                                 subject:=x,
                                                                 evalue:=Evalue,
                                                                 EXPORT:=EXPORT,
                                                                 num_threads:=Environment.ProcessorCount / 2,
                                                                 [overrides]:=[Overrides])
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
                Return Handle(query:=paired.Key, subject:=paired.Value, evalue:=Evalue, EXPORT:=EXPORT, num_threads:=Environment.ProcessorCount / 2, [overrides]:=True)
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
    End Module
End Namespace