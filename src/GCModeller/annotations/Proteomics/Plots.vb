Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Analysis.GO

Public Module Plots

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="input"></param>
    ''' <param name="obo$"></param>
    ''' <param name="p#">Default cutoff is ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>) &lt;= 0.05``</param>
    ''' <returns></returns>
    <Extension>
    Public Function GOEnrichmentPlot(input As IEnumerable(Of EnrichmentTerm), obo$, Optional p# = 0.05) As Bitmap
        Dim GO_terms = GO_OBO.Open(obo).ToDictionary(Function(x) x.id)
        Dim getData = Function(gene As EnrichmentTerm) (gene.ID, gene.number)
        Return input.Where(Function(x) x.P <= p#).Plot(getData, GO_terms)
    End Function
End Module
