Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class Environment

    Public Property Space As Spot()()()

    Dim itr As i32

    Public Sub Tick()
        Call Tick(++itr)
    End Sub

    Private Sub Tick(iteration As Integer)
        For Each row In Space
            For Each col In row
                For Each spot In col
                    Call spot.Tick(iteration)
                Next
            Next
        Next
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetAllCells() As IEnumerable(Of VirtualCella)
        Return Space.IteratesALL.IteratesALL.SelectMany(Function(s) s.cells)
    End Function

End Class
