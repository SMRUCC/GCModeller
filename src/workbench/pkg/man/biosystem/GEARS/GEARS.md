# GEARS

GEARS: the graph neural network based in silico perturbation prediction toolkit
> This R# package module provides the toolkit for train a GEARS model(Gene 
>  Expression Additive Response Simulator) from the Perturb-seq experiment data, 
>  the trained model can be used for predict the gene expression response of the 
>  in silico gene perturbation(the knockout/overexpression/knockdown 
>  experiment):
>  
>  + ``new``: create a new GEARS model from the gene expression matrix, the prior 
>    regulatory network and the model configuration;
>  + ``training_set``: set the Perturb-seq training sample set of the GEARS model;
>  + ``train``: train the GEARS model with the given training sample set.
>  
>  the trained @``T:SMRUCC.genomics.Analysis.GEARS.GEARS`` model object implements the 
>  @``T:SMRUCC.genomics.Analysis.BNLearn.InsilicoPerturbationExperiment`` interface, so that it can be used 
>  by the ``knockouts``, ``overexpress`` and ``knockdown`` api of the ``bnlearn`` 
>  package module, and the perturbation result can be exported via the 
>  ``make_exports`` api.

+ [new](GEARS/new.1) create a new GEARS model
+ [training_set](GEARS/training_set.1) Set the training sample set
+ [train](GEARS/train.1) Train the GEARS model with the given training sample set
