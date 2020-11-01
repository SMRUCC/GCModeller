Imports System.Diagnostics

Namespace Diagnostics
    <DebuggerDisplay("{Display,nq}")>
    Friend Class DataFrameColumnDisplay
        <DebuggerBrowsable(DebuggerBrowsableState.Never)>
        Private ReadOnly data As DataFrame
        <DebuggerBrowsable(DebuggerBrowsableState.Never)>
        Private ReadOnly columnIndex As Integer

        Public Sub New(ByVal data As DataFrame, ByVal columnIndex As Integer)
            Me.data = data
            Me.columnIndex = columnIndex
        End Sub

        <DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>
        Public ReadOnly Property Value As Object()
            Get
                Dim column = data(columnIndex)
                Return If(column.IsFactor(), column.AsFactor().GetFactors(), column.ToArray())
            End Get
        End Property

        <DebuggerBrowsable(DebuggerBrowsableState.Never)>
        Public ReadOnly Property Display As String
            Get
                Dim column = data(columnIndex)
                Dim names = data.ColumnNames

                If names Is Nothing OrElse Equals(names(columnIndex), Nothing) Then
                    Return String.Format("NA ({0})", column.Type)
                Else
                    Return String.Format("""{0}"" ({1})", names(columnIndex), column.Type)
                End If
            End Get
        End Property
    End Class
End Namespace
