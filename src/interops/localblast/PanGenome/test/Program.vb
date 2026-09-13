Imports SMRUCC.genomics.Analysis.PanGenome

Module Program
    Sub Main(args As String())
        Dim source As String = "N:\GMNDesigner\pangenome\Escherichia_coli"
        Dim result = PanGenomeResult.LoadStream($"{source}\result.zip".Open(IO.FileMode.Open, [readOnly]:=True))
        Dim report As String = result.GenerateReport(templateContent:="G:\GCModeller\src\interops\localblast\PanGenome\Report.html".ReadAllText)

        Call report.SaveTo($"{source}\result.html")
    End Sub
End Module
