#Region "Microsoft.VisualBasic::ac8e436f58f6138bf0663159a57df02d, annotations\Bifrost\Prodigal\Models\PredictedGene.vb"

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

    '   Total Lines: 124
    '    Code Lines: 47 (37.90%)
    ' Comment Lines: 37 (29.84%)
    '    - Xml Docs: 89.19%
    ' 
    '   Blank Lines: 40 (32.26%)
    '     File Size: 4.18 KB


    ' Class CandidateORF
    ' 
    '     Properties: [End], AaSequence, CodingScore, DpScore, Frame
    '                 Length, NtSequence, PartialType, PrevIndex, RawEnd
    '                 RawStart, RbsMotif, RbsScore, RbsSpacing, Selected
    '                 SeqId, SortIndex, Start, StartCodon, StartScore
    '                 StopCodon, Strand, TotalScore, TypeScore, UpstreamScore
    ' 
    ' Class PredictedGene
    ' 
    '     Properties: Confidence, GeneIndex
    ' 
    '     Function: CreateGeneFasta, CreateProteinFasta, FastaTitle
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' ============================================================================
' ProdigalModels.vb - Prodigal VB.NET 基因预测程序 数据模型定义
' 基于 Prodigal (PROkaryotic DYnamic Programming Gene-finding ALgorithm) 算法
' ============================================================================

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 候选ORF（开放阅读框）
''' </summary>
Public Class CandidateORF
    ''' <summary>所属序列ID</summary>
    Public Property SeqId As String

    ''' <summary>起始位置（1-based，基因组坐标）</summary>
    Public Property Start As Integer

    ''' <summary>终止位置（1-based，基因组坐标）</summary>
    Public Property [End] As Integer

    ''' <summary>链方向：'+' 正向，'-' 反向</summary>
    Public Property Strand As Char

    ''' <summary>阅读框编号（0, 1, 2）</summary>
    Public Property Frame As Integer

    ''' <summary>起始密码子（ATG/GTG/TTG）</summary>
    Public Property StartCodon As String

    ''' <summary>终止密码子（TAA/TAG/TGA）</summary>
    Public Property StopCodon As String

    ''' <summary>ORF长度（核苷酸数）</summary>
    Public Property Length As Integer

    ''' <summary>编码区得分（coding score）</summary>
    Public Property CodingScore As Double

    ''' <summary>起始位点得分（start score）</summary>
    Public Property StartScore As Double

    ''' <summary>RBS得分</summary>
    Public Property RbsScore As Double

    ''' <summary>上游序列得分</summary>
    Public Property UpstreamScore As Double

    ''' <summary>起始密码子类型得分</summary>
    Public Property TypeScore As Double

    ''' <summary>总得分 = CodingScore + StartScore</summary>
    Public Property TotalScore As Double

    ''' <summary>氨基酸序列</summary>
    Public Property AaSequence As String

    ''' <summary>核苷酸序列</summary>
    Public Property NtSequence As String

    ''' <summary>检测到的RBS模体</summary>
    Public Property RbsMotif As String

    ''' <summary>RBS与起始密码子的间距</summary>
    Public Property RbsSpacing As Integer

    ''' <summary>是否被DP选中</summary>
    Public Property Selected As Boolean

    ''' <summary>在原始序列上的起始位置（0-based，用于ORF查找内部）</summary>
    Public Property RawStart As Integer

    ''' <summary>在原始序列上的终止位置（0-based，用于ORF查找内部）</summary>
    Public Property RawEnd As Integer

    ''' <summary>ORF在排序后的索引（DP用）</summary>
    Public Property SortIndex As Integer

    ''' <summary>DP前驱索引</summary>
    Public Property PrevIndex As Integer = -1

    ''' <summary>DP累积得分</summary>
    Public Property DpScore As Double = Double.MinValue
    ''' <summary>部分基因标记（5'端或3'端截断）</summary>
    Public Property PartialType As String = ""
End Class

''' <summary>
''' 预测基因（从DP选出的最终结果）
''' </summary>
Public Class PredictedGene : Inherits CandidateORF

    ''' <summary>置信度</summary>
    Public Property Confidence As Double

    ''' <summary>基因编号（在序列内的顺序号）</summary>
    Public Property GeneIndex As Integer

    Private Function FastaTitle(seq_id As String) As String
        Dim loc As New NucleotideLocation(Start, [End], Strand.GetStrands)
        Dim title As String = $"{seq_id}_{GeneIndex} {loc.ToString} ID=gene_{GeneIndex};partial={PartialType}"

        Return title
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateProteinFasta(seq_id As String) As FastaSeq
        Return New FastaSeq(AaSequence, title:=FastaTitle(seq_id))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateGeneFasta(seq_id As String) As FastaSeq
        Return New FastaSeq(NtSequence, title:=FastaTitle(seq_id))
    End Function

End Class








