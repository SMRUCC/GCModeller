#Region "Microsoft.VisualBasic::c19d5458783c9bff25937cc971cd262b, R#\gseakit\GSEABackground.vb"

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

'   Total Lines: 1006
'    Code Lines: 675 (67.10%)
' Comment Lines: 231 (22.96%)
'    - Xml Docs: 94.81%
' 
'   Blank Lines: 100 (9.94%)
'     File Size: 41.05 KB


' Module GSEABackground
' 
'     Function: appendIdTerms, asGenesetList, assembleBackground, BackgroundIDmapping, backgroundSummary
'               backgroundTabular, clusterIDs, ClusterIntersections, create_metpa, CreateCluster
'               createGene, CreateKOBackground, CreateKOReference, DAGbackground, flatDAGBackground
'               (+2 Overloads) geneSetAnnotation, GetCluster, getMemberItem, (+2 Overloads) id_translation, KEGGCompoundBriteClassBackground
'               KOTable, metabolismBackground, MetaEnrichBackground, moleculeIDs, ParsePathwayObject
'               PrintBackground, ReadBackground, WriteBackground
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KnowledgeBase.Metabolism.Metpa
Imports SMRUCC.genomics.Analysis.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Model.Network.KEGG.ReactionNetwork
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting
Imports GSEATools = SMRUCC.genomics.Analysis.HTS.GSEA
Imports Pathway = SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' tools for handling GSEA background model.
''' </summary>
<Package("background", Category:=APICategories.ResearchTools)>
<RTypeExport("metpa", GetType(metpa))>
<RTypeExport("gsea_background", GetType(Background))>
<RTypeExport("gene_symbol", GetType(BackgroundGene))>
Public Module GSEABackground

    Sub Main()
        Call REnv.AttachConsoleFormatter(Of Background)(AddressOf PrintBackground)
        Call RInternal.Object.Converts.makeDataframe.addHandler(GetType(Background), AddressOf backgroundTabular)
    End Sub

    <RGenericOverloads("as.data.frame")>
    Private Function backgroundTabular(bg As Background, args As list, env As Environment) As Rdataframe
        Dim gene_names As Boolean = args.getValue("gene.names", env, [default]:=False)
        Dim id_class As Boolean = args.getValue("id_class", env, [default]:=False)
        Dim df As Rdataframe

        If id_class Then
            df = backgroundGeneIdTable(bg)
        Else
            df = backgroundSummary(bg, gene_names)
        End If

        Return df
    End Function

    Private Function backgroundGeneIdTable(bg As Background) As Rdataframe
        Dim inflate = bg.clusters _
            .Select(Function(c)
                        Return c.AsEnumerable _
                            .Select(Function(gene) (gene, c))
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim data As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = inflate.Sequence(1).AsCharacter.ToArray
        }

        Call data.add("id", inflate.Select(Function(a) a.gene.accessionID))
        Call data.add("name", From a In inflate Select a.gene.name)
        Call data.add("pathway_id", From a In inflate Select a.c.ID)
        Call data.add("pathway_name", From a In inflate Select a.c.names)
        Call data.add("class", From a In inflate Select a.c.class)
        Call data.add("sub_class", From a In inflate Select a.c.category)

        Return data
    End Function

    Private Function PrintBackground(x As Background) As String
        Dim summary As New StringBuilder

        Call summary.AppendLine($"           name: {x.name}")
        Call summary.AppendLine($"    description: {x.comments}")
        Call summary.AppendLine($"background_size: {x.size}")

        Return summary.ToString
    End Function

    ''' <summary>
    ''' get all of the cluster id set from the given background model object 
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns>A character vector of the cluster id(or pathway id) that defined inside the given background model</returns>
    ''' <example>
    ''' let kb = read.background("hsa.xml");
    ''' let ids = clusterIDs(kb);
    ''' 
    ''' print(ids);
    ''' </example>
    <ExportAPI("clusterIDs")>
    Public Function clusterIDs(background As Background) As String()
        Return background.clusters.Select(Function(a) a.ID).ToArray
    End Function

    ''' <summary>
    ''' get all of the molecule id set from the given background model object
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns>A character vector of the gene id that defined inside the given background model</returns>
    ''' <example>
    ''' let kb = read.background("hsa.xml");
    ''' let ids = moleculeIDs(kb);
    ''' 
    ''' print(ids);
    ''' </example>
    <ExportAPI("moleculeIDs")>
    Public Function moleculeIDs(background As Background) As String()
        Return background.clusters _
            .Select(Function(c)
                        Return c.members.Select(Function(a) a.accessionID)
                    End Function) _
            .IteratesALL _
            .Distinct _
            .ToArray
    End Function

    <ExportAPI("meta_background")>
    <RApiReturn(GetType(Background))>
    Public Function MetaEnrichBackground(enrich As EnrichmentResult(), graphQuery As GraphQuery) As Object
        Return enrich.CastBackground(graphQuery)
    End Function

    ''' <summary>
    ''' create gsea background from a given obo ontology file data.
    ''' </summary>
    ''' <param name="dag"></param>
    ''' <param name="flat">
    ''' Flat the ontology tree into cluster via the ``is_a`` relationship?
    ''' 
    ''' default false, required of the ``enrichment.go`` function for run enrichment analysis
    ''' value true, will flat the ontology tree into cluster, then the enrichment analysis could be
    ''' applied via the ``enrichment`` function.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("dag.background")>
    Public Function DAGbackground(dag As GO_OBO,
                                  Optional flat As Boolean = False,
                                  Optional verbose_progress As Boolean = True,
                                  Optional env As Environment = Nothing) As Background

        Dim getCluster = dag.terms.GOClusters
        Dim background = dag.terms _
            .Select(Function(t) t.id) _
            .CreateGOGeneric(getCluster, dag.terms.Length)

        If flat Then
            background = dag.flatDAGBackground(background, verbose_progress, env)
        End If

        background.name = dag.headers.Ontology
        background.id = background.name
        background.comments = dag.ToString

        Return background
    End Function

    <Extension>
    Private Function flatDAGBackground(dag As GO_OBO, background As Background, verbose_progress As Boolean, env As Environment)
        Dim ontology As New GeneOntology.DAG.Graph(DirectCast(dag, GO_OBO).AsEnumerable)
        Dim index = background.GetClusterTable
        Dim clusters As New List(Of Cluster)
        Dim n As Integer = background.clusters.Length
        Dim d As Integer = n / 20
        Dim pc As New PerformanceCounter
        Dim i As i32 = 0
        Dim println = env.WriteLineHandler
        Dim bar As Tqdm.ProgressBar = Nothing

        Call pc.Set()

        For Each cluster As Cluster In TqdmWrapper.Wrap(background.clusters, bar:=bar, wrap_console:=verbose_progress)
            cluster = cluster.PullOntologyTerms(ontology, index)
            clusters.Add(cluster)

            Call bar.SetLabel(pc.Mark(cluster.names).ToString)
        Next

        background.clusters = clusters.ToArray
        background.size = background.clusters.BackgroundSize

        Return background
    End Function

    ''' <summary>
    ''' Append id terms to a given gsea background
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="term_name"></param>
    ''' <param name="terms"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("append.id_terms")>
    Public Function appendIdTerms(background As Background,
                                  term_name As String,
                                  terms As list,
                                  Optional env As Environment = Nothing) As Object

        Dim termList As Dictionary(Of String, String()) = terms.AsGeneric(Of String())(env)

        For Each cluster As Cluster In background.clusters
            For Each gene As BackgroundGene In cluster.members
                Dim termIds As String() = Nothing

                If termList.ContainsKey(gene.accessionID) Then
                    termIds = termList(gene.accessionID)
                ElseIf termList.ContainsKey(gene.locus_tag.name) Then
                    termIds = termList(gene.locus_tag.name)
                Else
                    For Each id As String In gene.alias.JoinIterates(gene.term_id.Select(Function(t) t.text))
                        If termList.ContainsKey(id) Then
                            termIds = termList(id)
                            Exit For
                        End If
                    Next
                End If

                If Not termIds.IsNullOrEmpty Then
                    gene.term_id = termIds _
                        .Select(Function(id)
                                    Return New NamedValue With {
                                        .name = term_name,
                                        .text = id
                                    }
                                End Function) _
                        .JoinIterates(gene.term_id) _
                        .ToArray
                End If
            Next
        Next

        Return background
    End Function

    ''' <summary>
    ''' do id mapping of the members in the background cluster
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="mapping">
    ''' do id translation via this id source list
    ''' </param>
    ''' <param name="subset">
    ''' only the cluster which has the member gene id exists in this
    ''' collection then the cluster will be keeps from the result
    ''' background if this parameter is not null or empty.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("background.id_mapping")>
    <RApiReturn(GetType(Background))>
    Public Function BackgroundIDmapping(background As Background, mapping As list,
                                        Optional subset As String() = Nothing,
                                        Optional env As Environment = Nothing) As Object

        Dim maps As Dictionary(Of String, String()) = mapping.AsGeneric(Of String())(env, [default]:={})
        Dim filterIndex As Index(Of String) = subset.Indexing
        Dim newClusterList = background.clusters _
            .Select(Function(c)
                        Return c.id_translation(maps)
                    End Function) _
            .Where(Function(c) c.members.Length > 0) _
            .ToArray

        If filterIndex > 0 Then
            newClusterList = newClusterList _
                .Where(Function(c)
                           Return c.memberIds.Any(Function(id) id Like filterIndex)
                       End Function) _
                .ToArray
        End If

        Return New Background With {
            .build = Now,
            .clusters = newClusterList,
            .comments = background.comments,
            .id = background.id,
            .name = background.name,
            .size = .clusters.BackgroundSize
        }
    End Function

    ''' <summary>
    ''' Do translation of the <see cref="BackgroundGene.accessionID"/>
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    <Extension>
    Private Function id_translation(g As BackgroundGene, maps As Dictionary(Of String, String())) As IEnumerable(Of BackgroundGene)
        Dim multipleID As String() = maps.TryGetValue(g.accessionID)

        If multipleID.IsNullOrEmpty Then
            Return Nothing
        End If

        Return multipleID _
            .Distinct _
            .Select(Function(mapId)
                        Return New BackgroundGene With {
                            .accessionID = mapId,
                            .[alias] = g.alias _
                                .SafeQuery _
                                .Select(AddressOf maps.TryGetValue) _
                                .Where(Function(id) Not id.IsNullOrEmpty) _
                                .IteratesALL _
                                .Distinct _
                                .ToArray,
                            .locus_tag = g.locus_tag,
                            .name = g.name,
                            .term_id = g.term_id
                        }
                    End Function)
    End Function

    ''' <summary>
    ''' Create the id translation of the <see cref="BackgroundGene"/> inside 
    ''' the given <see cref="Cluster"/> model <paramref name="c"/>.
    ''' </summary>
    ''' <param name="c"></param>
    ''' <param name="maps"></param>
    ''' <returns></returns>
    <Extension>
    Private Function id_translation(c As Cluster, maps As Dictionary(Of String, String())) As Cluster
        Dim geneList As BackgroundGene() = c.members _
            .Select(Function(g)
                        Return g.id_translation(maps)
                    End Function) _
            .IteratesALL _
            .Where(Function(g)
                       ' skip of all genes with no id mapping result
                       Return g IsNot Nothing AndAlso Not g.accessionID.StringEmpty
                   End Function) _
            .ToArray

        Return New Cluster With {
            .description = c.description,
            .ID = c.ID,
            .names = c.names,
            .members = geneList
        }
    End Function

    ''' <summary>
    ''' Load GSEA background model from a xml file.
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <example>
    ''' let kb = read.background("hsa.xml");
    ''' </example>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("read.background")>
    Public Function ReadBackground(file As String) As Background
        Return file.LoadXml(Of Background)
    End Function

    ''' <summary>
    ''' Save GSEA background model as xml file
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="file"></param>
    ''' <returns></returns>
    ''' <example>
    ''' write.background(kb, "hsa.xml");
    ''' </example>
    <ExportAPI("write.background")>
    Public Function WriteBackground(background As Background, file$) As Boolean
        Return background.GetXml.SaveTo(file)
    End Function

    ''' <summary>
    ''' make gsea background dynamic cut
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="annotated"></param>
    ''' <returns></returns>
    <ExportAPI("cut_background")>
    Public Function cut_background(background As Background, <RRawVectorArgument> annotated As Object) As Background
        Dim takes As Index(Of String) = CLRVector.asCharacter(annotated).Indexing

        If takes.Count = 0 Then
            Call "no annotated id for make the background dynamic cut!".Warning
            Return Nothing
        End If

        background = New Background With {
            .build = Now,
            .comments = background.comments,
            .id = background.id,
            .name = background.name,
            .clusters = background.clusters _
                .Select(Function(c)
                            Return New Cluster With {
                                .category = c.category,
                                .[class] = c.class,
                                .description = c.description,
                                .ID = c.ID,
                                .names = c.names,
                                .members = c.members _
                                    .Where(Function(a)
                                               Return a.accessionID Like takes OrElse (a.locus_tag IsNot Nothing AndAlso a.locus_tag.name Like takes)
                                           End Function) _
                                    .ToArray
                            }
                        End Function) _
                .Where(Function(c) c.size > 0) _
                .ToArray
        }
        background.size = background.clusters.BackgroundSize

        Return background
    End Function

    ''' <summary>
    ''' summary of the background model as dataframe
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns>row item is the cluster object</returns>
    <ExportAPI("background_summary")>
    Public Function backgroundSummary(background As Background, Optional gene_names As Boolean = True) As Rdataframe
        Dim table As New Rdataframe With {
            .columns = New Dictionary(Of String, Array),
            .rownames = background.clusters _
                .Select(Function(c) c.ID) _
                .ToArray
        }

        Call table.add("class", background.clusters.Select(Function(a) a.class))
        Call table.add("category", background.clusters.Select(Function(a) a.category))
        Call table.add("name", background.clusters.Select(Function(c) c.names))
        Call table.add("description", background.clusters.Select(Function(c) c.description.TrimNewLine.Trim))
        Call table.add("cluster_size", background.clusters.Select(Function(c) c.size))
        Call table.add("factors", background.clusters _
            .Select(Iterator Function(c) As IEnumerable(Of String)
                        If gene_names Then
                            For Each gene As BackgroundGene In c.members
                                Yield gene.name
                            Next
                        Else
                            For Each gene As BackgroundGene In c.members
                                Yield gene.accessionID
                            Next
                        End If
                    End Function) _
            .Select(Function(a) a.Distinct.JoinBy("; ")))

        Return table
    End Function

    ''' <summary>
    ''' get cluster info data table
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="clusterId"></param>
    ''' <returns></returns>
    <ExportAPI("clusterInfo")>
    Public Function GetCluster(background As Background, clusterId As String) As Rdataframe
        Dim cluster As Cluster = background.clusters _
            .Where(Function(c) c.ID = clusterId) _
            .FirstOrDefault
        Dim data As New Rdataframe With {
            .columns = New Dictionary(Of String, Array)
        }

        If Not cluster Is Nothing Then
            data.rownames = cluster.members _
                .Select(Function(gene) gene.accessionID) _
                .ToArray

            data.columns("clusterId") = {cluster.ID}
            data.columns("clusterName") = {cluster.names}
            data.columns("name") = cluster.members.Select(Function(gene) gene.name).ToArray
            data.columns("locus_tag") = cluster.members.Select(Function(gene) If(gene.locus_tag Is Nothing, "", gene.locus_tag.name)).ToArray
            data.columns("description") = cluster.members.Select(Function(gene) If(gene.locus_tag Is Nothing, "", gene.locus_tag.text)).ToArray
            data.columns("terms") = cluster.members.Select(Function(gene) gene.term_id.JoinBy("; ")).ToArray
        End If

        Return data
    End Function

    ''' <summary>
    ''' make gene set annotation via a given gsea background model
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="geneSet"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("geneSet.annotations")>
    Public Function geneSetAnnotation(background As Background,
                                      <RRawVectorArgument>
                                      geneSet As Object,
                                      Optional env As Environment = Nothing) As Object

        If TypeOf geneSet Is Rdataframe Then
            ' the row names is the gene id set
            Dim geneId As String() = DirectCast(geneSet, Rdataframe).rownames
            Dim append = background.geneSetAnnotation(geneId, DirectCast(geneSet, Rdataframe))

            Return append
        Else
            Dim idSet As pipeline = pipeline.TryCreatePipeline(Of String)(geneSet, env)

            If idSet.isError Then
                Return idSet.getError
            Else
                Dim geneId As String() = idSet _
                    .populates(Of String)(env) _
                    .ToArray
                Dim empty As New Rdataframe With {
                    .rownames = geneId,
                    .columns = New Dictionary(Of String, Array)
                }

                Return background.geneSetAnnotation(geneId, empty)
            End If

            Return RInternal.debug.stop(New NotImplementedException, env)
        End If
    End Function

    <Extension>
    Private Function geneSetAnnotation(background As Background, geneSet As String(), table As Rdataframe) As Rdataframe
        Dim genes As BackgroundGene() = geneSet.Select(Function(id) background.GetBackgroundGene(id)).ToArray
        Dim geneNames As String() = genes.Select(Function(g) If(g Is Nothing, "", g.name)).ToArray
        Dim desc As String() = genes _
            .Select(Function(g) If(g Is Nothing OrElse g.locus_tag Is Nothing, "", g.locus_tag.text)) _
            .ToArray

        Call table.add("geneName", geneNames)
        Call table.add("description", desc)

        Return table
    End Function

    ''' <summary>
    ''' get an intersection id list between the background
    ''' model and the given gene id list.
    ''' </summary>
    ''' <param name="cluster">
    ''' A gene cluster model or gsea background model object.
    ''' </param>
    ''' <param name="geneSet"></param>
    ''' <param name="isLocusTag"></param>
    ''' <param name="get_clusterID">
    ''' this function will returns a set of mapping cluster id if this 
    ''' parameter value is set to value TRUE, otherwise this function
    ''' returns a set of intersected geneID list by default.
    ''' 
    ''' this parameter only works when the cluster object is a 
    ''' gsea background model object.
    ''' </param>
    ''' <returns>
    ''' a character vector of the intersected gene id set or the cluster id set based on the option of parameter <paramref name="get_clusterID"/>.
    ''' </returns>
    ''' <example>
    ''' let kb = read.background("kegg_background.xml");
    ''' let idset = c("id1","id2","id3");
    ''' 
    ''' print("intersect gene ids:");
    ''' print(kb |> geneSet.intersects(idset));
    ''' 
    ''' print("intersect cluster ids:");
    ''' print(kb |> geneSet.intersects(idset, get_clusterID=TRUE));
    ''' </example>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("geneSet.intersects")>
    <RApiReturn(GetType(String))>
    Public Function ClusterIntersections(cluster As Object, geneSet$(),
                                         Optional isLocusTag As Boolean = False,
                                         Optional get_clusterID As Boolean = False,
                                         Optional term_map As Boolean = False,
                                         Optional id_map As Boolean = False,
                                         Optional env As Environment = Nothing) As Object
        If cluster Is Nothing Then
            Return Nothing
        End If
        If TypeOf cluster Is Cluster Then
            If term_map Then
                Dim c As Cluster = DirectCast(cluster, Cluster)
                Dim map_term As New List(Of String)

                For Each id As String In geneSet
                    Call map_term.Add(c.GetMemberById(id).getTermId(id, id_map))
                Next

                Return New dataframe With {
                    .columns = New Dictionary(Of String, Array) From {
                        {"id", geneSet},
                        {"term", map_term.ToArray}
                    }
                }
            Else
                Return DirectCast(cluster, Cluster) _
                    .Intersect(geneSet, isLocusTag) _
                    .ToArray
            End If
        ElseIf TypeOf cluster Is Background Then
            If term_map Then
                Dim kb As Background = DirectCast(cluster, Background)
                Dim map_term As New List(Of String)

                For Each id As String In geneSet
                    Dim gene As BackgroundGene = Nothing

                    For Each c As Cluster In kb.clusters
                        gene = c.GetMemberById(id)

                        If Not gene Is Nothing Then
                            Exit For
                        End If
                    Next

                    Call map_term.Add(gene.getTermId(id, id_map))
                Next

                Return New dataframe With {
                    .columns = New Dictionary(Of String, Array) From {
                        {"id", geneSet},
                        {"term", map_term.ToArray}
                    }
                }
            ElseIf get_clusterID Then
                Return DirectCast(cluster, Background).clusters _
                    .Where(Function(c)
                               Return c.Intersect(geneSet, isLocusTag).Any
                           End Function) _
                    .Select(Function(c) c.ID) _
                    .ToArray
            Else
                Return DirectCast(cluster, Background).clusters _
                    .Select(Function(c)
                                Return c.Intersect(geneSet, isLocusTag)
                            End Function) _
                    .IteratesALL _
                    .Distinct _
                    .ToArray
            End If
        Else
            Return Message.InCompatibleType(GetType(Background), cluster.GetType, env)
        End If
    End Function

    <Extension>
    Private Function getTermId(gene As BackgroundGene, id As String, id_map As Boolean) As String
        If gene Is Nothing Then
            Return ""
        Else
            Dim term = gene.term_id.SafeQuery.FirstOrDefault

            If Not term Is Nothing Then
                id = term.name
            ElseIf id_map Then
                id = id
            Else
                id = Nothing
            End If

            Return id
        End If
    End Function

    ''' <summary>
    ''' make filter of the background model 
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="geneSet">usually be a character of the gene id set.</param>
    ''' <param name="min_size">the min feature size is required for each cluster. 
    ''' all of the cluster that have the feature number less than this cutoff 
    ''' will be removed from the background.</param>
    ''' <param name="max_intersects">the max intersect number that each cluster 
    ''' intersect with the input geneSet. all of the clusters that greater than 
    ''' this value will be removed from the background.</param>
    ''' <returns>
    ''' a new background model that has cluster filtered by the given rule.
    ''' </returns>
    ''' <example>
    ''' let kb = read.background("hsa.xml");
    ''' let idset = c("id1","id2","id3");
    ''' let filter_kb = kb |> geneSet.filter(idset, min.size=5, max.intersects=500);
    ''' 
    ''' print(background_summary(filter_kb));
    ''' </example>
    <ExportAPI("geneSet.filter")>
    <RApiReturn(GetType(Background))>
    Public Function ClusterFilter(background As Background,
                                  <RRawVectorArgument>
                                  Optional geneSet As Object = Nothing,
                                  Optional min_size As Integer = 3,
                                  Optional max_intersects As Integer = 500,
                                  <RRawVectorArgument>
                                  Optional remove_clusters As Object = Nothing,
                                  Optional env As Environment = Nothing) As Object

        Dim idset As String() = CLRVector.asCharacter(geneSet).SafeQuery.Distinct.ToArray
        Dim removeClusters As Index(Of String) = CLRVector _
            .asCharacter(remove_clusters) _
            .Indexing

        If idset.IsNullOrEmpty AndAlso removeClusters.Count = 0 Then
            Return RInternal.debug.stop("the required gene idset for test intersect or the cluster id collection for make filtered from the background should not be empty!", env)
        End If

        Dim filtered As Cluster() = background.clusters _
            .Where(Function(c)
                       Dim test1 As Boolean = True
                       Dim test2 As Boolean = True

                       If Not idset.IsNullOrEmpty Then
                           test1 = c.size >= min_size AndAlso
                               c.Intersect(idset).Count <= max_intersects
                       End If
                       If removeClusters.Count > 0 Then
                           test2 = Not (c.ID Like removeClusters)
                       End If

                       Return test1 AndAlso test2
                   End Function) _
            .ToArray

        If filtered.IsNullOrEmpty Then
            Return RInternal.debug.stop($"no cluster left after filter by the given feature size range(min.size={min_size}, max.intersects={max_intersects})!", env)
        Else
            Return New Background With {
                .build = Now,
                .comments = background.comments,
                .id = background.id,
                .name = background.name,
                .clusters = filtered,
                .size = filtered.BackgroundSize
            }
        End If
    End Function

    ''' <summary>
    ''' convert the background model to a data table
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("KO.table")>
    Public Function KOTable(background As Background) As EntityObject()
        Return background.clusters _
            .Select(Function(c) c.members) _
            .IteratesALL _
            .GroupBy(Function(gene) gene.accessionID) _
            .Select(Function(gene)
                        Return New EntityObject With {
                            .ID = gene.Key,
                            .Properties = New Dictionary(Of String, String) From {
                                {"KO", gene.First.term_id(Scan0).text}
                            }
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Create a cluster for gsea background
    ''' </summary>
    ''' <param name="x">
    ''' id, name data fields should be exists in current dataframe object, 
    ''' other data fields will be used as the gene member terms
    ''' </param>
    ''' <param name="clusterId">id of the cluster</param>
    ''' <param name="clusterName">display name of the cluster model</param>
    ''' <param name="desc">
    ''' the description of the cluster model 
    ''' </param>
    ''' <param name="id">
    ''' the field column name for get gene members id
    ''' </param>
    ''' <param name="name">
    ''' the field column name for get gene members name
    ''' </param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the input dataframe could be a set of database xrefs, example as:
    ''' 
    ''' |xref|name|alias|KEGG|uniprot|
    ''' |----|----|-----|----|-------|
    ''' |    |    |     |    |       |
    ''' 
    ''' </remarks>
    <ExportAPI("gsea_cluster")>
    Public Function CreateCluster(x As Rdataframe, clusterId$, clusterName$,
                                  Optional desc$ = "n/a",
                                  Optional id$ = "xref",
                                  Optional name$ = "name") As Cluster

        Dim fields As Dictionary(Of String, String()) = x.columns _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return CLRVector.asCharacter(a.Value)
                          End Function)
        Dim idvec As String() = fields(id)
        Dim namevec As String() = fields(name)
        Dim [alias] As String() = fields.TryGetValue("alias")

        Call fields.Remove(id)
        Call fields.Remove(name)
        Call fields.Remove("alias")

        Dim cluster As New Cluster With {
            .ID = clusterId,
            .description = desc.TrimNewLine().StringReplace("\s{2,}", " "),
            .names = clusterName.TrimNewLine().StringReplace("\s{2,}", " "),
            .members = idvec _
                .Select(Function(idstr, i)
                            Dim name_str As String = namevec(i)
                            Dim alias_id = [alias].ElementAtOrNull(i)
                            Dim alias_set = alias_id.StringSplit("\s*;\s*")

                            Return fields.getMemberItem(idstr, i, name_str, alias_set)
                        End Function) _
                .ToArray
        }

        Return cluster
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fields"></param>
    ''' <param name="idStr"></param>
    ''' <param name="i">the row index of current gene</param>
    ''' <param name="name"></param>
    ''' <returns></returns>
    <Extension>
    Private Function getMemberItem(fields As Dictionary(Of String, String()), idStr As String, i As Integer, name As String, alias_id As String()) As BackgroundGene
        ' scan for other database reference id
        ' slice current row which is reference by the row index i
        Dim terms As Dictionary(Of String, String) = fields _
            .Where(Function(a) Not a.Value(i).StringEmpty) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.Value(i)
                          End Function)
        Dim termList As New List(Of NamedValue)

        For Each tuple As KeyValuePair(Of String, String) In terms
            For Each term_id As String In tuple.Value.StringSplit("\s*;\s*")
                Call termList.Add(New NamedValue With {
                    .name = tuple.Key,
                    .text = term_id
                })
            Next
        Next

        Return New BackgroundGene With {
            .accessionID = idStr,
            .[alias] = alias_id,
            .locus_tag = New NamedValue With {
                .name = idStr,
                .text = name
            },
            .name = name,
            .term_id = termList.ToArray
        }
    End Function

    ''' <summary>
    ''' Create the gsea background model for metabolism analysis
    ''' </summary>
    ''' <param name="kegg">the kegg <see cref="Pathway"/> model collection of current organism or 
    ''' the KEGG <see cref="Map"/> data collection.
    ''' andalso could be a tuple list of the idset.
    ''' </param>
    ''' <param name="reactions">A collection of the reference <see cref="ReactionTable"/> model 
    ''' data for build the metabolism network</param>
    ''' <param name="org_name"></param>
    ''' <param name="is_ko_ref"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("metpa")>
    <RApiReturn(GetType(metpa))>
    Public Function create_metpa(<RRawVectorArgument> kegg As Object,
                                 <RRawVectorArgument> reactions As Object,
                                 Optional org_name As String = Nothing,
                                 Optional is_ko_ref As Boolean = False,
                                 Optional multipleOmics As Boolean = False,
                                 Optional env As Environment = Nothing) As Object

        Dim pathways As pipeline = pipeline.TryCreatePipeline(Of Pathway)(kegg, env)
        Dim reactionList As pipeline = pipeline.TryCreatePipeline(Of ReactionTable)(reactions, env, suppress:=True)
        Dim ignoreEnzymes As Boolean = False

        If pathways.isError Then
            pathways = pipeline.TryCreatePipeline(Of Map)(kegg, env)

            If pathways.isError Then
                If TypeOf kegg Is list Then
                    ' a tuple list of the idset
                    ' convert to a pathway collection object
                    pathways = pipeline.CreateFromPopulator(ParsePathwayObject(kegg))
                    ignoreEnzymes = True
                Else
                    Return pathways.getError
                End If
            End If
        End If
        If reactionList.isError Then
            reactionList = pipeline.TryCreatePipeline(Of Reaction)(reactions, env)

            If reactionList.isError Then
                Return reactionList.getError
            End If

            reactionList = reactionList _
                .populates(Of Reaction)(env) _
                .DoCall(AddressOf ReactionTable.Load) _
                .ToArray _
                .DoCall(AddressOf pipeline.CreateFromPopulator)
        End If

        If pathways.elementType Like GetType(Map) Then
            Return EnrichmentNetwork.KEGGModels(
                models:=pathways.populates(Of Map)(env).ToArray,
                isKo_ref:=is_ko_ref,
                reactions:=reactionList.populates(Of ReactionTable)(env).CreateIndex(indexByCompounds:=True),
                orgName:=org_name,
                multipleOmics:=multipleOmics,
                ignoreEnzymes:=ignoreEnzymes
            )
        ElseIf pathways.elementType Like GetType(Pathway) Then
            Return EnrichmentNetwork.KEGGModels(
                models:=pathways.populates(Of Pathway)(env).ToArray,
                isKo_ref:=is_ko_ref,
                reactions:=reactionList.populates(Of ReactionTable)(env).CreateIndex(indexByCompounds:=True),
                orgName:=org_name,
                multipleOmics:=multipleOmics,
                ignoreEnzymes:=ignoreEnzymes
            )
        Else
            Return RInternal.debug.stop(New NotImplementedException(pathways.elementType.ToString), env)
        End If
    End Function

    ''' <summary>
    ''' parse the tuple list as the pathway object
    ''' </summary>
    ''' <param name="idset">An id collection</param>
    ''' <returns></returns>
    Private Iterator Function ParsePathwayObject(idset As list) As IEnumerable(Of Pathway)
        For Each name As String In idset.getNames
            Dim id As String() = CLRVector.asCharacter(idset.slots(name))
            Dim compounds As NamedValue() = id _
                .Select(Function(si) New NamedValue(si, si)) _
                .ToArray

            Yield New Pathway With {
                .name = name,
                .compound = compounds,
                .description = name,
                .EntryId = name
            }
        Next
    End Function

    ''' <summary>
    ''' cast the cluster data as the enrichment background
    ''' </summary>
    ''' <param name="geneSet">
    ''' a collection of the gene feature clusters, usualy be a tuple list in format of:
    ''' cluster id as the list name and the corresponding tuple list value should be
    ''' the gene id character vector.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("fromList")>
    Public Function backgroundFromList(geneSet As list, Optional env As Environment = Nothing) As Object
        If geneSet Is Nothing OrElse geneSet.is_empty Then
            Call env.AddMessage("the input gene feature set should not be nothing!", MSG_TYPES.WRN)
            Return Nothing
        End If

        Return New Background With {
            .clusters = DirectCast(geneSet, list).slots _
                .Select(Function(c)
                            Return New Cluster With {
                                .ID = c.Key,
                                .description = c.Key,
                                .names = c.Key,
                                .members = CLRVector.asCharacter(c.Value) _
                                    .Select(AddressOf createGene) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' cast the cluster data as the enrichment background
    ''' </summary>
    ''' <param name="clusters">
    ''' a data cluster or a collection of kegg pathway model
    ''' </param>
    ''' <param name="background_size">
    ''' default value -1 or zero means auto evaluated
    ''' </param>
    ''' <param name="name">
    ''' the background model name
    ''' </param>
    ''' <param name="tax_id">
    ''' ncbi taxonomy id of the target organism
    ''' </param>
    ''' <param name="desc">
    ''' the model description
    ''' </param>
    ''' <param name="omics">
    ''' Create a enrichment background model for run multiple omics data analysis?
    ''' this parameter is only works for the kegg pathway model where you are 
    ''' speicifc via the <paramref name="clusters"/> parameter.
    ''' </param>
    ''' <param name="filter_compoundId">
    ''' do compound id filtering when target model is <paramref name="omics"/>?
    ''' (all of the KEGG drug id and KEGG glycan id will be removed from the cluster model)
    ''' </param>
    ''' <param name="kegg_code">
    ''' the kegg organism code when the given <paramref name="clusters"/> collection is
    ''' a collection of the pathway object.
    ''' </param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("as.background")>
    <RApiReturn(GetType(Background))>
    Public Function assembleBackground(<RRawVectorArgument> clusters As Object,
                                       Optional background_size% = -1,
                                       Optional name$ = "n/a",
                                       Optional tax_id$ = "n/a",
                                       Optional desc$ = "n/a",
                                       Optional omics As OmicsData = OmicsData.Transcriptomics,
                                       Optional filter_compoundId As Boolean = True,
                                       Optional kegg_code As String = Nothing,
                                       Optional env As Environment = Nothing) As Object

        Dim clusterList As pipeline = pipeline.TryCreatePipeline(Of Cluster)(clusters, env, suppress:=True)
        Dim clusterVec As Cluster()

        If clusterList.isError Then
            clusterList = pipeline.TryCreatePipeline(Of Pathway)(clusters, env)

            If clusterList.isError Then
                Return clusterList.getError
            Else
                If omics = OmicsData.MultipleOmics Then
                    Dim kegg_pathways = clusterList.populates(Of Pathway)(env)

                    Return MultipleOmics.CreateOmicsBackground(
                        model:=kegg_pathways,
                        filter_compoundId:=filter_compoundId,
                        kegg_code:=kegg_code
                    )
                Else
                    Return clusterList.populates(Of Pathway)(env).CreateModel(omics)
                End If
            End If
        Else
            clusterVec = clusterList.populates(Of Cluster)(env) _
                .Where(Function(c) c.size > 0 AndAlso Not c.ID.StringEmpty) _
                .ToArray

            If background_size <= 0 Then
                background_size = clusterVec.BackgroundSize
            End If
        End If

        Dim background As New Background With {
            .clusters = clusterVec,
            .build = Now,
            .comments = desc,
            .id = tax_id,
            .name = name,
            .size = background_size
        }

        Return background
    End Function

    ''' <summary>
    ''' gene/protein KO id background
    ''' </summary>
    ''' <returns>
    ''' A reference background of the kegg pathway by parse the internal resource file.
    ''' </returns>
    <ExportAPI("KO_reference")>
    Public Function CreateKOReference() As Background
        Dim ko00001 = htext.ko00001.Hierarchical.categoryItems
        Dim clusters As Cluster() = ko00001 _
            .Select(Function(category)
                        Return category.KO_category
                    End Function) _
            .IteratesALL _
            .ToArray

        Return New Background With {
            .clusters = clusters,
            .build = Now,
            .comments = "The KEGG orthology reference pathway model",
            .id = NameOf(ko00001),
            .name = "KEGG Orthology (KO)"
        }
    End Function

    ''' <summary>
    ''' Extract the gene set list from the background model
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns>
    ''' a tuple list object that contains the gene set information,
    ''' data result in format like:
    ''' 
    ''' ```r
    ''' list(
    '''     "cluster id 1" = c("gene id", "gene id", ...),
    '''     "cluster id 2" = c("gene id", "gene id", ...),
    '''     ...
    ''' )
    ''' ```
    ''' </returns>
    ''' <remarks>
    ''' the result list could be used for save as json file for 
    ''' parsed in R by ``jsonlite::fromJSON`` function, and used
    ''' for the gsva analysis.
    ''' </remarks>
    ''' <example>
    ''' let kb = read.background("hsa.xml");
    ''' let geneSet = as.geneSet(kb);
    ''' 
    ''' # save as json file
    ''' geneSet 
    ''' |> JSON::json_encode()
    ''' |> writeLines(con = "hsa.json")
    ''' ;
    ''' 
    ''' # load in R
    ''' library(jsonlite);
    ''' geneSet = jsonlite::fromJSON("hsa.json");
    ''' 
    ''' # use for gsva analysis
    ''' gsva(data, geneSet, method="gsva", ...);
    ''' </example>
    <ExportAPI("as.geneSet")>
    Public Function asGenesetList(background As Background) As list
        Return New list With {
            .slots = background _
                .clusters _
                .ToDictionary(Function(c)
                                  Return $"{c.ID} - {c.names}"
                              End Function,
                              Function(c)
                                  Return CObj(c.members.Select(Function(d) d.accessionID).ToArray)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' create kegg maps background for the metabolism data analysis
    ''' </summary>
    ''' <param name="kegg">Should be a collection of the kegg map object</param>
    ''' <returns></returns>
    <ExportAPI("metabolism.background")>
    <RApiReturn(GetType(Background))>
    Public Function metabolismBackground(<RRawVectorArgument>
                                         kegg As Object,
                                         Optional filter As String() = Nothing,
                                         Optional env As Environment = Nothing) As Object

        Dim mapIdFilter As Index(Of String) = filter _
            .SafeQuery _
            .Select(Function(id) id.Match("\d+")) _
            .Indexing
        Dim maps As Map()

        If TypeOf kegg Is MapRepository Then
            maps = DirectCast(kegg, MapRepository).Maps _
                .Select(Function(m) DirectCast(m, Map)) _
                .ToArray
        Else
            Dim pull As pipeline = pipeline.TryCreatePipeline(Of Map)(kegg, env)

            If pull.isError Then
                Return pull.getError
            End If

            maps = pull.populates(Of Map)(env).ToArray
        End If

        Dim background As Background = maps _
            .Where(Function(map)
                       If mapIdFilter.Count > 0 Then
                           Return map.EntryId.Match("\d+") Like mapIdFilter
                       Else
                           Return True
                       End If
                   End Function) _
            .CreateGeneralBackground

        Return background
    End Function

    ''' <summary>
    ''' get kegg compound class brite background model
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' this function generates a background model from the internal
    ''' kegg resource database, the generated background model which 
    ''' could be used for the compound class category annotation.
    ''' </remarks>
    <ExportAPI("compoundBrite")>
    Public Function KEGGCompoundBriteClassBackground() As Background
        Return GSEATools.CompoundBriteBackground
    End Function

    ''' <summary>
    ''' create kegg background model
    ''' </summary>
    ''' <param name="genes">a set of molecules with the kegg orthology/compound id mapping</param>
    ''' <param name="maps">the kegg maps model</param>
    ''' <param name="size">the background gene size, default -1 or zero means auto calculation.</param>
    ''' <param name="genomeName">the genome name that tagged with this background model.</param>
    ''' <returns></returns>
    <ExportAPI("KO.background")>
    <RApiReturn(GetType(Background))>
    Public Function CreateKOBackground(<RRawVectorArgument> genes As Object,
                                       <RRawVectorArgument> maps As Object,
                                       Optional size% = -1,
                                       Optional genomeName$ = "unknown",
                                       <RRawVectorArgument>
                                       Optional id_map As Object = Nothing,
                                       Optional multiple_omics As Boolean = False,
                                       Optional term_db As String = "unknown",
                                       Optional env As Environment = Nothing) As Object
        Dim geneId, KO, name As String
        Dim kegg As GetClusterTerms
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of Map)(maps, env, suppress:=True)

        If pull.isError Then
            If TypeOf maps Is MapRepository Then
                kegg = GSEATools.KEGGClusters(DirectCast(maps, MapRepository).Maps)
            ElseIf TypeOf maps Is htext Then
                kegg = GSEATools.KEGGClusters(DirectCast(maps, htext))
            Else
                Return Message.InCompatibleType(GetType(htext), maps.GetType, env)
            End If
        Else
            Dim kegg_maps As Map() = pull.populates(Of Map)(env).ToArray
            Dim clusters As New KOMapCluster(kegg_maps, multipleOmics:=multiple_omics)

            kegg = AddressOf clusters.KOIDMap
        End If

        If Not id_map Is Nothing Then
            If TypeOf id_map Is list Then
                With DirectCast(id_map, list)
                    If .length = 1 Then
                        ' list(geneid = ko);
                        geneId = .slots.Keys.First
                        KO = any.ToString([single](.slots(geneId)))
                        name = Nothing
                    Else
                        ' list(geneid = xxx, ko = xxx, name = xxx);
                        geneId = CLRVector.asScalarCharacter(.getBySynonyms("geneid", "geneID", "gene_id", "GeneID", "Gene ID"))
                        KO = CLRVector.asScalarCharacter(.getBySynonyms("ko", "KO", "kegg_id", "KEGG", "KEGG ID"))
                        name = CLRVector.asScalarCharacter(.getBySynonyms("name", "Name", "gene_name", "gene name", "commonName", "common name", "symbol"))
                    End If
                End With
            Else
                ' is vector
                ' c(gene_id, ko, name);
                Dim vec As String() = CLRVector.asCharacter(id_map)

                geneId = vec.ElementAtOrNull(0)
                KO = vec.ElementAtOrNull(1)
                name = vec.ElementAtOrNull(2)
            End If
        Else
            KO = Nothing
            geneId = Nothing
            name = Nothing
        End If

        ' [geneID -> KO] mapping
        Dim mapping As NamedValue(Of String)()
        Dim mapsResult = MapBackground.KOMaps(genes, geneId, KO, name, env)

        If TypeOf mapsResult Is Message Then
            Return mapsResult
        Else
            mapping = mapsResult
        End If

        Dim keggSet = mapping _
            .Where(Function(gene) Not gene.Value.StringEmpty(, True)) _
            .GroupBy(Function(gene) gene.Value) _
            .Select(Function(ko_set)
                        Return New NamedCollection(Of String)(
                            ko_set.Key,
                            ko_set.Keys,
                            ko_set.Select(Function(i) i.Description) _
                                .Where(Function(desc)
                                           Return Not desc.StringEmpty(, True)
                                       End Function) _
                                .Distinct _
                                .JoinBy("; "))
                    End Function) _
            .ToArray
        Dim model As Background = GSEATools.CreateBackground(
            db:=keggSet,
            createGene:=AddressOf createGene,
            getTerms:=Function(gene)
                          Return {gene.name}
                      End Function,
            define:=kegg,
            genomeName:=genomeName
        )
        model.size = size

        Return model
    End Function

    Private Function createGene(gene As NamedCollection(Of String), terms As String()) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = gene.name,
            .[alias] = gene.value,
            .locus_tag = New NamedValue With {
                .name = gene.name,
                .text = If(gene.description.StringEmpty, gene.value.JoinBy("; "), gene.description)
            },
            .name = gene.name,
            .term_id = BackgroundGene.UnknownTerms(terms).ToArray
        }
    End Function

    Private Function createGene(name As String) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = name,
            .name = name,
            .[alias] = {name},
            .locus_tag = New NamedValue With {
                .name = name,
                .text = name
            },
            .term_id = BackgroundGene.UnknownTerms(name).ToArray
        }
    End Function
End Module
