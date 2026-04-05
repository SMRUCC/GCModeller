#Region "Microsoft.VisualBasic::WGCNA\ModulePhenotype.vb"

' Author:
' 
'       WGCNA Module-Phenotype Correlation Analysis
'       新增功能：模块与表型相关性计算
' 
' Copyright (c) 2024 GPL3 Licensed
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

' Module ModulePhenotype
' 
'     Function: CalculateModuleEigengene, CalculateModulePhenotypeCorrelation,
'               CalculateGeneSignificance, CalculateModuleMembership,
'               CalculateAllModulePhenotypeCorrelations
' 
' Class ModuleEigengeneResult
'     Properties: Eigengene, VarianceExplained, ModuleName
' 
' Class ModulePhenotypeCorrelation
'     Properties: ModuleName, PhenotypeName, Correlation, PValue
' 
' Class GeneSignificanceResult
'     Properties: GeneId, PhenotypeName, Correlation, PValue
' 
' Class ModuleMembershipResult
'     Properties: GeneId, ModuleName, Correlation, PValue
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.Matrix
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports std = System.Math
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' WGCNA模块与表型相关性分析模块
''' 
''' 该模块实现了WGCNA分析中关键的模块-表型关联分析功能：
''' 1. 模块特征基因(Module Eigengene, ME)计算 - 使用PCA提取模块的第一主成分
''' 2. 模块与表型相关性计算 - 分析模块特征基因与表型数据的相关性
''' 3. 基因显著性(Gene Significance, GS)计算 - 分析单个基因与表型的相关性
''' 4. 模块成员(Module Membership, MM)计算 - 分析基因与模块特征基因的相关性
''' </summary>
Public Module ModulePhenotype

    ''' <summary>
    ''' 计算模块特征基因 (Module Eigengene, ME)
    ''' 
    ''' 模块特征基因是模块内所有基因表达值的第一主成分(PC1)，
    ''' 它代表了该模块的整体表达模式，是模块的代表性"虚拟基因"。
    ''' 
    ''' 计算方法：
    ''' 1. 提取模块内所有基因的表达矩阵
    ''' 2. 对表达矩阵进行PCA分析
    ''' 3. 取第一主成分作为模块特征基因
    ''' 4. 计算第一主成分解释的方差比例
    ''' </summary>
    ''' <param name="expressionMatrix">基因表达矩阵（基因×样本）</param>
    ''' <param name="moduleGenes">模块内的基因ID列表</param>
    ''' <param name="moduleName">模块名称</param>
    ''' <returns>模块特征基因计算结果</returns>
    Public Function CalculateModuleEigengene(expressionMatrix As Matrix, moduleGenes As String(), moduleName As String) As ModuleEigengeneResult
        ' 验证输入参数
        If moduleGenes Is Nothing OrElse moduleGenes.Length = 0 Then
            Throw New ArgumentException("模块基因列表不能为空")
        End If

        ' 提取模块内基因的表达数据
        Dim moduleExpression As New List(Of Double())
        Dim validGenes As New List(Of String)

        For Each geneId As String In moduleGenes
            moduleExpression.Add(expressionMatrix(geneId).experiments)
            validGenes.Add(geneId)
        Next

        If validGenes.Count = 0 Then
            Throw New ArgumentException($"模块 '{moduleName}' 中没有找到有效的基因表达数据")
        End If

        ' 构建模块表达矩阵（基因×样本）
        Dim nGenes As Integer = validGenes.Count
        Dim nSamples As Integer = moduleExpression(0).Length
        Dim moduleMatrix As Double()() = New Double(nGenes - 1)() {}

        For i As Integer = 0 To nGenes - 1
            moduleMatrix(i) = moduleExpression(i)
        Next

        ' 进行PCA分析
        ' 注意：PCA通常在列上进行，所以需要转置（样本×基因）
        Dim pcaInput As New List(Of NamedCollection(Of Double))
        For j As Integer = 0 To nSamples - 1
            Dim sampleData As Double() = New Double(nGenes - 1) {}
            For i As Integer = 0 To nGenes - 1
                sampleData(i) = moduleMatrix(i)(j)
            Next
            pcaInput.Add(New NamedCollection(Of Double)("Sample" & j, sampleData))
        Next

        ' 执行PCA
        Dim pcaResult = pcaInput.PrincipalComponentAnalysis(maxPC:=1)

        ' 获取第一主成分得分作为模块特征基因
        Dim eigengene As Double() = pcaResult.GetPCAScore(0)

        ' 计算方差解释比例
        Dim varianceExplained As Double = pcaResult.ProportionOfVariance(0)

        Return New ModuleEigengeneResult With {
            .ModuleName = moduleName,
            .Eigengene = eigengene,
            .VarianceExplained = varianceExplained,
            .GeneCount = nGenes
        }
    End Function

    ''' <summary>
    ''' 计算模块特征基因与表型的相关性
    ''' 
    ''' 这是WGCNA分析的核心步骤之一，用于识别与特定表型显著相关的模块。
    ''' 
    ''' 计算方法：
    ''' 1. 获取模块特征基因的表达值
    ''' 2. 计算特征基因与表型值的Pearson相关系数
    ''' 3. 计算相关性的统计显著性(p值)
    ''' </summary>
    ''' <param name="eigengene">模块特征基因值（样本数）</param>
    ''' <param name="phenotypeValues">表型值（样本数）</param>
    ''' <param name="moduleName">模块名称</param>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <returns>模块-表型相关性结果</returns>
    Public Function CalculateModulePhenotypeCorrelation(eigengene As Double(),
                                                        phenotypeValues As Double(),
                                                        moduleName As String,
                                                        phenotypeName As String) As ModulePhenotypeCorrelation
        ' 验证输入
        If eigengene.Length <> phenotypeValues.Length Then
            Throw New ArgumentException($"模块特征基因长度({eigengene.Length})与表型值长度({phenotypeValues.Length})不匹配")
        End If

        Dim n As Integer = eigengene.Length
        If n < 3 Then
            Throw New ArgumentException("样本数量太少，无法计算相关性（至少需要3个样本）")
        End If

        ' 计算Pearson相关系数
        Dim correlation As Double = PearsonCorrelation(eigengene, phenotypeValues)

        ' 计算p值（双尾检验）
        ' t统计量: t = r * sqrt((n-2)/(1-r^2))
        Dim tStat As Double
        Dim pValue As Double

        If std.Abs(correlation) >= 1.0 Then
            ' 完全相关的情况
            pValue = 0.0
        Else
            tStat = correlation * std.Sqrt((n - 2) / (1 - correlation * correlation))
            ' 使用t分布计算p值
            pValue = TTestPValue(tStat, n - 2)
        End If

        Return New ModulePhenotypeCorrelation With {
            .ModuleName = moduleName,
            .PhenotypeName = phenotypeName,
            .Correlation = correlation,
            .PValue = pValue,
            .SampleCount = n
        }
    End Function

    ''' <summary>
    ''' 计算所有模块与所有表型的相关性矩阵
    ''' 
    ''' 这是WGCNA分析中常用的功能，生成模块×表型的相关性热图数据
    ''' </summary>
    ''' <param name="expressionMatrix">基因表达矩阵</param>
    ''' <param name="modules">模块字典（模块名→基因列表）</param>
    ''' <param name="phenotypeData">表型数据字典（表型名→值数组）</param>
    ''' <returns>模块-表型相关性结果列表</returns>
    Public Function CalculateAllModulePhenotypeCorrelations(expressionMatrix As Matrix,
                                                             modules As Dictionary(Of String, String()),
                                                             phenotypeData As Dictionary(Of String, Double())) As List(Of ModulePhenotypeCorrelation)
        Dim results As New List(Of ModulePhenotypeCorrelation)

        ' 计算每个模块的特征基因
        Dim eigengenes As New Dictionary(Of String, Double())
        For Each moduleKvp In modules
            Try
                Dim meResult = CalculateModuleEigengene(expressionMatrix, moduleKvp.Value, moduleKvp.Key)
                eigengenes(moduleKvp.Key) = meResult.Eigengene
            Catch ex As Exception
                ' 跳过无法计算特征基因的模块
                Continue For
            End Try
        Next

        ' 计算每个模块与每个表型的相关性
        For Each phenotypeKvp In phenotypeData
            For Each eigengeneKvp In eigengenes
                Try
                    Dim corrResult = CalculateModulePhenotypeCorrelation(
                        eigengeneKvp.Value,
                        phenotypeKvp.Value,
                        eigengeneKvp.Key,
                        phenotypeKvp.Key
                    )
                    results.Add(corrResult)
                Catch ex As Exception
                    ' 跳过计算失败的相关性
                    Continue For
                End Try
            Next
        Next

        Return results
    End Function

    ''' <summary>
    ''' 计算基因显著性 (Gene Significance, GS)
    ''' 
    ''' 基因显著性定义为基因表达与表型值的相关性。
    ''' 高GS值的基因可能与所研究的表型密切相关。
    ''' 
    ''' 计算方法：
    ''' GS(i) = |cor(x_i, T)|
    ''' 其中x_i是基因i的表达值，T是表型值
    ''' </summary>
    ''' <param name="geneExpression">基因表达值（样本数）</param>
    ''' <param name="phenotypeValues">表型值（样本数）</param>
    ''' <param name="geneId">基因ID</param>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <returns>基因显著性结果</returns>
    Public Function CalculateGeneSignificance(geneExpression As Double(),
                                              phenotypeValues As Double(),
                                              geneId As String,
                                              phenotypeName As String) As GeneSignificanceResult
        ' 验证输入
        If geneExpression.Length <> phenotypeValues.Length Then
            Throw New ArgumentException($"基因表达长度({geneExpression.Length})与表型值长度({phenotypeValues.Length})不匹配")
        End If

        Dim n As Integer = geneExpression.Length

        ' 计算Pearson相关系数
        Dim correlation As Double = PearsonCorrelation(geneExpression, phenotypeValues)

        ' 计算p值
        Dim pValue As Double
        If std.Abs(correlation) >= 1.0 Then
            pValue = 0.0
        Else
            Dim tStat As Double = correlation * std.Sqrt((n - 2) / (1 - correlation * correlation))
            pValue = TTestPValue(tStat, n - 2)
        End If

        Return New GeneSignificanceResult With {
            .GeneId = geneId,
            .PhenotypeName = phenotypeName,
            .Correlation = correlation,
            .AbsoluteCorrelation = std.Abs(correlation),
            .PValue = pValue,
            .SampleCount = n
        }
    End Function

    ''' <summary>
    ''' 计算所有基因对特定表型的显著性
    ''' </summary>
    ''' <param name="expressionMatrix">基因表达矩阵</param>
    ''' <param name="phenotypeValues">表型值</param>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <returns>基因显著性结果列表</returns>
    Public Function CalculateAllGeneSignificance(expressionMatrix As Matrix,
                                                  phenotypeValues As Double(),
                                                  phenotypeName As String) As List(Of GeneSignificanceResult)
        Dim results As New List(Of GeneSignificanceResult)

        For Each geneKvp In expressionMatrix.expression
            Try
                Dim gsResult = CalculateGeneSignificance(
                    geneKvp.experiments,
                    phenotypeValues,
                    geneKvp.geneID,
                    phenotypeName
                )
                results.Add(gsResult)
            Catch ex As Exception
                Continue For
            End Try
        Next

        Return results
    End Function

    ''' <summary>
    ''' 计算模块成员 (Module Membership, MM)
    ''' 
    ''' 模块成员定义为基因表达与模块特征基因的相关性。
    ''' 高MM值的基因是该模块的核心成员。
    ''' 
    ''' 计算方法：
    ''' MM(i) = cor(x_i, ME)
    ''' 其中x_i是基因i的表达值，ME是模块特征基因
    ''' </summary>
    ''' <param name="geneExpression">基因表达值（样本数）</param>
    ''' <param name="eigengene">模块特征基因值（样本数）</param>
    ''' <param name="geneId">基因ID</param>
    ''' <param name="moduleName">模块名称</param>
    ''' <returns>模块成员结果</returns>
    Public Function CalculateModuleMembership(geneExpression As Double(),
                                              eigengene As Double(),
                                              geneId As String,
                                              moduleName As String) As ModuleMembershipResult
        ' 验证输入
        If geneExpression.Length <> eigengene.Length Then
            Throw New ArgumentException($"基因表达长度({geneExpression.Length})与特征基因长度({eigengene.Length})不匹配")
        End If

        Dim n As Integer = geneExpression.Length

        ' 计算Pearson相关系数
        Dim correlation As Double = PearsonCorrelation(geneExpression, eigengene)

        ' 计算p值
        Dim pValue As Double
        If std.Abs(correlation) >= 1.0 Then
            pValue = 0.0
        Else
            Dim tStat As Double = correlation * std.Sqrt((n - 2) / (1 - correlation * correlation))
            pValue = TTestPValue(tStat, n - 2)
        End If

        Return New ModuleMembershipResult With {
            .GeneId = geneId,
            .ModuleName = moduleName,
            .Correlation = correlation,
            .PValue = pValue,
            .SampleCount = n
        }
    End Function

    ''' <summary>
    ''' 计算所有基因对所有模块的成员资格
    ''' </summary>
    ''' <param name="expressionMatrix">基因表达矩阵</param>
    ''' <param name="modules">模块字典</param>
    ''' <returns>模块成员结果列表</returns>
    Public Function CalculateAllModuleMembership(expressionMatrix As Matrix,
                                                  modules As Dictionary(Of String, String())) As List(Of ModuleMembershipResult)
        Dim results As New List(Of ModuleMembershipResult)

        ' 首先计算所有模块的特征基因
        Dim eigengenes As New Dictionary(Of String, Double())
        For Each moduleKvp In modules
            Try
                Dim meResult = CalculateModuleEigengene(expressionMatrix, moduleKvp.Value, moduleKvp.Key)
                eigengenes(moduleKvp.Key) = meResult.Eigengene
            Catch ex As Exception
                Continue For
            End Try
        Next

        ' 计算每个基因与每个模块特征基因的相关性
        For Each geneKvp In expressionMatrix.expression
            For Each eigengeneKvp In eigengenes
                Try
                    Dim mmResult = CalculateModuleMembership(
                        geneKvp.experiments,
                        eigengeneKvp.Value,
                        geneKvp.geneID,
                        eigengeneKvp.Key
                    )
                    results.Add(mmResult)
                Catch ex As Exception
                    Continue For
                End Try
            Next
        Next

        Return results
    End Function

