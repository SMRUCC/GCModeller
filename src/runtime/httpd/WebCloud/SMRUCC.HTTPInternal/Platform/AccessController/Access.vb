
Namespace Platform.AccessController

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <typeparam name="T">
    ''' 应该是一个枚举类型
    ''' </typeparam>
    Public Class Access(Of T)

        Public ReadOnly UserGroup As T

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="userGroup"></param>
        Sub New(userGroup As T)
            Me.UserGroup = userGroup
        End Sub

        Public Overrides Function ToString() As String
            Return $"Accessed by ${DirectCast(CObj(UserGroup), [Enum]).Description}"
        End Function
    End Class
End Namespace