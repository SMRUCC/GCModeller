#Region "Microsoft.VisualBasic::eddabb5d3f368c8aa2460d4dfbde8130, ..\R.Bioconductor\RDotNET\R.NET\Dynamic\DataFrameDynamicMeta.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Collections.Generic
Imports System.Dynamic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection

Namespace Dynamic
	''' <summary>
	''' Dynamic and binding logic for R data frames
	''' </summary>
	Public Class DataFrameDynamicMeta
		Inherits SymbolicExpressionDynamicMeta
        Private Shared ReadOnly IndexerNameType As Type() = New System.Type() {GetType(String)}

		''' <summary>
		''' Creates a new object dealing with the dynamic and binding logic for R data frames
		''' </summary>
		''' <param name="parameter">The expression representing this new DataFrameDynamicMeta in the binding process</param>
		''' <param name="frame">The runtime value of the DataFrame, that this new DataFrameDynamicMeta represents</param>
		Public Sub New(parameter As System.Linq.Expressions.Expression, frame As DataFrame)
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
		Public Overrides Function BindGetMember(binder As GetMemberBinder) As DynamicMetaObject
			If Not GetNames().Contains(binder.Name) Then
				Return MyBase.BindGetMember(binder)
			End If
			Return BindGetMember(Of DataFrame, DynamicVector)(binder, IndexerNameType)
		End Function

		Private Function GetNames() As String()
			Return If(DirectCast(Value, DataFrame).ColumnNames, Empty)
		End Function
	End Class
End Namespace
