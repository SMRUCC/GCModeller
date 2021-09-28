Imports System.Drawing
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class Grid(Of T)

    ReadOnly matrix2D As Dictionary(Of Long, Dictionary(Of Long, GridCell(Of T)))

    Private Sub New(points As IEnumerable(Of GridCell(Of T)))
        matrix2D = points _
            .GroupBy(Function(d) d.index.X) _
            .ToDictionary(Function(d) CLng(d.Key),
                          Function(d)
                              Return d _
                                  .GroupBy(Function(p) p.index.Y) _
                                  .ToDictionary(Function(p) CLng(p.Key),
                                                Function(p)
                                                    Return p.First
                                                End Function)
                          End Function)
    End Sub

    Public Iterator Function EnumerateData() As IEnumerable(Of T)
        For Each row In matrix2D
            For Each col In row.Value
                Yield col.Value.data
            Next
        Next
    End Function

    Public Function GetData(x As Integer, y As Integer) As T
        Dim xkey = CLng(x), ykey = CLng(y)

        If Not matrix2D.ContainsKey(xkey) Then
            Return Nothing
        End If

        If Not matrix2D(xkey).ContainsKey(ykey) Then
            Return Nothing
        End If

        Return matrix2D(xkey)(ykey).data
    End Function

    Public Function Query(x As Integer, y As Integer, gridSize As Integer) As IEnumerable(Of T)
        Return Query(x, y, New Size(gridSize, gridSize))
    End Function

    Public Iterator Function Query(x As Integer, y As Integer, gridSize As Size) As IEnumerable(Of T)
        Dim q As New Value(Of T)

        For i As Integer = x - gridSize.Width To x + gridSize.Width
            For j As Integer = y - gridSize.Height To y + gridSize.Height
                If Not q = GetData(i, j) Is Nothing Then
                    Yield CType(q, T)
                End If
            Next
        Next
    End Function

    Public Shared Function Create(data As IEnumerable(Of T), getX As Func(Of T, Integer), getY As Func(Of T, Integer)) As Grid(Of T)
        Return data _
            .SafeQuery _
            .Select(Function(d)
                        Return New GridCell(Of T)(getX(d), getY(d), d)
                    End Function) _
            .DoCall(Function(vec)
                        Return New Grid(Of T)(vec)
                    End Function)
    End Function

End Class
