#Region "Microsoft.VisualBasic::1f51b2cb7fe23bf438cc8e7c7f55bbcc, ..\GCModeller\visualize\SVG.Extensions\Graphics.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Graphics
Imports System.Drawing.Imaging
Imports System.Drawing.Text

Public Class Graphics

    Implements IDeviceContext
    Implements System.IDisposable

    Public ReadOnly Property SVGHandle As SVG.SvgDocument


#Region "Implements Class Graphics"

    ''
    '' Summary:
    ''     Gets or sets a System.Drawing.Region that limits the drawing region of this System.Drawing.Graphics.
    ''
    '' Returns:
    ''     A System.Drawing.Region that limits the portion of this System.Drawing.Graphics
    ''     that is currently available for drawing.
    'Public Property Clip As Region
    '    Get
    '        Return Gr_Device.Clip
    '    End Get
    '    Set(value As Region)
    '        Gr_Device.Clip = value
    '    End Set
    'End Property
    '
    ' Summary:
    '     Gets a System.Drawing.RectangleF structure that bounds the clipping region of
    '     this System.Drawing.Graphics.
    '
    ' Returns:
    '     A System.Drawing.RectangleF structure that represents a bounding rectangle for
    '     the clipping region of this System.Drawing.Graphics.
    Public ReadOnly Property ClipBounds As RectangleF
        Get

        End Get
    End Property

    ''' <summary>
    ''' Gets a value that specifies how composited images are drawn to this System.Drawing.Graphics.
    ''' </summary>
    ''' <returns>
    ''' This property specifies a member of the System.Drawing.Drawing2D.CompositingMode enumeration. 
    ''' The default is System.Drawing.Drawing2D.CompositingMode.SourceOver.
    ''' </returns>
    Public Property CompositingMode As CompositingMode
        Get

        End Get
        Set(value As CompositingMode)

        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the rendering quality of composited images drawn to this System.Drawing.Graphics.
    ''' </summary>
    ''' <returns>
    ''' This property specifies a member of the System.Drawing.Drawing2D.CompositingQuality enumeration. 
    ''' The default is System.Drawing.Drawing2D.CompositingQuality.Default.
    ''' </returns>
    Public Property CompositingQuality As CompositingQuality
        Get

        End Get
        Set(value As CompositingQuality)

        End Set
    End Property

    ''' <summary>
    ''' Gets the horizontal resolution of this System.Drawing.Graphics.
    ''' </summary>
    ''' <returns>
    ''' The value, in dots per inch, for the horizontal resolution supported by this System.Drawing.Graphics.
    ''' </returns>
    Public ReadOnly Property DpiX As Single
        Get

        End Get
    End Property
    '
    ' Summary:
    '     Gets the vertical resolution of this System.Drawing.Graphics.
    '
    ' Returns:
    '     The value, in dots per inch, for the vertical resolution supported by this System.Drawing.Graphics.
    Public ReadOnly Property DpiY As Single
        Get

        End Get
    End Property

    ''' <summary>
    ''' Gets or sets the interpolation mode associated with this System.Drawing.Graphics.
    ''' </summary>
    ''' <returns>One of the System.Drawing.Drawing2D.InterpolationMode values.</returns>
    Public Property InterpolationMode As InterpolationMode
        Get

        End Get
        Set(value As InterpolationMode)

        End Set
    End Property
    '
    ' Summary:
    '     Gets a value indicating whether the clipping region of this System.Drawing.Graphics
    '     is empty.
    '
    ' Returns:
    '     true if the clipping region of this System.Drawing.Graphics is empty; otherwise,
    '     false.
    Public ReadOnly Property IsClipEmpty As Boolean
        Get

        End Get
    End Property
    '
    ' Summary:
    '     Gets a value indicating whether the visible clipping region of this System.Drawing.Graphics
    '     is empty.
    '
    ' Returns:
    '     true if the visible portion of the clipping region of this System.Drawing.Graphics
    '     is empty; otherwise, false.
    Public ReadOnly Property IsVisibleClipEmpty As Boolean
        Get

        End Get
    End Property
    '
    ' Summary:
    '     Gets or sets the scaling between world units and page units for this System.Drawing.Graphics.
    '
    ' Returns:
    '     This property specifies a value for the scaling between world units and page
    '     units for this System.Drawing.Graphics.
    Public Property PageScale As Single
        Get

        End Get
        Set(value As Single)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets the unit of measure used for page coordinates in this System.Drawing.Graphics.
    '
    ' Returns:
    '     One of the System.Drawing.GraphicsUnit values other than System.Drawing.GraphicsUnit.World.
    '
    ' Exceptions:
    '   T:System.ComponentModel.InvalidEnumArgumentException:
    '     System.Drawing.Graphics.PageUnit is set to System.Drawing.GraphicsUnit.World,
    '     which is not a physical unit.
    Public Property PageUnit As GraphicsUnit
        Get

        End Get
        Set(value As GraphicsUnit)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or set a value specifying how pixels are offset during rendering of this
    '     System.Drawing.Graphics.
    '
    ' Returns:
    '     This property specifies a member of the System.Drawing.Drawing2D.PixelOffsetMode
    '     enumeration
    Public Property PixelOffsetMode As PixelOffsetMode
        Get

        End Get
        Set(value As PixelOffsetMode)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets the rendering origin of this System.Drawing.Graphics for dithering
    '     and for hatch brushes.
    '
    ' Returns:
    '     A System.Drawing.Point structure that represents the dither origin for 8-bits-per-pixel
    '     and 16-bits-per-pixel dithering and is also used to set the origin for hatch
    '     brushes.
    Public Property RenderingOrigin As Point
        Get

        End Get
        Set(value As Point)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets the rendering quality for this System.Drawing.Graphics.
    '
    ' Returns:
    '     One of the System.Drawing.Drawing2D.SmoothingMode values.
    Public Property SmoothingMode As SmoothingMode
        Get

        End Get
        Set(value As SmoothingMode)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets the gamma correction value for rendering text.
    '
    ' Returns:
    '     The gamma correction value used for rendering antialiased and ClearType text.
    Public Property TextContrast As Integer
        Get

        End Get
        Set(value As Integer)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets the rendering mode for text associated with this System.Drawing.Graphics.
    '
    ' Returns:
    '     One of the System.Drawing.Text.TextRenderingHint values.
    Public Property TextRenderingHint As TextRenderingHint
        Get

        End Get
        Set(value As TextRenderingHint)

        End Set
    End Property
    '
    ' Summary:
    '     Gets or sets a copy of the geometric world transformation for this System.Drawing.Graphics.
    '
    ' Returns:
    '     A copy of the System.Drawing.Drawing2D.Matrix that represents the geometric world
    '     transformation for this System.Drawing.Graphics.
    Public Property Transform As Matrix
        Get

        End Get
        Set(value As Matrix)

        End Set
    End Property
    '
    ' Summary:
    '     Gets the bounding rectangle of the visible clipping region of this System.Drawing.Graphics.
    '
    ' Returns:
    '     A System.Drawing.RectangleF structure that represents a bounding rectangle for
    '     the visible clipping region of this System.Drawing.Graphics.
    Public ReadOnly Property VisibleClipBounds As RectangleF
        Get

        End Get
    End Property

    '
    ' Summary:
    '     Adds a comment to the current System.Drawing.Imaging.Metafile.
    '
    ' Parameters:
    '   data:
    '     Array of bytes that contains the comment.
    Public Sub AddMetafileComment(data() As Byte)

    End Sub
    '
    ' Summary:
    '     Clears the entire drawing surface and fills it with the specified background
    '     color.
    '
    ' Parameters:
    '   color:
    '     System.Drawing.Color structure that represents the background color of the drawing
    '     surface.
    Public Sub Clear(color As Color)

    End Sub
    '
    ' Summary:
    '     Performs a bit-block transfer of color data, corresponding to a rectangle of
    '     pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
    '
    ' Parameters:
    '   upperLeftSource:
    '     The point at the upper-left corner of the source rectangle.
    '
    '   upperLeftDestination:
    '     The point at the upper-left corner of the destination rectangle.
    '
    '   blockRegionSize:
    '     The size of the area to be transferred.
    '
    ' Exceptions:
    '   T:System.ComponentModel.Win32Exception:
    '     The operation failed.
    Public Sub CopyFromScreen(upperLeftSource As Point, upperLeftDestination As Point, blockRegionSize As Size)

    End Sub
    '
    ' Summary:
    '     Performs a bit-block transfer of color data, corresponding to a rectangle of
    '     pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
    '
    ' Parameters:
    '   upperLeftSource:
    '     The point at the upper-left corner of the source rectangle.
    '
    '   upperLeftDestination:
    '     The point at the upper-left corner of the destination rectangle.
    '
    '   blockRegionSize:
    '     The size of the area to be transferred.
    '
    '   copyPixelOperation:
    '     One of the System.Drawing.CopyPixelOperation values.
    '
    ' Exceptions:
    '   T:System.ComponentModel.InvalidEnumArgumentException:
    '     copyPixelOperation is not a member of System.Drawing.CopyPixelOperation.
    '
    '   T:System.ComponentModel.Win32Exception:
    '     The operation failed.
    Public Sub CopyFromScreen(upperLeftSource As Point, upperLeftDestination As Point, blockRegionSize As Size, copyPixelOperation As CopyPixelOperation)

    End Sub
    '
    ' Summary:
    '     Performs a bit-block transfer of the color data, corresponding to a rectangle
    '     of pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
    '
    ' Parameters:
    '   sourceX:
    '     The x-coordinate of the point at the upper-left corner of the source rectangle.
    '
    '   sourceY:
    '     The y-coordinate of the point at the upper-left corner of the source rectangle.
    '
    '   destinationX:
    '     The x-coordinate of the point at the upper-left corner of the destination rectangle.
    '
    '   destinationY:
    '     The y-coordinate of the point at the upper-left corner of the destination rectangle.
    '
    '   blockRegionSize:
    '     The size of the area to be transferred.
    '
    ' Exceptions:
    '   T:System.ComponentModel.Win32Exception:
    '     The operation failed.
    Public Sub CopyFromScreen(sourceX As Integer, sourceY As Integer, destinationX As Integer, destinationY As Integer, blockRegionSize As Size)

    End Sub
    '
    ' Summary:
    '     Performs a bit-block transfer of the color data, corresponding to a rectangle
    '     of pixels, from the screen to the drawing surface of the System.Drawing.Graphics.
    '
    ' Parameters:
    '   sourceX:
    '     The x-coordinate of the point at the upper-left corner of the source rectangle.
    '
    '   sourceY:
    '     The y-coordinate of the point at the upper-left corner of the source rectangle
    '
    '   destinationX:
    '     The x-coordinate of the point at the upper-left corner of the destination rectangle.
    '
    '   destinationY:
    '     The y-coordinate of the point at the upper-left corner of the destination rectangle.
    '
    '   blockRegionSize:
    '     The size of the area to be transferred.
    '
    '   copyPixelOperation:
    '     One of the System.Drawing.CopyPixelOperation values.
    '
    ' Exceptions:
    '   T:System.ComponentModel.InvalidEnumArgumentException:
    '     copyPixelOperation is not a member of System.Drawing.CopyPixelOperation.
    '
    '   T:System.ComponentModel.Win32Exception:
    '     The operation failed.
    Public Sub CopyFromScreen(sourceX As Integer, sourceY As Integer, destinationX As Integer, destinationY As Integer, blockRegionSize As Size, copyPixelOperation As CopyPixelOperation)

    End Sub

    '
    ' Summary:
    '     Draws an arc representing a portion of an ellipse specified by a System.Drawing.Rectangle
    '     structure.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the arc.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the starting point of
    '     the arc.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to ending point
    '     of the arc.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawArc(pen As Pen, rect As Rectangle, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws an arc representing a portion of an ellipse specified by a System.Drawing.RectangleF
    '     structure.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the arc.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the starting point of
    '     the arc.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to ending point
    '     of the arc.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null
    Public Sub DrawArc(pen As Pen, rect As RectangleF, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws an arc representing a portion of an ellipse specified by a pair of coordinates,
    '     a width, and a height.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the arc.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
    '
    '   width:
    '     Width of the rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the rectangle that defines the ellipse.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the starting point of
    '     the arc.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to ending point
    '     of the arc.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-rects is null.
    '
    '   T:System.ArgumentNullException:
    '     rects is a zero-length array.
    Public Sub DrawArc(pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer, startAngle As Integer, sweepAngle As Integer)

    End Sub
    '
    ' Summary:
    '     Draws an arc representing a portion of an ellipse specified by a pair of coordinates,
    '     a width, and a height.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the arc.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle that defines the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle that defines the ellipse.
    '
    '   width:
    '     Width of the rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the rectangle that defines the ellipse.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the starting point of
    '     the arc.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to ending point
    '     of the arc.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawArc(pen As Pen, x As Single, y As Single, width As Single, height As Single, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws a Bézier spline defined by four System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen structure that determines the color, width, and style of the
    '     curve.
    '
    '   pt1:
    '     System.Drawing.Point structure that represents the starting point of the curve.
    '
    '   pt2:
    '     System.Drawing.Point structure that represents the first control point for the
    '     curve.
    '
    '   pt3:
    '     System.Drawing.Point structure that represents the second control point for the
    '     curve.
    '
    '   pt4:
    '     System.Drawing.Point structure that represents the ending point of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawBezier(pen As Pen, pt1 As Point, pt2 As Point, pt3 As Point, pt4 As Point)

    End Sub
    '

    ' Summary:
    '     Draws a Bézier spline defined by four System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   pt1:
    '     System.Drawing.PointF structure that represents the starting point of the curve.
    '
    '   pt2:
    '     System.Drawing.PointF structure that represents the first control point for the
    '     curve.
    '
    '   pt3:
    '     System.Drawing.PointF structure that represents the second control point for
    '     the curve.
    '
    '   pt4:
    '     System.Drawing.PointF structure that represents the ending point of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawBezier(pen As Pen, pt1 As PointF, pt2 As PointF, pt3 As PointF, pt4 As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a Bézier spline defined by four ordered pairs of coordinates that represent
    '     points.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   x1:
    '     The x-coordinate of the starting point of the curve.
    '
    '   y1:
    '     The y-coordinate of the starting point of the curve.
    '
    '   x2:
    '     The x-coordinate of the first control point of the curve.
    '
    '   y2:
    '     The y-coordinate of the first control point of the curve.
    '
    '   x3:
    '     The x-coordinate of the second control point of the curve.
    '
    '   y3:
    '     The y-coordinate of the second control point of the curve.
    '
    '   x4:
    '     The x-coordinate of the ending point of the curve.
    '
    '   y4:
    '     The y-coordinate of the ending point of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawBezier(pen As Pen, x1 As Single, y1 As Single, x2 As Single, y2 As Single, x3 As Single, y3 As Single, x4 As Single, y4 As Single)

    End Sub
    '
    ' Summary:
    '     Draws a series of Bézier splines from an array of System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that represent the points that determine
    '     the curve. The number of points in the array should be a multiple of 3 plus 1,
    '     such as 4, 7, or 10.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawBeziers(pen As Pen, points() As Point)

    End Sub
    '
    ' Summary:
    '     Draws a series of Bézier splines from an array of System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the points that determine
    '     the curve. The number of points in the array should be a multiple of 3 plus 1,
    '     such as 4, 7, or 10.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawBeziers(pen As Pen, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a closed cardinal spline defined by an array of System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and height of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawClosedCurve(pen As Pen, points() As Point)

    End Sub
    '
    ' Summary:
    '     Draws a closed cardinal spline defined by an array of System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and height of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawClosedCurve(pen As Pen, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a closed cardinal spline defined by an array of System.Drawing.Point structures
    '     using a specified tension.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and height of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled. This parameter is required but ignored.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawClosedCurve(pen As Pen, points() As Point, tension As Single, fillmode As FillMode)

    End Sub
    '
    ' Summary:
    '     Draws a closed cardinal spline defined by an array of System.Drawing.PointF structures
    '     using a specified tension.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and height of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled. This parameter is required but is ignored.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawClosedCurve(pen As Pen, points() As PointF, tension As Single, fillmode As FillMode)

    End Sub
    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and height of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As Point)

    End Sub
    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As PointF)

    End Sub

    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.Point structures
    '     using a specified tension.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As Point, tension As Single)

    End Sub

    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.PointF structures
    '     using a specified tension.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the points that define
    '     the curve.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As PointF, tension As Single)

    End Sub

    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.PointF structures.
    '     The drawing begins offset from the beginning of the array.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    '   offset:
    '     Offset from the first element in the array of the points parameter to the starting
    '     point in the curve.
    '
    '   numberOfSegments:
    '     Number of segments after the starting point to include in the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As PointF, offset As Integer, numberOfSegments As Integer)

    End Sub

    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.Point structures
    '     using a specified tension.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    '   offset:
    '     Offset from the first element in the array of the points parameter to the starting
    '     point in the curve.
    '
    '   numberOfSegments:
    '     Number of segments after the starting point to include in the curve.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As Point, offset As Integer, numberOfSegments As Integer, tension As Single)

    End Sub


    '
    ' Summary:
    '     Draws a cardinal spline through a specified array of System.Drawing.PointF structures
    '     using a specified tension. The drawing begins offset from the beginning of the
    '     array.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the curve.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    '   offset:
    '     Offset from the first element in the array of the points parameter to the starting
    '     point in the curve.
    '
    '   numberOfSegments:
    '     Number of segments after the starting point to include in the curve.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawCurve(pen As Pen, points() As PointF, offset As Integer, numberOfSegments As Integer, tension As Single)

    End Sub
    '
    ' Summary:
    '     Draws an ellipse specified by a bounding System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the ellipse.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that defines the boundaries of the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Function DrawEllipse(pen As Pen, rect As Rectangle) As SvgEllipse
        Dim center As Point = rect.Center
        Dim es As New SVG.SvgEllipse With {
            .CenterX = New SvgUnit(center.X),
            .CenterY = New SvgUnit(center.Y),
            .Opacity = pen.Color.A / 255
        }
        Call SVGHandle.Children.Add(es)

        Return es
    End Function
    '
    ' Summary:
    '     Draws an ellipse defined by a bounding System.Drawing.RectangleF.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the ellipse.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that defines the boundaries of the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawEllipse(pen As Pen, rect As RectangleF)

    End Sub
    '
    ' Summary:
    '     Draws an ellipse defined by a bounding rectangle specified by coordinates for
    '     the upper-left corner of the rectangle, a height, and a width.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the ellipse.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawEllipse(pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer)

    End Sub
    '
    ' Summary:
    '     Draws an ellipse defined by a bounding rectangle specified by a pair of coordinates,
    '     a height, and a width.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the ellipse.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawEllipse(pen As Pen, x As Single, y As Single, width As Single, height As Single)

    End Sub
    '
    ' Summary:
    '     Draws the image represented by the specified System.Drawing.Icon within the area
    '     specified by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   icon:
    '     System.Drawing.Icon to draw.
    '
    '   targetRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     resulting image on the display surface. The image contained in the icon parameter
    '     is scaled to the dimensions of this rectangular area.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     icon is null.
    Public Sub DrawIcon(icon As Icon, targetRect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws the image represented by the specified System.Drawing.Icon at the specified
    '     coordinates.
    '
    ' Parameters:
    '   icon:
    '     System.Drawing.Icon to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     icon is null.
    Public Sub DrawIcon(icon As Icon, x As Integer, y As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the image represented by the specified System.Drawing.Icon without scaling
    '     the image.
    '
    ' Parameters:
    '   icon:
    '     System.Drawing.Icon to draw.
    '
    '   targetRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     resulting image. The image is not scaled to fit this rectangle, but retains its
    '     original size. If the image is larger than the rectangle, it is clipped to fit
    '     inside it.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     icon is null.
    Public Sub DrawIconUnstretched(icon As Icon, targetRect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, rect As RectangleF)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified shape and size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As Point)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified shape and size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As PointF)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image, using its original physical size, at
    '     the specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   point:
    '     System.Drawing.Point structure that represents the location of the upper-left
    '     corner of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, point As Point)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image, using its original physical size, at
    '     the specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   point:
    '     System.Drawing.PointF structure that represents the upper-left corner of the
    '     drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, point As PointF)

    End Sub
    '
    ' Summary:
    '     Draws the specified image, using its original physical size, at the location
    '     specified by a coordinate pair.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Integer, y As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image, using its original physical size, at
    '     the specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Single, y As Single)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcRect As Rectangle, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As RectangleF, srcRect As RectangleF, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws a portion of an image at a specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the System.Drawing.Image
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Single, y As Single, srcRect As RectangleF, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Draws a portion of an image at a specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Integer, y As Integer, srcRect As Rectangle, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    '   width:
    '     Width of the drawn image.
    '
    '   height:
    '     Height of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Integer, y As Integer, width As Integer, height As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the specified System.Drawing.Image at the specified location and with the
    '     specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    '   width:
    '     Width of the drawn image.
    '
    '   height:
    '     Height of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, x As Single, y As Single, width As Single, height As Single)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
    '     method according to application-determined criteria.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, imageAttr As ImageAttributes, callback As DrawImageAbort)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
    '     method according to application-determined criteria.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, imageAttr As ImageAttributes, callback As DrawImageAbort)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Integer, srcY As Integer, srcWidth As Integer, srcHeight As Integer, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
    '     method according to application-determined criteria.
    '
    '   callbackData:
    '     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
    '     delegate to use when checking whether to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Point[],System.Drawing.Rectangle,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
    '     method.
    Public Sub DrawImage(image As Image, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, imageAttr As ImageAttributes, callback As DrawImageAbort, callbackData As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Single, srcY As Single, srcWidth As Single, srcHeight As Single, srcUnit As GraphicsUnit)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the image object
    '     to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used by the srcRect parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
    '     method according to application-determined criteria.
    '
    '   callbackData:
    '     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
    '     delegate to use when checking whether to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.PointF[],System.Drawing.RectangleF,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.Int32)
    '     method.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, imageAttr As ImageAttributes, callback As DrawImageAbort, callbackData As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Integer, srcY As Integer, srcWidth As Integer, srcHeight As Integer, srcUnit As GraphicsUnit, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttrs:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Single, srcY As Single, srcWidth As Single, srcHeight As Single, srcUnit As GraphicsUnit, imageAttrs As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for image.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
    '     method according to application-determined criteria.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Integer, srcY As Integer, srcWidth As Integer, srcHeight As Integer, srcUnit As GraphicsUnit, imageAttr As ImageAttributes, callback As DrawImageAbort)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttrs:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort)
    '     method according to application-determined criteria.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Single, srcY As Single, srcWidth As Single, srcHeight As Single, srcUnit As GraphicsUnit, imageAttrs As ImageAttributes, callback As DrawImageAbort)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttrs:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Int32,System.Int32,System.Int32,System.Int32,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)
    '     method according to application-determined criteria.
    '
    '   callbackData:
    '     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
    '     delegate to use when checking whether to stop execution of the DrawImage method.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Integer, srcY As Integer, srcWidth As Integer, srcHeight As Integer, srcUnit As GraphicsUnit, imageAttrs As ImageAttributes, callback As DrawImageAbort, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Draws the specified portion of the specified System.Drawing.Image at the specified
    '     location and with the specified size.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn image. The image is scaled to fit the rectangle.
    '
    '   srcX:
    '     The x-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcY:
    '     The y-coordinate of the upper-left corner of the portion of the source image
    '     to draw.
    '
    '   srcWidth:
    '     Width of the portion of the source image to draw.
    '
    '   srcHeight:
    '     Height of the portion of the source image to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the units
    '     of measure used to determine the source rectangle.
    '
    '   imageAttrs:
    '     System.Drawing.Imaging.ImageAttributes that specifies recoloring and gamma information
    '     for the image object.
    '
    '   callback:
    '     System.Drawing.Graphics.DrawImageAbort delegate that specifies a method to call
    '     during the drawing of the image. This method is called frequently to check whether
    '     to stop execution of the System.Drawing.Graphics.DrawImage(System.Drawing.Image,System.Drawing.Rectangle,System.Single,System.Single,System.Single,System.Single,System.Drawing.GraphicsUnit,System.Drawing.Imaging.ImageAttributes,System.Drawing.Graphics.DrawImageAbort,System.IntPtr)
    '     method according to application-determined criteria.
    '
    '   callbackData:
    '     Value specifying additional data for the System.Drawing.Graphics.DrawImageAbort
    '     delegate to use when checking whether to stop execution of the DrawImage method.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImage(image As Image, destRect As Rectangle, srcX As Single, srcY As Single, srcWidth As Single, srcHeight As Single, srcUnit As GraphicsUnit, imageAttrs As ImageAttributes, callback As DrawImageAbort, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Draws a specified image using its original physical size at a specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   rect:
    '     System.Drawing.Rectangle that specifies the upper-left corner of the drawn image.
    '     The X and Y properties of the rectangle specify the upper-left corner. The Width
    '     and Height properties are ignored.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImageUnscaled(image As Image, rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws a specified image using its original physical size at a specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   point:
    '     System.Drawing.Point structure that specifies the upper-left corner of the drawn
    '     image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImageUnscaled(image As Image, point As Point)

    End Sub
    '
    ' Summary:
    '     Draws the specified image using its original physical size at the location specified
    '     by a coordinate pair.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImageUnscaled(image As Image, x As Integer, y As Integer)

    End Sub
    '
    ' Summary:
    '     Draws a specified image using its original physical size at a specified location.
    '
    ' Parameters:
    '   image:
    '     System.Drawing.Image to draw.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn image.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn image.
    '
    '   width:
    '     Not used.
    '
    '   height:
    '     Not used.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImageUnscaled(image As Image, x As Integer, y As Integer, width As Integer, height As Integer)

    End Sub
    '
    ' Summary:
    '     Draws the specified image without scaling and clips it, if necessary, to fit
    '     in the specified rectangle.
    '
    ' Parameters:
    '   image:
    '     The System.Drawing.Image to draw.
    '
    '   rect:
    '     The System.Drawing.Rectangle in which to draw the image.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     image is null.
    Public Sub DrawImageUnscaledAndClipped(image As Image, rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws a line connecting two System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line.
    '
    '   pt1:
    '     System.Drawing.Point structure that represents the first point to connect.
    '
    '   pt2:
    '     System.Drawing.Point structure that represents the second point to connect.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawLine(pen As Pen, pt1 As Point, pt2 As Point)

    End Sub
    '
    ' Summary:
    '     Draws a line connecting two System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line.
    '
    '   pt1:
    '     System.Drawing.PointF structure that represents the first point to connect.
    '
    '   pt2:
    '     System.Drawing.PointF structure that represents the second point to connect.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawLine(pen As Pen, pt1 As PointF, pt2 As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a line connecting the two points specified by the coordinate pairs.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line.
    '
    '   x1:
    '     The x-coordinate of the first point.
    '
    '   y1:
    '     The y-coordinate of the first point.
    '
    '   x2:
    '     The x-coordinate of the second point.
    '
    '   y2:
    '     The y-coordinate of the second point.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawLine(pen As Pen, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer)

    End Sub
    '
    ' Summary:
    '     Draws a line connecting the two points specified by the coordinate pairs.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line.
    '
    '   x1:
    '     The x-coordinate of the first point.
    '
    '   y1:
    '     The y-coordinate of the first point.
    '
    '   x2:
    '     The x-coordinate of the second point.
    '
    '   y2:
    '     The y-coordinate of the second point.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawLine(pen As Pen, x1 As Single, y1 As Single, x2 As Single, y2 As Single)

    End Sub
    '
    ' Summary:
    '     Draws a series of line segments that connect an array of System.Drawing.Point
    '     structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line segments.
    '
    '   points:
    '     Array of System.Drawing.Point structures that represent the points to connect.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawLines(pen As Pen, points() As Point)

    End Sub
    '
    ' Summary:
    '     Draws a series of line segments that connect an array of System.Drawing.PointF
    '     structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the line segments.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the points to connect.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawLines(pen As Pen, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a System.Drawing.Drawing2D.GraphicsPath.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the path.
    '
    '   path:
    '     System.Drawing.Drawing2D.GraphicsPath to draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-path is null.
    Public Sub DrawPath(pen As Pen, path As GraphicsPath)

    End Sub
    '
    ' Summary:
    '     Draws a pie shape defined by an ellipse specified by a System.Drawing.Rectangle
    '     structure and two radial lines.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the pie shape.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that represents the bounding rectangle that
    '     defines the ellipse from which the pie shape comes.
    '
    '   startAngle:
    '     Angle measured in degrees clockwise from the x-axis to the first side of the
    '     pie shape.
    '
    '   sweepAngle:
    '     Angle measured in degrees clockwise from the startAngle parameter to the second
    '     side of the pie shape.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawPie(pen As Pen, rect As Rectangle, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws a pie shape defined by an ellipse specified by a System.Drawing.RectangleF
    '     structure and two radial lines.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the pie shape.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that represents the bounding rectangle that
    '     defines the ellipse from which the pie shape comes.
    '
    '   startAngle:
    '     Angle measured in degrees clockwise from the x-axis to the first side of the
    '     pie shape.
    '
    '   sweepAngle:
    '     Angle measured in degrees clockwise from the startAngle parameter to the second
    '     side of the pie shape.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawPie(pen As Pen, rect As RectangleF, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws a pie shape defined by an ellipse specified by a coordinate pair, a width,
    '     a height, and two radial lines.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the pie shape.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie shape comes.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie shape comes.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse from which the pie shape
    '     comes.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse from which the pie
    '     shape comes.
    '
    '   startAngle:
    '     Angle measured in degrees clockwise from the x-axis to the first side of the
    '     pie shape.
    '
    '   sweepAngle:
    '     Angle measured in degrees clockwise from the startAngle parameter to the second
    '     side of the pie shape.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawPie(pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer, startAngle As Integer, sweepAngle As Integer)

    End Sub
    '
    ' Summary:
    '     Draws a pie shape defined by an ellipse specified by a coordinate pair, a width,
    '     a height, and two radial lines.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the pie shape.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie shape comes.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie shape comes.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse from which the pie shape
    '     comes.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse from which the pie
    '     shape comes.
    '
    '   startAngle:
    '     Angle measured in degrees clockwise from the x-axis to the first side of the
    '     pie shape.
    '
    '   sweepAngle:
    '     Angle measured in degrees clockwise from the startAngle parameter to the second
    '     side of the pie shape.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.

    Public Sub DrawPie(pen As Pen, x As Single, y As Single, width As Single, height As Single, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Draws a polygon defined by an array of System.Drawing.Point structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the polygon.
    '
    '   points:
    '     Array of System.Drawing.Point structures that represent the vertices of the polygon.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawPolygon(pen As Pen, points() As Point)

    End Sub
    '
    ' Summary:
    '     Draws a polygon defined by an array of System.Drawing.PointF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the polygon.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the vertices of the
    '     polygon.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-points is null.
    Public Sub DrawPolygon(pen As Pen, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Draws a rectangle specified by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   pen:
    '     A System.Drawing.Pen that determines the color, width, and style of the rectangle.
    '
    '   rect:
    '     A System.Drawing.Rectangle structure that represents the rectangle to draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawRectangle(pen As Pen, rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws a rectangle specified by a coordinate pair, a width, and a height.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the rectangle.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to draw.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to draw.
    '
    '   width:
    '     Width of the rectangle to draw.
    '
    '   height:
    '     Height of the rectangle to draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawRectangle(pen As Pen, x As Integer, y As Integer, width As Integer, height As Integer)

    End Sub
    '
    ' Summary:
    '     Draws a rectangle specified by a coordinate pair, a width, and a height.
    '
    ' Parameters:
    '   pen:
    '     A System.Drawing.Pen that determines the color, width, and style of the rectangle.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to draw.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to draw.
    '
    '   width:
    '     The width of the rectangle to draw.
    '
    '   height:
    '     The height of the rectangle to draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.
    Public Sub DrawRectangle(pen As Pen, x As Single, y As Single, width As Single, height As Single)

    End Sub
    '
    ' Summary:
    '     Draws a series of rectangles specified by System.Drawing.Rectangle structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the outlines
    '     of the rectangles.
    '
    '   rects:
    '     Array of System.Drawing.Rectangle structures that represent the rectangles to
    '     draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-rects is null.
    '
    '   T:System.ArgumentException:
    '     rects is a zero-length array.
    Public Sub DrawRectangles(pen As Pen, rects() As Rectangle)

    End Sub
    '
    ' Summary:
    '     Draws a series of rectangles specified by System.Drawing.RectangleF structures.
    '
    ' Parameters:
    '   pen:
    '     System.Drawing.Pen that determines the color, width, and style of the outlines
    '     of the rectangles.
    '
    '   rects:
    '     Array of System.Drawing.RectangleF structures that represent the rectangles to
    '     draw.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     pen is null.-or-rects is null.
    '
    '   T:System.ArgumentException:
    '     rects is a zero-length array.
    Public Sub DrawRectangles(pen As Pen, rects() As RectangleF)

    End Sub
    '
    ' Summary:
    '     Draws the specified text string in the specified rectangle with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   layoutRectangle:
    '     System.Drawing.RectangleF structure that specifies the location of the drawn
    '     text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Function DrawString(s As String, font As Font, brush As Brush, layoutRectangle As RectangleF) As SvgText
        Dim txt As New SvgText(s)
        If TypeOf brush Is SolidBrush Then
            txt.Color = New SvgColourServer(DirectCast(brush, SolidBrush).Color)
        End If
        txt.FontFamily = font.FontFamily.Name
        txt.FontSize = font.Size.SVGUnit
        txt.FontStyle = font.Style.SVGFontStyle

        Return txt
    End Function
    '
    ' Summary:
    '     Draws the specified text string at the specified location with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   point:
    '     System.Drawing.PointF structure that specifies the upper-left corner of the drawn
    '     text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Sub DrawString(s As String, font As Font, brush As Brush, point As PointF)

    End Sub
    '
    ' Summary:
    '     Draws the specified text string at the specified location with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
    '     of the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   point:
    '     System.Drawing.PointF structure that specifies the upper-left corner of the drawn
    '     text.
    '
    '   format:
    '     System.Drawing.StringFormat that specifies formatting attributes, such as line
    '     spacing and alignment, that are applied to the drawn text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Sub DrawString(s As String, font As Font, brush As Brush, point As PointF, format As StringFormat)

    End Sub
    '
    ' Summary:
    '     Draws the specified text string in the specified rectangle with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
    '     of the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   layoutRectangle:
    '     System.Drawing.RectangleF structure that specifies the location of the drawn
    '     text.
    '
    '   format:
    '     System.Drawing.StringFormat that specifies formatting attributes, such as line
    '     spacing and alignment, that are applied to the drawn text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Sub DrawString(s As String, font As Font, brush As Brush, layoutRectangle As RectangleF, format As StringFormat)

    End Sub
    '
    ' Summary:
    '     Draws the specified text string at the specified location with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn text.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Sub DrawString(s As String, font As Font, brush As Brush, x As Single, y As Single)

    End Sub
    '
    ' Summary:
    '     Draws the specified text string at the specified location with the specified
    '     System.Drawing.Brush and System.Drawing.Font objects using the formatting attributes
    '     of the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   s:
    '     String to draw.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   brush:
    '     System.Drawing.Brush that determines the color and texture of the drawn text.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the drawn text.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the drawn text.
    '
    '   format:
    '     System.Drawing.StringFormat that specifies formatting attributes, such as line
    '     spacing and alignment, that are applied to the drawn text.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-s is null.
    Public Sub DrawString(s As String, font As Font, brush As Brush, x As Single, y As Single, format As StringFormat)

    End Sub
    '
    ' Summary:
    '     Closes the current graphics container and restores the state of this System.Drawing.Graphics
    '     to the state saved by a call to the System.Drawing.Graphics.BeginContainer method.
    '
    ' Parameters:
    '   container:
    '     System.Drawing.Drawing2D.GraphicsContainer that represents the container this
    '     method restores.
    Public Sub EndContainer(container As GraphicsContainer)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified parallelogram using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structures that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records of the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display in a specified rectangle using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point using specified image
    '     attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc)

    End Sub
    '
    ' Summary:
    '     Sends the records in the specified System.Drawing.Imaging.Metafile, one at a
    '     time, to a callback method for display at a specified point using specified image
    '     attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, srcRect As Rectangle, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   srcUnit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, srcRect As RectangleF, srcUnit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.Point structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As Point, srcRect As Rectangle, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram
    '     using specified image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.PointF structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As PointF, srcRect As RectangleF, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle using
    '     specified image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.Rectangle structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As Rectangle, srcRect As Rectangle, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records of a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified rectangle using
    '     specified image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destRect:
    '     System.Drawing.RectangleF structure that specifies the location and size of the
    '     drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destRect As RectangleF, srcRect As RectangleF, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display in a specified parallelogram
    '     using specified image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoints:
    '     Array of three System.Drawing.Point structures that define a parallelogram that
    '     determines the size and location of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.Rectangle structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoints() As Point, srcRect As Rectangle, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Sends the records in a selected rectangle from a System.Drawing.Imaging.Metafile,
    '     one at a time, to a callback method for display at a specified point using specified
    '     image attributes.
    '
    ' Parameters:
    '   metafile:
    '     System.Drawing.Imaging.Metafile to enumerate.
    '
    '   destPoint:
    '     System.Drawing.PointF structure that specifies the location of the upper-left
    '     corner of the drawn metafile.
    '
    '   srcRect:
    '     System.Drawing.RectangleF structure that specifies the portion of the metafile,
    '     relative to its upper-left corner, to draw.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure used to determine the portion of the metafile that the rectangle specified
    '     by the srcRect parameter contains.
    '
    '   callback:
    '     System.Drawing.Graphics.EnumerateMetafileProc delegate that specifies the method
    '     to which the metafile records are sent.
    '
    '   callbackData:
    '     Internal pointer that is required, but ignored. You can pass System.IntPtr.Zero
    '     for this parameter.
    '
    '   imageAttr:
    '     System.Drawing.Imaging.ImageAttributes that specifies image attribute information
    '     for the drawn image.
    Public Sub EnumerateMetafile(metafile As Metafile, destPoint As PointF, srcRect As RectangleF, unit As GraphicsUnit, callback As EnumerateMetafileProc, callbackData As IntPtr, imageAttr As ImageAttributes)

    End Sub
    '
    ' Summary:
    '     Updates the clip region of this System.Drawing.Graphics to exclude the area specified
    '     by a System.Drawing.Region.
    '
    ' Parameters:
    '   region:
    '     System.Drawing.Region that specifies the region to exclude from the clip region.
    Public Sub ExcludeClip(region As Region)

    End Sub
    '
    ' Summary:
    '     Updates the clip region of this System.Drawing.Graphics to exclude the area specified
    '     by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.Rectangle structure that specifies the rectangle to exclude from
    '     the clip region.
    Public Sub ExcludeClip(rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As Point)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
    '     structures using the specified fill mode.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As Point, fillmode As FillMode)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
    '     structures using the specified fill mode.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As PointF, fillmode As FillMode)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.Point
    '     structures using the specified fill mode and tension.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.Point structures that define the spline.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As Point, fillmode As FillMode, tension As Single)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a closed cardinal spline curve defined by an array of System.Drawing.PointF
    '     structures using the specified fill mode and tension.
    '
    ' Parameters:
    '   brush:
    '     A System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that define the spline.
    '
    '   fillmode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines how
    '     the curve is filled.
    '
    '   tension:
    '     Value greater than or equal to 0.0F that specifies the tension of the curve.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillClosedCurve(brush As Brush, points() As PointF, fillmode As FillMode, tension As Single)

    End Sub
    '
    ' Summary:
    '     Fills the interior of an ellipse defined by a bounding rectangle specified by
    '     a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that represents the bounding rectangle that
    '     defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillEllipse(brush As Brush, rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Fills the interior of an ellipse defined by a bounding rectangle specified by
    '     a System.Drawing.RectangleF structure.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rect:
    '     System.Drawing.RectangleF structure that represents the bounding rectangle that
    '     defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillEllipse(brush As Brush, rect As RectangleF)

    End Sub
    '
    ' Summary:
    '     Fills the interior of an ellipse defined by a bounding rectangle specified by
    '     a pair of coordinates, a width, and a height.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillEllipse(brush As Brush, x As Integer, y As Integer, width As Integer, height As Integer)

    End Sub
    '
    ' Summary:
    '     Fills the interior of an ellipse defined by a bounding rectangle specified by
    '     a pair of coordinates, a width, and a height.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillEllipse(brush As Brush, x As Single, y As Single, width As Single, height As Single)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a System.Drawing.Drawing2D.GraphicsPath.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   path:
    '     System.Drawing.Drawing2D.GraphicsPath that represents the path to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-path is null.
    Public Sub FillPath(brush As Brush, path As GraphicsPath)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a pie section defined by an ellipse specified by a System.Drawing.RectangleF
    '     structure and two radial lines.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that represents the bounding rectangle that
    '     defines the ellipse from which the pie section comes.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the first side of the
    '     pie section.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to the second
    '     side of the pie section.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillPie(brush As Brush, rect As Rectangle, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a pie section defined by an ellipse specified by a pair
    '     of coordinates, a width, a height, and two radial lines.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie section comes.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie section comes.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse from which the pie section
    '     comes.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse from which the pie
    '     section comes.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the first side of the
    '     pie section.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to the second
    '     side of the pie section.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillPie(brush As Brush, x As Integer, y As Integer, width As Integer, height As Integer, startAngle As Integer, sweepAngle As Integer)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a pie section defined by an ellipse specified by a pair
    '     of coordinates, a width, a height, and two radial lines.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie section comes.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the bounding rectangle that defines
    '     the ellipse from which the pie section comes.
    '
    '   width:
    '     Width of the bounding rectangle that defines the ellipse from which the pie section
    '     comes.
    '
    '   height:
    '     Height of the bounding rectangle that defines the ellipse from which the pie
    '     section comes.
    '
    '   startAngle:
    '     Angle in degrees measured clockwise from the x-axis to the first side of the
    '     pie section.
    '
    '   sweepAngle:
    '     Angle in degrees measured clockwise from the startAngle parameter to the second
    '     side of the pie section.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Sub FillPie(brush As Brush, x As Single, y As Single, width As Single, height As Single, startAngle As Single, sweepAngle As Single)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a polygon defined by an array of points specified by System.Drawing.Point
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.Point structures that represent the vertices of the polygon
    '     to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillPolygon(brush As Brush, points() As Point)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a polygon defined by an array of points specified by System.Drawing.PointF
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the vertices of the
    '     polygon to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillPolygon(brush As Brush, points() As PointF)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a polygon defined by an array of points specified by System.Drawing.Point
    '     structures using the specified fill mode.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.Point structures that represent the vertices of the polygon
    '     to fill.
    '
    '   fillMode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the
    '     style of the fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillPolygon(brush As Brush, points() As Point, fillMode As FillMode)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a polygon defined by an array of points specified by System.Drawing.PointF
    '     structures using the specified fill mode.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   points:
    '     Array of System.Drawing.PointF structures that represent the vertices of the
    '     polygon to fill.
    '
    '   fillMode:
    '     Member of the System.Drawing.Drawing2D.FillMode enumeration that determines the
    '     style of the fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-points is null.
    Public Sub FillPolygon(brush As Brush, points() As PointF, fillMode As FillMode)

    End Sub
    '
    ' Summary:
    '     Fills the interior of a rectangle specified by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rect:
    '     System.Drawing.Rectangle structure that represents the rectangle to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Function FillRectangle(brush As Brush, rect As Rectangle) As SvgRectangle
        Return FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height)
    End Function

    ''' <summary>
    ''' Fills the interior of a rectangle specified by a System.Drawing.RectangleF structure.
    ''' </summary>
    ''' <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    ''' <param name="rect">System.Drawing.RectangleF structure that represents the rectangle to fill.</param>
    ''' <returns></returns>
    Public Function FillRectangle(brush As Brush, rect As RectangleF) As SvgRectangle
        Return FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height)
    End Function
    '
    ' Summary:
    '     Fills the interior of a rectangle specified by a pair of coordinates, a width,
    '     and a height.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to fill.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to fill.
    '
    '   width:
    '     Width of the rectangle to fill.
    '
    '   height:
    '     Height of the rectangle to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Function FillRectangle(brush As Brush, x As Integer, y As Integer, width As Integer, height As Integer) As SvgRectangle
        Dim rct As SvgRectangle = SVGRectangle(x, y, width, height)
        If TypeOf brush Is SolidBrush Then
            rct.Color = New SvgColourServer(DirectCast(brush, SolidBrush).Color)
        End If
        Call SVGHandle.Children.Add(rct)
        Return rct
    End Function
    '
    ' Summary:
    '     Fills the interior of a rectangle specified by a pair of coordinates, a width,
    '     and a height.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to fill.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to fill.
    '
    '   width:
    '     Width of the rectangle to fill.
    '
    '   height:
    '     Height of the rectangle to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.
    Public Function FillRectangle(brush As Brush, x As Single, y As Single, width As Single, height As Single) As SvgRectangle
        Return FillRectangle(brush, CInt(x), CInt(y), CInt(width), CInt(height))
    End Function
    '
    ' Summary:
    '     Fills the interiors of a series of rectangles specified by System.Drawing.Rectangle
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rects:
    '     Array of System.Drawing.Rectangle structures that represent the rectangles to
    '     fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-rects is null.
    '
    '   T:System.ArgumentException:
    '     rects is a zero-length array.
    Public Function FillRectangles(brush As Brush, rects() As Rectangle) As SvgRectangle()
        Return rects.ToArray(Function(x) FillRectangle(brush, x))
    End Function
    '
    ' Summary:
    '     Fills the interiors of a series of rectangles specified by System.Drawing.RectangleF
    '     structures.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   rects:
    '     Array of System.Drawing.RectangleF structures that represent the rectangles to
    '     fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-rects is null.
    '
    '   T:System.ArgumentException:
    '     Rects is a zero-length array.
    Public Function FillRectangles(brush As Brush, rects() As RectangleF) As SvgRectangle()
        Return rects.ToArray(Function(x) FillRectangle(brush, x))
    End Function
    '
    ' Summary:
    '     Fills the interior of a System.Drawing.Region.
    '
    ' Parameters:
    '   brush:
    '     System.Drawing.Brush that determines the characteristics of the fill.
    '
    '   region:
    '     System.Drawing.Region that represents the area to fill.
    '
    ' Exceptions:
    '   T:System.ArgumentNullException:
    '     brush is null.-or-region is null.
    Public Sub FillRegion(brush As Brush, region As Region)

    End Sub
    '
    ' Summary:
    '     Forces execution of all pending graphics operations and returns immediately without
    '     waiting for the operations to finish.
    Public Sub Flush()

    End Sub
    '
    ' Summary:
    '     Forces execution of all pending graphics operations with the method waiting or
    '     not waiting, as specified, to return before the operations finish.
    '
    ' Parameters:
    '   intention:
    '     Member of the System.Drawing.Drawing2D.FlushIntention enumeration that specifies
    '     whether the method returns immediately or waits for any existing operations to
    '     finish.
    Public Sub Flush(intention As FlushIntention)

    End Sub
    '
    ' Summary:
    '     Updates the clip region of this System.Drawing.Graphics to the intersection of
    '     the current clip region and the specified System.Drawing.RectangleF structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.RectangleF structure to intersect with the current clip region.
    Public Sub IntersectClip(rect As RectangleF)

    End Sub
    '
    ' Summary:
    '     Updates the clip region of this System.Drawing.Graphics to the intersection of
    '     the current clip region and the specified System.Drawing.Region.
    '
    ' Parameters:
    '   region:
    '     System.Drawing.Region to intersect with the current region.
    Public Sub IntersectClip(region As Region)

    End Sub
    '
    ' Summary:
    '     Updates the clip region of this System.Drawing.Graphics to the intersection of
    '     the current clip region and the specified System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.Rectangle structure to intersect with the current clip region.
    Public Sub IntersectClip(rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Multiplies the world transformation of this System.Drawing.Graphics and specified
    '     the System.Drawing.Drawing2D.Matrix.
    '
    ' Parameters:
    '   matrix:
    '     4x4 System.Drawing.Drawing2D.Matrix that multiplies the world transformation.
    Public Sub MultiplyTransform(matrix As Matrix)

    End Sub
    '
    ' Summary:
    '     Multiplies the world transformation of this System.Drawing.Graphics and specified
    '     the System.Drawing.Drawing2D.Matrix in the specified order.
    '
    ' Parameters:
    '   matrix:
    '     4x4 System.Drawing.Drawing2D.Matrix that multiplies the world transformation.
    '
    '   order:
    '     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that determines
    '     the order of the multiplication.
    Public Sub MultiplyTransform(matrix As Matrix, order As MatrixOrder)

    End Sub
    '
    ' Summary:
    '     Releases a device context handle obtained by a previous call to the System.Drawing.Graphics.GetHdc
    '     method of this System.Drawing.Graphics.
    Public Sub ReleaseHdc() Implements IDeviceContext.ReleaseHdc

    End Sub
    '
    ' Summary:
    '     Releases a device context handle obtained by a previous call to the System.Drawing.Graphics.GetHdc
    '     method of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   hdc:
    '     Handle to a device context obtained by a previous call to the System.Drawing.Graphics.GetHdc
    '     method of this System.Drawing.Graphics.
    <EditorBrowsable(EditorBrowsableState.Advanced)>
    Public Sub ReleaseHdc(hdc As IntPtr)

    End Sub
    '
    ' Summary:
    '     Releases a handle to a device context.
    '
    ' Parameters:
    '   hdc:
    '     Handle to a device context.
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Sub ReleaseHdcInternal(hdc As IntPtr)

    End Sub
    '
    ' Summary:
    '     Resets the clip region of this System.Drawing.Graphics to an infinite region.
    Public Sub ResetClip()

    End Sub
    '
    ' Summary:
    '     Resets the world transformation matrix of this System.Drawing.Graphics to the
    '     identity matrix.
    Public Sub ResetTransform()

    End Sub
    '
    ' Summary:
    '     Restores the state of this System.Drawing.Graphics to the state represented by
    '     a System.Drawing.Drawing2D.GraphicsState.
    '
    ' Parameters:
    '   gstate:
    '     System.Drawing.Drawing2D.GraphicsState that represents the state to which to
    '     restore this System.Drawing.Graphics.
    Public Sub Restore(gstate As GraphicsState)

    End Sub
    '
    ' Summary:
    '     Applies the specified rotation to the transformation matrix of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   angle:
    '     Angle of rotation in degrees.
    Public Sub RotateTransform(angle As Single)

    End Sub
    '
    ' Summary:
    '     Applies the specified rotation to the transformation matrix of this System.Drawing.Graphics
    '     in the specified order.
    '
    ' Parameters:
    '   angle:
    '     Angle of rotation in degrees.
    '
    '   order:
    '     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
    '     whether the rotation is appended or prepended to the matrix transformation.
    Public Sub RotateTransform(angle As Single, order As MatrixOrder)

    End Sub
    '
    ' Summary:
    '     Applies the specified scaling operation to the transformation matrix of this
    '     System.Drawing.Graphics by prepending it to the object's transformation matrix.
    '
    ' Parameters:
    '   sx:
    '     Scale factor in the x direction.
    '
    '   sy:
    '     Scale factor in the y direction.
    Public Sub ScaleTransform(sx As Single, sy As Single)

    End Sub
    '
    ' Summary:
    '     Applies the specified scaling operation to the transformation matrix of this
    '     System.Drawing.Graphics in the specified order.
    '
    ' Parameters:
    '   sx:
    '     Scale factor in the x direction.
    '
    '   sy:
    '     Scale factor in the y direction.
    '
    '   order:
    '     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
    '     whether the scaling operation is prepended or appended to the transformation
    '     matrix.
    Public Sub ScaleTransform(sx As Single, sy As Single, order As MatrixOrder)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the rectangle specified
    '     by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.Rectangle structure that represents the new clip region.
    Public Sub SetClip(rect As Rectangle)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the rectangle specified
    '     by a System.Drawing.RectangleF structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.RectangleF structure that represents the new clip region.
    Public Sub SetClip(rect As RectangleF)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the specified System.Drawing.Drawing2D.GraphicsPath.
    '
    ' Parameters:
    '   path:
    '     System.Drawing.Drawing2D.GraphicsPath that represents the new clip region.
    Public Sub SetClip(path As GraphicsPath)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the Clip property
    '     of the specified System.Drawing.Graphics.
    '
    ' Parameters:
    '   g:
    '     System.Drawing.Graphics from which to take the new clip region.
    Public Sub SetClip(g As Graphics)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the result of the
    '     specified operation combining the current clip region and the specified System.Drawing.Region.
    '
    ' Parameters:
    '   region:
    '     System.Drawing.Region to combine.
    '
    '   combineMode:
    '     Member from the System.Drawing.Drawing2D.CombineMode enumeration that specifies
    '     the combining operation to use.
    Public Sub SetClip(region As Region, combineMode As CombineMode)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the result of the
    '     specified operation combining the current clip region and the rectangle specified
    '     by a System.Drawing.RectangleF structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.RectangleF structure to combine.
    '
    '   combineMode:
    '     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
    '     the combining operation to use.
    Public Sub SetClip(rect As RectangleF, combineMode As CombineMode)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the result of the
    '     specified operation combining the current clip region and the rectangle specified
    '     by a System.Drawing.Rectangle structure.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.Rectangle structure to combine.
    '
    '   combineMode:
    '     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
    '     the combining operation to use.
    Public Sub SetClip(rect As Rectangle, combineMode As CombineMode)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the result of the
    '     specified operation combining the current clip region and the specified System.Drawing.Drawing2D.GraphicsPath.
    '
    ' Parameters:
    '   path:
    '     System.Drawing.Drawing2D.GraphicsPath to combine.
    '
    '   combineMode:
    '     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
    '     the combining operation to use.
    Public Sub SetClip(path As GraphicsPath, combineMode As CombineMode)

    End Sub
    '
    ' Summary:
    '     Sets the clipping region of this System.Drawing.Graphics to the result of the
    '     specified combining operation of the current clip region and the System.Drawing.Graphics.Clip
    '     property of the specified System.Drawing.Graphics.
    '
    ' Parameters:
    '   g:
    '     System.Drawing.Graphics that specifies the clip region to combine.
    '
    '   combineMode:
    '     Member of the System.Drawing.Drawing2D.CombineMode enumeration that specifies
    '     the combining operation to use.
    Public Sub SetClip(g As Graphics, combineMode As CombineMode)

    End Sub
    '
    ' Summary:
    '     Transforms an array of points from one coordinate space to another using the
    '     current world and page transformations of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   destSpace:
    '     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
    '     the destination coordinate space.
    '
    '   srcSpace:
    '     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
    '     the source coordinate space.
    '
    '   pts:
    '     Array of System.Drawing.Point structures that represents the points to transformation.
    Public Sub TransformPoints(destSpace As CoordinateSpace, srcSpace As CoordinateSpace, pts() As Point)

    End Sub
    '
    ' Summary:
    '     Transforms an array of points from one coordinate space to another using the
    '     current world and page transformations of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   destSpace:
    '     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
    '     the destination coordinate space.
    '
    '   srcSpace:
    '     Member of the System.Drawing.Drawing2D.CoordinateSpace enumeration that specifies
    '     the source coordinate space.
    '
    '   pts:
    '     Array of System.Drawing.PointF structures that represent the points to transform.
    Public Sub TransformPoints(destSpace As CoordinateSpace, srcSpace As CoordinateSpace, pts() As PointF)

    End Sub
    '
    ' Summary:
    '     Translates the clipping region of this System.Drawing.Graphics by specified amounts
    '     in the horizontal and vertical directions.
    '
    ' Parameters:
    '   dx:
    '     The x-coordinate of the translation.
    '
    '   dy:
    '     The y-coordinate of the translation.
    Public Sub TranslateClip(dx As Integer, dy As Integer)

    End Sub
    '
    ' Summary:
    '     Translates the clipping region of this System.Drawing.Graphics by specified amounts
    '     in the horizontal and vertical directions.
    '
    ' Parameters:
    '   dx:
    '     The x-coordinate of the translation.
    '
    '   dy:
    '     The y-coordinate of the translation.
    Public Sub TranslateClip(dx As Single, dy As Single)

    End Sub
    '
    ' Summary:
    '     Changes the origin of the coordinate system by prepending the specified translation
    '     to the transformation matrix of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   dx:
    '     The x-coordinate of the translation.
    '
    '   dy:
    '     The y-coordinate of the translation.
    Public Sub TranslateTransform(dx As Single, dy As Single)

    End Sub
    '
    ' Summary:
    '     Changes the origin of the coordinate system by applying the specified translation
    '     to the transformation matrix of this System.Drawing.Graphics in the specified
    '     order.
    '
    ' Parameters:
    '   dx:
    '     The x-coordinate of the translation.
    '
    '   dy:
    '     The y-coordinate of the translation.
    '
    '   order:
    '     Member of the System.Drawing.Drawing2D.MatrixOrder enumeration that specifies
    '     whether the translation is prepended or appended to the transformation matrix.
    Public Sub TranslateTransform(dx As Single, dy As Single, order As MatrixOrder)

    End Sub
    Protected Overrides Sub Finalize()

    End Sub


    '
    ' Summary:
    '     Saves a graphics container with the current state of this System.Drawing.Graphics
    '     and opens and uses a new graphics container.
    '
    ' Returns:
    '     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
    '     the state of this System.Drawing.Graphics at the time of the method call.
    Public Function BeginContainer() As GraphicsContainer
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Saves a graphics container with the current state of this System.Drawing.Graphics
    '     and opens and uses a new graphics container with the specified scale transformation.
    '
    ' Parameters:
    '   dstrect:
    '     System.Drawing.Rectangle structure that, together with the srcrect parameter,
    '     specifies a scale transformation for the container.
    '
    '   srcrect:
    '     System.Drawing.Rectangle structure that, together with the dstrect parameter,
    '     specifies a scale transformation for the container.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure for the container.
    '
    ' Returns:
    '     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
    '     the state of this System.Drawing.Graphics at the time of the method call.
    Public Function BeginContainer(dstrect As Rectangle, srcrect As Rectangle, unit As GraphicsUnit) As GraphicsContainer
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Saves a graphics container with the current state of this System.Drawing.Graphics
    '     and opens and uses a new graphics container with the specified scale transformation.
    '
    ' Parameters:
    '   dstrect:
    '     System.Drawing.RectangleF structure that, together with the srcrect parameter,
    '     specifies a scale transformation for the new graphics container.
    '
    '   srcrect:
    '     System.Drawing.RectangleF structure that, together with the dstrect parameter,
    '     specifies a scale transformation for the new graphics container.
    '
    '   unit:
    '     Member of the System.Drawing.GraphicsUnit enumeration that specifies the unit
    '     of measure for the container.
    '
    ' Returns:
    '     This method returns a System.Drawing.Drawing2D.GraphicsContainer that represents
    '     the state of this System.Drawing.Graphics at the time of the method call.
    Public Function BeginContainer(dstrect As RectangleF, srcrect As RectangleF, unit As GraphicsUnit) As GraphicsContainer
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Gets the cumulative graphics context.
    '
    ' Returns:
    '     An System.Object representing the cumulative graphics context.
    <EditorBrowsable(EditorBrowsableState.Never)>
    Public Function GetContextInfo() As Object
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Gets the handle to the device context associated with this System.Drawing.Graphics.
    '
    ' Returns:
    '     Handle to the device context associated with this System.Drawing.Graphics.
    Public Function GetHdc() As IntPtr Implements IDeviceContext.GetHdc
        Return SVGHandle.GetHashCode
    End Function
    '
    ' Summary:
    '     Gets the nearest color to the specified System.Drawing.Color structure.
    '
    ' Parameters:
    '   color:
    '     System.Drawing.Color structure for which to find a match.
    '
    ' Returns:
    '     A System.Drawing.Color structure that represents the nearest color to the one
    '     specified with the color parameter.
    Public Function GetNearestColor(color As Color) As Color
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the rectangle specified by a System.Drawing.Rectangle structure
    '     is contained within the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.Rectangle structure to test for visibility.
    '
    ' Returns:
    '     true if the rectangle specified by the rect parameter is contained within the
    '     visible clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(rect As Rectangle) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the rectangle specified by a System.Drawing.RectangleF structure
    '     is contained within the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   rect:
    '     System.Drawing.RectangleF structure to test for visibility.
    '
    ' Returns:
    '     true if the rectangle specified by the rect parameter is contained within the
    '     visible clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(rect As RectangleF) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the specified System.Drawing.PointF structure is contained
    '     within the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   point:
    '     System.Drawing.PointF structure to test for visibility.
    '
    ' Returns:
    '     true if the point specified by the point parameter is contained within the visible
    '     clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(point As PointF) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the specified System.Drawing.Point structure is contained within
    '     the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   point:
    '     System.Drawing.Point structure to test for visibility.
    '
    ' Returns:
    '     true if the point specified by the point parameter is contained within the visible
    '     clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(point As Point) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the point specified by a pair of coordinates is contained within
    '     the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   x:
    '     The x-coordinate of the point to test for visibility.
    '
    '   y:
    '     The y-coordinate of the point to test for visibility.
    '
    ' Returns:
    '     true if the point defined by the x and y parameters is contained within the visible
    '     clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(x As Single, y As Single) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the point specified by a pair of coordinates is contained within
    '     the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   x:
    '     The x-coordinate of the point to test for visibility.
    '
    '   y:
    '     The y-coordinate of the point to test for visibility.
    '
    ' Returns:
    '     true if the point defined by the x and y parameters is contained within the visible
    '     clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(x As Integer, y As Integer) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the rectangle specified by a pair of coordinates, a width,
    '     and a height is contained within the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to test for visibility.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to test for visibility.
    '
    '   width:
    '     Width of the rectangle to test for visibility.
    '
    '   height:
    '     Height of the rectangle to test for visibility.
    '
    ' Returns:
    '     true if the rectangle defined by the x, y, width, and height parameters is contained
    '     within the visible clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(x As Single, y As Single, width As Single, height As Single) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Indicates whether the rectangle specified by a pair of coordinates, a width,
    '     and a height is contained within the visible clip region of this System.Drawing.Graphics.
    '
    ' Parameters:
    '   x:
    '     The x-coordinate of the upper-left corner of the rectangle to test for visibility.
    '
    '   y:
    '     The y-coordinate of the upper-left corner of the rectangle to test for visibility.
    '
    '   width:
    '     Width of the rectangle to test for visibility.
    '
    '   height:
    '     Height of the rectangle to test for visibility.
    '
    ' Returns:
    '     true if the rectangle defined by the x, y, width, and height parameters is contained
    '     within the visible clip region of this System.Drawing.Graphics; otherwise, false.
    Public Function IsVisible(x As Integer, y As Integer, width As Integer, height As Integer) As Boolean
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Gets an array of System.Drawing.Region objects, each of which bounds a range
    '     of character positions within the specified string.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   layoutRect:
    '     System.Drawing.RectangleF structure that specifies the layout rectangle for the
    '     string.
    '
    '   stringFormat:
    '     System.Drawing.StringFormat that represents formatting information, such as line
    '     spacing, for the string.
    '
    ' Returns:
    '     This method returns an array of System.Drawing.Region objects, each of which
    '     bounds a range of character positions within the specified string.
    Public Function MeasureCharacterRanges(text As String, font As Font, layoutRect As RectangleF, stringFormat As StringFormat) As Region()
        Throw New NotImplementedException
    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified by the text parameter as drawn with the font parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font that defines the format of the string.
    '
    '   width:
    '     Maximum width of the string in pixels.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified in the text parameter as drawn with the font parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, width As Integer) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font
    '     within the specified layout area.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font defines the text format of the string.
    '
    '   layoutArea:
    '     System.Drawing.SizeF structure that specifies the maximum layout area for the
    '     text.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified by the text parameter as drawn with the font parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, layoutArea As SizeF) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font
    '     and formatted with the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font defines the text format of the string.
    '
    '   layoutArea:
    '     System.Drawing.SizeF structure that specifies the maximum layout area for the
    '     text.
    '
    '   stringFormat:
    '     System.Drawing.StringFormat that represents formatting information, such as line
    '     spacing, for the string.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified in the text parameter as drawn with the font parameter and the
    '     stringFormat parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, layoutArea As SizeF, stringFormat As StringFormat) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font
    '     and formatted with the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   width:
    '     Maximum width of the string.
    '
    '   format:
    '     System.Drawing.StringFormat that represents formatting information, such as line
    '     spacing, for the string.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified in the text parameter as drawn with the font parameter and the
    '     stringFormat parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, width As Integer, format As StringFormat) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font
    '     and formatted with the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font defines the text format of the string.
    '
    '   origin:
    '     System.Drawing.PointF structure that represents the upper-left corner of the
    '     string.
    '
    '   stringFormat:
    '     System.Drawing.StringFormat that represents formatting information, such as line
    '     spacing, for the string.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size,
    '     in the units specified by the System.Drawing.Graphics.PageUnit property, of the
    '     string specified by the text parameter as drawn with the font parameter and the
    '     stringFormat parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, origin As PointF, stringFormat As StringFormat) As SizeF

    End Function
    '
    ' Summary:
    '     Measures the specified string when drawn with the specified System.Drawing.Font
    '     and formatted with the specified System.Drawing.StringFormat.
    '
    ' Parameters:
    '   text:
    '     String to measure.
    '
    '   font:
    '     System.Drawing.Font that defines the text format of the string.
    '
    '   layoutArea:
    '     System.Drawing.SizeF structure that specifies the maximum layout area for the
    '     text.
    '
    '   stringFormat:
    '     System.Drawing.StringFormat that represents formatting information, such as line
    '     spacing, for the string.
    '
    '   charactersFitted:
    '     Number of characters in the string.
    '
    '   linesFilled:
    '     Number of text lines in the string.
    '
    ' Returns:
    '     This method returns a System.Drawing.SizeF structure that represents the size
    '     of the string, in the units specified by the System.Drawing.Graphics.PageUnit
    '     property, of the text parameter as drawn with the font parameter and the stringFormat
    '     parameter.
    '
    ' Exceptions:
    '   T:System.ArgumentException:
    '     font is null.
    Public Function MeasureString(text As String, font As Font, layoutArea As SizeF, stringFormat As StringFormat, ByRef charactersFitted As Integer, ByRef linesFilled As Integer) As SizeF

    End Function
    '
    ' Summary:
    '     Saves the current state of this System.Drawing.Graphics and identifies the saved
    '     state with a System.Drawing.Drawing2D.GraphicsState.
    '
    ' Returns:
    '     This method returns a System.Drawing.Drawing2D.GraphicsState that represents
    '     the saved state of this System.Drawing.Graphics.
    Public Function Save() As GraphicsState

    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

#End Region
End Class

