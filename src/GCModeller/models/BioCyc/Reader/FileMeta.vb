#Region "Microsoft.VisualBasic::694f33e24691b596fb46cdabbfa9f9e9, GCModeller\models\BioCyc\Reader\FileMeta.vb"

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


    ' Code Statistics:

    '   Total Lines: 81
    '    Code Lines: 67
    ' Comment Lines: 1
    '   Blank Lines: 13
    '     File Size: 2.89 KB


    ' Class FileMeta
    ' 
    '     Properties: attributes, authors, copyright, create_time, database
    '                 fileName, organism, version
    ' 
    '     Function: readMeta, ToString
    ' 
    ' /********************************************************************************/

#End Region

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

    Public Overrides Function ToString() As String
        Return fileName
    End Function

    Friend Shared Function readMeta(file As StreamReader, ByRef line As Value(Of String)) As FileMeta
        Dim lines As New List(Of String)
        Dim meta As New FileMeta

        Do While (line = file.ReadLine).StartsWith("#"c)
            Call lines.Add(line.Trim({"#"c}))
        Loop

        lines = (From str As String
                 In lines
                 Where Not str.StringEmpty).AsList

        If lines.First.Trim.StartsWith("Copyright") Then
            meta.copyright = lines.First.Trim
            lines.RemoveAt(Scan0)
        End If

        Dim authors As New List(Of String)
        Dim attrNames As New List(Of String)

        For i As Integer = 0 To lines.Count - 1
            If lines(i).Trim.StartsWith("Authors:") Then
                For j As Integer = i + 1 To lines.Count - 1
                    If lines(j).StartsWith("  ") Then
                        Call authors.Add(lines(j).Trim)
                    Else
                        i = j
                        Exit For
                    End If
                Next
            ElseIf lines(i).Trim.StartsWith("Attributes:") Then
                For j As Integer = i + 1 To lines.Count - 1
                    If lines(j).StartsWith("  ") Then
                        Call attrNames.Add(lines(j).Trim)
                    Else
                        i = j
                        Exit For
                    End If
                Next
            Else
                Dim tag = lines(i).Trim.GetTagValue(":")

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

