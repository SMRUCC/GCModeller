#Region "Microsoft.VisualBasic::4e293d982d116b1860afe08841ce1a53, annotations\Bifrost\Prodigal\UpstreamModel.vb"

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

    '   Total Lines: 53
    '    Code Lines: 30 (56.60%)
    ' Comment Lines: 13 (24.53%)
    '    - Xml Docs: 61.54%
    ' 
    '   Blank Lines: 10 (18.87%)
    '     File Size: 1.93 KB


    ' Class UpstreamModel
    ' 
    '     Function: ComputeUpstreamScore
    ' 
    ' /********************************************************************************/

#End Region


' ========================================================================
' 上游序列评分
' ========================================================================

''' <summary>
''' 上游序列评分（uscore）
''' 评估起始密码子上游的序列特征，如A/T丰富度（典型原核启动子区域）
''' </summary>
Public Class UpstreamModel

    ''' <summary>
    ''' 计算上游序列得分
    ''' 原核生物启动子上游通常有A/T丰富的-10区和-35区
    ''' </summary>
    Public Shared Function ComputeUpstreamScore(orf As CandidateORF, fullSequence As String) As Double
        If String.IsNullOrEmpty(fullSequence) Then Return 0.0

        Dim seq = fullSequence.ToUpper()
        Dim upstreamLen As Integer = 30
        Dim upstreamStart As Integer

        If orf.Strand = "+"c Then
            upstreamStart = Math.Max(0, orf.RawStart - upstreamLen)
            upstreamLen = orf.RawStart - upstreamStart
        Else
            Dim orfEnd = orf.RawEnd
            upstreamStart = Math.Min(seq.Length, orfEnd + 1)
            upstreamLen = Math.Min(30, seq.Length - upstreamStart)
        End If

        If upstreamLen < 5 Then Return 0.0

        Dim upstream As String
        If orf.Strand = "+"c Then
            upstream = seq.Substring(upstreamStart, upstreamLen)
        Else
            upstream = SequenceUtils.ReverseComplement(seq.Substring(upstreamStart, upstreamLen))
        End If

        ' 计算A/T含量（启动子区域通常A/T丰富）
        Dim atCount As Integer = 0
        For Each c In upstream
            If c = "A"c OrElse c = "T"c Then atCount += 1
        Next
        Dim atFreq = CDbl(atCount) / upstreamLen

        ' A/T丰富度得分：AT频率>0.6时给正分
        Dim uscore As Double = (atFreq - 0.5) * 4.0
        Return Math.Max(-2.0, Math.Min(2.0, uscore))
    End Function

End Class

