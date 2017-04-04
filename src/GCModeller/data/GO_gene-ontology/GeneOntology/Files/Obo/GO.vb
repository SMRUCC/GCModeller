#Region "Microsoft.VisualBasic::8018a473770e626c11f341ea44b334de, ..\GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\GO.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
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

Imports System.IO
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    ''' <summary>
    ''' go.obo/go-basic.obo(Go注释功能定义文件)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class GO_OBO

        Public Property header As header
        Public Property Terms As Term()

        Public Function Save(path As String) As Boolean
            Dim bufs As List(Of String) = New List(Of String)
            Dim schema = LoadClassSchema(Of Term)()
            Dim LQuery = From x As Term
                         In Terms
                         Select x.ToLines(schema)

            Call bufs.AddRange(header.ToLines)
            Call bufs.Add("")

            For Each x In LQuery
                Call bufs.Add(Term.Term)
                Call bufs.AddRange(x)
                Call bufs.Add("")
            Next

            Return bufs.SaveTo(path, Encodings.ASCII.CodePage)
        End Function

        ''' <summary>
        ''' 对于小文件可以使用这个方法来读取
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(path As String) As GO_OBO
            Using obo As New OBOFile(path$)
                Return New GO_OBO With {
                    .header = obo.header,
                    .Terms = ReadTerms(obo).ToArray
                }
            End Using
        End Function

        Public Shared Function ParseHeader(path$) As header
            Using obo As New OBOFile(path$)
                Return obo.header
            End Using
        End Function

        Public Shared Iterator Function ReadTerms(obo As OBOFile) As IEnumerable(Of Term)
            Dim schema As Dictionary(Of BindProperty(Of foundation.OBO_Foundry.Field)) =
                LoadClassSchema(Of Term)()

            For Each x As RawTerm In obo.GetDatas
                If x.Type = Term.Term Then
                    Yield schema.LoadData(Of Term)(x.GetData)
                End If
            Next
        End Function

        ''' <summary>
        ''' 使用迭代器来读取大型的GO OBO文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        Public Shared Function Open(path$) As IEnumerable(Of Term)
            Return ReadTerms(New OBOFile(path))
        End Function

        Public Sub SaveTable(path$, Optional encoding As Encodings = Encodings.ASCII)
            Using writer As StreamWriter = path.OpenWriter(encoding)
                Call writer.WriteLine(
                    New RowObject({"goID", "namespace", "name"}).AsLine)

                For Each term As Term In Terms
                    Call writer.WriteLine(
                        New RowObject({term.id, term.namespace, term.name}).AsLine)
                Next
            End Using
        End Sub
    End Class
End Namespace
