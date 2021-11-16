#Region "Microsoft.VisualBasic::2f315f93049d9483d3b2ff7ffabdac41, Shared\RepositoryCommon\MySQL.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    ' 
    ' /********************************************************************************/

#End Region

'#Region "Microsoft.VisualBasic::3cc73fcbda9052e4bbddd915e8db054a, Shared\RepositoryCommon\MySQL.vb"

'    ' Author:
'    ' 
'    '       asuka (amethyst.asuka@gcmodeller.org)
'    '       xie (genetics@smrucc.org)
'    '       xieguigang (xie.guigang@live.com)
'    ' 
'    ' Copyright (c) 2018 GPL3 Licensed
'    ' 
'    ' 
'    ' GNU GENERAL PUBLIC LICENSE (GPL3)
'    ' 
'    ' 
'    ' This program is free software: you can redistribute it and/or modify
'    ' it under the terms of the GNU General Public License as published by
'    ' the Free Software Foundation, either version 3 of the License, or
'    ' (at your option) any later version.
'    ' 
'    ' This program is distributed in the hope that it will be useful,
'    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
'    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    ' GNU General Public License for more details.
'    ' 
'    ' You should have received a copy of the GNU General Public License
'    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



'    ' /********************************************************************************/

'    ' Summaries:

'    ' Module MySQLExtensions
'    ' 
'    '     Properties: MySQL
'    ' 
'    '     Constructor: (+1 Overloads) Sub New
'    '     Function: (+2 Overloads) GetMySQLClient, GetURI
'    ' 
'    ' /********************************************************************************/

'#End Region

'Imports Microsoft.VisualBasic.Language
'Imports Oracle.LinuxCompatibility.MySQL.Uri
'Imports MySql = Oracle.LinuxCompatibility.MySQL.MySqli

'''' <summary>
'''' The mysqli extension helper for the GCModeller repository system.
'''' </summary>
'Module MySQLExtensions

'    ''' <summary>
'    ''' Mysqli connection information from configuration file.
'    ''' </summary>
'    ''' <returns></returns>
'    Public Property MySQL As ConnectionUri
'        Get
'            Return ConnectionUri.CreateObject(Settings.Session.SettingsFile.MySQL, AddressOf Settings.Session.SHA256Provider.DecryptString)
'        End Get
'        Set(value As ConnectionUri)
'            Settings.Session.SettingsFile.MySQL = value.GenerateUri(AddressOf Settings.Session.SHA256Provider.EncryptData)
'        End Set
'    End Property

'    Sub New()
'        If Not Settings.Session.Initialized Then
'            Call Settings.Session.Initialize()
'        End If
'    End Sub

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="uri">
'    ''' Mysqli connection information from external.(来自于外部的可选的连接信息参数)
'    ''' </param>
'    ''' <returns></returns>
'    Public Function GetMySQLClient(Optional uri As ConnectionUri = Nothing,
'                                   Optional DBName As String = "gcmodeller",
'                                   Optional timeOut As Integer = 5000) As MySql

'        With uri Or MySQLExtensions.MySQL.AsDefault

'            Dim mysqli As New MySql
'            Dim ping As New Value(Of Double)

'            If Not String.IsNullOrEmpty(DBName) Then
'                .Database = DBName
'            End If

'            If (ping = (mysqli <= .ByRef)) = -1.0R Then
'                Throw New NullReferenceException("No mysql connection!")
'            Else
'                Call $"Ping to mysql server in {ping}ms...".__DEBUG_ECHO
'            End If

'            Return mysqli
'        End With
'    End Function

'    ''' <summary>
'    ''' Parsing the mysqli connection information from an encrypted url
'    ''' </summary>
'    ''' <param name="uri"></param>
'    ''' <returns></returns>
'    Public Function GetURI(Optional uri As String = "") As ConnectionUri
'        Dim cnn As ConnectionUri

'        If String.IsNullOrEmpty(uri) Then
'            cnn = MySQL
'        Else
'            cnn = ConnectionUri.CreateObject(uri, AddressOf Settings.SHA256Provider.DecryptString)
'        End If

'        Return cnn
'    End Function

'    ''' <summary>
'    ''' 
'    ''' </summary>
'    ''' <param name="uri">来自于外部的可选的连接信息参数</param>
'    ''' <returns></returns>
'    Public Function GetMySQLClient(uri As String) As MySql
'        Return GetMySQLClient(GetURI(uri))
'    End Function
'End Module

