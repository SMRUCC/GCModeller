Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory

Public Class LatentSymbol

    Public Property [class] As String
    Public Property latent As String
    Public Property manifest_id As String

    Public Overrides Function ToString() As String
        Return $"[{[class]}:{latent}] {manifest_id}"
    End Function

    Public Shared Iterator Function MakeLatents(symbols As IEnumerable(Of LatentSymbol)) As IEnumerable(Of LatentDefinition)
        For Each cls_group As IGrouping(Of String, LatentSymbol) In symbols.GroupBy(Function(s) s.class)
            For Each latent As IGrouping(Of String, LatentSymbol) In cls_group.GroupBy(Function(s) s.latent)
                Yield New LatentDefinition(
                    name:=$"{cls_group.Key}:{latent.Key}",
                    manifest:=From s As LatentSymbol
                              In latent
                              Select s.manifest_id
                )
            Next
        Next
    End Function

    Public Shared Iterator Function MakeFullPath(symbols As IEnumerable(Of LatentSymbol), from As String(), [to] As String()) As IEnumerable(Of SparseGraph.Edge)
        Dim class_group As Dictionary(Of String, String()) = symbols _
            .GroupBy(Function(s) s.class) _
            .ToDictionary(Function(s) s.Key,
                          Function(s)
                              Return LatentSymbols(s)
                          End Function)

        For i As Integer = 0 To from.Length - 1
            Dim listFrom = class_group(from(i))
            Dim listTo = class_group([to](i))

            For Each u In listFrom
                For Each v In listTo
                    If u <> v Then
                        Yield New SparseGraph.Edge(u, v)
                    End If
                Next
            Next
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Shared Function LatentSymbols(g As IGrouping(Of String, LatentSymbol)) As String()
        Return MakeLatents(g).Select(Function(s) s.varName).Distinct.ToArray
    End Function

End Class
