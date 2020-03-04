imports "bioseq.fasta" from "seqtoolkit.dll";

let genes as string = "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\gene.fasta";
let prots as string = `${dirname(genes)}/${basename(genes)}.prot.fasta`;
let rnaId = $"sRNA\d+";

genes
:> read.fasta
:> as.vector
:> which(fa -> !(as.object(fa)$Title like rnaId))
:> translate(table = "BacterialArchaealAndPlantPlastidCode", checkNt = FALSE, bypassStop = TRUE)
:> write.fasta(file = prots, lineBreak = 60)
;