#Region "Microsoft.VisualBasic::5b49a54f2e8f072388db5d628f83f4f1, analysis\Motifs\VirtualFootprint\Extensions.vb"

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

    ' Module Extensions
    ' 
    '     Function: __uid, KEGGRegulon, KEGGRegulons, MergeLocis, RegulationFootprintTRN
    '               TrimStranded, uid
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.Data.Regprecise
Imports SMRUCC.genomics.Model.Network.VirtualFootprint.DocumentFormat
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
'''
''' </summary>
''' 
<HideModuleName>
Public Module Extensions

    <Extension> Public Function uid(x As RegPreciseRegulon) As String
        Dim sId As String

        If String.IsNullOrWhiteSpace(x.Pathway) Then ' 集合里面的元素的位置发生变化了之后，这个就失效了？？？？这样子会不会有问题？
            sId = $"{x.Regulator}.{x.hits.First}".NormalizePathString
        Else
            sId = $"{x.Regulator}.{x.Pathway}".NormalizePathString
        End If

        sId = sId.Replace(" ", "_")

        Return sId
    End Function

    <Extension> Public Function KEGGRegulons(footprints As IEnumerable(Of RegulatesFootprints), cats As ModuleClassAPI) As KEGGRegulon()
        Dim uids = (From x As RegulatesFootprints In footprints.AsParallel Select x.__uid, x Group By __uid Into Group).ToArray
        Dim LQuery = (From x In uids Select x.Group.Select(Function(o) o.x).ToArray.KEGGRegulon(cats)).ToArray
        Return LQuery
    End Function

    <Extension>
    Public Function KEGGRegulon(footprints As RegulatesFootprints(), cats As ModuleClassAPI) As KEGGRegulon
        Dim modId As String = footprints.First.motif_id.Split("."c).First
        Dim A As String = "", B As String = "", C As String = ""
        Dim modX = cats.GetBriteInfo(modId, A, B, C)

        Return New KEGGRegulon With {
            .Family = footprints.Select(Function(x) x.motif_family).Distinct.ToArray,
            .Members = footprints.Select(Function(x) x.ORF).Distinct.ToArray,
            .ModId = modId,
            .Regulator = footprints.Select(Function(x) x.Regulator).Distinct.ToArray,
            .Category = C,
            .Class = B,
            .Type = A,
            .Name = cats.GetName(modX)
        }
    End Function

    <Extension> Private Function __uid(x As RegulatesFootprints) As String
        Return $"{x.motif_id.Split("."c).First}-{x.Sequence}"
    End Function

    ''' <summary>
    ''' 将相邻的位点进行合并
    ''' </summary>
    ''' <param name="source">假设都是同一条链上面的</param>
    ''' <param name="offset"></param>
    ''' <param name="getDist"></param>
    ''' <param name="getTag"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Id和距离是使用一些方法来读取的，例如<see cref="SimpleSegment.ID"/>是ID:Dist这种形式的话，就可以分别分离出编号和记录数据
    ''' </remarks>
    <Extension>
    Public Iterator Function MergeLocis(source As IEnumerable(Of SimpleSegment),
                                        offset As Integer,
                                        getDist As Func(Of SimpleSegment, Integer),
                                        Optional getTag As Func(Of SimpleSegment, String) = Nothing) As IEnumerable(Of SimpleSegment)
        If getTag Is Nothing Then
            getTag = Function(x) x.ID
        End If

        Dim data As TagSite(Of SimpleSegment)() =
            LinqAPI.Exec(Of TagSite(Of SimpleSegment)) <= From x As SimpleSegment
                                                          In source
                                                          Let id As String = getTag(x)
                                                          Let d As Integer = getDist(x)
                                                          Select New TagSite(Of SimpleSegment) With {
                                                              .tag = id,
                                                              .contig = x,
                                                              .Distance = d
                                                          }
        Dim out = data.Groups(offset)

        For Each part As GroupResult(Of TagSite(Of SimpleSegment), String) In out
            Dim locis As Integer() = part.Group.Select(Function(x) {x.contig.Ends, x.contig.Start}).ToVector
            Dim min As Integer = locis.Min
            Dim max As Integer = locis.Max
            Dim ref As SimpleSegment = part.Group.First.contig
            Dim loci As New NucleotideLocation(min, max, ref.MappingLocation.Strand)
            Dim copy As New SimpleSegment(ref, loci)

            Yield copy
        Next
    End Function

    ''' <summary>
    ''' 筛选出和标记的基因相同的链方向的位点数据
    ''' </summary>
    ''' <param name="sites"></param>
    ''' <param name="genome"></param>
    ''' <param name="getId"></param>
    ''' <returns></returns>
    <Extension>
    Public Iterator Function TrimStranded(sites As IEnumerable(Of SimpleSegment), genome As PTT, getId As Func(Of SimpleSegment, String)) As IEnumerable(Of SimpleSegment)
        For Each x As SimpleSegment In sites
            Dim sid As String = getId(x)
            Dim gene As GeneBrief = genome(sid)

            If Not gene Is Nothing Then
                If gene.Location.Strand = x.MappingLocation.Strand Then
                    Yield x
                End If
            End If
        Next
    End Function

    <Extension>
    Public Function RegulationFootprintTRN(network As IEnumerable(Of RegulationFootprint)) As NetworkGraph
        Dim g As New NetworkGraph
        Dim node As Node

        For Each footprint As IGrouping(Of String, RegulationFootprint) In network.GroupBy(Function(reg) $"{reg.regulator}->{reg.regulated}")
            Dim matchedList As RegulationFootprint() = footprint.ToArray
            Dim regulator As String = matchedList(Scan0).regulator.Split("|"c).Last
            Dim target As String = matchedList(Scan0).regulated.Split("|"c).Last
            Dim maxSupportsFamily As String = matchedList _
                .Select(Function(r) r.family) _
                .GroupBy(Function(name) name) _
                .OrderByDescending(Function(family) family.Count) _
                .First _
                .Key
            Dim supports As Integer = matchedList.Length

            node = g.GetElementByID(regulator)

            If node Is Nothing Then
                node = g.CreateNode(regulator)
            End If

            node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE) = "TF"
            node.data("family") = maxSupportsFamily

            node = g.GetElementByID(target)

            If node Is Nothing Then
                node = g.CreateNode(target)
            End If

            Dim regulates As Edge = g _
                .GetEdges(
                    u:=g.GetElementByID(regulator),
                    v:=g.GetElementByID(target)
                ) _
                .FirstOrDefault

            If regulates Is Nothing Then
                regulates = g.CreateEdge(g.GetElementByID(regulator), g.GetElementByID(target))
            End If

            regulates.weight = supports
            regulates.isDirected = True
            regulates.data("supports") = supports
            regulates.data.label = footprint.Key
            regulates.data(NamesOf.REFLECTION_ID_MAPPING_INTERACTION_TYPE) = "regulates"
            regulates.data("family") = matchedList.Select(Function(r) r.family).Distinct.JoinBy("; ")
            regulates.data("regulog") = matchedList.Select(Function(r) r.regulog).GroupBy(Function(r) r).OrderByDescending(Function(r) r.Count).First.Key
            regulates.data("biological_process") = matchedList.Select(Function(r) r.biological_process).Where(Function(r) Not r.StringEmpty).Distinct.JoinBy("; ")
            regulates.data("effector") = matchedList.Select(Function(r) r.effector.StringSplit(";\s*")).IteratesALL.Where(Function(r) Not r.StringEmpty).Distinct.JoinBy("; ")
            regulates.data("loci") = matchedList.Select(Function(r) $"{r.distance}:{r.sequenceData}").Distinct.JoinBy("; ")
        Next

        Return g
    End Function
End Module
