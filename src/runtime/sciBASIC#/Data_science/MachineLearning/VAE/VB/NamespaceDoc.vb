' ============================================================================
' NamespaceDoc.vb - VariationalBayes 命名空间文档
'
' 均值场变分贝叶斯（Mean-Field Variational Bayes）模型族
' ============================================================================

Namespace VariationalBayes

    ''' <summary>
    ''' 均值场变分贝叶斯（Mean-Field Variational Bayes）模型族。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这一命名空间提供了基于坐标上升（coordinate ascent）的变分推断框架，
    ''' 以及建立在同一个框架之上的三个具体模型：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><see cref="VariationalGaussianMixture"/>：Dirichlet + Normal-Wishart 共轭先验的变分贝叶斯高斯混合模型；</item>
    ''' <item><see cref="VariationalLinearRegression"/>：对权重与精度超参数同时做均值场近似的贝叶斯线性回归；</item>
    ''' <item><see cref="VariationalPCA"/>：带自动相关性判定（ARD）的变分贝叶斯主成分分析。</item>
    ''' </list>
    ''' <para>
    ''' 公共数据接口统一使用 <c>Microsoft.VisualBasic.MachineLearning.TensorFlow.Tensor</c>，
    ''' 内部的矩阵分解（求逆、行列式、Cholesky）下沉到 <c>LinearAlgebra.Matrix</c> 项目。
    ''' </para>
    ''' </remarks>
    Public Class NamespaceDoc
    End Class

End Namespace
