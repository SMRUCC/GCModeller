
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
    ''' <param name="gears">
    ''' a <see cref="GEARS"/> model object that is created by the ``GEARS::new`` 
    ''' api.
    ''' </param>
    ''' <param name="x">
    ''' the gene expression matrix object of the Perturb-seq experiment data, which 
    ''' should contains both of the control sample columns and the perturbed sample 
    ''' columns.
    ''' </param>
    ''' <param name="controls">
    ''' a character vector of the control(wildtype) sample column names in the given 
    ''' expression matrix: the mean value and the standard deviation of these control 
    ''' sample columns will be used as the shared wildtype baseline of the model 
    ''' training, at least two control sample columns are required.
    ''' </param>
    ''' <param name="perturbed">
    ''' a collection of the <see cref="SampleInfo"/> sample information data of the 
    ''' perturbed samples: the ``ID`` property of the sample data should be matched 
    ''' with the sample column of the given expression matrix, the perturbed gene id 
    ''' set of each sample is stored as a json string array in the ``metadata`` 
    ''' property via the ``perturbed_genes`` key, and the intervention mode of each 
    ''' sample could be stored in the ``metadata`` property via the 
    ''' ``intervention_mode`` key(the value should be one of ``Knockout``, 
    ''' ``Knockdown``, ``Overexpression`` or ``Custom``).
    ''' </param>
    ''' <param name="env">the R# runtime environment object.</param>
    ''' <returns>
    ''' the input <see cref="GEARS"/> model object that the training sample set has 
    ''' been set;
    ''' 
    ''' this function returns a R# error message object if the given perturbed sample 
    ''' data can not be cast to a collection of the <see cref="SampleInfo"/> data.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the wildtype baseline(the mean and the standard deviation of each gene) of 
    ''' the GEARS model will be recomputed from the given control sample columns, so 
    ''' that this api should be called before the ``train`` api.
    ''' </remarks>
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
    ''' Train the GEARS model with the given training sample set
    ''' </summary>
    ''' <param name="gears">
    ''' a <see cref="GEARS"/> model object that is created by the ``GEARS::new`` api, 
    ''' and the training sample set has been set via the ``training_set`` api.
    ''' </param>
    ''' <returns>
    ''' the input <see cref="GEARS"/> model object that has been trained, which can 
    ''' be used for run the in silico gene perturbation experiment via the 
    ''' ``knockouts``, ``overexpress`` and ``knockdown`` api of the ``bnlearn`` 
    ''' package module.
    ''' </returns>
    ''' 
    ''' <remarks>
    ''' the training epoch numbers and the learning rate of the model training is 
    ''' determined by the ``GEARS_opts`` configuration object that is given at the 
    ''' model creation time, and the loss value of each training epoch is stored in 
    ''' the ``LossCurve`` property of the trained model object.
    ''' 
    ''' an error will be thrown if the training sample set is empty, i.e. there is 
    ''' no gene of the prior network could be mapped into the gene expression 
    ''' matrix.
    ''' </remarks>
    <ExportAPI("train")>
    <RApiReturn(GetType(GEARS))>
    Public Function train(gears As GEARS) As GEARS
        Call gears.Train()
        Return gears
    End Function

End Module
