#Region "Microsoft.VisualBasic::4930484f4c984d58e24830c74cad2c5e, engine\GCModeller\DataVisualization.DynamicMap\StaticMapBuilder.vb"

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

    '     Class StaticMapBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: __refToNode, BuildEnzymaticFluxModel, BuildMetabolismFluxModel, BuildMetabolismNetwork, BuildThread
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.EquationModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem

Namespace DataVisualization.DynamicMap

    Public Class StaticMapBuilder : Inherits IMapBuilder

        Sub New(ObjectModel As CellSystem)
            Call MyBase.New(ObjectModel)
        End Sub

        Public Function BuildThread() As NetworkEdge()

        End Function

        Public Shared Function BuildEnzymaticFluxModel(Model As EnzymaticFlux) As List(Of ComponentInteraction)
            Dim enzymes = (From Enzyme In Model.Enzymes
                           Select New ComponentInteraction With {
                               .FromNode = Enzyme.Identifier,
                               .ToNode = Model.Identifier,
                               .InteractionType = InteractionTypes.ReactionEnzyme})
            Return BuildMetabolismFluxModel(Model).Join(enzymes)
        End Function

        Public Shared Function BuildMetabolismFluxModel(model As MetabolismFlux) As List(Of ComponentInteraction)
            Dim reactants = (From reactant In model._Reactants Select __refToNode(reactant, InteractionTypes.ReactionReactants, model))
            Dim products = (From product In model._Products Select __refToNode(product, InteractionTypes.ReactionProducts, model))
            Return reactants.Join(products)
        End Function

        Private Shared Function __refToNode(obj As CompoundSpecieReference, rel As InteractionTypes, model As MetabolismFlux) As ComponentInteraction
            Return New ComponentInteraction With {
                .FromNode = obj.Identifier,
                .ToNode = model.Identifier,
                .InteractionType = InteractionTypes.ReactionReactants
            }
        End Function

        Public Shared Function BuildMetabolismNetwork(DelegateSystem As DelegateSystem) As ComponentInteraction()
            Dim ChunkBuffer As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Dim LQuery = (From Flux In DelegateSystem.NetworkComponents
                          Where Flux.TypeId = EngineSystem.ObjectModels.ObjectModel.TypeIds.MetabolismFlux
                          Select BuildMetabolismFluxModel(Flux)).ToArray
            For Each Line In LQuery
                Call ChunkBuffer.AddRange(Line)
            Next
            LQuery = (From EnzymaticFlux In DelegateSystem.NetworkComponents
                      Where EnzymaticFlux.TypeId = EngineSystem.ObjectModels.ObjectModel.TypeIds.EnzymaticFlux
                      Select BuildEnzymaticFluxModel(DirectCast(EnzymaticFlux, EnzymaticFlux))).ToArray
            For Each Line In LQuery
                Call ChunkBuffer.AddRange(Line)
            Next

            Return ChunkBuffer.ToArray
        End Function
    End Class
End Namespace
