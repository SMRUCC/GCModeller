#Region "Microsoft.VisualBasic::fd0bcbb3f2f53e5a1255de08839a158a, ..\workbench\devenv\TabPages\NCBIViewer\GenbankViewer.vb"

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

Imports LANS.SystemsBiology.Assembly.NCBI.GenBank
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.Extensions
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.GBFF

Friend Class GenbankViewer

    Dim GbkFile As GBFF.File
    Dim Viewer As NCBIViewer

    Dim GenList As KeyValuePair(Of String, String)()

    Dim Current As GeneObject

    Private Sub LoadFile()
        If Viewer.OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Viewer.Label1.Text = Viewer.OpenFileDialog1.FileName
            GbkFile = Viewer.OpenFileDialog1.FileName

            Dim GeneList As System.Windows.Forms.ListBox = Viewer.ListBox1
            GeneList.Items.Clear()
            GenList = GbkFile.GeneList
            For Each Gene In GenList
                If String.IsNullOrEmpty(Gene.Value) Then
                    GeneList.Items.Add(Gene.Key)
                Else
                    GeneList.Items.Add(String.Format("{0}   [{1}]", Gene.Key, Gene.Value))
                End If
            Next

            GeneList.SelectedIndex = 0
        End If
    End Sub

    Private Sub ListFeature()
        Dim List As System.Windows.Forms.ListBox = Viewer.ListBox2

        Call List.Items.Clear()
        Current = GbkFile.GetGene(GenList(Viewer.ListBox1.SelectedIndex).Key)
        For Each Item In Current.Features
            List.Items.Add(String.Format("{0}  [{1}]", Item.KeyName, Item.Location.ToString))
        Next

        List.SelectedIndex = 0

        Dim Query = From e In Current.Features Where String.Equals(e.KeyName, "CDS") Select e '
        Dim CDS = Query.First

        Viewer.TextBox1.Text = Current.LocusTag
        Viewer.TextBox2.Text = CDS.Query("translation")
    End Sub

    Private Sub LocalBLAST()
        Dim FASTA As LANS.SystemsBiology.SequenceModel.FASTA.FastaToken = New SequenceModel.FASTA.FastaToken

        FASTA.Attributes = {Current.LocusTag, Current.Gene}
        FASTA.SequenceData = Viewer.TextBox2.Text
        Dim TEMPFile As String = String.Format("{0}/{1}.fsa", Settings.TEMP, Current.LocusTag)

        Call FASTA.SaveTo(TEMPFile)
        Call Viewer.LocalBLAST.Load(TEMPFile)

        Viewer.TabControl1.SelectedIndex = 1
    End Sub

    Private Sub ListFeatureInside()
        Dim List As System.Windows.Forms.ListBox = Viewer.ListBox3

        Call List.Items.Clear()
        For Each Item In Current.Features(Viewer.ListBox2.SelectedIndex).PairedValues
            List.Items.Add(String.Format("{0} = {1}", Item.Key, Item.Value))
        Next
    End Sub

    Public Sub New(e As NCBIViewer)
        Viewer = e
        Call Initialize()
    End Sub

    Public Sub Initialize()
        AddHandler Viewer.LinkLabel2.Click, Sub() LoadFile()
        AddHandler Viewer.ListBox1.SelectedIndexChanged, Sub() Call ListFeature()
        AddHandler Viewer.ListBox2.SelectedIndexChanged, Sub() Call ListFeatureInside()
        AddHandler Viewer.Button5.Click, Sub() Call LocalBLAST()
    End Sub
End Class
