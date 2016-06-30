Imports System.Diagnostics
Imports System.Linq

Imports RDotNET.SymbolicExpressionExtension
Imports RDotNET.REngineExtension

Namespace Diagnostics
	<DebuggerDisplay("{Display,nq}")> _
	Friend Class DataFrameColumnDisplay
		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly data As DataFrame

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Private ReadOnly columnIndex As Integer

		Public Sub New(data As DataFrame, columnIndex As Integer)
			Me.data = data
			Me.columnIndex = columnIndex
		End Sub

		<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)> _
		Public ReadOnly Property Value() As Object()
			Get
				Dim column = Me.data(Me.columnIndex)
				Return If(column.IsFactor(), column.AsFactor().GetFactors(), column.ToArray())
			End Get
		End Property

		<DebuggerBrowsable(DebuggerBrowsableState.Never)> _
		Public ReadOnly Property Display() As String
			Get
				Dim column = Me.data(Me.columnIndex)
				Dim names = Me.data.ColumnNames
				If names Is Nothing OrElse names(Me.columnIndex) Is Nothing Then
					Return [String].Format("NA ({0})", column.Type)
				Else
					Return [String].Format("""{0}"" ({1})", names(Me.columnIndex), column.Type)
				End If
			End Get
		End Property
	End Class
End Namespace
