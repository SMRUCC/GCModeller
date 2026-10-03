require(GCModeller);

imports "metaTraits" from "metagenomics_kit";

let traits = metaTraits::load.trait_annotations(c("C:\Users\Administrator\Downloads\ncbi_species_summary_filtered.tsv",
"C:\Users\Administrator\Downloads\gtdb_species_summary_filtered.tsv"));
let pfam = metaTraits::read.pfam_proteomes("C:\Users\Administrator\Downloads\pfam");
let trait_info = load.phenotype_traits("G:\GCModeller\src\GCModeller\analysis\Metagenome\MetaFunction\metaTraits\phenotype_traits_and_types.csv");
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