﻿#Region "Microsoft.VisualBasic::350f037accbe11a513cd27ed6e5c8071, gseakit\Profiles.vb"

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

    ' Module profiles
    ' 
    '     Function: GOEnrichmentProfiles, KEGGEnrichmentProfiles
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GO
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Data.GeneOntology.OBO

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
    Public Function GOEnrichmentProfiles(enrichments As EnrichmentTerm(), goDb As GO_OBO, Optional top% = 10) As CatalogProfiles
        Dim GO_terms = goDb.AsEnumerable.ToDictionary
        ' 在这里是不进行筛选的
        ' 筛选应该是发生在脚本之中
        Dim profiles = enrichments.CreateEnrichmentProfiles(GO_terms, False, top, 1)

        Return profiles
    End Function

    <ExportAPI("KEGG.enrichment.profile")>
    Public Function KEGGEnrichmentProfiles(enrichments As EnrichmentTerm(), Optional top% = 10) As CatalogProfiles
        Dim profiles As Dictionary(Of String, Double) = enrichments.ToDictionary(Function(a) a.ID, Function(a) -Math.Log10(a.Pvalue))
        Dim result As CatalogProfiles = profiles.DoKeggProfiles(displays:=top)

        Return result
    End Function
End Module

