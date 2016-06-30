Public Class GeneObject

    Public ReadOnly Property Gff As LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.Feature



    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Sub New(Element As Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow, BackColor As Color)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Size = Element.BodySize

        Dim gdi = Size.CreateGDIDevice(BackColor)

        Element = New Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow(source:=Element, GDI:=gdi)
        Element.Location = New Point(If(Element.DirectionLeft, 0, Width), CInt(Height / 2))
        Gff = LANS.SystemsBiology.Assembly.NCBI.GenBank.PttGenomeBrief.GenomeFeature.Feature.CreateObject(Element.TooltipTag, version:=3)
        Me.Name = Gff.attributes("name")
        Call Element.InvokeDrawing()
        BackgroundImage = gdi.ImageResource

        Dim UI = New MolkPlusTheme.Visualise.Elements.ButtonResource With {.Normal = gdi.ImageResource}
        UI.InSensitive = gdi.ImageResource

        gdi = Size.CreateGDIDevice(BackColor)

        Element = New Microsoft.VisualBasic.Drawing.Drawing2D.VectorElements.Arrow(source:=Element, GDI:=gdi)
        Element.Location = New Point(If(Element.DirectionLeft, 0, Width), CInt(Height / 2))
        Element.Color = Color.OrangeRed

        Call Element.InvokeDrawing()
        UI.PreLight = gdi.ImageResource
        UI.Active = gdi.ImageResource

        Me.UI = UI

    End Sub

End Class
