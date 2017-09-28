Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining
Imports Microsoft.VisualBasic.Language

Partial Module CLI

    <ExportAPI("/Network.PCC")>
    <Usage("/Network.PCC /in <matrix.csv> [/cut <default=0.45> /out <out.DIR>]")>
    Public Function PccNetwork(args As CommandLine) As Integer
        Dim in$ = args <= "/in"
        Dim cut# = args.GetValue("/cut", 0.45)
        Dim out$ = (args <= "/out") Or ([in].TrimSuffix & ".PCC/").AsDefault
        Dim matrix As DataSet() = DataSet.LoadDataSet([in]).ToArray
        Dim network = CorrelationNetwork.BuildNetwork(matrix, cut)
        Return network.Save(out).CLICode
    End Function
End Module