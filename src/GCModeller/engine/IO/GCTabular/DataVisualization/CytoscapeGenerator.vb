#Region "Microsoft.VisualBasic::b077da0bfe1920cfea00f27b8774e61f, engine\IO\GCTabular\DataVisualization\CytoscapeGenerator.vb"

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

    '     Class CytoscapeGenerator
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: CreateNetworkFile, (+4 Overloads) CreateNodeAttributes, (+4 Overloads) CreateNodeInteractions, InternalCreateNodeAttribute, InternalCreateNodeAttributes
    '                   TrimNodeAttributes
    ' 
    '         Sub: AddStringInteractions
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.STRING
Imports SMRUCC.genomics.Data.STRING.SimpleCsv
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.IO
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.XmlFormat

Namespace DataVisualization

    Public Class CytoscapeGenerator

        Dim ModelIo As FileStream.IO.XmlresxLoader

        Sub New(CellXML As CellSystemXmlModel)
            ModelIo = New FileStream.IO.XmlresxLoader(CellXML)
        End Sub

        Sub New(ModelIO As XmlresxLoader)
            Me.ModelIo = ModelIO
        End Sub

        Public Function CreateNetworkFile(ByRef Interactions As DataVisualization.Interactions(), ByRef NodeAttributes As DataVisualization.NodeAttributes()) As Boolean
            Dim List As List(Of DataVisualization.Interactions) = New List(Of Interactions)
            Dim ListOfNodeAttributes As List(Of DataVisualization.NodeAttributes) = New List(Of NodeAttributes)

            Call List.AddRange(CreateNodeInteractions(ModelIo.MetabolismModel.ToArray))
            Call ListOfNodeAttributes.AddRange(InternalCreateNodeAttributes(ModelIo.MetabolitesModel.Values.ToArray))
            Call ListOfNodeAttributes.AddRange(CreateNodeAttributes(ModelIo.MetabolismModel.ToArray))

            If Not ModelIo.STrPModel Is Nothing AndAlso Not ModelIo.TranscriptionModel.IsNullOrEmpty Then
                Call New STrP(ModelIo.STrPModel, ModelIo.TranscriptionModel.ToArray).CreateObjectNetwork(List, ListOfNodeAttributes)
            End If
            If Not ModelIo.ProteinAssembly.IsNullOrEmpty Then Call ListOfNodeAttributes.AddRange(CreateNodeAttributes(ModelIo.ProteinAssembly.Values.ToArray))
            If Not ModelIo.ProteinAssembly.IsNullOrEmpty Then Call List.AddRange(CreateNodeInteractions(ModelIo.ProteinAssembly.Values.ToArray))

            Interactions = List.ToArray
            For i As Integer = 0 To Interactions.Count - 1
                Interactions(i).value = 1
            Next
            NodeAttributes = TrimNodeAttributes(ListOfNodeAttributes)

            Return True
        End Function

        Public Sub AddStringInteractions(stringDB As String, ByRef Interactions As DataVisualization.Interactions(), ByRef NodeAttributes As DataVisualization.NodeAttributes())
            Dim Network = SimpleCsv.Network.Compile(stringDB).Nodes
            Dim ListOfNodeAttributes = NodeAttributes.AsList
            Call ListOfNodeAttributes.AddRange(CreateNodeAttributes(Network))
            NodeAttributes = TrimNodeAttributes(ListOfNodeAttributes)

            Dim InteractionList = Interactions.AsList
            Call InteractionList.AddRange(CreateNodeInteractions(Network))

            Interactions = InteractionList.ToArray
        End Sub

        Public Shared Function TrimNodeAttributes(Nodes As List(Of DataVisualization.NodeAttributes)) As DataVisualization.NodeAttributes()
            Dim IdList As String() = (From item In Nodes Select item.ID Distinct).ToArray
            Dim List As List(Of DataVisualization.NodeAttributes) = New List(Of NodeAttributes)
            For Each Id As String In IdList
                Dim LQuery = (From item In Nodes Where String.Equals(item.ID, Id) Select item.NodeType Distinct).ToArray
                Dim sBuilder As StringBuilder = New StringBuilder(1024)
                For Each strType In LQuery
                    Call sBuilder.Append(strType & "|")
                Next
                Call sBuilder.Remove(sBuilder.Length - 1, 1)
                Call List.Add(New DataVisualization.NodeAttributes With {.ID = Id, .NodeType = sBuilder.ToString})
            Next

            Return List.ToArray
        End Function

