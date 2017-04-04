#Region "Microsoft.VisualBasic::57dac52d2b3b0c8fc493ca42824dbf04, ..\visualbasic.DBI\LibMySQL\MYSQL.Client\ConnectionHelper.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

''' <summary>
''' The connection parameter for the MYSQL database server.(MySQL服务器的远程连接参数)
''' </summary>
''' <remarks></remarks>
Public Class ConnectionUri

    Public Const MYSQL_CONNECTION As String = "Database={0}; Data Source={1}; User Id={2}; Password={3}; Port={4}"

    ''' <summary>
    ''' The server IP address, you can using 'localhost' to specific the local machine.(服务器的IP地址，可以使用localhost来指代本机)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Xml.Serialization.XmlAttribute> Public Property ServerIPAddress As String
    ''' <summary>
    ''' The port number of the remote database server.(数据库服务器的端口号)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Xml.Serialization.XmlAttribute> Public Property HostPort As UInteger

    ''' <summary>
    ''' Using &lt;database_name>.(数据库的名称)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Xml.Serialization.XmlAttribute> Public Property Database As String
    <Xml.Serialization.XmlAttribute> Public Property User As String
    <Xml.Serialization.XmlAttribute> Public Property Password As String

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

    Public Overrides Function ToString() As String
        If String.IsNullOrEmpty(Database) Then
            Return String.Format("Data Source={0}; User Id={1}; Password={2}; Port={3}", ServerIPAddress, User, Password, HostPort)
        Else
            Return String.Format(MYSQL_CONNECTION, Database, ServerIPAddress, User, Password, HostPort)
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
        Dim Server As String() =
            Regex.Match(url, SERVERSITE).Value.Split("/").Last.Split(":")
        Dim Profiles As String() = Mid(url.Split("/").Last, 8).Split("%")
        Dim NewConnection As New ConnectionUri With {.ServerIPAddress = Server.First, .HostPort = Val(Server.Last)}
        Dim NewKeyValue As System.Func(Of String, KeyValuePair(Of String, String)) =
            Function(s As String)
                Dim Tokens As String() = s.Split("=")
                Return New KeyValuePair(Of String, String) _
                    (key:=Tokens.First.ToLower, value:=Tokens.Last.ToLower)
            End Function
        Dim DataList As Generic.IEnumerable(Of KeyValuePair(Of String, String)) =
            From s As String
            In Profiles
            Select NewKeyValue(s) 'DataList Query
        DataList = DataList.ToArray

        Dim DictQuery As System.Func(Of String, String) =
            Function(Key As String) As String
                Dim LQuery As Generic.IEnumerable(Of String) _
                    =
                    From s As KeyValuePair(Of String, String)
                    In DataList
                    Where String.Equals(s.Key, Key)
                    Select s.Value 'LQuery Query
                Return LQuery.First
            End Function

        NewConnection.User = DictQuery("user")
        NewConnection.Password = DictQuery("password")
        NewConnection.Database = DictQuery("database")

        Return NewConnection
    End Operator

    Public Shared Widening Operator CType(strElement As Xml.Linq.XElement) As ConnectionUri
        Return CType(strElement.Value, ConnectionUri)
    End Operator

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
        Dim uri As String = String.Format("https://{0}:{1}/client?user={2}%password={3}%database={4}", ServerIPAddress, HostPort, usr, pwd, dbn)
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
        Dim Uri As ConnectionUri = CType(url, ConnectionUri)
        Uri.Database = passwordDecryption(Uri.Database)
        Uri.User = passwordDecryption(Uri.User)
        Uri.Password = passwordDecryption(Uri.Password)
        Return Uri
    End Function
End Class
