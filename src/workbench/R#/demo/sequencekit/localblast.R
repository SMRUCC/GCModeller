imports ["bioseq.blast", "bioseq.fasta"] from "seqtoolkit";

print(
	align.smith_waterman(
		query  = fasta("AAAATAAAAATTTTTTTTTTTTTTTTTTTTTTTAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", ["query"]),
		ref    = fasta("TTTTTTTTTTTTTTTTTCTTTTTTTTTTTTTTTTTTTTTTTTTAAAAAAAAAAAAAAAAAAAAA", ["reference"]),
		blosum = blosum("Blosum-62")
	)
)
;