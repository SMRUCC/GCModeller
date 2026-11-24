imports "HMP_portal" from "metagenomics_kit";

setwd(!script$dir);


"buccal_mucosa/hmp_manifest_346304a343.tsv"
:> read.manifest
:> fetch(outputdir = "buccal_mucosa")
;