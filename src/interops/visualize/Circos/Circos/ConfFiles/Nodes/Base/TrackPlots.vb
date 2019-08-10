#Region "Microsoft.VisualBasic::9d6a2ad856881a3f8cf6d4d6e6661be3, visualize\Circos\Circos\ConfFiles\Nodes\Base\TrackPlots.vb"

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

    '     Class TracksPlot
    ' 
    '         Properties: file, fill_color, max, min, orientation
    '                     r0, r1, rules, stroke_color, stroke_thickness
    '                     thickness, tracksData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Build, GeneratePlotsElementListChunk, (+2 Overloads) Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

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
                Return Tools.TrimPath(tracksData.FileName)
            End Get
            Set(value As String)
                tracksData.FileName = value
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
        ''' 圈的朝向，是<see cref="ORIENTATIONs.IN"/>向内还是<see cref="ORIENTATIONs.OUT"/>向外
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property orientation As orientations = orientations.in Implements ITrackPlot.orientation
        ''' <summary>
        ''' To turn off default outline, set the outline thickness to zero. 
        ''' If you want To permanently disable this Default, edit
        ''' ``etc/tracks/histogram.conf`` In the Circos distribution.
        ''' </summary>
        ''' <returns></returns>
        <Circos> Public Property thickness As String = "2p" Implements ITrackPlot.thickness
        <Circos> Public Property stroke_thickness As String = "0" Implements ITrackPlot.stroke_thickness
        <Circos> Public Property stroke_color As String = "grey" Implements ITrackPlot.stroke_color

        Public Property rules As List(Of ConditionalRule)

        ''' <summary>
        ''' data文件夹之中的绘图数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property tracksData As Idata Implements ITrackPlot.tracksData

        ''' <summary>
        ''' Creates plot element from the tracks data file.
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub New(data As data(Of T))
            tracksData = data
        End Sub

        Public Overrides Function ToString() As String
            Return $"({type}  --> {Me.tracksData.GetType.Name})  {Me.tracksData.ToString}"
        End Function

        Public Overridable Function Build(IndentLevel As Integer, directory$) As String Implements ICircosDocument.Build
            Dim blanks As New String(" "c, IndentLevel)
            Dim sb As New StringBuilder(blanks & "<plot>" & vbCrLf, 1024)

            Call sb.AppendLine()
            Call sb.AppendLine(String.Format("{0}#   --> ""{1}""", blanks, tracksData.GetType.FullName))
            Call sb.AppendLine()

            If TypeOf tracksData.GetEnumerator.FirstOrDefault Is ValueTrackData Then
                Dim values As ValueTrackData() = tracksData _
                    .GetEnumerator _
                    .Select(Function(o) TryCast(o, ValueTrackData)) _
                    .ToArray
                Dim ranges As DoubleRange = TrackDatas.Ranges(values)

                Me.max = CStr(ranges.Max)
                Me.min = CStr(ranges.Min)
            End If

            For Each line As String In GetProperties()
                Call sb.AppendLine(blanks & "  " & line)
            Next

            Dim plots = GeneratePlotsElementListChunk()

            If Not plots.IsNullOrEmpty Then
                For Each item In plots
                    Call sb.AppendLine(vbCrLf & blanks & String.Format("<{0}>", item.Key))

                    For Each o As CircosDocument In item.Value
                        Call sb.AppendLine(o.Build(IndentLevel + 2, directory))
                    Next

                    Call sb.AppendLine(blanks & String.Format("</{0}>", item.Key))
                Next
            End If

            Call sb.AppendLine(blanks & "</plot>")

            Return sb.ToString
        End Function

        ''' <summary>
        ''' SimpleConfig.GenerateConfigurations(Of &lt;PlotType>)(Me)，需要手工复写以得到正确的类型
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected MustOverride Function GetProperties() As String()

        Protected Overridable Function GeneratePlotsElementListChunk() As Dictionary(Of String, List(Of CircosDocument))
            If Not Me.rules.IsNullOrEmpty Then
                Dim rules = From item As ConditionalRule
                            In Me.rules
                            Select DirectCast(item, CircosDocument)

                Return New Dictionary(Of String, List(Of CircosDocument)) From {
                    {"rules", rules.AsList}
                }
            Else
                Return Nothing
            End If
        End Function

        Public Function Save(FilePath As String, Encoding As Encoding) As Boolean Implements ICircosDocument.Save, ITrackPlot.Save
            Return tracksData.GetDocumentText.SaveTo(FilePath, Encoding)
        End Function

        Public Function Save(Path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.CodePage)
        End Function
    End Class
End Namespace
