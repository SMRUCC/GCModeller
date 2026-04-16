#Region "Microsoft.VisualBasic::2a5ec8a08e9ab25b316342cdeb63f205, analysis\SequenceToolkit\Pfam-HMM\PfamHMMScan\HMM.vb"

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

    '   Total Lines: 55
    '    Code Lines: 34 (61.82%)
    ' Comment Lines: 15 (27.27%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 6 (10.91%)
    '     File Size: 1.21 KB


    ' Class BitScoreResult
    ' 
    '     Properties: AlignmentPath, Score
    ' 
    ' Class AlignmentPosition
    ' 
    '     Properties: ModelPosition, SequencePosition, State
    ' 
    ' Structure TracePointer
    ' 
    '     Properties: ModelPos, SeqPos, State
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' Enum TraceState
    ' 
    '     DELETE, INSERT, MATCH, NONE
    ' 
    '  
    ' 
    ' 
    ' 
    ' Enum TransitionType
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' 比特得分结果
''' </summary>
Public Class BitScoreResult
    Public Property Score As Double
    Public Property AlignmentPath As List(Of AlignmentPosition)
End Class

''' <summary>
''' 比对位置信息
''' </summary>
Public Class AlignmentPosition
    Public Property ModelPosition As Integer
    Public Property SequencePosition As Integer
    Public Property State As TraceState
End Class

''' <summary>
''' 回溯指针
''' </summary>
Public Structure TracePointer
    Public Property State As TraceState
    Public Property ModelPos As Integer
    Public Property SeqPos As Integer

    Public Sub New(state As TraceState, modelPos As Integer, seqPos As Integer)
        Me.State = state
        Me.ModelPos = modelPos
        Me.SeqPos = seqPos
    End Sub
End Structure

''' <summary>
''' 状态类型
''' </summary>
Public Enum TraceState
    NONE
    MATCH
    INSERT
    DELETE
End Enum

''' <summary>
''' 转移类型
''' </summary>
Public Enum TransitionType
    M_TO_M = 0 ' m->m
    M_TO_I = 1 ' m->i
    M_TO_D = 2 ' m->d
    I_TO_M = 3 ' i->m
    I_TO_I = 4 ' i->i
    D_TO_M = 5 ' d->m
    D_TO_D = 6 ' d->d
End Enum
