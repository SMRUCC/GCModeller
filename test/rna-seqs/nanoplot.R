require(GCModeller);

imports "FastQ" from "rnaseq";
imports "QC" from "rnaseq";

let reads = read.fastq("U:\metagenomics_LLMs\test_output\processed_reads\barcode23-1\filtered_fq\nonhost.fq");
let result = QC::nano_plot(reads);

JSON::json_encode(result)
|> writeLines(con = relative_work("nano_plot.json"))
;