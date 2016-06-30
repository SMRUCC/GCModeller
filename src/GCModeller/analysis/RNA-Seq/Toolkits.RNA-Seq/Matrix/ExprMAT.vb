Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection

Namespace dataExprMAT

    Public Interface IExprMAT
        Property LocusId As String
        Property dataExpr0 As Dictionary(Of String, Double)
    End Interface

    Public Class ExprMAT : Implements IExprMAT

        Public Property LocusId As String Implements IExprMAT.LocusId

        <Meta(GetType(Double))>
        Public Property dataExpr0 As Dictionary(Of String, Double) Implements IExprMAT.dataExpr0

        Public Overrides Function ToString() As String
            Return LocusId
        End Function

        Public Function ToSample() As ExprSamples
            Return New ExprSamples With {
                .locusId = LocusId,
                .Values = dataExpr0.Values.ToArray
            }
        End Function

        ''' <summary>
        ''' General load method.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function LoadMatrix(path As String) As ExprMAT()
            Dim File As DocumentStream.File = DocumentStream.File.Load(path)
            File(Scan0, Scan0) = NameOf(LocusId)
            Dim MAT As ExprMAT() = File.AsDataSource(Of ExprMAT)(False)
            Return MAT
        End Function
    End Class
End Namespace