Imports Microsoft.VisualBasic.Linq
Imports System.Xml.Serialization

Namespace ComponentModel.EquaionModel.DefaultTypes

    Public Class CompoundSpecieReference : Implements ICompoundSpecies
        <XmlAttribute> Public Property StoiChiometry As Double Implements ICompoundSpecies.StoiChiometry
        <XmlAttribute> Public Property Identifier As String Implements ICompoundSpecies.Identifier

        Sub New()
        End Sub

        Sub New(x As ICompoundSpecies)
            StoiChiometry = x.StoiChiometry
            Identifier = x.Identifier
        End Sub

        Public Overloads Function Equals(b As ICompoundSpecies, strict As Boolean) As Boolean
            Return Equivalence.Equals(Me, b, strict)
        End Function

        Public Overrides Function ToString() As String
            If StoiChiometry > 1 Then
                Return String.Format("{0} {1}", StoiChiometry, Identifier)
            Else
                Return Identifier
            End If
        End Function
    End Class

    ''' <summary>
    ''' 默认类型的反应表达式的数据结构
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Equation : Inherits Equation(Of CompoundSpecieReference)
        Implements IEquation(Of CompoundSpecieReference)

        Sub New()
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies), right As IEnumerable(Of ICompoundSpecies), canReverse As Boolean)
            Reactants = left.ToArray(Function(x) New CompoundSpecieReference(x))
            Products = right.ToArray(Function(x) New CompoundSpecieReference(x))
            Reversible = canReverse
        End Sub

        Sub New(left As IEnumerable(Of ICompoundSpecies),
                right As IEnumerable(Of ICompoundSpecies),
                idMaps As Dictionary(Of String, String),
                canReverse As Boolean)
            Reactants = left.ToArray(Function(x) New CompoundSpecieReference With {
                                         .Identifier = idMaps(x.Identifier),
                                         .StoiChiometry = x.StoiChiometry})
            Products = right.ToArray(Function(x) New CompoundSpecieReference With {
                                         .Identifier = idMaps(x.Identifier),
                                         .StoiChiometry = x.StoiChiometry})
            Reversible = canReverse
        End Sub

        Protected Overrides Function __equals(a As CompoundSpecieReference, b As CompoundSpecieReference, strict As Boolean) As Object
            Return a.Equals(b, strict)
        End Function
    End Class
End Namespace
