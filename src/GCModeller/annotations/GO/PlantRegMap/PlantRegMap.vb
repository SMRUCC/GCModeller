
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Microarray

Namespace PlantRegMap

    ''' <summary>
    ''' PlantRegMap. go enrichment result output
    ''' </summary>
    Public Class PlantRegMap_GoTermEnrichment
        Implements IGoTerm
        Implements IGoTermEnrichment

        <Column("GO.ID")> Public Property GoID As String Implements IGoTerm.Go_ID

        Public Property Term As String
        Public Property Annotated As String
        Public Property Count As String
        Public Property Expected As String
        <Column("p-value")> Public Property pvalue As Double Implements IGoTermEnrichment.Pvalue
        <Column("q-value")> Public Property qvalue As Double
        Public Property Aspect As String
        Public Property Genes As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace