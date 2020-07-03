
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Model.Network.KEGG
Imports SMRUCC.genomics.Model.Network.STRING
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop

''' <summary>
''' protein-protein interaction network model from string-db
''' </summary>
<Package("bioModels.stringdb.ppi")>
Module stringdbPPI

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="stringNetwork"></param>
    ''' <param name="uniprot">``STRING -> uniprot``</param>
    ''' <param name="coordinates"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.network")>
    <RApiReturn(GetType(NetworkGraph))>
    Public Function buildNetworkModel(stringNetwork As InteractExports(), <RRawVectorArgument> uniprot As Object,
                                      Optional coordinates As Coordinates() = Nothing,
                                      Optional env As Environment = Nothing) As Object

        Dim uniprotDb As pipeline = pipeline.TryCreatePipeline(Of entry)(uniprot, env)

        If uniprotDb.isError Then
            Return uniprotDb.getError
        End If

        Dim annotations = uniprotDb.populates(Of entry)(env).StringUniprot
        Dim model As NetworkTables = stringNetwork.BuildModel(uniprot:=annotations, groupValues:=FunctionalNetwork.KOGroupTable)
        Dim graph = model.CreateGraph

        If Not coordinates.IsNullOrEmpty Then
            Return graph.applyLayout(coordinates)
        Else
            Return graph
        End If
    End Function

    <ExportAPI("read.string_interactions")>
    Public Function readNetwork(string_interactions As String) As InteractExports()
        Return string_interactions.LoadTsv(Of InteractExports).ToArray
    End Function

    <ExportAPI("read.coordinates")>
    Public Function readLayout(string_network_coordinates As String) As Coordinates()
        Return string_network_coordinates.LoadTsv(Of Coordinates).ToArray
    End Function
End Module
