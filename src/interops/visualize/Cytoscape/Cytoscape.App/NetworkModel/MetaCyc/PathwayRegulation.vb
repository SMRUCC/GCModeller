#Region "Microsoft.VisualBasic::4c5ac99c9f878865e4e9734eb9cc336c, visualize\Cytoscape\Cytoscape.App\NetworkModel\MetaCyc\PathwayRegulation.vb"

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

    '     Class PathwayRegulation
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Exists, Regulates
    ' 
    '         Sub: AnalysisMetaPathwayRegulations
    '         Class PathwayRegulator
    ' 
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite

Namespace NetworkModel

    Public Class PathwayRegulation : Inherits Pathways

        Sub New(MetaCyc As DatabaseLoadder)
            Call MyBase.New(MetaCyc)
        End Sub

        Public Sub AnalysisMetaPathwayRegulations(ExportDir As String, RegulationProfiles As MatchedResult())
            Dim Edges As NetworkEdge() = Nothing, Nodes As Pathways.Pathway() = Nothing
            Call Export(Edges, Nodes)

            Call Console.WriteLine("Generate TF <--> pathway regulations...")
            Dim TFCollection As String() = (From Line In RegulationProfiles Select Line.TF Distinct Order By TF Ascending).ToArray
            Dim LQuery = (From Tf As String In TFCollection
                          Let Regulation = (From Line In RegulationProfiles Where String.Equals(Tf, Line.TF) Select Line).ToArray
                          Let Get_TFRegulatedGenes = Function() As String()
                                                         Dim _ChunkList As List(Of String) = New List(Of String)
                                                         For Each Line In Regulation
                                                             Call _ChunkList.AddRange(Line.OperonGeneIds)
                                                         Next
                                                         Return (From strId As String In _ChunkList Select strId Distinct Order By strId).ToArray
                                                     End Function
                          Let TFRegulatedGenes As String() = Get_TFRegulatedGenes()
                          Select New With {.TF = Tf, .RegulatedPathways = (From item In Nodes Where item.SuperPathway = False AndAlso Regulates(TFRegulatedGenes, item) Select item).ToArray}).ToArray

            Dim Network = New List(Of FileStream.NetworkEdge)
            Dim NodeList As List(Of PathwayRegulator) = New List(Of PathwayRegulator)

            Call Console.WriteLine("Start to create network visualization model...")

            For Each Line In LQuery
                Dim Regulations = (From Pathway In Line.RegulatedPathways
                                   Select New FileStream.NetworkEdge With {
                                       .FromNode = Line.TF,
                                       .ToNode = Pathway.Identifier,
                                       .Interaction = "Regulates"}).ToArray

                If Not Regulations.IsNullOrEmpty Then
                    Call Network.AddRange(Regulations)
                    Call NodeList.Add(New PathwayRegulator With {.ID = Line.TF})

                    Dim ChunkTemp As New List(Of NetworkEdge)
                    For Each Node In Regulations
                        If Not Exists(ChunkTemp, Node) Then
                            Call ChunkTemp.Add(Node)
                        End If
                    Next
                    Dim RegulationNodes As New List(Of FileStream.Node)
                    For Each Item In ChunkTemp
                        If RegulationNodes.GetItem(Item.ToNode) Is Nothing Then
                            Call RegulationNodes.Add(New FileStream.Node With {.ID = Item.ToNode, .NodeType = "Pathway"})
                        End If
                    Next
                    Dim PathwaysIds = (From item In RegulationNodes Select item.ID Distinct).ToArray
                    Dim Interactions = (From item In Edges.AsParallel Where Array.IndexOf(PathwaysIds, item.FromNode) > -1 AndAlso Array.IndexOf(PathwaysIds, item.ToNode) > -1 Select item).ToArray

                    Call ChunkTemp.AddRange(Interactions)
                    Call RegulationNodes.Add(New FileStream.Node With {.NodeType = "Regulator", .ID = Line.TF})

                    Call RegulationNodes.SaveTo(String.Format("{0}/{1}/Nodes.csv", ExportDir, Line.TF), False)
                    Call ChunkTemp.SaveTo(String.Format("{0}/{1}/PathwayRegulations.csv", ExportDir, Line.TF), True)
                End If
            Next

            Call Network.AddRange(Edges)
            Dim ChunkList As New List(Of NetworkEdge)

            Call Console.WriteLine("Remove the duplicated data!")

            For Each Node In Network
                If Not Exists(ChunkList, Node) Then
                    Call ChunkList.Add(Node)
                End If
            Next

            Network = ChunkList

            Call Console.WriteLine("All of the job done, start to saving data!")

            Call Network.SaveTo(String.Format("{0}/Edges.csv", ExportDir), False)
            Call Nodes.SaveTo(String.Format("{0}/Pathways.csv", ExportDir), False)
            Call NodeList.SaveTo(String.Format("{0}/Regulators.csv", ExportDir), False)
        End Sub

        Private Shared Function Regulates(RegulatedGenes As String(), Pathway As Pathways.Pathway) As Boolean
            For Each RegulatedGene As String In RegulatedGenes
                If Array.IndexOf(Pathway.GeneObjects, RegulatedGene) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Shared Function Exists(Network As IEnumerable(Of NetworkEdge), Node As NetworkEdge) As Boolean
            Dim LQuery As Integer =
                LinqAPI.DefaultFirst(Of Integer) <= From edge As NetworkEdge
                                                    In Network.AsParallel
                                                    Where edge.IsEqual(Node)
                                                    Select 100
            Return LQuery > 50
        End Function

        Public Class PathwayRegulator : Inherits FileStream.Node
        End Class
    End Class
End Namespace
