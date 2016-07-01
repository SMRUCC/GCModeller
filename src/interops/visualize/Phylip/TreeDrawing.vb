Imports System.Drawing
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Phylip.Evolview
Imports Microsoft.VisualBasic.Imaging

Public Module TreeDrawing

    Dim pxPerBranchLength As Integer
    Dim pxPerHeight As Single = 10

    Public Function InvokeDrawing(Tree As Evolview.PhyloTree) As System.Drawing.Image
        Dim Gr = (New Size(10000, 10000)).CreateGDIDevice()
        Dim Font As New Font(FontFace.Ubuntu, 12)
        Dim RootXY As Point = New Point(Gr.Width / 2, Gr.Height / 2)
        Dim MinBranchLength As Double = (From n In Tree.AllNodes Where n.BranchLength <> 0S Select Math.Abs(n.BranchLength)).ToArray.Min * 1000

        pxPerBranchLength = 1 / MinBranchLength

        Gr.Graphics.DrawLine(Pens.Black, RootXY.X, RootXY.Y, CInt(RootXY.X - pxPerBranchLength / 10 / 5), RootXY.Y)
        Call __treeDrawing(Tree.RootNode, Gr, RootXY, Font)

        Return Gr.ImageResource
    End Function

    ''' <summary>
    ''' 这个函数之中不绘制根节点
    ''' </summary>
    ''' <param name="plNode"></param>
    ''' <param name="LayerTree"></param>
    ''' <param name="ParentXY"></param>
    ''' <remarks>
    '''                                |------------
    '''                       ---------|   branchlength
    '''        branchLength   |        |--------------------------
    ''' root -----------------|                               ---------------------------------
    '''                       |      branchlength             |
    '''                       --------------------------------|
    '''                                                       |---------------
    ''' </remarks>
    Private Sub __treeDrawing(plNode As PhyloNode, LayerTree As GDIPlusDeviceHandle, ParentXY As Point, Font As Font)
        Dim Px As Single = ParentXY.X
        Dim Py As Single = ParentXY.Y
        Dim vLevel As Single = plNode.LevelVertical
        Dim hLevel As Single = plNode.LevelHorizontal

        For Each Node As PhyloNode In plNode.Descendents   ' iterate child nodes

            Dim VL As Single = Node.LevelVertical
            Dim HL As Single = Node.LevelHorizontal
            Dim HB As Single = Node.BranchLength
            Dim dX As Single = Px + HB * pxPerBranchLength
            Dim dY As Single = Py + (VL - vLevel) * pxPerHeight
            Dim CurrentXY As Point = New Drawing.Point(dX, dY)

            Call LayerTree.Graphics.DrawPie(Pens.Black, New Rectangle(CurrentXY, New Size(2, 2)), 0, 360)

            If Node.IsFork Then  '分支节点，则绘制当前节点之外，还需要绘制子节点
                Call __treeDrawing(Node, LayerTree, CurrentXY, Font)
            ElseIf Node.IsLeaf Then        '叶节点，则需要绘制编号
                Call LayerTree.Graphics.DrawString(Node.ID, Font, Brushes.Black, New Point(CurrentXY.X + 10, CurrentXY.Y + 10))
            End If

            '绘制分支长度
            Call LayerTree.Graphics.DrawString(HB, Font, Brushes.Black, New Point((Px + dX) / 2, dY))
        Next
    End Sub
End Module
