Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Bootstrap
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.MaximumLikelihood
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.TreeIO
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Evolution

    ''' <summary>
    ''' 进化树构建算法对外暴露的统一 API：串联“FASTA/比对 → 位点矩阵 / 距离矩阵 → 建树 → bootstrap 检验”的完整工作流。
    ''' </summary>
    ''' <remarks>
    ''' 提供两种输入入口：
    ''' <list type="bullet">
    ''' <item>由 FASTA（可选自动多序列比对）推导字符矩阵与距离矩阵；</item>
    ''' <item>直接使用 PHYLIP 距离矩阵文件（复用 <c>MatrixFile</c> 工作流）。</item>
    ''' </list>
    ''' </remarks>
    Public Module EvolutionAPI

        ''' <summary>
        ''' 从 FASTA 文件构建位点矩阵。<paramref name="align"/> 为 True 时先执行多序列比对。
        ''' </summary>
        <ExportAPI("evolution.character_matrix")>
        Public Function LoadCharacterMatrix(fasta As String, Optional align As Boolean = False) As CharacterMatrix
            Dim fa As FastaFile = FastaFile.Read(fasta)

            If align Then
                Return CharacterMatrix.FromMSA(fa.MultipleAlignment())
            Else
                Return CharacterMatrix.FromFasta(fa)
            End If
        End Function

        ''' <summary>
        ''' 从 FASTA 文件估计两两距离矩阵。
        ''' </summary>
        <ExportAPI("evolution.distance_matrix")>
        Public Function LoadDistanceMatrix(fasta As String,
                                           Optional align As Boolean = False,
                                           Optional model As DistanceModel = DistanceModel.PoissonCorrection) As DistanceMatrix

            Return Distance.SequenceDistance.PairwiseMatrix(LoadCharacterMatrix(fasta, align), model)
        End Function

        ''' <summary>
        ''' 读取 PHYLIP 格式的距离矩阵文件。
        ''' </summary>
        <ExportAPI("evolution.read_phylip_matrix")>
        Public Function ReadPhylipDistanceMatrix(path As String) As DistanceMatrix
            Return DistanceMatrix.ReadPhylip(path)
        End Function

        ''' <summary>
        ''' 使用指定算法由 FASTA（已比对或自动比对）构建进化树。
        ''' </summary>
        <ExportAPI("evolution.build_tree")>
        Public Function BuildEvolutionTree(fasta As String,
                                           Optional algorithm As EvolutionAlgorithm = EvolutionAlgorithm.NeighborJoining,
                                           Optional aminoAcidModel As AminoAcidModel = AminoAcidModel.LG,
                                           Optional distanceModel As DistanceModel = DistanceModel.PoissonCorrection,
                                           Optional align As Boolean = False,
                                           Optional rateCategories As Integer = 4,
                                           Optional gammaShape As Double = 1.0,
                                           Optional seed As Integer = 1234) As PhyloTree

            Dim matrix As CharacterMatrix = LoadCharacterMatrix(fasta, align)
            Dim options As New EvolutionOptions With {
                .AminoAcidModel = aminoAcidModel,
                .DistanceModel = distanceModel,
                .RateCategories = rateCategories,
                .GammaShape = gammaShape,
                .Seed = seed
            }

            Return PhyloTreeFactory.ToTree(TreeBuilder.Build(matrix, algorithm, options), algorithm.ToString())
        End Function

        ''' <summary>
        ''' 使用指定算法由距离矩阵构建进化树（仅支持 UPGMA / NJ）。
        ''' </summary>
        <ExportAPI("evolution.build_tree_from_distance")>
        Public Function BuildEvolutionTree(distances As DistanceMatrix, algorithm As EvolutionAlgorithm) As PhyloTree
            Return PhyloTreeFactory.ToTree(TreeBuilder.Build(distances, algorithm), algorithm.ToString())
        End Function

        ''' <summary>
        ''' 使用指定算法由位点矩阵构建进化树。
        ''' </summary>
        <ExportAPI("evolution.build_tree_from_matrix")>
        Public Function BuildEvolutionTree(matrix As CharacterMatrix,
                                           algorithm As EvolutionAlgorithm,
                                           Optional options As EvolutionOptions = Nothing) As PhyloTree

            Return PhyloTreeFactory.ToTree(TreeBuilder.Build(matrix, algorithm, options), algorithm.ToString())
        End Function

        ''' <summary>
        ''' 执行 bootstrap 支持度评估，并在返回的参考树内部节点上标注支持度（百分比）。
        ''' </summary>
        <ExportAPI("evolution.bootstrap")>
        Public Function BootstrapEvolutionTree(fasta As String,
                                               Optional algorithm As EvolutionAlgorithm = EvolutionAlgorithm.NeighborJoining,
                                               Optional replicates As Integer = 100,
                                               Optional aminoAcidModel As AminoAcidModel = AminoAcidModel.LG,
                                               Optional distanceModel As DistanceModel = DistanceModel.PoissonCorrection,
                                               Optional align As Boolean = False,
                                               Optional seed As Integer = 1234) As PhyloTree

            Dim matrix As CharacterMatrix = LoadCharacterMatrix(fasta, align)
            Dim options As New EvolutionOptions With {
                .AminoAcidModel = aminoAcidModel,
                .DistanceModel = distanceModel,
                .Seed = seed
            }

            Dim bootstrap As BootstrapResult = BootstrapAnalysis.Run(matrix, algorithm, replicates, options)

            Return PhyloTreeFactory.ToTree(bootstrap.Tree, algorithm.ToString() & " (bootstrap)")
        End Function

        ''' <summary>
        ''' 将树对象输出为 Newick 文本。
        ''' </summary>
        <ExportAPI("evolution.newick")>
        Public Function ToNewick(tree As PhyloNode, Optional showBootstrap As Boolean = True) As String
            Return PhyloTreeFactory.CreateNewick(tree, showBootstrap:=showBootstrap)
        End Function
    End Module

End Namespace
