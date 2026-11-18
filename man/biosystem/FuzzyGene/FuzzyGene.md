# FuzzyGene

FuzzyGene: the neuro-fuzzy (Wilson/Takagi-Sugeno ANFIS) gene regulatory network toolkit.
> This R# package module wraps the ``FuzzyGene`` sub-system. Every target gene is described by
>  its own first-order Takagi-Sugeno fuzzy model whose inputs are 3-6 candidate regulators;
>  the rules of that model can be read as biological statements ("IF TF-A is High AND
>  TF-B is Low THEN gene-G is activated"), and the analytic partial derivatives of the model
>  give a signed, weighted regulatory edge table.
>  
> 
> 
>  
> 
> 
>  The workflow exposed by this module is:
>  
> 
> 
>  
> - ``fuzzygene_config``: create the hyper-parameter configuration object
>  ([FuzzyGeneConfig](cref:T:SMRUCC.genomics.Analysis.FuzzyGene.Config.FuzzyGeneConfig), registered as the
>  ``fuzzygene_config`` R# type);
> - ``build_fuzzygene``: build an untrained model from a gene expression matrix and an optional
>  prior regulatory network;
> - ``train``: fit every target gene (static route, or the lagged `x(t+1) = F(x(t))` route
>  when the matrix carries multiple time points);
> - ``fuzzygene_rules`` / ``fuzzygene_edges``: export the interpretable IF-THEN rules and the
>  signed regulatory edge table;
> - ``fuzzygene_trace_perturbation`` / ``fuzzygene_export_trace``: record the whole-genome
>  steady-state trajectory of one virtual perturbation (mutant and wildtype), either as two
>  matrices in memory or as CSV + ``SampleInfo`` sidecar files on disk;
> - ``fuzzygene_trace_convergence``: the solver diagnostics of that run (iteration vs
>  ``max_delta``), which tells whether the flat tail of a curve is a real steady state;
> - ``fuzzygene_rank_perturbations`` / ``fuzzygene_strongest_perturbation``: rank candidate
>  perturbations by predicted response strength;
> - ``fuzzygene_relaxation``: change the damping factor / iteration budget of the solver, which
>  is what you want when a genome scale model keeps oscillating instead of converging;
> 
> 
>  
> 
> 
>  A trained [FuzzyGeneModel](cref:T:SMRUCC.genomics.Analysis.FuzzyGene.FuzzyGeneModel) implements the
>  `InsilicoPerturbationExperiment` interface, so the ``knockouts``, ``knockdown`` and
>  ``overexpress`` api of the ``bnlearn`` package can be applied to it directly for in silico
>  perturbation experiments (partial knockdown is natively supported because the fuzzy
>  membership functions describe graded regulation). The generic ``writeBin`` handler of this
>  module persists a model into a zip based binary stream, and ``readBin.fuzzygene`` restores
>  it back to a fully usable model object.

+ [fuzzygene_config](FuzzyGene/fuzzygene_config.1) Create a FuzzyGene modelling configuration object.
+ [build_fuzzygene](FuzzyGene/build_fuzzygene.1) Build an untrained neuro-fuzzy gene regulatory network model.
+ [train](FuzzyGene/train.1) Train the neuro-fuzzy model: one Takagi-Sugeno network per target gene.
+ [fuzzygene_rules](FuzzyGene/fuzzygene_rules.1) Export the IF-THEN rules of every fitted target gene.
+ [fuzzygene_edges](FuzzyGene/fuzzygene_edges.1) Export the signed regulatory edge table of a trained model.
+ [fuzzygene_regulation_matrix](FuzzyGene/fuzzygene_regulation_matrix.1) Export the signed regulation weight matrix of a trained model.
+ [fuzzygene_confidence_matrix](FuzzyGene/fuzzygene_confidence_matrix.1) Export the confidence matrix of a trained model (same shape as the weight matrix).
+ [fuzzygene_context](FuzzyGene/fuzzygene_context.1) Inject a transcriptome context as the initial condition of the virtual perturbation.
+ [fuzzygene_reset_context](FuzzyGene/fuzzygene_reset_context.1) Drop the injected context and restore the training baseline.
+ [fuzzygene_model_config](FuzzyGene/fuzzygene_model_config.1) The configuration attached to a model (identical to ``config.json`` inside the package).
+ [fuzzygene_context_upstream](FuzzyGene/fuzzygene_context_upstream.1) Inject the result of an **upstream perturbation** as the control baseline (iterative perturbation).
+ [fuzzygene_report](FuzzyGene/fuzzygene_report.1) Human readable training report of a fitted model.
+ [save_fuzzygene](FuzzyGene/save_fuzzygene.1) Save a trained model into a ``*.fuzzygene.zip`` package.
+ [load_fuzzygene](FuzzyGene/load_fuzzygene.1) Load a model from a ``*.fuzzygene.zip`` package.
+ [fuzzygene_trace_perturbation](FuzzyGene/fuzzygene_trace_perturbation.1) Record the whole-genome steady-state trajectory of one virtual perturbation.
+ [fuzzygene_trace_convergence](FuzzyGene/fuzzygene_trace_convergence.1) Solver diagnostics of one virtual perturbation, without building the two large matrices.
+ [fuzzygene_export_trace](FuzzyGene/fuzzygene_export_trace.1) Export the two trajectory matrices of one virtual perturbation to a folder.
+ [fuzzygene_rank_perturbations](FuzzyGene/fuzzygene_rank_perturbations.1) Rank candidate perturbations by their predicted response strength.
+ [fuzzygene_strongest_perturbation](FuzzyGene/fuzzygene_strongest_perturbation.1) The single gene whose perturbation produces the strongest predicted response.
+ [fuzzygene_relaxation](FuzzyGene/fuzzygene_relaxation.1) Set the damping factor, the iteration budget and the tolerance used by the steady-state solver.
