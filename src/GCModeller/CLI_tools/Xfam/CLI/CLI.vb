#Region "Microsoft.VisualBasic::3dccee762994d0256653110d01b0fc5b, CLI_tools\Xfam\CLI\CLI.vb"

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

' Module CLI
' 
'     Function: __batchExportOpr, __exportCommon, DumpSeedsDb, ExportBlastn, ExportBlastns
'               RfamAlignment
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO.Linq
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports Parallel
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Interops.NCBI
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

<Package("Xfam.CLI",
                  Description:="Xfam Tools (Pfam, Rfam, iPfam)",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://xfam.org")>
<CLI>
Module CLI

    <ExportAPI("/Rfam.Align")>
    <Usage("/Rfam.Align /query <sequence.fasta> [/rfam <DIR> /out <outDIR> /num_threads -1 /ticks 1000]")>
    Public Function RfamAlignment(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim outDIR As String = args.GetValue("/out", query.TrimSuffix)
        Dim rFam As String = args("/rfam")

        If String.IsNullOrEmpty(rFam) Then
            rFam = Global.Xfam.GCModeller.FileSystem.Xfam.Rfam.RfamFasta
        End If

        Dim blastbin As String = GCModeller.FileSystem.GetLocalblast
        Dim blast As New SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus(blastbin)
        Dim num_threads As Integer = args.GetValue("/num_threads", -1)
        Dim ticks As Integer = args.GetValue("/ticks", 1000)

        Return ParallelTask.Blastn(blast, query, rFam, outDIR,
                                                          reversed:=True,   ' 是反过来比对的？？？
                                                          numThreads:=num_threads,
                                                          TimeInterval:=ticks).CLICode
    End Function

    <ExportAPI("/Rfam.SeedsDb.Dump")>
    <Usage("/Rfam.SeedsDb.Dump /in <rfam.seed> [/out <rfam.csv>]")>
    Public Function DumpSeedsDb(args As CommandLine) As Integer
        Dim inDb As String = args("/in")
        Dim out As String = args.GetValue("/out", inDb.TrimSuffix & ".Csv")
        Dim loads As Dictionary(Of String, Rfam.Stockholm) = Rfam.API.ReadDb(inDb)
        Dim outDIR As String = out.TrimSuffix

        For Each Rfam As KeyValuePair(Of String, Rfam.Stockholm) In loads
            Dim path As String = outDIR & $"/{Rfam.Key}.fasta"
            Call Rfam.Value.Alignments.Save(-1, path, Encodings.ASCII)
        Next

        Return loads.Values.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.Blastn")>
    <Usage("/Export.Blastn /in <blastout.txt> [/out <blastn.Csv>]")>
    Public Function ExportBlastn(args As CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimSuffix & ".csv")
        Return __exportCommon(inFile, out)
    End Function

    Private Function __exportCommon(inFile As String, out As String) As Integer
        If BlastPlus.UltraLarge(inFile) Then

TEST:       Call $"{inFile.ToFileURL} is in ultra large size, start lazy loading...".__DEBUG_ECHO

            Using fileStream As New WriteStream(Of BlastnMapping)(out)
                Dim save As Action(Of BlastPlus.Query) =
                    Sub(query As BlastPlus.Query)
                        Dim lstBuffer As BlastnMapping() = MapsAPI.CreateObject(query)
                        Call fileStream.Flush(lstBuffer)
                    End Sub
                Dim chunkSize As Long = 768 * 1024 * 1024

                Call $"{inFile.ToFileURL} ===> {out.ToFileURL}....".__DEBUG_ECHO
                Call BlastPlus.Parser.Transform(inFile, chunkSize, save)
            End Using

            Return 0
        Else
            Dim blastn = BlastPlus.Parser.ParsingSizeAuto(inFile)
            Dim overviews As BlastnMapping() = MapsAPI.Export(blastn)
            Return overviews.SaveTo(out).CLICode
        End If
    End Function

    Private Function __batchExportOpr(inFile As String) As Integer
        Dim out As String = inFile.TrimSuffix & ".csv"
        Return __exportCommon(inFile, out)
    End Function

    <ExportAPI("/Export.Blastn.Batch")>
    <Usage("/Export.Blastn.Batch /in <blastout.DIR> [/out outDIR /large /num_threads <-1> /no_parallel]")>
    Public Function ExportBlastns(args As CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/" & FileIO.FileSystem.GetDirectoryInfo(inDIR).Name)
        Dim large As Boolean = args.GetBoolean("/large")
        Dim lstFiles = RepositoryFileSystem.LoadEntryList(inDIR, "*.txt")

        If large Then

            Dim this As String = App.ExecutablePath
            Dim getThis = Function() this
            Dim num_threads = args.GetValue("/num_threads", -1)
            Dim noParallel As Boolean = args.GetBoolean("/no_parallel")

            If noParallel Then
                Call lstFiles.Select(Function(x) __batchExportOpr(inFile:=x.Value))
            Else
                Call ThreadTask(Of Integer) _
                    .CreateThreads(lstFiles, Function(x)
                                                 Return New IORedirectFile(getThis(), $"/Export.Blastn /in {x.Value.CLIPath}").Run
                                             End Function) _
                    .WithDegreeOfParallelism(num_threads) _
                    .RunParallel _
                    .ToArray
            End If
        Else
            Dim LQuery = From file As NamedValue(Of String)
                         In lstFiles
                         Select Id = file.Name,
                             blastn = BlastPlus.Parser.TryParse(file.Value)
            Dim Exports = (From file In LQuery.AsParallel
                           Let exportData = MapsAPI.Export(file.blastn)
                           Let path As String = $"{out}/{file.Id}.csv"
                           Select exportData.SaveTo(path)).ToArray
        End If

        Return 0
    End Function
End Module
