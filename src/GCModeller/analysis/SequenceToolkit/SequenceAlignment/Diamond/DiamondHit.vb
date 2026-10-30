#Region "Microsoft.VisualBasic::81cc02bcf88962732227c53b21008c08, analysis\SequenceToolkit\SequenceAlignment\Diamond\DiamondHit.vb"

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

'   Total Lines: 115
'    Code Lines: 70 (60.87%)
' Comment Lines: 28 (24.35%)
'    - Xml Docs: 78.57%
' 
'   Blank Lines: 17 (14.78%)
'     File Size: 5.18 KB


'     Class DiamondHit
' 
'         Properties: AlignmentLength, BitScore, Evalue, GapOpens, Matches
'                     Mismatches, PercentIdentity, QueryEnd, QueryFragment, QueryStart
'                     QueryTitle, RawScore, SubjectEnd, SubjectFragment, SubjectStart
'                     SubjectTitle
' 
'         Function: FromBandHit, ToString
' 
' 
' /********************************************************************************/

#End Region

' DIAMOND 比对结果模型 (DiamondHit)
'
' 对应 BLAST m8 / tabular 输出的一行,供 GCModeller 内部 API 返回。
' 字段命名与 m8 格式对齐,便于后续对接外部分析流程。

Imports SMRUCC.genomics.Interops.NCBI.Extensions
Imports eval = SMRUCC.genomics.Analysis.SequenceAlignment.EValue

Namespace DIAMOND

    ''' <summary>
    ''' 单条 DIAMOND 比对命中(m8 风格)。
    ''' </summary>
    Public Class DiamondHit : Inherits DiamondAnnotation

        ''' <summary>
        ''' 将 BandHit(0-based 全局坐标)转换为 1-based 的 DiamondHit。
        ''' </summary>
        Public Shared Function FromBandHit(globalQuery As String,
                                           globalSubject As String,
                                           queryTitle As String,
                                           subjectTitle As String,
                                           band As BandHit) As DiamondHit
            Dim qLen = band.QueryEnd - band.QueryStart + 1
            Dim sLen = band.SubjectEnd - band.SubjectStart + 1
            Dim alnLen = Math.Max(qLen, sLen)

            Dim mismatches As Integer = 0
            Dim gaps As Integer = 0
            Dim matches As Integer = 0

            For i As Integer = 0 To alnLen - 1
                Dim qaa = If(i < band.QueryFragment.Length, band.QueryFragment(i), "-"c)
                Dim saa = If(i < band.SubjectFragment.Length, band.SubjectFragment(i), "-"c)

                If qaa = "-"c OrElse saa = "-"c Then
                    gaps += 1
                ElseIf qaa = saa Then
                    matches += 1
                Else
                    mismatches += 1
                End If
            Next

            ' 接入 Karlin-Altschul 统计模型(BLOSUM62 统计量)
            Dim queryLen = globalQuery.Length
            Dim subjectLen = globalSubject.Length
            ' λ=0.267, K=0.041 (BLOSUM62 经验标定,源自 EValue 模块常量)
            Dim bitScore = (0.267 * band.Score - Math.Log(0.041)) / Math.Log(2)
            Dim eValCalc = eval.Compute(band.Score, queryLen, subjectLen)
            Dim hit As New DiamondHit With {
                .QseqId = queryTitle,
                .SseqId = subjectTitle,
                .RawScore = band.Score,
                .BitScore = bitScore,
                .EValue = eValCalc,
                .Length = alnLen,
                .Matches = matches,
                .Mismatch = mismatches,
                .GapOpen = gaps,
                .Pident = If(alnLen > 0, 100.0 * matches / alnLen, 0),
                .QStart = band.QueryStart + 1,
                .QEnd = band.QueryEnd + 1,
                .SStart = band.SubjectStart + 1,
                .SEnd = band.SubjectEnd + 1,
                .QueryFragment = band.QueryFragment,
                .SubjectFragment = band.SubjectFragment
            }

            Return hit
        End Function

        ''' <summary>比对中的一致列数(内部计算用)。</summary>
        Friend Property Matches As Integer

        Public Overrides Function ToString() As String
            Return ToTsv()
        End Function
    End Class
End Namespace

