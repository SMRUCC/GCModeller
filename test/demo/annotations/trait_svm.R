require(GCModeller);

imports "metaTraits" from "annotationKit";

let traits = metaTraits::load.trait_annotations(c("K:\cultral_medium\ncbi_species_summary_filtered.tsv",
"K:\cultral_medium\gtdb_species_summary_filtered.tsv"));
let pfam = metaTraits::read.pfam_proteomes("K:\cultral_medium\pfam");
let trait_info = load.phenotype_traits("G:\GCModeller\data\phenotype_traits_and_types.csv");
let svm = trait_info 
|>  phenotype.problem(traits, pfam, pfam.embedding(pfam) ) 
|> train.phenotype_models()
;

svm |> save.trait_models(dir = "Z:/metaTraits/");
svm = load.trait_models(repo = "Z:/metaTraits/");

let test = svm |> make_predicts(pfam) |> phenotype_result(models = svm);

test 
|> jsonlite::toJSON()
|> writeLines(con = "Z:/metaTraits/test_result.json");