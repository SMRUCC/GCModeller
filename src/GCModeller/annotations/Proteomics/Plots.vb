Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Analysis.GO

Public Module Plots

    <Extension>
    Public Function GOEnrichmentPlot(input As IEnumerable(Of EnrichmentTerm), obo$) As Bitmap
        Dim GO_terms = GO_OBO.Open(obo).ToDictionary(Function(x) x.id)
        Dim getData = Function(gene As EnrichmentTerm) (gene.ID, gene.number)
        Return input.Plot(getData, GO_terms)
    End Function
End Module
