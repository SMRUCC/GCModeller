#Region "Microsoft.VisualBasic::18ef23a2e37e8cb0af6c26f28bf86921, R#\gseakit\Profiles.vb"

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

'   Total Lines: 205
'    Code Lines: 146 (71.22%)
' Comment Lines: 35 (17.07%)
'    - Xml Docs: 91.43%
' 
'   Blank Lines: 24 (11.71%)
'     File Size: 8.28 KB


' Module profiles
' 
'     Function: categoryLabels, CreateKEGGCategory, cutProfile, GOEnrichmentProfiles, KEGGEnrichmentProfiles
'               NoCatagoryProfile, sortProfile
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
Imports SMRUCC.genomics.Data.GeneOntology
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
        Return SMRUCC.genomics.Analysis.HTS.GSEA.IDCategoryFromBackground(background)
    End Function

    ''' <summary>
    ''' Create catalog profiles data for GO enrichment result its data visualization.
    ''' </summary>
    ''' <param name="enrichments">the kobas <see cref="EnrichmentTerm"/> or gcmodeller <see cref="EnrichmentResult"/>.</param>
    ''' <param name="goDb"></param>
    ''' <param name="top">display the top n enriched GO terms.</param>
    ''' <param name="sort">
    ''' sort of the namespace
    ''' </param>
    ''' <returns></returns>
    <ExportAPI("GO.enrichment.profile")>
    <RApiReturn(GetType(CatalogProfiles))>
    Public Function GOEnrichmentProfiles(<RRawVectorArgument>
                                         enrichments As Object,
                                         goDb As GO_OBO,
                                         Optional top% = 10,
                                         Optional pvalue_cut As Double = 1,
                                         Optional sort As Boolean = True,
                                         Optional env As Environment = Nothing) As Object

        Dim pull As pipeline = pipeline.TryCreatePipeline(Of EnrichmentTerm)(enrichments, env, suppress:=True)

        If pull.isError Then
            pull = pipeline.TryCreatePipeline(Of EnrichmentResult)(enrichments, env)

            If pull.isError Then
                Return pull.getError
            End If

            pull = pipeline.CreateFromPopulator(pull.populates(Of EnrichmentResult)(env).Converts.ToArray)
        End If

        Dim GO_terms As Dictionary(Of Term) = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles As CatalogProfiles = pull _
            .populates(Of EnrichmentTerm)(env) _
            .CreateEnrichmentProfiles(
                GO_terms:=GO_terms,
                usingCorrected:=False,
                top:=top,
                pvalue:=pvalue_cut,
                sort:=sort
        )

        Return profiles
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="obo"></param>
    ''' <param name="root">
    ''' the root term id
    ''' </param>
    ''' <param name="top"></param>
    ''' <param name="pvalue_cut"></param>
    ''' <param name="sort"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("dag_enrichment_profile")>
    Public Function dag_enrichment_profile(<RRawVectorArgument>
                                           enrichments As Object,
                                           obo As GO_OBO,
                                           root As String,
                                           Optional top% = 10,
                                           Optional pvalue_cut As Double = 1,
                                           Optional sort As Boolean = True,
                                           Optional env As Environment = Nothing)

        Dim graph As New DAG.Graph(DirectCast(obo, GO_OBO).AsEnumerable)
        Dim pull As pipeline = pipeline.TryCreatePipeline(Of EnrichmentTerm)(enrichments, env, suppress:=True)

        If pull.isError Then
            pull = pipeline.TryCreatePipeline(Of EnrichmentResult)(enrichments, env)

            If pull.isError Then
                Return pull.getError
            End If

            pull = pipeline.CreateFromPopulator(pull.populates(Of EnrichmentResult)(env).Converts.ToArray)
        End If

        Dim terms = pull.populates(Of EnrichmentTerm)(env).ToArray
        Dim category As New Dictionary(Of String, List(Of EnrichmentTerm))

        For Each term As EnrichmentTerm In terms
            Dim chain = graph.Family(term.ID, root).ToArray
            Dim category_label As String

            If chain.IsNullOrEmpty Then
                category_label = "Unknown"
            Else
                category_label = chain(0).Family(0)
            End If

            If Not category.ContainsKey(category_label) Then
                Call category.Add(category_label, New List(Of EnrichmentTerm))
            End If

            category(category_label).Add(term)
        Next

        Return New CatalogProfiles() With {
            .catalogs = category _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Dim list As New CatalogProfile

                                  For Each term As EnrichmentTerm In a.Value.Where(Function(t) t.Pvalue <= pvalue_cut)
                                      Call list.Add(term.Term, term.P)
                                  Next

                                  Return list.Take(top)
                              End Function)
        }
    End Function

    ''' <summary>
    ''' A method for cast the kegg enrichment result to the 
    ''' category profiles for run data visualization
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="top"></param>
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
    ''' 
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
