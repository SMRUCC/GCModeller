#Region "Microsoft.VisualBasic::cb5afc469d532fe155c9320dc8c381c0, ..\workbench\IDE_PlugIns\ModelEditor\ModelEditor.vb"

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

Public Class ModelEditor

    Dim Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO.XmlresxLoader
    Dim EquationList As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.MetabolismFlux()
    Dim CurrentMetabolite As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.Metabolite

    Private Sub ModelEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call ComboBox1.Items.Add("Reactants")
        Call ComboBox1.Items.Add("Products")

        Label1.Text = ""
    End Sub

    Public Function OpenModel() As Boolean
        OpenFileDialog1.Filter = "ModelFile|*.xml"
        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Model = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.CsvTabular.FileStream.IO.XmlresxLoader(OpenFileDialog1.FileName)
            Text = String.Format("[OPEN]  {0}", OpenFileDialog1.FileName)

            Dim Metabolites = (From item In Model.MetabolitesModel.Values.AsParallel
                               Where item.MetaboliteType = LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Metabolite.MetaboliteTypes.Compound
                               Select item).ToArray

            Call ListBox1.Items.Clear()
            For Each Item In Metabolites
                Dim UniqueId As String
                If String.IsNullOrEmpty(Item.KEGGCompound) Then
                    UniqueId = Item.Identifier
                Else
                    UniqueId = String.Format("{0} [{1}]", Item.Identifier, Item.KEGGCompound)
                End If

                Call ListBox1.Items.Add(UniqueId)
            Next
        Else
            Return False
        End If

        Return True
    End Function

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Dim UniqueId As String = ListBox1.SelectedItem.ToString.Split.First
        Call NavigateToMetabolite(UniqueId)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 0 Then
            Dim LQuery = (From item In EquationList Let IsReactant = Function() As Boolean
                                                                         If item.Reversible Then
                                                                             Return True
                                                                         Else
                                                                             Return item.get_Coefficient(CurrentMetabolite.Identifier) < 0
                                                                         End If
                                                                     End Function Where IsReactant() Select item).ToArray
            Call ListView1.Items.Clear()
            For Each item In LQuery
                Call ListView1.Items.Add(New System.Windows.Forms.ListViewItem(New String() {item.Identifier, item.Equation}))
            Next
        ElseIf ComboBox1.SelectedIndex = 1 Then
            Dim LQuery = (From item In EquationList Let IsProduct = Function() As Boolean
                                                                        If item.Reversible Then
                                                                            Return True
                                                                        Else
                                                                            Return item.get_Coefficient(CurrentMetabolite.Identifier) > 0
                                                                        End If
                                                                    End Function Where IsProduct() Select item).ToArray
            Call ListView1.Items.Clear()
            For Each item In LQuery
                Call ListView1.Items.Add(New System.Windows.Forms.ListViewItem(New String() {item.Identifier, item.Equation}))
            Next
        End If
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        'On Error Resume Next

        Dim UniqueId As String = ListView1.SelectedItems(0).Text
        Console.WriteLine(UniqueId)

        Dim Equation = EquationList.GetItem(uniqueId:=UniqueId)

        TextBox2.Text = Equation.CommonName

        Dim MaxWidth As List(Of Integer) = New List(Of Integer)

        Call Panel4.Controls.Clear()
        Dim x As Integer = 0, y = 0
        For Each Reactant In Equation.Left
            Dim Metabolite = New System.Windows.Forms.LinkLabel() With {.AutoSize = True}
            Metabolite.Location = New Drawing.Point(x, y)
            Metabolite.Text = Reactant.Value
            y += Metabolite.Height + 5
            Call Panel4.Controls.Add(Metabolite)
            Call MaxWidth.Add(Metabolite.Width)
            AddHandler Metabolite.LinkClicked, Sub() Call NavigateToMetabolite(Metabolite.Text)
        Next

        y = 0 : x = Panel4.Width / 2
        For Each Product In Equation.Right
            Dim Metabolite = New System.Windows.Forms.LinkLabel() With {.AutoSize = True}
            Metabolite.Location = New Drawing.Point(x, y)
            Metabolite.Text = Product.Value
            y += Metabolite.Height + 5
            Call Panel4.Controls.Add(Metabolite)
            AddHandler Metabolite.LinkClicked, Sub() Call NavigateToMetabolite(Metabolite.Text)
        Next

        Label1.Text = If(Equation.Reversible, "<==>", "-->")

        Call ListBox2.Items.Clear()
        If Not Equation.Enzymes.IsNullOrEmpty Then
            For Each Id As String In Equation.Enzymes
                Call ListBox2.Items.Add(Id)
            Next
        End If
    End Sub

    Private Sub NavigateToMetabolite(UniqueId As String)

        Dim LQuery = (From item In Model.MetabolismModel.AsParallel Where item.get_Coefficient(UniqueId) Select item).ToArray
        Call ListView1.Items.Clear()
        For Each item In LQuery
            Call ListView1.Items.Add(New System.Windows.Forms.ListViewItem(New String() {item.Identifier, item.Equation}))
            Console.WriteLine(item.Equation)
        Next

        EquationList = LQuery

        Dim Metabolite = Model.MetabolitesModel(UniqueId)
        CurrentMetabolite = Metabolite
        Call ToolTip1.SetToolTip(ListBox1, Metabolite.CommonNames.FirstOrDefault)

        Label2.Text = If(String.IsNullOrEmpty(Metabolite.KEGGCompound), UniqueId, String.Format("{0}  KEGG:{1}", UniqueId, Metabolite.KEGGCompound))
        TextBox3.Text = ""
        If Not Metabolite.CommonNames.IsNullOrEmpty Then
            For Each strName As String In Metabolite.CommonNames
                Call TextBox3.AppendText(strName & vbCrLf)
            Next
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        On Error Resume Next

        Call NavigateToMetabolite(TextBox1.Text.ToUpper)
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub
End Class
