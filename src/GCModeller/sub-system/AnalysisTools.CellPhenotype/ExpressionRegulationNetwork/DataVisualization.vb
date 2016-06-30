Imports LANS.SystemsBiology.Toolkits.RNA_Seq
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.dataExprMAT
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData

<[PackageNamespace]("Cellphenotype.DynamicsNetwork", Publisher:="xie.guigang@gcmodeller.org")>
Module DataVisualization

    Public Class Edge : Inherits NetworkEdge
        Public Property InteractValue As Double
    End Class

    <ExportAPI("Export.Dynamics", Info:="Export a dynamics cell system network from thje simulation data.")>
    Public Function ExportDynamics(Of T As NetworkEdge)(data As IEnumerable(Of ExprSamples),
                                                        Network As T(),
                                                        <Parameter("Read.Index")>
                                                        ReadIndex As Integer) As Edge()

        Dim dict = data.ToDictionary(Function(obj) obj.locusId,
                                     Function(obj) obj.Values)
        Dim LQuery = (From node As T In Network
                      Let [from] = dict(node.FromNode)(ReadIndex)
                      Let [to] = dict(node.ToNode)(ReadIndex)
                      Where from <> 0 AndAlso [to] <> 0
                      Select New Edge With {
                          .FromNode = node.FromNode,
                          .ToNode = node.ToNode,
                          .InteractionType = node.InteractionType,
                          .Confidence = node.Confidence,
                          .InteractValue = Math.Min([from], [to]) * node.Confidence}).ToArray
        Return LQuery
    End Function

    <ExportAPI("Read.Csv.SimulationResult")>
    Public Function LoadResult(path As String) As ExprSamples()
        Dim Csv As DocumentStream.File = DocumentStream.File.Load(path)
        Return MatrixAPI.ToSamples(Csv, True)
    End Function

    <ExportAPI("Write.Csv.DynamicsNetwork")>
    Public Function WriteDynamics(data As IEnumerable(Of Edge), <Parameter("Path.Save")> SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function
End Module
