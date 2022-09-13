#Region "Microsoft.VisualBasic::d90cb48b432272d4543b259c8cb30c13, R#\kegg_kit\profiles.vb"

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

    '   Total Lines: 232
    '    Code Lines: 184
    ' Comment Lines: 19
    '   Blank Lines: 29
    '     File Size: 10.08 KB


    ' Module profiles
    ' 
    '     Function: CompoundPathwayIndex, CompoundPathwayProfiles, FluxMapProfiles, GetProfileMapping, KEGGCategoryProfiles
    '               KOpathwayProfiles, MapCategory
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports RDataframe = SMRUCC.Rsharp.Runtime.Internal.Object.dataframe
Imports REnv = SMRUCC.Rsharp.Runtime

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
                .GroupBy(Function(m) m.id) _
                .ToDictionary(Function(m) m.Key,
                              Function(m)
                                  Return m.First.shapes _
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
    Public Function FluxMapProfiles(flux As Object, maps As MapRepository, Optional env As Environment = Nothing) As Object
        If TypeOf flux Is RDataframe Then
            Dim activity As Double() = REnv.asVector(Of Double)(DirectCast(flux, RDataframe).getColumnVector("activity"))
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

            rId = REnv.asVector(Of String)(data.getColumnVector("RID"))
            activity = REnv.asVector(Of Double)(data.getColumnVector("activity"))

            Dim fluxData = rId.SeqIterator.ToDictionary(Function(r) r.value, Function(i) activity(i))
            Dim profiles As New CatalogProfiles

            For Each catalog In brite
                Dim catProfiles As New CatalogProfile

                For Each mapId As BriteHEntry.Pathway In catalog.Value
                    Dim map = maps.GetByKey(mapId.EntryId)
                    Dim total As Double = Aggregate id As String In map.GetMembers Where fluxData.ContainsKey(id) Into Sum(fluxData(id))

                    If total > 0 Then
                        catProfiles.Add(map.Name.Replace(" - Reference pathway", "").Trim, total)
                    End If
                Next

                If Not catProfiles.isEmpty Then
                    profiles.catalogs(catalog.Key) = catProfiles
                End If
            Next

            Return profiles
        Else
            Return Internal.debug.stop(New NotImplementedException, env)
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
            Return Internal.debug.stop({
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
End Module
