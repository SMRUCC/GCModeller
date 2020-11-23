#Region "Microsoft.VisualBasic::2a6af726c0ae80c6e7c7b6e4ae43fb7d, gseakit\GSEABackground.vb"

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
'     Function: ClusterIntersections, CreateKOBackground, KOTable, PrintBackground, ReadBackground
'               WriteBackground
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Annotation.Ptf
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports GSEATools = SMRUCC.genomics.Analysis.HTS.GSEA
Imports Rdataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime.Internal.ConsolePrinter

<Package("gseakit.background", Category:=APICategories.ResearchTools)>
Public Module GSEABackground

    Sub New()
        Call REnv.AttachConsoleFormatter(Of Background)(AddressOf PrintBackground)
    End Sub

    Private Function PrintBackground(x As Object) As String
        Return DirectCast(x, Background).name
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

    Public Function KOMaps(genes As Object, geneId$, KO$, env As Environment) As Object
        If TypeOf genes Is list Then
            If KO.StringEmpty OrElse geneId.StringEmpty Then
                Return DirectCast(genes, list).slots _
                    .Select(Function(t)
                                Return New NamedValue(Of String) With {
                                    .Name = t.Key,
                                    .Value = Scripting.ToString([single](t.Value))
                                }
                            End Function) _
                    .ToArray
            Else
                Return DirectCast(genes, list).slots.Values _
                    .Select(Function(map)
                                Dim id As String = DirectCast(map, list).getValue(Of String)(geneId, env)
                                Dim koId As String = DirectCast(map, list).getValue(Of String)(KO, env)

                                Return New NamedValue(Of String)(id, koId)
                            End Function) _
                    .ToArray
            End If
        ElseIf TypeOf genes Is Rdataframe Then
            Dim idVec As String() = DirectCast(genes, Rdataframe).columns(geneId)
            Dim koVec As String() = DirectCast(genes, Rdataframe).columns(KO)

            Return idVec _
                .Select(Function(id, i)
                            Return New NamedValue(Of String) With {
                                .Name = id,
                                .Value = koVec(i)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is EntityObject() Then
            Return DirectCast(genes, EntityObject()) _
                .Select(Function(row)
                            Return New NamedValue(Of String) With {
                                .Name = row(geneId),
                                .Value = row(KO)
                            }
                        End Function) _
                .ToArray
        ElseIf TypeOf genes Is PtfFile OrElse TypeOf genes Is ProteinAnnotation() Then
            Dim prot As ProteinAnnotation()

            If TypeOf genes Is PtfFile Then
                prot = DirectCast(genes, PtfFile).proteins
            Else
                prot = DirectCast(genes, ProteinAnnotation())
            End If

            Return prot.Where(Function(p) p.has("ko")) _
                .Select(Function(protein)
                            Return protein.attributes("ko") _
                                .Select(Function(koid)
                                            Return New NamedValue(Of String) With {
                                                .Name = protein.geneId,
                                                .Value = koid
                                            }
                                        End Function)
                        End Function) _
                .IteratesALL _
                .ToArray
        Else
            Return Internal.debug.stop(New InvalidProgramException(genes.GetType.FullName), env)
        End If
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
    Public Function CreateKOBackground(<RRawVectorArgument>
                                       genes As Object,
                                       maps As MapRepository,
                                       Optional size% = -1,
                                       Optional genomeName$ = "unknown",
                                       Optional id_map As list = Nothing,
                                       Optional env As Environment = Nothing) As Object
        Dim geneId, KO As String
        Dim kegg As GetClusterTerms = GSEATools.KEGGClusters(maps.AsEnumerable)

        If Not id_map Is Nothing Then
            With DirectCast(id_map, list)
                geneId = .slots.Keys.First
                KO = Scripting.ToString([single](.slots(geneId)))
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
            createGene:=Function(gene, terms)
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
                        End Function,
            getTerms:=Function(gene)
                          Return {gene.Value}
                      End Function,
            define:=kegg,
            genomeName:=genomeName
        )
        model.size = size

        Return model
    End Function
End Module
