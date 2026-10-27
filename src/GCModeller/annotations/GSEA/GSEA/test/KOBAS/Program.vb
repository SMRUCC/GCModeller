Imports System.IO

Namespace Global.KOBAS.test

    ''' <summary>
    ''' KOBAS 算法移植的自检入口。
    ''' </summary>
    ''' <remarks>
    ''' 用法：
    ''' <code>
    ''' KOBAS.test.exe [--all] [--stats] [--gsea] [--similarity] [--snbea] [--rscript=PATH] [--work=DIR]
    ''' </code>
    ''' 不带任何开关时执行全部测试。R 交叉验证在找不到 Rscript 时记为 SKIP，不影响退出码。
    ''' </remarks>
    Module Program

        ''' <summary>
        ''' 默认的 Rscript 路径。
        ''' </summary>
        Public Const DefaultRscript As String = "C:\Program Files\R\R-4.5.0\bin\Rscript.exe"

        ''' <summary>
        ''' 自检入口。
        ''' </summary>
        ''' <param name="args">命令行参数。</param>
        ''' <returns>进程退出码：全部通过为 0。</returns>
        Function Main(args As String()) As Integer
            Dim runAll As Boolean = True
            Dim runFormats As Boolean = False
            Dim runStats As Boolean = False
            Dim runGsea As Boolean = False
            Dim runSimilarity As Boolean = False
            Dim runSnbea As Boolean = False
            Dim rscriptPath As String = DefaultRscript
            Dim workDir As String = Nothing

            For Each item As String In args
                Dim optionText As String = If(item, "").Trim()

                Select Case True
                    Case optionText = "--all"
                        runAll = True
                    Case optionText = "--formats"
                        runFormats = True
                        runAll = False
                    Case optionText = "--stats"
                        runStats = True
                        runAll = False
                    Case optionText = "--gsea"
                        runGsea = True
                        runAll = False
                    Case optionText = "--similarity"
                        runSimilarity = True
                        runAll = False
                    Case optionText = "--snbea"
                        runSnbea = True
                        runAll = False
                    Case optionText.StartsWith("--rscript=")
                        rscriptPath = optionText.Substring("--rscript=".Length)
                    Case optionText.StartsWith("--work=")
                        workDir = optionText.Substring("--work=".Length)
                    Case optionText = "--help", optionText = "-h"
                        Call Console.WriteLine("usage: KOBAS.test.exe [--all|--stats|--gsea|--similarity|--snbea] [--rscript=PATH] [--work=DIR]")
                        Return 0
                End Select
            Next

            If runAll Then
                runFormats = True
                runStats = True
                runGsea = True
                runSimilarity = True
                runSnbea = True
            End If

            If workDir Is Nothing Then
                workDir = Path.Combine(AppContext.BaseDirectory, "kobas_selftest")
            End If

            Call Directory.CreateDirectory(workDir)

            If runFormats Then
                Call FormatsTest.Run(workDir)
            End If
            If runStats Then
                Call StatisticsTest.Run(rscriptPath, workDir)
            End If
            If runGsea Then
                Call GseaTest.Run()
            End If
            If runSimilarity Then
                Call SimilarityTest.Run()
            End If
            If runSnbea Then
                Call SnbeaTest.Run()
            End If

            Call Console.WriteLine()
            Call Console.WriteLine(KobasSelfTest.SummaryText())

            If KobasSelfTest.Failures = 0 Then
                Call Console.WriteLine("全部通过!")
            Else
                Call Console.WriteLine("存在失败的断言!")
            End If

            Return KobasSelfTest.ExitCode()
        End Function
    End Module
End Namespace
