Imports Microsoft.VisualBasic.ComponentModel.Settings

Namespace Settings.Programs

    Public Class GCHOST

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ProfileItem(Name:=".net_sdk", Description:="")> Public Property SDK As String
        <ProfileItem(Name:="db_root")> Public Property DBRoot As String
        <ProfileItem(Name:="db_root_password")> Public Property DBRootPwd As String

    End Class
End Namespace

