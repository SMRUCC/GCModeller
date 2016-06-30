Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Sanger.Pfam
Imports LANS.SystemsBiology.AnalysisTools
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.Linq
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Parallel.Threads
Imports LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Application
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

<PackageNamespace("Xfam.CLI",
                  Description:="Xfam Tools (Pfam, Rfam, iPfam)",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://xfam.org")>
Module CLI

    <ExportAPI("/Rfam.Align", Usage:="/Rfam.Align /query <sequence.fasta> [/rfam <DIR> /out <outDIR> /num_threads -1 /ticks 1000]")>
    <ParameterInfo("/formatdb", True,
                   Description:="If the /rfam directory parameter is specific and the database is not formatted, then this value should be TRUE for local blast. 
                   If /rfam parameter is not specific, then the program will using the system database if it is exists, and the database is already be formatted as the installation of the database is includes this formation process.")>
    Public Function RfamAlignment(args As CommandLine.CommandLine) As Integer
        Dim query As String = args("/query")
        Dim outDIR As String = args.GetValue("/out", query.TrimFileExt)
        Dim rFam As String = args("/rfam")

        If String.IsNullOrEmpty(rFam) Then
            rFam = Global.Xfam.GCModeller.FileSystem.Xfam.Rfam.RfamFasta
        End If

        Dim blastbin As String = GCModeller.FileSystem.GetLocalBlast
        Dim blast As New LANS.SystemsBiology.NCBI.Extensions.LocalBLAST.Programs.BLASTPlus(blastbin)
        Dim num_threads As Integer = args.GetValue("/num_threads", -1)
        Dim ticks As Integer = args.GetValue("/ticks", 1000)

        Return LANS.SystemsBiology.NCBI.Extensions.Blastn(blast, query, rFam, outDIR,
                                                          reversed:=True,   ' 是反过来比对的？？？
                                                          numThreads:=num_threads,
                                                          TimeInterval:=ticks).CLICode
    End Function

    <ExportAPI("/Rfam.SeedsDb.Dump", Usage:="/Rfam.SeedsDb.Dump /in <rfam.seed> [/out <rfam.csv>]")>
    Public Function DumpSeedsDb(args As CommandLine.CommandLine) As Integer
        Dim inDb As String = args("/in")
        Dim out As String = args.GetValue("/out", inDb.TrimFileExt & ".Csv")
        Dim loads As Dictionary(Of String, Rfam.Stockholm) = Rfam.API.ReadDb(inDb)
        Dim outDIR As String = out.TrimFileExt

        For Each Rfam As KeyValuePair(Of String, Rfam.Stockholm) In loads
            Dim path As String = outDIR & $"/{Rfam.Key}.fasta"
            Call Rfam.Value.Alignments.Save(-1, path, Encodings.ASCII)
        Next

        Return loads.Values.SaveTo(out).CLICode
    End Function

    <ExportAPI("/Export.Blastn", Usage:="/Export.Blastn /in <blastout.txt> [/out <blastn.Csv>]")>
    Public Function ExportBlastn(args As CommandLine.CommandLine) As Integer
        Dim inFile As String = args("/in")
        Dim out As String = args.GetValue("/out", inFile.TrimFileExt & ".csv")
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
        Dim out As String = inFile.TrimFileExt & ".csv"
        Return __exportCommon(inFile, out)
    End Function

    <ExportAPI("/Export.Blastn.Batch", Usage:="/Export.Blastn.Batch /in <blastout.DIR> [/out outDIR /large /num_threads <-1> /no_parallel]")>
    Public Function ExportBlastns(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/in")
        Dim out As String = args.GetValue("/out", App.CurrentDirectory & "/" & FileIO.FileSystem.GetDirectoryInfo(inDIR).Name)
        Dim large As Boolean = args.GetBoolean("/large")
        Dim lstFiles = ProgramPathSearchTool.LoadEntryList(inDIR, "*.txt")

        If large Then

            Dim this As String = App.ExecutablePath
            Dim getThis = Function() this
            Dim num_threads = args.GetValue("/num_threads", -1)
            Dim noParallel As Boolean = args.GetBoolean("/no_parallel")

            If noParallel Then
                Call lstFiles.ToArray(Function(x) __batchExportOpr(inFile:=x.x))
            Else
                Call BatchTask(lstFiles,
                               getExe:=getThis,
                               getCLI:=Function(x) $"/Export.Blastn /in {x.x.CliPath}",
                               numThreads:=num_threads,
                               TimeInterval:=100)
            End If
        Else
            Dim LQuery = From file As NamedValue(Of String)
                         In lstFiles
                         Select Id = file.Name,
                             blastn = BlastPlus.Parser.TryParse(file.x)
            Dim Exports = (From file In LQuery.AsParallel
                           Let exportData = MapsAPI.Export(file.blastn)
                           Let path As String = $"{out}/{file.Id}.csv"
                           Select exportData.SaveTo(path)).ToArray
        End If

        Return 0
    End Function
End Module
