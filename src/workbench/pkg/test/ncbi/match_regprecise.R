require(GCModeller);

imports "regprecise" from "TRNtoolkit";
imports "NCBI" from "annotationKit";

let genbank = NCBI::genbank_assemblyDb("D:\datapool\assembly_summary_genbank.txt");
let regprecise = list.files("F:\ecoli\regprecise", pattern = "*.xml", recursive=FALSE)
|> tqdm()
|> sapply(filepath -> read.regulome(filepath))
;

for(let genome in tqdm(regprecise)) {
    genbank |> match_taxonomy(genome);
}

