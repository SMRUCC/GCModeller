#Region "Microsoft.VisualBasic::2739486ac2fdea68803bda58d63ce823, ..\visualize\visualizeTools\ComparativeGenomics\GeneObject.vb"

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

Namespace ComparativeGenomics

    Public Class GeneObject : Inherits MapModelCommon

        ''' <summary>
        ''' 基因号
        ''' </summary>
        ''' <returns></returns>
        Public Property locus_tag As String
        ''' <summary>
        ''' 基因名
        ''' </summary>
        ''' <returns></returns>
        Public Property geneName As String

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1},,   ({2},{3})", Direction, locus_tag, Left, Right)
        End Function

        ''' <summary>
        ''' 对于两个没有交叉的基因，不做任何附加处理。对于两个有相交部分的基因，则前一个基因会缩短以防止重叠，假若某一个基因完全的包裹另外一个基因，则也将不会做任何处理
        ''' </summary>
        ''' <param name="Gr"></param>
        ''' <param name="RefPoint"></param>
        ''' <param name="IdGrawingPositionDown">基因标号是否绘制与基因图形的下方</param>
        ''' <param name="Region">当前的基因对象所绘制的区域从这个参数进行返回</param>
        ''' <returns>函数返回下一个基因对象的左端的坐标的<see cref="Point.X"></see></returns>
        ''' <remarks></remarks> 
        Public Function InvokeDrawing(Gr As Graphics, RefPoint As Point, NextLeft As Integer,
                                      ConvertFactor As Double,
                                      ByRef Region As Rectangle,
                                      IdGrawingPositionDown As Boolean,
                                      Font As Font,
                                      AlternativeArrowStyle As Boolean,
                                      ByRef IDConflictedRegion As Rectangle) As Integer
            Dim GraphicPath As Drawing2D.GraphicsPath
            Dim Right As Integer = __nextLeft(Left, RefPoint, NextLeft, ConvertFactor)

            Me.ConvertFactor = ConvertFactor

            If Direction < 0 Then
                If AlternativeArrowStyle Then
                    GraphicPath = MyBase.CreateBackwardModel(RefPoint, Right)
                Else
                    GraphicPath = CreateBackwardModel(RefPoint, Right)
                End If
            ElseIf Direction > 0 Then
                If AlternativeArrowStyle Then
                    GraphicPath = MyBase.CreateForwardModel(RefPoint, Right)
                Else
                    GraphicPath = CreateForwardModel(RefPoint, RightLimit:=Right)
                End If
            Else
                GraphicPath = CreateNoneDirectionModel(RefPoint, RightLimit:=Right)
            End If

            Call Gr.DrawPath(New System.Drawing.Pen(System.Drawing.Brushes.Black, 2), GraphicPath)
            Call Gr.FillPath(If(Me.Color Is Nothing, Brushes.Brown, Color), GraphicPath)

            Dim RegionLoc = New Point((From n In GraphicPath.PathPoints Select n.X).ToArray.Min, (From n In GraphicPath.PathPoints Select n.Y).ToArray.Min)
            Dim RegionSiz = New Point((From n In GraphicPath.PathPoints Select n.X).ToArray.Max, (From n In GraphicPath.PathPoints Select n.Y).ToArray.Max)

            If Direction > 0 Then
                Region = New Rectangle(RegionLoc, New Size With {.Width = RegionSiz.X - RegionLoc.X - HeadLength, .Height = RegionSiz.Y - RegionLoc.Y})
            ElseIf Direction < 0 Then
                Region = New Rectangle(New Point(RegionLoc.X + HeadLength, RegionLoc.Y), New Size With {.Width = RegionSiz.X - RegionLoc.X - HeadLength, .Height = RegionSiz.Y - RegionLoc.Y})
            End If

            Dim size = Gr.MeasureString(Me.locus_tag, Font)
            Dim fleft As Integer = (Region.Width - size.Width) / 2 + Region.Left
            Dim Ptr As Point

            If IdGrawingPositionDown Then
                Ptr = New Point(fleft, Region.Bottom + offsets)
            Else
                Ptr = New Point(fleft, Region.Top - offsets - size.Height)
            End If

            Dim ThisIDRegion = New Rectangle(Ptr, New Size(size.Width, size.Height))
            If IDConflictedRegion.Right > ThisIDRegion.Left Then
                ThisIDRegion = New Rectangle(New Point(Ptr.X, IDConflictedRegion.Top - size.Height), New Size(size.Width, size.Height))
            Else

            End If

            IDConflictedRegion = ThisIDRegion

            Call Gr.DrawString(locus_tag, Font, brush:=Brushes.Black, x:=ThisIDRegion.X, y:=ThisIDRegion.Y)

            Return Right
        End Function

        ''' <summary>
        ''' 编号标签与ORF图形之间在水平位置上面的位移
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property offsets As Integer = 12

        ''' <summary>
        ''' 对于两个没有交叉的基因，不做任何附加处理。对于两个有相交部分的基因，则前一个基因会缩短以防止重叠，假若某一个基因完全的包裹另外一个基因，则也将不会做任何处理
        ''' </summary>
        ''' <param name="NextLeft">这个是基因组上面的位置，不是画图的位置</param>
        ''' <param name="RefPoint">参数里面的<see cref="Point.X"></see>参数就是当前的这个基因在绘图的时候的<see cref="Left"></see>在图上面的位置</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __nextLeft(Left As Integer, RefPoint As Point, NextLeft As Integer, ConvertFactor As Double) As Integer
            NextLeft = ConvertFactor * (NextLeft - Left) + RefPoint.X
            Return NextLeft
        End Function

        ''' <summary>
        ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
        ''' </summary>
        ''' <param name="refLoci"></param>
        ''' <param name="RightLimit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function CreateForwardModel(refLoci As Point, RightLimit As Integer) As Drawing2D.GraphicsPath
            Dim Graphic As New Drawing2D.GraphicsPath
            Dim pt_lefttop As New Point(refLoci.X, refLoci.Y)
            Dim pt_leftbottom As New Point(refLoci.X, refLoci.Y + Height)

            Dim pt_rightbottom As New Point(refLoci.X + Length - HeadLength, pt_leftbottom.Y)
            If pt_rightbottom.X > RightLimit Then
                pt_rightbottom = New Point(RightLimit, pt_rightbottom.Y)
            End If
            Dim pt_arrowHead As New Point(pt_rightbottom.X + HeadLength, pt_rightbottom.Y - 0.5 * Height)
            Dim pt_righttop As New Point(pt_rightbottom.X, refLoci.Y)

            Call Graphic.AddLine(pt_lefttop, pt_leftbottom)
            Call Graphic.AddLine(pt_leftbottom, pt_rightbottom)

            Call Graphic.AddLine(pt_rightbottom, pt_arrowHead)

            Call Graphic.AddLine(pt_arrowHead, pt_righttop)
            Call Graphic.AddLine(pt_righttop, pt_lefttop)

            Return Graphic
        End Function

        ''' <summary>
        ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
        ''' </summary>
        ''' <param name="refLoci"></param>
        ''' <param name="RightLimit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function CreateBackwardModel(refLoci As Point, RightLimit As Integer) As Drawing2D.GraphicsPath
            Dim Graphic As New Drawing2D.GraphicsPath
            Dim pt_lefttop As New Point(refLoci.X + HeadLength, refLoci.Y)
            Dim pt_arrowHead As New Point(refLoci.X, pt_lefttop.Y + 0.5 * Height)
            Dim pt_leftbottom As New Point(pt_lefttop.X, refLoci.Y + Height)

            Dim pt_righttop As New Point(refLoci.X + Length, refLoci.Y)
            If pt_righttop.X > RightLimit Then
                pt_righttop = New Point(RightLimit, pt_righttop.Y)
            End If
            Dim pt_rightbottom As New Point(pt_righttop.X, pt_leftbottom.Y)

            Call Graphic.AddLine(pt_lefttop, pt_arrowHead)
            Call Graphic.AddLine(pt_arrowHead, pt_leftbottom)
            Call Graphic.AddLine(pt_leftbottom, pt_rightbottom)
            Call Graphic.AddLine(pt_righttop, pt_rightbottom)
            Call Graphic.AddLine(pt_righttop, pt_lefttop)

            Return Graphic
        End Function
    End Class
End Namespace
