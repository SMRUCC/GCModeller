#Region "Microsoft.VisualBasic::ba2d5ab6ef7a9f3e2cce6ef5987ece54, RDotNET\RDotNET\Dynamic\SymbolicExpressionDynamicMeta.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' 	Class SymbolicExpressionDynamicMeta
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 
    ' 	    Function: (+2 Overloads) BindGetMember, CreateDynamicMetaObject, GetAttributeNames, GetDynamicMemberNames
    ' 
    ' 	    Sub: BuildInstanceAndName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection

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
		Public Sub New(parameter As System.Linq.Expressions.Expression, expression As SymbolicExpression)
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
		Protected Overloads Function BindGetMember(Of RType, BType)(binder As GetMemberBinder, indexerNameType As Type()) As DynamicMetaObject
            Dim instance As ConstantExpression = Nothing
            Dim name As ConstantExpression = Nothing
            BuildInstanceAndName(Of RType)(binder, instance, name)
			Dim indexer As PropertyInfo = GetType(RType).GetProperty("Item", indexerNameType)
			Dim [call] As IndexExpression = System.Linq.Expressions.Expression.[Property](instance, indexer, name)
			Return CreateDynamicMetaObject(Of BType)([call])
		End Function

		Private Shared Function CreateDynamicMetaObject(Of BType)([call] As System.Linq.Expressions.Expression) As DynamicMetaObject
			Return New DynamicMetaObject([call], BindingRestrictions.GetTypeRestriction([call], GetType(BType)))
		End Function

		Private Sub BuildInstanceAndName(Of RType)(binder As GetMemberBinder, ByRef instance As ConstantExpression, ByRef name As ConstantExpression)
			instance = System.Linq.Expressions.Expression.Constant(Value, GetType(RType))
			name = System.Linq.Expressions.Expression.Constant(binder.Name, GetType(String))
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
		Public Overrides Function BindGetMember(binder As GetMemberBinder) As DynamicMetaObject
			If Not GetAttributeNames().Contains(binder.Name) Then
				Return MyBase.BindGetMember(binder)
			End If

            Dim instance As ConstantExpression = Nothing
            Dim name As ConstantExpression = Nothing
            BuildInstanceAndName(Of SymbolicExpression)(binder, instance, name)
			Dim getAttribute As MethodInfo = GetType(SymbolicExpression).GetMethod("GetAttribute")
			Dim [call] As MethodCallExpression = System.Linq.Expressions.Expression.[Call](instance, getAttribute, name)
			Return CreateDynamicMetaObject(Of SymbolicExpression)([call])
		End Function

		Private Function GetAttributeNames() As String()
			Return If(DirectCast(Value, SymbolicExpression).GetAttributeNames(), Empty)
		End Function
	End Class
End Namespace

