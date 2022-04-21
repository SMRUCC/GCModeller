#Region "Microsoft.VisualBasic::4ef9cdd0bb54ecaac9a311982ce558a6, R#\gseakit\GSEABackground.vb"

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

' Module GSEABackground
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: assembleBackground, ClusterIntersections, CreateCluster, createGene, CreateKOBackground
'               KOTable, PrintBackground, ReadBackground, WriteBackground
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
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

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <ExportAPI("geneSet.intersects")>
    Public Function ClusterIntersections(cluster As Cluster, geneSet$(), Optional isLocusTag As Boolean = False) As String()
        Return cluster.Intersect(geneSet, isLocusTag).ToArray
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
    ''' <param name="data"></param>
    ''' <param name="clusterId$"></param>
    ''' <param name="clusterName$"></param>
    ''' <returns></returns>
    <ExportAPI("gsea.cluster")>
    Public Function CreateCluster(data As Rdataframe, clusterId$, clusterName$, Optional desc$ = "n/a", Optional id$ = "xref", Optional name$ = "name") As Cluster
        Dim idvec As String() = asVector(Of String)(data.columns(id))
        Dim namevec As String() = asVector(Of String)(data.columns(name))
        Dim cluster As New Cluster With {
            .ID = clusterId,
            .description = desc,
            .names = clusterName,
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
    Public Function assembleBackground(clusters As Object,
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
                background_size = Aggregate cluster In clusterVec Into Sum(cluster.size)
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

    <Extension>
    Private Function KO_category(category As BriteHText) As IEnumerable(Of Cluster)
        Return category.categoryItems _
            .SafeQuery _
            .Select(Function(subtype)
                        Return subtype.categoryItems _
                            .SafeQuery _
                            .Select(Function(pathway)
                                        Return New Cluster With {
                                            .ID = "map" & pathway.entryID,
                                            .description = pathway _
                                                .ToString _
                                                .Replace("[BR:ko]", "") _
                                                .Replace("[PATH:ko]", "") _
                                                .Trim,
                                            .names = pathway.description _
                                                .Replace("[BR:ko]", "") _
                                                .Replace("[PATH:ko]", "") _
                                                .Trim,
                                            .members = pathway.categoryItems _
                                                .SafeQuery _
                                                .Select(Function(ko)
                                                            Return New BackgroundGene With {
                                                                .accessionID = ko.entryID,
                                                                .[alias] = {ko.entryID},
                                                                .locus_tag = New NamedValue With {.name = ko.entryID, .text = ko.description},
                                                                .name = ko.description,
                                                                .term_id = {ko.entryID}
                                                            }
                                                        End Function) _
                                                .ToArray
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL
    End Function

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
            .ID = map.id,
            .names = map.Name,
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

    <ExportAPI("metabolism.background")>
    Public Function metabolismBackground(kegg As MapRepository) As Background
        Dim clusters As Cluster() = kegg.Maps _
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
