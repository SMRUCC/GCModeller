Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.Extensions
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Alignment
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Evaluation
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Model
Imports SMRUCC.genomics.Analysis.ProteinTools.Interactions.Bayesian.Statistics
Imports SMRUCC.genomics.Analysis.SequenceAlignment.Clustal
Imports SMRUCC.genomics.Interops
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Programs
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 从实验互作数据库(DIP)出发，为单个目标基因构建配对比对并预测其互作伙伴。
''' </summary>
''' <remarks>
''' <para>
''' 这是论文流水线的工程化版本，与旧实现的区别在于步骤 6 之后：
''' </para>
''' <list type="bullet">
''' <item><description>
''' 旧实现把两个家族的比对序列按字符串直接拼接之后交给 R 的 <c>bnlearn</c>，
''' 既没有「按基因组配对」的概念，也没有互作标签节点；
''' </description></item>
''' <item><description>
''' 新实现按照论文的要求把同一互作对(同一个 DIP 复合物)的两条比对序列放在同一行构造
''' <b>配对联合比对</b>，然后计算 Dirichlet-多项似然比、依赖树与静态打分 <c>X</c>，
''' 最终以 <c>X ≥ 阈值</c> 判定互作，并输出逐残基对的贡献以支持可解释性分析。
''' </description></item>
''' </list>
''' <para>
''' 需要说明的是：DIP 记录本身并不包含物种信息，因此在「单目标基因」这一使用场景下
''' 每个候选互作对自成一个分组，Gibbs 分配采样没有可置换的对象，
''' 此时直接使用静态打分即可；如果要运行完整的分配采样，应当改用 SwissTCS 的
''' 「同一基因组内多个激酶与多个响应调节蛋白」数据。
''' </para>
''' </remarks>
Public Class DataPreparations

    Dim LocalBLAST As BLASTPlus
    Dim DipInteractions As DipRecord()
    Dim DipFsaSequence As FASTA.FastaFile
    Dim WorkDir As String
    Dim GenomicsProteins As FASTA.FastaFile

    ''' <summary>
    ''' 模型配置
    ''' </summary>
    Public Property Options As PpiModelOptions = Nothing

    ''' <summary>
    ''' 是否输出残基对级别的可解释性明细
    ''' </summary>
    Public Property ExplainResidues As Boolean = True

    ''' <summary>
    ''' 调试模式下最多保留的同源序列条数，-1 表示不限制
    ''' </summary>
    Public Property MaxHomologs As Integer = -1

    ''' <summary>
    ''' 初始化会话
    ''' </summary>
    ''' <param name="GenomicProteins">目标基因组的蛋白质序列，每一条序列对象仅包含有基因号</param>
    ''' <param name="DipDir">包含 <c>dip.csv</c> 与 <c>dip.fsa</c> 的目录</param>
    ''' <param name="Blastbin">BLAST+ 可执行文件的目录</param>
    ''' <param name="ClustalBin">Clustal Omega 可执行文件的目录(当前由内置的 Clustal Omega 复现提供，此参数保留)</param>
    ''' <param name="Temp">工作目录，为空的时候使用系统临时目录</param>
    Public Sub New(GenomicProteins As FASTA.FastaFile, DipDir As String, Blastbin As String, ClustalBin As String, Optional Temp As String = "")
        Call Console.WriteLine("===================================INITIALIZE_SESSION==========================================")

        LocalBLAST = New BLASTPlus(Blastbin)
        DipInteractions = String.Format("{0}/dip.csv", DipDir).LoadCsv(Of DipRecord)(False).ToArray
        DipFsaSequence = FASTA.FastaFile.Read(DipDir & "/dip.fsa")
        Me.GenomicsProteins = GenomicProteins
        WorkDir = If(String.IsNullOrEmpty(Temp), System.IO.Path.GetTempPath(), Temp)

        Call LocalBLAST.FormatDb(DipFsaSequence.FilePath, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)

        Call Console.WriteLine("===============================Initialization job done!======================================" & vbCrLf & vbCrLf)
    End Sub

    ''' <summary>
    ''' 旧版本的构造函数重载。
    ''' </summary>
    ''' <remarks>
    ''' <c>rBin</c> 参数原本用于指定 R 解释器的路径。重构之后整个流水线已经完全用纯 .NET 实现，
    ''' 不再需要 R，因此该参数会被忽略。保留这个重载只是为了让历史调用代码仍然可以编译。
    ''' </remarks>
    <Obsolete("The R interpreter is no longer required; use the overload without the rBin parameter.")>
    Public Sub New(GenomicProteins As FASTA.FastaFile, DipDir As String, Blastbin As String, rBin As String, ClustalBin As String, Optional Temp As String = "")
        Me.New(GenomicProteins, DipDir, Blastbin, ClustalBin, Temp)
    End Sub

    ''' <summary>
    ''' 为一个目标基因推断其互作伙伴
    ''' </summary>
    ''' <param name="GeneId">目标基因的编号</param>
    ''' <returns>可以直接保存为 CSV 的预测结果表</returns>
    Public Function InferInteraction(GeneId As String) As IO.File
        Dim workDir As String = System.IO.Path.Combine(Me.WorkDir, GeneId)

        If Not System.IO.Directory.Exists(workDir) Then
            Call System.IO.Directory.CreateDirectory(workDir)
        End If

        Dim tempFile As String = System.IO.Path.Combine(workDir, GeneId)

        ' 1. 保存目标基因的蛋白质序列
        Call GenomicsProteins _
            .Select(Function(FsaObject As FASTA.FastaSeq) String.Equals(FsaObject.Headers.First, GeneId)) _
            .Save(tempFile)

        ' 2. blastp 搜索 DIP 数据库之中的同源蛋白，作为家族 A 的原始数据
        Call Console.WriteLine("Searching the homologous protein in the dip database for object ""{0}""...", GeneId)

        Dim blastOut As String = String.Format("{0}/blastp_{1}_vs._dip.txt", workDir, GeneId)
        Call LocalBLAST.Blastp(tempFile, DipFsaSequence.FilePath, blastOut, "1e-3").Start(waitForExit:=True)

        Dim hits As BestHit() = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus _
            .TryParse(Me.LocalBLAST.LastBLASTOutputFilePath) _
            .ExportAllBestHist
        Call hits.SaveTo(LocalBLAST.LastBLASTOutputFilePath & ".csv", False)

        Dim matched As String() = (From item In hits
                                   Where Not String.Equals(item.HitName, IBlastOutput.HITS_NOT_FOUND)
                                   Select item.HitName.Split.First Distinct).ToArray

        If MaxHomologs > 0 Then
            matched = matched.Take(MaxHomologs).ToArray()
        End If

        If matched.Length = 0 Then
            Call Console.WriteLine("No homologous protein was found in the DIP database, nothing to predict.")

            Return EmptyResult(GeneId)
        End If

        Dim familyASequences As FASTA.FastaFile = DipFsaSequence.Select(
            Function(FsaObject As FASTA.FastaSeq) Array.IndexOf(matched, FsaObject.Headers.First.Split.First) > -1)

        Call familyASequences.AddRange(FASTA.FastaFile.Read(tempFile))

        ' 3. 在 DIP 记录之中查找这些同源蛋白的互作伙伴
        Call Console.WriteLine("Looking for the interaction pairs in the dip database...")

        Dim interactionPairs As New List(Of (a As String, b As String))
        Dim partnerIds As New List(Of String)

        For Each id As String In matched
            For Each item As DipRecord In DipInteractions
                If String.Equals(item.IdInteractorA, id) Then
                    Call partnerIds.Add(item.IdInteractorB)
                    Call interactionPairs.Add((id, item.IdInteractorB))
                ElseIf String.Equals(item.IdInteractorB, id) Then
                    Call partnerIds.Add(item.IdInteractorA)
                    Call interactionPairs.Add((item.IdInteractorA, id))
                End If
            Next
        Next

        Dim partners As String() = partnerIds.Distinct().OrderBy(Function(s) s).ToArray()

        Call Console.WriteLine("There are {0} partner records was found in the database!", partners.Length)

        If partners.Length = 0 Then
            Call Console.WriteLine("No interaction partner was recorded for the homologous proteins.")

            Return EmptyResult(GeneId)
        End If

        Dim familyBSequences As FASTA.FastaFile = DipFsaSequence.Select(
            Function(FsaObject As FASTA.FastaSeq) Array.IndexOf(partners, FsaObject.Headers.First.Split.First) > -1)

        ' 4. 再搜索一次，把目标基因组之中与这些伙伴同源的蛋白也纳入家族 B
        Dim partnerMsa As String = System.IO.Path.Combine(workDir, GeneId & "_alignment_partners.fsa")
        Call familyBSequences.Save(partnerMsa)
        Call LocalBLAST.FormatDb(partnerMsa, LocalBLAST.MolTypeProtein).Start(waitForExit:=True)

        Dim targetHitsOut As String = String.Format("{0}/blastp_genomics_proteins_vs.dip_partner.txt", workDir)
        Call LocalBLAST.Blastp(GenomicsProteins.FilePath, familyBSequences.FilePath, targetHitsOut, "1e-3").Start(waitForExit:=True)

        hits = NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus _
            .TryParse(Me.LocalBLAST.LastBLASTOutputFilePath) _
            .ExportAllBestHist

        Dim targetHomologs As String() = (From item In hits
                                          Where Not String.Equals(IBlastOutput.HITS_NOT_FOUND, item.HitName)
                                          Select item.QueryName Distinct).ToArray

        If MaxHomologs > 0 Then
            targetHomologs = targetHomologs.Take(MaxHomologs).ToArray()
        End If

        Dim extra As FASTA.FastaFile = GenomicsProteins.Select(
            Function(FsaObject As FASTA.FastaSeq) Array.IndexOf(targetHomologs, FsaObject.Headers.First.Split.First) > -1)

        Call extra.Save(System.IO.Path.Combine(workDir, "target_genomics_proteins_homologous_partners.fsa"))
        Call familyBSequences.AddRange(extra)

        ' 5. 两个家族各自做多重序列比对
        Call Console.WriteLine("Start multiple sequence alignment...")

        Dim alignmentA As ClustalOmegaResult = ClustalOmega.MultipleAlignmentOmega(familyASequences)
        Dim alignmentB As ClustalOmegaResult = ClustalOmega.MultipleAlignmentOmega(familyBSequences)

        Call alignmentA.SaveAs(MSAFormat.Fasta, System.IO.Path.Combine(workDir, "___target_aligned.fsa"))
        Call alignmentB.SaveAs(MSAFormat.Fasta, System.IO.Path.Combine(workDir, "__target_partner_aligned.fsa"))

        Call Console.WriteLine("Alignment job done!")

        ' 6. 构造配对联合比对
        Dim builder As New PairedMsaBuilder With {
            .Alphabet = If(Options, New PpiModelOptions()).Alphabet
        }
        Dim pairings As PairedMsaBuilder.Pairing() = interactionPairs _
            .Select(Function(p) New PairedMsaBuilder.Pairing(p.a & "-" & p.b, p.a, p.b)) _
            .Distinct() _
            .ToArray()

        Dim joint As JointAlignment

        Try
            joint = builder.Build(alignmentA, alignmentB, pairings)
        Catch ex As Exception
            Call Console.WriteLine($"Can not build the paired alignment: {ex.Message}")

            Return EmptyResult(GeneId)
        End Try

        Call Console.WriteLine("Paired alignment: {0}", builder.LastReport)

        ' 7. 建模与打分
        Call Console.WriteLine("Start to model the protein interaction Bayesian network...")

        Dim optionsUsed As PpiModelOptions = If(Options, New PpiModelOptions())
        Dim model As New PpiBayesianModel(joint, optionsUsed)
        Dim scoring As New PpiScoring(model)

        Call Console.WriteLine("Dependency tree: {0}", model.FullTree)

        Dim alignA As Dictionary(Of String, String) = AlignedSequences(alignmentA)
        Dim alignB As Dictionary(Of String, String) = AlignedSequences(alignmentB)
        Dim result As New IO.File

        Call result.Add(New String() {"GeneId", "InteractionPartner", "Score", "IsInteraction", "TopContacts"})

        Call Console.WriteLine("Predicting protein interactions...")

        Dim targetSequence As String = Nothing
        Dim alphabet As ResidueAlphabet = optionsUsed.Alphabet

        If Not alignA.TryGetValue(GeneId, targetSequence) Then
            Call Console.WriteLine("The query protein was removed by the column filter, falling back to the unaligned sequence.")
            targetSequence = Nothing
        End If

        For Each partnerId As String In targetHomologs
            Dim partnerSequence As String = Nothing

            If Not alignB.TryGetValue(partnerId, partnerSequence) Then
                Call Console.WriteLine("  partner ""{0}"" was not found in the family B alignment, skipped.", partnerId)
                Continue For
            End If

            Dim codesA As Integer()
            Dim codesB As Integer()

            Try
                If targetSequence Is Nothing Then
                    codesA = alphabet.Encode(familyASequences.First(Function(s) String.Equals(s.Headers.First.Split.First, GeneId)).SequenceData)
                Else
                    codesA = alphabet.Encode(targetSequence)
                End If

                codesB = alphabet.Encode(partnerSequence)
            Catch ex As ArgumentException
                Call Console.WriteLine("  {0} -> {1}: {2}", GeneId, partnerId, ex.Message)
                Continue For
            End Try

            If codesA.Length <> joint.LengthA OrElse codesB.Length <> joint.LengthB Then
                Call Console.WriteLine("  {0} -> {1}: the aligned length does not match the paired alignment, skipped.", GeneId, partnerId)
                Continue For
            End If

            Dim detail As PpiScoring.ScoreDetail = scoring.ScoreDetailed(codesA, codesB)
            Dim explanation As String = ""

            If ExplainResidues Then
                explanation = String.Join("; ", detail.Contributions.Take(5).Select(Function(c) c.ToString()))
            End If

            Call result.Add(New String() {
                GeneId,
                partnerId,
                detail.Score.ToString("0.0000"),
                detail.IsInteraction.ToString(),
                explanation
            })

            Call Console.WriteLine("  {0} -> {1}: X={2:0.0000} interaction={3}", GeneId, partnerId, detail.Score, detail.IsInteraction)
        Next

        Call Console.WriteLine("==================END_OF_PREDITION_OF ""{0}""=====================", GeneId)

        Dim output As String = System.IO.Path.Combine(workDir, String.Format("interaction_partners_for_{0}.csv", GeneId))
        Call result.Save(output, False)

        Return result
    End Function

    Private Shared Function AlignedSequences(alignment As ClustalOmegaResult) As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.Ordinal)

        For i As Integer = 0 To alignment.names.Length - 1
            Dim id As String = alignment.names(i).Split(" "c).First

            If Not result.ContainsKey(id) Then
                result(id) = alignment.MSA(i)
            End If
        Next

        Return result
    End Function

    Private Shared Function EmptyResult(geneId As String) As IO.File
        Dim file As New IO.File

        Call file.Add(New String() {"GeneId", "InteractionPartner", "Score", "IsInteraction", "TopContacts"})

        Return file
    End Function

    ''' <summary>
    ''' 删除空位比例过高的比对列(readme 与论文共有的预处理步骤)。
    ''' </summary>
    ''' <param name="AlignedData">已经比对好的序列集合</param>
    Public Shared Function Trim(AlignedData As FASTA.FastaFile) As FASTA.FastaFile
        If AlignedData Is Nothing OrElse AlignedData.Count = 0 Then
            Return AlignedData
        End If

        Dim sequences As List(Of Char)() = (From fsa In AlignedData Select fsa.SequenceData.AsList).ToArray
        Dim cutoff As Integer = CInt(AlignedData.Count * 0.75)
        Dim position As Integer = 0

        Do While position < sequences(0).Count
            Dim gaps As Integer = 0

            For i As Integer = 0 To sequences.Length - 1
                If sequences(i)(position) = "-"c Then
                    gaps += 1
                End If
            Next

            If gaps >= cutoff Then
                For i As Integer = 0 To sequences.Length - 1
                    Call sequences(i).RemoveAt(position)
                Next
            Else
                position += 1
            End If
        Loop

        For i As Integer = 0 To AlignedData.Count - 1
            AlignedData(i).SequenceData = sequences(i).ToArray
        Next

        Return AlignedData
    End Function

    Public Class DipRecord
        <Column("ID interactor A")> Public Property IdInteractorA As String
        <Column("ID interactor B")> Public Property IdInteractorB As String
        <Column("Interaction detection method(s)")> Public Property InteractionDetectionMethods As String
        <Column("Publication Identifier(s)")> Public Property PublicationIdentifiers As String
        <Column("Taxid interactor A")> Public Property TaxidInteractorA As String
        <Column("Taxid interactor B")> Public Property TaxidInteractorB As String
        <Column("Interaction type(s)")> Public Property InteractionTypes As String
        <Column("Source database(s)")> Public Property SourceDatabases As String
        <Column("Interaction identifier(s)")> Public Property InteractionIdentifiers As String
        <Column("Confidence value(s)")> Public Property ConfidenceValues As String
        <Column("Processing Status")> Public Property ProcessingStatus As String

        Public Shared Function Convert(DataSet As DipRecord()) As DipRecord()
            Dim ChunkBuffer As DipRecord() = New DipRecord(DataSet.Count - 1) {}

            For i As Integer = 0 To DataSet.Count - 1
                Dim itemObject = DataSet(i)
                itemObject.IdInteractorA = itemObject.IdInteractorA.Split(CChar("|")).First
                itemObject.IdInteractorB = itemObject.IdInteractorB.Split(CChar("|")).First

                ChunkBuffer(i) = itemObject
            Next

            Return ChunkBuffer
        End Function
    End Class
End Class
