#Region "Microsoft.VisualBasic::f451eb35509a01167667cd4abca29049, ..\workbench\GenomicsBrowser\GenomeBrowser\BrowserControls\GeneObject.vb"

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
