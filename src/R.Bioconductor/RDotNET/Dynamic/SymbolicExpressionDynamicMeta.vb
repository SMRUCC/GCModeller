Imports System.Dynamic
Imports System.Linq.Expressions
Imports System.Runtime.InteropServices

Namespace Dynamic
    ''' <summary>
    ''' Dynamic and binding logic for S expressions
    ''' </summary>
    Public Class SymbolicExpressionDynamicMeta
        Inherits DynamicMetaObject
        ''' <summary>
        ''' A string array of length zero
        ''' </summary>
        Protected Shared ReadOnly Empty As String() = New String(-1) {}

        ''' <summary>
        ''' Dynamic and binding logic for S expressions
        ''' </summary>
        ''' <param name="parameter">The expression representing this new SymbolicExpressionDynamicMeta in the binding process</param>
        ''' <param name="expression">The runtime value of this SymbolicExpression represented by this new SymbolicExpressionDynamicMeta</param>
        Public Sub New(ByVal parameter As Expressions.Expression, ByVal expression As SymbolicExpression)
            MyBase.New(parameter, BindingRestrictions.Empty, expression)
        End Sub

        ''' <summary>
        ''' Creates the binding of the dynamic get member operation.
        ''' </summary>
        ''' <typeparam name="RType">The type of R object that this dynamic meta object represents</typeparam>
        ''' <typeparam name="BType">The type passed to define the binding restrictions</typeparam>
        ''' <param name="binder">The binder; its name must be one of the names of the R object represented by this meta object</param>
        ''' <param name="indexerNameType"></param>
        ''' <returns></returns>
        Protected Overloads Function BindGetMember(Of RType, BType)(ByVal binder As GetMemberBinder, ByVal indexerNameType As Type()) As DynamicMetaObject
            Dim instance As ConstantExpression = Nothing
            Dim name As ConstantExpression = Nothing
            BuildInstanceAndName(Of RType)(binder, instance, name)
            Dim indexer = GetType(RType).GetProperty("Item", indexerNameType)
            Dim [call] = Expressions.Expression.Property(instance, indexer, name)
            Return CreateDynamicMetaObject(Of BType)([call])
        End Function

        Private Shared Function CreateDynamicMetaObject(Of BType)(ByVal [call] As Expressions.Expression) As DynamicMetaObject
            Return New DynamicMetaObject([call], BindingRestrictions.GetTypeRestriction([call], GetType(BType)))
        End Function

        Private Sub BuildInstanceAndName(Of RType)(ByVal binder As GetMemberBinder, <Out> ByRef instance As ConstantExpression, <Out> ByRef name As ConstantExpression)
            instance = Expressions.Expression.Constant(Value, GetType(RType))
            name = Expressions.Expression.Constant(binder.Name, GetType(String))
        End Sub

        ''' <summary>
        ''' Returns the enumeration of all dynamic member names.
        ''' </summary>
        ''' <returns>The list of dynamic member names</returns>
        Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
            Return MyBase.GetDynamicMemberNames().Concat(GetAttributeNames())
        End Function

        ''' <summary>
        ''' Performs the binding of the dynamic get member operation.
        ''' </summary>
        ''' <param name="binder">
        ''' An instance of the System.Dynamic.GetMemberBinder that represents the details of the dynamic operation.
        ''' </param>
        ''' <returns>The new System.Dynamic.DynamicMetaObject representing the result of the binding.</returns>
        Public Overrides Function BindGetMember(ByVal binder As GetMemberBinder) As DynamicMetaObject
            If Not GetAttributeNames().Contains(binder.Name) Then
                Return MyBase.BindGetMember(binder)
            End If

            Dim instance As ConstantExpression = Nothing
            Dim name As ConstantExpression = Nothing
            BuildInstanceAndName(Of SymbolicExpression)(binder, instance, name)
            Dim getAttribute = GetType(SymbolicExpression).GetMethod("GetAttribute")
            Dim [call] = Expressions.Expression.Call(instance, getAttribute, name)
            Return CreateDynamicMetaObject(Of SymbolicExpression)([call])
        End Function

        Private Function GetAttributeNames() As String()
            Return If(CType(Value, SymbolicExpression).GetAttributeNames(), Empty)
        End Function
    End Class
End Namespace
