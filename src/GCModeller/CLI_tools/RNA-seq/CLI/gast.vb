Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.Metagenome

Partial Module CLI

    <ExportAPI("/gast")>
    Public Function gastInvoke(args As CommandLine) As Integer

        '   Dim test = gast.load_reftaxa("G:\gast_files\refssu.tax")

        gast.mothur = "G:\gast_files\mothur.exe"
        gast.usearch = "G:\gast_files\usearch8.1.1861_win32.exe"

        Return gast.Invoke(args).CLICode
    End Function
End Module
