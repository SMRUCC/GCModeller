#Region "Microsoft.VisualBasic::22479e8a72e12a3a798330687f480072, GCModeller\visualize\ChromosomeMap\DrawingModels\SegmentObject.vb"

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


    ' Code Statistics:

    '   Total Lines: 152
    '    Code Lines: 95
    ' Comment Lines: 29
    '   Blank Lines: 28
    '     File Size: 5.32 KB


    '     Class SegmentObject
    ' 
    '         Properties: CommonName, I_COGEntry_Length, Location, LocusTag, Product
    ' 
    '         Function: Draw, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.ComponentModel.Annotation
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
        Public Property LocusTag As String Implements IGeneBrief.Key
        ''' <summary>
        ''' 基因名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CommonName As String Implements IFeatureDigest.Feature

        ''' <summary>
        ''' 基因功能注释文字
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Product As String Implements IGeneBrief.Product

        Private Property I_COGEntry_Length As Integer Implements IGeneBrief.Length
        Public Property Location As NucleotideLocation Implements IContig.Location

        Const LocusTagOffset = 20
        Const CommonNameOffset = 15

        Public Overrides Function ToString() As String
            Return String.Format("{0}:  [{1}, {2}]", LocusTag, Left, Right)
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
                             locusTagFont As Font,
                             Optional drawLocusTag As Boolean = True,
                             Optional showInfo As Boolean = False,
                             Optional drawShapeStroke As Boolean = True) As Size

            ' 基因对象的绘制图形
            Dim shape As GraphicsPath
            Dim locusTagLocation As Integer = location.X
            Dim font As Font = locusTagFont
            Dim size As SizeF

            Me.ConvertFactor = factor

            If Direction < 0 Then
                shape = CreateBackwardModel(location, RightLimited)
            ElseIf Direction > 0 Then
                shape = CreateForwardModel(location, RightLimited)
            Else
                shape = CreateNoneDirectionModel(location, RightLimited)
            End If

            If drawShapeStroke Then
                Call g.DrawPath(New Pen(Brushes.Black, 5), shape)
            End If

            Call g.FillPath(Me.Color, shape)

            size = g.MeasureString(LocusTag, font)

            Dim maxLen! = Math.Max(size.Width, Length)

            If size.Width > Length Then
                locusTagLocation -= 0.5 * Math.Abs(Length - size.Width)
            Else
                locusTagLocation += 0.5 * Math.Abs(Length - size.Width)
            End If

            Dim locusTagPoint As New Point With {
                .X = locusTagLocation,
                .Y = location.Y - size.Height - LocusTagOffset
            }
            locusTagPoint = checkRightEndTrimmed(locusTagPoint, maxLen, RightLimited)

            If drawLocusTag Then
                Call g.DrawString(LocusTag, font, Brushes.Black, locusTagPoint)
            End If

            size = g.MeasureString(CommonName, font)
            maxLen = Math.Max(size.Width, Length)
            locusTagLocation = location.X

            If size.Width > Length Then
                locusTagLocation -= 0.5 * Math.Abs(Length - size.Width)
            Else
                locusTagLocation += 0.5 * Math.Abs(Length - size.Width)
            End If

            locusTagPoint = New Point With {
                .X = locusTagLocation,
                .Y = locusTagPoint.Y + Height + 10 + size.Height + LocusTagOffset
            }

            Call g.DrawString(Me.CommonName, locusTagFont, Brushes.Black, locusTagPoint)

            font = New Font("Microsoft YaHei", 6)

            locusTagLocation = location.X

            If Direction < 0 Then
                locusTagLocation += (10 + HeadLength)
            End If

            If showInfo Then
                locusTagPoint = New Point With {
                    .X = locusTagLocation,
                    .Y = location.Y + 5 + Height
                }
                g.DrawString(Product, font, Brushes.DarkOliveGreen, locusTagPoint)
            End If

#If DEBUG Then
            Dim debugText$ = $"{Left / 1000} .. {Right / 1000} KBp"
            Dim debugX = locusTagLocation
            Dim debugY = location.Y + 0.2 * Height

            Call g.DrawString(debugText, font, Brushes.White, New Point(debugX, debugY))
#End If
            Return New Size(maxLen, Height)
        End Function
    End Class
End Namespace
