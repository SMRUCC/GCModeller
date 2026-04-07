#Region "Microsoft.VisualBasic::244b8b77ae9d7b5372898ca2e7265cf6, annotations\WGCNA\WGCNA\PhenotypeResult.vb"

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

    '   Total Lines: 151
    '    Code Lines: 48 (31.79%)
    ' Comment Lines: 78 (51.66%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 25 (16.56%)
    '     File Size: 3.64 KB


    ' Class ModuleEigengeneResult
    ' 
    '     Properties: Eigengene, GeneCount, ModuleName, VarianceExplained
    ' 
    ' Class ModulePhenotypeCorrelation
    ' 
    '     Properties: AbsoluteCorrelation, Correlation, IsSignificant, ModuleName, PhenotypeName
    '                 PValue, SampleCount
    ' 
    '     Function: ToString
    ' 
    ' Class GeneSignificanceResult
    ' 
    '     Properties: AbsoluteCorrelation, Correlation, GeneId, PhenotypeName, PValue
    '                 SampleCount
    ' 
    '     Function: ToString
    ' 
    ' Class ModuleMembershipResult
    ' 
    '     Properties: Correlation, GeneId, ModuleName, PValue, SampleCount
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports std = System.Math

''' <summary>
''' 模块特征基因计算结果
''' </summary>
Public Class ModuleEigengeneResult
    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 模块特征基因值（每个样本一个值）
    ''' </summary>
    Public Property Eigengene As Double()

    ''' <summary>
    ''' 第一主成分解释的方差比例
    ''' </summary>
    Public Property VarianceExplained As Double

    ''' <summary>
    ''' 模块内基因数量
    ''' </summary>
    Public Property GeneCount As Integer
End Class

''' <summary>
''' 模块与表型相关性结果
''' </summary>
Public Class ModulePhenotypeCorrelation
    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 表型名称
    ''' </summary>
    Public Property PhenotypeName As String

    ''' <summary>
    ''' 相关系数
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' 相关系数的绝对值
    ''' </summary>
    Public ReadOnly Property AbsoluteCorrelation As Double
        Get
            Return std.Abs(Correlation)
        End Get
    End Property

    ''' <summary>
    ''' p值（统计显著性）
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    ''' <summary>
    ''' 是否显著（默认p &lt; 0.05）
    ''' </summary>
    Public ReadOnly Property IsSignificant As Boolean
        Get
            Return PValue < 0.05
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return $"Module '{ModuleName}' vs '{PhenotypeName}': r={Correlation:F3}, p={PValue:F4}"
    End Function
End Class

''' <summary>
''' 基因显著性结果
''' </summary>
Public Class GeneSignificanceResult
    ''' <summary>
    ''' 基因ID
    ''' </summary>
    Public Property GeneId As String

    ''' <summary>
    ''' 表型名称
    ''' </summary>
    Public Property PhenotypeName As String

    ''' <summary>
    ''' 相关系数
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' 相关系数的绝对值（基因显著性GS）
    ''' </summary>
    Public Property AbsoluteCorrelation As Double

    ''' <summary>
    ''' p值
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    Public Overrides Function ToString() As String
        Return $"Gene '{GeneId}' vs '{PhenotypeName}': GS={AbsoluteCorrelation:F3}, p={PValue:F4}"
    End Function
End Class

''' <summary>
''' 模块成员结果
''' </summary>
Public Class ModuleMembershipResult
    ''' <summary>
    ''' 基因ID
    ''' </summary>
    Public Property GeneId As String

    ''' <summary>
    ''' 模块名称
    ''' </summary>
    Public Property ModuleName As String

    ''' <summary>
    ''' 相关系数（模块成员MM）
    ''' </summary>
    Public Property Correlation As Double

    ''' <summary>
    ''' p值
    ''' </summary>
    Public Property PValue As Double

    ''' <summary>
    ''' 样本数量
    ''' </summary>
    Public Property SampleCount As Integer

    Public Overrides Function ToString() As String
        Return $"Gene '{GeneId}' in '{ModuleName}': MM={Correlation:F3}, p={PValue:F4}"
    End Function
End Class

