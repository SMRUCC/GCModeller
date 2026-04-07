#Region "Microsoft.VisualBasic::3cc5a29c07ca71a950dc91900660337c, annotations\KEGG\PathForceBuilder.vb"

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

    '   Total Lines: 84
    '    Code Lines: 64 (76.19%)
    ' Comment Lines: 7 (8.33%)
    '    - Xml Docs: 85.71%
    ' 
    '   Blank Lines: 13 (15.48%)
    '     File Size: 3.21 KB


    ' Module PathForceBuilder
    ' 
    '     Function: corVec, CreateForce
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.ObjectModel
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA

Public Module PathForceBuilder

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expr1"></param>
    ''' <param name="expr2"></param>
    ''' <param name="background">enrichment background for the molecules in <paramref name="expr2"/> matrix</param>
    ''' <returns></returns>
    <Extension>
    Public Function CreateForce(expr1 As Matrix, expr2 As Matrix, background As Background) As Matrix
        Dim cor As New List(Of Double())
        Dim gene_ids As New List(Of String)

        For Each gene As DataFrameRow In TqdmWrapper.Wrap(expr1.expression)
            Call cor.Add(gene.corVec(expr2, background))
            Call gene_ids.Add(gene.geneID)
        Next

        Dim ds As New StatisticsObject(cor, Nothing) With {
            .decoder = gene_ids.Indexing,
            .YLabels2 = New ObservableCollection(Of String)
        }

        For Each id As String In gene_ids
            Call ds.YLabels2.Add(id)
        Next

        Call Enumerable.Range(0, cor(0).Length).DoEach(AddressOf ds.XIndexes.Add)
        Call Enumerable.Range(0, cor.Count).DoEach(AddressOf ds.YIndexes.Add)
        Call background.clusters.Keys.DoEach(AddressOf ds.XLabels.Add)

        Dim pca As MultivariateAnalysisResult = ds.PrincipalComponentAnalysis(maxPC:=2)
        Dim force = pca.GetPCAScore

        Return New Matrix With {
            .sampleID = {"x", "y"},
            .tag = $"{expr1.tag} ~ {expr2.tag}",
            .expression = force _
                .foreachRow _
                .Select(Function(r, i)
                            Return New DataFrameRow With {
                                .geneID = gene_ids(i),
                                .experiments = r.value _
                                    .Select(Function(d) CDbl(d)) _
                                    .ToArray
                            }
                        End Function) _
                .ToArray
        }
    End Function

    <Extension>
    Private Function corVec(gene As DataFrameRow, expr2 As Matrix, background As Background) As Double()
        Dim cor As Double() = New Double(background.clusters.Length - 1) {}
        Dim i As Integer = 0

        For Each map As Cluster In background.clusters
            Dim idset As String() = map.memberIds
            Dim mat As Matrix = expr2(idset)
            Dim cordata As Double() = mat.expression _
                .Select(Function(v)
                            Return Correlations.GetPearson(gene.experiments, v.experiments)
                        End Function) _
                .ToArray

            cor(i) = cordata.Sum
            i += 1
        Next

        Return cor
    End Function

End Module

