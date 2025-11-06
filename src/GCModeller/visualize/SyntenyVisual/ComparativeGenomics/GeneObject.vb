#Region "Microsoft.VisualBasic::bef0f307ff02426c0c9249b7a4faa67f, visualize\SyntenyVisual\ComparativeGenomics\GeneObject.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
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



' /********************************************************************************/

' Summaries:

'     Class GeneObject
' 
'         Properties: geneName, locus_tag, offsets
' 
'         Function: __nextLeft, CreateBackwardModel, CreateForwardModel, InvokeDrawing, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging

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
        ''' <param name="g"></param>
        ''' <param name="RefPoint"></param>
        ''' <param name="IdDrawPositionDown">基因标号是否绘制与基因图形的下方</param>
        ''' <param name="arrowRect">当前的基因对象所绘制的区域从这个参数进行返回</param>
        ''' <returns>函数返回下一个基因对象的左端的坐标的<see cref="Point.X"></see></returns>
        ''' <remarks></remarks> 
        Public Function InvokeDrawing(g As IGraphics, RefPoint As Point, NextLeft As Integer,
                                      scaleFactor As Double,
                                      ByRef arrowRect As Rectangle,
                                      IdDrawPositionDown As Boolean,
                                      Font As Font,
                                      AlternativeArrowStyle As Boolean,
                                      ByRef overlapLayout As MapLabelLayout,
                                      Optional drawConflictLine As Boolean = False) As Integer

            Dim path As GraphicsPath
            Dim Right As Integer = __nextLeft(Left, RefPoint, NextLeft, scaleFactor)

            Me.ConvertFactor = scaleFactor

            If Direction < 0 Then
                If AlternativeArrowStyle Then
                    path = MyBase.CreateBackwardModel(RefPoint, Right)
                Else
                    path = CreateBackwardModel(RefPoint, Right)
                End If
            ElseIf Direction > 0 Then
                If AlternativeArrowStyle Then
                    path = MyBase.CreateForwardModel(RefPoint, Right)
                Else
                    path = CreateForwardModel(RefPoint, RightLimit:=Right)
                End If
            Else
                path = CreateNoneDirectionModel(RefPoint, RightLimit:=Right)
            End If

            Call g.DrawPath(New Pen(Brushes.Black, 2), path)
            Call g.FillPath(If(Me.Color Is Nothing, Brushes.Brown, Color), path)

            Dim rectLocation = New Point((From n In path.PathPoints Select n.X).Min, (From n In path.PathPoints Select n.Y).Min)
            Dim RegionSiz = New Point((From n In path.PathPoints Select n.X).Max, (From n In path.PathPoints Select n.Y).Max)

            If Direction > 0 Then
                arrowRect = New Rectangle(
                    rectLocation, New Size With {
                        .Width = RegionSiz.X - rectLocation.X - HeadLength,
                        .Height = RegionSiz.Y - rectLocation.Y
                    })
            ElseIf Direction < 0 Then
                arrowRect = New Rectangle With {
                    .Location = New Point(rectLocation.X + HeadLength, rectLocation.Y),
                    .Size = New Size With {
                        .Width = RegionSiz.X - rectLocation.X - HeadLength,
                        .Height = RegionSiz.Y - rectLocation.Y
                    }
                }
            End If

            Dim size = g.MeasureString(Me.locus_tag, Font)
            Dim fleft As Integer = (arrowRect.Width - size.Width) / 2 + arrowRect.Left
            Dim ptr As Point
            Dim conflicts As Boolean = False

            If IdDrawPositionDown Then
                ptr = New Point(fleft, arrowRect.Bottom + offsets)
            Else
                ptr = New Point(fleft, arrowRect.Top - offsets - size.Height)
            End If

            overlapLayout = New MapLabelLayout With {
                .OverlapRegion = overlapLayout _
                .ForceNextLocation(New Rectangle With {
                    .Location = ptr,
                    .Size = New Size(size.Width, size.Height)
                }, conflicts, IdDrawPositionDown)
            }

            With overlapLayout.OverlapRegion.Location
                Call g.DrawString(locus_tag, Font, brush:=Brushes.Black, x:= .X, y:= .Y)

                If conflicts AndAlso drawConflictLine Then ' 在label的文和箭头之间画一条连线
                    Dim a, b As Point
                    Dim textRect = overlapLayout.OverlapRegion

                    If IdDrawPositionDown Then
                        ' 则连线在文本上方和箭头矩形的下方
                        a = New Point With {
                            .X = arrowRect.Left + arrowRect.Width / 2,
                            .Y = arrowRect.Bottom
                        }
                        b = New Point With {
                            .X = textRect.Left + textRect.Width / 2,
                            .Y = textRect.Top
                        }
                    Else
                        ' 连线在文本的下方和箭头矩形的上方
                        a = New Point With {
                            .X = arrowRect.Left + arrowRect.Width / 2,
                            .Y = arrowRect.Top
                        }
                        b = New Point With {
                            .X = textRect.Left + textRect.Width / 2,
                            .Y = textRect.Bottom
                        }
                    End If

                    Call g.DrawLine(Pens.Gray, a, b)
                End If
            End With

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
        ''' <param name="nextLeft">这个是基因组上面的位置，不是画图的位置</param>
        ''' <param name="RefPoint">参数里面的<see cref="Point.X"></see>参数就是当前的这个基因在绘图的时候的<see cref="Left"></see>在图上面的位置</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function __nextLeft(Left As Integer, RefPoint As Point, nextLeft As Integer, scaleFactor As Double) As Integer
            nextLeft = scaleFactor * (nextLeft - Left) + RefPoint.X
            Return nextLeft
        End Function

        ''' <summary>
        ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
        ''' </summary>
        ''' <param name="refLoci"></param>
        ''' <param name="RightLimit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function CreateForwardModel(refLoci As Point, RightLimit As Integer) As GraphicsPath
            Dim shape As New GraphicsPath
            Dim leftTop As New Point(refLoci.X, refLoci.Y)
            Dim leftBottom As New Point(refLoci.X, refLoci.Y + Height)

            Dim rightBottom As New Point(refLoci.X + Length - HeadLength, leftBottom.Y)

            If rightBottom.X > RightLimit Then
                rightBottom = New Point(RightLimit, rightBottom.Y)
            End If

            Dim arrowHead As New Point(rightBottom.X + HeadLength, rightBottom.Y - 0.5 * Height)
            Dim rightTop As New Point(rightBottom.X, refLoci.Y)

            Call shape.AddLine(leftTop, leftBottom)
            Call shape.AddLine(leftBottom, rightBottom)
            Call shape.AddLine(rightBottom, arrowHead)
            Call shape.AddLine(arrowHead, rightTop)
            Call shape.AddLine(rightTop, leftTop)

            Return shape
        End Function

        ''' <summary>
        ''' 假若所绘制出来的模型的右部分的坐标超过了<paramref name="RightLimit"></paramref>这个参数，则会被缩短
        ''' </summary>
        ''' <param name="refLoci"></param>
        ''' <param name="RightLimit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function CreateBackwardModel(refLoci As Point, RightLimit As Integer) As GraphicsPath
            Dim shape As New GraphicsPath
            Dim leftTop As New Point(refLoci.X + HeadLength, refLoci.Y)
            Dim arrowHead As New Point(refLoci.X, leftTop.Y + 0.5 * Height)
            Dim leftBottom As New Point(leftTop.X, refLoci.Y + Height)

            Dim rightTop As New Point(refLoci.X + Length, refLoci.Y)

            If rightTop.X > RightLimit Then
                rightTop = New Point(RightLimit, rightTop.Y)
            End If

            Dim rightBottom As New Point(rightTop.X, leftBottom.Y)

            Call shape.AddLine(leftTop, arrowHead)
            Call shape.AddLine(arrowHead, leftBottom)
            Call shape.AddLine(leftBottom, rightBottom)
            Call shape.AddLine(rightTop, rightBottom)
            Call shape.AddLine(rightTop, leftTop)

            Return shape
        End Function
    End Class
End Namespace
