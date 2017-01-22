Imports Microsoft.VisualBasic.Data.csv.DocumentStream
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize

Module Module2

    Sub Main()

        Dim sample = EntityObject.LoadDataSet("G:\GCModeller\GCModeller\R\vocano\qlfTable.csv")
        Call Volcano.PlotDEGs(sample, pvalue:="PValue", displayLabel:=LabelTypes.DEG).SaveAs("x:\test.png")
    End Sub
End Module
