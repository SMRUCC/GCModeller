# GPS

GPS: the GNN + Transformer based gene regulatory network modelling toolkit.
> This R# package module wraps the [GPSModel](cref:T:SMRUCC.genomics.Analysis.GPS.GPSModel) of the ``GPS`` sub-system.
>  The model fuses a local message passing branch (edge type aware aggregation over the
>  prior regulatory network) with a global multi head attention branch (Graphormer style
>  shortest path bias), encodes the obtained gene representations over time with a GRU
>  and finally decodes the expression of every single gene with its own linear readout.
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
> - ``gps_config``: create the hyper-parameter configuration object
>  ([GPSConfig](cref:T:SMRUCC.genomics.Analysis.GPS.Config.GPSConfig), registered as the ``gps_config`` R# type);
> - ``build_gps``: build a model object from a time series gene expression matrix and a
>  prior regulatory network object;
> - ``train``: run the masked reconstruction pre-training followed by the next-step
>  prediction fine-tuning, and attach the training diagnostics to the returned object.
> 
> 
>  
> 
> 
>  A trained [GPSModel](cref:T:SMRUCC.genomics.Analysis.GPS.GPSModel) implements the
>  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.BioSystem.InsilicoPerturbationExperiment) interface,
>  so the ``knockouts``, ``knockdown`` and ``overexpress`` api of the ``bnlearn`` package
>  can be applied to it directly for in silico perturbation experiments. The generic
>  ``writeBin`` handler of this module persists a model into a zip based binary stream,
>  and ``readBin.modular_bayesian`` restores it back to a fully usable model object.

+ [gps_config](GPS/gps_config.1) Create a GPS model hyper-parameter configuration object.
+ [build_gps](GPS/build_gps.1) Build a GPS (GNN + Transformer) gene regulatory network model.
+ [train](GPS/train.1) Train the GPS model and return it together with the training diagnostics.
+ [gene_names](GPS/gene_names.1) Get the gene ids of the trained GPS model, in the modelling order.
+ [gene_means](GPS/gene_means.1) Get the per gene mean of the preprocessed expression data (log-CPM scale).
+ [gene_sds](GPS/gene_sds.1) Get the per gene standard deviation of the preprocessed expression data (log-CPM scale).
+ [library_sizes](GPS/library_sizes.1) Get the raw library size of every training sample (the denominator of the CPM normalization).
+ [gene_stats](GPS/gene_stats.1) Get the gene alignment and the de-normalization parameters of the model as a tuple list.
+ [set_context](GPS/set_context.1) Inject an external cell context into the model as the control baseline of the following
+ [reset_context](GPS/reset_context.1) Restore the control baseline of the model back to the training derived one.
+ [knockdown_residual](GPS/knockdown_residual.1) Get or set the fraction of the wild type expression that is kept by an in silico knockdown.
+ [perturbation_channel](GPS/perturbation_channel.1) Get or set whether the perturbation channel is enabled.
+ [perturbation_channel_gain](GPS/perturbation_channel_gain.1) Get or set the injection gain of the perturbation channel.
+ [perturbation_channel_coregulation](GPS/perturbation_channel_coregulation.1) Get or set the co-regulation fallback of the perturbation channel.
+ [graph_normalization](GPS/graph_normalization.1) The edge normalisation mode that the graph convolution actually uses.
+ [export_profiles](GPS/export_profiles.1) Write the mutant (and the control) expression profiles of a set of in silico perturbation
