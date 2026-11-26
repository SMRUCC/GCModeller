Imports System
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Namespace Model

    ''' <summary>
    ''' 模型包的读写：ZIP + 定长二进制面板 + 文本清单。
    ''' </summary>
    ''' <remarks>
    ''' <para>条目：</para>
    ''' <list type="bullet">
    ''' <item><description><c>manifest.tsv</c>：键值文本（版本、轴长、nnz、指纹、view、扰动名、种子）；</description></item>
    ''' <item><description><c>genes.txt</c> / <c>cells.csv</c>：基因轴与细胞元数据（后者复用 <see cref="CellMetadata.Load"/> 的解析器）；</description></item>
    ''' <item><description><c>controls.bin</c>：<c>rows, cols, nnz</c> + <c>indptr/indices/values</c>；</description></item>
    ''' <item><description><c>panel.&lt;view&gt;.bin</c>：该 view 下每条记录的 <c>effect/probability/offset</c>。</description></item>
    ''' </list>
    ''' <para>
    ''' 全部小端、定长、行主序：<c>BinaryWriter</c> 在 .NET 上就是小端，
    ''' 不引入任何第三方序列化，跨平台可读。
    ''' </para>
    ''' </remarks>
    Public Module ModelStorage

        Private Const EntryManifest As String = "manifest.tsv"
        Private Const EntryGenes As String = "genes.txt"
        Private Const EntryCells As String = "cells.csv"
        Private Const EntryControls As String = "controls.bin"
        Private Const FormatVersion As Integer = 1

        ''' <summary>写入模型包。</summary>
        Public Sub Write(stream As Stream, model As VirtualCellModel)
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If
            If model?.Bundle?.Catalog Is Nothing Then
                Throw New ArgumentException("the model has no summary catalog", NameOf(model))
            End If

            Dim manifest As ModelManifest = If(model.Manifest, New ModelManifest())

            manifest.FormatVersion = FormatVersion
            manifest.CreatedAt = If(manifest.CreatedAt, DateTime.Now.ToString("o"))
            manifest.GeneCount = model.GeneCount
            manifest.CellCount = model.CellCount
            manifest.NonZeros = If(model.Controls, Nothing)?.NonZeros
            manifest.GeneAxisHash = ModelManifest.HashOf(model.Genes)
            manifest.Perturbations = model.Perturbations
            manifest.Views = model.Bundle.Catalog.Rows.
                Select(Function(r) r.View).Distinct().ToArray()
            manifest.NormalSelector = If(model.Metadata?.NormalSelector, "")

            Using zip As New ZipArchive(stream, ZipArchiveMode.Create, leaveOpen:=True)
                Call WriteText(zip, EntryManifest, ManifestText(manifest))
                Call WriteText(zip, EntryGenes, String.Join(vbLf, If(model.Genes, New String() {})))
                Call WriteText(zip, EntryCells, CellsCsv(model))

                If model.Controls IsNot Nothing Then
                    Call WriteControls(zip, EntryControls, model.Controls)
                End If

                For Each view As String In manifest.Views
                    Call WritePanel(zip, $"panel.{view}.bin", model, view)
                Next
            End Using
        End Sub

        ''' <summary>读取模型包。</summary>
        Public Function Read(stream As Stream) As VirtualCellModel
            If stream Is Nothing Then
                Throw New ArgumentNullException(NameOf(stream))
            End If

            Using zip As New ZipArchive(stream, ZipArchiveMode.Read, leaveOpen:=True)
                Dim manifest As ModelManifest = ParseManifest(ReadText(zip, EntryManifest))

                If manifest.FormatVersion <> FormatVersion Then
                    Throw New InvalidDataException(
                        $"model format v{manifest.FormatVersion} is not supported (expected v{FormatVersion})")
                End If

                Dim genes As String() = ReadText(zip, EntryGenes).Split(ControlChars.Lf)
                Dim cells As String() = ReadCellsCsv(zip, manifest, genes)
                Dim controls As CsrMatrix = ReadControls(zip, EntryControls)
                Dim rows As New List(Of SummaryRow)

                For Each view As String In manifest.Views
                    Call ReadPanel(zip, $"panel.{view}.bin", genes, rows)
                Next

                Dim metadata As CellMetadata = ReadMetadata(zip, EntryCells)

                If String.IsNullOrEmpty(manifest.GeneAxisHash) = False Then
                    Dim actual As String = ModelManifest.HashOf(genes)

                    If Not String.Equals(actual, manifest.GeneAxisHash, StringComparison.Ordinal) Then
                        Throw New InvalidDataException(
                            $"gene axis mismatch: manifest {manifest.GeneAxisHash}, payload {actual}")
                    End If
                End If

                Return New VirtualCellModel With {
                    .Options = CellularAiModel.Default(),
                    .Bundle = New SummaryBundle With {
                        .Catalog = New SummaryCatalog With {
                            .PrimaryView = If(manifest.Views.Length > 0, manifest.Views(0), "stage"),
                            .Rows = rows.ToArray
                        }
                    },
                    .Controls = controls,
                    .Genes = genes,
                    .Cells = cells,
                    .Metadata = metadata,
                    .Manifest = manifest
                }
            End Using
        End Function

