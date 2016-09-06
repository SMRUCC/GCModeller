#Region "Microsoft.VisualBasic::e14b52a9b63f629c846e11bb398c0ddc, ..\GCModeller\data\GO_gene-ontology\AnnotationFile\Obo\AnnotationFile.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.foundation.OBO_Foundry

''' <summary>
''' go.obo/go-basic.obo(Go注释功能定义文件)
''' </summary>
''' <remarks></remarks>
Public Class AnnotationFile

    Public Property header As header
    Public Property Terms As Term()

    Public Shared Function LoadDocument(path As String) As AnnotationFile
        Dim lines As String() = LinqAPI.Exec(Of String) <=
 _
            From strLine As String
            In IO.File.ReadAllLines(path)
            Where Not String.IsNullOrEmpty(strLine)
            Select strLine.Replace(",", ";")

        Dim bufs As String()
        Dim i As Integer = Array.IndexOf(lines, Term.TERM)
        Dim File As New AnnotationFile

        If i = -1 Then
            Dim Head As header = LoadData(Of header)(lines)
            Return New AnnotationFile With {
                .header = Head,
                .Terms = New Term() {}
            }
        Else
            bufs = New String(i - 1) {}
            Call Array.ConstrainedCopy(lines, 0, bufs, 0, bufs.Length)
            File.header = LoadData(Of header)(bufs)
        End If

        Dim Terms As List(Of Term) = New List(Of Term)
        Dim pre As Integer = i

        i += 1
        pre = i
        i = Array.IndexOf(lines, Term.TERM, i)

        Do While i > -1
            Dim Length As Integer = i - pre
            bufs = New String(Length - 1) {}
            Call Array.ConstrainedCopy(lines, pre, bufs, 0, Length)
            Call Terms.Add(LoadData(Of Term)(bufs))

            i += 1
            pre = i
            i = Array.IndexOf(lines, Term.TERM, i)
        Loop

        bufs = New String(lines.Length - 1 - pre) {}
        Call Array.ConstrainedCopy(lines, pre, bufs, 0, bufs.Length)
        Call Terms.Add(LoadData(Of Term)(bufs))

        File.Terms = Terms.ToArray

        Return File
    End Function

    Public Function Save(path As String) As Boolean
        Dim bufs As List(Of String) = New List(Of String)
        Dim schema = LoadClassSchema(Of Term)()
        Dim LQuery = From x As Term
                     In Terms
                     Select x.ToLines(schema)

        Call bufs.AddRange(header.ToLines)
        Call bufs.Add("")

        For Each x In LQuery
            Call bufs.Add(Term.TERM)
            Call bufs.AddRange(x)
            Call bufs.Add("")
        Next

        Return bufs.SaveTo(path, Encodings.ASCII.GetEncodings)
    End Function
End Class
