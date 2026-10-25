// export R# package module type define for javascript/typescript language
//
//    imports "ssGrid" from "biosystem";
//
// ref=biosystem.ssGrid@biosystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * The S-system based gene regulatory network inference toolkit
 *  (基于 S-system 动力学系统的基因表达调控网络拟合工具)
 * 
 * > This R# package module exposes the ``SGrid`` pipeline as a set of R functions:
 * >  
 * >  + ``sfit_options``: create the fitting configuration object([FittingOptions](cref:T:SMRUCC.genomics.Analysis.SGrid.GRN.FittingOptions));
 * >  + ``sfit``: infer the S-system gene regulatory network from a gene expression 
 * >    matrix(rows are genes and the columns are the time points), the optional 
 * >    ``prior`` knowledge network([PriorNetwork](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork)) is used as a soft 
 * >    constraint of the network structure;
 * >  + ``network_edges``, ``sgrid_parameters``, ``sgrid_coefficients`` and ``sgrid_r2``:
 * >    query the fitted model, these functions return ``data.frame`` objects;
 * >  + ``sgrid_simulate``: integrate the fitted S-system model and return the 
 * >    trajectory as a ``data.frame``;
 * >  + ``sgrid_context``: map a pseudobulk context(the raw counts) onto the model 
 * >    state space, so that it can be used as the ODE initial state;
 * >  + ``perturb``: run the in silico knockdown/knockout/overexpression 
 * >    experiment, optionally under a given sample context;
 * >  + ``perturb_export``: save the perturbation screening result as csv files;
 * >  + ``sgrid_prior``: read the prior knowledge regulatory network from a csv 
 * >    table(or a ``data.frame``);
 * >  + ``sgrid_save``: export all of the model tables and the ``.plas`` model script;
 * >  
 * >  and the binary serialization support of the fitted model:
 * >  ``writeBin(model, con = file("network.zip"))`` and 
 * >  ``readBin(file("network.zip"), what = "ssgrid_network")``.
