# ARACNE

ARACNE (Algorithm for the Reconstruction of Accurate Cellular Networks) gene
 regulatory network inference.
> The pipeline is: ``mi -> threshold -> DPI -> direction``, and the transcription
>  factor (TF) prior knowledge is used in four places:
>  
>  + the computation space is reduced from ``N x N`` to ``TF x N``;
>  + the ``TF -> target`` direction is assigned from the TF list;
>  + the curated ``TF -> target`` evidence relaxes the MI threshold and protects
>    the supported edges from the top-K pruning;
>  + the curated evidence also protects the supported edges from the DPI filtering,
>    and the protection strength is graded by the prior confidence.
>  
>  The api groups:
>  
>  + ``read_tf`` / ``read_prior``: load the prior knowledge (the TF annotation table and
>    the curated ``TF -> target`` table) directly from the data files;
>  + ``aracne``: run the whole inference from an expression matrix;
>  + ``aracne_options`` / ``infer``: build the options object, then run the inference
>    in two steps (useful when the same options are reused for several matrices);
>  + ``edges`` / ``regulons`` / ``summary`` / ``threshold`` / ``dpi_stats`` /
>    ``mi_matrix`` / ``as_graph`` / ``write_edges``: inspect and export the network.

+ [aracne_options](ARACNE/aracne_options.1) Create an [AracneOptions](cref:T:SMRUCC.genomics.Model.Network.Regulons.ARACNE.AracneOptions) object from the scalar arguments.
+ [aracne](ARACNE/aracne.1) Infer the ARACNE gene regulatory network from the expression matrix.
+ [infer](ARACNE/infer.1) Run the inference with a pre-built [AracneOptions](cref:T:SMRUCC.genomics.Model.Network.Regulons.ARACNE.AracneOptions) object.
+ [edges](ARACNE/edges.1) Get the regulation edges of the inferred network as a data frame.
+ [regulons](ARACNE/regulons.1) Get the regulons (the target gene count of each regulator) of the network.
+ [gene_names](ARACNE/gene_names.1) Get the gene ids of the network, in the row order of the internal expression vectors.
+ [tf_ids](ARACNE/tf_ids.1) Get the transcription factors that are matched by the network (the regulators).
+ [mi_matrix](ARACNE/mi_matrix.1) Get the mutual information matrix of the inferred network.
+ [coef](ARACNE/coef.1) Get the mutual information matrix of the inferred network (the alias of ``mi_matrix``).
+ [summary](ARACNE/summary.1) Get the inference statistics of the network.
+ [threshold](ARACNE/threshold.1) Get the significance threshold (``tau``) information of the network.
+ [dpi_stats](ARACNE/dpi_stats.1) Get the DPI (data processing inequality) filtering statistics of the network.
+ [as_graph](ARACNE/as_graph.1) Convert the inferred network into a graph model, so that it can be exported as a
+ [write_edges](ARACNE/write_edges.1) Export the regulation edges of the network into a csv file.
+ [read_tf](ARACNE/read_tf.1) Load the transcription factor gene symbols from a TF annotation table
+ [read_prior](ARACNE/read_prior.1) Load the curated ``TF -> target`` prior network from a csv table.
