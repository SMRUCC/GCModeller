#Region "Microsoft.VisualBasic::09efe2bb93bf1b325680852317013b9c, visualize\Cytoscape\Cytoscape.App\NetworkModel\KEGG\PfsNET\PfsNET.vb"

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

    '     Class PfsNET
    ' 
    '         Properties: [Class], Category, Description, Flag, n
    '                     PhenotypePair, PValue, SignificantGeneObjects, Statistics, SubNET_Vector
    '                     UniqueId, Vectors, weight2, weights
    ' 
    '         Function: ToString
    ' 
    '     Module NetworkGenerator
    ' 
    '         Function: __assignPathways, CreateMetaCycNetwork, CreateNetwork, CreatePathwayNetwork, EnzymeCatalyst
    '                   GenerateAnnotations, ReadPfsNET, SaveNetwork
    ' 
    '         Class Edge
    ' 
    ' 
    ' 
    '         Class Node
    ' 
    '             Properties: InPathways
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Namespace NetworkModel.PfsNET

    Public Class PfsNET

        Public Property Category As String
        Public Property [Class] As String

        ''' <summary>
        ''' 使用结果文件名来表示
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PhenotypePair As String
        Public Property UniqueId As String
        Public Property Description As String
        Public Property n As Integer
        <Column("flag")> Public Property Flag As Boolean
        Public Property Statistics As Double
        Public Property PValue As Double
        Public Property SignificantGeneObjects As String()

        Public Property weights As Double()
        Public Property weight2 As Double()
        Public Property SubNET_Vector As Double()
        Public Property Vectors As String()

        Public Overrides Function ToString() As String
            Return String.Format("{0}  [{1}]", UniqueId, String.Join(", ", SignificantGeneObjects))
        End Function
    End Class

    <Package("Cytoscape.PfsNET", Publisher:="xie.guigang@live.com")>
    Public Module NetworkGenerator

        <ExportAPI("Creates.Network.Basaical")>
        Public Function CreateNetwork(data As IEnumerable(Of PfsNET)) As KeyValuePair(Of NetworkEdge(), FileStream.Node())
            Dim edgesBuffer As New List(Of NetworkEdge)
            Dim nodesBuffer As New List(Of FileStream.Node)

            For Each SubNET In data
                edgesBuffer += From strId As String
                               In SubNET.SignificantGeneObjects
                               Select New NetworkEdge With {
                                   .value = 1,
                                   .FromNode = strId,
                                   .ToNode = SubNET.UniqueId,
                                   .Interaction = SubNET.PhenotypePair
                               }
                nodesBuffer += From strId As String
                               In SubNET.SignificantGeneObjects
                               Select New FileStream.Node With {
                                   .NodeType = "Significant Gene",
                                   .ID = strId
                               }
                nodesBuffer += New FileStream.Node With {
                    .ID = SubNET.UniqueId,
                    .NodeType = "Significant Pathway"
                }
            Next

            Return New KeyValuePair(Of NetworkEdge(), FileStream.Node())(edgesBuffer.ToArray, nodesBuffer.ToArray)
        End Function

        <ExportAPI("Write.PfsNet")>
        Public Function SaveNetwork(network As KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node()), saveDIR As String) As Boolean
            Dim filePath As String = String.Format("{0}/pfsnet.edges.csv", saveDIR)
            Call network.Key.SaveTo(filePath, False)
            Call (From strLine As String In filePath.ReadAllLines Select strLine Distinct).FlushAllLines(filePath)

            filePath = String.Format("{0}/pfsnet.nodes.csv", saveDIR)
            Call network.Value.SaveTo(filePath, False)
            Call (From strLine As String In filePath.ReadAllLines Select strLine Distinct).ToArray.FlushAllLines(filePath)

            Return True
        End Function

        <ExportAPI("Read.PfsNet")>
        Public Function ReadPfsNET(path As String) As PfsNET()
            Return path.LoadCsv(Of PfsNET)(False).ToArray
        End Function

        <ExportAPI("PfsNet.Annotation")>
        Public Function GenerateAnnotations(pfsnet As PfsNET(), saveDIR As String) As Boolean
            Dim GAF As New List(Of GAF)
            Dim Go As New GO_OBO

            Call Go.Save(String.Format("{0}/pfsnet.go_annotation.txt", saveDIR))
            Call GeneOntology.GAF.Save(GAF.ToArray, String.Format("{0}/pfsnet.gaf.txt", saveDIR))

            Return True
        End Function

        <ExportAPI("PfsNet.Metacyc")>
        Public Function CreateMetaCycNetwork(PfsNET As PfsNET(), MetaCyc As DatabaseLoadder) As KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())
            Dim PathwayIdlist As String() = (From item In PfsNET Select item.UniqueId Distinct).ToArray
            Dim Pathways = MetaCyc.GetPathways.Takes(PathwayIdlist)
            Dim ReactionList As List(Of Slots.Reaction) = New List(Of Slots.Reaction)
            Dim Reactions = MetaCyc.GetReactions
            For Each Pathway In Pathways
                Call ReactionList.AddRange(Reactions.Takes(Pathway.ReactionList))
            Next
            ReactionList = (From rxn In ReactionList Select rxn Distinct).AsList

            Dim EdgeList As List(Of NetworkModel.Edge) = New List(Of NetworkModel.Edge)
            Dim NodeList As List(Of NetworkModel.Node) = New List(Of NetworkModel.Node)
            Dim Network = CreatePathwayNetwork(ReactionList)

            If Not Network.Key.IsNullOrEmpty Then Call EdgeList.AddRange(Network.Key)
            If Not Network.Value.IsNullOrEmpty Then Call NodeList.AddRange(Network.Value)

            Network = EnzymeCatalyst(PfsNET, MetaCyc, ReactionList)
            If Not Network.Key.IsNullOrEmpty Then Call EdgeList.AddRange(Network.Key)
            If Not Network.Value.IsNullOrEmpty Then Call NodeList.AddRange(Network.Value)

            Return New KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())(EdgeList.ToArray, __assignPathways(PfsNET, NodeList.ToArray, MetaCyc))
        End Function

        Private Function __assignPathways(pfsNET As PfsNET(), Nodes As NetworkModel.Node(), MetaCyc As DatabaseLoadder) As NetworkModel.Node()
            Dim Reactions = (From item In Nodes Where String.Equals(item.NodeType, "Reaction") Select item).ToArray
            Dim PathwayList = MetaCyc.GetPathways

            For i As Integer = 0 To Reactions.Count - 1
                Dim Reaction = Reactions(i)
                Dim Pathways = (From pathway In PathwayList Where pathway.ReactionList.IndexOf(Reaction.ID) > -1 Select pathway.Identifier).ToArray
                Reaction.InPathways = Pathways
            Next

            Dim Genes = (From item In Nodes Where String.Equals(item.NodeType, "GeneObject") Select item).ToArray
            For i As Integer = 0 To Genes.Count - 1
                Dim Gene = Genes(i)
                Dim Pathways = (From item In pfsNET Where Array.IndexOf(item.SignificantGeneObjects, Gene.ID) > -1 Select item.UniqueId Distinct).ToArray
                Gene.InPathways = Pathways
            Next

            Return Nodes
        End Function

        Private Function EnzymeCatalyst(PfsNET As PfsNET(), MetaCyc As DatabaseLoadder, ReactionList As List(Of Slots.Reaction)) As KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())
            Dim EdgeList As List(Of NetworkModel.Edge) = New List(Of NetworkModel.Edge)
            Dim NodeList As List(Of NetworkModel.Node) = New List(Of NetworkModel.Node)
            Dim GeneList As List(Of String) = New List(Of String)
            For Each subNet In PfsNET
                Call GeneList.AddRange(subNet.SignificantGeneObjects)
            Next
            GeneList = (From strId As String In GeneList Select strId Distinct).AsList

            Call NodeList.AddRange((From GeneId As String In GeneList Select New NetworkModel.Node With {.NodeType = "GeneObject", .ID = GeneId}).ToArray)

            Dim CatalystsList As Dictionary(Of String, String())
            Using op = New SMRUCC.genomics.Assembly.MetaCyc.Schema.PathwayBrief.AssignGene(MetaCyc)
                CatalystsList = op.Performance
                CatalystsList = op.ConvertId(CatalystsList)
            End Using

            For Each Reaction In ReactionList
                If Not CatalystsList.ContainsKey(Reaction.Identifier) Then
                    Continue For
                End If

                Dim GeneIdlist = CatalystsList(Reaction.Identifier)
                Call EdgeList.AddRange((From strId As String In GeneIdlist Select New NetworkModel.Edge With {.FromNode = strId, .ToNode = Reaction.Identifier, .Interaction = "Enzyme_Catalyst"}).ToArray)
            Next

            Return New KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())(EdgeList.ToArray, NodeList.ToArray)
        End Function

        Private Function CreatePathwayNetwork(ReactionList As List(Of Slots.Reaction)) As KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())
            Dim EdgeList As New List(Of NetworkModel.Edge)
            Dim NodeList As List(Of NetworkModel.Node) =
                LinqAPI.MakeList(Of NetworkModel.Node) <=
 _
                From r As Slots.Reaction
                In ReactionList
                Select New NetworkModel.Node With {
                    .ID = r.Identifier,
                    .NodeType = "Reaction"
                }

            For Each reaction As Slots.Reaction In ReactionList
                For Each Substrate As String In reaction.Substrates
                    Dim LQuery = From r As Slots.Reaction
                                 In ReactionList
                                 Let c = r.GetCoefficient(Substrate)
                                 Where Not reaction.Equals(r) AndAlso c <> 0
                                 Select New With {
                                     .Coefficient = c,
                                     .Reaction = r.Identifier,
                                     .Reversible = r.Reversible
                                 }

                    Dim coefficient% = reaction.GetCoefficient(Substrate)
                    Dim itType As String = ""

                    For Each connected In LQuery
                        If connected.Coefficient > 0 Then  '在右端
                            If coefficient > 0 Then
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = reaction.Identifier, .ToNode = Substrate, .Interaction = "Confluence"})
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = connected.Reaction, .ToNode = Substrate, .Interaction = "Confluence"})
                                Call NodeList.Add(New NetworkModel.Node With {.ID = Substrate, .NodeType = "Metabolite"})
                            Else
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = connected.Reaction, .ToNode = reaction.Identifier, .Interaction = "Metabolite_Flux_Flow"})
                            End If
                        Else '在左端
                            If coefficient > 0 Then
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = reaction.Identifier, .ToNode = connected.Reaction, .Interaction = "Metabolite_Flux_Flow"})
                            Else
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = Substrate, .ToNode = connected.Reaction, .Interaction = "Diffluence"})
                                Call EdgeList.Add(New NetworkModel.Edge With {.FromNode = Substrate, .ToNode = reaction.Identifier, .Interaction = "Diffluence"})
                                Call NodeList.Add(New NetworkModel.Node With {.ID = Substrate, .NodeType = "Metabolite"})
                            End If
                        End If
                    Next
                Next
            Next

            Return New KeyValuePair(Of NetworkModel.Edge(), NetworkModel.Node())(EdgeList.ToArray, NodeList.ToArray)
        End Function
    End Module

    Namespace NetworkModel

        Public Class Edge : Inherits NetworkEdge

        End Class

        Public Class Node : Inherits FileStream.Node
            Public Property InPathways As String()

        End Class
    End Namespace
End Namespace
