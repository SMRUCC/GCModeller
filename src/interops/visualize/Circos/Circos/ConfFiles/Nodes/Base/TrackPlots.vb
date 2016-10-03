#Region "Microsoft.VisualBasic::abeb31ffebfee646aa0f8b53a1c4f1e5, ..\interops\visualize\Circos\Circos\ConfFiles\Nodes\Base\TrackPlots.vb"

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

Imports System.Text
Imports SMRUCC.genomics.Visualize.Circos.Configurations
Imports SMRUCC.genomics.Visualize.Circos.Configurations.Nodes.Plots
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text

Namespace Configurations.Nodes.Plots

    ''' <summary>
    ''' Abstract model of the tracks plot
    ''' </summary>
    ''' <remarks>Using this interface to solved the problem of generics type</remarks>
    Public Interface ITrackPlot : Inherits ICircosDocNode

        <Circos> ReadOnly Property type As String

        ''' <summary>
        ''' 输入的路径会根据配置情况转换为相对路径或者绝对路径
        ''' </summary>
        ''' <returns></returns>
        <Circos> Property file As String
        ''' <summary>
        ''' 圈外径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Property r1 As String

        ''' <summary>
        ''' 圈内径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' The track is confined within r0/r1 radius limits. When using the
        ''' relative "r" suffix, the values are relative To the position Of the
        ''' ideogram.
        ''' </remarks>
        <Circos> Property r0 As String

        Property orientation As String
        Property fill_color As String
        Property stroke_thickness As String
        Property stroke_color As String

        ReadOnly Property TracksData As Idata

        Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
    End Interface

    Public MustInherit Class TracksPlot(Of T As ITrackData)
        Implements ICircosDocument
        Implements ITrackPlot

        <Circos> Public MustOverride ReadOnly Property type As String Implements ITrackPlot.type

        ''' <summary>
        ''' 输入的路径会根据配置情况转换为相对路径或者绝对路径
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property file As String Implements ITrackPlot.file
            Get
                Return Tools.TrimPath(TracksData.FileName)
            End Get
            Set(value As String)
                TracksData.FileName = value
            End Set
        End Property

        ''' <summary>
        ''' 圈外径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Circos> Public Property r1 As String = "0.75r" Implements ITrackPlot.r1

        ''' <summary>
        ''' 圈内径(单位 r，请使用格式"&lt;double>r")
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' The track is confined within r0/r1 radius limits. When using the
        ''' relative "r" suffix, the values are relative To the position Of the
        ''' ideogram.
        ''' </remarks>
        <Circos> Public Property r0 As String = "0.6r" Implements ITrackPlot.r0
        <Circos> Public Property max As String = "1"
        <Circos> Public Property min As String = "0"
        <Circos> Public Overridable Property fill_color As String = "orange" Implements ITrackPlot.fill_color
        ''' <summary>
        ''' 圈的朝向，是<see cref="ORIENTATION_IN"/>向内还是<see cref="ORIENTATION_OUT"/>向外
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property orientation As String = "in" Implements ITrackPlot.orientation
        ''' <summary>
        ''' To turn off default outline, set the outline thickness to zero. 
        ''' If you want To permanently disable this Default, edit
        ''' ``etc/tracks/histogram.conf`` In the Circos distribution.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property thickness As String = "2p"
        <Circos> Public Property stroke_thickness As String = "0" Implements ITrackPlot.stroke_thickness
        <Circos> Public Property stroke_color As String = "grey" Implements ITrackPlot.stroke_color

        Public Const ORIENTATION_OUT As String = "out"
        Public Const ORIENTATION_IN As String = "in"

        Public Property Rules As List(Of ConditionalRule)

        ''' <summary>
        ''' data文件夹之中的绘图数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TracksData As Idata Implements ITrackPlot.TracksData

        ''' <summary>
        ''' Creates plot element from the tracks data file.
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub New(data As data(Of T))
            TracksData = data
        End Sub

        Public Overrides Function ToString() As String
            Return $"({type}  --> {Me.TracksData.GetType.Name})  {Me.TracksData.ToString}"
        End Function

        Public Overridable Function Build(IndentLevel As Integer) As String Implements ICircosDocument.Build
            Dim IndentBlanks As String = New String(" "c, IndentLevel)
            Dim sb As StringBuilder = New StringBuilder(IndentBlanks & "<plot>" & vbCrLf, 1024)

            Call sb.AppendLine()
            Call sb.AppendLine(String.Format("{0}#   --> ""{1}""", IndentBlanks, TracksData.GetType.FullName))
            Call sb.AppendLine()

            If TypeOf TracksData.GetEnumerator.FirstOrDefault Is ValueTrackData Then
                Dim ranges As DoubleRange =
                    TrackDatas.Ranges(TracksData.GetEnumerator.Select(Function(o) TryCast(o, ValueTrackData)))

                Me.max = CStr(ranges.Max)
                Me.min = CStr(ranges.Min)
            End If

            For Each strLine As String In GetProperties()
                Call sb.AppendLine(IndentBlanks & "  " & strLine)
            Next

            Dim PlotElements = GeneratePlotsElementListChunk()

            If Not PlotElements.IsNullOrEmpty Then

                For Each item In PlotElements
                    Call sb.AppendLine(vbCrLf & IndentBlanks & String.Format("<{0}>", item.Key))

                    For Each o As CircosDocument In item.Value
                        Call sb.AppendLine(o.Build(IndentLevel + 2))
                    Next

                    Call sb.AppendLine(IndentBlanks & String.Format("</{0}>", item.Key))
                Next
            End If

            Call sb.AppendLine(IndentBlanks & "</plot>")

            Return sb.ToString
        End Function

        ''' <summary>
        ''' SimpleConfig.GenerateConfigurations(Of &lt;PlotType>)(Me)，需要手工复写以得到正确的类型
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function GetProperties() As String()

        Protected Overridable Function GeneratePlotsElementListChunk() As Dictionary(Of String, List(Of CircosDocument))
            If Not Rules.IsNullOrEmpty Then
                Return New Dictionary(Of String, List(Of CircosDocument)) From {{"rules", (From item In Rules Select DirectCast(item, CircosDocument)).ToList}}
            Else
                Return Nothing
            End If
        End Function

        Public Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean Implements ICircosDocument.Save, ITrackPlot.Save
            Return TracksData.GetDocumentText.SaveTo(FilePath, Encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace
