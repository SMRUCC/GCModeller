#Region "Microsoft.VisualBasic::d22e9c410674762b1338b3acc749d100, ..\workbench\devenv\TabPages\NCBIViewer\LocalBlast.vb"

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

Imports Microsoft.VisualBasic

Friend Class LocalBlast

    Dim SelectDBFile As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
    Dim Viewer As NCBIViewer
    Dim SelectedFASTA As String
    Dim c2Profile As Settings.Programs.C2 '=填入初始化代码
    Public Shared c2 As String = My.Application.Info.DirectoryPath & "/c2.exe"

    Dim InstalledDb As List(Of String) = New List(Of String)
    Dim InstalledNotCorrect As List(Of String) = New List(Of String)

    Public Sub Load(FASTA As String)
        Viewer.TextBox3.Text = FileIO.FileSystem.ReadAllText(FASTA)
        SelectedFASTA = FASTA
    End Sub

    Sub New(e As NCBIViewer)
        Viewer = e
        SelectDBFile.Filter = "NCBI Genbank Flat File(*.gbk;*.gb)|*.gbk;*.gb|FASTA sequence file(*.fasta;*.fsa)|*.fasta;*.fsa"

        Call Initialize()
    End Sub

    Public Shared Sub InstallNewDB()
        Using SelectDBFile As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog With {
            .Filter = "NCBI Genbank Flat File(*.gbk;*.gb)|*.gbk;*.gb|FASTA sequence file(*.fasta;*.fsa)|*.fasta;*.fsa"}

            If SelectDBFile.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Dim Process As Microsoft.VisualBasic.CommandLine.IORedirect
                If InStr("gbk", System.IO.Path.GetExtension(SelectDBFile.FileName).Replace(".", "").ToLower) Then 'is gbk format
                    Process = New Microsoft.VisualBasic.CommandLine.IORedirect(c2, String.Format("build -i ""{0}"" -f gbk", SelectDBFile.FileName))
                Else 'fsa
                    Process = New Microsoft.VisualBasic.CommandLine.IORedirect(c2, String.Format("build -i ""{0}"" -f fsa", SelectDBFile.FileName))
                End If

                AddHandler Process.DataArrival, Sub(s As String) Call Out(s, ConsoleColor.White, "C2", Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
                AddHandler Process.ProcessExit, Sub(c As Integer, s As String)
                                                    If c = 0 Then
                                                        Out(String.Format("Installation done!  c2 exit code = {0}, exitTime = {1}", c, s))
                                                    Else
                                                        Out(String.Format("Installation failure! Something was going wrong....."))
                                                        s = String.Format("Install database ""{0}"" failure!", SelectDBFile.FileName)
                                                        Program.IDEStatueText(s, Drawing.Color.Red)
                                                        Out(s, ConsoleColor.Red, "GCMODELLER_IDE_LOCALBLAST_INSTALL_DATABASE", Microsoft.VisualBasic.Logging.MSG_TYPES.ERR)
                                                    End If
                                                End Sub
                Process.Start(False)
            End If
        End Using
    End Sub

    Private Sub Initialize()
        AddHandler Viewer.LinkLabel1.Click, Sub() Call InstallNewDB()
        AddHandler Viewer.LinkLabel4.Click, Sub() Call (New InstallFastaDb(InstalledNotCorrect)).ShowDialog()

        Dim DbFastas As String() = If(String.IsNullOrEmpty(Program.Profile.SettingsData.BlastDb),
                                        New String() {},
                                        FileIO.FileSystem.GetFiles(Program.Profile.SettingsData.BlastDb, FileIO.SearchOption.SearchAllSubDirectories, "*.fsa", "*.fasta").ToArray)

        Dim Query As Generic.IEnumerable(Of String) = From s As String In DbFastas Let psq = s & ".psq" Where Not FileIO.FileSystem.FileExists(psq) Select s '没有被格式化的数据库
        Dim LQuery = From s As String In DbFastas Let psq = s & ".psq" Where FileIO.FileSystem.FileExists(psq) Select s '已经格式化的

        InstalledDb = LQuery.ToList
        InstalledNotCorrect = Query.ToList

        For Each File In InstalledDb
            Me.Viewer.CheckedListBox1.Items.Add(File.Replace("\", "/").Split(CChar("/")).Last.Split(CChar(".")).First)
        Next

        If InstalledNotCorrect.Count > 0 Then
            Viewer.LinkLabel4.Visible = True
        End If
    End Sub
End Class
