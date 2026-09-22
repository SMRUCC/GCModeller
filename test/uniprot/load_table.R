require(GCModeller);

imports "uniprot" from "seqtoolkit";

let tablefile = "E:\biodeep\uniprotkb_taxonomy_id_9606_2024_12_12.tsv";
let table = read.proteinTable(tablefile);

