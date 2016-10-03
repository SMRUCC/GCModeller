#Region "Microsoft.VisualBasic::9081cf687f1ca7ae8b45eff96d26b684, ..\R.Bioconductor\RDotNET\R.NET\DataFrame.vb"

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

Imports RDotNet.Diagnostics
Imports RDotNet.Dynamic
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Dynamic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Permissions

Imports RDotNET.REngineExtension
Imports RDotNET.SymbolicExpressionExtension

''' <summary>
''' A data frame.
''' </summary>
<DebuggerDisplay("ColumnCount = {ColumnCount}; RowCount = {RowCount}; RObjectType = {Type}")> _
<DebuggerTypeProxy(GetType(DataFrameDebugView))> _
<SecurityPermission(SecurityAction.Demand, Flags := SecurityPermissionFlag.UnmanagedCode)> _
Public Class DataFrame
	Inherits Vector(Of DynamicVector)
	Private Const RRowNamesSymbolName As String = "R_RowNamesSymbol"

	''' <summary>
	''' Creates a new instance.
	''' </summary>
	''' <param name="engine">The <see cref="REngine"/> handling this instance.</param>
	''' <param name="coerced">The pointer to a data frame.</param>
	Protected Friend Sub New(engine As REngine, coerced As IntPtr)
		MyBase.New(engine, coerced)
	End Sub

	''' <summary>
	''' Gets or sets the column at the specified index as a vector.
	''' </summary>
	''' <param name="columnIndex">The zero-based index of the column to get or set.</param>
	''' <returns>The column at the specified index.</returns>
	Public Overrides Default Property Item(columnIndex As Integer) As DynamicVector
		Get
			If columnIndex < 0 OrElse Length <= columnIndex Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				Return GetColumn(columnIndex)
			End Using
		End Get
		Set
			If columnIndex < 0 OrElse Length <= columnIndex Then
				Throw New ArgumentOutOfRangeException()
			End If
			Using New ProtectedPointer(Me)
				SetColumn(columnIndex, value)
			End Using
		End Set
	End Property

	''' <summary>
	''' Gets an array of the columns of this R data frame object
	''' </summary>
	''' <returns></returns>
	Protected Overrides Function GetArrayFast() As DynamicVector()
		Dim res = New DynamicVector(Me.Length - 1) {}
		For i As Integer = 0 To res.Length - 1
			res(i) = GetColumn(i)
		Next
		Return res
	End Function

	Private Function GetColumn(columnIndex As Integer) As DynamicVector
		Dim offset As Integer = GetOffset(columnIndex)
		Dim pointer As IntPtr = Marshal.ReadIntPtr(DataPointer, offset)
		Return New DynamicVector(Engine, pointer)
	End Function

	Private Sub SetColumn(columnIndex As Integer, value As DynamicVector)
		Dim offset As Integer = GetOffset(columnIndex)
		Marshal.WriteIntPtr(DataPointer, offset, (If(value, Engine.NilValue)).DangerousGetHandle())
	End Sub

	''' <summary>
	''' Efficient initialisation of R vector values from an array representation in the CLR
	''' </summary>
	Protected Overrides Sub SetVectorDirect(values As DynamicVector())
		For i As Integer = 0 To values.Length - 1
			SetColumn(i, values(i))
		Next
	End Sub

	''' <summary>
	''' Gets or sets the element at the specified indexes.
	''' </summary>
	''' <param name="rowIndex">The row index.</param>
	''' <param name="columnIndex">The column index.</param>
	''' <returns>The element.</returns>
    Default Public Overloads Property Item(rowIndex As Integer, columnIndex As Integer) As Object
        Get
            Dim column As DynamicVector = Me(columnIndex)
            Return column(rowIndex)
        End Get
        Set(value As Object)
            Dim column As DynamicVector = Me(columnIndex)
            column(rowIndex) = Value
        End Set
    End Property

	''' <summary>
	''' Gets or sets the element at the specified index and name.
	''' </summary>
	''' <param name="rowIndex">The row index.</param>
	''' <param name="columnName">The column name.</param>
	''' <returns>The element.</returns>
    Default Public Overloads Property Item(rowIndex As Integer, columnName As String) As Object
        Get
            Dim column As DynamicVector = Me(columnName)
            Return column(rowIndex)
        End Get
        Set(value As Object)
            Dim column As DynamicVector = Me(columnName)
            column(rowIndex) = Value
        End Set
    End Property

	''' <summary>
	''' Gets or sets the element at the specified names.
	''' </summary>
	''' <param name="rowName">The row name.</param>
	''' <param name="columnName">The column name.</param>
	''' <returns>The element.</returns>
    Default Public Overloads Property Item(rowName As String, columnName As String) As Object
        Get
            Dim column As DynamicVector = Me(columnName)
            Return column(rowName)
        End Get
        Set(value As Object)
            Dim column As DynamicVector = Me(columnName)
            column(rowName) = Value
        End Set
    End Property

	''' <summary>
	''' Gets the number of data sets.
	''' </summary>
	Public ReadOnly Property RowCount() As Integer
		Get
			Return If(ColumnCount = 0, 0, Me(0).Length)
		End Get
	End Property

	''' <summary>
	''' Gets the number of kinds of data.
	''' </summary>
	Public ReadOnly Property ColumnCount() As Integer
		Get
			Return Length
		End Get
	End Property

	''' <summary>
	''' Gets the names of rows.
	''' </summary>
	Public ReadOnly Property RowNames() As String()
		Get
			Dim rowNamesSymbol As SymbolicExpression = Engine.GetPredefinedSymbol(RRowNamesSymbolName)
			Dim rowNames__1 As SymbolicExpression = GetAttribute(rowNamesSymbol)
			If rowNames__1 Is Nothing Then
				Return Nothing
			End If
			Dim rowNamesVector As CharacterVector = rowNames__1.AsCharacter()
			If rowNamesVector Is Nothing Then
				Return Nothing
			End If

			Dim length As Integer = rowNamesVector.Length
			Dim result = New String(length - 1) {}
			rowNamesVector.CopyTo(result, length)
			Return result
		End Get
	End Property

	''' <summary>
	''' Gets the names of columns.
	''' </summary>
	Public ReadOnly Property ColumnNames() As String()
		Get
			Return Names
		End Get
	End Property

	''' <summary>
	''' Gets the data size of each element in this vector, i.e. the offset in memory between elements.
	''' </summary>
	Protected Overrides ReadOnly Property DataSize() As Integer
		Get
			Return Marshal.SizeOf(GetType(IntPtr))
		End Get
	End Property

	''' <summary>
	''' Gets the row at the specified index.
	''' </summary>
	''' <param name="rowIndex">The index.</param>
	''' <returns>The row.</returns>
	Public Function GetRow(rowIndex As Integer) As DataFrameRow
		Return New DataFrameRow(Me, rowIndex)
	End Function

	''' <summary>
	''' Gets the row at the specified index mapping a specified class.
	''' </summary>
	''' <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
	''' <returns>The row.</returns>
	Public Function GetRow(Of TRow As {Class, New})(rowIndex As Integer) As TRow
		Dim rowType = GetType(TRow)
		Dim attribute = DirectCast(rowType.GetCustomAttributes(GetType(DataFrameRowAttribute), False).[Single](), DataFrameRowAttribute)
		If attribute Is Nothing Then
			Throw New ArgumentException("DataFrameRowAttribute is required.")
		End If
		Dim row = GetRow(rowIndex)
		Return attribute.Convert(Of TRow)(row)
	End Function

	''' <summary>
	''' Enumerates all the rows in the data frame.
	''' </summary>
	''' <returns>The collection of the rows.</returns>
    Public Iterator Function GetRows() As IEnumerable(Of DataFrameRow)
        Dim rowCount__1 As Integer = RowCount
        For rowIndex As Integer = 0 To rowCount__1 - 1
            Yield GetRow(rowIndex)
        Next
    End Function

	''' <summary>
	''' Enumerates all the rows in the data frame mapping a specified class.
	''' </summary>
	''' <typeparam name="TRow">The row type with <see cref="DataFrameRowAttribute"/>.</typeparam>
	''' <returns>The collection of the rows.</returns>
    Public Iterator Function GetRows(Of TRow As {Class, New})() As IEnumerable(Of TRow)
        Dim rowType = GetType(TRow)
        Dim attribute = DirectCast(rowType.GetCustomAttributes(GetType(DataFrameRowAttribute), False).[Single](), DataFrameRowAttribute)
        If attribute Is Nothing Then
            Throw New ArgumentException("DataFrameRowAttribute is required.")
        End If
        Dim rowCount__1 As Integer = RowCount
        For rowIndex As Integer = 0 To rowCount__1 - 1
            Dim row = GetRow(rowIndex)
            Yield attribute.Convert(Of TRow)(row)
        Next
    End Function

	''' <summary>
	''' returns a new DataFrameDynamicMeta for this DataFrame
	''' </summary>
	''' <param name="parameter"></param>
	''' <returns></returns>
	Public Overrides Function GetMetaObject(parameter As System.Linq.Expressions.Expression) As DynamicMetaObject
		Return New DataFrameDynamicMeta(parameter, Me)
	End Function
End Class
