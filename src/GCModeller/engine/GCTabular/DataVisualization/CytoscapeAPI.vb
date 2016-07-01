Imports System.Text
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular
Imports LANS.SystemsBiology
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.XmlFormat
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DataVisualization.Network

Namespace DataVisualization

    <PackageNamespace("Cytoscape.GCModeller.NetVisual")>
    Public Module CytoscapeAPI

        <ExportAPI("cytoscape_generator.create")>
        Public Function CreateObject(Model As CellSystemXmlModel) As CytoscapeGenerator
            Return New CytoscapeGenerator(Model)
        End Function

        <ExportAPI("cytoscape_generator.create_from_loader")>
        Public Function CreateObject(Loader As XmlresxLoader) As CytoscapeGenerator
            Return New CytoscapeGenerator(Loader)
        End Function

        <ExportAPI("create_network")>
        Public Function CreateNetwork([operator] As CytoscapeGenerator) As NetModel
            Dim Interactions As DataVisualization.Interactions() = Nothing, NodeAttributes As DataVisualization.NodeAttributes() = Nothing
            Call [operator].CreateNetworkFile(Interactions, NodeAttributes)
            Return New NetModel(Interactions, NodeAttributes)
        End Function

        <ExportAPI("get.network")>
        Public Function GetInteractions(network As NetModel) As Interactions()
            Return network.Edges
        End Function

        <ExportAPI("write.csv.network")>
        Public Function SaveNetwork(network As NetModel, outDIR As String) As Boolean
            Return network.Save(outDIR, Encodings.ASCII)
        End Function

        <ExportAPI("create.path_finder")>
        Public Function CreateFindPath(Network As Interactions()) As PathFinder(Of Interactions)
            Return New PathFinder(Of Interactions)(Network)
        End Function

        <ExportAPI("find.paths")>
        Public Function FindAllPath([operator] As PathFinder(Of Interactions), start As String, ends As String) As KeyValuePair(Of Integer, Interactions())()
            Return [operator].FindAllPath(start, ends)
        End Function

        <ExportAPI("save.paths")>
        Public Function SaveResult(path As KeyValuePair(Of Integer, Interactions())(), saveto As String) As Boolean
            Dim CsvData As DocumentStream.File = New DocumentStream.File

            For Each line In path
                CsvData += New String() {String.Format("Path contains {0} nodes", line.Key)}

                For Each item As Interactions In line.Value
                    CsvData += New String() {item.FromNode, item.ToNode, item.InteractionType}
                Next
                CsvData += New String() {"</>"}
            Next

            Return CsvData.Save(saveto, False)
        End Function
    End Module
End Namespace

