Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

''' <summary>
''' fontawesome ver5
''' </summary>
Public Class Icon

    Public Property Style As Styles
    Public Property Name As String
    Public Property Color As Color

    Public ReadOnly Property Preview As String
        Get
            Return ToString()
        End Get
    End Property

    Sub New(icon As icons, Optional style As Styles = Styles.Regular, Optional color As Color = Nothing)
        Call Me.New(icon.Description, style, color)
    End Sub

    Sub New(name$, Optional style As Styles = Styles.Regular, Optional color As Color = Nothing)
        Me.Name = name
        Me.Style = style
        Me.Color = color
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        If Color.IsEmpty Then
            Return (<li class=<%= Style.Description & " " & Strings.LCase(Name) %>></li>).ToString
        Else
            Return (<li class=<%= Style.Description & " " & Strings.LCase(Name) %> style=<%= $"color:{Color.ToHtmlColor}" %>></li>).ToString
        End If
    End Function

End Class