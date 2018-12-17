#Region "Microsoft.VisualBasic::070cd5e6f5cef92d68d41f226c9f721d, ChromosomeMap\PlasmidMap\SegmentObject.vb"

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

'     Class SegmentObject
' 
'         Properties: Color, CommonName, Direction, FunctionalAnnotation, GenomeLength
'                     Left, Location, LocusTag, Right
' 
'         Function: CreateBackwardModel, CreateForwardModel, CreateNoneDirectionModel, Draw, Internal_createArc
'                   Internal_createArcBase, Internal_getCircleRelativeLocation
' 
'         Sub: DrawingStringMethod
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.ChromosomeMap.DrawingModels

Namespace PlasmidMap.DrawingModels

    Public Module DrawGene

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="CenterLocation">图形的左上角的坐标</param>
        ''' <returns>返回绘制的图形的大小</returns>
        ''' <remarks></remarks>
        Public Function Draw(g As IGraphics, centerLocation As Point, gene As SegmentObject, genomeSize%, r1 As Integer, r2 As Integer) As Size
            Dim Model As GraphicsPath

            If gene.Direction = 0 Then
                Model = CreateNoneDirectionModel(centerLocation, gene, genomeSize, r1, r2)
            ElseIf gene.Direction > 0 Then
                Model = CreateForwardModel(centerLocation, r1, r2)
            Else
                Model = CreateBackwardModel(centerLocation, r1, r2)
            End If

            Call g.DrawLines(Pens.Black, Model.PathPoints)
            '  Call Gr.FillPath(New SolidBrush(Color), path:=Model)
            Call DrawingStringMethod(g, centerLocation)

            Return Nothing
        End Function

#Region "Creating gene object drawing models"

        ''' <summary>
        ''' 绘制基因编号与基因功能注释
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="refLoc"></param>
        ''' <remarks></remarks>
        Private Sub DrawingStringMethod(g As IGraphics, refLoc As Point)

        End Sub

        Const FULL_CIRCLE As Double = 360

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="p">序列片段的在基因组序列之上的位点</param>
        ''' <param name="TotalLength">整个基因组的序列总长度</param>
        ''' <param name="r">弧所处的圆的半径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Internal_getCircleRelativeLocation(p As Integer, TotalLength As Integer, r As Integer, offset As Point) As Point
            Dim theta As Double = p / TotalLength * FULL_CIRCLE
            Dim y = r * Math.Cos(theta)
            Dim x = r * Math.Sin(theta)

            Return New Point(x + offset.X, y + offset.Y)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ReferenceLocation"></param>
        ''' <param name="r1">外圈</param>
        ''' <param name="r2">内圆</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CreateBackwardModel(ReferenceLocation As Point, r1 As Integer, r2 As Integer) As GraphicsPath
            Throw New NotImplementedException
        End Function

        Private Function CreateForwardModel(ReferenceLocation As Point, r1 As Integer, r2 As Integer) As GraphicsPath
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="r">圆的半径</param>
        ''' <param name="refPoint">正方形的中心的坐标</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Internal_createArcBase(r As Integer, refPoint As Point) As Rectangle
            Dim size As New Size(r, r)
            Dim leftTop As Point = New Point(refPoint.X - 0.5 * r, refPoint.Y - 0.5 * r)
            Dim value = New Rectangle(leftTop, size)
            Return value
        End Function

        Private Function CreateNoneDirectionModel(ReferenceLocation As Point, gene As SegmentObject, genomeSize%, r1 As Integer, r2 As Integer) As GraphicsPath
            Dim p1 As Point = Internal_getCircleRelativeLocation(gene.Left, genomeSize, r1, ReferenceLocation),
                p3 As Point = Internal_getCircleRelativeLocation(gene.Right, genomeSize, r1, ReferenceLocation)
            Dim p2 As Point = Internal_getCircleRelativeLocation(gene.Left, genomeSize, r2, ReferenceLocation),
                p4 As Point = Internal_getCircleRelativeLocation(gene.Right, genomeSize, r2, ReferenceLocation)
            Dim Model As New GraphicsPath(FillMode.Winding)

            Dim startArc As Single = gene.Left / genomeSize * FULL_CIRCLE
            Dim endsArc As Single = gene.Right / genomeSize * FULL_CIRCLE

            Call Model.AddCurve(Internal_createArc(r1, ReferenceLocation, gene.Left, gene.Right, genomeSize), True)
            ' Call Model.AddPie(Internal_createArcBase(r1, ReferenceLocation), startArc, endsArc) '外圆
            '  Call Model.AddLine(p3, p4) '外圆结束与内圆结束进行连接
            Call Model.AddCurve(Internal_createArc(r2, ReferenceLocation, gene.Left, gene.Right, genomeSize), True)
            'Call Model.AddPie(Internal_createArcBase(r2, ReferenceLocation), endsArc, startArc)  '内圆
            '  Call Model.AddLine(p2, p1)      '内圆开始与外圆开始进行连接

            Return Model
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="r"></param>
        ''' <param name="refPoint"></param>
        ''' <param name="startArc"></param>
        ''' <param name="endsArc"></param>
        ''' <param name="d">步进角度</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Internal_createArc(r As Integer, refPoint As Point, startArc As Integer, endsArc As Integer, GenomeLength As Integer, Optional d As Double = 1) As Point()
            Dim Path As List(Of Point) = New List(Of Point)

            For i As Integer = startArc To endsArc Step d
                Call Path.Add(Internal_getCircleRelativeLocation(i, GenomeLength, r, refPoint))
            Next

            Return Path.ToArray
        End Function
#End Region
    End Module
End Namespace
