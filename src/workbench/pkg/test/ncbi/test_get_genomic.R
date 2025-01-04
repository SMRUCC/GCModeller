require(GCModeller);

let data = read.csv("K:\test\ASSEMBLY_REPORTS\ANI_report_prokaryotes.txt",tsv=TRUE);

print(data);

ncbi_assembly_ftp(id = "GCA_000003135");