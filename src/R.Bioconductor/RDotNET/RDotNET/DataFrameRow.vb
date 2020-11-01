#Region "Microsoft.VisualBasic::168f0c0974ed4c0a7246933a51e268bf, RDotNET\RDotNET\DataFrameRow.vb"

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

    ' Class DataFrameRow
    ' 
    '     Properties: DataFrame, (+2 Overloads) Item, RowIndex
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetDynamicMemberNames, GetInnerValue, TryGetMember, TrySetMember
    ' 
    '     Sub: SetInnerValue
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic
Imports System.Dynamic

''' <summary>
''' A data frame row.
''' </summary>
Public Class DataFrameRow
	Inherits DynamicObject
	Private frame As DataFrame
	Private m_rowIndex As Integer

	''' <summary>
	''' Creates a new object representing a data frame row
	''' </summary>
	''' <param name="frame">R Data frame</param>
	''' <param name="rowIndex">zero-based row index</param>
	Public Sub New(frame As DataFrame, rowIndex As Integer)
		Me.frame = frame
		Me.m_rowIndex = rowIndex
	End Sub

	''' <summary>
	''' Gets and sets the value at the specified column.
	''' </summary>
	''' <param name="index">The zero-based column index.</param>
	''' <returns>The value.</returns>
	Public Default Property Item(index As Integer) As Object
		Get
			Dim column As DynamicVector = DataFrame(index)
			Return column(RowIndex)
		End Get
		Set
			Dim column As DynamicVector = DataFrame(index)
			column(RowIndex) = value
		End Set
	End Property

	''' <summary>
	''' Gets the inner representation of the value; an integer if the column is a factor
	''' </summary>
	''' <param name="index"></param>
	''' <returns></returns>
	Friend Function GetInnerValue(index As Integer) As Object
		Dim column As DynamicVector = DataFrame(index)
		If column.IsFactor() Then
			Return column.AsInteger()(RowIndex)
		Else
			Return column(RowIndex)
		End If
	End Function

	''' <summary>
	''' Sets the inner representation of the value; an integer if the column is a factor
	''' </summary>
	''' <param name="index"></param>
	''' <param name="value"></param>
	Friend Sub SetInnerValue(index As Integer, value As Object)
		Dim column As DynamicVector = DataFrame(index)
		If column.IsFactor() Then
			column.AsInteger()(RowIndex) = CInt(value)
		Else
			column(RowIndex) = value
		End If
	End Sub

	''' <summary>
	''' Gets and sets the value at the specified column.
	''' </summary>
	''' <param name="name">The column name.</param>
	''' <returns>The value.</returns>
	Public Default Property Item(name As String) As Object
		Get
			Dim column As DynamicVector = DataFrame(name)
			Return column(RowIndex)
		End Get
		Set
			Dim column As DynamicVector = DataFrame(name)
			column(RowIndex) = value
		End Set
	End Property

	''' <summary>
	''' Gets the data frame containing this row.
	''' </summary>
	Public ReadOnly Property DataFrame() As DataFrame
		Get
			Return Me.frame
		End Get
	End Property

	''' <summary>
	''' Gets the index of this row.
	''' </summary>
	Public ReadOnly Property RowIndex() As Integer
		Get
			Return Me.m_rowIndex
		End Get
	End Property

	''' <summary>
	''' Gets the column names of the data frame.
	''' </summary>
	''' <returns></returns>
	Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
		Return DataFrame.ColumnNames
	End Function

	''' <summary>
	''' Try to get a member to a specified value
	''' </summary>
	''' <param name="binder">Dynamic get member operation at the call site; Binder whose name should be one of the data frame column name</param>
	''' <param name="result">The value of the member</param>
	''' <returns>false if setting failed</returns>
	Public Overrides Function TryGetMember(binder As GetMemberBinder, ByRef result As Object) As Boolean
		Dim columnNames As String() = DataFrame.ColumnNames
		If columnNames Is Nothing OrElse Array.IndexOf(columnNames, binder.Name) < 0 Then
			result = Nothing
			Return False
		End If
		result = Me(binder.Name)
		Return True
	End Function

	''' <summary>
	''' Try to set a member to a specified value
	''' </summary>
	''' <param name="binder">Dynamic set member operation at the call site; Binder whose name should be one of the data frame column name</param>
	''' <param name="value">The value to set</param>
	''' <returns>false if setting failed</returns>
	Public Overrides Function TrySetMember(binder As SetMemberBinder, value As Object) As Boolean
		Dim columnNames As String() = DataFrame.ColumnNames
		If columnNames Is Nothing OrElse Array.IndexOf(columnNames, binder.Name) < 0 Then
			Return False
		End If
		Me(binder.Name) = value
		Return True
	End Function
End Class

