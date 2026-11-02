# geneTree

XGBoost based gene regulatory network simulator and the in silico perturbation toolkit
> This package module is the R# front end of the ``GeneTree`` model: the ARACNE
>  inferred mutual information network is turned into a non linear dynamical system
>  by fitting one gradient boosting tree regressor per target gene
>  (`x_i = f_i(X_Reg(i))`), and the virtual perturbation experiment is then
>  performed by solving the fixed point of the fitted system with the damped
>  iteration `x_i^new = (1 - alpha) * x_i^old + alpha * f_i(X_Reg(i)^old)`.
>  
> 
> 
>  
>  
> 
> 
>  The typical workflow:
>  
> 
> 
>  
>  
> ```
>  imports ["geneTree", "bnlearn"] from "biosystem";
>  
>  let opts = geneTree::make_profile("fast", population = 100);
>  let grn = geneTree::new(aracne, expr, opts) |> fit_tree();
>  
>  # 相对训练时的野生型基线做虚拟敲除
>  let ko = grn |> knockouts(c("TP53", "MYC"));
>  
>  # 切换到指定的表达状态上下文之后再扰动（fix = TRUE 时上下文基因被钳制）
>  let state = grn |> geneTree::steady_state();
>  let perturbed = grn
>      |> geneTree::set_context(c("TP53"), c(0.0), fix = TRUE)
>      |> knockouts(c("EGFR"))
>  ;
>  
> ```
> 
>  
>  
> 
> 
>  The fitted [XGBoostGRN](cref:T:SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) object implements the
>  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment) interface,
>  so the generic ``knockouts`` / ``overexpress`` / ``knockdown`` / ``make_exports``
>  api of the ``bnlearn`` package module can be used directly on it.

+ [make_profile](geneTree/make_profile.1) Create the training/solving configuration profile of the GeneTree model.
+ [parallel_opts](geneTree/parallel_opts.1) Override the parallelism of a trained model (the perturbation solver) or of a
+ [discovery](geneTree/discovery.1) Enable the residual two-stage structure discovery on a GeneTree configuration.
+ [load_topology](geneTree/load_topology.1) Build the regulation graph model from the ARACNE inferred network.
+ [load_expression](geneTree/load_expression.1) Attach the gene expression matrix to the regulation graph, the created model is
+ [new](geneTree/new.1) Create the GeneTree model from the ARACNE network and the gene expression matrix.
+ [fit_tree](geneTree/fit_tree.1) Train the gradient boosting tree models of each target gene.
+ [write_report](geneTree/write_report.1) Export the training report as two readable table files.
+ [training_stats](geneTree/training_stats.1) Get the summary statistics of the model training.
+ [discovered_edges](geneTree/discovered_edges.1) Get the candidate edges that were examined by the residual two-stage structure discovery.
+ [report_table](geneTree/report_table.1) Get the per gene fitting quality table of the trained model.
+ [feature_importance](geneTree/feature_importance.1) Get the split importance of the regulators, this table is the data driven
+ [gene_stats](geneTree/gene_stats.1) Get the gene alignment and the wildtype baseline of the model.
+ [gene_names](geneTree/gene_names.1) Get the gene ids of the model.
+ [set_context](geneTree/set_context.1) Inject an expression state context into the model, the context becomes the
+ [solver_opts](geneTree/solver_opts.1) Override the fixed point solving options of a trained model at runtime, so that
+ [reset_context](geneTree/reset_context.1) Remove the expression state context, the model is restored back to the wildtype
+ [get_context](geneTree/get_context.1) Get the expression state context that is currently applied on the model.
+ [steady_state](geneTree/steady_state.1) Solve the control steady state of the network, i.e. the "attractor" of the fitted
+ [solver_history](geneTree/solver_history.1) Get the diagnostics of the last fixed point solving.
+ [perturb](geneTree/perturb.1) Set the expression value of a set of genes to the given values, i.e. run a
+ [perturb_combined](geneTree/perturb_combined.1) Run a combined in silico intervention on a set of genes at the same time.
