#Region "Microsoft.VisualBasic::c8242282d49ea0e8ffb49ea28b71bfd7, annotations\WGCNA\WGCNA\Result.vb"

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

    '   Total Lines: 20
    '    Code Lines: 13 (65.00%)
    ' Comment Lines: 4 (20.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 3 (15.00%)
    '     File Size: 709 B


    ' Class Result
    ' 
    '     Properties: beta, hclust, K, modules, network
    '                 softBeta, TOM
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

''' <summary>
''' WGCNA分析结果类
''' </summary>
Public Class Result

    ''' <summary>
    ''' 最佳beta值测试结果
    ''' </summary>
    Public Property beta As BetaTest
    ''' <summary>
    ''' 网络图对象
    ''' </summary>
    Public Property network As NetworkGraph
    ''' <summary>
    ''' 连接度向量
    ''' </summary>
    Public Property K As Vector
    ''' <summary>
    ''' TOM矩阵
    ''' </summary>
    Public Property TOM As GeneralMatrix
    ''' <summary>
    ''' 层次聚类树
    ''' </summary>
    Public Property hclust As Cluster
    ''' <summary>
    ''' 模块字典（模块名→基因列表）
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' modules is clustered based on the <see cref="TOM"/> matrix
    ''' </remarks>
    Public Property modules As Dictionary(Of String, String())
    ''' <summary>
    ''' 所有beta候选值的测试结果
    ''' </summary>
    Public Property softBeta As BetaTest()

    ''' <summary>
    ''' 模块特征基因字典（模块名→特征基因值数组）
    ''' 每个模块的特征基因是该模块内所有基因表达值的第一主成分
    ''' </summary>
    Public Property moduleEigengenes As Dictionary(Of String, Double())

    ''' <summary>
    ''' 模块特征基因详细结果列表
    ''' 包含方差解释比例等信息
    ''' </summary>
    Public Property moduleEigengeneResults As List(Of ModuleEigengeneResult)

    ''' <summary>
    ''' 模块与表型相关性结果列表
    ''' 存储每个模块与每个表型的相关性分析结果
    ''' </summary>
    Public Property modulePhenotypeCorrelations As List(Of ModulePhenotypeCorrelation)

    ''' <summary>
    ''' 基因显著性结果列表
    ''' 存储每个基因与每个表型的相关性分析结果
    ''' </summary>
    Public Property geneSignificance As List(Of GeneSignificanceResult)

    ''' <summary>
    ''' 模块成员结果列表
    ''' 存储每个基因与每个模块特征基因的相关性
    ''' </summary>
    Public Property moduleMembership As List(Of ModuleMembershipResult)

    ''' <summary>
    ''' 获取指定模块的特征基因
    ''' </summary>
    ''' <param name="moduleName">模块名称</param>
    ''' <returns>特征基因值数组，如果模块不存在则返回Nothing</returns>
    Public Function GetModuleEigengene(moduleName As String) As Double()
        If moduleEigengenes IsNot Nothing AndAlso moduleEigengenes.ContainsKey(moduleName) Then
            Return moduleEigengenes(moduleName)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' 获取指定模块与指定表型的相关性
    ''' </summary>
    ''' <param name="moduleName">模块名称</param>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <returns>相关性结果，如果不存在则返回Nothing</returns>
    Public Function GetModulePhenotypeCorrelation(moduleName As String, phenotypeName As String) As ModulePhenotypeCorrelation
        If modulePhenotypeCorrelations IsNot Nothing Then
            Return modulePhenotypeCorrelations.FirstOrDefault(Function(c) c.ModuleName = moduleName AndAlso c.PhenotypeName = phenotypeName)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' 获取与指定表型显著相关的模块列表
    ''' </summary>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <param name="pValueThreshold">p值阈值，默认0.05</param>
    ''' <returns>显著相关模块的相关性结果列表</returns>
    Public Function GetSignificantModules(phenotypeName As String, Optional pValueThreshold As Double = 0.05) As List(Of ModulePhenotypeCorrelation)
        If modulePhenotypeCorrelations Is Nothing Then
            Return New List(Of ModulePhenotypeCorrelation)()
        End If

        Return modulePhenotypeCorrelations _
            .Where(Function(c) c.PhenotypeName = phenotypeName AndAlso c.PValue < pValueThreshold) _
            .OrderByDescending(Function(c) c.AbsoluteCorrelation) _
            .ToList()
    End Function

    ''' <summary>
    ''' 获取指定模块中成员资格最高的基因
    ''' </summary>
    ''' <param name="moduleName">模块名称</param>
    ''' <param name="topN">返回的基因数量，默认10</param>
    ''' <returns>模块成员资格最高的基因列表</returns>
    Public Function GetTopModuleMembers(moduleName As String, Optional topN As Integer = 10) As List(Of ModuleMembershipResult)
        If moduleMembership Is Nothing Then
            Return New List(Of ModuleMembershipResult)()
        End If

        Return moduleMembership _
            .Where(Function(m) m.ModuleName = moduleName) _
            .OrderByDescending(Function(m) Math.Abs(m.Correlation)) _
            .Take(topN) _
            .ToList()
    End Function

    ''' <summary>
    ''' 获取对指定表型显著性最高的基因
    ''' </summary>
    ''' <param name="phenotypeName">表型名称</param>
    ''' <param name="topN">返回的基因数量，默认10</param>
    ''' <returns>基因显著性最高的基因列表</returns>
    Public Function GetTopSignificantGenes(phenotypeName As String, Optional topN As Integer = 10) As List(Of GeneSignificanceResult)
        If geneSignificance Is Nothing Then
            Return New List(Of GeneSignificanceResult)()
        End If

        Return geneSignificance _
            .Where(Function(g) g.PhenotypeName = phenotypeName) _
            .OrderByDescending(Function(g) g.AbsoluteCorrelation) _
            .Take(topN) _
            .ToList()
    End Function
End Class
