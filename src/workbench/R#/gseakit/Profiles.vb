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
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports REnv = SMRUCC.Rsharp.Runtime

<Package("profiles")>
Module profiles

    ''' <summary>
    ''' Create catalog profiles data for GO enrichment result its data visualization.
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="goDb"></param>
    ''' <param name="top">display the top n enriched GO terms.</param>
    ''' <returns></returns>
    <ExportAPI("GO.enrichment.profile")>
    Public Function GOEnrichmentProfiles(enrichments As EnrichmentTerm(),
                                         goDb As GO_OBO,
                                         Optional top% = 10) As CatalogProfiles

        Dim GO_terms As Dictionary(Of Term) = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles = enrichments.CreateEnrichmentProfiles(GO_terms, False, top, 1)

        Return profiles
    End Function

    ''' <summary>
    ''' create the kegg pathway catagory profiles from a given
    ''' enrichment analysis result
    ''' </summary>
    ''' <param name="enrichments"></param>
    ''' <param name="top">
    ''' get top n terms in each category when create the category profiles
    ''' </param>
    ''' <returns>
    ''' A category profiles object that can be apply to do data
    ''' visualization plot of the given enrichment analysis
    ''' result terms.
    ''' </returns>
    <ExportAPI("KEGG.enrichment.profile")>
    Public Function KEGGEnrichmentProfiles(enrichments As EnrichmentTerm(), Optional top% = 10) As CatalogProfiles
        Dim profiles As Dictionary(Of String, Double) = enrichments _
            .ToDictionary(Function(a) a.ID,
                          Function(a)
                              Return -Math.Log10(a.Pvalue)
                          End Function)
        Dim result As CatalogProfiles = profiles.DoKeggProfiles(displays:=top)

        Return result
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
End Module
