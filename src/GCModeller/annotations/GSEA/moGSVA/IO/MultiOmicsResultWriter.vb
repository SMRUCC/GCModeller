Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSVA

Namespace Export

    ''' <summary>
    ''' 把 <see cref="MultiOmicsGsvaResult"/> 导出为可读的 csv 表格。
    ''' </summary>
    ''' <remarks>
    ''' 数值一律使用 <c>G17</c> + InvariantCulture 写出，保证与 R/Python 端比对时不丢精度。
    ''' </remarks>
    Public Module MultiOmicsResultWriter

        ''' <summary>
        ''' 导出全部结果表，返回写出的文件列表。
        ''' </summary>
        ''' <param name="result">分析结果</param>
        ''' <param name="outDir">输出目录（不存在时自动创建）</param>
        Public Function Save(result As MultiOmicsGsvaResult, outDir As String) As String()
            If result Is Nothing Then
                Throw New ArgumentNullException(NameOf(result))
            End If
            If String.IsNullOrWhiteSpace(outDir) Then
                Throw New ArgumentException("the output directory can not be empty!", NameOf(outDir))
            End If
            If Not Directory.Exists(outDir) Then
                Call Directory.CreateDirectory(outDir)
            End If

            Dim files As New List(Of String)

            files.Add(WriteMatrix(result.Scores, Path.Combine(outDir, "scores.csv"), "pathway"))
            files.Add(WriteMatrix(result.RawScores, Path.Combine(outDir, "raw_scores.csv"), "pathway"))
            files.Add(WriteMatrix(result.Consistency, Path.Combine(outDir, "consistency.csv"), "pathway"))
            files.Add(WriteLayerScores(result, Path.Combine(outDir, "layer_scores.csv")))
            files.Add(WriteWeights(result, Path.Combine(outDir, "weights.csv")))
            files.Add(WriteContributions(result, Path.Combine(outDir, "contributions.csv")))
            files.Add(WriteCoverage(result, Path.Combine(outDir, "coverage.csv")))
            files.Add(WriteSignificance(result, Path.Combine(outDir, "significance.csv")))

            If result.Pooled IsNot Nothing AndAlso result.Pooled.Available Then
                files.Add(WritePooled(result.Pooled, Path.Combine(outDir, "pooled_diagnostics.csv")))
                files.Add(WriteMatrix(result.Pooled.Scores, Path.Combine(outDir, "pooled_scores.csv"), "pathway"))
            End If

            Return files.ToArray()
        End Function

        ''' <summary>
        ''' 写出「通路 × 样本」矩阵。
        ''' </summary>
        Public Function WriteMatrix(mat As Matrix, path As String, Optional idColumn As String = "geneID") As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine((New String() {idColumn}).Join(mat.sampleID).JoinBy(","))

                For Each row As DataFrameRow In mat.expression
                    Dim line As New List(Of String) From {Quote(row.geneID)}

                    For Each x As Double In row.experiments
                        line.Add(Format(x))
                    Next

                    Call writer.WriteLine(line.JoinBy(","))
                Next
            End Using

            Return path
        End Function

        Private Function WriteLayerScores(result As MultiOmicsGsvaResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("omics,pathway,sample,score")

                For Each omics As String In result.LayerScores.Keys
                    Dim mat As Matrix = result.LayerScores(omics)

                    If mat Is Nothing Then
                        Continue For
                    End If

                    For Each row As DataFrameRow In mat.expression
                        For j As Integer = 0 To row.experiments.Length - 1
                            Call writer.WriteLine($"{Quote(omics)},{Quote(row.geneID)},{Quote(result.SampleID(j))},{Format(row.experiments(j))}")
                        Next
                    Next
                Next
            End Using

            Return path
        End Function

        Private Function WriteWeights(result As MultiOmicsGsvaResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("pathway,omics,weight,member_count,skipped")

                For Each pathway As String In result.Pathways
                    Dim weights As Dictionary(Of String, Double) = result.Weights(pathway)
                    Dim coverage As Dictionary(Of String, Integer) = result.Coverage(pathway)
                    Dim skipped As String() = result.SkippedLayers(pathway)

                    For Each omics As String In result.OmicsNames
                        Dim w As Double = If(weights.ContainsKey(omics), weights(omics), 0)
                        Dim n As Integer = If(coverage.ContainsKey(omics), coverage(omics), 0)
                        Dim flag As String = If(skipped.Contains(omics, StringComparer.Ordinal), "TRUE", "FALSE")

                        Call writer.WriteLine($"{Quote(pathway)},{Quote(omics)},{Format(w)},{n},{flag}")
                    Next
                Next
            End Using

            Return path
        End Function

        Private Function WriteCoverage(result As MultiOmicsGsvaResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("pathway,omics,member_count,weight")

                For Each pathway As String In result.Pathways
                    For Each omics As String In result.OmicsNames
                        Dim n As Integer = result.MemberCountOf(pathway, omics)

                        Call writer.WriteLine($"{Quote(pathway)},{Quote(omics)},{n},{Format(result.WeightOf(pathway, omics))}")
                    Next
                Next
            End Using

            Return path
        End Function

        Private Function WriteContributions(result As MultiOmicsGsvaResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("pathway,sample,omics,contribution,share,dominant")

                For Each pathway As String In result.Pathways
                    Dim contribution As Dictionary(Of String, Double()) = result.Contributions(pathway)
                    Dim share As Dictionary(Of String, Double()) = result.ContributionShare(pathway)
                    Dim dominant As String() = result.DominantOmics(pathway)

                    For j As Integer = 0 To result.SampleID.Length - 1
                        For Each omics As String In result.OmicsNames
                            Dim c As Double = If(contribution.ContainsKey(omics), contribution(omics)(j), 0)
                            Dim s As Double = If(share.ContainsKey(omics), share(omics)(j), 0)
                            Dim flag As String = If(dominant(j).TextEquals(omics, null_equals:=False, empty_equals:=False), "TRUE", "FALSE")

                            Call writer.WriteLine($"{Quote(pathway)},{Quote(result.SampleID(j))},{Quote(omics)},{Format(c)},{Format(s)},{flag}")
                        Next
                    Next
                Next
            End Using

            Return path
        End Function

        Private Function WriteSignificance(result As MultiOmicsGsvaResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("pathway,statistic,p_value,q_value")

                If result.Significance Is Nothing Then
                    ' 没跑置换检验时仍然写一个只有表头的文件，保持下游读取逻辑一致
                    Return path
                End If

                For Each pathway As String In result.Pathways
                    Dim statistic As Double
                    Dim ratio As Double = result.PValueOf(pathway)
                    Dim q As Double = result.QValueOf(pathway)

                    If result.Significance.ObservedStatistics.TryGetValue(pathway, statistic) Then
                        Call writer.WriteLine($"{Quote(pathway)},{Format(statistic)},{Format(ratio)},{Format(q)}")
                    End If
                Next
            End Using

            Return path
        End Function

        Private Function WritePooled(pooled As Ablation.PooledBaselineResult, path As String) As String
            Using writer As New StreamWriter(path, False, Text.Encoding.UTF8)
                Call writer.WriteLine("pathway,pearson_r")

                For Each pathway As String In pooled.Correlation.Keys
                    Call writer.WriteLine($"{Quote(pathway)},{Format(pooled.Correlation(pathway))}")
                Next
            End Using

            Return path
        End Function

        Private Function Format(x As Double) As String
            If Double.IsNaN(x) Then
                Return "NA"
            ElseIf Double.IsInfinity(x) Then
                Return If(x > 0, "Inf", "-Inf")
            Else
                Return x.ToString("G17", CultureInfo.InvariantCulture)
            End If
        End Function

        Private Function Quote(s As String) As String
            If s Is Nothing Then
                Return """"""""
            End If

            Return """" & s.Replace("""", """""") & """"
        End Function

    End Module

End Namespace
