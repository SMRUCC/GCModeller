#Region "Microsoft.VisualBasic::c60dc65e2a061c7199a9265b1d4a0a0a, models\Networks\STRING\FunctionalNetwork\FunctionalEnrichmentNetwork.vb"

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

    ' Module FunctionalEnrichmentNetwork
    ' 
    '     Function: BuildModel, RenderDEGsColor, RenderDEGsColorSchema, StringUniprot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING
Imports NetGraph = Microsoft.VisualBasic.Data.visualize.Network.FileStream.NetworkTables
Imports NetNode = Microsoft.VisualBasic.Data.visualize.Network.FileStream.Node

''' <summary>
''' 功能富集网络
''' </summary>
Public Module FunctionalEnrichmentNetwork

    ''' <summary>
    ''' Using string-db ID as the uniprot data index key
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <returns></returns>
    <Extension> Public Function StringUniprot(uniprot As IEnumerable(Of entry)) As Dictionary(Of String, entry)
        Return uniprot _
            .Where(Function(protein) protein.xrefs.ContainsKey(InteractExports.STRING)) _
            .Select(Function(protein) protein.xrefs(InteractExports.STRING) _
            .Select(Function(id) (id, protein))) _
            .IteratesALL _
            .GroupBy(Function(id) id.Item1.id) _
            .ToDictionary(Function(x) x.Key,
                          Function(proteins)
                              Return proteins.First.Item2
                          End Function)
    End Function

    Const delimiter$ = " === "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="interacts"></param>
    ''' <param name="uniprot"></param>
    ''' <param name="groupValues">进行Node分组所需要的group信息来源</param>
    ''' <returns></returns>
    <Extension>
    Public Function BuildModel(interacts As IEnumerable(Of InteractExports),
                               uniprot As Dictionary(Of String, entry),
                               groupValues As Dictionary(Of String, String)) As NetGraph

        Dim name2STRING = interacts _
            .Select(Function(x) {
                (x.node1, x.node1_external_id),
                (x.node2, x.node2_external_id)
            }) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(x) x.Key,
                          Function(stringID) stringID.First.Item2)
        Dim nodes = interacts _
            .NodesID _
            .Select(Function(stringID$)
                        Dim pathways$()
                        Dim KO$()
                        Dim uniprotID$()
                        Dim name$ = stringID

                        stringID = name2STRING(name)

                        If uniprot.ContainsKey(stringID) Then
                            With uniprot(stringID)
                                KO = .Xrefs.TryGetValue("KO") _
                                     .SafeQuery _
                                     .Select(Function(x) x.id) _
                                     .ToArray
                                pathways = KO _
                                    .Where(Function(ID) groupValues.ContainsKey(ID)) _
                                    .Select(Function(ID) groupValues(ID)) _
                                    .Distinct _
                                    .ToArray
                                uniprotID = .accessions
                            End With
                        Else
                            KO = {}
                            pathways = {}
                            uniprotID = {}
                        End If

                        Dim data As New Dictionary(Of String, String)

                        With data
                            !KO = KO.JoinBy("|")
                            !uniprotID = uniprotID.JoinBy("|")
                            !STRING_ID = stringID
                        End With

                        Return New NetNode(name) With {
                            .NodeType = pathways.JoinBy(FunctionalEnrichmentNetwork.delimiter),
                            .Properties = data
                        }
                    End Function) _
            .ToDictionary
        Dim links = interacts _
            .Select(Function(l)
                        Dim a = nodes(l.node1)
                        Dim b = nodes(l.node2)
                        Dim pa = Strings.Split(a.NodeType, FunctionalEnrichmentNetwork.delimiter)
                        Dim pb = Strings.Split(b.NodeType, FunctionalEnrichmentNetwork.delimiter)
                        Dim type$

                        If pa.Where(Function(pathway) pb.IndexOf(pathway) > -1).Count > 0 Then
                            type = "pathway_internal"
                        ElseIf pa.IsNullOrEmpty AndAlso pb.IsNullOrEmpty Then
                            type = "Unknown"
                        Else
                            type = "pathway_outbounds"
                        End If

                        Return New NetworkEdge With {
                            .FromNode = l.node1,
                            .ToNode = l.node2,
                            .Interaction = type,
                            .value = l.combined_score
                        }
                    End Function).ToArray

        Return New NetGraph(links, nodes.Values)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="model">是以STRING的蛋白编号为标识符的，所以在这里还需要使用uniprot的数据进行转换</param>
    ''' <param name="DEGs">uniprot蛋白编号</param>
    ''' <param name="colors"></param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function RenderDEGsColor(ByRef model As NetGraph,
                                    DEGs As (up As String(), down As String()),
                                    colors As (up$, down$),
                                    Optional nonDEPcolor$ = "gray") As NetGraph

        Dim up = DEGs.up.Indexing
        Dim down = DEGs.down.Indexing

        For Each node As NetNode In model.Nodes
            With node
                Dim id$ = !STRING_ID

                If up.IndexOf(id) > -1 Then
                    !color = colors.up
                ElseIf down.IndexOf(id) > -1 Then
                    !color = colors.down
                Else
                    !color = nonDEPcolor
                End If
            End With
        Next

        Return model
    End Function

    <Extension>
    Public Function RenderDEGsColorSchema(ByRef model As NetGraph,
                                          DEGs As (up As Dictionary(Of String, Double), down As Dictionary(Of String, Double)),
                                          schema As (up$, down$),
                                          Optional nonDEPColor$ = NameOf(Color.Gray)) As NetGraph
        Dim upColors$() = Designer _
            .GetColors(schema.up, 256) _
            .Skip(56) _
            .Select(Function(c) c.ToHtmlColor) _
            .ToArray
        Dim downColors$() = Designer _
            .GetColors(schema.down, 256) _
            .Skip(56) _
            .Select(Function(c) c.ToHtmlColor) _
            .ToArray
        Dim colorIndex As DoubleRange = {0, 199}
        Dim upRange As DoubleRange = DEGs.up.Values.Range
        Dim downRange As DoubleRange = DEGs.down.Values.Range

        For Each node As NetNode In model.Nodes
            With node
                Dim id$ = !STRING_ID

                If DEGs.up.ContainsKey(id) Then
                    !color = upColors(upRange.ScaleMapping(DEGs.up(id), colorIndex))
                ElseIf DEGs.down.ContainsKey(id) Then
                    !color = downColors(downRange.ScaleMapping(DEGs.down(id), colorIndex))
                Else
                    !color = nonDEPColor
                End If
            End With
        Next

        Return model
    End Function
End Module
