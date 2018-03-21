Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging

Public Class Icon

    Public Property Style As Types
    Public Property Name As String
    Public Property Color As Color

    Public ReadOnly Property Preview As String
        Get
            Return ToString()
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        If Color.IsEmpty Then
            Return (<li class=<%= Style.Description & " " & Strings.LCase(Name) %>></li>).ToString
        Else
            Return (<li class=<%= Style.Description & " " & Strings.LCase(Name) %> style=<%= $"color:{Color.ToHtmlColor}" %>></li>).ToString
        End If
    End Function

End Class

Public Enum Types
    <Description("fas")> Solid
    <Description("far")> Regular
    <Description("fal")> Light
End Enum