imports "package_utils" from "devkit";

package_utils::attach("E:\GCModeller\src\workbench\pkg");

require(GCModeller);

imports "UniProt" from "annotationKit";
imports "uniprot" from "seqtoolkit";
imports "bioseq.fasta" from "seqtoolkit";

using pack as UniProt::ECnumber_pack("K:\Downloads\EC_numbers.db", create_new = TRUE) {

	pack |> add_ecNumbers(
		"K:\Downloads\uniprot-compressed_true_download_true_format_xml_query__28_28taxonom-2022.10.02-09.21.31.14.xml"
		|> open.uniprot()
	);

}

using pack as UniProt::ECnumber_pack("K:\Downloads\EC_numbers.db", create_new = FALSE) {

	pack 
	|> extract_fasta()
	|> write.fasta(file = "K:\Downloads\EC_numbers.fasta")
	;
	
}
