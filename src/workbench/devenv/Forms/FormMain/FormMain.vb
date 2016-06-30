Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Interaction

Public NotInheritable Class FormMain : Inherits Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Form

    Dim WithEvents Caption As MolkPlusTheme.Windows.Forms.Controls.Caption

    Dim PlugInsManager As PlugIn.PlugInManager

    Dim WithEvents LabelButtonConsole As LabelButton = New LabelButton With {.Text = "Output"}

    Public Property TabpageProxy As MultipleTabpageProxy

    Public ReadOnly Property IDEInstance As IDEInstance
        Get
            Return Program.IDEInstance
        End Get
    End Property

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False

        ToolStripManager.Renderer = New Microsoft.VisualBasic.MolkPlusTheme.Office2007Renderer()

        Me.MultipleTabpagePanel1.Renderer = MolkPlusTheme.Visualise.Elements.MultipleTabPage.MolkPlusTheme
        Me.MultipleTabpagePanel1.DisabledCloseControl = False

        Size = New Size(Program.Dev2Profile.IDE.Size.Width, Program.Dev2Profile.IDE.Size.Height)
        Location = New Point(Program.Dev2Profile.IDE.Location.Left, Program.Dev2Profile.IDE.Location.Top)

        Caption = New MolkPlusTheme.Windows.Forms.Controls.Caption
        Controls.Add(Caption)

        Caption.Text = "GCModeller"
        Caption.SubCaption = "Project Home"
        Caption.Icon = My.Resources.IDE_ICON
        Caption.Update()
        Caption.ContextMenuStrip = Me.ControlboxMenu

        Call InitializeMenuColor()   'Initialize menu color
        Call InitializeHandlers()

        TabpageProxy = Me

        Call Me.Controls.Add(LabelButtonConsole)
        Call LabelButtonConsole.BringToFront()

        Call Out("Show IDE main.")

        Program.IDEStatueText("Loading IDE plugins...")
        PlugInsManager = PlugIn.PlugInManager.LoadPlugins(Me.MenuStrip1, "./plugins", "./Settings/PlugInsManager.xml")  'Load IDE PlugIns

        Call Microsoft.VisualBasic.ShellScript.Dynamics.IDE_PlugIns.IDEPlugIn.LoadScripts(Me.ToolsToolStripMenuItem, "./plugins/shellscripts/")

        Program.IDEStatueText("Ready")
    End Sub

    Private Sub InitializeMenuColor()
        Dim Color As System.Drawing.Color = Drawing.Color.FromArgb(234, 240, 255)
        Dim Item As ToolStripMenuItem

        For i As Integer = 0 To MenuStrip1.Items.Count - 1
            Item = MenuStrip1.Items(i)
            For j As Integer = 0 To Item.DropDownItems.Count - 1
                Item.DropDownItems(j).BackColor = Color
            Next
        Next

        Dim Query As Generic.IEnumerable(Of ToolStripDropDownButton) = From btn As ToolStripItem In ToolStrip1.Items
                                                                       Where TypeOf btn Is ToolStripDropDownButton
                                                                       Select CType(btn, ToolStripDropDownButton) '
        Dim ButtonItem As ToolStripDropDownButton
        For i As Integer = 0 To Query.ToArray.Count - 1
            ButtonItem = Query(i)
            For j As Integer = 0 To ButtonItem.DropDownItems.Count - 1
                ButtonItem.DropDownItems(j).BackColor = Color
            Next
        Next
    End Sub

    ''' <summary>
    ''' Save the IDE profile data and exit.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub IDEExit(sender As Object, e As EventArgs) _
        Handles CloseToolStripMenuItem.Click, ExitToolStripMenuItem.Click

        If Not WindowState = FormWindowState.Maximized Then
            Program.Dev2Profile.IDE.Location.Left = Location.X
            Program.Dev2Profile.IDE.Location.Top = Location.Y
            Program.Dev2Profile.IDE.Size.Width = Size.Width
            Program.Dev2Profile.IDE.Size.Height = Size.Height
        End If

        Call Close()
    End Sub

    Private Sub FormMain_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        If WindowState = System.Windows.Forms.FormWindowState.Maximized Then
            Me.RestoreToolStripMenuItem.Enabled = True
            Me.MaximizeToolStripMenuItem.Enabled = False
        Else
            Me.RestoreToolStripMenuItem.Enabled = False
            Me.MaximizeToolStripMenuItem.Enabled = True
        End If

        MultipleTabpagePanel1.Location = New Drawing.Point With {.X = 4, .Y = 102}
        MultipleTabpagePanel1.Size = New Drawing.Size With {.Width = Width - 10, .Height = Height - 152}

        StatusBar.Location = New Drawing.Point With {.X = 0, .Y = Height - 22}
        StatusBar.Width = Width

        OutputConsole1.Location = New Drawing.Point(x:=MultipleTabpagePanel1.Left, y:=Height - OutputConsole1.Height - LabelButtonConsole.Height - StatusBar.Height - 5)
        OutputConsole1.Width = MultipleTabpagePanel1.Width + 2

        LabelButtonConsole.Location = New Drawing.Point(x:=10, y:=Height - 48)
    End Sub

    Private Sub InitializeHandlers()
        AddHandler RestoreToolStripMenuItem.Click, Sub() WindowState = System.Windows.Forms.FormWindowState.Normal
        AddHandler MaximizeToolStripMenuItem.Click, Sub() WindowState = System.Windows.Forms.FormWindowState.Maximized
        AddHandler MinimizeToolStripMenuItem.Click, Sub() WindowState = System.Windows.Forms.FormWindowState.Minimized

        AddHandler Caption.CallFormClose, AddressOf ExitIDE

        AddHandler LabelButtonConsole.Click, Sub() Call OutputConsole1.PopOut()
    End Sub

    Private Sub ExitIDE()
        If Microsoft.VisualBasic.MolkPlusTheme.MessageBox.Show(Title:="GCModeller Workbench",
                                                               Message:="Sure to exit GCModeller workbench?",
                                                               MButtons:=MolkPlusTheme.MessageBox.CYButtons.YesNo,
                                                               MIcon:=MolkPlusTheme.MessageBox.CYIcon.Question) = DialogResult.Yes Then
            Call IDEExit(Nothing, Nothing)
        End If
    End Sub

    Private Sub CreateFromMetaCycToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateFromMetaCycToolStripMenuItem.Click
        If MetaCycLoad.ShowDialog = System.Windows.Forms.DialogResult.OK Then

        End If
    End Sub

    Private Sub BuildFASTADBToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Using OpenFileDialog As New OpenFileDialog
            OpenFileDialog.Filter = "Genbank Flat File(*.gbk)|*.gbk|FASTA sequence(*.fsa;*.fasta)|*.fsa;*.fasta"
            If OpenFileDialog.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Call ExternalCommands.Build(OpenFileDialog.FileName, "")
            End If
        End Using
    End Sub

    Private Sub ReengineeringWizardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReengineeringWizardToolStripMenuItem.Click
        Call (New RWizard).Show()
    End Sub

    Private Sub InstallNewDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstallNewDatabaseToolStripMenuItem.Click
        Call LocalBlast.InstallNewDB()
    End Sub

    Private Sub CommandLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommandLineToolStripMenuItem.Click
        Call Microsoft.VisualBasic.Interaction.Shell(String.Format("{0}/Terminal.exe ""{1}""", My.Application.Info.DirectoryPath, My.Application.Info.DirectoryPath), AppWinStyle.NormalFocus)
    End Sub

    Private Sub PlugInsManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlugInsManagerToolStripMenuItem.Click
        Call PlugInsManager.ShowDialog()
    End Sub

    Private Sub ReconstructeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReconstructeToolStripMenuItem.Click
        Call New Threading.Thread(Sub()
                                      Dim IO = New Microsoft.VisualBasic.CommandLine.IORedirect(".\c2.exe")
                                      AddHandler IO.DataArrival, Sub(s As String) Call Program.IDEInstance.Out(s)
                                      Call IO.Start(WaitForExit:=True)
                                  End Sub).Start()
    End Sub

    Public Sub New()
        Call MyBase.New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub GenomeincodeProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GenomeincodeProjectToolStripMenuItem.Click

    End Sub

    Private Sub ShoalShellLanguageProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShoalShellLanguageProjectToolStripMenuItem.Click

    End Sub
End Class