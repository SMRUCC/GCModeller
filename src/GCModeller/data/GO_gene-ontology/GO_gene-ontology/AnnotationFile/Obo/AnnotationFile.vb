#Region "Microsoft.VisualBasic::7e1ef2e1d0022b79c33ae6194efb2b07, ..\GO_gene-ontology\AnnotationFile\Obo\AnnotationFile.vb"

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

''' <summary>
''' go.obo(Go注释功能定义文件)
''' </summary>
''' <remarks></remarks>
Public Class AnnotationFile

    Public Property Head As Head
    Public Property Terms As Term()

    Public Shared Function LoadDocument(path As String) As AnnotationFile
        Dim FileContent As String() = (From strLine As String In IO.File.ReadAllLines(path) Where Not String.IsNullOrEmpty(strLine) Select strLine.Replace(",", ";")).ToArray
        Dim ChunkBuffer As String()
        Dim i As Integer = Array.IndexOf(FileContent, Term.TERM)
        Dim File As AnnotationFile = New AnnotationFile

        If i = -1 Then
            Dim Head As Head = Field.LoadData(Of Head)(FileContent)
            Return New AnnotationFile With {.Head = Head, .Terms = New Term() {}}
        Else
            ChunkBuffer = New String(i - 1) {}
            Call Array.ConstrainedCopy(FileContent, 0, ChunkBuffer, 0, ChunkBuffer.Length)
            File.Head = Field.LoadData(Of Head)(ChunkBuffer)
        End If

        Dim Terms As List(Of Term) = New List(Of Term)
        Dim pre As Integer = i

        i += 1
        pre = i
        i = Array.IndexOf(FileContent, Term.TERM, i)

        Do While i > -1
            Dim Length As Integer = i - pre
            ChunkBuffer = New String(Length - 1) {}
            Call Array.ConstrainedCopy(FileContent, pre, ChunkBuffer, 0, Length)
            Call Terms.Add(Field.LoadData(Of Term)(ChunkBuffer))

            i += 1
            pre = i
            i = Array.IndexOf(FileContent, Term.TERM, i)
        Loop

        ChunkBuffer = New String(FileContent.Count - 1 - pre) {}
        Call Array.ConstrainedCopy(FileContent, pre, ChunkBuffer, 0, ChunkBuffer.Length)
        Call Terms.Add(Field.LoadData(Of Term)(ChunkBuffer))

        File.Terms = Terms.ToArray

        Return File
    End Function

    Public Function Save(path As String) As Boolean
        Dim ChunkBuffer As List(Of String) = New List(Of String)
        Dim LQuery = (From TermItem As Term In Terms Select Field.GenerateValueCollection(Of Term)(TermItem)).ToArray

        Call ChunkBuffer.AddRange(Field.GenerateValueCollection(Of Head)(Head))
        Call ChunkBuffer.Add("")

        For Each item In LQuery
            Call ChunkBuffer.Add(Term.TERM)
            Call ChunkBuffer.AddRange(item)
            Call ChunkBuffer.Add("")
        Next

        Call FileIO.FileSystem.CreateDirectory(FileIO.FileSystem.GetParentPath(path))
        Call IO.File.WriteAllLines(path, ChunkBuffer.ToArray, encoding:=System.Text.Encoding.ASCII)

        Return True
    End Function
End Class

