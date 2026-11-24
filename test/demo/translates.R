imports "bioseq.fasta" from "seqtoolkit.dll";

let genes as string = ?"--seq" || stop("no gene sequence fasta file input provided!");
let prots as string = `${dirname(genes)}/${basename(genes)}.prot.fasta`;
let rnaId = $"sRNA\d+";

genes
:> read.fasta
:> as.vector
:> which(fa -> !(as.object(fa)$Title like rnaId))
# BacterialArchaealAndPlantPlastidCode
:> translate(table = "BacterialArchaealAndPlantPlastidCode", checkNt = FALSE, bypassStop = TRUE)
:> write.fasta(file = prots, lineBreak = 60)
;