Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Class KGMLRender

    ReadOnly kgml As pathway
    ReadOnly graph As NetworkGraph

    Sub New(kgml As pathway)
        Me.kgml = kgml
        Me.graph = GetNetwork(kgml)
    End Sub

    Public Shared Function GetNetwork(pathway As pathway) As NetworkGraph

    End Function

End Class
