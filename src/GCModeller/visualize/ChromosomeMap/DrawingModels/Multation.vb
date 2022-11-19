#Region "Microsoft.VisualBasic::36ef116272ada32e8a046fcbd97d4830, GCModeller\visualize\ChromosomeMap\DrawingModels\Multation.vb"

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

    '   Total Lines: 104
    '    Code Lines: 60
    ' Comment Lines: 29
    '   Blank Lines: 15
    '     File Size: 3.93 KB


    '     Enum MutationTypes
    ' 
    '         DeleteMutation, IntegrationMutant, MotifSite
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class MultationPointData
    ' 
    '         Properties: Direction, MutationType
    ' 
    '         Function: GetDeleteMutationModel, GetTriangleModel
    ' 
    '         Sub: Draw
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging
Imports SMRUCC.genomics.Visualize.ChromosomeMap.FootprintMap

Namespace DrawingModels

    ''' <summary>
    ''' 突变的类型
    ''' </summary>
    Public Enum MutationTypes
        Unknown = -1
        ''' <summary>
        ''' 缺失突变
        ''' </summary>
        DeleteMutation
        ''' <summary>
        ''' 整合突变
        ''' </summary>
        IntegrationMutant
        ''' <summary>
        ''' Motif 位点
        ''' </summary>
        MotifSite
    End Enum

    ''' <summary>
    ''' 基因组上面的突变位点的绘图数据
    ''' </summary>
    Public Class MultationPointData : Inherits Site

        Public Property MutationType As MutationTypes
        Public Property Direction As Integer

        Protected Shared ReadOnly __drawingModel As New Dictionary(Of MutationTypes, Func(Of Point, Integer, Integer, Integer, GraphicsPath)) From {
            {MutationTypes.DeleteMutation, AddressOf GetDeleteMutationModel},
            {MutationTypes.IntegrationMutant, AddressOf GetTriangleModel},
            {MutationTypes.MotifSite, AddressOf RegulationMotifSite.TriangleModel},
            {MutationTypes.Unknown, AddressOf GetDeleteMutationModel}
        }

        Protected Shared ReadOnly __color As New Dictionary(Of MutationTypes, Color) From {
            {MutationTypes.DeleteMutation, Color.Red},
            {MutationTypes.IntegrationMutant, Color.Blue},
            {MutationTypes.Unknown, Color.RosyBrown}
        }

        Public Overrides Sub Draw(g As IGraphics, location As Point, flagLen As Integer, flagHeight As Integer)
            Dim flagShape As GraphicsPath = __drawingModel(Me.MutationType)(location, Me.Direction, flagLen, flagHeight)
            Dim color As Color = __color(Me.MutationType)

            Call g.DrawPath(New Pen(color, 8), flagShape)
            Call g.FillPath(New SolidBrush(color), flagShape)
        End Sub

        ''' <summary>
        ''' 三角形
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="direction"></param>
        ''' <param name="flagLen"></param>
        ''' <param name="flagHeight"></param>
        ''' <returns></returns>
        Private Shared Function GetTriangleModel(ref As Point, direction As Integer, flagLen As Integer, flagHeight As Integer) As GraphicsPath
            Dim modelGraph As New GraphicsPath
            Dim height = flagHeight * 0.4
            Dim offset As Integer = 5
            Dim pt_top As New Point(ref.X, ref.Y - height - offset)
            Dim pt_direction As New Point(ref.X + direction * flagLen, pt_top.Y + height / 2)
            Dim pt_trroot As New Point(ref.X, ref.Y - offset)

            Call modelGraph.AddLine(pt_top, pt_direction)
            Call modelGraph.AddLine(pt_direction, pt_trroot)
            Call modelGraph.AddLine(pt_trroot, pt_top)

            Call modelGraph.CloseAllFigures()

            Return modelGraph
        End Function

        ''' <summary>
        ''' 小旗子
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetDeleteMutationModel(ref As Point, direction As Integer, flagLen As Integer, flagHeight As Integer) As GraphicsPath
            Dim flag As New GraphicsPath
            Dim top As New Point(ref.X, ref.Y - flagHeight)
            Dim flagDirection As New Point With {
                .X = ref.X + direction * flagLen,
                .Y = top.Y + 0.4 * flagHeight
            }
            Dim flagroot As New Point(ref.X, flagDirection.Y)
            Dim flagmain = ref

            Call flag.AddLine(top, flagDirection)
            Call flag.AddLine(flagDirection, flagroot)
            Call flag.AddLine(top, flagmain)

            Return flag
        End Function
    End Class
End Namespace
