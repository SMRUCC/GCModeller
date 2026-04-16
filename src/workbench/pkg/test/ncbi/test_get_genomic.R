require(GCModeller);

imports "NCBI" from "annotationKit";

let data = NCBI::genome_assembly_index("K:\test\ASSEMBLY_REPORTS\assembly_summary_genbank.txt");

for(let ref in data) {
    ncbi_assembly_ftp(ref, "Z:/test_ftp");
    stop();
}
