Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace PlantRegMap

    Public Module EnrichmentPlots

        <Extension>
        Public Function PlantEnrichmentPlot(data As IEnumerable(Of PlantRegMap_GoTermEnrichment),
                                            GO_terms As Dictionary(Of Term),
                                            Optional pvalue# = 0.05,
                                            Optional size As Size = Nothing,
                                            Optional tick# = 1) As Bitmap
            Return data.EnrichmentPlot(GO_terms, pvalue, size, tick)
        End Function
    End Module
End Namespace