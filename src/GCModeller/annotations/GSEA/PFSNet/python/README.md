# PFSNet

> https://github.com/abha-b/PFSNet

Please cite the following papers if you use this code:

Abha Belorkar, Rajanikanth Vadigepalli, Limsoon Wong. SPSNet: Subpopulation-sensitive network-based analysis of heterogeneous gene expression data. BMC Systems Biology, 12(Suppl 2):28, April 2018

Kevin Lim, Limsoon Wong. Finding consistent disease subnetworks using PFSNet. Bioinformatics, 30(2):189--196, January 2014


```
ABOUT PFSNET V2

	As on: http://compbio.ddns.comp.nus.edu.sg:8080/pfsnet/
	
	Recent work in microarray data analysis have seen a paradigm shift from 
	identifying individual disease related genes [3] to identifying pathways 
	responsible for the disease [2]. Methods that study pathways are superior
	to those that study individual genes because they allow us to better 
	interpret the cause of disease. Moreover, some genes that have little 
	differential expression can also be identified.

	However, large pathways can be too generalized and can be missed by 
	methods that work on whole pathways. In contrast, by breaking pathways 
	down into smaller components (subnetworks), pathways that are responsible
	for a particular disease can be implicated [1].

	PFSNet [4] is a method to identify significant subnetworks relevant to a 
	particular phenotype. Our prediction method is tested to have high 
	agreement across independent mircroarray datasets of the same disease 
	phenotype and also shown to correlate with known biological findings.
	
DEPENDENCIES

	python 2.7.x, numpy, scipy, networkx

USAGE

	python pfsnet.py -c C_EXPR_MATRIX -t T_EXPR_MATRIX -p PATHWAY_FILE ...

	-c, --control=C_EXPR_FILE (compulsory)
		Absolute/relative path of the file containing expression matrix for 
		control group samples; tab separated text file; matrix dimensions:
		genes x samples (first column: gene ids/names); first line skipped
		as header.

	-t, --test=T_EXPR_FILE (compulsory)
		Absolute/relative path of the file containing expression matrix for
		test group samples; tab separated text file; matrix dimensions: 
		genes x samples (first column: gene ids/names); first line skipped
		as header.
		
	-p, --pathway=PATHWAY_FILE (compulsory)
		Tab separated txt file; first line skipped as header; each edge as a 
		row; 3 columns - pathway name<\t>gene_1<\t>gene_2.
	
	-h, --theta1=THETA_1 (optional; default=0.95)
		Value between 0 to 1; denotes the quantile threshold above which 
		patient gives full vote to a gene.
		
	-l, --theta2=THETA_2 (optional; default=0.85)
		Value between 0 to 1; denotes the quantile threshold below which 
		patient gives zero vote to a gene.
		
	-b, --beta=BETA (optional; default=0.5)
		Value between 0 to 1; denotes the minimum average vote for which the 
		gene is considered highly expressed.
		
	-n, --permutations=N_PERMUTATIONS (optional; default=1000)
		Denotes the number of points to be generated in the null distribution 
		for permutation test.	
		
EXAMPLE COMMAND

	python pfsnet.py -c sample_control.txt -t sample_test.txt -p pathways.txt
	
OUTPUT

	On completion, a new folder 'pfsnet_results' is created (in the directory 
	where the script is run). This folder contains two files:
	
	significant_subnetworks_control.csv
	significant_subnetworks_test.csv
	
	These files contain the subnetworks found significant (in control and 
	test group respectively), their corresponding p-values, and ids of
	the genes they contain.
		
REFERENCES

	[1] Soh, Donny, Difeng Dong, Yike Guo, and Limsoon Wong. 
	"Finding consistent disease subnetworks across microarray datasets." 
	BMC bioinformatics 12, no. Suppl 13 (2011): S15.
	
	[2] Subramanian, Aravind, Pablo Tamayo, Vamsi K. Mootha, et al. 
	"Gene set enrichment analysis: a knowledge-based approach for interpreting 
	genome-wide expression profiles." 
	Proceedings of the National Academy of Sciences of the United States of 
	America 102, no. 43 (2005): 15545-15550.
	
	[3] Tusher, Virginia Goss, Robert Tibshirani, and Gilbert Chu. 
	"Significance analysis of microarrays applied to the ionizing 
	radiation response." 
	Proceedings of the National Academy of Sciences 98, no. 9 (2001): 
	5116-5121.

PUBLICATION

	[4] Lim, Kevin, and Limsoon Wong. "Finding consistent disease subnetworks 
	using PFSNet." 
	Bioinformatics, 30(2):189--196, January 2014

AUTHOR

	Abha Belorkar (email: abhab@comp.nus.edu.sg)

```

