
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.IO
Imports SMRUCC.genomics.Analysis.GEARS
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("GEARS")>
<RTypeExport("GEARS_opts", GetType(GEARSConfig))>
Public Module gearsTools

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="prior"></param>
    ''' <param name="config"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <see cref="GEARS"/> implements of the interface <see cref="InsilicoPerturbationExperiment"/>, which could be used as the virtual perturbation experiment container for run knockouts/overexpress/knockdown experiments
    ''' </remarks>
    ''' <example>
    ''' imports ["GEARS","bnlearn"] from "biosystem";
    ''' imports "geneExpression" from "phenotype_kit";
    ''' 
    ''' let exprData = load.expr("./dataset.csv");
    ''' let samples = read.sampleinfo("./experiment.csv");
    ''' let opts = new("GEARS_opts", Activation = "tanh");
    ''' let net = bnlearn::prior_network(
    '''     TF = c(), target_gene = c(), regulation_type = c(), confidence = c(), evidence = c()
    ''' ) |> as.prior_net();
    ''' 
    ''' net &lt;- GEARS::new(exprData, net, opts)
    ''' |> training_set(exprData, controls = c(), perturbed = samples)
    ''' |> train()
    ''' ;
    ''' 
    ''' make_exports(c( 
    '''    net |> knockouts(c()), 
    '''    net |> overexpress(c()), 
    '''    net |> knockdown(c()) 
    ''' ), dir = "./GNN_result"); 
    ''' </example>
    <ExportAPI("new")>
    <RApiReturn(GetType(GEARS))>
    Public Function create_GEARS(x As Matrix, prior As PriorNetwork, config As GEARSConfig) As GEARS
        Dim exprData As GeneExpressionData = BnIO.ReadGeneExpressionMatrix(x)
        Dim gears As New GEARS(exprData, prior, config)
        Return gears
    End Function

    ''' <summary>
    ''' Set the training sample set
    ''' </summary>
    ''' <param name="gears"></param>
    ''' <param name="x"></param>
    ''' <param name="controls"></param>
    ''' <param name="perturbed"></param>
    ''' <param name="env"></param>
    ''' <returns></returns>
    <ExportAPI("training_set")>
    <RApiReturn(GetType(GEARS))>
    Public Function set_trainingSampleSet(gears As GEARS, x As Matrix,
                                          <RRawVectorArgument(TypeCodes.string)> controls As Object,
                                          <RRawVectorArgument(GetType(SampleInfo))> perturbed As Object,
                                          Optional env As Environment = Nothing) As Object

        Dim controlsId As String() = CLRVector.asCharacter(controls)
        Dim perturbedSet As PipeIterator(Of SampleInfo) = pipeline.Stream(Of SampleInfo)(perturbed, env)

        If perturbedSet.isError Then
            Return perturbedSet.getError
        End If

        Return gears.SetTrainingSamples(x, controlsId, perturbedSet.ToArray)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="gears"></param>
    ''' <returns></returns>
    <ExportAPI("train")>
    <RApiReturn(GetType(GEARS))>
    Public Function train(gears As GEARS) As GEARS
        Call gears.Train()
        Return gears
    End Function

End Module
