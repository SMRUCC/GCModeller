Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class KGMLRender

    ReadOnly kgml As pathway
    ReadOnly graph As NetworkGraph

    ReadOnly entryIndex As New Dictionary(Of entry)

    Sub New(kgml As pathway)
        Me.entryIndex = kgml.entries.SafeQuery.ToDictionary
        Me.kgml = kgml
        Me.graph = GetNetwork(kgml)
    End Sub

    Public Shared Function GetNetwork(pathway As pathway) As NetworkGraph
        Dim g As New NetworkGraph

        For Each entry As entry In pathway.entries.SafeQuery

        Next

        Return g
    End Function

End Class
