Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection

Namespace Dynamic
	''' <summary>
	''' Dynamic and binding logic for R lists
	''' </summary>
	Public Class ListDynamicMeta
		Inherits SymbolicExpressionDynamicMeta
        Private Shared ReadOnly IndexerNameType As Type() = New System.Type() {GetType(String)}

		''' <summary>
		''' Creates a new object dealing with the dynamic and binding logic for R lists
		''' </summary>
		''' <param name="parameter">The expression representing this new ListDynamicMeta in the binding process</param>
		''' <param name="list">The runtime value of the GenericVector, that this new ListDynamicMeta represents</param>
		Public Sub New(parameter As System.Linq.Expressions.Expression, list As GenericVector)
			MyBase.New(parameter, list)
		End Sub

		''' <summary>
		''' Returns the enumeration of all dynamic member names.
		''' </summary>
		''' <returns>The list of dynamic member names</returns>
		Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
			Return MyBase.GetDynamicMemberNames().Concat(GetNames())
		End Function

		''' <summary>
		''' Performs the binding of the dynamic get member operation.
		''' </summary>
		''' <param name="binder">
		''' An instance of the System.Dynamic.GetMemberBinder that represents the details of the dynamic operation.
		''' </param>
		''' <returns>The new System.Dynamic.DynamicMetaObject representing the result of the binding.</returns>
		Public Overrides Function BindGetMember(binder As GetMemberBinder) As DynamicMetaObject
			If Not GetNames().Contains(binder.Name) Then
				Return MyBase.BindGetMember(binder)
			End If
			Return BindGetMember(Of GenericVector, SymbolicExpression)(binder, IndexerNameType)
		End Function

		Private Function GetNames() As String()
			Return If(DirectCast(Value, GenericVector).Names, Empty)
		End Function
	End Class
End Namespace
