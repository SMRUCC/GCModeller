require(GCModeller);

imports "BioCyc" from "annotationKit";
imports "compiler" from "vcellkit";

let repo = open.biocyc("F:\\ecoli\\29.0");
let vcell = compile_biocyc(repo, logfile = here("gcc.log"));

vcell 
|> xml
|> writeLines(con = here("ecoli.xml"))
;