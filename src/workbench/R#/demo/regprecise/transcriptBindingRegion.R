imports ["bioseq.fasta", "annotation.genomics"] from "seqtoolkit.dll";

let genes = ["K:\20200226\IGV_data\genome.gtf"]
:> read.gtf
:> genome.genes
;

let region = genes :> upstream(length = 250);
let geneId = genes :> sapply(gene -> as.object(gene)$Synonym);
let nt = ["K:\20200226\IGV_data\genome.fasta"] :> read.seq;
let fasta = allocate(length(geneId));

for(i in 1:length(geneId)) {
	fasta[i] <- nt :> cut_seq.linear(loci = region[i]);
	as.object(fasta[i])$Headers = geneId;
}

fasta :> write.fasta(file = "K:\20200226\TRN\genomics\nt.fasta");