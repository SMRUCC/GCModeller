#Region "Microsoft.VisualBasic::0ad9d39d6ee335db30fd9d76963d0982, ..\R.Bioconductor\RDotNet.Extensions.Bioinformatics\Declares\clusterProfiler\clusterProfiler.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

Namespace clusterProfiler

    Public Module clusterProfiler

        <Extension> Public Function LoadGoBriefTable(goBriefTable As IO.File) As String
            Dim goBrief$ = goBriefTable.PushAsDataFrame
            Dim go2name$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{go2name} <- {goBrief}[, c(""goID"", ""name"")]"
                End With
            End SyncLock

            Return go2name
        End Function

        Public Function enricher(gene As IEnumerable(Of String), universe As IEnumerable(Of String), TERM2GENE As IEnumerable(Of NamedValue(Of String)),
                                 Optional pvalueCutoff# = 0.05,
                                 Optional pAdjustMethod$ = "BH",
                                 Optional minGSSize% = 5,
                                 Optional qvalueCutoff# = 0.2,
                                 Optional TERM2NAME As IEnumerable(Of NamedValue(Of String)) = Nothing) As enrichResult

            Dim t2g = TERM2GENE.dataframe(MappingsHelper.NamedValueMapsWrite("goID", "geneID"))
            Dim genes = base.c(gene.Select(AddressOf Rstring))
            Dim background = base.c(universe.Select(AddressOf Rstring))

            Return enricherS4(gene:=genes,
                              universe:=background,
                              TERM2GENE:=t2g,
                              minGSSize:=minGSSize,
                              pAdjustMethod:=pAdjustMethod,
                              pvalueCutoff:=pvalueCutoff,
                              qvalueCutoff:=qvalueCutoff)
        End Function

        Public Function enricherS4(gene$, universe$, TERM2GENE$,
                                    Optional pvalueCutoff# = 0.05,
                                    Optional pAdjustMethod$ = "BH",
                                    Optional minGSSize% = 5,
                                    Optional qvalueCutoff# = 0.2,
                                    Optional TERM2NAME$ = "NA") As enrichResult
            Dim var$ = enricher(gene, universe, TERM2GENE, pvalueCutoff, pAdjustMethod, minGSSize, qvalueCutoff, TERM2NAME)

            SyncLock R
                With R
                    Dim pointer = .Evaluate(var)


                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' A universal enrichment analyzer
        ''' </summary>
        ''' <param name="gene$">a vector of gene id</param>
        ''' <param name="universe$">background genes</param>
        ''' <param name="TERM2GENE$">user input annotation of TERM TO GENE mapping, a data.frame of 2 column with term and gene</param>
        ''' <param name="pvalueCutoff#">pvalue cutoff</param>
        ''' <param name="pAdjustMethod$">one of "holm", "hochberg", "hommel", "bonferroni", "BH", "BY", "fdr", "none"</param>
        ''' <param name="minGSSize%">minimal size of genes annotated for testing</param>
        ''' <param name="qvalueCutoff#">qvalue cutoff</param>
        ''' <param name="TERM2NAME$">user input of TERM TO NAME mapping, a data.frame of 2 column with term and name</param>
        ''' <returns>A <see cref="enrichResult"/> instance</returns>
        Public Function enricher(gene$, universe$, TERM2GENE$,
                                 Optional pvalueCutoff# = 0.05,
                                 Optional pAdjustMethod$ = "BH",
                                 Optional minGSSize% = 5,
                                 Optional qvalueCutoff# = 0.2,
                                 Optional TERM2NAME$ = "NA") As String
            Dim var$ = App.NextTempName

            SyncLock R
                With R
                    .call = $"{var} <- enricher({gene},
pvalueCutoff = {pvalueCutoff}, pAdjustMethod = {Rstring(pAdjustMethod)},
universe = {universe}, minGSSize = {minGSSize}, qvalueCutoff = {qvalueCutoff}, 
TERM2GENE = {TERM2GENE}, TERM2NAME = {TERM2NAME})"
                End With
            End SyncLock

            Return var
        End Function
    End Module

    Public Class Term2Gene

        Public Property GO_ID As String
        Public Property Locus_tag As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class enrichResult

    End Class
End Namespace
