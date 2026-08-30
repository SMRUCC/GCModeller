
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

''' <summary>
''' GEARS: the graph neural network based in silico perturbation prediction toolkit
''' </summary>
''' 
''' <remarks>
''' This R# package module provides the toolkit for train a GEARS model(Gene 
''' Expression Additive Response Simulator) from the Perturb-seq experiment data, 
''' the trained model can be used for predict the gene expression response of the 
''' in silico gene perturbation(the knockout/overexpression/knockdown 
''' experiment):
''' 
''' + ``new``: create a new GEARS model from the gene expression matrix, the prior 
'''   regulatory network and the model configuration;
''' + ``training_set``: set the Perturb-seq training sample set of the GEARS model;
''' + ``train``: train the GEARS model with the given training sample set.
''' 
''' the trained <see cref="GEARS"/> model object implements the 
''' <see cref="InsilicoPerturbationExperiment"/> interface, so that it can be used 
''' by the ``knockouts``, ``overexpress`` and ``knockdown`` api of the ``bnlearn`` 
''' package module, and the perturbation result can be exported via the 
''' ``make_exports`` api.
''' </remarks>
<Package("GEARS")>
<RTypeExport("GEARS_opts", GetType(GEARSConfig))>
Public Module gearsTools

    ''' <summary>
    ''' create a new GEARS model
    ''' </summary>
    ''' <param name="x">
    ''' the gene expression matrix object of the Perturb-seq experiment data, which 
    ''' could be loaded from a csv table file via the 
    ''' ``geneExpression::load.expr`` api.
    ''' </param>
    ''' <param name="prior">
    ''' the prior knowledge regulatory network object, which could be created by the 
    ''' ``bnlearn::as.prior_net`` api: only the gene that is described in this prior 
    ''' network could be mapped into the gene regulatory graph of the GEARS model.
    ''' </param>
    ''' <param name="config">
    ''' the hyper parameter configuration of the GEARS model, which could be created 
    ''' via the ``new("GEARS_opts")`` syntax in R# environment: the embedding 
    ''' dimension, the hidden layer dimension, the graph convolution layer numbers, 
    ''' the activation function, the learning rate, the epochs, etc.
    ''' </param>
    ''' <returns>
    ''' a new <see cref="GEARS"/> model object that the gene regulatory graph has 
    ''' been created from the given prior network and expression data, the training 
    ''' sample set should be set via the ``training_set`` api at first and then the 
    ''' model can be trained via the ``train`` api.
    ''' </returns>
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
