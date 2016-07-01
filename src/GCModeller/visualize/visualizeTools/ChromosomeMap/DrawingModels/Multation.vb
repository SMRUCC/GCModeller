Imports System.Drawing
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.ChromosomeMap.FootprintMap

Namespace ChromosomeMap.DrawingModels

    ''' <summary>
    ''' 基因组之中的一个位点
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Site

        Public Property SiteName As String
        Public Property Comments As String
        Public Property Left As Integer
        Public Property Right As Integer

        Public Overridable ReadOnly Property Width As Integer
            Get
                Return Math.Abs(Left - Right)
            End Get
        End Property

        Public MustOverride Sub Draw(Device As System.Drawing.Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)

        Public Overrides Function ToString() As String
            Return SiteName
        End Function
    End Class

    Public Class MultationPointData : Inherits Site

        Public Enum MutationTypes
            Unknown = -1
            DeleteMutation
            IntegrationMutant
            MotifSite
        End Enum

        Public Property MutationType As MutationTypes
        Public Property Direction As Integer

        Protected Shared ReadOnly get_MutationDrawingModel As Dictionary(Of MutationTypes, Func(Of Point, Integer, Integer, Integer, Drawing2D.GraphicsPath)) =
            New Dictionary(Of MutationTypes, Func(Of Point, Integer, Integer, Integer, Drawing2D.GraphicsPath)) From {
            {MutationTypes.DeleteMutation, AddressOf GetDeleteMutationModel},
            {MutationTypes.IntegrationMutant, AddressOf GetTriangleModel},
            {MutationTypes.MotifSite, AddressOf RegulationMotifSite.TriangleModel}}

        Protected Shared ReadOnly get_color As Dictionary(Of MutationTypes, Color) = New Dictionary(Of MutationTypes, Color) From {
            {MutationTypes.DeleteMutation, Color.Red},
            {MutationTypes.IntegrationMutant, Color.Blue}}

        Public Overrides Sub Draw(Device As System.Drawing.Graphics, Location As Point, FlagLength As Integer, FLAG_HEIGHT As Integer)
            Dim GraphModel = get_MutationDrawingModel(Me.MutationType)(Location, Me.Direction, FlagLength, FLAG_HEIGHT)
            Dim Color As Color = get_color(Me.MutationType)

            Call Device.DrawPath(New Pen(Color, 8), GraphModel)
            Call Device.FillPath(New SolidBrush(Color), GraphModel)
        End Sub

        Private Shared Function GetTriangleModel(ReferenceLocation As System.Drawing.Point, direction As Integer, FlagLength As Integer, FLAG_HEIGHT As Integer) As System.Drawing.Drawing2D.GraphicsPath
            Dim ModelGraph = New Drawing2D.GraphicsPath
            Dim Height = FLAG_HEIGHT * 0.4
            Dim offset As Integer = 5
            Dim pt_top As Point = New Point(ReferenceLocation.X, ReferenceLocation.Y - Height - offset)
            Dim pt_direction = New Point(ReferenceLocation.X + direction * FlagLength, pt_top.Y + Height / 2)
            Dim pt_trroot = New Point(ReferenceLocation.X, ReferenceLocation.Y - offset)

            Call ModelGraph.AddLine(pt_top, pt_direction)
            Call ModelGraph.AddLine(pt_direction, pt_trroot)
            Call ModelGraph.AddLine(pt_trroot, pt_top)

            Call ModelGraph.CloseAllFigures()

            Return ModelGraph
        End Function

        ''' <summary>
        ''' 小旗子
        ''' </summary>
        ''' <param name="ReferenceLocation"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function GetDeleteMutationModel(ReferenceLocation As System.Drawing.Point, direction As Integer, FlagLength As Integer, FLAG_HEIGHT As Integer) As System.Drawing.Drawing2D.GraphicsPath
            Dim ModelGraph = New Drawing2D.GraphicsPath
            Dim pt_top As Point = New Point(ReferenceLocation.X, ReferenceLocation.Y - FLAG_HEIGHT)
            Dim pt_flagDirection = New Point(ReferenceLocation.X + direction * FlagLength, pt_top.Y + 0.4 * FLAG_HEIGHT)
            Dim pt_flagroot = New Point(ReferenceLocation.X, pt_flagDirection.Y)
            Dim pt_flagmain = ReferenceLocation

            Call ModelGraph.AddLine(pt_top, pt_flagDirection)
            Call ModelGraph.AddLine(pt_flagDirection, pt_flagroot)
            Call ModelGraph.AddLine(pt_top, pt_flagmain)

            Return ModelGraph
        End Function
    End Class
End Namespace