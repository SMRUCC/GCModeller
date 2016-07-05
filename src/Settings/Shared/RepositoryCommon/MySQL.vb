#Region "Microsoft.VisualBasic::d8204dc5b66d68acad7c2d69f4bea362, ..\Settings\Shared\RepositoryCommon\MySQL.vb"

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

Module MySQLExtensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property MySQL As Oracle.LinuxCompatibility.MySQL.ConnectionUri
        Get
            Return Oracle.LinuxCompatibility.MySQL.ConnectionUri.CreateObject(Settings.Session.SettingsFile.MySQL, AddressOf Settings.Session.SHA256Provider.DecryptString)
        End Get
        Set(value As Oracle.LinuxCompatibility.MySQL.ConnectionUri)
            Settings.Session.SettingsFile.MySQL = value.GenerateUri(AddressOf Settings.Session.SHA256Provider.EncryptData)
        End Set
    End Property

    Sub New()
        Call Settings.Session.Initialize()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uri">来自于外部的可选的连接信息参数</param>
    ''' <returns></returns>
    Public Function GetMySQLClient(uri As Oracle.LinuxCompatibility.MySQL.ConnectionUri,
                                   Optional DBName As String = "gcmodeller",
                                   Optional timeOut As Integer = 5000) As Oracle.LinuxCompatibility.MySQL.MySQL
        If uri Is Nothing Then
            If Not Settings.Session.Initialized Then
                Call Settings.Session.Initialize()
            End If
            uri = MySQLExtensions.MySQL
        End If

        If Not String.IsNullOrEmpty(DBName) Then
            uri.Database = DBName
        End If

        Dim MySQL As New Oracle.LinuxCompatibility.MySQL.MySQL
        Call $"Connect to database server in {MySQL.Connect(uri)}ms...".__DEBUG_ECHO

        Return MySQL
    End Function

    Public Function GetURI(Optional uri As String = "") As Oracle.LinuxCompatibility.MySQL.ConnectionUri
        Dim cnn As Oracle.LinuxCompatibility.MySQL.ConnectionUri

        If String.IsNullOrEmpty(uri) Then
            cnn = MySQL
        Else
            cnn = Oracle.LinuxCompatibility.MySQL.ConnectionUri.CreateObject(uri, AddressOf Settings.SHA256Provider.DecryptString)
        End If

        Return cnn
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uri">来自于外部的可选的连接信息参数</param>
    ''' <returns></returns>
    Public Function GetMySQLClient(uri As String) As Oracle.LinuxCompatibility.MySQL.MySQL
        Return GetMySQLClient(GetURI(uri))
    End Function
End Module

