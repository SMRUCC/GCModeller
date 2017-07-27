#Region "Microsoft.VisualBasic::3e04b0b960df095ac6976fca0a5e783a, ..\interops\visualize\Cytoscape\Cytoscape.App\NetworkModel\KEGG\PfsNET\ModInteractions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat

Namespace NetworkModel.KEGG

    ''' <summary>
    ''' 基因和模块之间的从属关系的示意图
    ''' </summary>
    <Package("Cytoscape.NET.KEGG_Mods")>
    Public Module ModInteractions

        <ExportAPI("Load.Modules")>
        Public Function LoadModules(DIR As String) As bGetObject.Module()
            Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
            Dim LQuery = (From xml As String
                          In files.AsParallel
                          Select xml.LoadXml(Of bGetObject.Module)).ToArray
            Return LQuery
        End Function

        <ExportAPI("Load.Pathways")>
        Public Function LoadPathways(DIR) As bGetObject.Pathway()
            Dim files = FileIO.FileSystem.GetFiles(DIR, FileIO.SearchOption.SearchAllSubDirectories, "*.xml")
            Dim LQuery = (From xml As String
                          In files.AsParallel
                          Select xml.LoadXml(Of bGetObject.Pathway)).ToArray
            Return LQuery
        End Function

        <ExportAPI("Build.NET")>
        <Extension> Public Function BuildNET(Of T As PathwayBrief)(mods As IEnumerable(Of T)) As NetworkTables
            Dim net As New NetworkTables
            Dim modType As String = GetType(T).Name
            Dim modHash = New ModsBrite(Of T)
            Dim netEdges = (From x As T In mods
                            Let genes As String() = x.GetPathwayGenes
                            Select (From g As String
                                    In genes
                                    Select g,
                                        __mod = x)).IteratesALL
            net += (From x As T
                    In mods
                    Select New Node With {
                        .ID = x.EntryId,
                        .NodeType = modType,
                        .Properties = modHash.__modProperty(x)}).ToArray
            net += (From x In netEdges
                    Select x
                    Group x By x.g Into Group) _
                         .ToArray(Function(x) (From edge In x.Group
                                               Select New NetworkEdge With {
                                                   .value = 1,
                                                   .FromNode = edge.__mod.EntryId,
                                                   .ToNode = edge.g,
                                                   .Interaction = PathwayGene})).IteratesALL
            net += net.__modProperty(net.Edges)

            Return net
        End Function

        ''' <summary>
        ''' Label for interation pathway genes
        ''' </summary>
        Public Const PathwayGene As String = "Pathway Gene"

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="edges">Mod -> Gene</param>
        ''' <returns></returns>
        <Extension>
        Private Function __modProperty(net As NetworkTables, edges As NetworkEdge()) As IEnumerable(Of Node)
            Dim LQuery = (From x As NetworkEdge In edges
                          Let mId As String = x.FromNode
                          Let mX As Node = net & mId
                          Where Not mX Is Nothing AndAlso
                              Not mX.Properties Is Nothing
                          Let props = New Dictionary(Of String, String)(mX.Properties)
                          Select New Node With {
                              .ID = x.ToNode,
                              .NodeType = "Enzyme",
                              .Properties = props})
            Dim Groups = (From x In LQuery Select x Group x By x.ID Into Group)
            Return (From x In Groups Select x.Group.First)
        End Function

        <Extension>
        Private Function __modProperty(Of T As PathwayBrief)(hash As ModsBrite(Of T), x As T) As Dictionary(Of String, String)
            Return New Dictionary(Of String, String) From {
 _
                {"A", hash.GetType(x)},
                {"B", hash.GetClass(x)},
                {"C", hash.GetCategory(x)}
            }
        End Function

        ''' <summary>
        ''' 向网络之中添加调控信息
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="footprints">基因调控信息</param>
        ''' <returns></returns>
        <ExportAPI("NET.Add.Footprints")>
        <Extension>
        Public Function AddFootprints(net As NetworkTables,
                                      footprints As IEnumerable(Of RegulatesFootprints),
                                      Optional brief As Boolean = False) As NetworkTables

            footprints = (From x In footprints Where InStr(x.MotifTrace, "@") = 0 Select x).ToArray  ' 拓展的不需要，因为会让图太密了

            net += (From x As RegulatesFootprints
                    In footprints
                    Where Not String.IsNullOrEmpty(x.Regulator)
                    Select x.Regulator Distinct) _
                          .ToArray(AddressOf __tfNode)  ' 生成调控因子节点

            If brief Then
                footprints = (From x In footprints Where True = net ^ x.ORF Select x).ToArray
            End If

            net += (From x In footprints.AsParallel
                    Where (Not String.IsNullOrEmpty(x.Regulator))   ' 生成被调控的基因的节点
                    Let uid = x.Regulator & x.ORF
                    Let c As Double = x.Pcc * 0.8 + x.sPcc * 0.2
                    Select uid,
                        x.Regulator,
                        x.ORF,
                        c
                    Group By uid Into Group).ToArray(
                        Function(x) New NetworkEdge With {
                            .FromNode = x.Group.First.Regulator,
                            .ToNode = x.Group.First.ORF,
                            .Interaction = "Regulates",
                            .value = x.Group.First.c})
            Return net
        End Function

        Private Function __tfNode(TF As String) As Node
            Return New Node With {
                .ID = TF,
                .NodeType = "TF"
            }
        End Function

        <ExportAPI("Write.Csv.Network")>
        Public Function SaveNetwork(net As NetworkTables, DIR As String) As Boolean
            Return net.Save(DIR, Encodings.ASCII)
        End Function

        <ExportAPI("Build.NET")>
        Public Function BuildNET(mods As IEnumerable(Of bGetObject.Pathway)) As NetworkTables
            Return mods.BuildNET
        End Function

        <ExportAPI("Build.NET")>
        Public Function BuildNET(mods As IEnumerable(Of bGetObject.Module)) As NetworkTables
            Return mods.BuildNET
        End Function
    End Module
End Namespace
