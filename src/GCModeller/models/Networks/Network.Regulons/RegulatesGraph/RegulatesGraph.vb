Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.ComponentModel
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataVisualization.Network
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq.Extensions
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.Serialization
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Cytoscape.CytoscapeGraphView.XGMML

Namespace RegulatesGraph

    <PackageNamespace("MEME.RegulatesGraph", Category:=APICategories.ResearchTools, Publisher:="xie.guigang@gcmodeller.org")>
    Public Module GraphAPI

        <ExportAPI("Create.Doc")>
        Public Function Create(sites As String, <Parameter("Dir.Modules")> mods As String) As Graph
            Dim motifSites = sites.LoadCsv(Of MotifSite)
            Dim modDetails = FileIO.FileSystem.GetFiles(mods, FileIO.SearchOption.SearchAllSubDirectories, "*.xml") _
                .ToArray(Function(file) file.LoadXml(Of bGetObject.Module), Parallel:=True) _
                .ToDictionary(Function([mod]) [mod].EntryId.Split("_"c).Last.ToUpper)
            Return Create(motifSites, modDetails)
        End Function

        Private Function __family(Family As String) As String
            Family = If(String.IsNullOrEmpty(Family.Trim), "Other", Family.NormalizePathString(normAs:=""))
            Return Family
        End Function

        Public Function Create(motifSites As IEnumerable(Of MotifSite), modDetails As Dictionary(Of String, bGetObject.Module)) As Graph
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
                                                                             Function(gg) gg.Group.ToArray(Function(ggg) ggg.Locus_tag)))
            Dim Edges As PathwayRegulates() = LQuery.ToArray(
                Function(site) site.Value.ToArray(
                Function(obj, target) __regulates(site.Key, obj, target))).MatrixToVector
            Dim Nodes = LQuery.ToArray(AddressOf __node).Join(modDetails.ToArray(AddressOf __node))
            Dim doc = ExportToFile.Export(Nodes, Edges)
            Return doc
        End Function

        Private Function __regulates(from As String, obj As String, target As String()) As PathwayRegulates
            Return New PathwayRegulates With {
                .FromNode = obj,
                .Regulates = target.Distinct.ToArray.JoinBy(", "),
                .ToNode = obj,
                .Confidence = target.Length,
                .InteractionType = "regulates"
            }
        End Function

        Private Function __node(Id As String, modX As bGetObject.Module) As Entity
            Return New Entity With {
                .NodeType = "Module",
                .Identifier = Id,
                .Size = modX.GetPathwayGenes.Length
            }
        End Function

        Private Function __node(Id As String, hash As Dictionary(Of String, String())) As Entity
            Return New Entity With {
                .Identifier = Id,
                .NodeType = "Motif Family",
                .Size = hash.Count
            }
        End Function

        <ExportAPI("Write.XGMML")>
        Public Function SaveGraph(graph As Graph, SaveTo As String) As Boolean
            Return graph.Save(SaveTo)
        End Function
    End Module
End Namespace
