Imports System.Xml.Serialization

Namespace Runtime.Debugging

    Public Class DebuggerMessage

        <XmlText> Public Property Message As String
        <XmlAttribute> Public Property Pid As Integer
        <XmlAttribute> Public Property MessageType As MessageTypes

        Public Enum MessageTypes

            ''' <summary>
            ''' IDE向Shoal调试程序推送脚本
            ''' </summary>
            ''' <remarks></remarks>
            CTRL_PUSH_SCRIPT
            ''' <summary>
            ''' IDE发送终止脚本调试的信号
            ''' </summary>
            ''' <remarks></remarks>
            CTRL_KILL_SCRIPT
            ''' <summary>
            ''' IDE请求Shoal的变量内容
            ''' </summary>
            ''' <remarks></remarks>
            CTRL_GETS_MEMORY
            ''' <summary>
            ''' IDE修改Shoal内存之中的变量的内容
            ''' </summary>
            ''' <remarks></remarks>
            CTRL_MODIFY_VALUE
            ''' <summary>
            ''' 调试客户端向服务器返回初始化信息
            ''' </summary>
            ''' <remarks></remarks>
            CTRL_DEBUGGER_INIT_INFO

            ''' <summary>
            ''' Shoal向IDE发送一般的消息
            ''' </summary>
            ''' <remarks></remarks>
            OUTPUT_MESSAGE
            ''' <summary>
            ''' Shoal向IDE发送错误消息
            ''' </summary>
            ''' <remarks></remarks>
            OUTPUT_ERROR
            ''' <summary>
            ''' Shoal向IDE发送警告消息
            ''' </summary>
            ''' <remarks></remarks>
            OUTPUT_WARNING
        End Enum
    End Class

End Namespace