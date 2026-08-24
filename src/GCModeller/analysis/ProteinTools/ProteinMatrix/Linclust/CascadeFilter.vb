#Region "Microsoft.VisualBasic::d9e779f0b1d78bc143401761aa7925b0, analysis\ProteinTools\ProteinMatrix\Linclust\CascadeFilter.vb"

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

    '   Total Lines: 122
    '    Code Lines: 74 (60.66%)
    ' Comment Lines: 31 (25.41%)
    '    - Xml Docs: 61.29%
    ' 
    '   Blank Lines: 17 (13.93%)
    '     File Size: 5.08 KB


    '     Structure FastPass
    ' 
    ' 
    ' 
    '     Module CascadeFilter
    ' 
    '         Function: CharMatch, Filter
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 阶段三:级联快速过滤
'
' 对"成员 vs 中心"先以几乎零成本的汉明距离做粗筛,再通过无缺口局部比对
' 计算覆盖率与一致性,淘汰绝大多数假阳性 k-mer 匹配。
' 这一阶段的幸存者才会进入阶段四昂贵的 Smith-Waterman 带缺口比对。
'
' 汉明距离:从 k-mer 匹配位置向两端无缺口延伸,统计不匹配字符数。
' 无缺口局部比对:以 k-mer 锚点为中心,向两端线性延伸取最长匹配区,
' 计算该区的 identity(一致性)与 coverage(覆盖率 = 匹配区长度 / 较短序列长度)。

Imports System.Runtime.CompilerServices

Namespace Linclust

    ''' <summary>
    ''' 快速过滤结果
    ''' </summary>
    Public Structure FastPass
        ''' <summary>是否通过阶段三预筛选</summary>
        Public Pass As Boolean
        ''' <summary>锚点向两端延伸得到的最佳无缺口匹配长度</summary>
        Public MatchLength As Integer
        ''' <summary>该匹配区内的一致字符数(含匹配)</summary>
        Public Identical As Integer
        ''' <summary>匹配区在成员序列上的起点</summary>
        Public QueryStart As Integer
        ''' <summary>匹配区在中心序列上的起点</summary>
        Public SubjectStart As Integer
    End Structure

    Public Module CascadeFilter

        ''' <summary>
        ''' 阶段三:对 (member, center) 做汉明距离预筛 + 无缺口局部比对。
        ''' </summary>
        ''' <param name="member">成员原始序列(缩减字母表编码后)</param>
        ''' <param name="center">中心原始序列(缩减字母表编码后)</param>
        ''' <param name="kmerPosMember">k-mer 在成员序列中的位置</param>
        ''' <param name="kmerPosCenter">k-mer 在中心序列中的位置</param>
        ''' <param name="k">k-mer 长度</param>
        ''' <param name="coverage">覆盖率阈值(0-1)</param>
        ''' <param name="seqid">一致性阈值(0-1)</param>
        ''' <param name="allowWildcardMatch">通配符是否视为匹配(默认 True,容忍未知残基)</param>
        Public Function Filter(member As String, center As String, kmerPosMember As Integer, kmerPosCenter As Integer, k As Integer, coverage As Double, seqid As Double, Optional allowWildcardMatch As Boolean = True) As FastPass
            Dim result As New FastPass With {
                .Pass = False,
                .MatchLength = 0,
                .Identical = 0,
                .QueryStart = kmerPosMember,
                .SubjectStart = kmerPosCenter
            }

            If member Is Nothing OrElse center Is Nothing Then
                Return result
            End If
            If member.Length < k OrElse center.Length < k Then
                Return result
            End If

            ' 以 k-mer 锚点为中心向两端无缺口延伸,求最长匹配窗口
            Dim qStart = kmerPosMember
            Dim sStart = kmerPosCenter
            Dim qEnd = kmerPosMember + k - 1
            Dim sEnd = kmerPosCenter + k - 1

            ' 向左延伸
            While qStart > 0 AndAlso sStart > 0
                If CharMatch(member(qStart - 1), center(sStart - 1), allowWildcardMatch) Then
                    qStart -= 1
                    sStart -= 1
                Else
                    Exit While
                End If
            End While

            ' 向右延伸
            While qEnd < member.Length - 1 AndAlso sEnd < center.Length - 1
                If CharMatch(member(qEnd + 1), center(sEnd + 1), allowWildcardMatch) Then
                    qEnd += 1
                    sEnd += 1
                Else
                    Exit While
                End If
            End While

            Dim matchLen = qEnd - qStart + 1
            Dim identical = 0

            For i As Integer = 0 To matchLen - 1
                If CharMatch(member(qStart + i), center(sStart + i), allowWildcardMatch) Then
                    identical += 1
                End If
            Next

            Dim cov = CDbl(matchLen) / Math.Min(member.Length, center.Length)
            Dim id = CDbl(identical) / matchLen

            result.MatchLength = matchLen
            result.Identical = identical
            result.QueryStart = qStart
            result.SubjectStart = sStart
            result.Pass = (cov >= coverage) AndAlso (id >= seqid)

            Return result
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function CharMatch(a As Char, b As Char, allowWildcardMatch As Boolean) As Boolean
            If a = b Then
                Return True
            End If

            If allowWildcardMatch Then
                If a = ReducedAlphabet.Wildcard OrElse b = ReducedAlphabet.Wildcard Then
                    Return True
                End If
            End If

            Return False
        End Function
    End Module
End Namespace