#Region "辅助函数"

    ''' <summary>
    ''' 计算Pearson相关系数
    ''' </summary>
    Private Function PearsonCorrelation(x As Double(), y As Double()) As Double
        Dim n As Integer = x.Length
        Dim sumX As Double = 0, sumY As Double = 0
        Dim sumXY As Double = 0, sumX2 As Double = 0, sumY2 As Double = 0

        For i As Integer = 0 To n - 1
            sumX += x(i)
            sumY += y(i)
            sumXY += x(i) * y(i)
            sumX2 += x(i) * x(i)
            sumY2 += y(i) * y(i)
        Next

        Dim numerator As Double = n * sumXY - sumX * sumY
        Dim denominator As Double = std.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY))

        If denominator = 0 Then
            Return 0
        End If

        Return numerator / denominator
    End Function

    ''' <summary>
    ''' 计算t检验的双尾p值
    ''' 使用近似计算方法
    ''' </summary>
    Private Function TTestPValue(tStat As Double, df As Integer) As Double
        ' 使用正态近似计算p值（对于大样本）
        ' 对于小样本，这里使用简化的近似方法
        ' 实际应用中建议使用更精确的统计库

        Dim absT As Double = std.Abs(tStat)

        ' 使用近似公式计算p值
        ' p ≈ 2 * (1 - Φ(|t|)) 其中Φ是标准正态CDF
        ' 对于大df，t分布近似于正态分布

        If df >= 30 Then
            ' 使用正态近似
            Dim pValue As Double = 2 * (1 - NormalCDF(absT))
            Return pValue
        Else
            ' 对于小样本，使用简化的近似
            ' 这是一个粗略的近似，实际应用中应使用精确的t分布计算
            Dim x As Double = df / (df + absT * absT)
            Dim pValue As Double = BetaIncomplete(0.5 * df, 0.5, x)
            Return pValue
        End If
    End Function

    ''' <summary>
    ''' 标准正态分布的累积分布函数（近似计算）
    ''' </summary>
    Private Function NormalCDF(x As Double) As Double
        ' 使用近似公式计算标准正态CDF
        ' 基于Abramowitz and Stegun的近似
        Dim a1 As Double = 0.254829592
        Dim a2 As Double = -0.284496736
        Dim a3 As Double = 1.421413741
        Dim a4 As Double = -1.453152027
        Dim a5 As Double = 1.061405429
        Dim p As Double = 0.3275911

        Dim sign As Integer = If(x < 0, -1, 1)
        x = std.Abs(x) / std.Sqrt(2)

        Dim t As Double = 1.0 / (1.0 + p * x)
        Dim y As Double = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * std.Exp(-x * x)

        Return 0.5 * (1.0 + sign * y)
    End Function

    ''' <summary>
    ''' 不完全Beta函数（近似计算）
    ''' 用于计算t分布的p值
    ''' </summary>
    Private Function BetaIncomplete(a As Double, b As Double, x As Double) As Double
        ' 简化的不完全Beta函数近似
        ' 这是一个粗略的实现，实际应用中应使用专业的统计库
        If x <= 0 Then Return 0
        If x >= 1 Then Return 1

        ' 使用连分数展开的近似
        Dim bt As Double = std.Exp(std.Log(x) * a + std.Log(1 - x) * b - LogBeta(a, b))

        If x < (a + 1) / (a + b + 2) Then
            Return bt * BetaCF(a, b, x) / a
        Else
            Return 1 - bt * BetaCF(b, a, 1 - x) / b
        End If
    End Function

    ''' <summary>
    ''' Beta函数的连分数展开
    ''' </summary>
    Private Function BetaCF(a As Double, b As Double, x As Double) As Double
        Const MAX_ITER As Integer = 100
        Const EPS As Double = 0.0000003

        Dim qab As Double = a + b
        Dim qap As Double = a + 1
        Dim qam As Double = a - 1
        Dim c As Double = 1
        Dim d As Double = 1 - qab * x / qap

        If std.Abs(d) < 1.0E-30 Then d = 1.0E-30
        d = 1 / d
        Dim h As Double = d

        For m As Integer = 1 To MAX_ITER
            Dim m2 As Integer = 2 * m
            Dim aa As Double = m * (b - m) * x / ((qam + m2) * (a + m2))
            d = 1 + aa * d
            If std.Abs(d) < 1.0E-30 Then d = 1.0E-30
            c = 1 + aa / c
            If std.Abs(c) < 1.0E-30 Then c = 1.0E-30
            d = 1 / d
            h *= d * c
            aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
            d = 1 + aa * d
            If std.Abs(d) < 1.0E-30 Then d = 1.0E-30
            c = 1 + aa / c
            If std.Abs(c) < 1.0E-30 Then c = 1.0E-30
            d = 1 / d
            Dim del As Double = d * c
            h *= del
            If std.Abs(del - 1) < EPS Then Exit For
        Next

        Return h
    End Function

    ''' <summary>
    ''' 计算Beta函数的对数
    ''' </summary>
    Private Function LogBeta(a As Double, b As Double) As Double
        Return LogGamma(a) + LogGamma(b) - LogGamma(a + b)
    End Function

    ''' <summary>
    ''' 计算Gamma函数的对数（使用Stirling近似）
    ''' </summary>
    Private Function LogGamma(x As Double) As Double
        Dim cof() As Double = {76.180091729471457, -86.505320329416776,
                               24.014098240830911, -1.231739572450155,
                               0.001208650973866179, -0.000005395239384953}
        Dim ser As Double = 1.0000000001900149
        Dim tmp As Double = x + 5.5
        tmp -= (x + 0.5) * std.Log(tmp)
        Dim y As Double = x

        For j As Integer = 0 To 5
            y += 1
            ser += cof(j) / y
        Next

        Return -tmp + std.Log(2.506628274631 * ser / x)
    End Function

#End Region

End Module
