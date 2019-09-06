#Region "Microsoft.VisualBasic::1ee509ce7ae0c0315964a19e13ea28a6, RDotNET\RDotNET\Dynamic\ListDynamicMeta.vb"

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

    ' 	Class ListDynamicMeta
    ' 
    ' 	    Constructor: (+1 Overloads) Sub New
    ' 	    Function: BindGetMember, GetDynamicMemberNames, GetNames
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq

Namespace Dynamic
	''' <summary>
	''' Dynamic and binding logic for R lists
	''' </summary>
	Public Class ListDynamicMeta
		Inherits SymbolicExpressionDynamicMeta
		Private Shared ReadOnly IndexerNameType As Type() =  {GetType(String)}

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

