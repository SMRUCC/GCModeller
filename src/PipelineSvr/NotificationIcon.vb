'
' Created by SharpDevelop.
' User: ThinkPad
' Date: 1/21/2016
' Time: 4:31 PM
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Threading

Friend NotInheritable Class NotificationIcon

    Public ReadOnly Property notifyIcon As NotifyIcon
    Public ReadOnly Property notificationMenu As ContextMenu

#Region "Initialize icon and menu"
    Public Sub New()
        notifyIcon = New NotifyIcon()
        notificationMenu = New ContextMenu(InitializeMenu())

        AddHandler notifyIcon.DoubleClick, AddressOf IconDoubleClick
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(NotificationIcon))
        notifyIcon.Icon = DirectCast(resources.GetObject("$this.Icon"), Icon)
        notifyIcon.ContextMenu = notificationMenu
    End Sub

    Private Function InitializeMenu() As MenuItem()
        Dim menu As MenuItem() = New MenuItem() {New MenuItem("About", AddressOf menuAboutClick), New MenuItem("Exit", AddressOf menuExitClick)}
        Return menu
    End Function
#End Region

#Region "Event Handlers"

    Const Msg As String =
        "This is a program services for running the cli programming in the GCModeller to pipe streaming the data between the tools in the GCModeller, if your analysis is complete and data is no needs in the memory, then you can exit this program."

    Private Sub menuAboutClick(sender As Object, e As EventArgs)
        Call MsgBox(Msg, MsgBoxStyle.Information, "About This Application")
    End Sub

    Private Sub menuExitClick(sender As Object, e As EventArgs)
        Application.[Exit]()
    End Sub

    Private Sub IconDoubleClick(sender As Object, e As EventArgs)
    End Sub
#End Region
End Class
