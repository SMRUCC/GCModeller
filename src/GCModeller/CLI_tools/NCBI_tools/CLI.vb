Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Assembly.NCBI

Module CLI

    <ExportAPI("/Build_gi2taxi",
               Usage:="/Build_gi2taxi /in <gi2taxi.dmp> [/out <out.dat>]")>
    Public Function Build_gi2taxi(args As CommandLine) As Integer
        Dim [in] As String = args("/in")
        Dim out As String = args.GetValue("/out", [in].TrimFileExt & ".bin")
        Return Taxonomy.Archive([in], out)
    End Function
End Module
