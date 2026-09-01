Namespace Inference

    Public Module Extensions

        ''' <summary>
        ''' 将离散状态映射为数值分值（Low=0, Medium=0.5, High=1），供父节点证据离散化使用。
        ''' </summary>
        Public Function StateToScore(state As String) As Double
            Select Case state
                Case "Low" : Return 0.0
                Case "Medium" : Return 0.5
                Case "High" : Return 1.0
                Case Else : Return 0.5
            End Select
        End Function

        ''' <summary>
        ''' 将离散状态映射为数值，便于以轨迹数组形式输出虚拟敲降模拟结果。
        ''' </summary>
        Public Function StateToValue(state As String) As Double
            Select Case state
                Case "Low" : Return 0.0
                Case "Medium" : Return 1.0
                Case "High" : Return 2.0
                Case Else : Return 1.0
            End Select
        End Function
    End Module
End Namespace