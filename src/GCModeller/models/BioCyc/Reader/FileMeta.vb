Imports System.IO
Imports Microsoft.VisualBasic.Language

Public Class FileMeta

    Public Property copyright As String
    Public Property authors As String()
    Public Property fileName As String
    Public Property organism As String
    Public Property database As String
    Public Property version As Version
    Public Property create_time As Date
    Public Property attributes As String()

    Friend Function readMeta(file As StreamReader, ByRef line As Value(Of String)) As FileMeta
        Dim lines As New List(Of String)
        Dim meta As New FileMeta

        Do While (line = file.ReadLine).StartsWith("#"c)
            Call lines.Add(line.Trim({"#"c}))
        Loop

        lines = (From str As String
                 In lines
                 Where Not str.StringEmpty).AsList

        If lines.First.StartsWith("Copyright") Then
            meta.copyright = lines.First.Trim
            lines.RemoveAt(Scan0)
        End If

        Dim authors As New List(Of String)
        Dim attrNames As New List(Of String)

        For i As Integer = 0 To lines.Count - 1
            If lines(i).StartsWith("Authors:") Then
                For j As Integer = i + 1 To lines.Count - 1
                    If lines(j).StartsWith(" ") Then
                        Call authors.Add(lines(j).Trim)
                    Else
                        i = j
                        Exit For
                    End If
                Next
            ElseIf lines(i).StartsWith("Attributes:") Then
                For j As Integer = i + 1 To lines.Count - 1
                    If lines(j).StartsWith(" ") Then
                        Call attrNames.Add(lines(j).Trim)
                    Else
                        i = j
                        Exit For
                    End If
                Next
            Else
                Dim tag = lines(i).GetTagValue(":")

                If (Not tag.Name.StringEmpty) AndAlso (Not tag.Value.StringEmpty) Then
                    Select Case tag.Name
                        Case "Filename" : meta.fileName = tag.Value
                        Case "Organism" : meta.organism = tag.Value
                        Case "Database" : meta.database = tag.Value
                        Case "Version" : meta.version = Version.Parse(tag.Value)
                        Case "Date and time generated" : meta.create_time = Date.Parse(tag.Value)
                        Case Else
                            ' do nothing?
                    End Select
                End If
            End If
        Next

        meta.authors = authors.ToArray
        meta.attributes = attrNames.ToArray

        Return meta
    End Function

End Class
