Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS
Imports std = System.Math

Namespace Global.KOBAS.test

    ''' <summary>
    ''' 统计检验与多重校正的自检（含 R 交叉验证）。
    ''' </summary>
    Public Module StatisticsTest

        ''' <summary>
        ''' 一条用于交叉验证的列联表。
        ''' </summary>
        Private Class TestCase
            Public Property ID As String
            Public Property M1 As Integer
            Public Property N1 As Integer
            Public Property M2 As Integer
            Public Property N2 As Integer
            Public Property A As Integer
            Public Property B As Integer
            Public Property C As Integer
            Public Property D As Integer
        End Class

        ''' <summary>
        ''' 执行统计检验的全部断言。
        ''' </summary>
        ''' <param name="rscriptPath">Rscript 可执行文件路径。</param>
        ''' <param name="workDir">交叉验证的工作目录。</param>
        Public Sub Run(rscriptPath As String, workDir As String)
            Call Console.WriteLine("[statistics] 超几何 / Fisher / 二项 / 卡方 / 多重校正")

            Dim cases As TestCase() = CreateCases()
            Dim fishers As Double() = New Double(cases.Length - 1) {}

            For i As Integer = 0 To cases.Length - 1
                Dim item As TestCase = cases(i)

                fishers(i) = ExactTests.FisherGreater(item.A, item.B, item.C, item.D)

                Call KobasSelfTest.Check(
                    $"Fisher greater p in (0,1] ({item.ID})",
                    fishers(i) > 0 AndAlso fishers(i) <= 1,
                    $"p={fishers(i).ToString("G6", CultureInfo.InvariantCulture)}")

                Call KobasSelfTest.CheckClose(
                    $"超几何上尾与 Fisher greater 一致 ({item.ID})",
                    fishers(i),
                    ExactTests.HypergeometricUpperTail(item.M1, item.M2, item.N2, item.N1),
                    1.0E-12)
            Next

            ' 二项检验的闭式解
            Call KobasSelfTest.CheckClose(
                "二项上尾闭式解 P(X>=8 | n=10, p=0.5)",
                56.0 / 1024.0,
                ExactTests.BinomialUpperTail(8, 10, 0.5),
                1.0E-15)
            Call KobasSelfTest.CheckClose(
                "二项上尾 k=0 时为 1",
                1.0,
                ExactTests.BinomialUpperTail(0, 10, 0.3),
                1.0E-15)
            Call KobasSelfTest.CheckClose(
                "二项上尾 p=0 且 k>0 时为 0",
                0.0,
                ExactTests.BinomialUpperTail(3, 10, 0),
                1.0E-15)

            ' 卡方期望频数过小时退回 Fisher
            Dim smallCase = cases.Where(Function(c) ExactTests.MinimumExpectedFrequency(c.A, c.B, c.C, c.D) < 10).First()
            Call KobasSelfTest.CheckClose(
                "期望频数<10 时卡方退回 Fisher",
                ExactTests.FisherGreater(smallCase.A, smallCase.B, smallCase.C, smallCase.D),
                ExactTests.ImprovedChiSquare(smallCase.A, smallCase.B, smallCase.C, smallCase.D),
                1.0E-15)

            ' 多重校正的基本性质
            Dim pvalues As Double() = {0.01, 0.02, 0.03, 0.04}
            Dim bh As Double() = FdrAdjust.Adjust(pvalues, FdrMethod.BH)

            Call KobasSelfTest.CheckClose("BH 校正 [0.01..0.04] 全部为 0.04", 0.04, bh(0), 1.0E-12)
            Call KobasSelfTest.CheckClose("BH 校正单调（第 4 项）", 0.04, bh(3), 1.0E-12)

            Dim by As Double() = FdrAdjust.Adjust(pvalues, FdrMethod.BY)

            Call KobasSelfTest.Check("BY 不小于 BH（逐项）", by.Zip(bh, Function(x, y) x >= y - 1.0E-12).All(Function(flag) flag))

            Dim unchanged As Double() = FdrAdjust.Adjust(pvalues, FdrMethod.None)

            Call KobasSelfTest.Check("FDR=None 原样返回", unchanged.SequenceEqual(pvalues))

            Dim q As Double() = FdrAdjust.Adjust({0.001, 0.01, 0.2, 0.6, 0.9}, FdrMethod.QValue)

            Call KobasSelfTest.CheckClose("Storey q 值（第 1 项）", 0.004, q(0), 1.0E-12)
            Call KobasSelfTest.CheckClose("Storey q 值（第 2 项）", 0.02, q(1), 1.0E-12)
            Call KobasSelfTest.CheckClose("Storey q 值（第 3 项）", 4 * 0.2 / 3, q(2), 1.0E-12)

            ' 前景必须为背景子集
            Dim thrown As Boolean = False

            Try
                Call TwoSampleTest.Contingency(10, 20, 5, 100)
            Catch ex As InvalidOperationException
                thrown = True
            End Try

            Call KobasSelfTest.Check("前景非背景子集时抛出异常", thrown)

            ' R 交叉验证
            Call CrossValidateWithR(rscriptPath, workDir, cases, fishers)
        End Sub

        ''' <summary>
        ''' 调用 R 对拍 Fisher / 卡方 / 二项 / p.adjust。
        ''' </summary>
        Private Sub CrossValidateWithR(rscriptPath As String, workDir As String, cases As TestCase(), fishers As Double())
            Dim script As String = Path.Combine(AppContext.BaseDirectory, "reference", "kobas_reference.R")

            If Not File.Exists(rscriptPath) Then
                Call KobasSelfTest.Skip("R 交叉验证", $"Rscript not found: {rscriptPath}")
                Return
            End If
            If Not File.Exists(script) Then
                Call KobasSelfTest.Skip("R 交叉验证", $"reference script not found: {script}")
                Return
            End If

            Dim inputFile As String = Path.Combine(workDir, "stats_input.tsv")
            Dim text As New StringBuilder

            Call text.AppendLine(String.Join(ControlChars.Tab, {"id", "a", "b", "c", "d", "m1", "n1", "m2", "n2"}))

            For Each item As TestCase In cases
                Call text.AppendLine($"{item.ID}{ControlChars.Tab}{item.A}{ControlChars.Tab}{item.B}{ControlChars.Tab}{item.C}{ControlChars.Tab}{item.D}" &
                                     $"{ControlChars.Tab}{item.M1}{ControlChars.Tab}{item.N1}{ControlChars.Tab}{item.M2}{ControlChars.Tab}{item.N2}")
            Next

            Call File.WriteAllText(inputFile, text.ToString())

            Dim output As String

            Try
                output = RunR(rscriptPath, script, inputFile, workDir)
            Catch ex As Exception
                Call KobasSelfTest.Skip("R 交叉验证", ex.Message)
                Return
            End Try

            Dim statsFile As String = Path.Combine(workDir, "stats_r.tsv")

            If Not File.Exists(statsFile) Then
                Call KobasSelfTest.Skip("R 交叉验证", "R did not produce the expected output." & vbCrLf & output)
                Return
            End If

            Dim table As Dictionary(Of String, Double()) = ReadTable(statsFile)

            For Each item As TestCase In cases
                Dim row As Double() = Nothing

                If Not table.TryGetValue(item.ID, row) Then
                    Continue For
                End If

                Call KobasSelfTest.CheckClose($"Fisher 与 R 一致 ({item.ID})", row(0), ExactTests.FisherGreater(item.A, item.B, item.C, item.D), 1.0E-10)
                Call KobasSelfTest.CheckClose($"二项与 R 一致 ({item.ID})", row(2), ExactTests.BinomialUpperTail(item.M1, item.N1, item.M2 / CDbl(item.N2)), 1.0E-10)

                If ExactTests.MinimumExpectedFrequency(item.A, item.B, item.C, item.D) >= 10 Then
                    Call KobasSelfTest.CheckClose($"卡方(Yates) 与 R 一致 ({item.ID})", row(1), ExactTests.ChiSquareYates(item.A, item.B, item.C, item.D), 1.0E-10)
                End If
            Next

            Dim adjustFile As String = Path.Combine(workDir, "adjust_r.tsv")

            If File.Exists(adjustFile) Then
                Dim lines As String() = File.ReadAllLines(adjustFile)
                Dim bh As Double() = FdrAdjust.Adjust(fishers, FdrMethod.BH)
                Dim by As Double() = FdrAdjust.Adjust(fishers, FdrMethod.BY)

                For i As Integer = 1 To std.Min(lines.Length - 1, fishers.Length)
                    Dim words As String() = lines(i).Split(ControlChars.Tab)

                    Call KobasSelfTest.CheckClose(
                        $"BH 与 R p.adjust 一致 #{i}",
                        Double.Parse(words(0), CultureInfo.InvariantCulture), bh(i - 1), 1.0E-12)
                    Call KobasSelfTest.CheckClose(
                        $"BY 与 R p.adjust 一致 #{i}",
                        Double.Parse(words(1), CultureInfo.InvariantCulture), by(i - 1), 1.0E-12)
                Next
            End If
        End Sub

        ''' <summary>
        ''' 启动 Rscript 执行参考脚本。
        ''' </summary>
        Private Function RunR(rscriptPath As String, script As String, inputFile As String, workDir As String) As String
            Dim info As New ProcessStartInfo(rscriptPath) With {
                .UseShellExecute = False,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True,
                .CreateNoWindow = True
            }

            Call info.ArgumentList.Add(script)
            Call info.ArgumentList.Add(inputFile)
            Call info.ArgumentList.Add(workDir)

            Using process As Process = Process.Start(info)
                Dim stdout As String = process.StandardOutput.ReadToEnd()
                Dim stderr As String = process.StandardError.ReadToEnd()

                Call process.WaitForExit()

                If process.ExitCode <> 0 Then
                    Throw New InvalidOperationException($"Rscript exits with code {process.ExitCode}:{vbCrLf}{stderr}")
                End If

                Return stdout & stderr
            End Using
        End Function

        ''' <summary>
        ''' 读取两列式（id + 若干数值列）的结果表。
        ''' </summary>
        Private Function ReadTable(tableFile As String) As Dictionary(Of String, Double())
            Dim lines As String() = File.ReadAllLines(tableFile)
            Dim header As String() = lines(0).Split(ControlChars.Tab)
            Dim result As New Dictionary(Of String, Double())(StringComparer.Ordinal)

            For i As Integer = 1 To lines.Length - 1
                If lines(i).Trim().Length = 0 Then
                    Continue For
                End If

                Dim words As String() = lines(i).Split(ControlChars.Tab)
                Dim values As Double() = New Double(header.Length - 2) {}

                For j As Integer = 1 To header.Length - 1
                    values(j - 1) = Double.Parse(words(j), CultureInfo.InvariantCulture)
                Next

                result(words(0)) = values
            Next

            Return result
        End Function

        ''' <summary>
        ''' 构造交叉验证所使用的列联表集合。
        ''' </summary>
        Private Function CreateCases() As TestCase()
            Return New TestCase() {
                MakeCase("c1", 5, 20, 10, 100),
                MakeCase("c2", 12, 50, 40, 500),
                MakeCase("c3", 200, 1000, 400, 5000),
                MakeCase("c4", 2, 10, 30, 200),
                MakeCase("c5", 300, 1500, 500, 8000),
                MakeCase("c6", 8, 25, 16, 150)
            }
        End Function

        ''' <summary>
        ''' 由前景/背景计数构造标准 2x2 列联表。
        ''' </summary>
        Private Function MakeCase(id As String, m1 As Integer, n1 As Integer, m2 As Integer, n2 As Integer) As TestCase
            Dim table As Integer() = TwoSampleTest.Contingency(m1, n1, m2, n2)

            Return New TestCase With {
                .ID = id,
                .M1 = m1,
                .N1 = n1,
                .M2 = m2,
                .N2 = n2,
                .A = table(0),
                .B = table(1),
                .C = table(2),
                .D = table(3)
            }
        End Function
    End Module
End Namespace
