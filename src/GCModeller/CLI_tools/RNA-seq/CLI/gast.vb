Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.Metagenome

Partial Module CLI

    <ExportAPI("/gast")>
    Public Function gastInvoke(args As CommandLine) As Integer

        Dim test = gast.load_reftaxa("G:\gast_files\refssu.tax")

        Return gast.Invoke(args).CLICode
    End Function
End Module
