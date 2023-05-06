import "package_utils" from "devkit";

setwd(@dir);
package_utils.attach("../")

import {"bioseq.patterns", "bioseq.fasta"} from "seqtoolkit";

var seq = read.fasta("")
var graph = as.seq_graph(seq, mol_type = "DNA")

console.log(graph)