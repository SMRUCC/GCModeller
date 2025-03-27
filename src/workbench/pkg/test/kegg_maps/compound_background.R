require(GCModeller);

imports "background" from "gseakit";

metabolism.background(load_kegg_maps())
|> write.background(file = "Z:/kegg_compounds.xml")
;