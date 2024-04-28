#Region "Microsoft.VisualBasic::a7925f325963cb1f5faeed7f3fed2dac, WebCloud\SMRUCC.WebCloud.Mail\Subscription.vb"

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

    ' Class SubscriptionMgr
    ' 
    '     Properties: AppId, MySQL
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __changeStatus, Active, Add
    ' 
    '     Sub: Remove
    ' 
    ' /********************************************************************************/

#End Region

Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports SMRUCC.WebCloud.DataCenter.mysql

Public Class SubscriptionMgr

    Public ReadOnly Property MySQL As MySqli
    Public ReadOnly Property AppId As Integer

    Sub New(app%, cnn As ConnectionUri)
        MySQL = New MySqli(cnn)
        AppId = app
    End Sub

    ''' <summary>
    ''' 添加订阅用户
    ''' </summary>
    ''' <param name="email$"></param>
    Public Function Add(email$) As String
        Dim user As New subscription With {
            .app = AppId,
            .email = email,
            .hash = $"{AppId}-{LCase(email)}".MD5
        }
        Call MySQL.ExecInsert(user)

        Return user.hash
    End Function

    Public Function Active(hash$) As String
        Return __changeStatus(1, hash)
    End Function

    Private Function __changeStatus(active&, hash$) As String
        Dim SQL$ = String.Format(SQLGetUserByHash, hash)
        Dim user As subscription =
            MySQL.ExecuteScalar(Of subscription)(SQL)

        If Not user Is Nothing Then
            user.active = active
            MySQL.ExecUpdate(user)
            Return user.email
        Else
            Return Nothing
        End If
    End Function

    Const SQLGetUserByHash$ = "SELECT * FROM smrucc_webcloud.subscription where lower(hash) = lower('{0}');"

    Public Sub Remove(hash$)
        Call __changeStatus(0, hash)
    End Sub
End Class
