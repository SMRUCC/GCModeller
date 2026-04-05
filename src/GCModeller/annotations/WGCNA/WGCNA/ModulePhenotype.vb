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
Imports Microsoft.VisualBasic.Math.Correlations

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
        Dim pValue As Double
        ' 计算Pearson相关系数
        Dim correlation As Double = Correlations.GetPearson(eigengene, phenotypeValues, prob:=pValue)

        If std.Abs(correlation) >= 1.0 Then
            ' 完全相关的情况
            pValue = 0.0
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
    Public Iterator Function CalculateAllModulePhenotypeCorrelations(expressionMatrix As Matrix,
                                                             modules As Dictionary(Of String, String()),
                                                             phenotypeData As Dictionary(Of String, Double())) As IEnumerable(Of ModulePhenotypeCorrelation)

        ' 计算每个模块的特征基因
        Dim eigengenes As New Dictionary(Of String, Double())
        For Each moduleKvp In modules
            Dim meResult = CalculateModuleEigengene(expressionMatrix, moduleKvp.Value, moduleKvp.Key)
            eigengenes(moduleKvp.Key) = meResult.Eigengene
        Next

        ' 计算每个模块与每个表型的相关性
        For Each phenotypeKvp In phenotypeData
            For Each eigengeneKvp In eigengenes
                Dim corrResult = CalculateModulePhenotypeCorrelation(
     eigengeneKvp.Value,
     phenotypeKvp.Value,
     eigengeneKvp.Key,
     phenotypeKvp.Key
 )
                Yield corrResult
            Next
        Next
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
        ' 计算p值
        Dim pValue As Double
        ' 计算Pearson相关系数
        Dim correlation As Double = Correlations.GetPearson(geneExpression, phenotypeValues, prob:=pValue)

        If std.Abs(correlation) >= 1.0 Then
            pValue = 0.0
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
    Public Iterator Function CalculateAllGeneSignificance(expressionMatrix As Matrix,
                                                  phenotypeValues As Double(),
                                                  phenotypeName As String) As IEnumerable(Of GeneSignificanceResult)

        For Each geneKvp In expressionMatrix.expression
            Dim gsResult = CalculateGeneSignificance(
     geneKvp.experiments,
     phenotypeValues,
     geneKvp.geneID,
     phenotypeName
 )
            Yield gsResult
        Next
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
        ' 计算p值
        Dim pValue As Double
        ' 计算Pearson相关系数
        Dim correlation As Double = Correlations.GetPearson(geneExpression, eigengene, prob:=pValue)

        If std.Abs(correlation) >= 1.0 Then
            pValue = 0.0
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
    Public Iterator Function CalculateAllModuleMembership(expressionMatrix As Matrix, modules As Dictionary(Of String, String())) As IEnumerable(Of ModuleMembershipResult)
        ' 首先计算所有模块的特征基因
        Dim eigengenes As New Dictionary(Of String, Double())
        For Each moduleKvp In modules
            Dim meResult = CalculateModuleEigengene(expressionMatrix, moduleKvp.Value, moduleKvp.Key)
            eigengenes(moduleKvp.Key) = meResult.Eigengene
        Next

        ' 计算每个基因与每个模块特征基因的相关性
        For Each geneKvp In expressionMatrix.expression
            For Each eigengeneKvp In eigengenes
                Dim mmResult = CalculateModuleMembership(
                     geneKvp.experiments,
                     eigengeneKvp.Value,
                     geneKvp.geneID,
                     eigengeneKvp.Key
                 )
                Yield mmResult
            Next
        Next
    End Function
End Module
