#Region "Microsoft.VisualBasic::5177ada67154875153550fd8f921d905, visualize\Cytoscape\Cytoscape.App\NetworkModel\SBMLrFBA.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Module SBMLrFBA
    ' 
    '         Function: __flux2Edges, __flux2Node, CreateNetwork, LoadFBAResult
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.SBML.Level2
Imports SMRUCC.genomics.Model.SBML.Level2.Elements
Imports SMRUCC.genomics.Model.SBML.Components
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace NetworkModel

    <Package("NET.SBML.rFBA")>
    Public Module SBMLrFBA

        <ExportAPI("FBA_OUT.Load")>
        Public Function LoadFBAResult(path As String) As FBA_OUTPUT.TabularOUT()
            Return path.LoadCsv(Of FBA_OUTPUT.TabularOUT).ToArray
        End Function

        <ExportAPI("NET.Generate")>
        Public Function CreateNetwork(model As XmlFile, flux As IEnumerable(Of FBA_OUTPUT.TabularOUT)) As NetworkTables
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
                                    Function(xx) xx.species)).IteratesALL.Distinct.ToArray
            Dim nodes = allCompounds.Select(
                Function(x) New Node With {
                    .ID = x,
                    .NodeType = "Metabolite"}).ToArray
            Dim fluxNodes As Node() = nZ.Select(Function(x) __flux2Node(x, fluxValue))
            Dim edges As NetworkEdge() = nZ.Select(AddressOf __flux2Edges).ToVector
            Return New NetworkTables With {
                .Edges = edges,
                .Nodes = nodes.Join(fluxNodes).ToArray
            }
        End Function

        Private Function __flux2Edges(flux As Reaction) As NetworkEdge()
            Dim from As NetworkEdge() = flux.Reactants.Select(
                Function(x) New NetworkEdge With {
                    .FromNode = x.species,
                    .Interaction = "Reactant",
                    .ToNode = flux.id}).ToArray
            Dim toEdges As NetworkEdge() = flux.Products.Select(
                Function(x) New NetworkEdge With {
                    .FromNode = flux.id,
                    .ToNode = x.species,
                    .Interaction = "Product"}).ToArray
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
                .ID = flux.id,
                .NodeType = "Flux",
                .Properties = meta
            }
            Return node
        End Function
    End Module
End Namespace
