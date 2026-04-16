#Region "Microsoft.VisualBasic::0b15650095754b6720019afe751ac933, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\AnnotationResult.vb"

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

    '   Total Lines: 57
    '    Code Lines: 15 (26.32%)
    ' Comment Lines: 30 (52.63%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (21.05%)
    '     File Size: 1.31 KB


    ' Class AnnotationResult
    ' 
    '     Properties: AlignedSequence, AlignmentEnd, AlignmentStart, BitScore, Confidence
    '                 EValue, FunctionalAnnotation, IsSignificant, ModelName, SeqId
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

''' <summary>
''' 注释结果类
''' </summary>
Public Class AnnotationResult

    Public Property SeqId As String

    ''' <summary>
    ''' 匹配的模型名称
    ''' </summary>
    Public Property ModelName As String

    ''' <summary>
    ''' 比特得分
    ''' </summary>
    Public Property BitScore As Double

    ''' <summary>
    ''' E值
    ''' </summary>
    Public Property EValue As Double

    ''' <summary>
    ''' 是否通过阈值
    ''' </summary>
    Public Property IsSignificant As Boolean

    ''' <summary>
    ''' 匹配的起始位置
    ''' </summary>
    Public Property AlignmentStart As Integer

    ''' <summary>
    ''' 匹配的结束位置
    ''' </summary>
    Public Property AlignmentEnd As Integer

    ''' <summary>
    ''' 匹配区域序列
    ''' </summary>
    Public Property AlignedSequence As String

    ''' <summary>
    ''' 置信度
    ''' </summary>
    Public Property Confidence As Double

    ''' <summary>
    ''' 功能注释
    ''' </summary>
    Public Property FunctionalAnnotation As String

    Public Overrides Function ToString() As String
        Return $"Model: {ModelName}, Score: {BitScore:F2}, E-value: {EValue:G2}, Significant: {IsSignificant}"
    End Function

End Class

