// export R# package module type define for javascript/typescript language
//
//    imports "intervention" from "biosystem";
//
// ref=biosystem.InterventionTools@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * permutation experiment and analysis tools for the in silico gene perturbation experiment
 * 
*/
declare namespace intervention {
   /**
    * run the in silico gene knockdown experiment on the given network model
    * 
    * 
     * @param bnlearn the trained network model object, which could be created by the ``bnlearn`` 
     *  api or the ``GEARS::new`` api.
     * @param geneNames a character vector of the gene id for run the knockdown experiment, one 
     *  [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object will be generated for each gene.
     * @param verbose 
     * + default value Is ``false``.
     * @return a vector of the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  knocked down, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function knockdown(bnlearn: any, geneNames: any, verbose?: boolean): object|object;
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
     *  [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object will be generated for each gene.
     * @param verbose 
     * + default value Is ``false``.
     * @return a vector of the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  knocked out, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function knockouts(bnlearn: any, geneNames: any, verbose?: boolean): object|object;
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
     *  the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object, or a pipeline object that 
     *  produces a set of the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) data.
     * @param dir the output directory for save the generated csv table files, this directory 
     *  will be created if it is not exists.
     * @param pathway_info an optional tuple list of the [MetabolicPathway](cref:T:SMRUCC.genomics.MetabolicModel.MetabolicPathway) knowledge data: 
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
     *  cast to a collection of the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) data.
   */
   function make_exports(results: any, dir: string, pathway_info?: object, top_n?: object, env?: object): any;
   /**
    * run the in silico gene overexpression experiment on the given network model
    * 
    * 
     * @param bnlearn the trained network model object, which could be created by the ``bnlearn`` 
     *  api or the ``GEARS::new`` api.
     * @param geneNames a character vector of the gene id for run the overexpression experiment, 
     *  one [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) object will be generated for each gene.
     * @param verbose 
     * + default value Is ``false``.
     * @return a vector of the [InterventionResult](cref:T:SMRUCC.genomics.BioSystem.Intervention.InterventionResult) perturbation result: the 
     *  ``WildtypeMeans`` is the wildtype expression value of each gene, the 
     *  ``MutantMeans`` is the expression value of each gene after the gene has been 
     *  overexpressed, and the ``FoldChanges``, ``PercentChanges``, ``ZScores`` and 
     *  ``IsSignificant`` data is the differential analysis result of the 
     *  perturbation.
   */
   function overexpress(bnlearn: any, geneNames: any, verbose?: boolean): object|object;
   /**
   */
   function target_gene(result: object): string;
}
