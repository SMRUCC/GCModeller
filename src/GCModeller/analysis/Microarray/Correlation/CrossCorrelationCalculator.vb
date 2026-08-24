#Region "Microsoft.VisualBasic::41ed23d55b89133d0518c5a818815f7d, analysis\Microarray\Correlation\CrossCorrelationCalculator.vb"

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

    '   Total Lines: 153
    '    Code Lines: 95 (62.09%)
    ' Comment Lines: 36 (23.53%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 22 (14.38%)
    '     File Size: 6.40 KB


    ' Module CrossCorrelationCalculator
    ' 
    ' 
    '     Enum CorrelationMethod
    ' 
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: Compute, ComputeAll, ComputeCCLasso, ComputeMIC, ComputeSparCC
    '               ComputeSpearmanMIC
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Model.Network.Regulons
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

' ============================================================================
' 第七部分：统一调用接口
' ============================================================================

''' <summary>
''' 交叉相关性统一计算入口
''' 提供对四种相关性方法的统一调用接口
''' </summary>
Public Module CrossCorrelationCalculator

    ''' <summary>
    ''' 相关性计算方法枚举
    ''' </summary>
    Public Enum CorrelationMethod
        Pearson = 0
        Spearman = 1
        SparCC = 2
        CCLasso = 3
        MIC = 4
        SpearmanMIC = 5
    End Enum

    ''' <summary>
    ''' 使用指定方法计算 OTU 与代谢物之间的交叉相关矩阵
    ''' </summary>
    ''' <param name="otuMatrix">OTU 丰度表达矩阵</param>
    ''' <param name="metaboliteMatrix">代谢物表达矩阵</param>
    ''' <param name="method">相关性计算方法</param>
    ''' <returns>交叉相关性计算结果</returns>
    Public Function Compute(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        method As CorrelationMethod) As CrossOmicsCorrelation

        Select Case method
            Case CorrelationMethod.Pearson : Return PearsonCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.Spearman : Return SpearmanCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.SparCC : Return SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
            Case CorrelationMethod.CCLasso : Return CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
                ' Case CorrelationMethod.MIC : Return MICComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
                ' Case CorrelationMethod.SpearmanMIC : Return SpearmanMICCombined.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)

            Case Else
                Throw New ArgumentException("不支持的相关性计算方法: " & method.ToString())
        End Select
    End Function

    ''' <summary>
    ''' 使用全部 4 种方法计算 OTU 与代谢物之间的交叉相关矩阵
    ''' </summary>
    ''' <returns>包含 4 个 CorrelationResult 的数组，顺序为 Pearson, Spearman, SparCC, CCLasso</returns>
    Public Function ComputeAll(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix) As CrossOmicsCorrelation()

        Dim results As CrossOmicsCorrelation() = New CrossOmicsCorrelation(4) {}

        results(0) = PearsonCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(1) = SpearmanCorrelation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(2) = SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        results(3) = CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)
        ' results(4) = MICComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix)

        Return results
    End Function

    ''' <summary>
    ''' 使用自定义参数的 SparCC 计算
    ''' </summary>
    Public Function ComputeSparCC(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        iterations As Integer,
        correlationThreshold As Double,
        pseudoCount As Double) As CrossOmicsCorrelation

        Dim config As New SparCCComputation.SparCCConfig()
        config.Iterations = iterations
        config.CorrelationThreshold = correlationThreshold
        config.PseudoCount = pseudoCount
        config.UseExclusion = True

        Return SparCCComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function

    ''' <summary>
    ''' 使用自定义参数的 CCLasso 计算
    ''' </summary>
    Public Function ComputeCCLasso(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        lambda As Double,
        maxIterations As Integer,
        tolerance As Double,
        pseudoCount As Double) As CrossOmicsCorrelation

        Dim config As New CCLassoComputation.CCLassoConfig()
        config.Lambda = lambda
        config.MaxIterations = maxIterations
        config.Tolerance = tolerance
        config.PseudoCount = pseudoCount

        Return CCLassoComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function

    ''' <summary>
    ''' 使用自定义参数的 MIC 计算
    ''' </summary>
    Public Function ComputeMIC(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        Optional alpha As Double = 0.6,
        Optional useOptimalPartition As Boolean = True,
        Optional optimalIterations As Integer = 3,
        Optional permutationCount As Integer = 0) As SpearmanMICResult

        Dim config As New MICComputation.MICConfig()
        config.Alpha = alpha
        config.UseOptimalPartition = useOptimalPartition
        config.OptimalIterations = optimalIterations
        config.PermutationCount = permutationCount

        Return MICComputation.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function

    ''' <summary>
    ''' 使用自定义参数的 Spearman+MIC 联合分析
    ''' </summary>
    Public Function ComputeSpearmanMIC(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        Optional spearmanThreshold As Double = 0.3,
        Optional micThreshold As Double = 0.3,
        Optional pValueThreshold As Double = 0.05,
        Optional combinationMethod As SpearmanMICCombined.CombinationMethod =
            SpearmanMICCombined.CombinationMethod.FisherCombined,
        Optional weightSpearman As Double = 0.5,
        Optional weightMIC As Double = 0.5) As SpearmanMICResult

        Dim config As New SpearmanMICCombined.SpearmanMICConfig()
        config.SpearmanThreshold = spearmanThreshold
        config.MICThreshold = micThreshold
        config.PValueThreshold = pValueThreshold
        config.Method = combinationMethod
        config.WeightSpearman = weightSpearman
        config.WeightMIC = weightMIC

        Return SpearmanMICCombined.ComputeCrossCorrelation(otuMatrix, metaboliteMatrix, config)
    End Function
End Module

