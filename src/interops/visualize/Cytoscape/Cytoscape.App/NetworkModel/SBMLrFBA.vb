Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports SMRUCC.genomics.Assembly.SBML.Level2
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc
Imports SMRUCC.genomics.Assembly.SBML.Components
Imports Microsoft.VisualBasic.Language

Namespace NetworkModel

    <PackageNamespace("NET.SBML.rFBA")>
    Public Module SBMLrFBA

        <ExportAPI("FBA_OUT.Load")>
        Public Function LoadFBAResult(path As String) As FBA_OUTPUT.TabularOUT()
            Return path.LoadCsv(Of FBA_OUTPUT.TabularOUT).ToArray
        End Function

        <ExportAPI("NET.Generate")>
        Public Function CreateNetwork(model As XmlFile, flux As IEnumerable(Of FBA_OUTPUT.TabularOUT)) As Network
            Dim ZEROS As String() =
                LinqAPI.Exec(Of String) <= From x As FBA_OUTPUT.TabularOUT
                                           In flux
                                           Where x.Flux = 0R
                                           Select x.Rxn     ' 移除流量为零的过程
            Dim nZ As Reaction() =
                LinqAPI.Exec(Of Reaction) <= From x As Reaction
                                             In model.Model.listOfReactions
                                             Where Array.IndexOf(ZEROS, x.id) = -1  ' 得到所有非零的过程
                                             Select x
            Dim fluxValue As Dictionary(Of String, Double) =
                flux.ToDictionary(Function(x) x.Rxn,
                                  Function(x) x.Flux)
            Dim allCompounds = (From x As Reaction
                                In nZ
                                Select x.GetMetabolites.Select(
                                    Function(xx) xx.species)).MatrixAsIterator.Distinct.ToArray
            Dim nodes = allCompounds.ToArray(
                Function(x) New Node With {
                    .Identifier = x,
                    .NodeType = "Metabolite"})
            Dim fluxNodes As Node() = nZ.ToArray(Function(x) __flux2Node(x, fluxValue))
            Dim edges As NetworkEdge() = nZ.Select(AddressOf __flux2Edges).MatrixToVector
            Return New Network With {
                .Edges = edges,
                .Nodes = nodes.Join(fluxNodes).ToArray
            }
        End Function

        Private Function __flux2Edges(flux As Reaction) As NetworkEdge()
            Dim from As NetworkEdge() = flux.Reactants.ToArray(
                Function(x) New NetworkEdge With {
                    .FromNode = x.species,
                    .InteractionType = "Reactant",
                    .ToNode = flux.id})
            Dim toEdges As NetworkEdge() = flux.Products.ToArray(
                Function(x) New NetworkEdge With {
                    .FromNode = flux.id,
                    .ToNode = x.species,
                    .InteractionType = "Product"})
            Return from.Join(toEdges).ToArray
        End Function

        Private Function __flux2Node(flux As Reaction, value As Dictionary(Of String, Double)) As Node
            Dim prop As New FluxPropReader(flux.Notes)
            Dim meta As New Dictionary(Of String, String)

            Call meta.Add("Reversible", CStr(flux.reversible))
            Call meta.Add("Flux", value(flux.id))

            For Each x As [Property] In prop
                Call meta.Add(x.Name, x.value)
            Next

            Dim node As New Node With {
                .Identifier = flux.id,
                .NodeType = "Flux",
                .Properties = meta
            }
            Return node
        End Function
    End Module
End Namespace