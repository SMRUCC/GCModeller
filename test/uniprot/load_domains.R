require(GCModeller);

imports "uniprot" from "seqtoolkit";

let uniprot = open.uniprot("C:\Users\Administrator\Downloads\uniprotkb_taxonomy_id_9606_2026_03_04.xml");

for(let prot in uniprot) {
    print(as.data.frame(prot |> get_domain()));
    stop();
}