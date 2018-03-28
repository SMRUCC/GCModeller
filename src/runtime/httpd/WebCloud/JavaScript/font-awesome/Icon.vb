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

    Sub New(icon As icons, Optional style As Styles = Styles.Solid, Optional color As Color = Nothing)
        Call Me.New(icon.Description, style, color)
    End Sub

    Sub New(name$, Optional style As Styles = Styles.Solid, Optional color As Color = Nothing)
        Me.Name = name
        Me.Style = style
        Me.Color = color
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        If Color.IsEmpty Then
            Return (<i class=<%= Style.Description & " " & Strings.LCase(Name) %>></i>).ToString
        Else
            Return (<i class=<%= Style.Description & " " & Strings.LCase(Name) %> style=<%= $"color:{Color.ToHtmlColor};" %>></i>).ToString
        End If
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Operator &(html$, fa As Icon) As String
        Return html & fa.ToString
    End Operator

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Operator &(fa As Icon, html$) As String
        Return fa.ToString & html
    End Operator
End Class