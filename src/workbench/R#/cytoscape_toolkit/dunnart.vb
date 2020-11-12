
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Model.Network.KEGG.Dunnart

<Package("dunnart")>
Module dunnart

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("as.graphObj")>
    Public Function graphObject(network As NetworkGraph, Optional colorSet As String = "Paired:c12", Optional group_key As String = "map") As GraphObject
        Return network.FromNetwork(colorSet, group_key)
    End Function

    <ExportAPI("network_map")>
    Public Function CreateModel(template As NetworkGraph, maps As Pathway(),
                                Optional desc As Boolean = False,
                                Optional colorSet As String = "Paired:c12") As GraphObject
        Return template.CreateModel(maps, desc, colorSet)
    End Function
End Module
