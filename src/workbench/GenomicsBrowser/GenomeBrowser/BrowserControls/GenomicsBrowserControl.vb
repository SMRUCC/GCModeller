Imports Microsoft.VisualBasic.MolkPlusTheme

Public Class GenomicsBrowserControl

    Const MAXLength = 30000

    Dim VCS As Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram

    Dim TitleModifier As Action(Of String)

    Sub New(FormTitle As Action(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.TitleModifier = FormTitle
    End Sub

    Public Sub LoadDocument(Gff As LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.GFF)

        Call LoadDocument(VectorScriptProject.Gff2VectorScript(Gff))

    End Sub

    Public Sub LoadDocument(VCS As Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram)

        Call New Threading.Thread(Sub() Call Me.BeginInvoke(Sub() InternalLoadThread(VCS))).Start()

    End Sub


    Private Sub InternalLoadThread(VCS As Microsoft.VisualBasic.Drawing.Drawing2D.Vectogram)
        Me.VCS = VCS

        Dim XOffset As Integer = 0
        Dim YOffset As Integer = 0

        Dim MaxY As Integer

        Dim ChunkBuffer = VCS.ToArray
        Dim LQuery = (From idx As Integer In VCS.Sequence.AsParallel
                      Let Element = ChunkBuffer(idx)
                      Where TypeOf Element Is Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow
                      Select idx, Element, GeneObject = New GeneObject(DirectCast(Element, Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow), BackColor), Location = Element.Location
                      Order By idx Ascending).ToArray

        For Each Element In LQuery

            Dim GeneObject = Element.GeneObject

            Call Me.Controls.Add(GeneObject)
            GeneObject.Location = New Point(Element.Location.X - XOffset, Element.Location.Y + YOffset)

            If GeneObject.Location.Y > MaxY Then
                MaxY = GeneObject.Location.Y
            End If

            Call Toolstip.SetToolTip(GeneObject, " Description:    " & GeneObject.Gff.attributes("product") & vbCrLf & vbCrLf &
                                                     " Feature Location:    " & GeneObject.Gff.BaseLocation.ToString)
            Call Toolstip.SetToolTipTitle(GeneObject, GeneObject.Gff.attributes("name"))

            If GeneObject.Location.X >= MAXLength Then
                '换行
                YOffset = MaxY + 100
                XOffset += MAXLength

                Call Console.WriteLine()
                Call Console.WriteLine(New String("-", 120) & XOffset & ",  " & YOffset)
            End If

            Call Me.TitleModifier(100 * Element.idx / LQuery.Count)
        Next
    End Sub

    Dim WithEvents Toolstip As Microsoft.VisualBasic.MolkPlusTheme.ToolTip

    Private Sub GenomicsBrowserControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Toolstip = New ToolTip

        Call Me.components.Add(Toolstip)

        Toolstip.AnimationSpeed = 5
        Toolstip.EnableAutoClose = False
        Toolstip.ShowShadow = True
        '  Toolstip.OwnerDrawBackground = True

    End Sub

    Private Sub Toolstip_Popup(sender As Object, e As PopupEventArgs) Handles Toolstip.Popup
        e.Size = New Size(250, 120)
    End Sub

    Private Sub Toolstip_DrawBackground(sender As Object, e As DrawEventArgs) Handles Toolstip.DrawBackground
        Dim Entry = DirectCast(DirectCast(sender, Microsoft.VisualBasic.MolkPlusTheme.ToolTip).Control, GeneObject)
        Dim Feature = Entry.Gff

        Dim Gr = e.Graphics

        Call Gr.FillRectangle(Brushes.White, e.Rectangle)
        Call Gr.DrawRectangle(New Pen(Color.Black, 2), e.Rectangle)

    End Sub
End Class
