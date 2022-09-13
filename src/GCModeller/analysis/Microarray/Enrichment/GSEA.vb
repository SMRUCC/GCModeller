#Region "Microsoft.VisualBasic::efb02345c4bbf1a0a6b4627473e967f9, GCModeller\analysis\Microarray\Enrichment\GSEA.vb"

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

    '   Total Lines: 66
    '    Code Lines: 47
    ' Comment Lines: 14
    '   Blank Lines: 5
    '     File Size: 2.81 KB


    ' Module GSEA
    ' 
    '     Function: (+2 Overloads) Convert, (+2 Overloads) Converts
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.Microarray
Imports SMRUCC.genomics.Analysis.Microarray.DAVID
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

    ''' <summary>
    ''' Converts the GCModeller enrichment analysis output as the KOBAS enrichment analysis result output table.
    ''' </summary>
    ''' <param name="terms"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Converts(terms As IEnumerable(Of EnrichmentResult), Optional database$ = "n/a") As IEnumerable(Of EnrichmentTerm)
        Return terms.Select(Function(term) term.Convert(database))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function Converts(terms As IEnumerable(Of FunctionCluster), Optional database$ = "n/a") As IEnumerable(Of EnrichmentTerm)
        Return terms.Select(Function(term) term.Convert(database))
    End Function

    <Extension>
    Private Function Convert(term As FunctionCluster, database$) As EnrichmentTerm
        Return New EnrichmentTerm With {
            .Backgrounds = term.PopHits,
            .CorrectedPvalue = term.Benjamini,
            .Database = database,
            .ID = term.Term.Split("~"c).First,
            .Term = term.Term.GetTagValue("~").Value,
            .Input = term.ORFs.JoinBy("; "),
            .link = term.Link,
            .number = term.Count,
            .ORF = term.ORFs,
            .Pvalue = term.PValue
        }
    End Function

    <Extension>
    Private Function Convert(term As EnrichmentResult, database$) As EnrichmentTerm
        Return New EnrichmentTerm With {
            .Backgrounds = term.cluster,
            .number = term.enriched.Split("/"c).First,
            .ID = term.term,
            .ORF = term.geneIDs,
            .Pvalue = term.pvalue,
            .Term = term.name.Replace("Reference pathway", "").Trim(" "c, "-"c),
            .CorrectedPvalue = term.FDR,
            .Database = database,
            .Input = .ORF.JoinBy(", ")
        }
    End Function
End Module
