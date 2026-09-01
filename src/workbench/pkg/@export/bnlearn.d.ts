// export R# package module type define for javascript/typescript language
//
//    imports "bnlearn" from "biosystem";
//
// ref=biosystem.bnlearn@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * Bayesian network learning and the in silico gene perturbation toolkit
 * 
 * > This R# package module provides the toolkit for learn the gene regulatory 
 * >  bayesian network from the gene expression data, and then run the in silico 
 * >  gene perturbation experiment based on the learned network model:
 * >  
 * >  + ``prior_network`` and ``as.prior_net``: create the prior knowledge 
 * >    regulatory network(TF -> target gene) which is used as the whitelist of 
 * >    the network structure learning;
 * >  + ``bnlearn``: learn the network structure(MMHC algorithm with the whitelist 
 * >    prior) and the network parameters(Gaussian bayesian network MLE);
 * >  + ``knockouts``, ``overexpress`` and ``knockdown``: run the in silico gene 
 * >    perturbation experiment on the learned network model;
 * >  + ``make_exports``: export the perturbation experiment result as a set of the 
 * >    csv table files;
 * >  + ``save_model``: save the learned bayesian network model as the tsv table 
 * >    files.
*/
declare namespace bnlearn {
   module as {
      /**
       * build prior network object based on a given vector of the knowledge network edges data
       * 
       * 
        * @param priorNet a collection of the regulatory edge data, which can be a vector of the 
        *  @``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge`` object, the output of the ``prior_network`` 
        *  api, or a pipeline object that produces a set of the 
        *  @``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge`` data.
        * @param env the R# runtime environment object.
        * 
        * + default value Is ``null``.
        * @return a @``T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork`` object that contains all of the given 
        *  regulatory edges, the TF name set and the target gene name set;
        *  
        *  this function returns a R# error message object if the given data can not 
        *  be cast to a collection of the @``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge`` data.
      */
      function prior_net(priorNet: any, env?: object): object;
   }
   /**
    * learn the gene regulatory bayesian network from the gene expression data
    * 
    * > this function runs the network learning in two steps:
    * >  
    * >  1. the structure learning: the MMHC algorithm with the whitelist prior 
    * >     network;
    * >  2. the parameter learning: the maximum likelihood estimation(MLE) of the 
    * >     Gaussian bayesian network.
    * 
     * @param exprData the gene expression matrix object, could be load from csv file via ``geneExpression::load.expr`` api
     * @param priorNet a collection of the prior knowledge regulatory edge data 
     *  (@``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge``), which is used as the whitelist of the 
     *  network structure learning: only the regulation relation that is described 
     *  in this prior network will be considered in the structure learning.
     *  
     *  this parameter is optional, the network structure will be learned from the 
     *  expression data alone if the prior network is not provided.
     * 
     * + default value Is ``null``.
     * @param modules 
     * + default value Is ``null``.
     * @param TF 
     * + default value Is ``null``.
     * @param max_itrs the max iteration numbers of the network structure learning, by default is 
     *  500.
     * 
     * + default value Is ``500``.
     * @param crossModuleCorThreshold 
     * + default value Is ``0.3``.
     * @param strict the strict option of the in silico perturbation experiment: if this 
     *  parameter is TRUE, then an error will be thrown when the target gene of the 
     *  perturbation is missing from the learned network; if this parameter is 
     *  FALSE, then a warning message will be printed and the wildtype expression 
     *  data will be returned as the perturbation result with the ``Undefined`` flag 
     *  marked as TRUE.
     *  
     *  if this parameter is not specified, then the strict option of the R# 
     *  runtime environment will be used.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a @``T:SMRUCC.genomics.Analysis.BNLearn.Core.BNLearnWorkflow`` object that contains the learned bayesian 
     *  network model, which implements the 
     *  @``T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment`` interface, so that it can be 
     *  used by the ``knockouts``, ``overexpress`` and ``knockdown`` api for run 
     *  the in silico gene perturbation experiment;
     *  
     *  this function returns a R# error message object if the given prior network 
     *  data can not be cast to a collection of the 
     *  @``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge`` data.
   */
   function bnlearn(exprData: object, priorNet?: any, modules?: any, TF?: any, max_itrs?: object, crossModuleCorThreshold?: number, strict?: object, env?: object): object|object;
   /**
    * run the in silico gene knockdown experiment on the given network model
    * 
    * 
     * @param bnlearn the trained network model object, which could be created by the ``bnlearn`` 
     *  api or the ``GEARS::new`` api.
     * @param geneNames a character vector of the gene id for run the knockdown experiment, one 
     *  @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` object will be generated for each gene.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  knocked down, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function knockdown(bnlearn: object, geneNames: any): object;
   /**
    * run the in silico gene knockout experiment on the given network model
    * 
    * > the behavior of this function is determined by the ``strict`` option of the 
    * >  input network model: an error will be thrown when the target gene is missing 
    * >  from the network model in the strict mode, otherwise the wildtype expression 
    * >  data will be returned as the result with the ``Undefined`` flag marked as 
    * >  TRUE.
    * 
     * @param bnlearn the trained network model object, which could be created by the ``bnlearn`` 
     *  api or the ``GEARS::new`` api.
     * @param geneNames a character vector of the gene id for run the knockout experiment, one 
     *  @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` object will be generated for each gene.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  knocked out, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function knockouts(bnlearn: object, geneNames: any): object;
   /**
    * export the virtual permutation result as csv table files
    * 
    * > the generated csv table files in the given output directory:
    * >  
    * >  + ``foldchange_matrix.csv``, ``percentchange_matrix.csv``, 
    * >    ``significance_matrix.csv``, ``zscore_matrix.csv``, 
    * >    ``wildtype_means_matrix.csv``, ``mutant_means_matrix.csv``;
    * >  + ``comprehensive_comparison.csv``, ``condition_similarity.csv``;
    * >  + ``intervention_ranking.csv``: the top n affected genes of each 
    * >    perturbation condition;
    * >  + ``pathway_summary.csv`` and ``cross_impact_matrix.csv``: these two table 
    * >    files will be generated only when the ``pathway_info`` parameter is 
    * >    provided.
    * 
     * @param results a collection of the in silico perturbation result, which can be a vector of 
     *  the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` object, or a pipeline object that 
     *  produces a set of the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` data.
     * @param dir the output directory for save the generated csv table files, this directory 
     *  will be created if it is not exists.
     * @param pathway_info an optional tuple list of the @``T:SMRUCC.genomics.MetabolicModel.MetabolicPathway`` knowledge data: 
     *  the slot key of the list is the pathway id and the slot value is the 
     *  corresponding pathway object, this parameter is used for run the pathway 
     *  level analysis of the perturbation result.
     * 
     * + default value Is ``null``.
     * @param top_n the top n affected genes for export in the intervention ranking table, by 
     *  default is 50.
     * 
     * + default value Is ``50``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return TRUE will be returns if all of the result table files have been exported 
     *  into the given directory successfully;
     *  
     *  this function returns a R# error message object if the given data can not be 
     *  cast to a collection of the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` data.
   */
   function make_exports(results: any, dir: string, pathway_info?: object, top_n?: object, env?: object): any;
   /**
    * run the in silico gene overexpression experiment on the given network model
    * 
    * 
     * @param bnlearn the trained network model object, which could be created by the ``bnlearn`` 
     *  api or the ``GEARS::new`` api.
     * @param geneNames a character vector of the gene id for run the overexpression experiment, 
     *  one @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` object will be generated for each gene.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult`` perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  overexpressed, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function overexpress(bnlearn: object, geneNames: any, env?: object): object;
   /**
    * create prior knowledge netwoek edges from the given vector data
    * 
    * 
     * @param TF a character vector of the transcript factor protein/rna id of each 
     *  regulatory edge.
     * @param target_gene a character vector of the target gene id of each regulatory edge.
     * @param regulation_type a character vector of the regulation type of each regulatory edge, the 
     *  value could be ``Unknown``, ``Activator`` or ``Inhibitor``.
     * @param confidence a numeric vector of the confidence score of each regulatory edge, which 
     *  should be a value in the range ``[0,1]``.
     * @param evidence a character vector of the evidence source description of each regulatory 
     *  edge.
     * @return a vector of the @``T:SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge`` regulatory edge data, all of 
     *  the input vectors should be in the same size as the input TF vector, this 
     *  generated edge collection can be used as the whitelist of the network 
     *  structure learning via the ``bnlearn`` api, or be converted to a 
     *  @``T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork`` object via the ``as.prior_net`` api.
   */
   function prior_network(TF: any, target_gene: any, regulation_type: any, confidence: any, evidence: any): object;
   /**
    * save bnlearn model
    * 
    * > the learned bayesian network model will be saved as two tsv table files in 
    * >  the given output directory: the ``network_structure.tsv`` file for the 
    * >  network structure data and the ``network_parameters.tsv`` file for the 
    * >  conditional probability distribution(CPD) parameter data of each network 
    * >  node.
    * 
     * @param bnlearn the trained @``T:SMRUCC.genomics.Analysis.BNLearn.Core.BNLearnWorkflow`` network model object, which is the 
     *  output of the ``bnlearn`` api.
     * @param dir the output directory for save the network model data, this directory will be 
     *  created if it is not exists.
     * @return TRUE will be returns if the network model data has been saved into the given 
     *  directory successfully.
   */
   function save_model(bnlearn: object, dir: string): any;
}
