#Region "Microsoft.VisualBasic::a73f9d73d1f05901b6ec3a248043a1aa, GCModeller\visualize\ChromosomeMap\FootPrintMap\RegulationMotifSite.vb"

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

    '   Total Lines: 64
    '    Code Lines: 37
    ' Comment Lines: 12
    '   Blank Lines: 15
    '     File Size: 2.70 KB


    '     Class RegulationMotifSite
    ' 
    '         Function: __topVertex, __vertexDown, TriangleModel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace FootprintMap

    ''' <summary>
    ''' Drawing model and method for the footprint motifs.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class RegulationMotifSite

        Dim Device As ComponentModel.DrawingDevice

        ''' <summary>
        ''' 生成一个三角形的绘图模型
        ''' </summary>
        ''' <param name="Position"></param>
        ''' <param name="Height"></param>
        ''' <param name="Width"></param>
        ''' <param name="UpSideDown"></param>
        ''' <returns></returns>
        Public Shared Function TriangleModel(Position As System.Drawing.Point,
                                             Height As Integer,
                                             Width As Integer,
                                             UpSideDown As Integer) As System.Drawing.Drawing2D.GraphicsPath

            Dim Model = If(UpSideDown <> 0,
                           __topVertex(Position, Height, Width),  ' 顶点在上面
                           __vertexDown(Position, Height, Width)) ' 顶点朝下
            Call Model.CloseFigure()

            Return Model
        End Function

        Private Shared Function __vertexDown(Position As System.Drawing.Point,
                                     Height As Integer,
                                     Width As Integer) As System.Drawing.Drawing2D.GraphicsPath

            Dim Model As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

            Dim RightTop = New System.Drawing.Point(Position.X + 0.5 * Width, Position.Y - Height)
            Dim LeftTop = New System.Drawing.Point(RightTop.X - Width, RightTop.Y)

            Call Model.AddLine(Position, RightTop)
            Call Model.AddLine(RightTop, LeftTop)
            Call Model.AddLine(LeftTop, Position)

            Return Model
        End Function

        Private Shared Function __topVertex(Position As System.Drawing.Point,
                                     Height As Integer,
                                     Width As Integer) As System.Drawing.Drawing2D.GraphicsPath

            Dim Model As System.Drawing.Drawing2D.GraphicsPath = New System.Drawing.Drawing2D.GraphicsPath

            Dim RightButtom = New System.Drawing.Point(Position.X + 0.5 * Width, Position.Y + Height)
            Dim LeftButtom = New System.Drawing.Point(RightButtom.X - Width, RightButtom.Y)

            Call Model.AddLine(Position, RightButtom)
            Call Model.AddLine(RightButtom, LeftButtom)
            Call Model.AddLine(LeftButtom, Position)

            Return Model
        End Function
    End Class
End Namespace
