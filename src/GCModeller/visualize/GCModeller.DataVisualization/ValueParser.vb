#Region "Microsoft.VisualBasic::034b73c6e6a3216e2a08e03882dd90ea, ..\GCModeller\visualize\GCModeller.DataVisualization\ValueParser.vb"

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
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

Public Module ValueParser

    ''' <summary>
    ''' default is <see cref="ImageFormat.Bmp"/>
    ''' </summary>
    ''' <param name="format"></param>
    ''' <returns></returns>
    <Extension>
    Public Function GetSaveImageFormat(format As String) As ImageFormat
        Dim value As String = format.ToLower.Trim

        If ImagingFormats.ContainsKey(value) Then
            Return ImagingFormats(value)
        Else
            Return ImageFormat.Bmp
        End If
    End Function

    Private ReadOnly ImagingFormats As Dictionary(Of String, ImageFormat) =
        New Dictionary(Of String, ImageFormat) From {
            {"jpg", ImageFormat.Jpeg},
            {"bmp", ImageFormat.Bmp},
            {"emf", ImageFormat.Emf},
            {"exif", ImageFormat.Exif},
            {"gif", ImageFormat.Gif},
            {"png", ImageFormat.Png},
            {"wmf", ImageFormat.Wmf},
            {"tiff", ImageFormat.Tiff}
    }

    Public ReadOnly Property FontStyles As Dictionary(Of String, FontStyle) =
        New Dictionary(Of String, FontStyle) From {
            {"bold", FontStyle.Bold},
            {"italic", FontStyle.Italic},
            {"regular", FontStyle.Regular},
            {"strikeout", FontStyle.Strikeout},
            {"underline", FontStyle.Underline}
    }

    Public Function GetFontValue(s As String) As Font
        If String.IsNullOrEmpty(s) Then
            Return New Font(FontFace.MicrosoftYaHei, 14)
        End If

        Dim Tokens As String() = s.Split(CChar(","))
        Dim FontName As String = Tokens.First.Replace("""", "")
        Dim FontSize = CType(Val(Tokens(1)), Single)
        Dim FontStyle As FontStyle

        If Tokens.Length > 2 Then
            Dim fstlName As String = Tokens.Last.ToLower

            If FontStyles.ContainsKey(fstlName) Then
                FontStyle = FontStyles(fstlName)
            Else
                FontStyle = FontStyle.Regular
            End If
        Else
            FontStyle = FontStyle.Regular
        End If

        Return New Font(FontName, FontSize, FontStyle)
    End Function
End Module
