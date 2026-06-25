Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.metaTraits.Traitar.Modules

Namespace metaTraits.Traitar

    Public Class ReportJSON

        Public Property phenotypeId As String
        Public Property accession As String
        Public Property category As String
        Public Property predict As PredictionResults
        Public Property positive As Integer
        Public Property negative As Integer
        Public Property confidence As Double
        Public Property scores As Double()
        Public Property labels As Integer()

        Public Property KeyFeatures As FeatureSelection.KeyFeature()

    End Class
End Namespace