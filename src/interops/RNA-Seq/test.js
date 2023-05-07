require("GCModeller");

// load the fastq module from rna-seq package 
// inside the GCModeller
import {FastQ} from "rnaseq";

// do short reads assembling via SCS algorithm
var assem = FastQ.assemble([
	"AACAAATGAGACGCTGTGCAATTGCTGA",
	"AACAAATGAGACGCTGTGCAATTGCTGA",
	"CAAATGAGACGCTGTGCAATTGCTGAGT",
	"GAAATGATACGCTGTGCAATTGCTGAGA",
	"ATGAGACGCTGTGCAATTGCTGAGTACC",
	"CTGTGCAATTGCTGAGTACCGTAGGTAG",
	"CTGTGCAATTGCTGAGTACCGTAGGTAG"
])
// view the short reads assemble result
console.table(assem)