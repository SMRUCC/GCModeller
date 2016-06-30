Imports System.Drawing
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic

Public Class ColorProfiles

    ReadOnly _defaultColor As Color

    Public ReadOnly Property ColorProfiles As Dictionary(Of String, Color)

    Default Public ReadOnly Property GetColor(Name As String) As Color
        Get
            If _ColorProfiles.ContainsKey(Name) Then
                Return _ColorProfiles(Name)
            Else
                Return _defaultColor
            End If
        End Get
    End Property

    Sub New(ColorProfiles As IEnumerable(Of String), Optional DefaultColor As Color = Nothing)
        _ColorProfiles = ColorProfiles.GenerateColorProfiles
        _defaultColor = DefaultColor

        If _defaultColor = Nothing Then _defaultColor = Color.Black
    End Sub

    Public Overrides Function ToString() As String
        Return String.Join(__describ, ColorProfiles.Count, _defaultColor.ToString)
    End Function

    Const __describ As String =
        "{0} color(s) in the rendering profile, default color is ""{1}"""

End Class
