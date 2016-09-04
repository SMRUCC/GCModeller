#Region "Microsoft.VisualBasic::c63711bd2e8757606cf2c1fc43059c20, ..\LibMySQL\MYSQL.Client\ConnectionUri.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization

''' <summary>
''' The connection parameter for the MYSQL database server.(MySQL服务器的远程连接参数)
''' </summary>
''' <remarks></remarks>
Public Class ConnectionUri

    ''' <summary>
    ''' ```
    ''' Database={0}; Data Source={1}; User Id={2}; Password={3}; Port={4};
    ''' ```
    ''' </summary>
    Public Const MYSQL_CONNECTION As String = "Database={0}; Data Source={1}; User Id={2}; Password={3}; Port={4};"

    ''' <summary>
    ''' The server IP address, you can using ``localhost`` to specific the local machine.(服务器的IP地址，可以使用localhost来指代本机)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property IPAddress As String
    ''' <summary>
    ''' The port number of the remote database server.(数据库服务器的端口号)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property ServicesPort As UInteger

    ''' <summary>
    ''' ``Using &lt;database_name>``.(数据库的名称)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <XmlAttribute> Public Property Database As String
    <XmlAttribute> Public Property User As String
    <XmlAttribute> Public Property Password As String
    <XmlAttribute> Public Property TimeOut As Integer = -1

    Sub New()
    End Sub

    ''' <summary>
    ''' 复制值
    ''' </summary>
    ''' <param name="o"></param>
    Sub New(o As ConnectionUri)
        Me.Database = o.Database
        Me.IPAddress = o.IPAddress
        Me.Password = o.Password
        Me.ServicesPort = o.ServicesPort
        Me.TimeOut = o.TimeOut
        Me.User = o.User
    End Sub

    ''' <summary>
    ''' ``Database={0}; Data Source={1}; User Id={2}; Password={3}; Port={4}``
    ''' </summary>
    ''' <param name="cnn"></param>
    ''' <returns></returns>
    Public Shared Function MySQLParser(cnn As String) As ConnectionUri
        Dim Uri As New ConnectionUri

        Dim Database = Regex.Match(cnn, "Database=\S+", RegexOptions.IgnoreCase).Value
        Dim User = Regex.Match(cnn, "User Id=\S+", RegexOptions.IgnoreCase).Value
        Dim Password = Regex.Match(cnn, "Password=\S+", RegexOptions.IgnoreCase).Value
        Dim IPAddress = Regex.Match(cnn, "Data Source=\S+", RegexOptions.IgnoreCase).Value
        Dim ServicesPort = Regex.Match(cnn, "Port=\d+", RegexOptions.IgnoreCase).Value

        If Not String.IsNullOrEmpty(ServicesPort) Then
            Uri.ServicesPort = CUInt(Val(ServicesPort.Split("="c).Last))
        Else
            Uri.ServicesPort = 3306
        End If

        User = Regex.Replace(User, "User Id=", "", RegexOptions.IgnoreCase)
        Database = Regex.Replace(Database, "Database=", "", RegexOptions.IgnoreCase)
        Password = Regex.Replace(Password, "Password=", "", RegexOptions.IgnoreCase)
        IPAddress = Regex.Replace(IPAddress, "Data Source=", "", RegexOptions.IgnoreCase)

        Uri.User = TrimSeperator(User)
        Uri.Database = TrimSeperator(Database)
        Uri.Password = TrimSeperator(Password)
        Uri.IPAddress = TrimSeperator(IPAddress)

        Return Uri
    End Function

    Private Shared Function TrimSeperator(str As String) As String
        If String.IsNullOrEmpty(str) Then
            Return ""
        Else
            If str.Last = ";"c Then
                str = Mid(str, 1, Len(str) - 1)
            End If
        End If

        Return str
    End Function

    ''' <summary>
    ''' Get a connection string for the connection establish of a client to a mysql database 
    ''' server using the specific paramenter that was assigned by the user.
    ''' (获取一个由用户指定连接参数的用于建立客户端和MySql数据库服务器之间的连接的连接字符串)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetConnectionString() As String
        Return Me.ToString
    End Function

    Public Function GetDisplayUri() As String
        Return $"https://{Me.IPAddress}:{Me.ServicesPort}/mysql/db_{Me.Database}/user_{Me.User}"
    End Function

    Public Overrides Function ToString() As String
        Dim cnn As String = __basicllyConfig()

        If TimeOut >= 0 Then
            cnn &= $" default command timeout={TimeOut};"
        End If

        Return cnn
    End Function

    Private Function __basicllyConfig() As String
        If String.IsNullOrEmpty(Database) Then
            Return $"Data Source={IPAddress}; User Id={User}; Password={Password}; Port={ServicesPort};"
        Else
            Return String.Format(MYSQL_CONNECTION, Database, IPAddress, User, Password, ServicesPort)
        End If
    End Function

    ''' <summary>
    ''' Create a mysql connection using the connection uri
    ''' </summary>
    ''' <param name="url"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateObject(url As String) As MySQL
        Dim Connection As ConnectionUri = CType(url, ConnectionUri)
        Return CType(Connection, MySQL)
    End Function

    ''' <summary>
    ''' Conver the ConnectionHelper object to a mysql connection string using 
    ''' the specific parameter which assigned by the user.
    ''' (将使用由用户指定连接参数的连接生成器转换为Mysql数据库的连接字符串)
    ''' </summary>
    ''' <param name="uri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Narrowing Operator CType(uri As ConnectionUri) As String
        Return uri.GetConnectionString
    End Operator

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="url">MySql connection string.(MySql连接字符串)</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Example: 
    ''' http://localhost:8080/client?user=username%password=password%database=database
    ''' </remarks>
    Public Shared Widening Operator CType(url As String) As ConnectionUri
        Return TryParsing(url)
    End Operator

    ''' <summary>
    ''' 函数会自动解析MySQL格式或者uri拓展格式
    ''' </summary>
    ''' <param name="uri">MySQL连接字符串或者uri拓展格式</param>
    ''' <returns></returns>
    Public Shared Function TryParsing(uri As String) As ConnectionUri
        If Not InStr(uri, "https://", CompareMethod.Text) > 0 Then
            Return ConnectionUri.MySQLParser(uri)
        Else
            Return ConnectionUri.UriParser(uri)
        End If
    End Function

    Private Shared Function UriParser(uri As String) As ConnectionUri
        Dim Temp As String = Regex.Match(uri, SERVERSITE).Value
        Call Debug.Write(Temp)
        Dim Server As String() = Temp.Split(CChar("/")).Last.Split(CChar(":"))
        Call Debug.Write(String.Join(";", Server))
        Dim Profiles As String() = Mid(uri, InStr(uri, "client?", CompareMethod.Text) + 7).Split(CChar("%"))
        Call Debug.Write(String.Join("; ", Profiles))
        Dim newURI As New ConnectionUri With {
            .IPAddress = Server.First,
            .ServicesPort = CInt(Val(Server.Last))
        }
        Dim fieldHash As Dictionary(Of String, String) = (From s As String
                                                          In Profiles
                                                          Select __getValue(s)).ToDictionary
#If debug Then
          For Each item In fieldHash
            Call Console.WriteLine(item.ToString)
        Next
#End If
        newURI.User = fieldHash.TryGetValue("user")
        newURI.Password = fieldHash.TryGetValue("password")
        newURI.Database = fieldHash.TryGetValue("database")

        Return newURI
    End Function

    Private Shared Function __getValue(s As String) As KeyValuePair(Of String, String)
        Dim p As Integer = InStr(s, "=")
        Dim key As String = Mid(s, 1, p - 1)
        Dim value As String = Mid(s, p + 1)
        Return New KeyValuePair(Of String, String)(key.ToLower, value)
    End Function

    Public Shared Widening Operator CType(strElement As Xml.Linq.XElement) As ConnectionUri
        Return CType(strElement.Value, ConnectionUri)
    End Operator

#Region "假若需要将连接参数的配置数据保存至文件之中的话，则可以使用这两个方法来完成"

    ''' <summary>
    ''' 重新生成链接url字符串
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="passwordEncryption">用户自定义的密码加密信息</param>
    ''' <remarks></remarks>
    Public Function GenerateUri(passwordEncryption As Func(Of String, String)) As String
        Dim usr As String = passwordEncryption(Me.User)
        Dim pwd As String = passwordEncryption(Password)
        Dim dbn As String = passwordEncryption(Database)
        Dim uri As String = $"https://{IPAddress}:{ServicesPort}/client?user={usr}%password={pwd}%database={dbn}"
        Return uri
    End Function

    ''' <summary>
    ''' 从配置数据之中加载数据库的连接信息
    ''' </summary>
    ''' <param name="url"></param>
    ''' <param name="passwordDecryption"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function CreateObject(url As String, passwordDecryption As Func(Of String, String)) As ConnectionUri
        Dim URI As ConnectionUri = ConnectionUri.TryParsing(url)
        URI.Database = passwordDecryption(URI.Database)
        URI.User = passwordDecryption(URI.User)
        URI.Password = passwordDecryption(URI.Password)

        Call Debug.WriteLine(URI.GetConnectionString)

        Return URI
    End Function
#End Region
End Class

