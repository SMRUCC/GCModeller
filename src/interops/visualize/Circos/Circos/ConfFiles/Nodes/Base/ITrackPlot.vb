Imports System.Text
Imports SMRUCC.genomics.Visualize.Circos.TrackDatas

Namespace Configurations.Nodes.Plots

    Public Enum orientations
        [in]
        out
    End Enum

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

        Property orientation As orientations
        Property fill_color As String
        Property stroke_thickness As String
        Property stroke_color As String
        Property thickness As String

        ReadOnly Property TracksData As Idata

        Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
    End Interface
End Namespace