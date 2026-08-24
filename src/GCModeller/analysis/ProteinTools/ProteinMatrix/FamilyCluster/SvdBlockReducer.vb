#Region "Microsoft.VisualBasic::d4a3ec0d2caed207abc1466aad2f4d9e, analysis\ProteinTools\ProteinMatrix\FamilyCluster\SvdBlockReducer.vb"

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
    '    Code Lines: 89 (69.53%)
    ' Comment Lines: 17 (13.28%)
    '    - Xml Docs: 94.12%
    ' 
    '   Blank Lines: 22 (17.19%)
    '     File Size: 5.65 KB


    '     Class SvdBlockReducer
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: ColumnCountPlaceholder, LoadMeta, ReadEmbeddings
    ' 
    '         Sub: FlushBlock, Reduce
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace FamilyCluster

    ''' <summary>
    ''' block-wise randomized SVD reducer for the streaming pipeline.
    '''
    ''' the COO sparse vectors are read one block at a time, accumulated into a
    ''' <see cref="SparseMatrix"/> (row-dictionary layout, so only the current block is ever in
    ''' memory) and reduced with <see cref="TruncatedSVD.Reduce"/>. the resulting m x k dense
    ''' embedding is streamed straight to disk row-by-row, so the full embedding never has to be
    ''' materialized as one big array.
    ''' </summary>
    Public Class SvdBlockReducer

        Public Const SVD_FILE As String = "svd_vectors.tsv"
        Public Const SVD_META_FILE As String = "svd_meta.json"

        Private ReadOnly workDir As String
        Private svdWriter As StreamWriter

        Public Sub New(workDir As String)
            Me.workDir = workDir
        End Sub

        ''' <summary>
        ''' reduce the sparse vectors (provided as blocks of (rowIndex, cols, vals)) down to
        ''' <paramref name="dims"/> dimensions and stream the dense embeddings to disk.
        ''' </summary>
        Public Sub Reduce(rows As IEnumerable(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double())), dims As Integer, blockSize As Integer)
            Dim path As String = System.IO.Path.Combine(workDir, SVD_FILE)
            svdWriter = New StreamWriter(New BufferedStream(New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20)), Encoding.ASCII)

            Dim blockRows As New List(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double()))
            Dim totalRows As Integer = 0

            For Each row In rows.SafeQuery
                blockRows.Add(row)
                totalRows += 1

                If blockRows.Count >= blockSize Then
                    Call FlushBlock(blockRows, dims)
                    blockRows.Clear()
                End If
            Next

            If blockRows.Count > 0 Then
                Call FlushBlock(blockRows, dims)
            End If

            svdWriter.Flush()
            svdWriter.Dispose()

            Call File.WriteAllText(System.IO.Path.Combine(workDir, SVD_META_FILE), New Dictionary(Of String, String) From {
                {"rows", totalRows.ToString},
                {"dims", dims.ToString}
            }.GetJson)

            Call VBDebugger.EchoLine($" [svd] wrote {totalRows} x {dims} embeddings to {SVD_FILE}")
        End Sub

        Private Sub FlushBlock(block As List(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double())), dims As Integer)
            Dim m = block.Count
            Dim sparse As New SparseMatrix(m, ColumnCountPlaceholder(block))

            For i As Integer = 0 To m - 1
                Dim r = block(i)
                For j As Integer = 0 To r.cols.Length - 1
                    sparse.Set(r.vals(j), i, r.cols(j))
                Next
            Next

            Dim embedding = TruncatedSVD.Reduce(sparse, dims)

            For i As Integer = 0 To m - 1
                Dim rowIndex = block(i).rowIndex
                Dim parts As New List(Of String) From {rowIndex.ToString}
                For d As Integer = 0 To dims - 1
                    parts.Add(embedding(i)(d).ToString("G17"))
                Next
                svdWriter.WriteLine(String.Join(vbTab, parts))
            Next
        End Sub

        Private Shared Function ColumnCountPlaceholder(block As List(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double()))) As Integer
            Dim maxCol As Integer = 0
            For Each row In block
                For Each col In row.cols
                    If col > maxCol Then maxCol = col
                Next
            Next
            Return maxCol + 1
        End Function

        Public Shared Function LoadMeta(workDir As String) As (rows As Integer, dims As Integer)
            Dim json = CType(File.ReadAllText(System.IO.Path.Combine(workDir, SVD_META_FILE)).LoadObject(GetType(Dictionary(Of String, String))), Dictionary(Of String, String))
            Return (CInt(Val(json("rows"))), CInt(Val(json("dims"))))
        End Function

        ''' <summary>
        ''' stream the dense embeddings back row-by-row (rowIndex, embedding vector) so the KNN
        ''' stage can consume them in blocks.
        ''' </summary>
        Public Shared Iterator Function ReadEmbeddings(workDir As String) As IEnumerable(Of (rowIndex As Integer, vector As Double()))
            Dim path As String = System.IO.Path.Combine(workDir, SVD_FILE)

            Using reader = New StreamReader(New BufferedStream(New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1 << 20)))
                Dim line As String = Nothing
                Do
                    line = reader.ReadLine()
                    If line Is Nothing Then Exit Do

                    Dim parts = line.Split(vbTab)
                    Dim rowIndex = CInt(Val(parts(0)))
                    Dim vec(parts.Length - 2) As Double
                    For i As Integer = 1 To parts.Length - 1
                        vec(i - 1) = Val(parts(i))
                    Next
                    Yield (rowIndex, vec)
                Loop
            End Using
        End Function
    End Class
End Namespace

