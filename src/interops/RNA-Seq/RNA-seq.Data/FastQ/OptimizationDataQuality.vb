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


    ' Code Statistics:

    '   Total Lines: 87
    '    Code Lines: 47 (54.02%)
    ' Comment Lines: 28 (32.18%)
    '    - Xml Docs: 75.00%
    ' 
    '   Blank Lines: 12 (13.79%)
    '     File Size: 3.80 KB


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
                ' 检查序列或质量字符串是否为空
                If String.IsNullOrEmpty(read.SequenceData) OrElse String.IsNullOrEmpty(read.Quality) Then
                    Continue For ' 跳过无效的read
                End If

                ' 检查序列和质量长度是否一致
                If read.SequenceData.Length <> read.Quality.Length Then
                    Continue For ' 跳过数据不一致的read
                End If

                read = read.TrimLowQuality(quality)

                If Not read Is Nothing Then
                    Yield read
                End If
            Next
        End Function

        <Extension>
        Private Function TrimLowQuality(read As FastQ, Optional quality% = 20) As FastQ
            Dim trimStartIndex As Integer = -1
            Dim trimEndIndex As Integer = -1

            ' 1. 从左向右找到第一个质量值 >= quality 的碱基索引
            For i As Integer = 0 To read.Quality.Length - 1
                If FastQ.GetQualityOrder(read.Quality(i)) >= quality Then
                    trimStartIndex = i
                    Exit For ' 找到后立即退出循环
                End If
            Next

            ' 2. 从右向左找到最后一个质量值 >= quality 的碱基索引
            For i As Integer = read.Quality.Length - 1 To 0 Step -1
                If FastQ.GetQualityOrder(read.Quality(i)) >= quality Then
                    trimEndIndex = i
                    Exit For ' 找到后立即退出循环
                End If
            Next

            ' 3. 判断是否找到有效的高质量区域
            ' - trimStartIndex = -1 表示整个序列质量都低于阈值
            ' - trimStartIndex > trimEndIndex 表示没有连续的高质量区域（例如，所有碱基质量都低）
            If trimStartIndex = -1 OrElse trimStartIndex > trimEndIndex Then
                ' 整个序列都是低质量，或者没有可保留的高质量区域，直接丢弃此read
                Return Nothing
            End If

            ' 4. 计算保留区域的长度并执行修剪
            Dim length As Integer = trimEndIndex - trimStartIndex + 1
            read.SequenceData = read.SequenceData.Substring(trimStartIndex, length)
            read.Quality = read.Quality.Substring(trimStartIndex, length)

            Return read
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
            Return reads.Where(Function(r) r.Length >= minLen)
        End Function
    End Module
End Namespace
