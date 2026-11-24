imports "annotation.workflow" from "seqtoolkit";

let forward = "K:\20200226\1025_vs_uniprot.txt" :> read.blast(type = "prot") :> blasthit.sbh;
let reverse = "K:\20200226\uniprot_vs_1025.txt" :> read.blast(type = "prot") :> blasthit.sbh;

using file as "K:\20200226\搜库结果\proteinMapping.csv" :> open.stream(type = "BBH") {
   file :> stream.flush(blasthit.bbh(forward, reverse));
}
