#Region "Microsoft.VisualBasic::b1e529762aafbe3f08302726d33765ab, RNA-Seq\RNA-seq.Data\FastQ\OptimizationDataQuality.vb"

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

    '     Module OptimizationDataQuality
    ' 
    '         Function: TrimLowQuality, TrimShortReads
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices

Namespace FQ

    ''' <summary>
    ''' 高通量测序中通常会出现一些点突变等测序错误，而且序列末端的质量比较低，
    ''' 为了得到更高质量及更准确的生物信息分析结果，需要对测序原始数据进行
    ''' 优化处理。
    ''' 
    ''' 优化步骤及参数：
    '''
    ''' (1) 将两条序列进行比对，根据比对的末端重叠区进行拼接，拼接时保证至少有20bp的重叠区，去除拼接结果中含有N的序列；
    ''' (2) 去除引物和接头序列，去除两端质量值低于20的碱基，去除长度小于200bp的序列；
    ''' (3) 将上面拼接过滤后的序列与数据库进行比对，去除其中的嵌合体序列（chimera sequence），得到最终的有效数据；
    ''' </summary>
    Public Module OptimizationDataQuality

        ''' <summary>
        ''' 去除两端质量值低于20的碱基
        ''' </summary>
        ''' <param name="reads"></param>
        ''' <param name="quality%"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function TrimLowQuality(reads As IEnumerable(Of FastQ), Optional quality% = 20) As IEnumerable(Of FastQ)
            For Each read As FastQ In reads
debug_loop:     Dim qchrs = read.Quality.ToCharArray
                Dim seq = read.SequenceData
                Dim del_start% = -1
                Dim del_ends% = -1
                Dim qs$ = read.Quality

                For i As Integer = 0 To qchrs.Length / 2 - 1
                    If FastQ.GetQualityOrder(qchrs(i)) < quality Then
                        ' 标记为删除
                        del_start = i
                    End If
                Next
                For i As Integer = qchrs.Length - 1 To qchrs.Length / 2 Step -1
                    If FastQ.GetQualityOrder(qchrs(i)) <= quality Then
                        ' 标记为删除
                        del_ends = i
                    End If
                Next

                ' 无需做任何处理
                If del_start = -1 AndAlso del_ends = -1 Then
                    Yield read
                ElseIf del_start > -1 AndAlso del_ends > -1 Then

                    read.SequenceData = seq.Substring(del_start, del_ends - del_start)
                    read.Quality = qs.Substring(del_start, del_ends - del_start)
                    Yield read

                Else
                    If del_start > -1 Then

                        ' 只有左边的需要去掉低质量序列
                        read.SequenceData = seq.Substring(del_start)
                        read.Quality = qs.Substring(del_start)

                        Yield read
                    Else

                        ' 只有右边的需要去掉低质量序列
                        read.SequenceData = seq.Substring(0, del_ends)
                        read.Quality = qs.Substring(0, del_ends)

                        Yield read
                    End If
                End If
            Next
        End Function

        ''' <summary>
        ''' 删除低质量序列之后，可能有些reads的长度会低于预设的长度阈值<paramref name="minLen"/>，将这些过短的reads都删除掉
        ''' </summary>
        ''' <param name="reads"></param>
        ''' <param name="minLen%"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function TrimShortReads(reads As IEnumerable(Of FastQ), Optional minLen% = 200) As IEnumerable(Of FastQ)
            Return reads.Where(Function(r) r.Length >= 200)
        End Function
    End Module
End Namespace
