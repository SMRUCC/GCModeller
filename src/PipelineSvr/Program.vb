Imports System.Threading

Module Program

#Region "Main - Program entry point"
    ''' <summary>Program entry point.</summary>
    ''' <param name="args">Command Line Arguments</param>
    <STAThread>
    Public Sub Main(args As String())
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim isFirstInstance As Boolean
        ' Please use a unique name for the mutex to prevent conflicts with other programs
        Using mtx As New Mutex(True, "PipelineSvr", isFirstInstance)
            If isFirstInstance Then
                Dim notificationIcon As New NotificationIcon()
                notificationIcon.notifyIcon.Visible = True
                notificationIcon.notifyIcon.ShowBalloonTip(
                    3 * 1000,
                    "GCModeller PipelineSvr",
                    "GCModeller Memory Pipeline Services started!",
                    ToolTipIcon.Info)
                Application.Run()
                notificationIcon.notifyIcon.Dispose()
                ' The application is already running
                ' TODO: Display message box or change focus to existing application instance
            Else
            End If
        End Using
        ' releases the Mutex
    End Sub
#End Region
End Module
