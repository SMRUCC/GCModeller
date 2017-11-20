Imports System.Text

Public Structure InterpolateArgs

    Dim wwwroot$

    ''' <summary>
    ''' String resource from loader: <see cref="LoadStrings(String)"/>
    ''' </summary>
    Dim resource As Dictionary(Of String, String)
    Dim variables As Dictionary(Of String, String)
    Dim data As Dictionary(Of String, IEnumerable)
    Dim codepage As Encoding

    Public Overrides Function ToString() As String
        Return wwwroot
    End Function
End Structure
