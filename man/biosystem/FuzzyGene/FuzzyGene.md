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
+ [fuzzygene_context](FuzzyGene/fuzzygene_context.1) Inject a transcriptome context as the initial condition of the virtual perturbation.
+ [fuzzygene_reset_context](FuzzyGene/fuzzygene_reset_context.1) Drop the injected context and restore the training baseline.
+ [fuzzygene_report](FuzzyGene/fuzzygene_report.1) Human readable training report of a fitted model.
+ [save_fuzzygene](FuzzyGene/save_fuzzygene.1) Save a trained model into a ``*.fuzzygene.zip`` package.
+ [load_fuzzygene](FuzzyGene/load_fuzzygene.1) Load a model from a ``*.fuzzygene.zip`` package.
