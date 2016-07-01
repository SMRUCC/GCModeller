Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

''' <summary>
''' 基因组两两比较所得到的位点距离数据
''' </summary>
''' <remarks></remarks>
Public Class SiteSigma
    <Column("Site")> Public Property Site As Integer
    <Column("Sigma")> Public Property Sigma As Double
    Public Property Similarity As DifferenceMeasurement.SimilarDiscriptions
End Class