Imports System.IO
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.KOBAS

Namespace Global.KOBAS.test

    ''' <summary>
    ''' 输入格式解析（GMT / 命中矩阵 / KOBAS 文本矩阵 / annotate）的自检。
    ''' </summary>
    Public Module FormatsTest

        ''' <summary>
        ''' 项目中随附的真实 KEGG 基因集文件（存在时用于验证解析规模）。
        ''' </summary>
        Public Const DefaultGmt As String = "G:\GCModeller\src\GCModeller\annotations\GSEA\data\TCGA_COAD\kegg_v3.gmt"

        ''' <summary>
        ''' 执行格式解析的全部断言。
        ''' </summary>
        ''' <param name="workDir">用于写出临时文件的目录。</param>
        Public Sub Run(workDir As String)
            Call Console.WriteLine("[formats] GMT / 命中矩阵 / 表达矩阵 / annotate")

            Call GmtCases(workDir)
            Call HitMatrixCases()
            Call ExpressionCases(workDir)
            Call AnnotateCases(workDir)
        End Sub

        ''' <summary>
        ''' GMT 解析的断言。
        ''' </summary>
        Private Sub GmtCases(workDir As String)
            Dim background As Background = GmtFile.Parse(
                New String() {
                    "# a comment line",
                    "setA" & ControlChars.Tab & "pathway A" & ControlChars.Tab & "g1" & ControlChars.Tab & "g2" & ControlChars.Tab & "g2",
                    "setB" & ControlChars.Tab & "pathway B" & ControlChars.Tab & "g2" & ControlChars.Tab & "g3" & ControlChars.Tab & "g4"
                })

            Call KobasSelfTest.Check("GMT 解析保留全部基因集", background.clusters.Length = 2, $"{background.clusters.Length} 个")
            Call KobasSelfTest.Check("GMT 解析剔除重复成员", background.clusters.First(Function(c) c.ID = "setA").size = 2)
            Call KobasSelfTest.Check("GMT 背景规模为成员并集", background.size = 4, $"size={background.size}")

            Dim duplicatedThrown As Boolean = False

            Try
                Call GmtFile.Parse(New String() {
                    "setA" & ControlChars.Tab & "a" & ControlChars.Tab & "g1",
                    "setA" & ControlChars.Tab & "a" & ControlChars.Tab & "g2"
                })
            Catch ex As FormatException
                duplicatedThrown = True
            End Try

            Call KobasSelfTest.Check("GMT 重复编号默认抛出异常", duplicatedThrown)

            Dim kept As Background = GmtFile.Parse(New String() {
                "setA" & ControlChars.Tab & "a" & ControlChars.Tab & "g1",
                "setA" & ControlChars.Tab & "a" & ControlChars.Tab & "g2"
            }, DuplicateGeneSetPolicy.KeepFirst)

            Call KobasSelfTest.Check("GMT 重复编号可配置为保留首个", kept.clusters.Length = 1)

            Dim badThrown As Boolean = False

            Try
                Call GmtFile.Parse(New String() {"setA" & ControlChars.Tab & "only two columns"})
            Catch ex As FormatException
                badThrown = True
            End Try

            Call KobasSelfTest.Check("GMT 列数不足时抛出异常", badThrown)

            ' 真实 KEGG GMT（文件存在时才校验）
            If File.Exists(DefaultGmt) Then
                Dim kegg As Background = GmtFile.Load(DefaultGmt)

                Call KobasSelfTest.Check("真实 KEGG GMT 解析出 337 条通路", kegg.clusters.Length = 337, $"{kegg.clusters.Length} 条")
                Call KobasSelfTest.Check("真实 KEGG GMT 全部通路都有成员", kegg.clusters.All(Function(c) c.size > 0))
                Call KobasSelfTest.Check("真实 KEGG GMT 背景规模合理", kegg.size > 5000, $"size={kegg.size}")
            Else
                Call KobasSelfTest.Skip("真实 KEGG GMT 解析", $"file not found: {DefaultGmt}")
            End If
        End Sub

        ''' <summary>
        ''' 命中矩阵构建的断言。
        ''' </summary>
        Private Sub HitMatrixCases()
            Dim background As Background = KobasSelfTest.MakeBackground(
                New Dictionary(Of String, String()) From {
                    {"BIG", {"g1", "g2", "g3", "g4"}},
                    {"SMALL", {"g1", "g2"}}
                })
            Dim genes As String() = {"g1", "g2", "g3", "g4", "g5"}
            Dim matrix As HitMatrix = genes.BuildHitMatrix(background, 3, 10)

            Call KobasSelfTest.Check("命中矩阵按尺寸过滤", matrix.RowCount = 1 AndAlso matrix.GeneSets(0).ID = "BIG", $"{matrix.RowCount} 行")
            Call KobasSelfTest.Check("命中矩阵命中数量正确", matrix.GeneSets(0).HitCount = 4)
            Call KobasSelfTest.Check("命中矩阵列数等于基因数", matrix.ColumnCount = 5)
            Call KobasSelfTest.Check("命中矩阵命中基因列表", matrix.HitGenes(0).SequenceEqual({"g1", "g2", "g3", "g4"}))

            Dim thrown As Boolean = False

            Try
                Call (New String() {"x1", "x2"}).BuildHitMatrix(background, 1, 10)
            Catch ex As InvalidOperationException
                thrown = True
            End Try

            Call KobasSelfTest.Check("没有任何基因命中时抛出异常", thrown)
        End Sub

        ''' <summary>
        ''' KOBAS 文本表达矩阵读取的断言。
        ''' </summary>
        Private Sub ExpressionCases(workDir As String)
            Dim txtFile As String = Path.Combine(workDir, "expr_input.txt")
            Dim content As String() = {
                String.Join(ControlChars.Tab, {"Name", "Description", "s1", "s2"}),
                String.Join(ControlChars.Tab, {"g1", "gene 1", "1.5", "2.5"}),
                String.Join(ControlChars.Tab, {"g2", "gene 2", "3", "4"})
            }

            Call File.WriteAllLines(txtFile, content)

            Dim expr = ExpressionTable.LoadKobasTxt(txtFile)

            Call KobasSelfTest.Check("文本矩阵行数", expr.size = 2, $"{expr.size} 行")
            Call KobasSelfTest.Check("文本矩阵样本名", expr.sampleID.SequenceEqual({"s1", "s2"}))
            Call KobasSelfTest.CheckClose("文本矩阵数值解析", 2.5, expr.expression(0).experiments(1), 1.0E-12)

            Call KobasSelfTest.Check("文本矩阵通过表型校验", ExpressionTable.Load(txtFile, KobasSelfTest.MakePhenotype({0, 1})).size = 2)

            Dim mismatchThrown As Boolean = False

            Try
                Call ExpressionTable.Load(txtFile, KobasSelfTest.MakePhenotype({0, 1, 0}))
            Catch ex As InvalidOperationException
                mismatchThrown = True
            End Try

            Call KobasSelfTest.Check("样本数与表型不一致时抛出异常", mismatchThrown)
        End Sub

        ''' <summary>
        ''' KOBAS annotate 输出解析的断言。
        ''' </summary>
        Private Sub AnnotateCases(workDir As String)
            Dim annotatePath As String = Path.Combine(workDir, "annotate_output.txt")
            Dim content As String() = {
                "##hsa" & ControlChars.Tab & "Homo sapiens",
                "##Method: ID mapping (id:ncbigene)",
                "##Summary:" & ControlChars.Tab & "2 succeed, 0 fail",
                "#Query" & ControlChars.Tab & "Gene ID|Gene name|Hyperlink",
                "TP53" & ControlChars.Tab & "7157|P53|http://example.org/7157",
                "EGFR" & ControlChars.Tab & "1956|EGFR|http://example.org/1956!2|ALT|http://example.org/2",
                "NONE" & ControlChars.Tab & "None",
                AnnotateFile.Separator
            }

            Call File.WriteAllLines(annotatePath, content)

            Dim document As AnnotateDocument = AnnotateFile.Load(annotatePath)

            Call KobasSelfTest.Check("annotate 物种解析", document.Header.Species = "hsa", document.Header.Species)
            Call KobasSelfTest.Check("annotate ID mapping 解析", document.Header.IdMapping = "id:ncbigene", document.Header.IdMapping)
            Call KobasSelfTest.Check("annotate 注释基因数解析", document.Header.AnnotatedGenes = 2)
            Call KobasSelfTest.Check("annotate 记录数量", document.Entries.Length = 3, $"{document.Entries.Length} 条")
            Call KobasSelfTest.Check("annotate 多链接解析", document.Entries(1).Links.SequenceEqual({"1956", "2"}))
            Call KobasSelfTest.Check("annotate None 解析为空", Not document.Entries(2).HasLinks)

            Dim table As Dictionary(Of String, String()) = document.Entries.ToLinksTable()

            Call KobasSelfTest.Check("annotate 转链接表", table.Count = 2 AndAlso table("TP53")(0) = "7157")
        End Sub
    End Module
End Namespace
