#Region "Microsoft.VisualBasic::1ccefa8347e2388917873687960c72ff, ..\workbench\devenv\Forms\FormMain\MultipleTabpageProxy.vb"

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

Imports System.Windows.Forms

Public Class MultipleTabpageProxy

    Public Property WinForm As FormMain
    Public Property MultipleTabpagePanel As MolkPlusTheme.Windows.Forms.Controls.TabControl.TabPage.MultipleTabpagePanel

    Dim WindowsToolStripMenuItem As ToolStripMenuItem
    Dim TabList As New Dictionary(Of String, ToolStripMenuItem)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Tab">Tab name of this new tabpage</param>
    ''' <returns>The target tab was added to the panel(TRUE) or it exists on the panel before(FALSE).</returns>
    ''' <remarks></remarks>
    Private Function InternalAddNewTab(Tab As String, Control As Control) As Boolean
        Call MultipleTabpagePanel.AddTabPage(Tab, Control, TabCloseEventHandle:=Nothing)

        Dim NewWindowItem As New ToolStripMenuItem With {.Text = Tab}  '动态生成标签页列表菜单
        WindowsToolStripMenuItem.DropDownItems.Add(NewWindowItem)
        TabList.Add(Tab, NewWindowItem)

        AddHandler NewWindowItem.Click, Sub() Call MultipleTabpagePanel.ActiveTabPage(Tab)

        Return True
    End Function

    Private Function InternalActiveTab(Tab As String) As Boolean
        If MultipleTabpagePanel.ContainsTabPage(Tab) Then
            Call MultipleTabpagePanel.ActiveTabPage(Tab)
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub OpenBrowser(Tab As String, Url As String)
        If Not InternalActiveTab(Tab) Then
            Dim Browser As New TabPages.WebBrowser With {.MyHomePage = Url}

            Call InternalAddNewTab(Tab, Browser)
            Call New Threading.Thread(Sub()
                                          Threading.Thread.Sleep(1)
                                          Call Browser.GotoHomePage()
                                      End Sub).Start()
        End If
    End Sub

    Const LicensePage As String = "GPL 2.0 License"

    Private Sub AddLicensePage()
        If Not InternalActiveTab(LicensePage) Then
            Dim NewPage As New System.Windows.Forms.RichTextBox

            Call InternalAddNewTab(LicensePage, NewPage)
            If FileIO.FileSystem.FileExists(LicenseFile) Then
                NewPage.LoadFile(LicenseFile)
            Else
                NewPage.AppendText(String.Format("File ""{0}""not found, could not load document.", LicenseFile))
            End If
            NewPage.ReadOnly = True
        End If
    End Sub

    Public Sub AddTabPage(Of T As System.Windows.Forms.Control)(Name As String)
#If DEBUG Then
        Call Program.Out("Add " & Name)
#End If

        If Not InternalActiveTab(Name) Then
            Call InternalAddNewTab(Name, Activator.CreateInstance(Of T)())
        End If
    End Sub

    ''' <summary>
    ''' User click close button on the tabpage to removed this tab
    ''' </summary>
    ''' <param name="Tab">The name of the removed tabpage.</param>
    ''' <remarks>在这里仅移除Windows菜单下面的列表元素即可</remarks>
    Private Sub RemovedTab(Tab As String)
        Dim Item As ToolStripMenuItem = TabList(Tab)

        WindowsToolStripMenuItem.DropDownItems.Remove(Item)
        TabList.Remove(Tab)

        Call Item.Dispose()
    End Sub

    Private Sub RemoveAll()
        Dim Items As ToolStripMenuItem() = TabList.Values.ToArray
        For i As Integer = 0 To Items.Count - 1
            WindowsToolStripMenuItem.DropDownItems.Remove(Items(i))
            Items(i).Dispose()
        Next

        TabList.Clear()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Tab">The name of the tab that was lefted</param>
    ''' <remarks></remarks>
    Private Sub RemoveAllBut(Tab As String)
        Dim RQuery As Generic.IEnumerable(Of ToolStripMenuItem) = From e As ToolStripMenuItem In TabList.Values Where Not String.Equals(e.Text, Tab) Select e '
        Dim Items As ToolStripMenuItem() = RQuery.ToArray

        For i As Integer = 0 To Items.Count - 1
            WindowsToolStripMenuItem.DropDownItems.Remove(Items(i))
            TabList.Remove(Items(i).Text)
            Items(i).Dispose()
        Next
    End Sub

    Private Sub LocalBlast()
        Call AddTabPage(Of NCBIViewer)("NCBI Viewer")
        CType(MultipleTabpagePanel("NCBI Viewer"), NCBIViewer).TabControl1.SelectedIndex = 1
    End Sub

    Private Sub Initialize()
        AddHandler WinForm.GenomeincodeProjectToolStripMenuItem.Click, Sub() Call OpenBrowser("Home [""genome-in-code"" Project]", "http://code.google.com/p/genome-in-code")
        AddHandler WinForm.CodeProjectToolStripMenuItem.Click, Sub() Call OpenBrowser("Blog [codeProject.com]", "http://www.codeproject.com/Members/xieguigang")
        AddHandler WinForm.CSDNNETToolStripMenuItem.Click, Sub() Call OpenBrowser("Blog [csdn.net]", "http://blog.csdn.net/xie_guigang")
        AddHandler WinForm.ShoalShellLanguageProjectToolStripMenuItem.Click, Sub() Call OpenBrowser("Shoal Shell", "https://sourceforge.net/projects/shoal/")

        AddHandler WinForm.AboutGCModellerToolStripMenuItem_.Click, Sub() Call New FormAbout() With {.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent}.ShowDialog()
        AddHandler WinForm.AboutGCModellerToolStripMenuItem1.Click, Sub() Call New FormAbout() With {.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent}.ShowDialog()

        AddHandler WinForm.StartPageToolStripMenuItem.Click, Sub() Call AddTabPage(Of TabPages.StartPage)("Start Page")
        AddHandler WinForm.OptionsToolStripMenuItem.Click, Sub() Call AddTabPage(Of TabPages.Options)("Options")
        AddHandler WinForm.LicenseToolStripMenuItem.Click, Sub() Call AddLicensePage()
        AddHandler WinForm.ReportProblemsToolStripMenuItem.Click, Sub() Call AddTabPage(Of BugsReport)("Bugs Report")  '  Call AddBugsReport()
        AddHandler WinForm.NCBIToolStripMenuItem.Click, Sub() Call AddTabPage(Of NCBIViewer)("NCBI Viewer")
        AddHandler WinForm.OpenTabpageToolStripMenuItem.Click, Sub() Call LocalBlast()
        AddHandler WinForm.ShellScriptToolStripMenuItem.Click, Sub() AddTabPage(Of ShellScriptTerm)("ShellScript")

        AddHandler MultipleTabpagePanel.RemoveTab, AddressOf RemovedTab
        AddHandler MultipleTabpagePanel.RemoveAllTabs, AddressOf RemoveAll
        AddHandler MultipleTabpagePanel.RemoveAllTabsButOne, AddressOf RemoveAllBut

        Call AddTabPage(Of TabPages.StartPage)("Start Page")
    End Sub

    Public Shared Widening Operator CType(e As FormMain) As MultipleTabpageProxy
        Dim NewObj = New MultipleTabpageProxy With {.WinForm = e}
        NewObj.MultipleTabpagePanel = e.MultipleTabpagePanel1
        NewObj.WindowsToolStripMenuItem = e.WindowsToolStripMenuItem
        NewObj.Initialize()

        Return NewObj
    End Operator
End Class

