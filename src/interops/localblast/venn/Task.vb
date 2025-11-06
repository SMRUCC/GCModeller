#Region "Microsoft.VisualBasic::289672f60487d9d7267b5f57a4404fa3, localblast\venn\Task.vb"

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

'     Module ParallelTaskAPI
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: __bbh, __invokeInner, BatchBlastp, BBH, CheckIntegrity
'                   CreateHandle, CreateInvokeHandle, InvokeCreateBlastpHandle, NewBlastPlusSession, StartTask
' 
'         Sub: BatchBlastpRev
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Darwinism.HPC.Parallel.ThreadTask
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs
Imports SMRUCC.genomics.Interops.NCBI.ParallelTask
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports PathEntry = System.Collections.Generic.KeyValuePair(Of String, String)

Namespace BlastAPI

    ''' <summary>
    ''' NCBI blast parallel task
    ''' </summary>
    Public Module ParallelTaskAPI

        Sub New()
            Call Settings.Initialize()
        End Sub

#Region "Creates Handle"

        <ExportAPI("Blast_Plus.Handle.Creates", Info:="Creates the blastp+ program handle automaticaly from the environment variable.")>
        Public Function CreateHandle() As BLASTPlus
            Return New BLASTPlus(GCModeller.FileSystem.GetLocalblast)
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
                                       Optional Blastp As Boolean = True) As BlastInvoker

            If Blastp Then
                Return BuildBLASTP_InvokeHandle(SessionHandle)
            Else
                Return BuildBLASTN_InvokeHandle(SessionHandle)
            End If
        End Function

        <ExportAPI("BlastpHandle.From.Blastbin", Info:="Creates the blastp invoke handle from the installed location of the blast program group.")>
        Public Function InvokeCreateBlastpHandle(<Parameter("Blast.Bin", "The program group of the local blast program group.")> DIR As String) As BlastInvoker
            Return BuildBLASTP_InvokeHandle(New BLASTPlus(DIR))
        End Function
#End Region

        ''' <summary>
        ''' Batch parallel task scheduler.
        ''' 
        ''' {Queries} -> Subject
        ''' 
        ''' .(这个方法是与<see cref="BatchBlastp"></see>相反的，即使用多个Query来查询一个Subject)
        ''' </summary>
        ''' <remarks></remarks>
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
            Call Handle.FormatDb(Subject, Handle.MolTypeProtein).Start(waitForExit:=True)

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
            Dim runTask As Func(Of IORedirectFile, Integer) = Function(x) x.Start(waitForExit:=True)

            If Parallel Then
                Call ThreadTask(Of Integer).CreateThreads(tasks, runTask).WithDegreeOfParallelism(numThreads).RunParallel.ToArray
            Else
                Handle.NumThreads = Environment.ProcessorCount / 2
                Call ThreadTask(Of Integer).CreateThreads(tasks, runTask).WithDegreeOfParallelism(1).RunParallel.ToArray
            End If
        End Sub

        Const SubjectNotFound As String = "Could not found the subject protein database fasta file ""{0}""!"
        Const QueryNotFound As String = "Could not found the query protein fasta file ""{0}""!"

        ''' <summary>
        ''' Query -> {Subjects}
        ''' </summary>
        ''' <param name="Handle"></param>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="[overrides]"></param>
        ''' <returns></returns>
        <ExportAPI("Blastp.Invoke_Batch")>
        Public Function BatchBlastp(<Parameter("blastp", "This handle value is the blastp handle, not blastn handle.")> Handle As BlastInvoker,
                                    <Parameter("FASTA.query", "The file path value of the query protein fasta data.")> query$,
                                    <Parameter("DIR.subject", "The data directory which contains the subject protein fasta data.")> subject$,
                                    <Parameter("DIR.EXPORT", "The data directory for export the blastp data between the query and subject.")> EXPORT$,
                                    <Parameter("E-value", "The blastp except value.")>
                                    Optional Evalue$ = "1e-3",
                                    <Parameter("exists.overrides", "Overrides the exists blastp result if the file length is not ZERO length.")>
                                    Optional [overrides] As Boolean = False,
                                    Optional numThreads% = -1) As String()

            Dim subjects As Dictionary(Of String, String) =
                subject.LoadSourceEntryList({"*.fasta", "*.fsa", "*.txt", "*.faa"})

            If Not query.FileExists Then
                Dim msg As String = String.Format(QueryNotFound, query.ToFileURL)
                Throw New Exception(msg)
            Else
                Call EXPORT.MakeDir
            End If

            Dim task As Func(Of String, String) =
                Function(x)
                    Return Handle(query,
                                   subject:=x,
                                   evalue:=Evalue,
                                   EXPORT:=EXPORT,
                                   num_threads:=Environment.ProcessorCount / 2,
                                   [overrides]:=[overrides])
                End Function
            Dim LQuery As String() = ThreadTask(Of String) _
                .CreateThreads(subjects.Values, task) _
                .WithDegreeOfParallelism(numThreads) _
                .RunParallel _
                .ToArray

            Return LQuery
        End Function

        ''' <summary>
        ''' Only perfermence the bbh analysis for the query protein fasta, the subject source parameter is the fasta data dir path of the subject proteins.
        ''' </summary>
        ''' <param name="handle"></param>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="[overrides]"></param>
        ''' <returns></returns>
        Public Function BBH(<Parameter("Handle.Blastp", "This handle value is the blastp handle, not blastn handle.")> handle As BlastInvoker,
                            <Parameter("Path.Query", "The file path value of the query protein fasta data.")> query As String,
                            <Parameter("DIR.Subject", "The data directory which contains the subject protein fasta data.")> subject As String,
                            <Parameter("DIR.Export", "The data directory for export the blastp data between the query and subject.")> EXPORT As String,
                            <Parameter("E-Value", "The blastp except value.")>
                            Optional Evalue As String = "1e-3",
                            <Parameter("Exists.Overriding", "Overrides the exists blastp result if the file length is not ZERO length.")>
                            Optional [overrides] As Boolean = False) As AlignEntry()

            If Not query.FileExists Then
                Throw New Exception($"Could not found the query protein fasta file ""{query.ToFileURL}""!")
            Else
                Call EXPORT.MakeDir
            End If

            Dim run As Func(Of PathEntry, String()) =
                Function(path) __bbh(path, query, Evalue, EXPORT, handle, [overrides])
            Dim subjects As Dictionary(Of String, String) =
                subject.LoadSourceEntryList({"*.fasta", "*.fsa", "*.txt"})
            Dim LQuery As IEnumerable(Of String) = subjects _
                .AsParallel _
                .Select(run) _
                .IteratesALL

            Return LQuery.Select(AddressOf LogNameParser).ToArray
        End Function

        ''' <summary>
        ''' query -> hits;   hits -> query
        ''' </summary>
        ''' <param name="path"></param>
        ''' <param name="query"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="handle"></param>
        ''' <param name="[overrides]"></param>
        ''' <returns></returns>
        Private Function __bbh(path As PathEntry, query$, Evalue$, EXPORT$, handle As BlastInvoker, [overrides] As Boolean) As String()
            Dim files As List(Of String) =
                New List(Of String) +
                    handle(query, path.Value, 1, Evalue, EXPORT, [overrides]) +
                    handle(path.Value, query, 1, Evalue, EXPORT, [overrides])

            Return files.ToArray
        End Function

        <ExportAPI("Integrity.Checks")>
        Public Function CheckIntegrity(<Parameter("Blastp.Handle")> Handle As BlastInvoker,
                                   <Parameter("Dir.Source.Input", "The data directory which contains the protein sequence fasta files.")> Input As String,
                                   <Parameter("Dir.Blastp.Export", "The data directory for export the blastp result.")> EXPORT As String,
                                   <Parameter("E-value")> Optional Evalue As String = "1e-3") _
                                As <FunctionReturns("The file log path which is not integrity.")> String()

            Dim Files As IEnumerable(Of String) = ls - l - r - wildcards("*.fasta", "*.fsa", "*.fa") <= Input
            Dim ComboList = Comb(Of String).CreateCompleteObjectPairs(Files).IteratesALL

            Call FileIO.FileSystem.CreateDirectory(EXPORT)

            Dim LQuery As String() = (From paired As Tuple(Of String, String)
                                      In ComboList.AsParallel
                                      Let PathLog As String = BuildFileName(paired.Item1, paired.Item2, EXPORT)
                                      Let InternalInvoke = paired.__invokeInner(PathLog, Handle, Evalue, EXPORT)
                                      Where Not String.IsNullOrEmpty(InternalInvoke)
                                      Select InternalInvoke).ToArray
            Return LQuery
        End Function

        <Extension>
        Private Function __invokeInner(paired As Tuple(Of String, String),
                               PathLog As String,
                               Handle As BlastInvoker,
                               Evalue As String,
                               EXPORT As String) As String
            If NCBILocalBlast.FastCheckIntegrityProvider(FastaFile.Read(paired.Item1), PathLog) Then
                Call Console.Write(".")
                Return ""
            Else
                Call VBDebugger.warning($"File ""{PathLog.ToFileURL}"" is incorrect!")
                Return Handle(query:=paired.Item1, subject:=paired.Item2,
                              evalue:=Evalue,
                              EXPORT:=EXPORT,
                              num_threads:=Environment.ProcessorCount / 2,
                              [overrides]:=True)
            End If
        End Function

        ''' <summary>
        ''' Completely combination of the blastp search result for creating the venn diagram data model.
        ''' (两两组合的双向比对用来创建文氏图所需要的数据)
        ''' </summary>
        ''' <param name="Handle"></param>
        ''' <param name="Input"></param>
        ''' <param name="EXPORT"></param>
        ''' <param name="Evalue"></param>
        ''' <param name="Parallel"></param>
        ''' <param name="Overrides"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function StartTask(<Parameter("Blastp.Handle")> Handle As BlastInvoker,
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
                Return VennDataBuilder.TaskBuilder(Input, EXPORT, Evalue, Handle, [Overrides])
            End If
        End Function
    End Module
End Namespace
