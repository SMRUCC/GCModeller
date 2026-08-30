// export R# package module type define for javascript/typescript language
//
//    imports "GEARS" from "biosystem";
//
// ref=biosystem.gearsTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * GEARS: the graph neural network based in silico perturbation prediction toolkit
 * 
 * > This R# package module provides the toolkit for train a GEARS model(Gene 
 * >  Expression Additive Response Simulator) from the Perturb-seq experiment data, 
 * >  the trained model can be used for predict the gene expression response of the 
 * >  in silico gene perturbation(the knockout/overexpression/knockdown 
 * >  experiment):
 * >  
 * >  + ``new``: create a new GEARS model from the gene expression matrix, the prior 
 * >    regulatory network and the model configuration;
 * >  + ``training_set``: set the Perturb-seq training sample set of the GEARS model;
 * >  + ``train``: train the GEARS model with the given training sample set.
 * >  
 * >  the trained @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object implements the 
 * >  @``T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment`` interface, so that it can be used 
 * >  by the ``knockouts``, ``overexpress`` and ``knockdown`` api of the ``bnlearn`` 
 * >  package module, and the perturbation result can be exported via the 
 * >  ``make_exports`` api.
*/
declare namespace GEARS {
   /**
    * create a new GEARS model
    * 
    * > @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` implements of the interface @``T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment``, which could be used as the virtual perturbation experiment container for run knockouts/overexpress/knockdown experiments
    * 
     * @param x the gene expression matrix object of the Perturb-seq experiment data, which 
     *  could be loaded from a csv table file via the 
     *  ``geneExpression::load.expr`` api.
     * @param prior the prior knowledge regulatory network object, which could be created by the 
     *  ``bnlearn::as.prior_net`` api: only the gene that is described in this prior 
     *  network could be mapped into the gene regulatory graph of the GEARS model.
     * @param config the hyper parameter configuration of the GEARS model, which could be created 
     *  via the ``new("GEARS_opts")`` syntax in R# environment: the embedding 
     *  dimension, the hidden layer dimension, the graph convolution layer numbers, 
     *  the activation function, the learning rate, the epochs, etc.
     * @return a new @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object that the gene regulatory graph has 
     *  been created from the given prior network and expression data, the training 
     *  sample set should be set via the ``training_set`` api at first and then the 
     *  model can be trained via the ``train`` api.
   */
   function new(x: object, prior: object, config: object): object;
   /**
    * Train the GEARS model with the given training sample set
    * 
    * > the training epoch numbers and the learning rate of the model training is 
    * >  determined by the ``GEARS_opts`` configuration object that is given at the 
    * >  model creation time, and the loss value of each training epoch is stored in 
    * >  the ``LossCurve`` property of the trained model object.
    * >  
    * >  an error will be thrown if the training sample set is empty, i.e. there is 
    * >  no gene of the prior network could be mapped into the gene expression 
    * >  matrix.
    * 
     * @param gears a @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object that is created by the ``GEARS::new`` api, 
     *  and the training sample set has been set via the ``training_set`` api.
     * @return the input @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object that has been trained, which can 
     *  be used for run the in silico gene perturbation experiment via the 
     *  ``knockouts``, ``overexpress`` and ``knockdown`` api of the ``bnlearn`` 
     *  package module.
   */
   function train(gears: object): object;
   /**
    * Set the training sample set
    * 
    * > the wildtype baseline(the mean and the standard deviation of each gene) of 
    * >  the GEARS model will be recomputed from the given control sample columns, so 
    * >  that this api should be called before the ``train`` api.
    * 
     * @param gears a @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object that is created by the ``GEARS::new`` 
     *  api.
     * @param x the gene expression matrix object of the Perturb-seq experiment data, which 
     *  should contains both of the control sample columns and the perturbed sample 
     *  columns.
     * @param controls a character vector of the control(wildtype) sample column names in the given 
     *  expression matrix: the mean value and the standard deviation of these control 
     *  sample columns will be used as the shared wildtype baseline of the model 
     *  training, at least two control sample columns are required.
     * @param perturbed a collection of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` sample information data of the 
     *  perturbed samples: the ``ID`` property of the sample data should be matched 
     *  with the sample column of the given expression matrix, the perturbed gene id 
     *  set of each sample is stored as a json string array in the ``metadata`` 
     *  property via the ``perturbed_genes`` key, and the intervention mode of each 
     *  sample could be stored in the ``metadata`` property via the 
     *  ``intervention_mode`` key(the value should be one of ``Knockout``, 
     *  ``Knockdown``, ``Overexpression`` or ``Custom``).
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return the input @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object that the training sample set has 
     *  been set;
     *  
     *  this function returns a R# error message object if the given perturbed sample 
     *  data can not be cast to a collection of the @``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo`` data.
   */
   function training_set(gears: object, x: object, controls: any, perturbed: any, env?: object): object;
}
