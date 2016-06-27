Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization

Namespace ComponentModel.EquaionModel

    Public Class Reaction(Of T As ICompoundSpecies) : Inherits Equation(Of T)

        ReadOnly _equals As Func(Of T, T, Boolean, Boolean)

        Sub New(equals As Func(Of T, T, Boolean, Boolean))
            _equals = equals
        End Sub

        Protected Overrides Function __equals(a As T, b As T, strict As Boolean) As Object
            Return _equals(a, b, strict)
        End Function
    End Class
End Namespace