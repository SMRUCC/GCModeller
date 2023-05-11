require("GCModeller");
// load the fastq module from rna-seq package 
// inside the GCModeller
import {FastQ} from "rnaseq";

// do short reads assembling via SCS algorithm
var assem = FastQ.assemble([
	"AACAAATGAGACGCTGTGCAATTGCTGA",
	"AACAAATGAGACGCTGTGCAATTGCAAA",
	"CAAATGAGACGCTGTGCAATTGCTGAGT",
	"GCAAATGATACGCTGTGCAATTGCTAGA",
	"ATGAGACGCTGTGCAATTGCTGAGTACC",
	"CTGTGCAATTGCTGAGAACAAATGAGAC",
	"CTGTGCAATTGCTAGAAACAAATGAGAC"
])

// view the short reads assemble result
console.table(assem)

// Loading required package: GCModeller
// Loading required package: igraph
// Attaching package: 'igraph'
//
// The following object is masked from 'package:igraph':
//
//     eval, class
//
//
//
//   GCModeller: genomics CAD(Computer Assistant Design) Modeller System
//                                 author by: xie.guigang@gcmodeller.org
//
//                            (c) 2023 | SMRUCC genomics - GuiLin, China
//
//                                                                                                AssembleResult
// --------------------------------------------------------------------------------------------------------------
// <mode>                                                                                               <string>
// [1, ]  "CTGTGCAATTGCTGAGAACAAATGAGACGCTGTGCAATTGCAAATGATACGCTGTGCAATTGCTAGAAACAAATGAGACGCTGTGCAATTGCTGAGTACC"
// [2, ]  "...................................................................AACAAATGAGACGCTGTGCAATTGCTGA....."
// [3, ]  "................AACAAATGAGACGCTGTGCAATTGCAAA........................................................"
// [4, ]  ".....................................................................CAAATGAGACGCTGTGCAATTGCTGAGT..."
// [5, ]  ".......................................GCAAATGATACGCTGTGCAATTGCTAGA................................."
// [6, ]  "........................................................................ATGAGACGCTGTGCAATTGCTGAGTACC"
// [7, ]  "CTGTGCAATTGCTGAGAACAAATGAGAC........................................................................"
// [8, ]  "...................................................CTGTGCAATTGCTAGAAACAAATGAGAC....................."