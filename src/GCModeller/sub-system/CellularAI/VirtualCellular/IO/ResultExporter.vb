Imports System
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

Namespace IO

    ''' <summary>
    ''' 把虚拟扰动结果导出为文本：表达矩阵、差异表达表、Top 变化清单。
    ''' </summary>
    ''' <remarks>
    ''' 表达矩阵按"基因 × 细胞"写出（与输入 CSV 同向，便于直接回灌上游），
    ''' 每批 200 行 flush 一次，避免把 23,450 × 400 的矩阵拼成巨型字符串。
    ''' </remarks>
    Public Module ResultExporter

        ''' <summary>写表达矩阵（行=基因，列=细胞）。</summary>
        Public Sub WriteExpression(block As CsrMatrix, genes As String(), cells As String(), path As String)
            Using writer As New StreamWriter(path, append:=False, encoding:=New UTF8Encoding(False))
                Dim builder As New StringBuilder("name")

                For c As Integer = 0 To cells.Length - 1
                    Call builder.Append(",").Append(cells(c))
                Next

                Call writer.WriteLine(builder.ToString())

                ' 按细胞行展开成"逐基因"的列向量
                Dim column As Double() = New Double(cells.Length - 1) {}
                Dim batch As Integer = 0

                For g As Integer = 0 To genes.Length - 1
                    Call Array.Clear(column, 0, column.Length)

                    Dim builder2 As New StringBuilder(genes(g))

                    For c As Integer = 0 To block.RowDimension - 1
                        For k As Integer = block.Indptr(c) To block.Indptr(c + 1) - 1
                            If block.Indices(k) = g Then
                                column(c) = block.Values(k)
                            End If
                        Next
                    Next

                    For c As Integer = 0 To column.Length - 1
                        Call builder2.Append(",").Append(column(c).ToString("G17"))
                    Next

                    Call writer.WriteLine(builder2.ToString())
                    batch += 1

                    If batch Mod 200 = 0 Then
                        Call writer.Flush()
                    End If
                Next
            End Using
        End Sub

        ''' <summary>写差异表达表（显著基因优先）。</summary>
        Public Sub WriteDe(result As InterventionResult, path As String)
            Using writer As New StreamWriter(path, append:=False, encoding:=New UTF8Encoding(False))
                Call writer.WriteLine("gene,log2fc,percent,zscore,significant,target")

                Dim targets As New HashSet(Of String)(
                    If(result.Spec?.GeneName, "").Split("+"c), StringComparer.Ordinal)

                Dim order As Integer() = Enumerable.Range(0, result.GeneNames.Length).
                    OrderByDescending(Function(i) std.Abs(result.FoldChanges(i))).
                    ToArray

                For Each i As Integer In order
                    Call writer.WriteLine(
                        $"{result.GeneNames(i)},{Log2Ratio(result, i):G6},{result.PercentChanges(i):G6}," &
                        $"{result.ZScores(i):G6},{result.IsSignificant(i).ToString().ToLowerInvariant()}," &
                        $"{targets.Contains(result.GeneNames(i)).ToString().ToLowerInvariant()}")
                Next
            End Using
        End Sub

        ''' <summary>写 Top N 变化清单。</summary>
        Public Sub WriteSummary(result As InterventionResult, path As String, Optional n As Integer = 200)
            Using writer As New StreamWriter(path, append:=False, encoding:=New UTF8Encoding(False))
                Call writer.WriteLine("rank,gene,log2fc,percent,zscore")

                Dim ranked As List(Of (Gene As String, Fold As Double, Percent As Double, Z As Double)) =
                    result.GetTopChangedGenes(n).
                        Select(Function(t) (t.GeneName, t.FoldChange, t.PercentChange,
                                            result.ZScores(Array.IndexOf(result.GeneNames, t.GeneName)))).
                        ToList()

                For i As Integer = 0 To ranked.Count - 1
                    Dim item = ranked(i)

                    Call writer.WriteLine($"{i + 1},{item.Gene},{item.Fold:G6},{item.Percent:G6},{item.Z:G6}")
                Next
            End Using
        End Sub

        ''' <summary>
        ''' <c>log2((mutant + 1) / (wildtype + 1))</c>：<see cref="InterventionResult.FoldChanges"/>
        ''' 存的是"差值"，报告里的 lfc 需要在对数尺度上，因此在这里换算。
        ''' </summary>
        Private Function Log2Ratio(result As InterventionResult, i As Integer) As Double
            Dim wildtype As Double = If(result.WildtypeMeans Is Nothing, 0.0, result.WildtypeMeans(i))
            Dim mutant As Double = If(result.MutantMeans Is Nothing, 0.0, result.MutantMeans(i))

            Return std.Log((mutant + 1.0) / (wildtype + 1.0), 2.0)
        End Function

    End Module
End Namespace
