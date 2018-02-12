#Region "Microsoft.VisualBasic::b67e287e7d13ae5fe21ee53579fa5377, visualize\Cytoscape\Cytoscape\API\ImportantNodes\ImportantNodes.vb"

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

    '     Module ImportantNodes
    ' 
    '         Function: __equivalenceFast, __nodes, EquivalenceClass, LoadNodeTable, ReadRankedNodes
    '                   ReadRegulations, SaveResult, SignificantRegulator
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.InteractionModel
Imports SMRUCC.genomics.InteractionModel.Regulon
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

Namespace API.ImportantNodes

    '{Hu, 2010 #173}

    <Package("Cytoscape.ImportantNodes")>
    Public Module ImportantNodes

        <ExportAPI("Read.Csv.Nodes.Cytoscape")>
        Public Function LoadNodeTable(path As String) As Node()
            Return path.LoadCsv(Of Node)(False).ToArray
        End Function

        <ExportAPI("Read.Csv.RegulationEdges")>
        Public Function ReadRegulations(path As String) As RegulatorRegulation()
            Dim inBuf As IEnumerable(Of Regulations) = path.LoadCsv(Of Regulations)(False)
            Dim allORFs As String() =
                LinqAPI.Exec(Of String) <= From reg As Regulations
                                           In inBuf
                                           Select reg.ORF
                                           Distinct
            Return LinqAPI.Exec(Of RegulatorRegulation) <=
                From ORF As String
                In allORFs.AsParallel
                Let Regulators As String() = (From x As Regulations In inBuf
                                              Where String.Equals(x.ORF, ORF)
                                              Select x.Regulator
                                              Distinct).ToArray
                Select New RegulatorRegulation With {
                    .LocusId = ORF,
                    .Regulators = Regulators
                }
        End Function

        ''' <summary>
        '''这个仅仅是理论上面的计算结果，仅供参考
        ''' </summary>
        ''' <param name="ImportantNodes"></param>
        ''' <param name="Regulations"></param>
        ''' <param name="rankCutoff"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Regulator.Significants",
                   Info:="rank_cutoff = 0 stands for using the default value; -1 stands for using all of the nodes without any cutoff value screening, else 0 - 1 for the selected percentage.")>
        <Extension>
        Public Function SignificantRegulator(ImportantNodes As IEnumerable(Of KeyValuePair(Of Integer, Node())),
                                             Regulations As IEnumerable(Of IRegulatorRegulation),
                                             <Parameter("Rank.Cutoff",
                                                        "0 stands for using the default value; -1 stands for using all of the nodes without any cutoff value screening, else 0 - 1 for the selected percentage.")>
                                             Optional rankCutoff As Double = -1) As RankRegulations()
            If rankCutoff = 0.0R Then
                rankCutoff = 0.1 * ImportantNodes.Count
            ElseIf rankCutoff < 0 Then
                rankCutoff = ImportantNodes.Count
            Else
                rankCutoff = ImportantNodes.Count * rankCutoff
            End If

            Regulations = (From rel As IRegulatorRegulation
                           In Regulations.AsParallel
                           Where Not (String.IsNullOrEmpty(rel.LocusId) OrElse rel.Regulators.IsNullOrEmpty)
                           Select rel).ToArray    ' Trim Data
            ImportantNodes = (From node In ImportantNodes
                              Where node.Key <= rankCutoff
                              Select node).ToArray
            rankCutoff += 1

            Dim ImportantRankNodes = (From node In ImportantNodes.AsParallel
                                      Select New NodeRank With {
                                          .Rank = node.Key,
                                          .Nodes = (From n As Node In node.Value Select n.SharedName).ToArray}).ToArray
            Dim RegulatorRanks = (From ranks As NodeRank In ImportantRankNodes.AsParallel
                                  Select New RankRegulations With {
                                      .RankScore = rankCutoff - ranks.Rank,
                                      .Regulators = (From rel As IRegulatorRegulation
                                                     In Regulations
                                                     Where Array.IndexOf(ranks.Nodes, rel.LocusId) > -1
                                                     Select rel.Regulators).IteratesALL.Distinct.ToArray,
                                      .GeneCluster = ranks.Nodes}).ToArray
            Return RegulatorRanks
        End Function

        <ExportAPI("Write.Csv.Nodes.Important")>
        Public Function SaveResult(data As IEnumerable(Of KeyValuePair(Of Integer, Node())), saveCsv As String) As Boolean
            Dim LQuery = From x In data.AsParallel
                         Let Rank As Integer = x.Key
                         Select Rank,
                             importantNode = (From n As Node
                                              In x.Value
                                              Select n.SharedName).ToArray
                         Order By Rank Ascending
            Return LQuery.ToArray.SaveTo(saveCsv, False)
        End Function

        <ExportAPI("read.csv.rank_nodes")>
        Public Function ReadRankedNodes(path As String) As KeyValuePair(Of Integer, Node())()
            Dim data As IEnumerable(Of NodeRank) = path.LoadCsv(Of NodeRank)(False)
            Return LinqAPI.Exec(Of KeyValuePair(Of Integer, Node())) <=
                From x As NodeRank
                In data.AsParallel
                Let nodes As Node() = x.Nodes.__nodes
                Select New KeyValuePair(Of Integer, Node())(x.Rank, nodes)
        End Function

        <Extension>
        Private Function __nodes(nodes As String()) As Node()
            Return LinqAPI.Exec(Of Node) <= From name As String
                                            In nodes
                                            Select New Node With {
                                                .SharedName = name
                                            }
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="S"></param>
        ''' <param name="Fast">
        ''' if fast parameter is set to true, then a parallel edition of the algorithm 
        ''' will implemented for accelerates the network calculation, and this is much 
        ''' helpful for a large scale network.
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("evaluate.importance",
                   Info:="If fast parameter is set to true, then a parallel edition of the algorithm will implemented for accelerates the network calculation.")>
        Public Function EquivalenceClass(S As Node(), Optional Fast As Boolean = False) As KeyValuePair(Of Integer, Node())()
            If Fast Then Return __equivalenceFast(S.AsList, S)

            Dim NDS As List(Of Node) = S.AsList
            Dim Extra As List(Of Node) = New List(Of Node)
            Dim Rank As Integer = 0
            Dim SortResult As List(Of KeyValuePair(Of Integer, Node())) = New List(Of KeyValuePair(Of Integer, Node()))

            Do While Not NDS.IsNullOrEmpty
                For Each a As Node In NDS.ToArray

                    For Each b As Node In S
                        If NDS.IndexOf(b) > -1 AndAlso a < b Then

                            Call NDS.Remove(a)
                            Call Extra.Add(a)
                        End If
                    Next
                Next

                Rank += 1
                Call SortResult.Add(New KeyValuePair(Of Integer, Node())(Rank, NDS.ToArray))
                Call Console.WriteLine("Rank:= {0};  ImportantNodes:= {1}", SortResult.Last.Key, String.Join("; ", (From item In SortResult.Last.Value Select item.SharedName).ToArray))
                NDS = Extra.Distinct.AsList
                Call Extra.Clear()
            Loop

            Return SortResult.ToArray
        End Function

        Private Function __equivalenceFast(NDS As List(Of Node), S As Node()) As KeyValuePair(Of Integer, Node())()
            Dim Rank As Integer = 0
            Dim SortResult As List(Of KeyValuePair(Of Integer, Node())) = New List(Of KeyValuePair(Of Integer, Node()))

            Do While Not NDS.IsNullOrEmpty
                Dim LQuery = (From b As Node
                              In S.AsParallel
                              Where NDS.IndexOf(b) > -1
                              Let ia = (From a As Node In NDS Where a < b Select a).ToArray
                              Select ia).ToVector
                NDS = (From node As Node
                       In NDS.AsParallel
                       Where Array.IndexOf(LQuery, node) = -1
                       Select node
                       Distinct).AsList
                Rank += 1
                If NDS.IsNullOrEmpty Then
                    Exit Do
                End If
                Call SortResult.Add(New KeyValuePair(Of Integer, Node())(Rank, NDS.ToArray))
                Call Console.WriteLine("Rank:= {0};  ImportantNodes:= {1}", SortResult.Last.Key, String.Join("; ", (From item In SortResult.Last.Value Select item.SharedName).ToArray))
                NDS = LQuery.Distinct.AsList
            Loop

            Return SortResult.ToArray
        End Function
    End Module
End Namespace
