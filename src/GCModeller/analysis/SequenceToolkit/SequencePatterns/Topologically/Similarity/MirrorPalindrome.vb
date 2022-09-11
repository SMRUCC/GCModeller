#Region "Microsoft.VisualBasic::e72b578a2a135e634c099cf956e61a33, GCModeller\analysis\SequenceToolkit\SequencePatterns\Topologically\Similarity\MirrorPalindrome.vb"

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

    '   Total Lines: 107
    '    Code Lines: 63
    ' Comment Lines: 29
    '   Blank Lines: 15
    '     File Size: 4.53 KB


    '     Module MirrorPalindrome
    ' 
    '         Function: CreateMirrors, haveMirror
    ' 
    '     Class FuzzyMirrors
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Sub: DoSearch
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Topologically.Seeding
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically.SimilarityMatches

    ''' <summary>
    ''' 模糊相等的
    ''' </summary>
    Public Module MirrorPalindrome

        ''' <summary>
        ''' 判断目标序列<paramref name="sequence"/>上面是否包含有目标种子位点的镜像位点
        ''' </summary>
        ''' <param name="l">目标片段的长度</param>
        ''' <param name="loci">位点的位置</param>
        ''' <param name="mirror">进行模糊比较的参考序列</param>
        ''' <param name="sequence">基因组序列</param>
        ''' <returns></returns>
        <Extension> Private Function haveMirror(l%, loci%, mirror$, sequence$, cut#, maxDist%) As NamedValue(Of Integer)
            ' 左端的起始位置
            Dim mrStart As Integer = loci + l
            Dim ref As Integer() = mirror.Select(AddressOf Asc).ToArray

            For i As Integer = 0 To maxDist
                Dim mMirr As String = Mid(sequence, mrStart, l)
                ' 和参考序列做模糊匹配
                Dim edits = LevenshteinDistance.ComputeDistance(ref, mMirr)
                Dim score As Double = If(edits Is Nothing, -1, edits.MatchSimilarity)

                If score >= cut Then
                    ' 右端的结束位置
                    Return New NamedValue(Of Integer)(mMirr, mrStart + l)
                Else
                    l += 1
                End If
            Next

            Return New NamedValue(Of Integer)(Nothing, -1)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segment">搜索所使用到的种子序列片段</param>
        ''' <param name="sequence"></param>
        ''' <param name="maxDist">两个片段之间的最大的距离</param>
        ''' <param name="cut"></param>
        ''' <returns></returns>
        <ExportAPI("Mirrors.Locis.Get")>
        Public Function CreateMirrors(segment As String, sequence As String, maxDist As Integer, Optional cut As Double = 0.6) As PalindromeLoci()
            Dim locations As Integer() = IScanner.FindLocation(sequence, segment).ToArray

            If locations.IsNullOrEmpty Then
                Return Nothing
            End If

            ' 这个是目标片段的镜像回文部分，也是需要进行比较的参考序列
            Dim mirror As String = New String(segment.Reverse.ToArray)
            Dim l As Integer = Len(segment)
            Dim result = (From loci As Integer
                          In locations.AsParallel
                          Let ml As NamedValue(Of Integer) = haveMirror(l, loci, mirror, sequence, cut, maxDist)
                          Where ml.Value > -1
                          Select loci, ml).ToArray

            Return result _
                .Select(Function(site)
                            Return New PalindromeLoci With {
                                .Loci = segment,
                                .Start = site.loci,
                                .PalEnd = site.ml.Value,
                                .Palindrome = site.ml.Name,
                                .MirrorSite = mirror
                            }
                        End Function) _
                .ToArray
        End Function
    End Module

    Public Class FuzzyMirrors : Inherits Topologically.MirrorPalindrome

        ReadOnly _maxDist As Integer, cut As Double

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        Sub New(seq As IPolymerSequenceModel, min%, max%, maxDist%, cut#)
            Call MyBase.New(seq, min, max)

            Me.cut = cut
            Me._maxDist = maxDist
        End Sub

        Protected Overrides Sub DoSearch(seed As Seed)
            Call CreateMirrors(seed.sequence, seq, _maxDist, cut).DoCall(AddressOf m_resultSet.Add)
        End Sub
    End Class
End Namespace
