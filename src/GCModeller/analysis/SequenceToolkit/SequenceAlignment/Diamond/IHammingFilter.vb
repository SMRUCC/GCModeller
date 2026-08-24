#Region "Microsoft.VisualBasic::9c96a885de5ca98ef343f97207a1c800, analysis\SequenceToolkit\SequenceAlignment\Diamond\IHammingFilter.vb"

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

    '   Total Lines: 34
    '    Code Lines: 6 (17.65%)
    ' Comment Lines: 24 (70.59%)
    '    - Xml Docs: 62.50%
    ' 
    '   Blank Lines: 4 (11.76%)
    '     File Size: 1.72 KB


    '     Interface IHammingFilter
    ' 
    '         Function: Distance, Pass
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Hamming 距离过滤接口 (SIMD 后续替换边界)
'
' DIAMOND 在种子命中点周围 48 个氨基酸窗口内计算查询与参考的 Hamming 距离,
' 作为第一级廉价初筛,将命中数量削减 1-2 个数量级。
'
' 本接口封装了"判断一对命中是否通过 Hamming 初筛"的边界,
' 当前提供标量实现(<see cref="HammingFilter"/>),后续可替换为
' System.Runtime.Intrinsics.X86 (SSE2: pcmpeqb / pmovmskb / popcnt) 向量化实现,
' 而调用方(DiamondBlastp 流水线)无需改动。

Namespace DIAMOND

    ''' <summary>
    ''' 在种子命中周围窗口计算 Hamming 距离并判定是否通过的过滤器。
    ''' </summary>
    Public Interface IHammingFilter

        ''' <summary>
        ''' 判断查询序列在 <paramref name="qPos"/> 与参考序列在 <paramref name="sPos"/>
        ''' 的种子命中是否通过 Hamming 距离初筛。
        ''' </summary>
        ''' <param name="query">完整查询序列(原始氨基酸字符)。</param>
        ''' <param name="qPos">种子在查询中的起始位置。</param>
        ''' <param name="subject">完整参考序列(原始氨基酸字符)。</param>
        ''' <param name="sPos">种子在参考中的起始位置。</param>
        ''' <returns>通过初筛返回 True。</returns>
        Function Pass(query As String, qPos As Integer, subject As String, sPos As Integer) As Boolean

        ''' <summary>
        ''' 计算并返回该命中窗口的实际 Hamming 距离(供排序/诊断使用)。
        ''' </summary>
        Function Distance(query As String, qPos As Integer, subject As String, sPos As Integer) As Integer
    End Interface
End Namespace