#Region "entries"

        Private Sub WriteText(zip As ZipArchive, name As String, text As String)
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using writer As New StreamWriter(entry.Open(), Encoding.UTF8)
                Call writer.Write(text)
            End Using
        End Sub

        Private Function ReadText(zip As ZipArchive, name As String) As String
            Dim entry As ZipArchiveEntry = zip.GetEntry(name)

            If entry Is Nothing Then
                Throw New InvalidDataException($"the model package has no '{name}' entry")
            End If

            Using reader As New StreamReader(entry.Open(), Encoding.UTF8)
                Return reader.ReadToEnd()
            End Using
        End Function

        Private Sub WriteControls(zip As ZipArchive, name As String, controls As CsrMatrix)
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using writer As New BinaryWriter(entry.Open(), Encoding.UTF8)
                Call writer.Write(controls.RowDimension)
                Call writer.Write(controls.ColumnDimension)
                Call writer.Write(CInt(controls.NonZeros))

                For i As Integer = 0 To controls.Indptr.Length - 1
                    Call writer.Write(controls.Indptr(i))
                Next
                For i As Integer = 0 To controls.Indices.Length - 1
                    Call writer.Write(controls.Indices(i))
                Next
                For i As Integer = 0 To controls.Values.Length - 1
                    Call writer.Write(controls.Values(i))
                Next
            End Using
        End Sub

        Private Function ReadControls(zip As ZipArchive, name As String) As CsrMatrix
            Dim entry As ZipArchiveEntry = zip.GetEntry(name)

            If entry Is Nothing Then
                Return Nothing
            End If

            Using reader As New BinaryReader(entry.Open(), Encoding.UTF8)
                Dim rows As Integer = reader.ReadInt32()
                Dim columns As Integer = reader.ReadInt32()
                Dim nnz As Integer = reader.ReadInt32()
                Dim indptr(rows) As Integer
                Dim indices(If(nnz = 0, -1, nnz - 1)) As Integer
                Dim values(If(nnz = 0, -1, nnz - 1)) As Double

                For i As Integer = 0 To rows
                    indptr(i) = reader.ReadInt32()
                Next
                For i As Integer = 0 To nnz - 1
                    indices(i) = reader.ReadInt32()
                Next
                For i As Integer = 0 To nnz - 1
                    values(i) = reader.ReadDouble()
                Next

                Return New CsrMatrix(rows, columns, indptr, indices, values)
            End Using
        End Function

        Private Sub WritePanel(zip As ZipArchive, name As String, model As VirtualCellModel, view As String)
            Dim rows As SummaryRow() = model.Bundle.Catalog.Rows.
                Where(Function(r) String.Equals(r.View, view, StringComparison.Ordinal)).ToArray()
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using writer As New BinaryWriter(entry.Open(), Encoding.UTF8)
                Call writer.Write(rows.Length)

                For Each row As SummaryRow In rows
                    Dim perms As String() = If(row.EffectPerts, New String() {})

                    Call writer.Write(If(row.CellLine, ""))
                    Call writer.Write(perms.Length)

                    For Each pert As String In perms
                        Call writer.Write(pert)
                    Next

                    For g As Integer = 0 To model.GeneCount - 1
                        For p As Integer = 0 To perms.Length - 1
                            Call writer.Write(row.Effect(g)(p))
                            Call writer.Write(row.Probability(g)(p))
                        Next
                    Next

                    For g As Integer = 0 To model.GeneCount - 1
                        Call writer.Write(If(row.Offset?.EffectCenter, New Double(model.GeneCount - 1) {})(g))
                        Call writer.Write(If(row.Offset?.EffectCount, New Double(model.GeneCount - 1) {})(g))
                        Call writer.Write(If(row.Offset?.ResponseVariance, New Double(model.GeneCount - 1) {})(g))
                    Next
                Next
            End Using
        End Sub

        Private Sub ReadPanel(zip As ZipArchive, name As String, genes As String(), rows As List(Of SummaryRow))
            Dim entry As ZipArchiveEntry = zip.GetEntry(name)

            If entry Is Nothing Then
                Return
            End If

            Dim view As String = name.Substring("panel.".Length)
            view = view.Substring(0, view.Length - ".bin".Length)

            Using reader As New BinaryReader(entry.Open(), Encoding.UTF8)
                Dim count As Integer = reader.ReadInt32()

                For i As Integer = 0 To count - 1
                    Dim cellLine As String = reader.ReadString()
                    Dim nPerts As Integer = reader.ReadInt32()
                    Dim perms As String() = New String(nPerts - 1) {}

                    For p As Integer = 0 To nPerts - 1
                        perms(p) = reader.ReadString()
                    Next

                    Dim effect(genes.Length - 1)() As Double
                    Dim probability(genes.Length - 1)() As Double

                    For g As Integer = 0 To genes.Length - 1
                        effect(g) = New Double(nPerts - 1) {}
                        probability(g) = New Double(nPerts - 1) {}

                        For p As Integer = 0 To nPerts - 1
                            effect(g)(p) = reader.ReadDouble()
                            probability(g)(p) = reader.ReadDouble()
                        Next
                    Next

                    Dim center(genes.Length - 1) As Double
                    Dim measured(genes.Length - 1) As Double
                    Dim variance(genes.Length - 1) As Double

                    For g As Integer = 0 To genes.Length - 1
                        center(g) = reader.ReadDouble()
                        measured(g) = reader.ReadDouble()
                        variance(g) = reader.ReadDouble()
                    Next

                    Call rows.Add(New SummaryRow With {
                        .CellLine = cellLine,
                        .View = view,
                        .EffectGenes = genes,
                        .EffectPerts = perms,
                        .Effect = effect,
                        .Probability = probability,
                        .Control = New ControlProfile With {.Genes = genes, .Counts = New Double(genes.Length - 1) {}},
                        .Offset = New OffsetTable With {
                            .CellLine = cellLine,
                            .View = view,
                            .Genes = genes,
                            .EffectCenter = center,
                            .EffectCount = measured,
                            .SignedScoreCenter = New Double(genes.Length - 1) {},
                            .ResponseVariance = variance
                        }
                    })
                Next
            End Using
        End Sub

