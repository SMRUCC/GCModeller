#Region "Microsoft.VisualBasic::96483f5327f948be11968edc95d4368a, visualize\Cytoscape\Cytoscape.App\NetworkModel\MetaCyc\Pathways.vb"

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

    '     Class Pathways
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: Export, GenerateLinks
    ' 
    '         Sub: Export
    '         Class Pathway
    ' 
    '             Properties: CommonName, EnzymeCounts, GeneObjects, Identifier, ReactionCounts
    '                         SuperPathway
    ' 
    '             Function: ToString
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Metabolism

Namespace NetworkModel

    Public Class Pathways

        Protected _MetaCyc As DatabaseLoadder

        Sub New(MetaCyc As DatabaseLoadder)
            _MetaCyc = MetaCyc
        End Sub

        ''' <summary>
        ''' 导出代谢途径的网络
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Export(Dir As String) As Integer
            Dim Edges As FileStream.NetworkEdge() = Nothing, Nodes As Pathways.Pathway() = Nothing
            Call Export(Edges, Nodes)

            Call Edges.SaveTo(String.Format("{0}/Edges.csv", Dir), False)
            Call Nodes.SaveTo(String.Format("{0}/Nodes.csv", Dir), False)

            Return 0
        End Function

        Protected Sub Export(ByRef Edges As FileStream.NetworkEdge(), ByRef Nodes As Pathways.Pathway())
            Dim Pathways = _MetaCyc.GetPathways
            Dim Network As New List(Of FileStream.NetworkEdge)
            Dim NodeList As New List(Of Pathway)

            Dim EnyzmeAnalysis As New MetaCycPathways(Me._MetaCyc)
            Dim AnalysisResult = EnyzmeAnalysis.Performance
            Dim GeneObjects = _MetaCyc.GetGenes

            For Each Pwy In Pathways
                Dim LQuery As PathwayLink() =
                    LinqAPI.Exec(Of PathwayLink) <= From pwyItem As String
                                                    In Pwy.PathwayLinks
                                                    Select New PathwayLink(pwyItem) ' interaction list
                If LQuery.IsNullOrEmpty Then
                    Network += New FileStream.NetworkEdge With {
                        .FromNode = Pwy.Identifier
                    }
                Else
                    Network += GenerateLinks(LQuery, Pwy.Identifier)
                End If

                If Not Pwy.InPathway.IsNullOrEmpty Then
                    Network += From Id As String
                               In Pwy.InPathway
                               Select New FileStream.NetworkEdge With {
                                   .FromNode = Id.ToUpper,
                                   .Interaction = "Contains",
                                   .ToNode = Pwy.Identifier
                               }
                End If

                Dim assocEnzymes As String() =
                    AnalysisResult.GetItem(Pwy.Identifier).AssociatedGenes
                assocEnzymes =
                    LinqAPI.Exec(Of String) <= From gene As Gene
                                               In GeneObjects.Takes(assocEnzymes)
                                               Select gene.Accession1
                                               Distinct
                                               Order By Accession1 Ascending
                NodeList += New Pathway With {
                    .Identifier = Pwy.Identifier,
                    .GeneObjects = assocEnzymes,
                    .EnzymeCounts = assocEnzymes.Count,
                    .SuperPathway = Not Pwy.SubPathways.IsNullOrEmpty,
                    .ReactionCounts = Pwy.ReactionList.Count,
                    .CommonName = Pwy.CommonName
                }
            Next

            Edges = Network.ToArray
            Nodes = NodeList.ToArray
        End Sub

        Public Class Pathway : Implements INamedValue

            Public Property Identifier As String Implements INamedValue.Key
            Public Property ReactionCounts As Integer
            Public Property EnzymeCounts As Integer
            Public Property CommonName As String
            Public Property GeneObjects As String()
            Public Property SuperPathway As Boolean

            Public Overrides Function ToString() As String
                Return Identifier
            End Function
        End Class

        Public Function GenerateLinks(pwy As PathwayLink(), UniqueId As String) As FileStream.NetworkEdge()
            Dim EdgeList As New List(Of FileStream.NetworkEdge)

            For Each Link As PathwayLink In pwy
                EdgeList += From item In Link.LinkedPathways
                            Let iter As String =
                                If(item.LinkType = PathwayLink.PathwaysLink.LinkTypes.NotSpecific,
                                  "interact_with",
                                  item.LinkType.ToString)
                            Select New FileStream.NetworkEdge With {
                                .FromNode = UniqueId,
                                .Interaction = iter,
                                .ToNode = item.Id.Replace("|", "").ToUpper
                            }
            Next

            Return EdgeList.ToArray
        End Function
    End Class
End Namespace
