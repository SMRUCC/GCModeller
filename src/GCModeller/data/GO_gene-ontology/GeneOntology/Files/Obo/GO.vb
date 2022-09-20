#Region "Microsoft.VisualBasic::56dba3e0429f8d2cf2407c6adabc24a9, GCModeller\data\GO_gene-ontology\GeneOntology\Files\Obo\GO.vb"

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

'   Total Lines: 128
'    Code Lines: 82
' Comment Lines: 27
'   Blank Lines: 19
'     File Size: 4.73 KB


'     Class GO_OBO
' 
'         Properties: headers, terms, typedefs
' 
'         Function: GenericEnumerator, GetEnumerator, LoadDocument, Open, ParseHeader
'                   ReadTerms, Save, ToString
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
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models
Imports Field = SMRUCC.genomics.foundation.OBO_Foundry.IO.Reflection.Field

Namespace OBO

    ''' <summary>
    ''' The file reader/writer for ``go.obo``/``go-basic.obo``(Go注释功能定义文件)
    ''' </summary>
    ''' <remarks>
    ''' The Go database file I/O module
    ''' </remarks>
    Public Class GO_OBO : Implements Enumeration(Of Term)

        Public Property headers As header
        Public Property terms As Term()
        Public Property typedefs As Typedef()

        Public Overrides Function ToString() As String
            Return $"{headers} with {terms.Length} terms"
        End Function

        ''' <summary>
        ''' Save data as obo file
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function Save(path As String) As Boolean
            Using file As Stream = path.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                Call Save(file)
            End Using

            Return True
        End Function

        Public Sub Save(file As Stream, Optional excludes As String() = Nothing)
            Dim text As New StreamWriter(file, encoding:=Encodings.ASCII.CodePage) With {.NewLine = vbLf}
            Dim schema = Reflection.LoadClassSchema(Of Term)()
            Dim excludeList As Index(Of String) = Nothing

            If Not excludes.IsNullOrEmpty Then
                excludeList = excludes.Indexing
            End If

            Call headers.ToLines.ForEach(AddressOf text.WriteLine)
            Call text.WriteLine()

            For Each lines As String() In From t As Term
                                          In terms
                                          Select t.ToLines(schema, excludeList).ToArray

                Call text.WriteLine(Term.Term)
                Call lines.ForEach(AddressOf text.WriteLine)
                Call text.WriteLine()
            Next

            Call text.Flush()
        End Sub

        ''' <summary>
        ''' 对于小文件可以使用这个方法来读取
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Shared Function LoadDocument(path As String) As GO_OBO
            Using obo As New OBOFile(path$)
                Dim terms As Term() = obo _
                    .DoCall(AddressOf ReadTerms) _
                    .ToArray

                Return New GO_OBO With {
                    .headers = obo.header,
                    .terms = terms
                }
            End Using
        End Function

        Public Shared Function ParseHeader(path As String) As header
            Using obo As New OBOFile(path$)
                Return obo.header
            End Using
        End Function

        Public Shared Iterator Function ReadTerms(obo As OBOFile) As IEnumerable(Of Term)
            Dim schema As Dictionary(Of BindProperty(Of Field)) = Reflection.LoadClassSchema(Of Term)()

            For Each term As RawTerm In obo.GetRawTerms
                If term.type = GeneOntology.OBO.Term.Term Then
                    Yield schema.LoadData(Of Term)(term.GetData)
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
        Public Shared Function Open(path As String) As IEnumerable(Of Term)
            Return ReadTerms(New OBOFile(path))
        End Function

        ''' <summary>
        ''' Save as csv table file.
        ''' </summary>
        ''' <param name="path$"></param>
        ''' <param name="encoding"></param>
        Public Sub SaveTable(path$, Optional encoding As Encodings = Encodings.ASCII)
            Using writer As StreamWriter = path.OpenWriter(encoding)
                Dim write As Action(Of String) = AddressOf writer.WriteLine

                Call {"goID", "namespace", "name"} _
                    .JoinBy(",") _
                    .DoCall(write)

                For Each term As Term In terms
                    Call {term.id, term.namespace, term.name} _
                        .Select(Function(str) $"""{str}""") _
                        .JoinBy(",") _
                        .DoCall(write)
                Next
            End Using
        End Sub

        Public Iterator Function GenericEnumerator() As IEnumerator(Of Term) Implements Enumeration(Of Term).GenericEnumerator
            For Each item As Term In terms
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of Term).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace
