
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork
Imports SMRUCC.genomics.BioSystem
Imports SMRUCC.genomics.BioSystem.Intervention
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

''' <summary>
''' permutation experiment and analysis tools for the in silico gene perturbation experiment
''' </summary>
<Package("intervention")>
Module InterventionTools

    ''' <summary>
    ''' run the in silico gene knockout experiment on the given network model
    ''' </summary>
    ''' <param name="bnlearn">
    ''' the trained network model object, which could be created by the ``bnlearn`` 
    ''' api or the ``GEARS::new`` api.
    ''' </param>
    ''' <param name="geneNames">
    ''' a character vector of the gene id for run the knockout experiment, one 
    ''' <see cref="InterventionResult"/> object will be generated for each gene.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="InterventionResult"/> perturbation result: the 
    ''' ``WildtypeMeans`` is the wildtype expression value of each gene, the 
    ''' ``MutantMeans`` is the expression value of each gene after the gene has been 
    ''' knocked out, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
    ''' ``IsSignificant`` data is the differential analysis result of the 
    ''' perturbation.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the behavior of this function is determined by the ``strict`` option of the 
    ''' input network model: an error will be thrown when the target gene is missing 
    ''' from the network model in the strict mode, otherwise the wildtype expression 
    ''' data will be returned as the result with the ``Undefined`` flag marked as 
    ''' TRUE.
    ''' </remarks>
    <ExportAPI("knockouts")>
    <RApiReturn(GetType(InterventionResult), GetType(GlobalPerturbationResult))>
    Public Function KnockoutGene(bnlearn As Object, <RRawVectorArgument(TypeCodes.string)> geneNames As Object, Optional verbose As Boolean = False) As Object
        If TypeOf bnlearn Is ModularNetworkPipeline Then
            Return DirectCast(bnlearn, ModularNetworkPipeline).InsilicoPerturbation(CLRVector.asCharacter(geneNames), InterventionMode.Knockout).ToArray
        Else
            Dim result As New List(Of InterventionResult)

            If TypeOf bnlearn Is vbObject Then
                bnlearn = DirectCast(bnlearn, vbObject).target
            ElseIf bnlearn Is Nothing Then
                Return Nothing
            End If

            For Each geneName As String In TqdmWrapper.Wrap(CLRVector.asCharacter(geneNames), wrap_console:=verbose)
                Call result.Add(DirectCast(bnlearn, InsilicoPerturbationExperiment).KnockoutGene(geneName))
            Next

            Return result.ToArray
        End If
    End Function

    ''' <summary>
    ''' run the in silico gene overexpression experiment on the given network model
    ''' </summary>
    ''' <param name="bnlearn">
    ''' the trained network model object, which could be created by the ``bnlearn`` 
    ''' api or the ``GEARS::new`` api.
    ''' </param>
    ''' <param name="geneNames">
    ''' a character vector of the gene id for run the overexpression experiment, 
    ''' one <see cref="InterventionResult"/> object will be generated for each gene.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="InterventionResult"/> perturbation result: the 
    ''' ``WildtypeMeans`` is the wildtype expression value of each gene, the 
    ''' ``MutantMeans`` is the expression value of each gene after the gene has been 
    ''' overexpressed, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
    ''' ``IsSignificant`` data is the differential analysis result of the 
    ''' perturbation.
    ''' </returns>
    <ExportAPI("overexpress")>
    <RApiReturn(GetType(InterventionResult), GetType(GlobalPerturbationResult))>
    Public Function overexpress(bnlearn As Object, <RRawVectorArgument(TypeCodes.string)> geneNames As Object, Optional verbose As Boolean = False) As Object
        If TypeOf bnlearn Is ModularNetworkPipeline Then
            Return DirectCast(bnlearn, ModularNetworkPipeline).InsilicoPerturbation(CLRVector.asCharacter(geneNames), InterventionMode.Overexpression).ToArray
        Else
            Dim result As New List(Of InterventionResult)

            If TypeOf bnlearn Is vbObject Then
                bnlearn = DirectCast(bnlearn, vbObject).target
            ElseIf bnlearn Is Nothing Then
                Return Nothing
            End If

            For Each geneName As String In TqdmWrapper.Wrap(CLRVector.asCharacter(geneNames), wrap_console:=verbose)
                Call result.Add(DirectCast(bnlearn, InsilicoPerturbationExperiment).OverexpressGene(geneName))
            Next

            Return result.ToArray
        End If
    End Function

    ''' <summary>
    ''' run the in silico gene knockdown experiment on the given network model
    ''' </summary>
    ''' <param name="bnlearn">
    ''' the trained network model object, which could be created by the ``bnlearn`` 
    ''' api or the ``GEARS::new`` api.
    ''' </param>
    ''' <param name="geneNames">
    ''' a character vector of the gene id for run the knockdown experiment, one 
    ''' <see cref="InterventionResult"/> object will be generated for each gene.
    ''' </param>
    ''' <returns>
    ''' a vector of the <see cref="InterventionResult"/> perturbation result: the 
    ''' ``WildtypeMeans`` is the wildtype expression value of each gene, the 
    ''' ``MutantMeans`` is the expression value of each gene after the gene has been 
    ''' knocked down, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
    ''' ``IsSignificant`` data is the differential analysis result of the 
    ''' perturbation.
    ''' </returns>
    <ExportAPI("knockdown")>
    <RApiReturn(GetType(InterventionResult), GetType(GlobalPerturbationResult))>
    Public Function knockdownGene(bnlearn As Object, <RRawVectorArgument(TypeCodes.string)> geneNames As Object, Optional verbose As Boolean = False) As Object
        If TypeOf bnlearn Is ModularNetworkPipeline Then
            Return DirectCast(bnlearn, ModularNetworkPipeline).InsilicoPerturbation(CLRVector.asCharacter(geneNames), InterventionMode.Knockdown).ToArray
        Else
            Dim result As New List(Of InterventionResult)

            If TypeOf bnlearn Is vbObject Then
                bnlearn = DirectCast(bnlearn, vbObject).target
            ElseIf bnlearn Is Nothing Then
                Return Nothing
            End If

            For Each geneName As String In TqdmWrapper.Wrap(CLRVector.asCharacter(geneNames), wrap_console:=verbose)
                Call result.Add(DirectCast(bnlearn, InsilicoPerturbationExperiment).KnockDownGene(geneName))
            Next

            Return result.ToArray
        End If
    End Function

    <ExportAPI("target_gene")>
    Public Function target_gene(result As InterventionResult) As String
        Return result.Spec.GeneName
    End Function

    ''' <summary>
    ''' export the virtual permutation result as csv table files
    ''' </summary>
    ''' <param name="results">
    ''' a collection of the in silico perturbation result, which can be a vector of 
    ''' the <see cref="InterventionResult"/> object, or a pipeline object that 
    ''' produces a set of the <see cref="InterventionResult"/> data.
    ''' </param>
    ''' <param name="dir">
    ''' the output directory for save the generated csv table files, this directory 
    ''' will be created if it is not exists.
    ''' </param>
    ''' <param name="pathway_info">
    ''' an optional tuple list of the <see cref="MetabolicPathway"/> knowledge data: 
    ''' the slot key of the list is the pathway id and the slot value is the 
    ''' corresponding pathway object, this parameter is used for run the pathway 
    ''' level analysis of the perturbation result.
    ''' </param>
    ''' <param name="top_n">
    ''' the top n affected genes for export in the intervention ranking table, by 
    ''' default is 50.
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' TRUE will be returns if all of the result table files have been exported 
    ''' into the given directory successfully;
    ''' 
    ''' this function returns a R# error message object if the given data can not be 
    ''' cast to a collection of the <see cref="InterventionResult"/> data.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the generated csv table files in the given output directory:
    ''' 
    ''' + ``foldchange_matrix.csv``, ``percentchange_matrix.csv``, 
    '''   ``significance_matrix.csv``, ``zscore_matrix.csv``, 
    '''   ``wildtype_means_matrix.csv``, ``mutant_means_matrix.csv``;
    ''' + ``comprehensive_comparison.csv``, ``condition_similarity.csv``;
    ''' + ``intervention_ranking.csv``: the top n affected genes of each 
    '''   perturbation condition;
    ''' + ``pathway_summary.csv`` and ``cross_impact_matrix.csv``: these two table 
    '''   files will be generated only when the ``pathway_info`` parameter is 
    '''   provided.
    ''' </remarks>
    <ExportAPI("make_exports")>
    Public Function make_exports(<RRawVectorArgument(GetType(InterventionResult))> results As Object, dir As String,
                                 <RRawVectorArgument>
                                 Optional pathway_info As List = Nothing,
                                 Optional top_n As Integer = 50,
                                 Optional env As Environment = Nothing) As Object

        Dim pull As PipeIterator(Of InterventionResult) = pipeline.Stream(Of InterventionResult)(results, env)
        Dim pathways As Dictionary(Of String, MetabolicPathway) = Nothing

        If pull.isError Then
            Return pull.getError
        End If
        If pathway_info IsNot Nothing Then
            pathways = pathway_info.asGeneric(Of MetabolicPathway)(env)
        End If

        Call New InterventionComparisonExporter(pull).ExportAll(dir, pathways, topN:=top_n)

        Return True
    End Function
End Module
