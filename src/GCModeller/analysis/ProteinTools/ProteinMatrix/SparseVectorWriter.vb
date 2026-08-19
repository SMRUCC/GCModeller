Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ProteinStructure

    ''' <summary>
    ''' stream-friendly (coordinate list) writer / reader for the per-sequence TF-IDF sparse
    ''' vectors produced by the first pass of the streaming pipeline.
    '''
    ''' the on-disk layout is deliberately simple and append-friendly so that the first pass can
    ''' keep a single opened <see cref="BufferedStream"/> and emit one row per sequence without
    ''' ever holding the whole matrix in memory:
    '''
    ''' - <c>vectors.coo</c> : tab-separated "<paramref name="rowIndex"/> &lt;TAB&gt; <paramref name="colIndex"/> &lt;TAB&gt; <paramref name="value"/>" lines, grouped by row.
    ''' - <c>titles.idx</c>  : JSON array mapping <paramref name="rowIndex"/> -&gt; original sequence title, written once at the end.
    ''' - <c>meta.json</c>   : number of rows / columns so the second pass knows the matrix shape.
    ''' </summary>
    Public Class SparseVectorWriter

        Public Const COO_FILE As String = "tfidf_vectors.coo"
        Public Const TITLE_INDEX_FILE As String = "titles.idx"
        Public Const META_FILE As String = "tfidf_meta.json"

        Private ReadOnly workDir As String
        Private cooStream As StreamWriter
        Private titleIndex As New List(Of String)
        Private nCols As Integer

        Public ReadOnly Property RowCount As Integer
            Get
                Return titleIndex.Count
            End Get
        End Property

        Public ReadOnly Property ColumnCount As Integer
            Get
                Return nCols
            End Get
        End Property

        Public Sub New(workDir As String, nCols As Integer)
            Me.workDir = workDir
            Me.nCols = nCols
            Call Directory.CreateDirectory(workDir)
        End Sub

        ''' <summary>
        ''' reader-only constructor (column count is loaded from meta when needed)
        ''' </summary>
        Public Sub New(workDir As String)
            Me.workDir = workDir
            Dim meta = LoadMeta(workDir)
            Me.nCols = meta.cols
        End Sub

        ''' <summary>
        ''' open the append stream for writing COO rows (call once before <see cref="WriteRow"/>)
        ''' </summary>
        Public Sub OpenForWrite()
            Dim path = System.IO.Path.Combine(workDir, COO_FILE)
            cooStream = New StreamWriter(New BufferedStream(New FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20)), Encoding.ASCII)
        End Sub

        ''' <summary>
        ''' write one sequence as a set of COO triples; the row index is implied by call order.
        ''' </summary>
        Public Sub WriteRow(title As String, cols As Integer(), vals As Double())
            Dim row = titleIndex.Count
            titleIndex.Add(title)

            For i As Integer = 0 To cols.Length - 1
                cooStream.WriteLine($"{row}" & vbTab & cols(i) & vbTab & vals(i).ToString("G17"))
            Next
        End Sub

        Public Sub CloseForWrite()
            If cooStream IsNot Nothing Then
                cooStream.Flush()
                cooStream.Dispose()
                cooStream = Nothing
            End If

            ' persist the title index and matrix shape
            Call File.WriteAllText(System.IO.Path.Combine(workDir, TITLE_INDEX_FILE), titleIndex.ToArray.GetJson)
            Call File.WriteAllText(System.IO.Path.Combine(workDir, META_FILE), New Dictionary(Of String, String) From {
                {"rows", titleIndex.Count.ToString},
                {"cols", nCols.ToString}
            }.GetJson)
        End Sub

        Public Shared Function LoadMeta(workDir As String) As (rows As Integer, cols As Integer)
            Dim json = CType(File.ReadAllText(System.IO.Path.Combine(workDir, META_FILE)).LoadObject(GetType(Dictionary(Of String, String))), Dictionary(Of String, String))
            Return (CInt(Val(json("rows"))), CInt(Val(json("cols"))))
        End Function

        Public Shared Function LoadTitleIndex(workDir As String) As String()
            Return CType(File.ReadAllText(System.IO.Path.Combine(workDir, TITLE_INDEX_FILE)).LoadObject(GetType(String())), String())
        End Function

        ''' <summary>
        ''' stream the COO rows back in order, yielding (rowIndex, title, columnIndices, values)
        ''' blocks. the caller feeds each block into the next stage (SVD) so the whole matrix is
        ''' never materialized at once.
        ''' </summary>
        Public Iterator Function ReadRows() As IEnumerable(Of (rowIndex As Integer, title As String, cols As Integer(), vals As Double()))
            Dim titles = LoadTitleIndex(workDir)
            Dim rows = New Dictionary(Of Integer, (cols As List(Of Integer), vals As List(Of Double)))
            Dim currentRow As Integer = -1
            Dim path = System.IO.Path.Combine(workDir, COO_FILE)

            Using reader = New StreamReader(New BufferedStream(New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1 << 20)))
                Dim line As String = Nothing

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If

                    Dim parts = line.Split(vbTab)
                    Dim r = CInt(Val(parts(0)))
                    Dim c = CInt(Val(parts(1)))
                    Dim v = Val(parts(2))

                    If currentRow = -1 Then
                        currentRow = r
                    End If

                    If r <> currentRow Then
                        ' flush previous row
                        Dim prev = rows(currentRow)
                        Yield (currentRow, titles(currentRow), prev.cols.ToArray, prev.vals.ToArray)
                        rows.Remove(currentRow)
                        currentRow = r
                    End If

                    If Not rows.ContainsKey(r) Then
                        rows(r) = (New List(Of Integer), New List(Of Double))
                    End If

                    rows(r).cols.Add(c)
                    rows(r).vals.Add(v)
                Loop
            End Using

            ' flush the last row
            If currentRow >= 0 AndAlso rows.ContainsKey(currentRow) Then
                Dim prev = rows(currentRow)
                Yield (currentRow, titles(currentRow), prev.cols.ToArray, prev.vals.ToArray)
            End If
        End Function
    End Class
End Namespace
