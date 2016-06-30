Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class DispositionReactant

        ''' <summary>
        ''' 本属性仅能够取两个值：<see cref="DispositionReactant.GENERAL_TYPE_ID_POLYPEPTIDE">多肽链</see>和<see cref="DispositionReactant.GENERAL_TYPE_ID_TRANSCRIPTS">RNA分子</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property GeneralType As String
        <CollectionAttribute("Enzymes")> Public Property Enzymes As String()

        Public Const GENERAL_TYPE_ID_POLYPEPTIDE As String = "Polypeptide"
        Public Const GENERAL_TYPE_ID_TRANSCRIPTS As String = "Transcripts"

        Public Property UPPER_BOUND As Double

        Public Overrides Function ToString() As String
            Return GeneralType
        End Function
    End Class
End Namespace