﻿#Region "Microsoft.VisualBasic::49a44ac038080e263c5c741ddcd6af18, analysis\Microarray\Enrichment\GSEA.vb"

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

' Module GSEA
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray.KOBAS

''' <summary>
''' Gene set enrichment analysis (GSEA) (also functional enrichment analysis) is a method 
''' to identify classes of genes or proteins that are over-represented in a large set of 
''' genes or proteins, and may have an association with disease phenotypes. The method 
''' uses statistical approaches to identify significantly enriched or depleted groups of 
''' genes. Microarray and proteomics results often identify thousands of genes which are 
''' used for the analysis.
''' </summary>
Public Module GSEA

    <Extension>
    Public Function Converts(terms As IEnumerable(Of EnrichmentResult)) As IEnumerable(Of EnrichmentTerm)
        Return terms.Select(AddressOf Convert)
    End Function

    Private Function Convert(term As EnrichmentResult) As EnrichmentTerm
        Return New EnrichmentTerm With {
            .Backgrounds = term.enriched.Split("/"c).Last,
            .number = term.enriched.Split("/"c).First,
            .ID = term.term,
            .ORF = term.geneIDs,
            .Pvalue = term.pvalue,
            .Term = term.term,
            .CorrectedPvalue = term.FDR
        }
    End Function
End Module
