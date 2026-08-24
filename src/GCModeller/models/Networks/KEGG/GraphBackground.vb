#Region "Microsoft.VisualBasic::7d9ba40b8c8ad84becb7d963939e5b36, models\Networks\KEGG\GraphBackground.vb"

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


    ' Code Statistics:

    '   Total Lines: 121
    '    Code Lines: 84 (69.42%)
    ' Comment Lines: 18 (14.88%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 19 (15.70%)
    '     File Size: 4.68 KB


    ' Class MapGraphPopulator
    ' 
    ' 
    ' 
    ' Class DefaultMapGraphPopulator
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: CreateGraphModel
    ' 
    ' Module GraphBackground
    ' 
    '     Function: (+2 Overloads) CreateBackground, graphModel
    ' 
    ' /********************************************************************************/

#End Region


Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes

Public MustInherit Class MapGraphPopulator

    Public MustOverride Function CreateGraphModel(map As Map) As NetworkGraph

End Class

Friend Class DefaultMapGraphPopulator : Inherits MapGraphPopulator

    ReadOnly reactions As Dictionary(Of String, Reaction)

    Sub New(reactions As Dictionary(Of String, Reaction))
        Me.reactions = reactions
    End Sub

    Public Overrides Function CreateGraphModel(map As Map) As NetworkGraph
        Return map.graphModel(reactions)
    End Function
End Class

''' <summary>
''' create background network graph model for kegg data
''' </summary>
Public Module GraphBackground

    <Extension>
    Public Iterator Function CreateBackground(pathways As IEnumerable(Of Map), populator As MapGraphPopulator) As IEnumerable(Of NamedValue(Of NetworkGraph))
        For Each map As Map In From pwy In pathways Where Not pwy Is Nothing
            Dim model As NetworkGraph = populator.CreateGraphModel(map)
            Dim referId As String = If(map.EntryId.IsPattern("\d+"), $"map{map.EntryId}", map.EntryId)
            Dim name As String = map.name _
                .Replace("Reference pathway", "") _
                .Trim(" "c, "-"c)

            Yield New NamedValue(Of NetworkGraph) With {
                .Name = referId,
                .Description = name,
                .Value = model
            }
        Next
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateBackground(pathways As IEnumerable(Of Map), reactions As Dictionary(Of String, Reaction)) As IEnumerable(Of NamedValue(Of NetworkGraph))
        Return pathways.CreateBackground(New DefaultMapGraphPopulator(reactions))
    End Function

    ''' <summary>
    ''' convert the kegg map object to the graph model
    ''' </summary>
    ''' <param name="map"></param>
    ''' <param name="reactions"></param>
    ''' <returns>
    ''' the data graph model contains the metabolite item
    ''' which is connected via the reaction liks and a set
    ''' of the single metabolites which is currently no 
    ''' partner connections in this graph model.
    ''' </returns>
    <Extension>
    Friend Function graphModel(map As Map, reactions As Dictionary(Of String, Reaction)) As NetworkGraph
        Dim allShapes As String() = map.shapes.mapdata _
            .Select(Function(a) a.IDVector) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Dim reactionIds As String() = allShapes _
            .Where(Function(id)
                       Return id.IsPattern("R\d{5}")
                   End Function) _
            .ToArray
        Dim model As New NetworkGraph

        ' add connected graph
        ' via the reaction links
        For Each id As String In reactionIds.Where(AddressOf reactions.ContainsKey)
            Dim reaction As Reaction = reactions(id)
            Dim formula As Equation = reaction.ReactionModel

            Call model.CreateNode(id, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "reaction"}
                }
            })

            For Each cid As String In formula.GetMetabolites.Select(Function(c) c.ID)
                If model.GetElementByID(cid) Is Nothing Then
                    Call model.CreateNode(cid, New NodeData)
                End If
            Next

            For Each substrate As CompoundSpecieReference In formula.Reactants
                If model.QueryEdge(substrate.ID, id) Is Nothing Then
                    model.CreateEdge(
                        u:=model.GetElementByID(substrate.ID),
                        v:=model.GetElementByID(id)
                    )
                End If
            Next
        Next

        ' add single node into this graph object
        ' via loop through all shape id
        For Each id As String In allShapes
            If model.GetElementByID(id) Is Nothing Then
                model.CreateNode(id, New NodeData)
            End If
        Next

        Return model
    End Function
End Module