#Region "CreateNodeAttributes() As DataVisualization.NodeAttributes()"

        Private Shared Function CreateNodeAttributes(stringNodes As PitrNode()) As DataVisualization.NodeAttributes()
            Dim LQuery = (From item In stringNodes Select New NodeAttributes() {New NodeAttributes With {.ID = item.FromNode, .NodeType = "Protein"}, New NodeAttributes With {.ID = item.ToNode, .NodeType = "Protein"}}).ToArray
            Dim List As List(Of NodeAttributes) = New List(Of NodeAttributes)
            For Each item In LQuery
                Call List.AddRange(item)
            Next
            Return List.ToArray
        End Function

        Private Shared Function InternalCreateNodeAttributes(MetabolitesModel As FileStream.Metabolite()) As DataVisualization.NodeAttributes()
            Dim LQuery = (From item As FileStream.Metabolite
                          In MetabolitesModel.AsParallel
                          Let _createdNode = InternalCreateNodeAttribute(item)
                          Select _createdNode).ToArray
            Return LQuery
        End Function

        Private Shared Function InternalCreateNodeAttribute(Metabolite As FileStream.Metabolite) As DataVisualization.NodeAttributes
            Dim UniqueId As String = Metabolite.Identifier
            Dim NodeType = Metabolite.MetaboliteType
            Dim CommonNames As String = Metabolite.CommonNames.FirstOrDefault
            Return New DataVisualization.NodeAttributes With {.ID = UniqueId, .NodeType = NodeType.ToString, .CommonNames = CommonNames}
        End Function

        Private Shared Function CreateNodeAttributes(MetabolismModel As FileStream.MetabolismFlux()) As DataVisualization.NodeAttributes()
            Dim LQuery = (From item In MetabolismModel Select New DataVisualization.NodeAttributes With {.ID = item.Identifier, .NodeType = "ReactionFlux", .CommonNames = item.CommonName}).AsList
            For Each Flux In MetabolismModel
                If Not Flux.Enzymes.IsNullOrEmpty Then
                    Call LQuery.AddRange((From strId As String In Flux.Enzymes Select New NodeAttributes With {.ID = strId, .NodeType = "Metabolism Enzyme"}))
                End If
            Next
            Return LQuery.ToArray
        End Function

        Private Shared Function CreateNodeAttributes(ProteinAssembly As FileStream.ProteinAssembly()) As DataVisualization.NodeAttributes()
            Dim List As List(Of NodeAttributes) = New List(Of NodeAttributes)
            Dim LQuery = (From item In ProteinAssembly Select CreateNodeAttributes(item, List)).ToArray
            Return List.ToArray
        End Function

        Private Shared Function CreateNodeAttributes(ProteinAssembly As FileStream.ProteinAssembly, List As List(Of NodeAttributes)) As Integer
            Call List.Add(New NodeAttributes With {.ID = ProteinAssembly.ProteinComplexes, .NodeType = "ProteinComplexes"})
            Call List.AddRange((From id As String In ProteinAssembly.ProteinComponents Select New NodeAttributes With {.ID = id, .NodeType = "ProteinComponents"}))
            Return 0
        End Function
#End Region

