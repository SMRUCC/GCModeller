#Region "Microsoft.VisualBasic::532597956e98b90cf2a490cfd61509f1, ..\visualize\ChromosomeMap\DrawingModels\SegmentObject.vb"

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

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract

Namespace DrawingModels

    ''' <summary>
    ''' 染色体上面的一个基因的绘图模型
    ''' </summary>
    Public Class SegmentObject : Inherits MapModelCommon
        Implements INamedValue
        Implements IGeneBrief

        ''' <summary>
        ''' 基因号
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LocusTag As String Implements ICOGDigest.Key
        ''' <summary>
        ''' 基因名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommonName As String Implements ICOGDigest.COG

        ''' <summary>
        ''' 基因功能注释文字
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Product As String Implements ICOGDigest.Product

        Private Property I_COGEntry_Length As Integer Implements ICOGDigest.Length
        Public Property Location As NucleotideLocation Implements IContig.Location

        Const LocusTagOffset = 20
        Const CommonNameOffset = 15

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  [{1}, {2}]", LocusTag, Left, Right)
        End Function

        Public Shared ReadOnly TextAlignments As Dictionary(Of String, __TextAlignment) =
            New Dictionary(Of String, __TextAlignment) From {
 _
                {"left", AddressOf LeftAligned},
                {"middle", AddressOf MiddleAlignment},
                {"right", AddressOf RightAlignment}
        }

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segnmentLength"></param>
        ''' <param name="headLength"></param>
        ''' <param name="textLength"></param>
        ''' <param name="p"></param>
        ''' <returns>返回字符串的位置信息</returns>
        ''' <remarks></remarks>
        Public Delegate Function __TextAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="segnmentLength">基因对象的图形的绘制长度</param>
        ''' <param name="textLength">使用MeasureString获取得到的字符串的绘制长度</param>
        ''' <param name="p">基因对象额绘制坐标</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function LeftAligned(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Return p
        End Function

        Private Shared Function RightAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            p = New Point(p.X + segnmentLength - textLength, p.Y)
            p = __checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        Private Shared Function __checkRightEndTrimmed(p As Point, textLength As Integer, rightEnds As Integer) As Point
            If p.X + textLength > rightEnds Then
                Dim d = p.X + textLength - rightEnds
                d = p.X - d
                p = New Point(d, p.Y)
            End If

            Return p
        End Function

        Private Shared Function MiddleAlignment(segnmentLength As Integer, headLength As Integer, textLength As Integer, rightEnds As Integer, p As Point) As Point
            Dim d As Integer = (segnmentLength - textLength) / 2
            p = New Point(d + p.X - headLength, p.Y)
            p = __checkRightEndTrimmed(p, textLength, rightEnds)
            Return p
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="location">图形的左上角的坐标</param>
        ''' <returns>返回绘制的图形的大小</returns>
        ''' <remarks></remarks>
        Public Function Draw(g As IGraphics,
                             location As Point,
                             factor As Double,
                             RightLimited As Integer,
                             conf As Configuration.DataReader) As Size

            Dim GraphicPath As GraphicsPath
            Dim LocusTagLocation As Integer = location.X
            Dim Font As Font, size As SizeF

            Font = conf.LocusTagFont

            Me.ConvertFactor = factor

            If Direction < 0 Then
                GraphicPath = CreateBackwardModel(location, RightLimited)
            ElseIf Direction > 0 Then
                GraphicPath = CreateForwardModel(location, RightLimited)
            Else
                GraphicPath = CreateNoneDirectionModel(location, RightLimited)
            End If

            Call g.DrawPath(New Pen(Brushes.Black, 5), GraphicPath)
            Call g.FillPath(Me.Color, GraphicPath)

            size = g.MeasureString(LocusTag, Font)

            Dim MaxLength = System.Math.Max(size.Width, Length)

            If size.Width > Length Then
                LocusTagLocation -= 0.5 * Math.Abs(Length - size.Width)
            Else
                LocusTagLocation += 0.5 * Math.Abs(Length - size.Width)
            End If

            Dim pLocusTagLocation = __checkRightEndTrimmed(New Point(LocusTagLocation, location.Y - size.Height - LocusTagOffset), MaxLength, RightLimited)
            Call g.DrawString(LocusTag, Font, Brushes.Black, pLocusTagLocation)

            size = g.MeasureString(CommonName, Font)
            MaxLength = Math.Max(size.Width, Length)
            LocusTagLocation = location.X
            If size.Width > Length Then
                LocusTagLocation -= 0.5 * Global.System.Math.Abs(Length - size.Width)
            Else
                LocusTagLocation += 0.5 * Global.System.Math.Abs(Length - size.Width)
            End If
            pLocusTagLocation = New Point(LocusTagLocation, pLocusTagLocation.Y + Height + 10 + size.Height + LocusTagOffset)
            Call g.DrawString(Me.CommonName, conf.LocusTagFont, Brushes.Black, pLocusTagLocation)

            Font = New Font("Microsoft YaHei", 6)

            LocusTagLocation = location.X

            If Direction < 0 Then
                LocusTagLocation += (10 + HeadLength)
            End If

            Call g.DrawString(Product, Font, Brushes.DarkOliveGreen, New Point(LocusTagLocation, location.Y + 5 + Height))

#If DEBUG Then
            Call g.DrawString(String.Format("{0} .. {1} KBp", Left / 1000, Right / 1000), Font, Brushes.White, New Point(LocusTagLocation, Location.Y + 0.2 * Height))
#End If
            Return New Size(MaxLength, Height)
        End Function
    End Class
End Namespace
