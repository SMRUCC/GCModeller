imports ["annotation.workflow", "annotation.terms"] from "seqtoolkit.dll";

let forward = ["K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\seq_vs_uniprot.sbh.Csv"] 
:> open.stream(ioRead = TRUE)
:> besthit.filter(evalue = 0.00001)
;

let reverse = ["K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\uniprot_vs_seq.sbh.Csv"] 
:> open.stream(ioRead = TRUE)
:> besthit.filter(evalue = 0.00001)
;

using KO as open.stream("K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\KO.csv", type = "BBH") {
	KO :> stream.flush(data = assign.KO(forward, reverse));
}