#Region "Microsoft.VisualBasic::3645e9393ece5c9e64325b1ecbc9c030, R#\kegg_kit\profiles.vb"

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

'   Total Lines: 235
'    Code Lines: 187 (79.57%)
' Comment Lines: 19 (8.09%)
'    - Xml Docs: 89.47%
' 
'   Blank Lines: 29 (12.34%)
'     File Size: 10.25 KB


' Module profiles
' 
'     Function: CompoundPathwayIndex, CompoundPathwayProfiles, FluxMapProfiles, GetProfileMapping, KEGGCategoryProfiles
'               KOpathwayProfiles, MapCategory
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.Utility
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.XML
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

''' <summary>
''' annotation profiles tools
''' </summary>
<Package("profiles")>
Module profiles

    <ExportAPI("compounds.pathway.index")>
    <RApiReturn(GetType(Dictionary(Of String, Index(Of String))))>
    Public Function CompoundPathwayIndex(<RRawVectorArgument>
                                         pathways As Object,
                                         Optional env As Environment = Nothing) As Object

        Dim pathwayMaps As pipeline = pipeline.TryCreatePipeline(Of PathwayMap)(pathways, env, suppress:=True)

        If Not pathwayMaps.isError Then
            ' probably has duplicated pathway item
            ' removes the duplicated data at first
            Return pathwayMaps _
                .populates(Of PathwayMap)(env) _
                .GroupBy(Function(p) p.briteID) _
                .ToDictionary(Function(p)
                                  Return p.Key
                              End Function,
                              Function(p)
                                  Return p.First.KEGGCompound _
                                      .SafeQuery _
                                      .Keys _
                                      .Indexing
                              End Function)
        Else
            pathwayMaps = pipeline.TryCreatePipeline(Of Map)(pathways, env)
        End If

        If Not pathwayMaps.isError Then
            Return pathwayMaps _
                .populates(Of Map)(env) _
                .GroupBy(Function(m) m.EntryId) _
                .ToDictionary(Function(m) m.Key,
                              Function(m)
                                  Return m.First.shapes.mapdata _
                                     .Select(Function(a) a.IDVector) _
                                     .IteratesALL _
                                     .Distinct _
                                     .Where(Function(id) id.IsPattern("C\d+")) _
                                     .Indexing
                              End Function)
        End If

        Return pathwayMaps.getError
    End Function

    ''' <summary>
    ''' Do statistics of the KEGG pathway profiles based on the given kegg id
    ''' </summary>
    ''' <param name="pathways">The pathway compound reference index data</param>
    ''' <param name="compounds">The kegg compound id</param>
    ''' <returns></returns>
    <ExportAPI("compounds.pathway.profiles")>
    Public Function CompoundPathwayProfiles(pathways As Dictionary(Of String, Index(Of String)), compounds As String()) As Dictionary(Of String, Integer)
        Return pathways _
            .ToDictionary(Function(p) p.Key,
                          Function(p)
                              Return p.Value _
                                  .Intersect(collection:=compounds) _
                                  .Distinct _
                                  .Count
                          End Function)
    End Function

    <ExportAPI("getProfileMapping")>
    Public Function GetProfileMapping(map As Map(), mapping As list, Optional env As Environment = Nothing) As Object
        Dim err As Message = Nothing
        Dim metainfo As Dictionary(Of String, String) = mapping.AsGeneric(Of String)(env, err:=err)

        If Not err Is Nothing Then
            Return err
        End If

        Return map.GetProfileMapping(metainfo)
    End Function

    <ExportAPI("flux.map.profiles")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function FluxMapProfiles(flux As Object, maps As MapRepository, Optional env As Environment = Nothing) As Object
        If TypeOf flux Is RDataframe Then
            Dim activity As Double() = CLRVector.asNumeric(DirectCast(flux, RDataframe).getColumnVector("activity"))
            Dim rId As String()
            Dim flags = activity.Select(Function(a) a > 0).ToArray
            Dim data As RDataframe = DirectCast(flux, RDataframe).sliceByRow(flags, env)
            Dim brite As Dictionary(Of String, BriteHEntry.Pathway()) = BriteHEntry.Pathway _
                .LoadDictionary _
                .GroupBy(Function(p) p.Value.class) _
                .ToDictionary(Function(p) p.Key,
                              Function(p)
                                  Return p.Values
                              End Function)

            rId = CLRVector.asCharacter(data.getColumnVector("RID"))
            activity = CLRVector.asNumeric(data.getColumnVector("activity"))

            Dim fluxData = rId.SeqIterator.ToDictionary(Function(r) r.value, Function(i) activity(i))
            Dim profiles As New CatalogProfiles

            For Each catalog In brite
                Dim catProfiles As New CatalogProfile

                For Each mapId As BriteHEntry.Pathway In catalog.Value
                    Dim map = maps.GetByKey(mapId.EntryId)
                    Dim total As Double = Aggregate id As String In map.GetMembers Where fluxData.ContainsKey(id) Into Sum(fluxData(id))

                    If total > 0 Then
                        catProfiles.Add(map.name.Replace(" - Reference pathway", "").Trim, total)
                    End If
                Next

                If Not catProfiles.is_empty Then
                    profiles.catalogs(catalog.Key) = catProfiles
                End If
            Next

            Return profiles
        Else
            Return RInternal.debug.stop(New NotImplementedException, env)
        End If
    End Function

    ''' <summary>
    ''' create KEGG map prfiles via a given KO id list.
    ''' </summary>
    ''' <param name="KO">a character vector of KO id list.</param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("KO.map.profiles")>
    <RApiReturn(GetType(Dictionary(Of String, Double)))>
    Public Function KOpathwayProfiles(<RRawVectorArgument> KO As Object, Optional env As Environment = Nothing) As Object
        If KO Is Nothing Then
            Return Nothing
        ElseIf TypeOf KO Is String() Then
            KO = DirectCast(KO, String()) _
                .Select(Function(id)
                            Return New NamedValue(Of String)(id, id)
                        End Function) _
                .ToArray
        End If

        If Not TypeOf KO Is NamedValue(Of String)() Then
            Return RInternal.debug.stop({
                $"invalid data type for KO mapping statices",
                $"data type: {KO.GetType.FullName}"
            }, env)
        End If

        Dim profiles = DirectCast(KO, NamedValue(Of String)()).LevelAKOStatics.AsDouble
        Return profiles
    End Function

    ''' <summary>
    ''' create kegg catalog profiles data table
    ''' </summary>
    ''' <param name="profiles"></param>
    ''' <returns></returns>
    <ExportAPI("kegg.category_profiles")>
    Public Function KEGGCategoryProfiles(profiles As Dictionary(Of String, Integer)) As EntityObject()
        Return profiles _
            .AsNumeric _
            .KEGGCategoryProfiles _
            .Select(Function(category)
                        Return category.Value _
                            .Select(Function(term)
                                        Return New EntityObject With {
                                            .ID = term.Name,
                                            .Properties = New Dictionary(Of String, String) From {
                                                {"class", category.Key},
                                                {"count", term.Value}
                                            }
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    <ExportAPI("map_category")>
    Public Function MapCategory() As list
        Dim htext As htext = htext.br08901
        Dim classList As New list With {
            .slots = New Dictionary(Of String, Object)
        }

        For Each category As BriteHText In htext.Hierarchical.categoryItems
            Dim subcat As New list With {
                .slots = New Dictionary(Of String, Object)
            }

            For Each subcategory As BriteHText In category.categoryItems
                Dim index As New RDataframe With {
                    .columns = New Dictionary(Of String, Array)
                }
                Dim mapid As String() = subcategory.categoryItems.Select(Function(d) "map" & d.entryID).ToArray
                Dim desc As String() = subcategory.categoryItems.Select(Function(d) d.classLabel).ToArray
                Dim name As String() = subcategory.categoryItems.Select(Function(d) d.description).ToArray

                Call index.add("mapid", mapid)
                Call index.add("name", name)
                Call index.add("description", desc)

                subcat.add(subcategory.classLabel, index)
            Next

            classList.add(category.classLabel, subcat)
        Next

        Return classList
    End Function

    ''' <summary>
    ''' create gsea background based on the reference kegg map data
    ''' </summary>
    ''' <param name="kegg">a collection of the reference kegg maps</param>
    ''' <param name="ko">
    ''' a id mapping from kegg ko to gene id
    ''' </param>
    ''' <param name="tcode">the kegg organism code</param>
    ''' <param name="multiple_omics">
    ''' the compound id will be keeps if multiple omics flag is TRUE.
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("assemble_background")>
    Public Function AssembleBackground(kegg As Map(), ko As list,
                                       Optional multiple_omics As Boolean = False,
                                       Optional tcode As String = Nothing,
                                       <RRawVectorArgument>
                                       Optional map_set As Object = Nothing,
                                       Optional env As Environment = Nothing) As Background

        Dim koId As Dictionary(Of String, String()) = ko.AsGeneric(Of String())(env)
        Dim map_index As Index(Of String) = CLRVector.asCharacter(map_set).Indexing
        Dim clusters As Cluster() = kegg.SafeQuery _
            .Select(Function(map)
                        Return map.MapToCluster(tcode, map_index, koId, multiple_omics)
                    End Function) _
            .Where(Function(c) Not c Is Nothing) _
            .ToArray

        Return New Background With {
            .build = Now,
            .clusters = clusters,
            .comments = tcode,
            .name = tcode,
            .id = tcode,
            .size = clusters.BackgroundSize
        }
    End Function

    <Extension>
    Private Function MapToCluster(map As Map, tcode As String, map_index As Index(Of String), koId As Dictionary(Of String, String()), multiple_omics As Boolean) As Cluster
        Dim shapes As MapData = map.shapes
        Dim mapId = If(tcode.StringEmpty(), map.EntryId, map.EntryId.Replace("map", tcode))

        If map_index.Count > 0 Then
            If Not (mapId Like map_index) Then
                Return Nothing
            End If
        End If
        If shapes Is Nothing OrElse shapes.mapdata.IsNullOrEmpty Then
            Return Nothing
        End If

        Dim idset = shapes.mapdata _
            .Select(Function(a) a.IDVector) _
            .IteratesALL _
            .Distinct _
            .Where(Function(kid) koId.ContainsKey(kid)) _
            .Select(Function(kid)
                        Dim geneId = koId(kid)
                        Dim genes = geneId _
                            .Select(Function(id)
                                        Return New BackgroundGene With {
                                            .accessionID = id,
                                            .locus_tag = New NamedValue With {.name = id, .text = kid},
                                            .name = kid,
                                            .term_id = {New NamedValue With {.name = kid, .text = id}}
                                        }
                                    End Function) _
                            .ToArray

                        Return genes
                    End Function) _
            .IteratesALL _
            .ToArray

        If idset.IsNullOrEmpty Then
            Return Nothing
        ElseIf multiple_omics Then
            Dim compounds = shapes.mapdata _
                .Select(Function(a) a.IDVector) _
                .IteratesALL _
                .Distinct _
                .Where(Function(cid) cid.IsPattern("C\d+")) _
                .Select(Function(cid)
                            Return New BackgroundGene With {
                                .accessionID = cid,
                                .locus_tag = New NamedValue With {.name = cid, .text = cid},
                                .name = cid
                            }
                        End Function) _
                .ToArray

            idset = idset.JoinIterates(compounds).ToArray
        End If

        Return New Cluster With {
            .description = map.description,
            .ID = mapId,
            .members = idset,
            .names = Strings.Replace(map.name, "Reference pathway", "").Trim(" "c, "-"c)
        }
    End Function
End Module
