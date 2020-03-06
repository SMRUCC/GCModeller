imports ["bioseq.fasta", "annotation.genomics"] from "seqtoolkit.dll";

let genes = ["K:\20200226\IGV_data\genome.gtf"]
:> read.gtf
:> genome.genes
;

let region = genes :> upstream(length = 250);
let geneId = genes :> sapply(gene -> as.object(gene)$Synonym);
let nt = ["K:\20200226\IGV_data\genome.fasta"] :> read.seq;
let region.fasta = allocate(length(geneId));

for(i in 1:length(geneId)) {
	region.fasta[i] <- nt :> cut_seq.linear(loci = region[i]);
	as.object(region.fasta[i])$Headers = geneId;
}

region.fasta :> write.fasta(file = "K:\20200226\TRN\genomics\nt.fasta");