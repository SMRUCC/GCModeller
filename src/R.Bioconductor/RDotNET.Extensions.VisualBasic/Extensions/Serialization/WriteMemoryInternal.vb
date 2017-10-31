Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Module WriteMemoryInternal

    Public Sub WriteNumeric(Of T As IComparable(Of T))(var$, x As T)
        SyncLock R
            With R
                .call = $"{var} <- {x.ToString};"
            End With
        End SyncLock
    End Sub

    Public Sub WriteString(var$, s$)
        SyncLock R
            With R
                .call = $"{var} <- {Rstring(s)};"
            End With
        End SyncLock
    End Sub

    Public Sub WriteLogical(var$, b As Boolean)
        SyncLock R
            With R
                .call = $"{var} <- {b.ToString.ToUpper};"
            End With
        End SyncLock
    End Sub

    Public Sub WriteNumeric(Of T As IComparable(Of T))(var$, x As IEnumerable(Of T))
        SyncLock R
            With R
                .call = $"{var} <- c({x.JoinBy(", ")});"
            End With
        End SyncLock
    End Sub

    Public Sub WriteString(var$, s As IEnumerable(Of String))
        SyncLock R
            With R
                .call = $"{var} <- c({s.Select(AddressOf Rstring).JoinBy(", ")});"
            End With
        End SyncLock
    End Sub

    Public Sub WriteLogical(var$, b As IEnumerable(Of Boolean))
        SyncLock R
            With R
                .call = $"{var} <- c({b.Select(Function(f) f.ToString.ToUpper).JoinBy(", ")});"
            End With
        End SyncLock
    End Sub
End Module
