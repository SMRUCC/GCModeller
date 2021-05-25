#Region "Microsoft.VisualBasic::550966e0de0660df6f6d8b6b4be4a2cf, gseakit\GSEABackground.vb"

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
'     Function: assembleBackground, ClusterIntersections, CreateCluster, CreateKOBackground, KOMaps
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
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports GSEATools = SMRUCC.genomics.Analysis.HTS.GSEA
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter
Imports any = Microsoft.VisualBasic.Scripting

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
                                       Optional id_map As list = Nothing,
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
            With DirectCast(id_map, list)
                geneId = .slots.Keys.First
                KO = any.ToString([single](.slots(geneId)))
            End With
        Else
            KO = Nothing
            geneId = Nothing
        End If

        ' [geneID -> KO] mapping
        Dim mapping As NamedValue(Of String)()
        Dim mapsResult = KOMaps(genes, geneId, KO, env)

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
