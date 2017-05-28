Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Terminal
Imports Oracle.LinuxCompatibility.MySQL
Imports mysqliEnd = Oracle.LinuxCompatibility.MySQL.MySQL

''' <summary>
''' 一些比较常用的mysql连接拓展
''' </summary>
<RunDllEntryPoint(NameOf(mysqli))> Public Module mysqli

    ''' <summary>
    ''' Initializes MySQLi and returns a resource for use with <see cref="mysqliEnd"/>
    ''' (从命令行所设置的环境变量之中初始化mysql的数据库连接的通用拓展)
    ''' </summary>
    ''' <param name="mysql"></param>
    <Extension> Public Sub init_cli(ByRef mysql As mysqliEnd)
        If mysql <= New ConnectionUri With {
            .Database = App.GetVariable("database"),
            .IPAddress = App.GetVariable("host"),
            .Password = App.GetVariable("password"),
            .Port = App.GetVariable("port"),
            .User = App.GetVariable("user")
        } = -1.0R Then

#If Not DEBUG Then
            Throw New Exception("No MySQL database connection!")
#Else
            Call "No mysqli database connection!".Warning
#End If
        End If
    End Sub

    Public Sub RunConfig()
        Dim database$ = STDIO.Read(Of String)(
            "Please input the database name:",
            Function(s$, ByRef result$)
                result = s
                Return Not s.StringEmpty
            End Function, "smrucc-webcloud")

    End Sub
End Module
