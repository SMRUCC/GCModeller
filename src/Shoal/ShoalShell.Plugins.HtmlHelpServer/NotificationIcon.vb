'
' Created by SharpDevelop.
' User: WORKGROUP
' Date: 2015/1/2
' Time: 20:22
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports System.Threading
Imports System.Windows.Forms
Imports System.Drawing

Public NotInheritable Class NotificationIcon
    Private notifyIcon As NotifyIcon
    Private notificationMenu As ContextMenu

    Dim _InternalServerThreading As Thread

#Region "Initialize icon and menu"
    Public Sub New()
        notifyIcon = New NotifyIcon()
        notificationMenu = New ContextMenu(InitializeMenu())

        AddHandler notifyIcon.DoubleClick, AddressOf InternalShowAboutInfo
        Dim resources As New System.ComponentModel.ComponentResourceManager(GetType(NotificationIcon))
        notifyIcon.Icon = DirectCast(resources.GetObject("$this.Icon"), Icon)
        notifyIcon.ContextMenu = notificationMenu
    End Sub

    Private Function InitializeMenu() As MenuItem()
        Dim menuBuilder As MenuItem() = New MenuItem() {
            New MenuItem("Open Home Page", Sub() Call Process.Start("http://127.0.0.1:8080")),
            New MenuItem("About", AddressOf InternalShowAboutInfo),
            New MenuItem("Exit", AddressOf menuExitClick)
        }
        Return menuBuilder
    End Function
#End Region

#Region "Main - Program entry point"
    ''' <summary>Program entry point.</summary>
    ''' <param name="args">Command Line Arguments</param>
    <STAThread> _
    Public Shared Sub Main(args As String())
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Call HelpServer.BuildDoc()

        Dim isFirstInstance As Boolean
        ' Please use a unique name for the mutex to prevent conflicts with other programs
        Using mtx As New Mutex(True, "Shoal-Http-Server", isFirstInstance)
            If isFirstInstance Then
                Dim notificationIcon As New NotificationIcon()
                notificationIcon.notifyIcon.Visible = True
                notificationIcon.notifyIcon.ShowBalloonTip(30, "Http help server", "ShoalShell Http help server is running at 127.0.0.1:8080!", ToolTipIcon.Info)
                notificationIcon._InternalServerThreading = HelpServer.__runServer()
                Application.Run()
                notificationIcon.notifyIcon.Dispose()
                notificationIcon._InternalServerThreading.Abort()
                Call Console.WriteLine("Server thread shutdown...")
                ' The application is already running
                ' TODO: Display message box or change focus to existing application instance
            Else
            End If
        End Using
        ' releases the Mutex
    End Sub
#End Region

#Region "Event Handlers"
    Private Sub InternalShowAboutInfo(sender As Object, e As EventArgs)
        MessageBox.Show("ShoalShell Http help system local server Application plugin. ShoalShell version " & My.Application.Info.Version.ToString,
                        "ShoalShell Http Help Local Server",
                        buttons:=MessageBoxButtons.OK,
                        icon:=MessageBoxIcon.Information)
    End Sub

    Private Sub menuExitClick(sender As Object, e As EventArgs)
        Application.[Exit]()
    End Sub
#End Region
End Class
