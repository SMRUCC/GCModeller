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
>  [InsilicoPerturbationExperiment](cref:T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment) interface,
>  so the ``knockouts``, ``knockdown`` and ``overexpress`` api of the ``bnlearn`` package
>  can be applied to it directly for in silico perturbation experiments. The generic
>  ``writeBin`` handler of this module persists a model into a zip based binary stream,
>  and ``readBin.modular_bayesian`` restores it back to a fully usable model object.

+ [gps_config](GPS/gps_config.1) Create a GPS model hyper-parameter configuration object.
+ [build_gps](GPS/build_gps.1) Build a GPS (GNN + Transformer) gene regulatory network model.
+ [train](GPS/train.1) Train the GPS model and return it together with the training diagnostics.
