imports "vcellkit.compiler" from "vcellkit.dll";
imports "annotation.genomics" from "seqtoolkit.dll";

let genome <- list(genome = ["K:\20200226\IGV_data\genome.gtf"]
	:> read.gtf
	:> as.genbank
);

