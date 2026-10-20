# linear

The linear (elastic net) gene regulatory network builder and the in silico
 perturbation toolkit that runs on top of the fitted linear models.
> This package provides two groups of the R# api:
>  
>  + ``elastic_net`` / ``coef``: fit a ``regulator -> target`` linear regulatory
>    network from a gene expression matrix, and export the coefficient matrix;
>  + ``elastic_model`` / ``set_baseline`` / ``intervene`` / ``profiles_matrix``:
>    build an in silico perturbation engine from the fitted network, condition it
>    on a given cell context, and run the RNAi style knockdown experiments.
>  
>  The perturbation engine implements the `InsilicoPerturbationExperiment`
>  interface, so the ``knockouts`` / ``overexpress`` / ``knockdown`` api of the
>  ``bnlearn`` package can be used on the object that is created by
>  ``elastic_model`` directly.

+ [elastic_net](linear/elastic_net.1) Fit a linear (elastic net) gene regulatory network from the expression matrix.
+ [coef](linear/coef.1) Export the coefficient matrix of the fitted network.
+ [elastic_model](linear/elastic_model.1) Build the in silico perturbation engine from a fitted linear GRN model.
+ [gene_names](linear/gene_names.1) Get the gene ids of the fitted network, in the row order of the expression matrix.
+ [wildtype_means](linear/wildtype_means.1) Get the currently effective wildtype (baseline) expression vector of the model.
+ [library_sizes](linear/library_sizes.1) Get the implied library size of every training sample of the fitted network.
+ [set_baseline](linear/set_baseline.1) Set the wildtype baseline (the cell context) of the perturbation engine.
+ [intervene](linear/intervene.1) Run one in silico perturbation experiment and return the raw result object.
+ [profiles_matrix](linear/profiles_matrix.1) Run the perturbation experiment for a set of genes and collect the profiles into a matrix.
+ [export_profiles](linear/export_profiles.1) Run the perturbation experiment for a set of genes and save the profiles as a csv file.
