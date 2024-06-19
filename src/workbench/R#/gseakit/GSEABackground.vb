﻿#Region "Microsoft.VisualBasic::9f8a7d712b82e837482a02a767207473, R#\gseakit\GSEABackground.vb"

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

'   Total Lines: 929
'    Code Lines: 634 (68.25%)
' Comment Lines: 200 (21.53%)
'    - Xml Docs: 96.50%
' 
'   Blank Lines: 95 (10.23%)
'     File Size: 37.76 KB


' Module GSEABackground
' 
'     Function: appendIdTerms, asGenesetList, assembleBackground, BackgroundIDmapping, backgroundSummary
'               clusterIDs, ClusterIntersections, create_metpa, CreateCluster, createGene
'               CreateKOBackground, CreateKOReference, DAGbackground, (+2 Overloads) geneSetAnnotation, GetCluster
'               getMemberItem, (+2 Overloads) id_translation, KEGGCompoundBriteClassBackground, KOTable, metabolismBackground
'               MetaEnrichBackground, moleculeIDs, ParsePathwayObject, PrintBackground, ReadBackground
'               WriteBackground
' 
'     Sub: Main
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
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
    End Sub

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
    ''' <returns></returns>
    <ExportAPI("clusterIDs")>
    Public Function clusterIDs(background As Background) As String()
        Return background.clusters.Select(Function(a) a.ID).ToArray
    End Function

    ''' <summary>
    ''' get all of the molecule id set from the given background model object
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns></returns>
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
    Public Function DAGbackground(dag As GO_OBO, Optional flat As Boolean = False, Optional env As Environment = Nothing) As Background
        Dim getCluster = dag.terms.GOClusters
        Dim background = dag.terms _
            .Select(Function(t) t.id) _
            .CreateGOGeneric(getCluster, dag.terms.Length)

        If flat Then
            Dim ontology As New GeneOntology.DAG.Graph(DirectCast(dag, GO_OBO).AsEnumerable)
            Dim index = background.GetClusterTable
            Dim clusters As New List(Of Cluster)
            Dim n As Integer = background.clusters.Length
            Dim d As Integer = n / 20
            Dim pc As New PerformanceCounter
            Dim i As i32 = 0
            Dim println = env.WriteLineHandler

            Call pc.Set()

            For Each cluster As Cluster In background.clusters
                cluster = cluster.PullOntologyTerms(ontology, index)
                clusters.Add(cluster)

                If ++i Mod d = 0 Then
                    Call println(pc.Mark($"({i}/{n})  extract DAG graph ... {(i / n * 100).ToString("F1")}% {cluster.names}").ToString)
                End If
            Next

            background.clusters = clusters.ToArray
            background.size = background.clusters.BackgroundSize
        End If

        background.name = dag.headers.Ontology
        background.id = background.name
        background.comments = dag.ToString

        Return background
    End Function

    <ExportAPI("append.id_terms")>
    Public Function appendIdTerms(background As Background,
                                  term_name As String,
                                  terms As list,
                                  Optional env As Environment = Nothing) As Object

        Dim termList = terms.AsGeneric(Of String())(env)

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
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("read.background")>
    Public Function ReadBackground(file As String) As Background
        Return file.LoadXml(Of Background)
    End Function

    ''' <summary>
    ''' Save GSEA background model as xml file
    ''' </summary>
    ''' <param name="background"></param>
    ''' <param name="file$"></param>
    ''' <returns></returns>
    <ExportAPI("write.background")>
    Public Function WriteBackground(background As Background, file$) As Boolean
        Return background.GetXml.SaveTo(file)
    End Function

    <ExportAPI("background_summary")>
    Public Function backgroundSummary(background As Background) As Rdataframe
        Dim table As New Dictionary(Of String, Array)

        table("ID") = background.clusters.Select(Function(c) c.ID).ToArray
        table("name") = background.clusters.Select(Function(c) c.names).ToArray
        table("description") = background.clusters.Select(Function(c) c.description).ToArray
        table("cluster_size") = background.clusters.Select(Function(c) c.size).ToArray

        Return New Rdataframe With {
            .columns = table
        }
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

            Return Internal.debug.stop(New NotImplementedException, env)
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
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("geneSet.intersects")>
    <RApiReturn(GetType(String))>
    Public Function ClusterIntersections(cluster As Object, geneSet$(),
                                         Optional isLocusTag As Boolean = False,
                                         Optional get_clusterID As Boolean = False,
                                         Optional env As Environment = Nothing) As Object
        If cluster Is Nothing Then
            Return Nothing
        End If
        If TypeOf cluster Is Cluster Then
            Return DirectCast(cluster, Cluster) _
                .Intersect(geneSet, isLocusTag) _
                .ToArray
        ElseIf TypeOf cluster Is Background Then
            If get_clusterID Then
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
            Return Internal.debug.stop(New NotImplementedException(pathways.elementType.ToString), env)
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
    ''' <param name="is_multipleOmics">
    ''' Create a enrichment background model for run multiple omics data analysis?
    ''' this parameter is only works for the kegg pathway model where you are 
    ''' speicifc via the <paramref name="clusters"/> parameter.
    ''' </param>
    ''' <param name="filter_compoundId">
    ''' do compound id filtering when target model is <paramref name="is_multipleOmics"/>?
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
                                       Optional is_multipleOmics As Boolean = False,
                                       Optional filter_compoundId As Boolean = True,
                                       Optional kegg_code As String = Nothing,
                                       Optional env As Environment = Nothing) As Object

        Dim clusterList As pipeline = pipeline.TryCreatePipeline(Of Cluster)(clusters, env, suppress:=True)
        Dim clusterVec As Cluster()

        If clusterList.isError Then
            clusterList = pipeline.TryCreatePipeline(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)(clusters, env)

            If clusterList.isError Then
                Return clusterList.getError
            Else
                If is_multipleOmics Then
                    Dim kegg_pathways = clusterList.populates(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)(env)

                    Return MultipleOmics.CreateOmicsBackground(
                        model:=kegg_pathways,
                        filter_compoundId:=filter_compoundId,
                        kegg_code:=kegg_code
                    )
                Else
                    Return clusterList.populates(Of SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Pathway)(env).CreateModel
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
    ''' <returns></returns>
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

    <ExportAPI("as.geneSet")>
    Public Function asGenesetList(background As Background) As list
        Return New list With {
            .slots = background _
                .clusters _
                .ToDictionary(Function(c)
                                  Return $"{c.ID} {c.names}"
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
        Return KEGG.CompoundBriteBackground
    End Function

    ''' <summary>
    ''' create kegg background model
    ''' </summary>
    ''' <param name="genes"></param>
    ''' <param name="maps"></param>
    ''' <param name="size%"></param>
    ''' <param name="genomeName$"></param>
    ''' <returns></returns>
    <ExportAPI("KO.background")>
    <RApiReturn(GetType(Background))>
    Public Function CreateKOBackground(<RRawVectorArgument> genes As Object,
                                       <RRawVectorArgument> maps As Object,
                                       Optional size% = -1,
                                       Optional genomeName$ = "unknown",
                                       Optional id_map As Object = Nothing,
                                       Optional env As Environment = Nothing) As Object
        Dim geneId, KO As String
        Dim kegg As GetClusterTerms

        If TypeOf maps Is MapRepository Then
            kegg = GSEATools.KEGGClusters(DirectCast(maps, MapRepository).Maps)
        ElseIf TypeOf maps Is htext Then
            kegg = GSEATools.KEGGClusters(DirectCast(maps, htext))
        Else
            Return Message.InCompatibleType(GetType(htext), maps.GetType, env)
        End If

        If Not id_map Is Nothing Then
            If TypeOf id_map Is list Then
                With DirectCast(id_map, list)
                    geneId = .slots.Keys.First
                    KO = any.ToString([single](.slots(geneId)))
                End With
            Else
                geneId = any.ToString(id_map)
                KO = Nothing
            End If
        Else
            KO = Nothing
            geneId = Nothing
        End If

        ' [geneID -> KO] mapping
        Dim mapping As NamedValue(Of String)()
        Dim mapsResult = MapBackground.KOMaps(genes, geneId, KO, env)

        If TypeOf mapsResult Is Message Then
            Return mapsResult
        Else
            mapping = mapsResult
        End If

        Dim model As Background = GSEATools.CreateBackground(
            db:=mapping _
                .Where(Function(gene) Not gene.Value.StringEmpty) _
                .ToArray,
            createGene:=AddressOf createGene,
            getTerms:=Function(gene)
                          Return {gene.Value}
                      End Function,
            define:=kegg,
            genomeName:=genomeName
        )
        model.size = size

        Return model
    End Function

    Private Function createGene(gene As NamedValue(Of String), terms As String()) As BackgroundGene
        Return New BackgroundGene With {
            .accessionID = gene.Name,
            .[alias] = {gene.Name, gene.Value},
            .locus_tag = New NamedValue With {
                .name = gene.Name,
                .text = gene.Value
            },
            .name = gene.Name,
            .term_id = BackgroundGene.UnknownTerms(terms).ToArray
        }
    End Function
End Module
