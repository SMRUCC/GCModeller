require(GCModeller);

imports "proteinKit" from "seqtoolkit";
imports "metaTraits" from "annotationKit";

let svm = load.trait_models(repo = "G:\cell-render\data\phypat+PGL");
let pfam = read.pfam_string(here("pfam_annotation.csv"));
let test = svm |> make_predicts(pfam) |> phenotype_result(models = svm);

test 
|> jsonlite::toJSON()
|> writeLines(con = here( "test_trait_predicts.json"));

test = as.data.frame(test);

print(test, max.print = 6);

write.csv(test , file = here("test_trait_predicts.csv"));