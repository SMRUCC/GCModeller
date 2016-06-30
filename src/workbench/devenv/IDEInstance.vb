
''' <summary>
''' 本对象是传递进入插件模块之中的用于存储本IDE程序内所有对外开放的对象的容器
''' </summary>
''' <remarks></remarks>
Public Class IDEInstance

    ''' <summary>
    ''' IDE的配置数据文件
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Settings As Settings.Programs.IDE
        Get
            Return Program.Dev2Profile
        End Get
    End Property

    Public ReadOnly Property IDE As FormMain
        Get
            Return Program.IDE
        End Get
    End Property

    Public Sub Out(s As String, Optional Color As ConsoleColor = ConsoleColor.White,
        Optional [Object] As String = Strings.Modeller,
        Optional Type As Microsoft.VisualBasic.Logging.MSG_TYPES = Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
        Call Program.Out(s, Color, [Object], Type)
    End Sub

    Public Sub IDEStatueText(s As String, Optional Color As Drawing.Color = Nothing)
        Call Program.IDEStatueText(s, Color)
    End Sub
End Class
