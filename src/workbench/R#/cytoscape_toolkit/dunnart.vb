
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
    Public Function graphObject(network As NetworkGraph,
                                Optional colorSet As String = "Paired:c12",
                                Optional group_key As String = "map",
                                Optional fillOpacity As Double = 0.5,
                                Optional lighten As Double = 0.1) As GraphObject

        Return network.FromNetwork(colorSet, group_key, fillOpacity, lighten)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("network_map")>
    Public Function CreateModel(template As NetworkGraph, maps As Pathway(),
                                Optional desc As Boolean = False,
                                Optional colorSet As String = "Paired:c12",
                                Optional fillOpacity As Double = 0.5,
                                Optional lighten As Double = 0.1,
                                Optional isConnected As Boolean = True) As GraphObject

        Return template.CreateModel(
            maps, desc, colorSet, 
            fillOpacity:=fillOpacity, 
            lighten:=lighten, 
            isConnected:=isConnected
        )
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("optmize")>
    Public Function OptmizeGraph(template As NetworkGraph,
                                 Optional optmize_iterations As Integer = 100,
                                 Optional lower_degrees As Integer = 3,
                                 Optional lower_adjcents As Integer = 2) As NetworkGraph

        Return template.OptmizeGraph(optmize_iterations, lower_degrees, lower_adjcents)
    End Function
End Module
