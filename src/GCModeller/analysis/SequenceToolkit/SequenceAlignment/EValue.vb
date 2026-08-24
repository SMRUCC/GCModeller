#Region "Microsoft.VisualBasic::746f71042c8fab7c72d8424b08b97a12, analysis\SequenceToolkit\SequenceAlignment\EValue.vb"

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

    '   Total Lines: 57
    '    Code Lines: 19 (33.33%)
    ' Comment Lines: 31 (54.39%)
    '    - Xml Docs: 38.71%
    ' 
    '   Blank Lines: 7 (12.28%)
    '     File Size: 2.46 KB


    ' Module EValue
    ' 
    '     Function: Compute
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 阶段四:E-value 统计模型(Karlin-Altschul)
'
' 局部比对显著性统计,用于判定"成员 vs 中心"的 Smith-Waterman 比对
' 是否显著(同源)。标准 Karlin-Altschul 公式:
'
'     E = K * m * n * exp(-lambda * S)
'
' 其中:
'   S = 比对原始得分(HSP.score)
'   m, n = 比对两条序列的长度(成员长度 × 中心长度)
'   lambda, K = 替换矩阵的统计量(BLOSUM62 通用近似:
'               lambda ≈ 0.267, K ≈ 0.041;源自 NCBI BLAST
'               对 BLOSUM62 的经验标定)
'
' E 值含义:在随机序列库中期望出现的同等或更好比对数。
' E 越小越显著,通常以 E <= 0.001(或 1e-5)作为同源阈值。

Imports System.Runtime.CompilerServices

Public Module EValue

    ''' <summary>BLOSUM62 的 λ 统计量(经验标定)</summary>
    Public Const LambdaBlosum62 As Double = 0.267

    ''' <summary>BLOSUM62 的 K 统计量(经验标定)</summary>
    Public Const KBlosum62 As Double = 0.041

    ''' <summary>
    ''' Karlin-Altschul 局部比对 E-value:
    ''' E = K * m * n * exp(-lambda * S)
    ''' </summary>
    ''' <param name="rawScore">比对原始得分 S(如 HSP.score)</param>
    ''' <param name="m">查询/成员序列长度</param>
    ''' <param name="n">数据库/中心序列长度</param>
    ''' <param name="lambda">λ 统计量(默认 BLOSUM62)</param>
    ''' <param name="K">K 统计量(默认 BLOSUM62)</param>
    ''' <returns>E-value;边界异常时返回极大值(保证不误连边)</returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Compute(rawScore As Double, m As Integer, n As Integer,
                            Optional lambda As Double = LambdaBlosum62,
                            Optional K As Double = KBlosum62) As Double
        If m <= 0 OrElse n <= 0 OrElse Double.IsNaN(rawScore) OrElse Double.IsInfinity(rawScore) Then
            ' 非法输入:返回极大值,使其不满足 E <= evalue 判据
            Return Double.MaxValue
        End If

        ' 负得分理论上不可能产生显著比对,直接给极大值
        If rawScore <= 0 Then
            Return Double.MaxValue
        End If

        Dim exponent = -lambda * rawScore
        ' 防止正溢出(虽然负指数不会溢出,但稳妥处理)
        Dim factor = Math.Exp(exponent)
        Return K * CDbl(m) * CDbl(n) * factor
    End Function
End Module