#End Region

#Region "cells & manifest"

        Private Function CellsCsv(model As VirtualCellModel) As String
            Dim builder As New StringBuilder()

            Call builder.AppendLine("ID,sample_name,patient_id,stage,type")

            For Each barcode As String In If(model.Cells, New String() {})
                Dim info As SampleInfo = model.Metadata?.Find(barcode)
                Dim patient As String = CellMetadata.ValueOf(info, "patient_id")
                Dim stage As String = CellMetadata.ValueOf(info, "stage")
                Dim tissue As String = CellMetadata.ValueOf(info, "type")

                Call builder.AppendLine($"{barcode},{barcode},{If(patient, "")},{If(stage, "")},{If(tissue, "")}")
            Next

            Return builder.ToString()
        End Function

        Private Function ReadCellsCsv(zip As ZipArchive, manifest As ModelManifest, genes As String()) As String()
            Dim text As String = ReadText(zip, EntryCells)
            Dim lines As String() = text.Split(ControlChars.Lf)
            Dim cells As New List(Of String)

            For i As Integer = 1 To lines.Length - 1
                Dim line As String = lines(i).Trim()

                If line.Length = 0 Then
                    Continue For
                End If

                Call cells.Add(line.Split(","c)(0))
            Next

            Return cells.ToArray
        End Function

        Private Function ReadMetadata(zip As ZipArchive, name As String) As CellMetadata
            Dim text As String = ReadText(zip, name)
            Dim temp As String = Path.Combine(Path.GetTempPath(), $"vcm-{Guid.NewGuid():N}.csv")

            Try
                Call File.WriteAllText(temp, text, Encoding.UTF8)

                Return CellMetadata.Load(temp)
            Finally
                Try
                    Call File.Delete(temp)
                Catch ex As Exception
                    ' 临时文件清理失败不影响模型加载
                End Try
            End Try
        End Function

        Private Function ManifestText(manifest As ModelManifest) As String
            Dim builder As New StringBuilder()

            Call builder.AppendLine($"version={manifest.FormatVersion}")
            Call builder.AppendLine($"created={manifest.CreatedAt}")
            Call builder.AppendLine($"genes={manifest.GeneCount}")
            Call builder.AppendLine($"cells={manifest.CellCount}")
            Call builder.AppendLine($"nnz={manifest.NonZeros}")
            Call builder.AppendLine($"seed={manifest.Seed}")
            Call builder.AppendLine($"hash={manifest.GeneAxisHash}")
            Call builder.AppendLine($"normal={manifest.NormalSelector}")
            Call builder.AppendLine($"views={String.Join(",", If(manifest.Views, New String() {}))}")
            Call builder.AppendLine($"perts={String.Join(",", If(manifest.Perturbations, New String() {}))}")

            Return builder.ToString()
        End Function

        Private Function ParseManifest(text As String) As ModelManifest
            Dim manifest As New ModelManifest()
            Dim table As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In text.Split(ControlChars.Lf)
                Dim at As Integer = line.IndexOf("="c)

                If at <= 0 Then
                    Continue For
                End If

                table(line.Substring(0, at).Trim()) = line.Substring(at + 1).Trim()
            Next

            manifest.FormatVersion = CInt(ValueOf(table, "version", "0"))
            manifest.CreatedAt = ValueOf(table, "created", "")
            manifest.GeneCount = CInt(ValueOf(table, "genes", "0"))
            manifest.CellCount = CInt(ValueOf(table, "cells", "0"))
            manifest.NonZeros = CLng(ValueOf(table, "nnz", "0"))
            manifest.Seed = CInt(ValueOf(table, "seed", "0"))
            manifest.GeneAxisHash = ValueOf(table, "hash", "")
            manifest.NormalSelector = ValueOf(table, "normal", "")
            manifest.Views = Split(ValueOf(table, "views", ""))
            manifest.Perturbations = Split(ValueOf(table, "perts", ""))

            Return manifest
        End Function

        Private Function ValueOf(table As Dictionary(Of String, String), key As String, fallback As String) As String
            Dim value As String = Nothing

            If table.TryGetValue(key, value) AndAlso Not String.IsNullOrEmpty(value) Then
                Return value
            End If

            Return fallback
        End Function

        Private Function Split(text As String) As String()
            If String.IsNullOrWhiteSpace(text) Then
                Return New String() {}
            End If

            Return text.Split(","c).Select(Function(s) s.Trim()).Where(Function(s) s.Length > 0).ToArray
        End Function

#End Region

    End Module
End Namespace
