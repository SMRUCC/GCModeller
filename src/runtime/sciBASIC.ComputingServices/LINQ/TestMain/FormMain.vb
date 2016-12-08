#Region "Microsoft.VisualBasic::e1570de7c38c402c4924f65da9840e25, ..\sciBASIC.ComputingServices\LINQ\TestMain\FormMain.vb"

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

Imports Microsoft.VisualBasic.Linq.LDM.Statements

Public Class FormMain

    Dim LINQFramework As Linq.Script.DynamicsRuntime = New Linq.Script.DynamicsRuntime

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Call Exe(TextBox1.Text)
    End Sub

    ''' <summary>
    ''' Execute the linq script
    ''' </summary>
    ''' <param name="Linq"></param>
    ''' <remarks></remarks>
    Private Sub Exe(Linq As String)
        Dim Statement = LinqStatement.TryParse(Linq) ', LINQFramework.TypeRegistry)

        TextBox1.AppendText(String.Format("{0}{1}Auto-generated code for debug:{2}{3}{4}", vbCrLf, vbCrLf, vbCrLf, Statement.CompiledCode, vbCrLf))
        TextBox1.AppendText(vbCrLf & "Query Result:" & vbCrLf)

        Dim Collection = LINQFramework.EXEC(Statement)

        For Each obj In Collection
            Call TextBox1.AppendText(vbCrLf & obj.ToString & vbCrLf)
        Next
    End Sub

    ''' <summary>
    ''' registry the LINQ entity external module
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub RegistryExternalModuleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegistryExternalModuleToolStripMenuItem.Click
        Dim File As New Global.System.Windows.Forms.OpenFileDialog
        If File.ShowDialog = Global.System.Windows.Forms.DialogResult.OK Then
            Call LINQFramework.Types.Register(File.FileName)
            Call LINQFramework.Types.Save()
        End If
    End Sub

    Private Sub QueryXMLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QueryXMLToolStripMenuItem.Click
        '    Dim LINQ As String = "from member as member in ""TEST_XML_FILE.xml"" let name = member.name where microsoft.visualbasic.instr(name,""MetaCyc"") select microsoft.visualbasic.mid(member.summary,1,20)"
        Dim LINQ As String = "from member as member in ""TEST_XML_FILE.xml"" select microsoft.visualbasic.mid(member.summary,1,20)"
        TextBox1.Text = ""
        TextBox1.AppendText(LINQ)
    End Sub

    Private Sub SequencePatternSearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequencePatternSearchToolStripMenuItem.Click
        Dim LINQ As String = "from fasta as fasta in "".\xcc8004.fsa"" let seq = fasta.sequence let match = regex.match(seq,""A+T{2,}GCA+TT"") where match.success select fasta.tostring + microsoft.visualbasic.vbcrlf + ""***"" + match.value + ""***"""
        TextBox1.Text = ""
        TextBox1.AppendText(LINQ)
    End Sub

    Private Sub StringCollectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringCollectionToolStripMenuItem.Click
        Dim LINQ As String = "from str as string in "".\LINQ.xml"" where str.length > 50 select str"
        TextBox1.Text = ""
        TextBox1.AppendText(LINQ)
    End Sub
End Class

