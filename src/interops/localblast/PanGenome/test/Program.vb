Imports SMRUCC.genomics.Analysis.PanGenome

Module Program
    Sub Main(args As String())
        Dim result = PanGenomeResult.LoadStream("N:\GMNDesigner\pangenome\Saccharomyces_cerevisiae\result.zip".Open(IO.FileMode.Open, [readOnly]:=True))
        Dim report As String = result.GenerateReport(templateContent:="G:\GCModeller\src\interops\localblast\PanGenome\Report.html".ReadAllText)

        Call report.SaveTo("N:\GMNDesigner\pangenome\Saccharomyces_cerevisiae\result.html")
    End Sub
End Module
