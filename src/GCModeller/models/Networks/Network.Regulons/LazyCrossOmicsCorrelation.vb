#Region "Microsoft.VisualBasic::9190e6e28dbf450056f7b941e2af9535, models\Networks\Network.Regulons\LazyCrossOmicsCorrelation.vb"

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

    '   Total Lines: 76
    '    Code Lines: 35 (46.05%)
    ' Comment Lines: 28 (36.84%)
    '    - Xml Docs: 71.43%
    ' 
    '   Blank Lines: 13 (17.11%)
    '     File Size: 2.82 KB


    ' Class LazyCrossOmicsCorrelation
    ' 
    '     Properties: omics1, omics2
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: Correlation
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' Correlation between the molecules across two omics matrix data, such as gene expression and protein abundance
''' </summary>
Public Class LazyCrossOmicsCorrelation : Inherits CrossOmicsCorrelation

    ''' <summary>
    ''' the normalized expression matrix data of Omics 1
    ''' </summary>
    ReadOnly expr1 As Matrix

    ''' <summary>
    ''' the normalized expression matrix data of Omics 2
    ''' </summary>
    ReadOnly expr2 As Matrix

    Public Overrides ReadOnly Property omics1 As String()
        Get
            Return expr1.rownames
        End Get
    End Property

    Public Overrides ReadOnly Property omics2 As String()
        Get
            Return expr2.rownames
        End Get
    End Property

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="expr1">组学1的表达矩阵</param>
    ''' <param name="expr2">组学2的表达矩阵</param>
    ''' <param name="strict">是否严格要求样本对</param>
    Sub New(expr1 As Matrix, expr2 As Matrix, Optional strict As Boolean = True)
        Me.expr1 = expr1
        Me.expr2 = expr2

        Call ValidateSamples(expr1, expr2)
    End Sub

    ''' <summary>
    ''' 计算跨组学分子的相关性
    ''' </summary>
    ''' <param name="entity1">组学1中的分子名称 (如 Gene ID)</param>
    ''' <param name="entity2">组学2中的分子名称 (如 Protein ID)</param>
    ''' <returns></returns>
    Public Overrides Function Correlation(entity1 As String, entity2 As String) As (cor As Double, pval As Double)
        ' 检查缓存中是否已经计算过 (组学1分子在前，组学2分子在后)
        If Not cor.CheckElement(entity1, entity2) Then
            Dim c As Double, p As Double
            Dim v1 = expr1(entity1)
            Dim v2 = expr2(entity2)

            ' no correlation result for missing data
            If v1 Is Nothing OrElse v2 Is Nothing Then
                Return (0, 1)
            End If

            ' 计算皮尔逊相关性
            c = Correlations.GetPearson(v1.experiments, v2.experiments, p, throwMaxIterError:=False)

            ' 跨组学矩阵是不对称的，不需要像单组学那样同时写入反向 [entity2, entity1]
            ' 除非希望能够通过 Protein -> Gene 反向查询，如果内存允许，也可以加上反向缓存：
            ' Call cor.SetValue(entity2, entity1, c)
            ' Call pval.SetValue(entity2, entity1, p)

            Call cor.SetValue(entity1, entity2, c)
            Call pval.SetValue(entity1, entity2, p)
        End If

        Return (cor(entity1, entity2), pval(entity1, entity2))
    End Function
End Class

