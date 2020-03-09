imports "bioseq.fasta" from "seqtoolkit.dll";

let data <- read.csv("K:\20200226\TRN\1025.csv");
let geneId <- data[, "db_xref"] :> as.character;
let desc <- data[, "Name"] :> as.character;
let seq <- data[, "Sequence"] :> as.character;
let VF <- data[, "VF"] :> as.character;
let EG <- data[, "EG"] :> as.character;

let headers = sprintf("%s [VF=%s EG=%s]|%s", geneId, VF, EG, desc);

print(headers);

sapply(1:length(seq), i -> fasta(seq[i], geneId[i]))
:> as.fasta
:> write.fasta(file = "K:\20200226\TRN\1025.fasta")
;

