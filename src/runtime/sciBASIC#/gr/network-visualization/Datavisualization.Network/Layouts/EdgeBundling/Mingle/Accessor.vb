Imports Microsoft.VisualBasic.Data.GraphTheory.KdTree
Imports stdNum = System.Math

Namespace Layouts.EdgeBundling.Mingle

    Public Class Accessor : Inherits KdNodeAccessor(Of GraphKdNode)

        Public Overrides Function GetDimensions() As String()
            Return {"x", "y", "z", "w"}
        End Function

        Public Overrides Sub setByDimensin(x As GraphKdNode, dimName As String, value As Double)
            Select Case dimName
                Case "x" : x.x = value
                Case "y" : x.y = value
                Case "z" : x.z = value
                Case "w" : x.w = value
                Case Else
                    Throw New InvalidCastException(dimName)
            End Select
        End Sub

        Public Overrides Function metric(a As GraphKdNode, b As GraphKdNode) As Double
            Dim diff0 = a.x - b.x
            Dim diff1 = a.y - b.y
            Dim diff2 = a.z - b.z
            Dim diff3 = a.w - b.w

            Return stdNum.Sqrt(diff0 * diff0 + diff1 * diff1 + diff2 * diff2 + diff3 * diff3)
        End Function

        Public Overrides Function getByDimension(x As GraphKdNode, dimName As String) As Double
            Select Case dimName
                Case "x" : Return x.x
                Case "y" : Return x.y
                Case "z" : Return x.z
                Case "w" : Return x.w
                Case Else
                    Throw New InvalidCastException(dimName)
            End Select
        End Function

        Public Overrides Function nodeIs(a As GraphKdNode, b As GraphKdNode) As Boolean
            Return a Is b
        End Function
    End Class
End Namespace