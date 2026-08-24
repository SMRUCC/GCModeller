#Region "Microsoft.VisualBasic::9dd98d6b0f0e04f23f2ad06a862085e7, annotations\WGCNA\WGCNA\ResultTables\GeneSignificanceResult.vb"

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

    '   Total Lines: 39
    '    Code Lines: 11 (28.21%)
    ' Comment Lines: 21 (53.85%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (17.95%)
    '     File Size: 936 B


    ' Class GeneSignificanceResult
    ' 
    '     Properties: AbsoluteCorrelation, Correlation, GeneId, PhenotypeName, PValue
    '                 SampleCount
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region


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
