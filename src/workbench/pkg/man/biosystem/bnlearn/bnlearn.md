# bnlearn

Bayesian network learning and the in silico gene perturbation toolkit
> This R# package module provides the toolkit for learn the gene regulatory 
>  bayesian network from the gene expression data, and then run the in silico 
>  gene perturbation experiment based on the learned network model:
>  
>  + ``prior_network`` and ``as.prior_net``: create the prior knowledge 
>    regulatory network(TF -> target gene) which is used as the whitelist of 
>    the network structure learning;
>  + ``bnlearn``: learn the network structure(MMHC algorithm with the whitelist 
>    prior) and the network parameters(Gaussian bayesian network MLE);
>  + ``knockouts``, ``overexpress`` and ``knockdown``: run the in silico gene 
>    perturbation experiment on the learned network model;
>  + ``make_exports``: export the perturbation experiment result as a set of the 
>    csv table files;
>  + ``save_model``: save the learned bayesian network model as the tsv table 
>    files.

+ [bnlearn](bnlearn/bnlearn.1) learn the gene regulatory bayesian network from the gene expression data
+ [read_module_assignment](bnlearn/read_module_assignment.1) read WGCNA module color assignment result table
+ [set_baseline](bnlearn/set_baseline.1) 
+ [modular_intervene](bnlearn/modular_intervene.1) 
+ [export_modular_response](bnlearn/export_modular_response.1) 
+ [as.prior_net](bnlearn/as.prior_net.1) build prior network object based on a given vector of the knowledge network edges data
+ [prior_network](bnlearn/prior_network.1) create prior knowledge netwoek edges from the given vector data
+ [knockouts](bnlearn/knockouts.1) run the in silico gene knockout experiment on the given network model
+ [overexpress](bnlearn/overexpress.1) run the in silico gene overexpression experiment on the given network model
+ [knockdown](bnlearn/knockdown.1) run the in silico gene knockdown experiment on the given network model
+ [make_exports](bnlearn/make_exports.1) export the virtual permutation result as csv table files
+ [save_model](bnlearn/save_model.1) save bnlearn model
