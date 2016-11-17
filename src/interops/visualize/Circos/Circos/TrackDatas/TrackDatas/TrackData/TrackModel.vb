Imports System.Text

Namespace TrackDatas

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

            Call __attach(s, NameOf(glyph), glyph)
            Call __attach(s, NameOf(glyph_size), glyph_size)
            Call __attach(s, NameOf(fill_color), fill_color)
            Call __attach(s, "url", URL)

            Return s.ToString
        End Function

        Private Shared Sub __attach(ByRef s As StringBuilder, name As String, value As String)
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