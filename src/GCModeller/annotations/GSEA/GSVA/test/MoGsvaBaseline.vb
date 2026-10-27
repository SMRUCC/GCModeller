Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Omics
Imports std = System.Math

''' <summary>
''' moGSVA 的 R 基线驱动（Baseline 1：逐组学 GSVA 后简单平均）。
''' </summary>
''' <remarks>
''' <para>契约：</para>
''' <list type="bullet">
''' <item><description>输入写到 <c>&lt;dataDir&gt;/mogsva_input/</c></description></item>
''' <item><description>输出读自 <c>&lt;dataDir&gt;/mogsva_input/reference/</c></description></item>
''' <item><description>MOGSA / multiGSEA 属于可选基线：包缺失或调用失败时只记录 SKIP，不阻断测试</description></item>
''' </list>
''' </remarks>
Public Module MoGsvaBaseline

    ''' <summary>R 的 Rscript 路径</summary>
    Public Const RSCRIPT As String = "C:\Program Files\R\R-4.5.0\bin\Rscript.exe"

    ''' <summary>基线脚本文件名</summary>
    Public Const REF_SCRIPT As String = "mogsva_reference.R"

    ''' <summary>与 R 比对的默认容差（与既有 GSVA 测试保持一致）</summary>
    Public Const TOLERANCE As Double = 0.0000000001

    Public Class BaselineResult
        Public Property InputDir As String
        Public Property OutputDir As String
        Public Property LayerScores As New Dictionary(Of String, Dictionary(Of String, Double()))(StringComparer.Ordinal)
        Public Property MeanScores As Dictionary(Of String, Double())
        Public Property Status As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Public Property Message As String
        Public Property Ran As Boolean
        Public Property RawOutput As String

        Public ReadOnly Property Available As Boolean
            Get
                Return Ran AndAlso MeanScores IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property SkippedNames As String()
            Get
                Return Status.Where(Function(p) p.Value.StartsWith("SKIP")).Select(Function(p) p.Key).ToArray
            End Get
        End Property
    End Class

    Public Class CompareResult
        Public Property Compared As Integer
        Public Property MaxDifference As Double
        Public Property MeanDifference As Double
        Public Property WorstPathway As String
        Public Property BadPathways As Integer
        Public Property MissingPathways As Integer
        Public Property Details As String
    End Class

    ''' <summary>
    ''' 把多组学数据导出成 R 可以读取的输入目录。
    ''' </summary>
    Public Function PrepareInputs(real As MoGsvaData.RealDataset, inputDir As String) As String
        If Not Directory.Exists(inputDir) Then
            Call Directory.CreateDirectory(inputDir)
        End If

        Call File.WriteAllLines(Path.Combine(inputDir, "samples.txt"), real.SampleID)

        Dim features As String() = real.Dataset.Layers _
            .SelectMany(Function(l) l.Genes) _
            .Distinct(StringComparer.Ordinal) _
            .ToArray

        Call MoGsvaData.WriteGeneSets(real.GeneSets, features, Path.Combine(inputDir, "genesets.tsv"))

        For Each layer As OmicsLayer In real.Dataset.Layers
            Call MoGsvaData.WriteMatrixCsv(layer.Expr, Path.Combine(inputDir, $"layer_{layer.Name}.csv"))
        Next

        Return inputDir
    End Function

    ''' <summary>
    ''' 调用 Rscript 生成基线参考分数，并把结果读回内存。
    ''' </summary>
    Public Function Run(dataDir As String,
                        Optional installBaselines As Boolean = False,
                        Optional verbose As Boolean = True,
                        Optional omics As String() = Nothing) As BaselineResult

        Dim result As New BaselineResult With {
            .InputDir = Path.Combine(dataDir, "mogsva_input"),
            .OutputDir = Path.Combine(dataDir, "mogsva_input", "reference")
        }

        If Not File.Exists(RSCRIPT) Then
            result.Message = $"Rscript not found: {RSCRIPT}"

            Return result
        End If

        Dim script As String = LocateScript()

        If script Is Nothing Then
            result.Message = $"{REF_SCRIPT} could not be located"

            Return result
        End If

        Dim arguments As String = $"""{script}"" ""{dataDir}"""

        If installBaselines Then
            arguments &= " --install-baselines"
        End If
        If omics IsNot Nothing AndAlso omics.Length > 0 Then
            arguments &= $" --omics={String.Join(",", omics)}"
        End If

        If verbose Then
            Call $"[baseline] Rscript {arguments}".println
        End If

        Dim info As New ProcessStartInfo With {
            .FileName = RSCRIPT,
            .Arguments = arguments,
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True,
            .StandardOutputEncoding = Text.Encoding.UTF8,
            .StandardErrorEncoding = Text.Encoding.UTF8
        }

        Using proc As Process = Process.Start(info)
            Dim stdout As String = proc.StandardOutput.ReadToEnd()
            Dim stderr As String = proc.StandardError.ReadToEnd()

            proc.WaitForExit()

            result.Ran = proc.ExitCode = 0
            result.RawOutput = stdout

            If verbose Then
                Call stdout.println

                If stderr.Length > 0 Then
                    Call $"[R stderr] {stderr}".println
                End If
            End If

            If Not result.Ran Then
                result.Message = $"R exited with code {proc.ExitCode}: {stderr}"
            End If
        End Using

        If Not result.Ran AndAlso result.Message Is Nothing Then
            result.Message = "R baseline did not complete"
        End If

        Call LoadStatus(result)

        For Each layerName As String In {"mRNA", "protein", "methylation"}
            Dim scoreFile As String = Path.Combine(result.OutputDir, $"r_{layerName}_scores.csv")

            If File.Exists(scoreFile) Then
                result.LayerScores(layerName) = ReadReferenceMatrix(scoreFile)
            End If
        Next

        Dim meanFile As String = Path.Combine(result.OutputDir, "r_baseline_mean.csv")

        If File.Exists(meanFile) Then
            result.MeanScores = ReadReferenceMatrix(meanFile)
        End If

        Return result
    End Function

    ''' <summary>
    ''' 逐元素比对（方向为 -1 的组学层先取负，因为 R 端用的是原始数值）。
    ''' </summary>
    Public Function Compare(ours As Matrix,
                            reference As Dictionary(Of String, Double()),
                            Optional direction As Integer = 1,
                            Optional tolerance As Double = TOLERANCE) As CompareResult

        Dim result As New CompareResult()
        Dim index As Dictionary(Of String, Integer) = ours.rownames _
            .Select(Function(id, i) (id, i)) _
            .GroupBy(Function(t) t.id) _
            .ToDictionary(Function(g) g.Key, Function(g) g.First().i, StringComparer.Ordinal)
        Dim maxDiff As Double = 0
        Dim sumDiff As Double = 0
        Dim n As Integer = 0
        Dim bad As Integer = 0
        Dim worst As String = ""

        For Each pathway As String In reference.Keys
            Dim row As Integer

            If Not index.TryGetValue(pathway, row) Then
                result.MissingPathways += 1
                Continue For
            End If

            Dim expected As Double() = reference(pathway)
            Dim actual As Double() = ours.expression(row).experiments
            Dim pathMax As Double = 0

            If expected.Length <> actual.Length Then
                result.MissingPathways += 1
                Continue For
            End If

            For j As Integer = 0 To expected.Length - 1
                Dim d As Double = std.Abs(expected(j) - direction * actual(j))

                sumDiff += d
                n += 1

                If d > pathMax Then
                    pathMax = d
                End If
            Next

            If pathMax > maxDiff Then
                maxDiff = pathMax
                worst = pathway
            End If
            If pathMax > tolerance Then
                bad += 1
            End If
        Next

        result.Compared = n
        result.MaxDifference = maxDiff
        result.MeanDifference = If(n > 0, sumDiff / n, Double.NaN)
        result.WorstPathway = worst
        result.BadPathways = bad
        result.Details = $"compared {n} value(s), max|diff| = {maxDiff:E3} ({worst}), mean|diff| = {result.MeanDifference:E3}"

        Return result
    End Function

    ''' <summary>读取 R 的 <c>write.csv</c> 产出的「通路 × 样本」矩阵</summary>
    Public Function ReadReferenceMatrix(path As String) As Dictionary(Of String, Double())
        Dim result As New Dictionary(Of String, Double())(StringComparer.Ordinal)

        Using parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(path) With {
            .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited,
            .Delimiters = {","c},
            .HasFieldsEnclosedInQuotes = True
        }
            Call parser.ReadFields()

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()

                If fields Is Nothing OrElse fields.Length < 2 Then
                    Continue While
                End If

                Dim vals As Double() = New Double(fields.Length - 2) {}

                For i As Integer = 0 To vals.Length - 1
                    vals(i) = Double.Parse(fields(i + 1), Globalization.CultureInfo.InvariantCulture)
                Next

                result(fields(0)) = vals
            End While
        End Using

        Return result
    End Function

    ''' <summary>
    ''' 读取 multiGSEA 的 p 值长表（首列 pathway，其余列是各组学的 p 值与 combined_pval）。
    ''' </summary>
    Public Function ReadPValueTable(path As String) As Dictionary(Of String, Dictionary(Of String, Double))
        Dim result As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.Ordinal)

        Using parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(path) With {
            .TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited,
            .Delimiters = {","c},
            .HasFieldsEnclosedInQuotes = True
        }
            Dim header As String() = parser.ReadFields()
            Dim columns As String() = header.Skip(1).ToArray

            While Not parser.EndOfData
                Dim fields As String() = parser.ReadFields()

                If fields Is Nothing OrElse fields.Length < 2 Then
                    Continue While
                End If

                Dim row As New Dictionary(Of String, Double)(StringComparer.Ordinal)

                For i As Integer = 0 To std.Min(columns.Length, fields.Length - 1) - 1
                    Dim text As String = fields(i + 1)

                    If text.StringEmpty(, True) OrElse text.TextEquals("NA") Then
                        row(columns(i)) = Double.NaN
                    Else
                        row(columns(i)) = Double.Parse(text, Globalization.CultureInfo.InvariantCulture)
                    End If
                Next

                result(fields(0)) = row
            End While
        End Using

        Return result
    End Function

    Private Sub LoadStatus(result As BaselineResult)
        Dim statusFile As String = Path.Combine(result.OutputDir, "baseline_status.tsv")

        If Not File.Exists(statusFile) Then
            Return
        End If

        For Each line As String In File.ReadLines(statusFile)
            Dim fields As String() = line.Split(ControlChars.Tab)

            If fields.Length >= 2 Then
                result.Status(fields(0)) = $"{fields(1)}({If(fields.Length > 2, fields(2), "")})"
            End If
        Next
    End Sub

    Private Function LocateScript() As String
        Dim dir As DirectoryInfo = New DirectoryInfo(AppContext.BaseDirectory)
        Dim i As Integer = 0

        While dir IsNot Nothing AndAlso i < 10
            Dim candidate As String = Path.Combine(dir.FullName, "reference", REF_SCRIPT)

            If File.Exists(candidate) Then
                Return candidate
            End If

            candidate = Path.Combine(dir.FullName, REF_SCRIPT)

            If File.Exists(candidate) Then
                Return candidate
            End If

            dir = dir.Parent
            i += 1
        End While

        Return Nothing
    End Function

    <Extension>
    Private Sub println(msg As String)
        Call Console.WriteLine(msg)
    End Sub

End Module
