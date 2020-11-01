Imports System
Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq

Namespace Dynamic
    ''' <summary>
    ''' Dynamic and binding logic for R data frames
    ''' </summary>
    Public Class DataFrameDynamicMeta
        Inherits SymbolicExpressionDynamicMeta

        Private Shared ReadOnly IndexerNameType As Type() = {GetType(String)}

        ''' <summary>
        ''' Creates a new object dealing with the dynamic and binding logic for R data frames
        ''' </summary>
        ''' <param name="parameter">The expression representing this new DataFrameDynamicMeta in the binding process</param>
        ''' <param name="frame">The runtime value of the DataFrame, that this new DataFrameDynamicMeta represents</param>
        Public Sub New(ByVal parameter As Expressions.Expression, ByVal frame As DataFrame)
            MyBase.New(parameter, frame)
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
        Public Overrides Function BindGetMember(ByVal binder As GetMemberBinder) As DynamicMetaObject
            If Not GetNames().Contains(binder.Name) Then
                Return MyBase.BindGetMember(binder)
            End If

            Return BindGetMember(Of DataFrame, DynamicVector)(binder, IndexerNameType)
        End Function

        Private Function GetNames() As String()
            Return If(CType(Value, DataFrame).ColumnNames, Empty)
        End Function
    End Class
End Namespace
