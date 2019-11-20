#Region "Microsoft.VisualBasic::7650ecb260c919ca3643b587595fc474, models\Networks\Network.Regulons\RegulatesGraph\RegulatesGraph.vb"

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

    '     Module GraphAPI
    ' 
    '         Function: __family, (+2 Overloads) __node, __regulates, (+2 Overloads) Create, SaveGraph
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Serialization
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace RegulatesGraph

    <Package("MEME.RegulatesGraph", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
    Public Module GraphAPI

        <ExportAPI("Create.Doc")>
        Public Function Create(sites As String, <Parameter("Dir.Modules")> mods As String) As XGMMLgraph
            Dim motifSites = sites.LoadCsv(Of MotifSite)
            Dim modDetails = FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                .Select(Function(file) file.LoadXml(Of bGetObject.Module)) _
                .ToDictionary(Function([mod]) [mod].EntryId.Split("_"c).Last.ToUpper)
            Return Create(motifSites, modDetails)
        End Function

        Private Function __family(Family As String) As String
            Family = If(String.IsNullOrEmpty(Family.Trim), "Other", Family.NormalizePathString(normAs:=""))
            Return Family
        End Function

        Public Function Create(motifSites As IEnumerable(Of MotifSite), modDetails As Dictionary(Of String, bGetObject.Module)) As XGMMLgraph
            Dim LQuery = (From site As MotifSite In motifSites.AsParallel
                          Let [mod] As String = site.Tag.Split("\"c)(2).Split("_"c).Last.ToUpper
                          Select Family = __family(site.Family),
                              [mod],
                              site.Locus_tag
                          Group By Family Into Group) _
                             .ToDictionary(Function(obj) obj.Family,
                                           Function(obj) (From g In obj.Group.ToArray
                                                          Select g
                                                          Group g By g.mod Into Group) _
                                                               .ToDictionary(Function(oo) oo.mod,
                                                                             Function(gg) gg.Group.Select(Function(ggg) ggg.Locus_tag).ToArray))
            Dim Edges As PathwayRegulates() = LQuery.Select(
                Function(site) site.Value.Select(
                Function(obj, target) __regulates(site.Key, obj, target)).ToArray).ToVector
            Dim Nodes = LQuery.Select(AddressOf __node).Join(modDetails.Select(AddressOf __node))
            Dim doc = ExportToFile.Export(Nodes, Edges)
            Return doc
        End Function

        Private Function __regulates(from As String, obj As String, target As String()) As PathwayRegulates
            Return New PathwayRegulates With {
                .fromNode = obj,
                .Regulates = target.Distinct.ToArray.JoinBy(", "),
                .toNode = obj,
                .value = target.Length,
                .interaction = "regulates"
            }
        End Function

        Private Function __node(Id As String, modX As bGetObject.Module) As Entity
            Return New Entity With {
                .NodeType = "Module",
                .ID = Id,
                .Size = modX.GetPathwayGenes.Length
            }
        End Function

        Private Function __node(Id As String, hash As Dictionary(Of String, String())) As Entity
            Return New Entity With {
                .ID = Id,
                .NodeType = "Motif Family",
                .Size = hash.Count
            }
        End Function

        <ExportAPI("Write.XGMML")>
        Public Function SaveGraph(graph As XGMMLgraph, SaveTo As String) As Boolean
            Return graph.Save(SaveTo)
        End Function
    End Module
End Namespace