*/
declare namespace ssGrid {
   /**
    * get the network topology of the fitted S-system model as a data.frame
    * 
    * 
     * @param network -
     * @return a dataframe with the columns: ``source,target,weight,g,h,interaction,
     *  prior_supported,prior_type,prior_confidence``
   */
   function network_edges(network: object): object;
   /**
    * run the in silico perturbation experiment on the fitted S-system network model
    * 
    * 
     * @param network the fitted network model, which is the output of the ``sfit`` api.
     * @param geneIds a character vector of the target gene id of the perturbation.
     * @param type the perturbation type: ``knockdown``(default), ``knockout`` or 
     *  ``overexpress``.
     * 
     * + default value Is ``'knockdown'``.
     * @param magnitude the perturbation strength: for the knockdown, ``0.8`` means that the target 
     *  gene loses 80% of its activity; for the overexpress, ``1`` means that an 
     *  extra 100% of the baseline expression will be added to the target gene.
     * 
     * + default value Is ``0.8``.
     * @param kicks the times that the perturbation will be applied, a periodic stimulation is 
     *  used to simulate a sustained treatment.
     * 
     * + default value Is ``1``.
     * @param start the time point that the first perturbation kick is applied.
     * 
     * + default value Is ``0.1``.
     * @param final_time the final time of the simulation, zero means the end of the fitted time axis.
     * 
     * + default value Is ``0``.
     * @param context the optional sample context(the ODE initial state), which should be 
     *  created via the ``sgrid_context`` api.
     * 
     * + default value Is ``null``.
     * @param export the optional output directory for save the trajectory files of each 
     *  perturbation design; nothing will be written if this parameter is not 
     *  provided.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a [ScreeningResult](cref:T:SMRUCC.genomics.Analysis.SGrid.Perturbation.ScreeningResult) object which could be converted to a 
     *  ``data.frame`` via ``as.data.frame``, or a R# error message object.
   */
   function perturb(network: object, geneIds: any, type?: string, magnitude?: number, kicks?: object, start?: number, final_time?: number, context?: object, export?: string, env?: object): object;
   /**
    * save the virtual perturbation screening result as csv files
    * 
    * > this function writes the ``control.csv`` and the ``perturb_records.csv``; 
    * >  the trajectory of each perturbation design is not kept in the screening 
    * >  result object, so please use the ``export`` parameter of the ``perturb`` 
    * >  api if the trajectory files are required.
    * 
     * @param screening the output of the ``perturb`` api.
     * @param dir the output directory.
     * @return the number of the files that has been written.
   */
   function perturb_export(screening: object, dir: string): object;
   /**
    * infer the S-system gene regulatory network from the gene expression matrix
    * 
    * 
     * @param x the gene expression matrix(rows are genes and columns are the time points), 
     *  this matrix object could be loaded from a csv file via ``geneExpression::load.expr``.
     * @param opts the fitting configuration object, see also: [ssGrid.sfitOptions()](cref:M:biosystem.ssGrid.sfitOptions(System.String,System.Double,System.Double,System.Int32,System.Int32,System.Double,System.Double,System.Int32,System.Double,System.Double,System.Double,SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork,System.Int32,System.Int32,System.Int32,System.Boolean,System.Boolean,System.Boolean,System.Double,System.Double,System.Int32)).
     * 
     * + default value Is ``null``.
     * @param prior the prior knowledge regulatory network, which could be a 
     *  [PriorNetwork](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork) object(from ``bnlearn::as.prior_net`` or ``sgrid_prior``) 
     *  or a csv file path. This prior is used as a **soft** constraint: an interaction 
     *  that is not supported by the prior will be shrunk, but a new regulation 
     *  relation could still be discovered from the expression data.
     * 
     * + default value Is ``null``.
     * @param time the time points of the matrix columns, a numeric vector.
     * 
     * + default value Is ``null``.
     * @param env the R# runtime environment object.
     * 
     * + default value Is ``null``.
     * @return a [SSystemNetwork](cref:T:SMRUCC.genomics.Analysis.SGrid.GRN.SSystemNetwork) object that contains the fitted network 
     *  topology and the kinetic parameters; a R# error message object will be 
     *  returned if the input data is not valid.
   */
   function sfit(x: object, opts?: object, prior?: any, time?: any, env?: object): object;
   /**
    * create the fitting configuration object of the S-system network inference
    * 
    * 
     * @param normalize the normalization method of the gene expression matrix, could be 
     *  ``log2``(default), ``none``, ``rowzscore`` or ``quantile``.
     * 
     * + default value Is ``'log2'``.
     * @param pseudo the pseudo count that used by the log2 normalization.
     * 
     * + default value Is ``1``.
     * @param sparsity the sparsity threshold of the net elasticity, the interaction with a smaller 
     *  ``|g - h|`` than this threshold will not be kept in the network topology.
     * 
     * + default value Is ``0.2``.
     * @param max_edges the max number of the regulators that could be kept for each target gene.
     * 
     * + default value Is ``3``.
     * @param max_regulators the max number of the candidate regulators for each target gene, the prior 
     *  supported regulators are always included before the correlation based 
     *  preselection; zero means that all of the genes will be used as candidate.
     * 
     * + default value Is ``0``.
     * @param l1 the L1 penalty weight of the kinetic orders.
     * 
     * + default value Is ``0.01``.
     * @param stability the penalty weight of a positive self loop.
     * 
     * + default value Is ``0.2``.
     * @param smooth the moving average window that used in the numerical differentiation, 
     *  it is required by the noisy pseudotime data; zero means no smoothing.
     * 
     * + default value Is ``5``.
     * @param prior_penalty the soft penalty weight of the interactions that are not supported by the prior.
     * 
     * + default value Is ``0.2``.
     * @param prior_sign_penalty the penalty weight of the direction conflict against the prior.
     * 
     * + default value Is ``0.2``.
     * @param prior_rank_boost the ranking boost of the prior supported edges in the sparsification.
     * 
     * + default value Is ``1``.
     * @param prior the prior knowledge network object.
     * 
     * + default value Is ``null``.
     * @param de_pop the population size of the differential evolution solver.
     * 
     * + default value Is ``40``.
     * @param de_iters the max iteration number of the differential evolution solver.
     * 
     * + default value Is ``1200``.
     * @param seed the random seed.
     * 
     * + default value Is ``1234567``.
     * @param parallel run the per gene fitting in parallel or not?
     * 
     * + default value Is ``true``.
     * @param verbose print the fitting progress or not?
     * 
     * + default value Is ``false``.
     * @param refine refine the kinetic parameters by the trajectory fitting after the 
     *  decoupled slope fitting?
     * 
     * + default value Is ``false``.
     * @param final_time the final time of the simulation, zero means the end of the time axis.
     * 
     * + default value Is ``0``.
     * @param precise the integration step of the simulation.
     * 
     * + default value Is ``0.1``.
     * @param steps the number of the simulation sample points, zero means auto.
     * 
     * + default value Is ``0``.
     * @return a [FittingOptions](cref:T:SMRUCC.genomics.Analysis.SGrid.GRN.FittingOptions) object
   */
   function sfit_options(normalize?: string, pseudo?: number, sparsity?: number, max_edges?: object, max_regulators?: object, l1?: number, stability?: number, smooth?: object, prior_penalty?: number, prior_sign_penalty?: number, prior_rank_boost?: number, prior?: object, de_pop?: object, de_iters?: object, seed?: object, parallel?: boolean, verbose?: boolean, refine?: boolean, final_time?: number, precise?: number, steps?: object): object;
   /**
    * get the coefficient matrix of the fitted S-system model
    * 
    * 
     * @param network -
     * @param type the matrix type: ``net``(default, the net elasticity ``g - h``), 
     *  ``g``(the kinetic order of the synthesis term) or 
     *  ``h``(the kinetic order of the degradation term).
     * 
     * + default value Is ``'net'``.
     * @param original use the original gene symbol instead of the sanitized gene id? default is TRUE.
     * 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
     * @return a wide dataframe: the rows are the target genes and the columns are the 
     *  regulators. the matrix element is the fitted coefficient.
   */
   function sgrid_coefficients(network: object, type?: string, original?: boolean, env?: object): object;
   /**
    * map a pseudobulk context(the raw expression counts) onto the model state space
    * 
    * > the model state is not the raw counts, but ``log2(max(counts,0)+pseudo) + delta``, 
    * >  the per gene offset ``delta`` was generated in the model fitting step. So the 
    * >  pseudobulk context data must be transformed by this function before it can be 
    * >  used as the ODE initial state.
    * 
     * @param network -
     * @param x the pseudobulk context data, could be a csv file path, a data.frame(the 
     *  columns are ``gene`` and ``mean_counts``), or a named numeric vector/list 
     *  (the names are the gene symbols, the values are the raw counts).
     * @param missing the value that used for the genes which is missing in the given context: 
     *  ``baseline``(default) means the minimal value of that gene in the training 
     *  matrix, ``zero`` means the zero expression.
     * 
     * + default value Is ``'baseline'``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a named list of the model state space value, which can be used as the 
     *  ``context`` parameter of the ``perturb`` and ``sgrid_simulate`` api.
   */
   function sgrid_context(network: object, x: any, missing?: string, env?: object): object;
   /**
    * get the fitted kinetic parameters of each gene as a data.frame
    * 
    * 
     * @param network -
     * @return a dataframe with the columns: ``gene,original,alpha,beta,rmse,r2,converged``
   */
   function sgrid_parameters(network: object): object;
   /**
    * read the prior knowledge regulatory network from a csv table or a data.frame
    * 
    * 
     * @param data a csv file path, or a data.frame object which contains the columns of 
     *  ``tf,target,type,confidence,evidence``(the column name is case insensitive, 
     *  and the ``confidence`` column could be a numeric score or the 
     *  ``High/Medium/Low`` label).
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a [PriorNetwork](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork) object, or a R# error message object.
   */
   function sgrid_prior(data: any, env?: object): object;
   /**
    * get the R square of the slope fitting of each gene
    * 
    * 
     * @param network -
     * @return a numeric vector of the R square value of each gene, the order of the 
     *  values is the same as the row order of ``sgrid_parameters(network)$gene``
   */
   function sgrid_r2(network: object): number;
   /**
    * get the fitting report of the S-system network model
    * 
    * 
     * @param network -
     * @return a character text
   */
   function sgrid_report(network: object): string;
   /**
    * export the model tables and the generated ``.plas`` model script into a directory
    * 
    * 
     * @param network -
     * @param dir the output directory, it will be created if not exists.
     * @return TRUE if the model data has been saved successfully.
   */
   function sgrid_save(network: object, dir: string): boolean;
   /**
    * integrate the fitted S-system model and return the trajectory
    * 
    * 
     * @param network -
     * @param final_time the final time of the integration, zero means the end of the fitted time axis.
     * 
     * + default value Is ``0``.
     * @param context the optional initial state of the ODE integration, this value should be in 
     *  the model state space(see also: [ssGrid.sgridContext()](cref:M:biosystem.ssGrid.sgridContext(SMRUCC.genomics.Analysis.SGrid.GRN.SSystemNetwork,System.Object,System.String,SMRUCC.Rsharp.Runtime.Environment))), the first 
     *  sample point of the fitted time series will be used if this parameter is 
     *  not provided.
     * 
     * + default value Is ``null``.
     * @param genes the optional gene list that returns in the trajectory table.
     * 
     * + default value Is ``null``.
     * @param env -
     * 
     * + default value Is ``null``.
     * @return a dataframe, the first column ``#Time`` is the time axis.
   */
   function sgrid_simulate(network: object, final_time?: number, context?: object, genes?: any, env?: object): object;
}
