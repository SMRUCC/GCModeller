
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