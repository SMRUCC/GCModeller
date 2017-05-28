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
        Dim readString = Function(s$, ByRef result$)
                             result = s
                             Return Not s.StringEmpty
                         End Function
        Dim readInteger = Function(s$, ByRef i%)
                              i = Val(s)
                              Return i.ToString = s
                          End Function
        Dim database$ = STDIO.Read(Of String)("Please input the database name", readString, "smrucc-webcloud")
        Dim hostName$ = STDIO.Read(Of String)("Please input the host name", readString, "localhost")
        Dim port% = STDIO.Read(Of Integer)("Please input the port", readInteger, 3306)
        Dim user$ = STDIO.Read(Of String)("Enter your user name", readString, "root")
        Dim password$ = STDIO.InputPassword
        Dim mysqli As New ConnectionUri With {
            .Database = database,
            .IPAddress = hostName,
            .Password = password,
            .Port = port,
            .User = user
        }
        Dim confirm = STDIO.MsgBox("MySQLi Manager will update your connection with these information: " & mysqli.GetDisplayUri)

        If confirm = MsgBoxResult.No Then
            Call "User cancel update config...".__INFO_ECHO
        Else
            ' 更新到配置文件
        End If
    End Sub
End Module
