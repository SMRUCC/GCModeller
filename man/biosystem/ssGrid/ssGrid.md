# ssGrid

The S-system based gene regulatory network inference toolkit
 (基于 S-system 动力学系统的基因表达调控网络拟合工具)
> This R# package module exposes the ``SGrid`` pipeline as a set of R functions:
>  
>  + ``sfit_options``: create the fitting configuration object([FittingOptions](cref:T:SMRUCC.genomics.Analysis.SGrid.GRN.FittingOptions));
>  + ``sfit``: infer the S-system gene regulatory network from a gene expression 
>    matrix(rows are genes and the columns are the time points), the optional 
>    ``prior`` knowledge network([PriorNetwork](cref:T:SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork)) is used as a soft 
>    constraint of the network structure;
>  + ``network_edges``, ``sgrid_parameters``, ``sgrid_coefficients`` and ``sgrid_r2``:
>    query the fitted model, these functions return ``data.frame`` objects;
>  + ``sgrid_simulate``: integrate the fitted S-system model and return the 
>    trajectory as a ``data.frame``;
>  + ``sgrid_context``: map a pseudobulk context(the raw counts) onto the model 
>    state space, so that it can be used as the ODE initial state;
>  + ``perturb``: run the in silico knockdown/knockout/overexpression 
>    experiment, optionally under a given sample context;
>  + ``perturb_export``: save the perturbation screening result as csv files;
>  + ``sgrid_prior``: read the prior knowledge regulatory network from a csv 
>    table(or a ``data.frame``);
>  + ``sgrid_save``: export all of the model tables and the ``.plas`` model script;
>  
>  and the binary serialization support of the fitted model:
>  ``writeBin(model, con = file("network.zip"))`` and 
>  ``readBin(file("network.zip"), what = "ssgrid_network")``.

+ [sfit_options](ssGrid/sfit_options.1) create the fitting configuration object of the S-system network inference
+ [sfit](ssGrid/sfit.1) infer the S-system gene regulatory network from the gene expression matrix
+ [sgrid_prior](ssGrid/sgrid_prior.1) read the prior knowledge regulatory network from a csv table or a data.frame
+ [network_edges](ssGrid/network_edges.1) get the network topology of the fitted S-system model as a data.frame
+ [sgrid_parameters](ssGrid/sgrid_parameters.1) get the fitted kinetic parameters of each gene as a data.frame
+ [sgrid_coefficients](ssGrid/sgrid_coefficients.1) get the coefficient matrix of the fitted S-system model
+ [sgrid_r2](ssGrid/sgrid_r2.1) get the R square of the slope fitting of each gene
+ [sgrid_report](ssGrid/sgrid_report.1) get the fitting report of the S-system network model
+ [sgrid_save](ssGrid/sgrid_save.1) export the model tables and the generated ``.plas`` model script into a directory
+ [sgrid_simulate](ssGrid/sgrid_simulate.1) integrate the fitted S-system model and return the trajectory
+ [sgrid_context](ssGrid/sgrid_context.1) map a pseudobulk context(the raw expression counts) onto the model state space
+ [perturb](ssGrid/perturb.1) run the in silico perturbation experiment on the fitted S-system network model
+ [perturb_export](ssGrid/perturb_export.1) save the virtual perturbation screening result as csv files
