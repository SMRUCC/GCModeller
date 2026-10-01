# sigma_difference

Comparative genomics API module of the Karlin, Campbell & Mrazek (1998)
 ``Comparative DNA Analysis Across Diverse Genomes`` algorithm suite.

 This R# package exports:

 1. the pairwise / batch sliding window ``delta*`` difference calculations;
 2. the partition-based genome homogeneity measurements (``dnaA``-``gyrB`` ruler);
 3. the codon usage / CAI compilation;
 4. the full set of the 1998 paper's analysis modules:
    the ``rho*`` genome signature, the ``tau*`` tetranucleotide relative
    abundance, the r-scan word distribution statistics, the site-specific
    codon signature, the ``B(F|C)`` codon bias difference, the 2D threshold
    alien gene detection, the sliding window ``delta*`` island profiling and
    the ``(C-G)/(C+G)`` strand composition asymmetry.

+ [read.csv.site_delta](sigma_difference/read.csv.site_delta.1) Load one sliding window ``delta*`` profile from its csv document.
+ [compile.delta_query](sigma_difference/compile.delta_query.1) Compile a set of pairwise ``delta*`` profile csv files (all created from the
+ [sigma_diff.query](sigma_difference/sigma_diff.query.1) Compare one query genome against a directory of subject genomes: calculate
+ [genome.delta_star_profile](sigma_difference/genome.delta_star_profile.1) Sliding window ``delta*`` profile: the delta-difference between each local
+ [rendering_merge.delta_source](sigma_difference/rendering_merge.delta_source.1) Merge the exported partition-level ``delta*`` profile csv files with the
+ [compile.cai](sigma_difference/compile.cai.1) Compile the codon usage table of every species gene collection.
+ [Compile.CAI](sigma_difference/Compile.CAI.1) Compile the CAI w weight table of one reference gene collection and export
+ [partition_data.create](sigma_difference/partition_data.create.1) Create the chromosome partitioning data from the two-way BLAST best hit
+ [write.csv.genome_partition_data](sigma_difference/write.csv.genome_partition_data.1) Save the partitioning data collection as a csv document.
+ [read.csv.genome_partition_data](sigma_difference/read.csv.genome_partition_data.1) Load the partitioning data collection from its csv document.
+ [Read.Csv.Chromsome_Partitioning](sigma_difference/Read.Csv.Chromsome_Partitioning.1) Load the chromosome partitioning entries from their csv document.
+ [Partition.Similarity.Calculates](sigma_difference/Partition.Similarity.Calculates.1) Calculate the pairwise ``delta*`` homogeneity matrix between all of the
+ [Partitions.Creates](sigma_difference/Partitions.Creates.1) Create the partition data from a raw data frame: the start / stop columns
+ [measure_homogeneity](sigma_difference/measure_homogeneity.1) Measure the homogeneity property of the genome partition segments against
+ [seq.dist](sigma_difference/seq.dist.1) Create a delta* distance matrix for a given sequence collection.
+ [delta_star](sigma_difference/delta_star.1) The ``delta*`` measure of difference between two sequences f and g (from
+ [dnaA_gyrB](sigma_difference/dnaA_gyrB.1) Using the DNA segment between the ``dnaA`` and ``gyrB`` genes as the reference
+ [genome.signature_profile](sigma_difference/genome.signature_profile.1) The ``rho*`` genome signature profile: the double-strand symmetrized
+ [tau_star](sigma_difference/tau_star.1) The symmetrized ``tau*`` generalized odds ratio of one tetranucleotide:
+ [tau_star.profile](sigma_difference/tau_star.profile.1) The ``tau*`` tetranucleotide relative abundance profile of all 256
+ [tau_star.rare_frequent](sigma_difference/tau_star.rare_frequent.1) List the rare and the frequent tetranucleotides of the ``tau*`` profile
+ [word.count](sigma_difference/word.count.1) Count the absolute occurrences of one oligonucleotide word in the genome
+ [word.locate](sigma_difference/word.locate.1) Locate all occurrences of one oligonucleotide word in the genome sequence
+ [word.rscan](sigma_difference/word.rscan.1) Run the r-scan significance test on the spatial distribution of one
+ [codon.signature_profile](sigma_difference/codon.signature_profile.1) Build the site-specific codon signature profile of a gene collection:
+ [codon.usage_profile](sigma_difference/codon.usage_profile.1) Build the codon usage frequency profile c(x, y, z) of a gene collection
+ [codon.bias_B.FC](sigma_difference/codon.bias_B.FC.1) The ``B(F|C)`` codon usage bias difference between two gene classes (the
+ [codon.bias_B.gC](sigma_difference/codon.bias_B.gC.1) The ``B(g|C)`` codon usage bias difference of one single gene g against the
+ [alien.genes.rp_reference](sigma_difference/alien.genes.rp_reference.1) Build the codon usage profile of the ribosomal protein gene set (the
+ [alien.genes.scan](sigma_difference/alien.genes.scan.1) Identify the alien (laterally transferred) genes with the 2D threshold
+ [genome.delta_star_islands](sigma_difference/genome.delta_star_islands.1) The sliding window ``delta*`` profile of the genome against its own global
+ [genome.gc_skew](sigma_difference/genome.gc_skew.1) The ``(C - G) / (C + G)`` sliding window profile of the genome strand
+ [replication_origin.predict](sigma_difference/replication_origin.predict.1) Locate the candidate replication origin (oriC) and terminus positions from
