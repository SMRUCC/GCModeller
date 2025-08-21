Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Module Limma

    <Extension>
    Public Iterator Function LmFit(x As Matrix, design As DataAnalysis) As IEnumerable(Of DEGModel)
        Dim control = x.IndexOf(design.control)  ' 0
        Dim treat = x.IndexOf(design.experiment) ' 1
        Dim xi As Double() = Replicate(0.0, control.Length) _
            .JoinIterates(Replicate(1.0, treat.Length)) _
            .ToArray

        x = x.log(2)

        Dim logFC As Double() = New Double(x.size - 1) {}
        Dim residuals As Double()() = RectangularArray.Matrix(Of Double)(x.size, control.Length + treat.Length)
        Dim i As Integer = 0

        ' 拟合线性模型（逐基因）
        For Each gene As DataFrameRow In x.expression
            Dim y As Double() = gene(control).JoinIterates(gene(treat)).ToArray
            Dim lm As FitResult = LeastSquares.LinearFit(xi, y)

            logFC(i) = lm.Slope
            residuals(i) = lm.Residuals

            i += 1
        Next

        ' 经验贝叶斯方差调整
        ' a vs b
        Dim p As Integer = design.size
        Dim df_residual = (control.Length + treat.Length) - p
        Dim gene_vars As Double() = residuals _
            .Select(Function(e) (New Vector(e) ^ 2).Sum / df_residual) _
            .ToArray

        Dim prior_var As Double = gene_vars.Median
        Dim prior_df = 4  ' limma默认先验自由度
        Dim shrunk_vars = (prior_df * prior_var + df_residual * New Vector(gene_vars)) / (prior_df + df_residual)
        Dim shrunk_se = Vector.Sqrt(shrunk_vars)

        ' t检验与p值
        Dim t_stats = logFC / shrunk_se
        Dim df_total = prior_df + df_residual
        Dim t As Vector = -t_stats.Abs
        Dim p_values = SIMD.Multiply.f64_scalar_op_multiply_f64(2, Hypothesis.t.Pvalue(t, df:=df_total, Hypothesis.Hypothesis.Less))

        For offset As Integer = 0 To logFC.Length - 1
            Yield New DEGModel With {
                .logFC = logFC(offset),
                .label = x(offset).geneID,
                .pvalue = p_values(offset),
                .[class] = If(.pvalue < 0.05, If(.logFC > 0, "up", "down"), "not sig")
            }
        Next
    End Function
End Module
