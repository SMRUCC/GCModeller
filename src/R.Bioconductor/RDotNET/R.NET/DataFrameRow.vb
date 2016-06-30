Imports System.Collections.Generic
Imports System.Dynamic

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

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
