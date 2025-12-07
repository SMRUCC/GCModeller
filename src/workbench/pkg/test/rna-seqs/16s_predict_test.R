require(GCModeller);

imports "OTU_table" from "metagenomics_kit";
imports "microbiome" from "metagenomics_kit";

let otu_table = read.OTUtable("M:\project\20250818-ML\20251201\rawdata\发送数据_20251114\微生物数据\数据2_OTU分类表已抽平20251106\OTU_Taxon_Depth_otu.full .xls", OTUTaxonAnalysis=TRUE);
let PICRUSt = read_PICRUSt(file(J:\ko_13_5_precalculated.PICRUSt), open = "open");

PICRUSt |> predict_metagenomes(otu_table);
