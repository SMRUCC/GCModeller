#Region "Microsoft.VisualBasic::2a29e928f8a774ca74c37f1c836f7f40, engine\GCModeller\DataVisualization.DynamicMap\DynamicMapBuilder.vb"

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

    '     Class DynamicMapBuilder
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: BuildCentralDogmaRegulations, BuildEnzymaticFluxModel, BuildMetabolismFluxModel, ExportDynamicsCellNetwork, ExportExpressionRegulationNetwork
    '                   ExportMetabolismNetwork, Expressing, GetRegulators
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports System.Text
Imports SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.CentralDogmaInstance

Namespace DataVisualization.DynamicMap

    ''' <summary>
    ''' 构建动态的虚拟细胞内的网络
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DynamicMapBuilder : Inherits IMapBuilder

        Sub New(ObjectModel As EngineSystem.ObjectModels.SubSystem.CellSystem)
            Call MyBase.New(ObjectModel)
        End Sub

        Public Function ExportMetabolismNetwork() As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim ChunkBuffer As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Dim NodeList As List(Of Component) = New List(Of Component)

            Dim LQuery = (From Flux As EngineSystem.ObjectModels.Module.MetabolismFlux
                          In MyBase._cellSystemODM.Metabolism.DelegateSystem.NetworkComponents.AsParallel
                          Where Flux.TypeId = EngineSystem.ObjectModels.ObjectModel.TypeIds.MetabolismFlux
                          Select BuildMetabolismFluxModel(Flux)).ToArray

            For Each Line In LQuery
                If Not Line.Value.IsNullOrEmpty Then
                    Call ChunkBuffer.AddRange(Line.Value)
                End If
                If Not Line.Key.IsNullOrEmpty Then
                    Call NodeList.AddRange(Line.Key)
                End If
            Next
            LQuery = (From EnzymaticFlux In MyBase._cellSystemODM.Metabolism.DelegateSystem.NetworkComponents.AsParallel
                      Where EnzymaticFlux.TypeId = EngineSystem.ObjectModels.ObjectModel.TypeIds.EnzymaticFlux
                      Select BuildEnzymaticFluxModel(DirectCast(EnzymaticFlux, EngineSystem.ObjectModels.Module.EnzymaticFlux))).ToArray
            For Each Line In LQuery
                If Not Line.Value.IsNullOrEmpty Then
                    Call ChunkBuffer.AddRange(Line.Value)
                End If
                If Not Line.Key.IsNullOrEmpty Then
                    Call NodeList.AddRange(Line.Key)
                End If
            Next

            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = NodeList.ToArray, .Value = ChunkBuffer.ToArray}
        End Function

        Public Function ExportDynamicsCellNetwork() As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim Temp As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim NetworkEdges As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Dim NetworkNodes As List(Of Component) = New List(Of Component)

            Temp = ExportExpressionRegulationNetwork()
            If Not Temp.Value.IsNullOrEmpty Then Call NetworkEdges.AddRange(Temp.Value)
            If Not Temp.Key.IsNullOrEmpty Then Call NetworkNodes.AddRange(Temp.Key)

            Temp = ExportMetabolismNetwork()
            If Not Temp.Value.IsNullOrEmpty Then Call NetworkEdges.AddRange(Temp.Value)
            If Not Temp.Key.IsNullOrEmpty Then Call NetworkNodes.AddRange(Temp.Key)

            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = NetworkNodes.ToArray, .Value = NetworkEdges.ToArray}
        End Function

        Public Function ExportExpressionRegulationNetwork() As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim LQuery = (From CentralDogmaInstance In _cellSystemODM.ExpressionRegulationNetwork.NetworkComponents Select BuildCentralDogmaRegulations(CentralDogmaInstance)).ToArray
            Dim Network As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Dim Components As List(Of Component) = New List(Of Component)

            For Each Line In LQuery
                If Not Line.Key.IsNullOrEmpty Then
                    Call Components.AddRange(Line.Key)
                End If
                If Not Line.Value.IsNullOrEmpty Then
                    Call Network.AddRange(Line.Value)
                End If
            Next

            Dim ComponentsId As String() = (From cp In Components Select cp.ID Distinct).ToArray
            Dim NewComponentsList As List(Of Component) = New List(Of Component)

            For Each Id As String In ComponentsId
                Dim Items = Components.Takes(uniqueId:=Id, strict:=False)
                Items = (From item In Items.AsParallel Where item.Quantity > 0 Select item).ToArray

                If Items.IsNullOrEmpty Then
                    Continue For
                End If

                Dim Type As String = String.Join("; ", (From item In Items Select item.NodeType Distinct).ToArray)

                Call NewComponentsList.Add(New Component With {.ID = Id, .NodeType = Type, .Quantity = Items.First.Quantity})
            Next

            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = NewComponentsList.ToArray, .Value = (From item In Network Where item.value > 0 Select item).ToArray}
        End Function

        Private Function BuildCentralDogmaRegulations(CentrolDogma As CentralDogma) As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim Transcripts = (From Transcript In CentrolDogma.Transcripts Select New Component With {.ID = Transcript._TranscriptModelBase.Template, .NodeType = "Protein", .Quantity = Transcript.Quantity}).ToArray
            Dim Regulators = (From Regulator In CentrolDogma.get_Regulators Select New Component With {.ID = Regulator.Identifier, .NodeType = "TranscriptRegulator", .Quantity = Regulator.Quantity}).ToArray
            Dim Regulations = (From Regulator In CentrolDogma.get_Regulators Select New ComponentInteraction With {.value = CentrolDogma.ExpressionActivity, .FromNode = Regulator.Identifier, .InteractionType = InteractionTypes.TranscriptionRegulation, .ToNode = CentrolDogma.Identifier}).ToArray
            Dim TranscriptUnit = (From Transcript In CentrolDogma.Transcripts Select New ComponentInteraction With {.value = CentrolDogma.FluxValue, .FromNode = CentrolDogma.Identifier, .ToNode = Transcript.Identifier, .InteractionType = InteractionTypes.OperonConsists}).ToArray

            Dim Components As List(Of Component) = New List(Of Component)
            Call Components.AddRange(Transcripts)
            Call Components.AddRange(Regulators)
            Call Components.Add(New Component With {.ID = CentrolDogma.Identifier, .NodeType = "TranscriptUnit", .Quantity = CentrolDogma.FluxValue})
            Dim Interactions As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Call Interactions.AddRange(Regulations)
            Call Interactions.AddRange(TranscriptUnit)
            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = Components.ToArray, .Value = Interactions.ToArray}
        End Function

        Public Function BuildEnzymaticFluxModel(Model As EngineSystem.ObjectModels.Module.EnzymaticFlux) As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim ChunkList = BuildMetabolismFluxModel(Model)
            If ChunkList Is Nothing Then
                Return New KeyValuePairObject(Of Component(), ComponentInteraction())
            End If

            Dim Network = If(ChunkList.Value.IsNullOrEmpty, New List(Of ComponentInteraction), ChunkList.Value.AsList)
            Dim Components = If(ChunkList.Key.IsNullOrEmpty, New List(Of Component), ChunkList.Key.AsList)

            If Network.IsNullOrEmpty AndAlso Components.IsNullOrEmpty Then
                Return New KeyValuePairObject(Of Component(), ComponentInteraction())
            End If

            Call Network.AddRange((From Enzyme In Model.Enzymes
                                   Where Enzyme.Quantity > 1
                                   Select New ComponentInteraction With
                                          {
                                              .FromNode = Enzyme.Identifier,
                                              .ToNode = Model.Identifier,
                                              .InteractionType = InteractionTypes.ReactionEnzyme,
                                              .value = Enzyme.EnzymeActivity}).ToArray)
            Call Components.AddRange((From Enzyme In Model.Enzymes
                                      Where Enzyme.Quantity > 1
                                      Select New Component With
                                             {
                                                 .ID = Enzyme.Identifier, .Quantity = Enzyme.Quantity, .NodeType = "MetabolismEnzyme"}))

            Dim GetRegulators = (From Enzyme In Model.Enzymes
                                 Let Regulators = Me.GetRegulators(GeneId:=Enzyme.Identifier)
                                 Select EnzymeId = Enzyme, Regulators = Regulators).ToArray

            For Each Line In GetRegulators
                If Line.Regulators.IsNullOrEmpty Then
                    Continue For
                End If

                Call Components.AddRange((From Regulator
                                          In Line.Regulators
                                          Select New Component With
                                                 {
                                                     .ID = Regulator.Identifier,
                                                     .Quantity = Regulator.Quantity,
                                                     .NodeType = "TranscriptionRegulator"}).ToArray)
                Call Network.AddRange((From Regulator
                                       In Line.Regulators
                                       Select New ComponentInteraction With
                                              {
                                                  .value = Regulator.Quantity,
                                                  .FromNode = Regulator.Identifier,
                                                  .ToNode = Line.EnzymeId.Identifier,
                                                  .InteractionType = InteractionTypes.TranscriptionRegulation}))
            Next

            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = Components.ToArray, .Value = Network.ToArray}
        End Function

        Public Function BuildMetabolismFluxModel(Model As EngineSystem.ObjectModels.Module.MetabolismFlux) As KeyValuePairObject(Of Component(), ComponentInteraction())
            Dim ChunkList As List(Of ComponentInteraction) = New List(Of ComponentInteraction)
            Dim NodeList As List(Of Component) = New List(Of Component)

            If (Model.FluxValue > 0 AndAlso Model.FluxValue < Model.UPPER_BOUND * 0.25) OrElse (Model.FluxValue < 0 AndAlso Model.FluxValue > Model.LOWER_BOUND * 0.25) Then  '假若反映过成是不可逆的并且值接近于0，则认为网络中的代谢物节点没有连接
                Return New KeyValuePairObject(Of Component(), ComponentInteraction())
            End If

            If Model.FluxValue >= 0 Then
                Call ChunkList.AddRange((From Reactant As EngineSystem.ObjectModels.Module.EquationModel.CompoundSpecieReference
                                         In Model._Reactants
                                         Select New ComponentInteraction With
                                                {
                                                    .FromNode = Reactant.Identifier, .ToNode = Model.Identifier,
                                                    .InteractionType = InteractionTypes.ReactionReactants,
                                                    .value = Model.FluxValue * Reactant.Stoichiometry}).ToArray)
                Call ChunkList.AddRange((From Product As EngineSystem.ObjectModels.Module.EquationModel.CompoundSpecieReference
                                         In Model._Products
                                         Select New ComponentInteraction With
                                                {
                                                    .ToNode = Product.Identifier, .FromNode = Model.Identifier,
                                                    .InteractionType = InteractionTypes.ReactionProducts,
                                                    .value = Model.FluxValue * Product.Stoichiometry}).ToArray)
            Else
                Call ChunkList.AddRange((From Reactant As EngineSystem.ObjectModels.Module.EquationModel.CompoundSpecieReference
                                         In Model._Reactants
                                         Select New ComponentInteraction With
                                                {
                                                    .ToNode = Reactant.Identifier, .FromNode = Model.Identifier,
                                                    .InteractionType = InteractionTypes.ReactionProducts,
                                                    .value = Math.Abs(Model.FluxValue) * Reactant.Stoichiometry}).ToArray)
                Call ChunkList.AddRange((From Product As EngineSystem.ObjectModels.Module.EquationModel.CompoundSpecieReference
                                         In Model._Products
                                         Select New ComponentInteraction With
                                                {
                                                    .FromNode = Product.Identifier, .ToNode = Model.Identifier,
                                                    .InteractionType = InteractionTypes.ReactionReactants,
                                                    .value = Math.Abs(Model.FluxValue) * Product.Stoichiometry}).ToArray)
            End If

            Call NodeList.AddRange((From item In Model._Reactants Select New Component With {.ID = item.Identifier, .Quantity = item.EntityCompound.DataSource.value, .NodeType = "Metabolite"}).ToArray)
            Call NodeList.AddRange((From item In Model._Products Select New Component With {.ID = item.Identifier, .Quantity = item.EntityCompound.DataSource.value, .NodeType = "Metabolite"}).ToArray)
            Call NodeList.Add(New Component With {.ID = Model.Identifier, .Quantity = Model.FluxValue, .NodeType = "MetabolismFlux"})

            Return New KeyValuePairObject(Of Component(), ComponentInteraction()) With {.Key = NodeList.ToArray, .Value = ChunkList.ToArray}
        End Function

        Public Function GetRegulators(GeneId As String) As EngineSystem.ObjectModels.Entity.Regulator(Of Transcription)()
            Dim Cds = Me._cellSystemODM.ExpressionRegulationNetwork.NetworkComponents
            Dim LQuery = (From Cd As CentralDogma In Cds Where Expressing(Cd, GeneId) Select Cd).ToArray  '获取表达调控对象
            Dim ChunkBuffer = (From Line As CentralDogma
                               In LQuery
                               Let Regulators = Line.get_Regulators
                               Select Regulators).ToArray.ToVector

            Return (From Regulator In ChunkBuffer Where Regulator.Quantity >= 1 Select Regulator).ToArray
        End Function

        ''' <summary>
        ''' 判断当前的中心法则实例是否表达该目标基因
        ''' </summary>
        ''' <param name="CentralDogma"></param>
        ''' <param name="GeneId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Expressing(CentralDogma As EngineSystem.ObjectModels.Module.CentralDogmaInstance.CentralDogma, GeneId As String) As Boolean
            Return (From Transcript In CentralDogma.Transcripts Where String.Equals(Transcript._TranscriptModelBase.Template, GeneId) Select Transcript).ToArray.Length > 0
        End Function
    End Class
End Namespace
