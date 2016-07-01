Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.EquationModel
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module
Imports Microsoft.VisualBasic.DataVisualization
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network.FileStream
Imports Microsoft.VisualBasic

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