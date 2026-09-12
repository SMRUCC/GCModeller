Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Distance
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.MaximumLikelihood
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Models
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolution.Parsimony
Imports SMRUCC.genomics.Interops.Visualize.Phylip.Evolview

Namespace Evolution

    ''' <summary>
    ''' 进化树构建算法的类型
    ''' </summary>
    Public Enum EvolutionAlgorithm
        ''' <summary>非加权组平均法（距离法，有根树）</summary>
        UPGMA
        ''' <summary>邻接法（距离法，无根树）</summary>
        NeighborJoining
        ''' <summary>最大简约法（Fitch）</summary>
        MaximumParsimony
        ''' <summary>最大似然法</summary>
        MaximumLikelihood
        ''' <summary>贝叶斯推断（MCMC）</summary>
        Bayesian
    End Enum

    ''' <summary>
    ''' 建树算法的通用参数。
    ''' </summary>
    Public Class EvolutionOptions
        Public Property DistanceModel As DistanceModel = DistanceModel.PoissonCorrection
        Public Property AminoAcidModel As AminoAcidModel = AminoAcidModel.LG
        Public Property RateCategories As Integer = 4
        Public Property GammaShape As Double = 1.0
        Public Property InvariantProportion As Double = 0
        Public Property MaxIterations As Integer = 0
        Public Property Seed As Integer = 1234

        Public Shared ReadOnly Property Defaults As EvolutionOptions
            Get
                Return New EvolutionOptions
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 统一的建树入口：根据算法名称分派到对应的算法模块。
    ''' </summary>
    Public Module TreeBuilder

        ''' <summary>
        ''' 由比对位点矩阵构建进化树。
        ''' </summary>
        Public Function Build(matrix As CharacterMatrix,
                              algorithm As EvolutionAlgorithm,
                              Optional options As EvolutionOptions = Nothing) As PhyloNode

            If options Is Nothing Then
                options = New EvolutionOptions
            End If

            Console.WriteLine($"[Evolution] building tree with algorithm: {algorithm}")

            Select Case algorithm
                Case EvolutionAlgorithm.UPGMA
                    Return UPGMA.UpgmaTree.Build(Distance.SequenceDistance.PairwiseMatrix(matrix, options.DistanceModel))
                Case EvolutionAlgorithm.NeighborJoining
                    Return NeighborJoining.NeighborJoining.Build(Distance.SequenceDistance.PairwiseMatrix(matrix, options.DistanceModel))
                Case EvolutionAlgorithm.MaximumParsimony
                    Return MaximumParsimony.Build(matrix, maxIterations:=options.MaxIterations, seed:=options.Seed).Tree
                Case EvolutionAlgorithm.MaximumLikelihood
                    Return MaximumLikelihoodTree.Build(matrix,
                                                       model:=options.AminoAcidModel,
                                                       rateCategories:=options.RateCategories,
                                                       gammaShape:=options.GammaShape,
                                                       invariantProportion:=options.InvariantProportion,
                                                       maxIterations:=options.MaxIterations,
                                                       seed:=options.Seed).Tree
                Case EvolutionAlgorithm.Bayesian
                    Return Bayesian.BayesianInference.Run(matrix,
                                                          model:=options.AminoAcidModel,
                                                          rateCategories:=options.RateCategories,
                                                          gammaShape:=options.GammaShape,
                                                          invariantProportion:=options.InvariantProportion,
                                                          seed:=options.Seed).Tree
                Case Else
                    Throw New NotSupportedException($"不支持的建树算法：{algorithm}")
            End Select
        End Function

        ''' <summary>
        ''' 由距离矩阵构建进化树（仅支持 UPGMA 与 NJ 两种距离法）。
        ''' </summary>
        Public Function Build(distances As DistanceMatrix, algorithm As EvolutionAlgorithm) As PhyloNode
            Select Case algorithm
                Case EvolutionAlgorithm.UPGMA
                    Return UPGMA.UpgmaTree.Build(distances)
                Case EvolutionAlgorithm.NeighborJoining
                    Return NeighborJoining.NeighborJoining.Build(distances)
                Case Else
                    Throw New NotSupportedException($"算法 {algorithm} 需要位点矩阵作为输入，无法直接由距离矩阵构建。")
            End Select
        End Function
    End Module

End Namespace
