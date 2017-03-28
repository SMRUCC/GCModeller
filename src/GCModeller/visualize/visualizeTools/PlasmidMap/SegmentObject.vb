#Region "Microsoft.VisualBasic::697022b3cc1e25c4f8d36a179a81dcd3, ..\visualize\visualizeTools\PlasmidMap\SegmentObject.vb"

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

Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic

Namespace PlasmidMap.DrawingModels

    Public Class SegmentObject : Implements INamedValue, IGeneBrief

        ''' <summary>
        ''' 0表示没有方向，1表示正向，-1表示反向
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Direction As Integer
        Public Property Color As System.Drawing.Color
        Public Property CommonName As String Implements IGeneBrief.COG
        Public Property FunctionalAnnotation As String Implements IGeneBrief.Product
        Public Property LocusTag As String Implements IGeneBrief.Key
        Public Property Left As Integer
        Public Property Right As Integer
        Public Property GenomeLength As Integer Implements IGeneBrief.Length

        Private Property Location As NucleotideLocation Implements IContig.Location
            Get
                Return New NucleotideLocation(Left, Right, If(Direction > 0, Strands.Forward, Strands.Reverse))
            End Get
            Set(value As NucleotideLocation)
                Call value.Normalization()
                Left = value.Left
                Right = value.Right
                Direction = value.Strand
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Gr"></param>
        ''' <param name="CenterLocation">图形的左上角的坐标</param>
        ''' <returns>返回绘制的图形的大小</returns>
        ''' <remarks></remarks>
        Public Function Draw(Gr As System.Drawing.Graphics, CenterLocation As System.Drawing.Point, r1 As Integer, r2 As Integer) As Size
            Dim Model As Drawing2D.GraphicsPath
            If Direction = 0 Then
                Model = CreateNoneDirectionModel(CenterLocation, r1, r2)
            ElseIf Direction > 0 Then
                Model = CreateForwardModel(CenterLocation, r1, r2)
            Else
                Model = CreateBackwardModel(CenterLocation, r1, r2)
            End If

            Call Gr.DrawLines(Pens.Black, Model.PathPoints)
            '  Call Gr.FillPath(New SolidBrush(Color), path:=Model)
            Call DrawingStringMethod(Gr, CenterLocation)

            Return Nothing
        End Function

#Region "Creating gene object drawing models"

        ''' <summary>
        ''' 绘制基因编号与基因功能注释
        ''' </summary>
        ''' <param name="Gr"></param>
        ''' <param name="refLoc"></param>
        ''' <remarks></remarks>
        Private Sub DrawingStringMethod(Gr As Graphics, refLoc As Point)

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
        Private Shared Function Internal_getCircleRelativeLocation(p As Integer, TotalLength As Integer, r As Integer, offset As Point) As Point
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
        Protected Function CreateBackwardModel(ReferenceLocation As Point, r1 As Integer, r2 As Integer) As Drawing2D.GraphicsPath
            Throw New NotImplementedException
        End Function

        Protected Function CreateForwardModel(ReferenceLocation As Point, r1 As Integer, r2 As Integer) As Drawing2D.GraphicsPath
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="r">圆的半径</param>
        ''' <param name="refPoint">正方形的中心的坐标</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Internal_createArcBase(r As Integer, refPoint As Point) As Rectangle
            Dim size As Size = New Size(r, r)
            Dim leftTop As Point = New Point(refPoint.X - 0.5 * r, refPoint.Y - 0.5 * r)
            Dim value = New Rectangle(leftTop, size)
            Return value
        End Function

        Protected Function CreateNoneDirectionModel(ReferenceLocation As Point, r1 As Integer, r2 As Integer) As Drawing2D.GraphicsPath
            Dim p1 As Point = Internal_getCircleRelativeLocation(Me.Left, GenomeLength, r1, ReferenceLocation), p3 As Point = Internal_getCircleRelativeLocation(Me.Right, GenomeLength, r1, ReferenceLocation)
            Dim p2 As Point = Internal_getCircleRelativeLocation(Me.Left, GenomeLength, r2, ReferenceLocation), p4 As Point = Internal_getCircleRelativeLocation(Me.Right, GenomeLength, r2, ReferenceLocation)
            Dim Model As Drawing2D.GraphicsPath = New Drawing2D.GraphicsPath(Drawing2D.FillMode.Winding)

            Dim startArc As Single = Me.Left / Me.GenomeLength * FULL_CIRCLE
            Dim endsArc As Single = Me.Right / Me.GenomeLength * FULL_CIRCLE

            Call Model.AddCurve(Internal_createArc(r1, ReferenceLocation, Left, Right, GenomeLength), True)
            ' Call Model.AddPie(Internal_createArcBase(r1, ReferenceLocation), startArc, endsArc) '外圆
            '  Call Model.AddLine(p3, p4) '外圆结束与内圆结束进行连接
            Call Model.AddCurve(Internal_createArc(r2, ReferenceLocation, Left, Right, GenomeLength), True)
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
        Private Shared Function Internal_createArc(r As Integer, refPoint As Point, startArc As Integer, endsArc As Integer, GenomeLength As Integer, Optional d As Double = 1) As Point()
            Dim Path As List(Of Point) = New List(Of Point)

            For i As Integer = startArc To endsArc Step d
                Call Path.Add(Internal_getCircleRelativeLocation(i, GenomeLength, r, refPoint))
            Next

            Return Path.ToArray
        End Function
#End Region
    End Class
End Namespace
