Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Platform

    ''' <summary>
    ''' 用于在网页上面显示任务进度的返回值
    ''' </summary>
    Public Structure TaskProgress

        ''' <summary>
        ''' 当前的任务至今阶段的编号
        ''' </summary>
        ''' <returns></returns>
        Public Property current As Integer
        ''' <summary>
        ''' 任务的内容描述
        ''' </summary>
        ''' <returns></returns>
        Public Property progress As String()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace