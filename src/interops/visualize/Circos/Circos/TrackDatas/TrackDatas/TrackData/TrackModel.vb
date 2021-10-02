#Region "Microsoft.VisualBasic::455431215ce885ceaaee04f47eb7f7ff, visualize\Circos\Circos\TrackDatas\TrackDatas\TrackData\TrackModel.vb"

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

    '     Interface ITrackData
    ' 
    '         Properties: comment
    ' 
    '         Function: GetLineData
    ' 
    '     Structure Formatting
    ' 
    '         Function: ToString
    ' 
    '         Sub: attach
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Namespace TrackDatas

    ''' <summary>
    ''' 通常是使用<see cref="Object.ToString"/>方法来生成数据文件之中的行数据的
    ''' </summary>
    Public Interface ITrackData

        Property comment As String
        ''' <summary>
        ''' Usually Using <see cref="TrackData.ToString()"/> method for creates tracks data document.
        ''' </summary>
        ''' <returns></returns>
        Function GetLineData() As String
    End Interface

    ''' <summary>
    ''' Annotated with formatting parameters that control how the point Is drawn. 
    ''' </summary>
    Public Structure Formatting

        ''' <summary>
        ''' Only works in scatter, example is ``10p``
        ''' </summary>
        Dim glyph_size As String
        ''' <summary>
        ''' Only works in scatter, example is ``circle``
        ''' </summary>
        Dim glyph As String
        ''' <summary>
        ''' Works on histogram
        ''' </summary>
        Dim fill_color As String
        ''' <summary>
        ''' Works on any <see cref="Trackdata"/> data type.
        ''' </summary>
        Dim URL As String

        Public Overrides Function ToString() As String
            Dim s As New StringBuilder

            Call attach(s, NameOf(glyph), glyph)
            Call attach(s, NameOf(glyph_size), glyph_size)
            Call attach(s, NameOf(fill_color), fill_color)
            Call attach(s, "url", URL)

            Return s.ToString
        End Function

        Private Shared Sub attach(ByRef s As StringBuilder, name As String, value As String)
            If String.IsNullOrEmpty(value) Then
                Return
            End If

            If s.Length = 0 Then
                Call s.Append($"{name}={value}")
            Else
                Call s.Append($",{name}={value}")
            End If
        End Sub
    End Structure
End Namespace
