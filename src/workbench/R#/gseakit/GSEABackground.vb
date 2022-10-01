#Region "Microsoft.VisualBasic::72a5e101c1450bcea746aff59aa2e1fc, R#\gseakit\GSEABackground.vb"

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

'   Total Lines: 432
'    Code Lines: 330
' Comment Lines: 60
'   Blank Lines: 42
'     File Size: 16.99 KB


' Module GSEABackground
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: asGenesetList, assembleBackground, backgroundSummary, clusterIDs, ClusterIntersections
'               compoundCluster, CreateCluster, createGene, CreateKOBackground, CreateKOReference
'               DAGbackground, GetCluster, KOTable, metabolismBackground, MetaEnrichBackground
'               PrintBackground, ReadBackground, WriteBackground
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports GSEATools = SMRUCC.genomics.Analysis.HTS.GSEA
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

''' <summary>
''' tools for handling GSEA background model.
''' </summary>
<Package("background", Category:=APICategories.ResearchTools)>
Public Module GSEABackground

    Sub New()
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
    ''' <returns></returns>
    <ExportAPI("dag.background")>
    Public Function DAGbackground(dag As GO_OBO) As Background
        Dim getCluster = dag.terms.GOClusters
        Dim background = dag.terms _
            .Select(Function(t) t.id) _
            .CreateGOGeneric(getCluster, dag.terms.Length)

        Return background
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
    ''' get an intersection id list between the background
    ''' model and the given gene id list.
    ''' </summary>
    ''' <param name="cluster">
    ''' A gene cluster model or gsea background model object.
    ''' </param>
    ''' <param name="geneSet"></param>
    ''' <param name="isLocusTag"></param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("geneSet.intersects")>
    <RApiReturn(GetType(String))>
    Public Function ClusterIntersections(cluster As Object, geneSet$(),
                                         Optional isLocusTag As Boolean = False,
                                         Optional env As Environment = Nothing) As Object
        If cluster Is Nothing Then
            Return Nothing
        End If
        If TypeOf cluster Is Cluster Then
            Return DirectCast(cluster, Cluster) _
                .Intersect(geneSet, isLocusTag) _
                .ToArray
        ElseIf TypeOf cluster Is Background Then
            Return DirectCast(cluster, Background).clusters _
                .Select(Function(c)
                            Return c.Intersect(geneSet, isLocusTag)
                        End Function) _
                .IteratesALL _
                .Distinct _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(Background), cluster.GetType, env)
        End If
    End Function

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
                                {"KO", gene.First.term_id(Scan0)}
                            }
                        }
                    End Function) _
            .ToArray
    End Function

    ''' <summary>
    ''' Create a cluster for gsea background
    ''' </summary>
    ''' <param name="data">
    ''' id, name data fields should be exists in current dataframe object
    ''' </param>
    ''' <param name="clusterId$"></param>
    ''' <param name="clusterName$"></param>
    ''' <returns></returns>
    <ExportAPI("gsea.cluster")>
    Public Function CreateCluster(data As Rdataframe, clusterId$, clusterName$, Optional desc$ = "n/a", Optional id$ = "xref", Optional name$ = "name") As Cluster
        Dim idvec As String() = asVector(Of String)(data.columns(id))
        Dim namevec As String() = asVector(Of String)(data.columns(name))
        Dim cluster As New Cluster With {
            .ID = clusterId,
            .description = desc.TrimNewLine().StringReplace("\s{2,}", " "),
            .names = clusterName.TrimNewLine().StringReplace("\s{2,}", " "),
            .members = idvec _
                .Select(Function(idstr, i)
                            Return New BackgroundGene With {
                                .accessionID = idstr,
                                .[alias] = {idstr},
                                .locus_tag = New NamedValue With {
                                    .name = idstr,
                                    .text = namevec(i)
                                },
                                .name = namevec(i),
                                .term_id = {idstr}
                            }
                        End Function) _
                .ToArray
        }

        Return cluster
    End Function

    <ExportAPI("as.background")>
    <RApiReturn(GetType(Background))>
    Public Function assembleBackground(<RRawVectorArgument> clusters As Object,
                                       Optional background_size% = -1,
                                       Optional name$ = "n/a",
                                       Optional tax_id$ = "n/a",
                                       Optional desc$ = "n/a",
                                       Optional env As Environment = Nothing) As Object

        Dim clusterList As pipeline = pipeline.TryCreatePipeline(Of Cluster)(clusters, env)
        Dim clusterVec As Cluster()

        If clusterList.isError Then
            Return clusterList.GetType
        Else
            clusterVec = clusterList.populates(Of Cluster)(env).ToArray

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

    <Extension>
    Private Function compoundCluster(map As MapIndex) As Cluster
        Return New Cluster With {
            .description = map.description,
            .ID = If(map.id.IsPattern("\d+"), $"map{map.id}", map.id),
            .names = map.Name.Replace(" - Reference pathway", ""),
            .members = map.shapes _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .Where(Function(id) id.IsPattern("[CDG]\d+")) _
                .Select(Function(cid)
                            Return New BackgroundGene With {
                                .accessionID = cid,
                                .[alias] = {cid},
                                .locus_tag = New NamedValue With {.name = cid, .text = cid},
                                .name = cid,
                                .term_id = {cid}
                            }
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' create kegg maps background for the metabolism data analysis
    ''' </summary>
    ''' <param name="kegg"></param>
    ''' <returns></returns>
    <ExportAPI("metabolism.background")>
    Public Function metabolismBackground(kegg As MapRepository, Optional filter As String() = Nothing) As Background
        Dim mapIdFilter As Index(Of String) = filter _
            .SafeQuery _
            .Select(Function(id) id.Match("\d+")) _
            .Indexing
        Dim clusters As Cluster() = kegg.Maps _
            .Where(Function(map)
                       If mapIdFilter.Count > 0 Then
                           Return map.id.Match("\d+") Like mapIdFilter
                       Else
                           Return True
                       End If
                   End Function) _
            .Select(Function(map)
                        Return map.compoundCluster
                    End Function) _
            .ToArray

        Return New Background With {
            .clusters = clusters,
            .build = Now,
            .comments = "The KEGG compound metabolism background model",
            .id = "kegg maps",
            .name = "kegg maps"
        }
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
            .term_id = terms
        }
    End Function
End Module
