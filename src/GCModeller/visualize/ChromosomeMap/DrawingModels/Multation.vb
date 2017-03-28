#Region "Microsoft.VisualBasic::915f11425a7a85604b3498b8718e7b77, ..\visualize\ChromosomeMap\DrawingModels\Multation.vb"

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
            {MutationTypes.MotifSite, AddressOf RegulationMotifSite.TriangleModel}}

        Protected Shared ReadOnly __color As Dictionary(Of MutationTypes, Color) = New Dictionary(Of MutationTypes, Color) From {
            {MutationTypes.DeleteMutation, Color.Red},
            {MutationTypes.IntegrationMutant, Color.Blue}}

        Public Overrides Sub Draw(Device As Graphics, location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim GraphModel = __drawingModel(Me.MutationType)(location, Me.Direction, FlagLength, FLAG_HEIGHT)
            Dim Color As Color = __color(Me.MutationType)

            Call Device.DrawPath(New Pen(Color, 8), GraphModel)
            Call Device.FillPath(New SolidBrush(Color), GraphModel)
        End Sub

        ''' <summary>
        ''' 三角形
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <param name="direction"></param>
        ''' <param name="FlagLength"></param>
        ''' <param name="FLAG_HEIGHT"></param>
        ''' <returns></returns>
        Private Shared Function GetTriangleModel(ref As Point, direction As Integer, FlagLength As Integer, FLAG_HEIGHT As Integer) As GraphicsPath
            Dim ModelGraph = New Drawing2D.GraphicsPath
            Dim Height = FLAG_HEIGHT * 0.4
            Dim offset As Integer = 5
            Dim pt_top As Point = New Point(ref.X, ref.Y - Height - offset)
            Dim pt_direction = New Point(ref.X + direction * FlagLength, pt_top.Y + Height / 2)
            Dim pt_trroot = New Point(ref.X, ref.Y - offset)

            Call ModelGraph.AddLine(pt_top, pt_direction)
            Call ModelGraph.AddLine(pt_direction, pt_trroot)
            Call ModelGraph.AddLine(pt_trroot, pt_top)

            Call ModelGraph.CloseAllFigures()

            Return ModelGraph
        End Function

        ''' <summary>
        ''' 小旗子
        ''' </summary>
        ''' <param name="ref"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetDeleteMutationModel(ref As Point, direction As Integer, FlagLength As Integer, FLAG_HEIGHT As Integer) As GraphicsPath
            Dim ModelGraph = New Drawing2D.GraphicsPath
            Dim pt_top As Point = New Point(ref.X, ref.Y - FLAG_HEIGHT)
            Dim pt_flagDirection = New Point(ref.X + direction * FlagLength, pt_top.Y + 0.4 * FLAG_HEIGHT)
            Dim pt_flagroot = New Point(ref.X, pt_flagDirection.Y)
            Dim pt_flagmain = ref

            Call ModelGraph.AddLine(pt_top, pt_flagDirection)
            Call ModelGraph.AddLine(pt_flagDirection, pt_flagroot)
            Call ModelGraph.AddLine(pt_top, pt_flagmain)

            Return ModelGraph
        End Function
    End Class
End Namespace
