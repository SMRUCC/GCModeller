
Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree

Namespace KNN.KDTreeMethod

    Public Class KDAccessor : Inherits KdNodeAccessor(Of KDPoint)

        Public Overrides Sub setByDimensin(x As KDPoint, dimName As String, value As Double)
            Throw New NotImplementedException()
        End Sub

        Public Overrides Function GetDimensions() As String()
            Throw New NotImplementedException()
        End Function

        Public Overrides Function metric(a As KDPoint, b As KDPoint) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function getByDimension(x As KDPoint, dimName As String) As Double
            Throw New NotImplementedException()
        End Function

        Public Overrides Function nodeIs(a As KDPoint, b As KDPoint) As Boolean
            Throw New NotImplementedException()
        End Function

        Public Overrides Function activate() As KDPoint
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace