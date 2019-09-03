#Region "Microsoft.VisualBasic::e182348b0f7751c507df43e88c8846b7, data\GO_gene-ontology\GeneOntology\Files\Obo\GO.vb"

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

'     Class GO_OBO
' 
'         Properties: header, Terms
' 
'         Function: LoadDocument, Open, ParseHeader, ReadTerms, Save
' 
'         Sub: SaveTable
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports Field = SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection.Field

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
            Dim schema = Reflection.LoadClassSchema(Of Term)()
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
            Dim schema As Dictionary(Of BindProperty(Of Field)) = Reflection.LoadClassSchema(Of Term)()

            For Each x As RawTerm In obo.GetRawTerms
                If x.type = Term.Term Then
                    Yield schema.LoadData(Of Term)(x.GetData)
                End If
            Next
        End Function

        ''' <summary>
        ''' 使用迭代器来读取大型的GO OBO文件
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Open(path$) As IEnumerable(Of Term)
            Return ReadTerms(New OBOFile(path))
        End Function

        Public Sub SaveTable(path$, Optional encoding As Encodings = Encodings.ASCII)
            Using writer As StreamWriter = path.OpenWriter(encoding)
                Dim write As Action(Of String) = AddressOf writer.WriteLine

                Call {"goID", "namespace", "name"} _
                    .JoinBy(",") _
                    .DoCall(write)

                For Each term As Term In Terms
                    Call {term.id, term.namespace, term.name} _
                        .Select(Function(str) $"""{str}""") _
                        .JoinBy(",") _
                        .DoCall(write)
                Next
            End Using
        End Sub
    End Class
End Namespace
