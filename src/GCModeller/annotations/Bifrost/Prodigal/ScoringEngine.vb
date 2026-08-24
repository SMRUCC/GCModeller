#Region "Microsoft.VisualBasic::9130de803eae296d2f1f26c588024578, annotations\Bifrost\Prodigal\ScoringEngine.vb"

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

    '   Total Lines: 42
    '    Code Lines: 15 (35.71%)
    ' Comment Lines: 21 (50.00%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 6 (14.29%)
    '     File Size: 1.70 KB


    ' Class ScoringEngine
    ' 
    '     Sub: ScoreAll, ScoreForSequence
    ' 
    ' /********************************************************************************/

#End Region



' ========================================================================
' 综合打分引擎
' ========================================================================

''' <summary>
''' 综合打分引擎
''' 将编码区得分、RBS得分、起始密码子类型得分、上游序列得分
''' 组合为总得分：score = cscore + sscore
''' 其中 sscore = rscore + tscore + uscore
''' </summary>
Public Class ScoringEngine

    ''' <summary>
    ''' 对所有候选ORF进行打分
    ''' </summary>
    Public Shared Sub ScoreAll(orfs As IEnumerable(Of CandidateORF), model As TrainingModel, fullSequence As String)
        For Each orf As CandidateORF In orfs
            ' 编码区得分
            orf.CodingScore = CodingModel.ComputeCodingScore(orf, model)
            ' RBS得分
            orf.RbsScore = RbsModel.ComputeRbsScore(orf, fullSequence, model)
            ' 起始密码子类型得分
            orf.TypeScore = StartCodonModel.ComputeTypeScore(orf, model)
            ' 上游序列得分
            orf.UpstreamScore = UpstreamModel.ComputeUpstreamScore(orf, fullSequence)
            ' 起始位点得分 = rscore + tscore + uscore
            orf.StartScore = orf.RbsScore + orf.TypeScore + orf.UpstreamScore
            ' 总得分 = cscore + sscore
            orf.TotalScore = orf.CodingScore + orf.StartScore
        Next
    End Sub

    ''' <summary>
    ''' 对单条序列的所有ORF进行打分
    ''' </summary>
    Public Shared Sub ScoreForSequence(orfs As IEnumerable(Of CandidateORF), model As TrainingModel, sequence As String)
        Call ScoreAll(orfs, model, sequence)
    End Sub

End Class

