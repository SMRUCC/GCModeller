#Region "Microsoft.VisualBasic::29582efb7827bef6634fc72226a7a46d, annotations\Bifrost\Prodigal\StartCodonModel.vb"

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

    '   Total Lines: 61
    '    Code Lines: 35 (57.38%)
    ' Comment Lines: 19 (31.15%)
    '    - Xml Docs: 63.16%
    ' 
    '   Blank Lines: 7 (11.48%)
    '     File Size: 2.22 KB


    ' Class StartCodonModel
    ' 
    '     Function: ComputeTypeScore
    ' 
    '     Sub: BuildModel
    ' 
    ' /********************************************************************************/

#End Region

' ========================================================================
' 起始密码子评分模型
' ========================================================================

''' <summary>
''' 起始密码子评分模型
''' 统计不同起始密码子（ATG/GTG/TTG）的使用频率，
''' 计算起始密码子类型得分（tscore）
''' </summary>
Public Class StartCodonModel

    ''' <summary>
    ''' 从训练基因构建起始密码子频率模型
    ''' </summary>
    Public Shared Sub BuildModel(model As TrainingModel, trainingOrfs As List(Of CandidateORF))
        Dim counts As New Dictionary(Of String, Integer) From {
            {"ATG", 0}, {"GTG", 0}, {"TTG", 0}
        }
        Dim total As Integer = 0

        For Each orf In trainingOrfs
            Dim codon = orf.StartCodon.ToUpper()
            If counts.ContainsKey(codon) Then
                counts(codon) += 1
                total += 1
            End If
        Next

        If total > 0 Then
            For Each kv In counts
                model.StartCodonFreq(kv.Key) = CDbl(kv.Value) / total
            Next
        End If
    End Sub

    ''' <summary>
    ''' 计算起始密码子类型得分（tscore）
    ''' ATG通常最常见，得分最高；GTG次之；TTG最低
    ''' </summary>
    Public Shared Function ComputeTypeScore(orf As CandidateORF, model As TrainingModel) As Double
        Dim codon = orf.StartCodon.ToUpper()
        If model.StartCodonFreq.ContainsKey(codon) Then
            Dim freq = model.StartCodonFreq(codon)
            ' 使用对数频率作为得分，ATG约0.75 → log2(0.75) ≈ -0.42
            ' 但我们需要正向得分，所以使用 freq * 加权系数
            ' Prodigal中ATG约得2.5分，GTG约1.2分，TTG约0.5分
            If freq > 0 Then
                Return Math.Log(freq * 10, 2) * 2.0
            End If
        End If

        ' 默认得分
        Select Case codon
            Case "ATG" : Return 2.5
            Case "GTG" : Return 1.2
            Case "TTG" : Return 0.5
            Case Else : Return -1.0  ' 非标准起始密码子
        End Select
    End Function

End Class

