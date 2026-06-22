Public Class LatentSymbol

    Public Property [class] As String
    Public Property latent As String
    Public Property manifest_id As String

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

End Class
