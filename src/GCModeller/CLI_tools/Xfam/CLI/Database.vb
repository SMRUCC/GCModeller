Imports LANS.SystemsBiology.AnalysisTools.Rfam
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv

Partial Module CLI

    ''' <summary>
    ''' 包括文件复制以及FormatDb
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("--Install.Rfam", Usage:="--Install.Rfam /seed <rfam.seed>")>
    Public Function InstallRfam(args As CommandLine.CommandLine) As Integer
        Dim input As String = args("/seed")
        Dim respo As String = GCModeller.FileSystem.Xfam.Rfam.Rfam
        Dim seedDb As Dictionary(Of String, Stockholm) = ReadDb(input)
        Dim outDIR As String = GCModeller.FileSystem.Xfam.Rfam.RfamFasta
        Dim out As String = respo & "/Rfam.Csv"

        For Each rfam As KeyValuePair(Of String, Stockholm) In seedDb
            Dim path As String = outDIR & $"/{rfam.Key}.fasta"
            Call rfam.Value.Alignments.Save(-1, path, Encodings.ASCII)
        Next

        Return seedDb.Values.SaveTo(out).CLICode
    End Function
End Module