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
