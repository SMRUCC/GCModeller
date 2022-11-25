#Region "Microsoft.VisualBasic::48a3d7e956aa95a38690f989801ad933, R#\gseakit\Profiles.vb"

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

'   Total Lines: 38
'    Code Lines: 24
' Comment Lines: 9
'   Blank Lines: 5
'     File Size: 1.65 KB


' Module profiles
' 
'     Function: GOEnrichmentProfiles, KEGGEnrichmentProfiles
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

''' <summary>
''' enrichment term statics helper
''' </summary>
<Package("profiles")>
Public Module profiles

    ''' <summary>
    ''' create kegg category class model from a gsea background model
    ''' </summary>
    ''' <param name="background"></param>
    ''' <returns></returns>
    <ExportAPI("kegg_category")>
    Public Function CreateKEGGCategory(background As Background) As ClassProfiles
        Return KEGG.IDCategoryFromBackground(background)
    End Function

    ''' <summary>
    ''' Create catalog profiles data for GO enrichment result its data visualization.
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="goDb"></param>
    ''' <param name="top">display the top n enriched GO terms.</param>
    ''' <returns></returns>
    <ExportAPI("GO.enrichment.profile")>
    Public Function GOEnrichmentProfiles(enrichments As EnrichmentTerm(), goDb As GO_OBO, Optional top% = 10) As CatalogProfiles
        Dim GO_terms = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles = enrichments.CreateEnrichmentProfiles(GO_terms, False, top, 1)

        Return profiles
    End Function

    ''' <summary>
    ''' A method for cast the kegg enrichment result to the 
    ''' category profiles for run data visualization
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="top%"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("KEGG.enrichment.profile")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function KEGGEnrichmentProfiles(<RRawVectorArgument>
                                           enrichments As Object,
                                           Optional top% = 10,
                                           Optional env As Environment = Nothing) As Object
        Dim terms As EnrichmentTerm()
        Dim enrich As pipeline = pipeline.TryCreatePipeline(Of EnrichmentTerm)(enrichments, env, suppress:=True)

        If enrich.isError Then
            enrich = pipeline.TryCreatePipeline(Of EnrichmentResult)(enrichments, env)

            If enrich.isError Then
                Return enrich.getError
            Else
                terms = enrich _
                    .populates(Of EnrichmentResult)(env) _
                    .Converts(database:="KEGG Pathway") _
                    .ToArray
            End If
        Else
            terms = enrich.populates(Of EnrichmentTerm)(env).ToArray
        End If

        Dim profiles As Dictionary(Of String, Double) = terms.ToDictionary(Function(a) a.ID, Function(a) -Math.Log10(a.Pvalue))
        Dim result As CatalogProfiles = profiles.DoKeggProfiles(displays:=top)

        Return result
    End Function

    <ExportAPI("sort_profiles")>
    Public Function sortProfile(profile As CatalogProfiles, Optional top As Integer = 10) As CatalogProfiles
        Dim profiles = profile.catalogs _
            .ToDictionary(Function(c) c.Key,
                          Function(c)
                              Dim subset As CatalogProfile = c.Value
                              Dim sort = subset.Take(top)

                              Return sort
                          End Function)

        Return New CatalogProfiles With {
            .catalogs = profiles
        }
    End Function

    <ExportAPI("cut_profiles")>
    Public Function cutProfile(profile As CatalogProfiles, valueCut As Double) As CatalogProfiles
        Dim profiles = profile.catalogs _
            .ToDictionary(Function(c) c.Key,
                          Function(c)
                              Dim subset As CatalogProfile = c.Value
                              Dim cut = subset.profile _
                                 .Where(Function(k) k.Value >= valueCut) _
                                 .ToDictionary

                              Return New CatalogProfile With {
                                  .profile = cut,
                                  .information = subset.information
                              }
                          End Function)

        Return New CatalogProfiles With {
            .catalogs = profiles
        }
    End Function

    <ExportAPI("no_catagory_profile")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function NoCatagoryProfile(enrichments As Array, name As String,
                                      Optional top% = 30,
                                      Optional env As Environment = Nothing) As Object

        Dim profiles As NamedValue(Of Double)()

        enrichments = REnv.TryCastGenericArray(enrichments, env)

        If TypeOf enrichments Is EnrichmentTerm() Then
            profiles = DirectCast(enrichments, EnrichmentTerm()) _
                .OrderBy(Function(a) a.Pvalue) _
                .Take(top) _
                .Select(Function(a)
                            Return New NamedValue(Of Double)(a.ID, -Math.Log10(a.Pvalue), a.Term)
                        End Function) _
                .ToArray
        ElseIf TypeOf enrichments Is EnrichmentResult() Then
            profiles = DirectCast(enrichments, EnrichmentResult()) _
                .OrderBy(Function(a) a.pvalue) _
                .Take(top) _
                .Select(Function(a)
                            Return New NamedValue(Of Double)(a.term, -Math.Log10(a.pvalue), a.name)
                        End Function) _
                .ToArray
        Else
            Return Message.InCompatibleType(GetType(EnrichmentTerm), enrichments.GetType, env)
        End If

        Return New CatalogProfiles With {
            .catalogs = New Dictionary(Of String, CatalogProfile) From {
                {name, New CatalogProfile(data:=profiles)}
            }
        }
    End Function

    ''' <summary>
    ''' get category labels for a given id set
    ''' </summary>
    ''' <param name="category"></param>
    ''' <param name="idSet">
    ''' a character vector of the target id set
    ''' </param>
    ''' <param name="level">level1 or level2</param>
    ''' <returns></returns>
    <ExportAPI("category_labels")>
    Public Function categoryLabels(category As ClassProfiles,
                                   idSet As String(),
                                   Optional level As Integer = 1,
                                   Optional env As Environment = Nothing) As Object
        If level = 1 Then
            Return idSet _
                .SafeQuery _
                .Select(Function(id)
                            Return category.FindClass(id)
                        End Function) _
                .ToArray
        Else
            Dim subclass = category.Catalogs.Values.ToArray

            Return idSet _
                .SafeQuery _
                .Select(Function(id)
                            For Each [sub] In subclass
                                Dim findClass As String = [sub].FindCategory(id)

                                If Not findClass.StringEmpty Then
                                    Return findClass
                                End If
                            Next

                            Return ""
                        End Function) _
                .ToArray
        End If
    End Function
End Module
