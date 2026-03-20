' ============================================================================
' HMMER3蛋白质序列分类注释使用示例
' 
' 演示如何使用HMMER3Parser和ProteinAnnotator进行蛋白质序列分类注释
' 
' Author: 基于用户现有HMM代码框架扩展
' Copyright (c) 2024 GPL3 Licensed
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain.Models
Imports HMMER3
Imports HMMER3.HMMER3
Imports Microsoft.VisualBasic.DataMining.HiddenMarkovChain

Namespace HMMER3.Examples

    ''' <summary>
    ''' 使用示例模块
    ''' </summary>
    Public Module UsageExamples

        ''' <summary>
        ''' 示例1：基本使用 - 加载模型并注释单个序列
        ''' </summary>
        Public Sub Example1_BasicUsage()
            ' 创建注释器
            Dim annotator As New ProteinAnnotator()

            ' 加载HMMER3模型文件
            annotator.LoadModel("K00001.hmm.txt")

            ' 创建蛋白质序列
            Dim protein As New ProteinSequence With {
                .ID = "test_protein_1",
                .Description = "Test protein sequence",
                .Sequence = "MAGVKQLADDRTLLMAGVSHDLRTPLTRIRLATEMMSEQDGYLAESINKDIEEQ"
            }

            ' 进行注释
            Dim result As AnnotationResult = annotator.Annotate(protein)

            ' 输出结果
            If result IsNot Nothing Then
                Console.WriteLine($"Model: {result.ModelName}")
                Console.WriteLine($"Bit Score: {result.BitScore:F2}")
                Console.WriteLine($"E-value: {result.EValue:G3}")
                Console.WriteLine($"Is Significant: {result.IsSignificant}")
                Console.WriteLine($"Confidence: {result.Confidence:F3}")
            End If
        End Sub

        ''' <summary>
        ''' 示例2：批量处理FASTA文件
        ''' </summary>
        Public Sub Example2_BatchProcessing()
            ' 创建注释器
            Dim annotator As New ProteinAnnotator()

            ' 设置参数
            annotator.EValueThreshold = 0.01
            annotator.BitScoreThreshold = 25.0
            annotator.DatabaseSize = 100000

            ' 加载多个模型
            annotator.LoadModelsFromDirectory("./hmm_models/", "*.hmm")
            Console.WriteLine($"Loaded {annotator.ModelCount} models")

            ' 解析FASTA文件
            Dim proteins As List(Of ProteinSequence) = FastaParser.Parse("proteins.fasta")
            Console.WriteLine($"Loaded {proteins.Count} protein sequences")

            ' 批量注释
            annotator.AnnotateAll(proteins)

            ' 输出结果到文件
            File.WriteAllText("annotation_results.tsv", AnnotationOutput.ToTsv(proteins))
            File.WriteAllText("annotation_results.csv", AnnotationOutput.ToCsv(proteins))
            File.WriteAllText("annotation_results.json", AnnotationOutput.ToJson(proteins))

            ' 输出统计信息
            Dim significantCount As Integer = proteins.Count(Function(p) p.Annotation IsNot Nothing AndAlso p.Annotation.IsSignificant)
            Console.WriteLine($"Significant annotations: {significantCount}/{proteins.Count}")
        End Sub

        ''' <summary>
        ''' 示例3：直接解析模型内容字符串
        ''' </summary>
        Public Sub Example3_ParseModelContent()
            ' 模型内容字符串（可以从数据库或其他来源获取）
            Dim modelContent As String = "HMMER3/f [3.2.1 | June 2018]" & vbCrLf &
                                         "NAME  K00001" & vbCrLf &
                                         "LENG  427" & vbCrLf &
                                         "ALPH  amino" & vbCrLf &
                                         "..."

            ' 创建注释器并加载模型
            Dim annotator As New ProteinAnnotator()
            annotator.LoadModelContent(modelContent)

            ' 进行注释
            Dim protein As New ProteinSequence With {
                .ID = "test",
                .Sequence = "MAGVKQLADDRTLLMAGVSHDLRTPLTRIRLATEMMSEQDGYLAESINKDIEEQ"
            }
            annotator.Annotate(protein)
        End Sub

        ''' <summary>
        ''' 示例4：使用现有HMM框架进行Viterbi解码
        ''' </summary>
        Public Sub Example4_UseExistingHMMFramework()
            ' 解析HMMER3模型
            Dim profileHMM As ProfileHMM = HMMER3Parser.Parse("K00001.hmm.txt")

            ' 创建标准HMM对象
            Dim hmm As HMM = profileHMM.CreateHMM()

            ' 创建观测序列（氨基酸序列转换为字符串数组）
            Dim sequence As String = "MAGVKQLADDRTLLMAGVSHDLRTPLTRIRLATEMMSEQDGYLAESINKDIEEQ"
            Dim chain As New Chain()
            chain.obSequence = sequence.Select(Function(c) c.ToString()).ToArray()

            ' 使用Viterbi算法解码
            Dim viterbiResult As viterbiSequence = hmm.viterbiAlgorithm(chain)

            ' 输出最可能的状态路径
            Console.WriteLine($"Viterbi state sequence: {String.Join(" -> ", viterbiResult.stateSequence)}")
            Console.WriteLine($"Termination probability: {viterbiResult.terminationProbability}")

            ' 使用Forward算法计算序列概率
            Dim forwardResult As Alpha = hmm.forwardAlgorithm(chain)
            Console.WriteLine($"Forward probability: {forwardResult.alphaF}")
        End Sub

        ''' <summary>
        ''' 示例5：自定义阈值和过滤
        ''' </summary>
        Public Sub Example5_CustomThresholds()
            Dim annotator As New ProteinAnnotator()
            annotator.LoadModel("K00001.hmm.txt")

            ' 设置严格的阈值
            annotator.EValueThreshold = 0.00001
            annotator.BitScoreThreshold = 50.0

            Dim proteins As List(Of ProteinSequence) = FastaParser.Parse("proteins.fasta")
            annotator.AnnotateAll(proteins)

            ' 过滤显著结果
            Dim significantProteins As List(Of ProteinSequence) = proteins _
                .Where(Function(p) p.Annotation IsNot Nothing AndAlso p.Annotation.IsSignificant) _
                .ToList()

            ' 按比特得分排序
            significantProteins.Sort(Function(a, b) b.Annotation.BitScore.CompareTo(a.Annotation.BitScore))

            ' 输出Top 10
            Console.WriteLine("Top 10 significant annotations:")
            For i As Integer = 0 To Math.Min(9, significantProteins.Count - 1)
                Dim p As ProteinSequence = significantProteins(i)
                Console.WriteLine($"{i + 1}. {p.ID}: Score={p.Annotation.BitScore:F2}, E={p.Annotation.EValue:G3}")
            Next
        End Sub

        ''' <summary>
        ''' 示例6：完整工作流程
        ''' </summary>
        Public Sub Example6_CompleteWorkflow()
            Console.WriteLine("=== HMMER3 Protein Annotation Workflow ===")
            Console.WriteLine()

            ' Step 1: 初始化注释器
            Console.WriteLine("Step 1: Initializing annotator...")
            Dim annotator As New ProteinAnnotator With {
                .EValueThreshold = 0.01,
                .BitScoreThreshold = 25.0,
                .DatabaseSize = 1000000
            }

            ' Step 2: 加载模型
            Console.WriteLine("Step 2: Loading HMM models...")
            Dim modelPath As String = "K00001.hmm.txt"
            If File.Exists(modelPath) Then
                annotator.LoadModel(modelPath)
                Console.WriteLine($"  Loaded model: {annotator.ModelNames.FirstOrDefault()}")
            Else
                Console.WriteLine($"  Model file not found: {modelPath}")
                Return
            End If

            ' Step 3: 加载蛋白质序列
            Console.WriteLine("Step 3: Loading protein sequences...")
            Dim fastaPath As String = "proteins.fasta"
            Dim proteins As List(Of ProteinSequence)

            If File.Exists(fastaPath) Then
                proteins = FastaParser.Parse(fastaPath)
                Console.WriteLine($"  Loaded {proteins.Count} sequences")
            Else
                ' 使用示例序列
                proteins = New List(Of ProteinSequence) From {
                    New ProteinSequence With {
                        .ID = "example_protein_1",
                        .Description = "Example protein for testing",
                        .Sequence = "MAGVKQLADDRTLLMAGVSHDLRTPLTRIRLATEMMSEQDGYLAESINKDIEEQ"
                    },
                    New ProteinSequence With {
                        .ID = "example_protein_2",
                        .Description = "Another example protein",
                        .Sequence = "MKTVRQERLKSIVRILERSKEPVSGAQLAEELSVSRQVIVQDIAYLRSLGYNIVATPRGYVLAGG"
                    }
                }
                Console.WriteLine($"  Using {proteins.Count} example sequences")
            End If

            ' Step 4: 执行注释
            Console.WriteLine("Step 4: Annotating sequences...")
            annotator.AnnotateAll(proteins)

            ' Step 5: 输出结果
            Console.WriteLine("Step 5: Output results...")
            Console.WriteLine()

            ' 输出到控制台
            Console.WriteLine("Annotation Results:")
            Console.WriteLine("-" & StrDup(80, "-"c))
            For Each protein As ProteinSequence In proteins
                Console.WriteLine($"ID: {protein.ID}")
                Console.WriteLine($"Length: {protein.Length}")
                If protein.Annotation IsNot Nothing Then
                    Console.WriteLine($"Best Match: {protein.Annotation.ModelName}")
                    Console.WriteLine($"Bit Score: {protein.Annotation.BitScore:F2}")
                    Console.WriteLine($"E-value: {protein.Annotation.EValue:G3}")
                    Console.WriteLine($"Significant: {protein.Annotation.IsSignificant}")
                    Console.WriteLine($"Confidence: {protein.Annotation.Confidence:P1}")
                Else
                    Console.WriteLine("No significant match found")
                End If
                Console.WriteLine("-" & StrDup(80, "-"c))
            Next

            ' Step 6: 保存结果
            Console.WriteLine("Step 6: Saving results to files...")
            File.WriteAllText("results.tsv", AnnotationOutput.ToTsv(proteins))
            File.WriteAllText("results.csv", AnnotationOutput.ToCsv(proteins))
            File.WriteAllText("results.json", AnnotationOutput.ToJson(proteins))
            Console.WriteLine("  Results saved to results.tsv, results.csv, results.json")

            ' Step 7: 统计摘要
            Console.WriteLine()
            Console.WriteLine("Summary:")
            Console.WriteLine($"  Total sequences: {proteins.Count}")
            Console.WriteLine($"  Annotated: {proteins.Count(Function(p) p.Annotation IsNot Nothing)}")
            Console.WriteLine($"  Significant: {proteins.Count(Function(p) p.Annotation IsNot Nothing AndAlso p.Annotation.IsSignificant)}")

            Console.WriteLine()
            Console.WriteLine("=== Workflow Complete ===")
        End Sub

    End Module

End Namespace
