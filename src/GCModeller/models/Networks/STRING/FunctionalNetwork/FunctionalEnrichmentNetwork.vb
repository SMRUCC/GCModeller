#Region "Microsoft.VisualBasic::c683abda82ae15d1ebcef24381456e76, G:/GCModeller/src/GCModeller/models/Networks/STRING//FunctionalNetwork/FunctionalEnrichmentNetwork.vb"

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

    '   Total Lines: 211
    '    Code Lines: 161
    ' Comment Lines: 23
    '   Blank Lines: 27
    '     File Size: 8.11 KB


    ' Module FunctionalEnrichmentNetwork
    ' 
    '     Function: BuildModel, RenderDEGsColor, RenderDEGsColorSchema, stringNode, StringUniprot
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.STRING

''' <summary>
''' 功能富集网络
''' </summary>
Public Module FunctionalEnrichmentNetwork

    ''' <summary>
    ''' Using string-db ID as the uniprot data index key
    ''' </summary>
    ''' <param name="uniprot"></param>
    ''' <returns></returns>
    <Extension>
    Public Function StringUniprot(uniprot As IEnumerable(Of entry)) As Dictionary(Of String, entry)
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
                               groupValues As Dictionary(Of String, String)) As NetworkGraph

        Dim name2STRING As Dictionary(Of String, String) = interacts _
            .Select(Function(x) {
                (x.node1, x.node1_external_id),
                (x.node2, x.node2_external_id)
            }) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .ToDictionary(Function(x) x.Key,
                          Function(stringID)
                              Return stringID.First.Item2
                          End Function)
        Dim g As New NetworkGraph

        For Each stringId As String In interacts.NodesID
            Call g.AddNode(name2STRING.stringNode(uniprot, groupValues, stringId))
        Next

        For Each l As InteractExports In interacts
            Dim a = g.GetElementByID(l.node1)
            Dim b = g.GetElementByID(l.node2)
            Dim pa = Strings.Split(a.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), FunctionalEnrichmentNetwork.delimiter)
            Dim pb = Strings.Split(b.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), FunctionalEnrichmentNetwork.delimiter)
            Dim type$

            If pa.Where(Function(pathway) pb.IndexOf(pathway) > -1).Count > 0 Then
                type = "pathway_internal"
            ElseIf pa.IsNullOrEmpty AndAlso pb.IsNullOrEmpty Then
                type = "Unknown"
            Else
                type = "pathway_outbounds"
            End If

            Dim data As New EdgeData With {
                .label = l.ToString,
                .length = l.combined_score,
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE, type}
                }
            }

            Call g.CreateEdge(l.node1, l.node2, l.combined_score, data)
        Next

        Return g
    End Function

    <Extension>
    Private Function stringNode(name2STRING As Dictionary(Of String, String),
                                uniprot As Dictionary(Of String, entry),
                                groupValues As Dictionary(Of String, String),
                                stringID As String) As Node
        Dim pathways$()
        Dim KO$()
        Dim uniprotID$()
        Dim name$ = stringID

        stringID = name2STRING(name)

        If uniprot.ContainsKey(stringID) Then
            With uniprot(stringID)
                KO = .xrefs.TryGetValue("KO") _
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

        Dim data As New NodeData

        With data
            !KO = KO.JoinBy("|")
            !uniprotID = uniprotID.JoinBy("|")
            !STRING_ID = stringID

            .Properties(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = pathways.JoinBy(FunctionalEnrichmentNetwork.delimiter)
        End With

        Return New Node(name) With {
            .data = data
        }
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
    Public Function RenderDEGsColor(ByRef model As NetworkGraph,
                                    DEGs As (up As String(), down As String()),
                                    colors As (up As Brush, down As Brush),
                                    Optional nonDEPcolor$ = "gray") As NetworkGraph

        Dim up = DEGs.up.Indexing
        Dim down = DEGs.down.Indexing
        Dim notsigColor As Brush = nonDEPcolor.GetBrush

        For Each node As Node In model.vertex
            Dim id$ = node.data!STRING_ID

            If up.IndexOf(id) > -1 Then
                node.data.color = colors.up
            ElseIf down.IndexOf(id) > -1 Then
                node.data.color = colors.down
            Else
                node.data.color = notsigColor
            End If
        Next

        Return model
    End Function

    <Extension>
    Public Function RenderDEGsColorSchema(ByRef model As NetworkGraph,
                                          DEGs As (up As Dictionary(Of String, Double), down As Dictionary(Of String, Double)),
                                          schema As (up$, down$),
                                          Optional nonDEPColor$ = NameOf(Color.Gray)) As NetworkGraph
        Dim upColors() = Designer _
            .GetColors(schema.up, 256) _
            .Skip(56) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim downColors() = Designer _
            .GetColors(schema.down, 256) _
            .Skip(56) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim colorIndex As DoubleRange = New Double() {0, 199}
        Dim upRange As DoubleRange = DEGs.up.Values.Range
        Dim downRange As DoubleRange = DEGs.down.Values.Range
        Dim notsigColor As Brush = nonDEPColor.GetBrush

        For Each node As Node In model.vertex
            Dim id$ = node.data!STRING_ID
            Dim data As NodeData = node.data

            If DEGs.up.ContainsKey(id) Then
                data.color = upColors(upRange.ScaleMapping(DEGs.up(id), colorIndex))
            ElseIf DEGs.down.ContainsKey(id) Then
                data.color = downColors(downRange.ScaleMapping(DEGs.down(id), colorIndex))
            Else
                data.color = notsigColor
            End If
        Next

        Return model
    End Function
End Module
