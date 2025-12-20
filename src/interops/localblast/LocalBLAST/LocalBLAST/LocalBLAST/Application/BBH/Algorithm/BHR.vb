#Region "Microsoft.VisualBasic::ee0efea00a581dfd1209abb530ba0e11, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Algorithm\BHR.vb"

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

'   Total Lines: 248
'    Code Lines: 134 (54.03%)
' Comment Lines: 99 (39.92%)
'    - Xml Docs: 82.83%
' 
'   Blank Lines: 15 (6.05%)
'     File Size: 11.93 KB


'     Enum Levels
' 
'         BBH, NA, PartialBBH, SBH
' 
'  
' 
' 
' 
'     Module BHR
' 
'         Function: BHRResult, getHitRates, getTopBHR, HitRate, ReverseAssembly
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' The blast result alignment identify levels
    ''' </summary>
    Public Enum Levels
        ''' <summary>
        ''' A和B之间没有同源性
        ''' </summary>
        NA
        ''' <summary>
        ''' A vs B 以及 B vs A 都是得分最高的(BHR = 1)
        ''' </summary>
        BBH
        ''' <summary>
        ''' A vs B 以及 B vs A 都不是得分最高的,但是BHR得分是最高的 (BHR >= threshold)
        ''' </summary>
        PartialBBH
        ''' <summary>
        ''' 只有一个方向的比对结果是最高的
        ''' </summary>
        SBH
    End Enum

    ''' <summary>
    ''' **BHR(Bi-directional hit rate)** 
    ''' 
    ''' http://www.genome.jp/tools/kaas/help.html
    ''' 
    ''' 把要注释的genome作为``query``，和KEGG数据库中的``reference``进行blast比对，输出的结果（E>10）称为 homolog。
    ''' 同时把reference作为query，把genome作为refernce，进行blast比对。按照下面的条件对每个 homolog 进行过滤：
    ''' 
    ''' + Blast bits score > 60
    ''' + bi-directional hit rate (BHR) > 0.95。
    ''' 
    ''' Blast Bits Score 是在``Blast raw score``换算过来的。
    ''' 
    ''' ``BHR``是KEGG在``Bi-directioanl Best Hit``的基础上进行修改的一个选项，``BHR = Rf * Rr``。
    ''' 
    ''' + Rf和Rr分别为forward sbh以及reverse sbh的``R``值
    ''' 
    ''' Given a genome to be annotated, it is compared against each genome in the reference set of the KEGG GENES database 
    ''' by the homology searches in both forward and reverse directions, taking each gene in genome A as a query compared 
    ''' against all genes in genome B, and vice versa. Those hits with bit scores less than 60 are removed. Because the bit 
    ''' scores of a gene pair a and b from two genomes A and B, respectively, can be different in forward and reverse 
    ''' directions, and because the top scores do not necessarily reflect the order of the rigorous Smith-Waterman scores
    ''' 
    ''' Here, ``R = S'/Sb`` where S' is the bit score of a against b, and Sb is the score of a against the best-hit gene in 
    ''' genome B (which may not necessarily be b). Rf refers to the score from the forward hit (A against B), and Rr refers 
    ''' to the score from the reverse hit (B against A). We select those genes whose BHR is greater than 0.95 in BBH method, 
    ''' and Rf is greater than 0.95 in SBH method.
    ''' 
    ''' KEGG 在做注释的时候，并不是把所有的基因都作为refernce，而是按照是否来自同一个基因组分成一个一个的小的 
    ''' reference，分别进行blast，假设有两个基因组A和B，含有的基因分别为``a1, a2, a3...an``, ``b1, b2, b3...bn``先用A
    ''' 作为query，B作为refer，进行blast比对，A中的基因a1对B中的基因进行遍历，和基因b1有最高的bit score。
    ''' 现在用B作为refer,A作为query,进行blast比对，B中的基因b1对A中的基因进行遍历，如果bits score最高的是a1，
    ''' 则a1和a2就是一个BBH，但也有可能不是a1，只能成为 Single-directional hit rate。
    ''' 
    ''' 用刚才的A和B作为例子。``Rf``为用A作为query，B作为Refer,a1和B中的每一个基因都计算一次，
    ''' 
    ''' ```
    ''' R = Bits_score[a1-b1] / MaxBits_score[a1_b]
    ''' ```
    ''' 
    ''' 分子是a1和B中的一个基因的Bit_score,分母是a1和B中基因最大的bit_score。假设注释得到的a1和b1中的某个基因
    ''' 是``BBH``，则``BHR``一定等于1。当然，容许修改BHR参数``&lt;1``。计算``KO assignment score``后, 
    ''' 选择得分最高的``KO``作为这个``gene``的``KO``。
    ''' </summary>
    ''' <remarks>
    ''' + Moriya, Y., Itoh, M., Okuda, S., Yoshizawa, A., and Kanehisa, M.; KAAS: an automatic genome annotation and pathway reconstruction server. Nucleic Acids Res. 35, W182-W185 (2007). 
    ''' </remarks>
    Public Module BHR

        ''' <summary>
        ''' Calculate R score for hit
        ''' 
        ''' ```
        ''' R = bits / max(bits)
        ''' ```
        ''' </summary>
        ''' <param name="threshold">
        ''' bit score filter threshold, Those hits with bit scores less than 60 are removed.
        ''' </param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Iterator Function HitRate(query As Query, threshold As Double) As IEnumerable(Of NamedValue(Of Double))
            Dim bits = (From hit As SubjectHit
                        In query.SubjectHits
                        Where hit.Score.Score >= threshold
                        Select New NamedValue(Of Double)(hit.Name, hit.Score.Score)).ToArray

            If Not bits.Any Then
                Return
            End If

            Dim maxBits As Double = Aggregate score As NamedValue(Of Double)
                                    In bits
                                    Into Max(score.Value)

            For Each hit As NamedValue(Of Double) In bits
                Yield New NamedValue(Of Double) With {
                    .Name = hit.Name,
                    .Value = hit.Value / maxBits
                }
            Next
        End Function

        ''' <summary>
        ''' Calculate BBH through BHR score
        ''' </summary>
        ''' <param name="query">localblast alignment result of query vs. subj reference</param>
        ''' <param name="refer">localblast alignment result of subj reference vs. query</param>
        ''' <param name="threshold">
        ''' The BHR score threshold. (当这个参数为1的时候,算法会变为传统的BBH构建方法)
        ''' </param>
        ''' <returns></returns>
        ''' <remarks>
        ''' BHR的意义在于, 
        ''' + 假若BBH结果中,A在B中的bit得分并不是最高的那个的话,则按照传统的严格的BBH方法,A将不是B的BBH.(此时将会丢掉一个可能的BBH结果)
        ''' + 但是现在在引入BHR之后,即使A在B中的bit得分不是最高的,但是BHR是最高的,则可以认为A和B此时是BBH(保留下来了一个非常可能存在的BBH结果)
        ''' </remarks>
        Public Iterator Function BHRResult(query As v228, refer As v228, Optional threshold# = 0.95, Optional bitsCutoff As Double = 60) As IEnumerable(Of BiDirectionalBesthit)
            Dim Rf As NamedCollection(Of NamedValue(Of Double))() = query.GetHitRates(bitsCutoff)
            Dim Rr As Dictionary(Of String, NamedCollection(Of NamedValue(Of Double))) = refer _
                .GetHitRates(bitsCutoff) _
                .ReverseAssembly _
                .ToDictionary(Function(hit)
                                  Return hit.name
                              End Function)

            For Each q As NamedCollection(Of NamedValue(Of Double)) In Rf
                Yield q.EvaluateBHR(Rr:=Rr.TryGetValue(q.name), threshold)
            Next
        End Function

        <Extension>
        Private Function EvaluateBHR(Rf As NamedCollection(Of NamedValue(Of Double)), Rr As NamedCollection(Of NamedValue(Of Double)), threshold#) As BiDirectionalBesthit
            If Rr = 0 Then
                Return New BiDirectionalBesthit With {
                    .QueryName = Rf.name,
                    .length = Rf.description,
                    .HitName = HITS_NOT_FOUND,
                    .level = Levels.NA
                }
            End If

            Dim topBHR As Map(Of (q$, r$), Double) = Rr _
                .ToDictionary(Function(hit) hit.Name,
                                Function(hit)
                                    Return hit.Value
                                End Function) _
                .DoCall(Function(scores)
                            Return Rf.GetTopBHR(scores)
                        End Function)

            If topBHR.Maps >= threshold Then
                ' is a BBH
                Return New BiDirectionalBesthit With {
                    .level = If(topBHR.Maps = 1.0, Levels.BBH, Levels.PartialBBH),
                    .QueryName = Rf.name,
                    .length = Rf.description,
                    .HitName = topBHR.Key.r
                }
            Else
                Dim maxR = Rf.OrderByDescending(Function(hit) hit.Value).First

                If maxR.Value >= threshold Then
                    ' is an acceptable sbh hit
                    Return New BiDirectionalBesthit With {
                        .level = Levels.SBH,
                        .QueryName = Rf.name,
                        .length = Rf.description,
                        .HitName = maxR.Name
                    }
                Else
                    ' no hit
                    Return New BiDirectionalBesthit With {
                        .QueryName = Rf.name,
                        .length = Rf.description,
                        .HitName = HITS_NOT_FOUND,
                        .level = Levels.NA
                    }
                End If
            End If
        End Function

        ''' <summary>
        ''' 在这里计算出最高的BHR比对结果
        ''' </summary>
        ''' <param name="q"></param>
        ''' <param name="r"></param>
        ''' <returns></returns>
        <Extension>
        Private Function GetTopBHR(q As NamedCollection(Of NamedValue(Of Double)), r As Dictionary(Of String, Double)) As Map(Of (q$, r$), Double)
            Return q.Where(Function(hit) r.ContainsKey(hit.Name)) _
                .Select(Function(hit)
                            Dim bhr_score = hit.Value * r(hit.Name)
                            Dim align = (q.name, hit.Name)

                            Return New Map(Of (String, String), Double) With {
                                .Key = align,
                                .Maps = bhr_score
                            }
                        End Function) _
                .OrderByDescending(Function(bhr) bhr.Maps) _
                .FirstOrDefault
        End Function

        ''' <summary>
        ''' Reverse assembling of the subj vs. query result.
        ''' </summary>
        ''' <param name="Rr"></param>
        ''' <returns></returns>
        <Extension>
        Private Iterator Function ReverseAssembly(Rr As IEnumerable(Of NamedCollection(Of NamedValue(Of Double)))) As IEnumerable(Of NamedCollection(Of NamedValue(Of Double)))
            Dim queryGroups = Rr _
                .Select(Function(rq)
                            Return rq.Select(Function(hit) (refer:=rq.name, q:=hit))
                        End Function) _
                .IteratesALL _
                .GroupBy(Function(hit) hit.q.Name) _
                .ToArray

            For Each query In queryGroups
                Yield New NamedCollection(Of NamedValue(Of Double)) With {
                    .name = query.Key,
                    .value = query _
                        .Select(Function(hit)
                                    Return New NamedValue(Of Double) With {
                                        .Name = hit.refer,
                                        .Value = hit.q.Value
                                    }
                                End Function) _
                        .ToArray
                }
            Next
        End Function

        <Extension>
        Private Function GetHitRates(align As v228, threshold As Double) As NamedCollection(Of NamedValue(Of Double))()
            Return align.Queries _
                .Select(Function(q)
                            Return New NamedCollection(Of NamedValue(Of Double)) With {
                                .name = q.QueryName,
                                .description = q.QueryLength,
                                .value = q.HitRate(threshold)
                            }
                        End Function) _
                .ToArray
        End Function
    End Module
End Namespace
