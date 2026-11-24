require(GCModeller);

imports "background" from "gseakit";

let kegg = metabolism.background(load_kegg_maps());
let cid = NULL;

for(let id in ["00250" # MAlanine, aspartate and glutamate metabolism
"00260" # M NGlycine, serine and threonine metabolism
"00270" # M NCysteine and methionine metabolism
"00280" # M NValine, leucine and isoleucine degradation
"00290" # MValine, leucine and isoleucine biosynthesis
"00300" # M TLysine biosynthesis
"00310" # M NLysine degradation
"00220" # M NArginine biosynthesis
"00330" # M NArginine and proline metabolism
"00340" # M NHistidine metabolism
"00350" # M NTyrosine metabolism
"00360" # MPhenylalanine metabolism
"00380" # M NTryptophan metabolism
"00400" # M NPhenylalanine, tyrosine and tryptophan biosynthesis
]) {
    cid = rbind(cid, kegg |> clusterInfo("map" & id));
}

kegg
|> write.background(file = "Z:/kegg_compounds.xml")
;

kegg = as.data.frame(kegg, id_class = TRUE);
kegg[,"term"] = gsub(kegg$term," - Reference pathway","");

write.csv(kegg,file = "Z:/compound_pathway.csv");


print(cid);

write.csv(cid, file = "Z:/kegg_cid.csv");