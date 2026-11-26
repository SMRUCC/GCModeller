Imports System
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Summary
Imports SMRUCC.genomics.Analysis.VirtualCellular.Data

Namespace Model

    ''' <summary>模型包的构建清单。</summary>
    Public Class ModelManifest

        ''' <summary>包格式版本；加载时严格比对。</summary>
        Public Property FormatVersion As Integer = 1

        Public Property CreatedAt As String
        Public Property GeneCount As Integer
        Public Property CellCount As Integer
        Public Property NonZeros As Long
        Public Property Seed As Integer

        ''' <summary>参与装配的 view 名。</summary>
        Public Property Views As String() = {}

        ''' <summary>全部伪扰动名（并集）。</summary>
        Public Property Perturbations As String() = {}

        ''' <summary>基因轴指纹（SHA256 前 16 字节的十六进制）。</summary>
        Public Property GeneAxisHash As String

        ''' <summary>正常对照的判定方式（<c>stage=NA</c> 或 <c>type=normal</c>）。</summary>
        Public Property NormalSelector As String

        ''' <summary>实际使用的期别过滤。</summary>
        Public Property StageFilter As String() = {}

        ''' <summary>按基因轴算指纹。</summary>
        Public Shared Function HashOf(genes As String()) As String
            Using sha As SHA256 = SHA256.Create()
                Dim payload As String = String.Join(vbLf, If(genes, New String() {}))
                Dim digest As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes(payload))
                Dim builder As New StringBuilder()

                For i As Integer = 0 To 15
                    Call builder.Append(digest(i).ToString("x2"))
                Next

                Return builder.ToString()
            End Using
        End Function

    End Class

    ''' <summary>
    ''' 可持久化的虚拟细胞模型：参数 + 三个 view 的摘要 + 对照矩阵 + 细胞元数据。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 保存的是"装配结果"，不是原始 CSV：加载之后可以直接跑
    ''' <c>ContextPipeline.Predict</c>，无需再解析 4.6 GB 表达矩阵。
    ''' </para>
    ''' <para>
    ''' 版本与基因轴指纹在加载时严格校验：换了一套基因轴却复用旧模型，会让面板与矩阵错位，
    ''' 这种错误在数值上表现为"结果看起来还行但完全不对"，必须在入口处拒绝。
    ''' </para>
    ''' </remarks>
    Public Class VirtualCellModel

        ''' <summary>CellularAI 参数。</summary>
        Public Property Options As CellularAiModel

        ''' <summary>训练摘要（三个 view）。</summary>
        Public Property Bundle As SummaryBundle

        ''' <summary>对照矩阵（细胞 × 基因）。</summary>
        Public Property Controls As CsrMatrix

        ''' <summary>基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>细胞轴（barcode）。</summary>
        Public Property Cells As String()

        ''' <summary>细胞元数据。</summary>
        Public Property Metadata As CellMetadata

        ''' <summary>构建清单。</summary>
        Public Property Manifest As ModelManifest

        ''' <summary>全部伪扰动名（各 view 的并集，保持首次出现顺序）。</summary>
        Public ReadOnly Property Perturbations As String()
            Get
                Dim names As New List(Of String)
                Dim seen As New HashSet(Of String)(StringComparer.Ordinal)
                Dim rows As SummaryRow() = If(Bundle?.Catalog?.Rows, New SummaryRow() {})

                For Each row As SummaryRow In rows
                    For Each name As String In If(row.EffectPerts, New String() {})
                        If seen.Add(name) Then
                            Call names.Add(name)
                        End If
                    Next
                Next

                Return names.ToArray
            End Get
        End Property

        ''' <summary>某个 view 下的扰动名。</summary>
        Public Function PerturbationsOf(view As String) As String()
            Dim names As New List(Of String)
            Dim seen As New HashSet(Of String)(StringComparer.Ordinal)
            Dim rows As SummaryRow() = If(Bundle?.Catalog?.Rows, New SummaryRow() {})

            For Each row As SummaryRow In rows
                If Not String.Equals(row.View, view, StringComparison.Ordinal) Then
                    Continue For
                End If

                For Each name As String In If(row.EffectPerts, New String() {})
                    If seen.Add(name) Then
                        Call names.Add(name)
                    End If
                Next
            Next

            Return names.ToArray
        End Function

        ''' <summary>基因名 → 列下标。</summary>
        Public Function GeneIndex() As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To If(Genes, New String() {}).Length - 1
                map(Genes(i)) = i
            Next

            Return map
        End Function

        ''' <summary>保存到 ZIP 包。</summary>
        Public Sub Save(stream As Stream)
            Call ModelStorage.Write(stream, Me)
        End Sub

        ''' <summary>从 ZIP 包加载。</summary>
        Public Shared Function Load(stream As Stream) As VirtualCellModel
            Return ModelStorage.Read(stream)
        End Function

        Public Overrides Function ToString() As String
            Return $"{GeneCount} gene x {CellCount} cell, {Perturbations.Length} pseudo-perturbation(s)"
        End Function

        Public ReadOnly Property GeneCount As Integer
            Get
                Return If(Genes, New String() {}).Length
            End Get
        End Property

        Public ReadOnly Property CellCount As Integer
            Get
                Return If(Cells, New String() {}).Length
            End Get
        End Property

    End Class
End Namespace