#Region "CreateNodeInteractions() As DataVisualization.Interactions()"

        Private Shared Function CreateNodeInteractions(stringNodes As PitrNode()) As DataVisualization.Interactions()
            Dim LQuery = (From item In stringNodes Select New Interactions With {.FromNode = item.FromNode, .ToNode = item.ToNode, .Interaction = "Protein Interactions"}).AsList
            Call LQuery.AddRange((From item In stringNodes Select New Interactions With {.FromNode = item.ToNode, .ToNode = item.FromNode, .Interaction = "Protein Interactions"}).ToArray)
            Return LQuery.ToArray
        End Function

        Private Shared Function CreateNodeInteractions(ProteinAssembly As FileStream.ProteinAssembly()) As DataVisualization.Interactions()
            Dim LQuery = (From item In ProteinAssembly.AsParallel Select (From id As String In item.ProteinComponents Select New Interactions With {.FromNode = id, .ToNode = item.ProteinComplexes, .Interaction = "ProteinComplexes Assembly"}).ToArray).ToArray
            Dim List As List(Of Interactions) = New List(Of Interactions)
            For Each item In LQuery
                Call List.AddRange(item)
            Next

            LQuery = (From item In ProteinAssembly.AsParallel Select (From id As String In item.ProteinComponents Select New Interactions With {.ToNode = id, .FromNode = item.ProteinComplexes, .Interaction = "ProteinComplexes Assembly"}).ToArray).ToArray
            For Each item In LQuery
                Call List.AddRange(item)
            Next

            Return List.ToArray
        End Function

        Private Shared Function CreateNodeInteractions(MetabolismModel As FileStream.MetabolismFlux()) As DataVisualization.Interactions()
            Dim List As List(Of DataVisualization.Interactions) = New List(Of Interactions)
            Dim LQuery = (From item In MetabolismModel Select CreateNodeInteractions(item, List)).ToArray
            Return List.ToArray
        End Function

        Private Shared Function CreateNodeInteractions(MetabolismModel As FileStream.MetabolismFlux, List As List(Of DataVisualization.Interactions)) As Integer
            Dim FluxModel = EquationBuilder.CreateObject(Of DefaultTypes.CompoundSpecieReference, DefaultTypes.Equation)(MetabolismModel.Equation)

            If Not MetabolismModel.Enzymes.IsNullOrEmpty Then
                Call List.AddRange((From strId As String In MetabolismModel.Enzymes Select New DataVisualization.Interactions With {.FromNode = strId, .ToNode = MetabolismModel.Identifier, .Interaction = "Enzyme Catalysts"}).ToArray)
            End If

            If MetabolismModel.Reversible Then
                Call List.AddRange((From item In FluxModel.Reactants Select New DataVisualization.Interactions With {.FromNode = MetabolismModel.Identifier, .ToNode = item.ID, .Interaction = "MetabolismFlux Substrates"}).ToArray)
                Call List.AddRange((From item In FluxModel.Products Select New DataVisualization.Interactions With {.FromNode = item.ID, .ToNode = MetabolismModel.Identifier, .Interaction = "MetabolismFlux Substrates"}).ToArray)
                '处于路径搜索的查找方向以及整个网络的连通性考虑，在这里添加冗余数据
                Call List.AddRange((From item In FluxModel.Reactants Select New DataVisualization.Interactions With {.FromNode = item.ID, .ToNode = MetabolismModel.Identifier, .Interaction = "MetabolismFlux Substrates"}).ToArray)
                Call List.AddRange((From item In FluxModel.Products Select New DataVisualization.Interactions With {.FromNode = MetabolismModel.Identifier, .ToNode = item.ID, .Interaction = "MetabolismFlux Substrates"}).ToArray)
            Else
                Call List.AddRange((From item In FluxModel.Reactants Select New DataVisualization.Interactions With {.FromNode = item.ID, .ToNode = MetabolismModel.Identifier, .Interaction = "MetabolismFlux Reactants"}).ToArray)
                Call List.AddRange((From item In FluxModel.Products Select New DataVisualization.Interactions With {.FromNode = MetabolismModel.Identifier, .ToNode = item.ID, .Interaction = "MetabolismFlux Products"}).ToArray)
            End If
            Return 0
        End Function
#End Region
    End Class
End Namespace
