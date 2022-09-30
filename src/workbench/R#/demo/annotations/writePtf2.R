imports "package_utils" from "devkit";

package_utils::attach("D:\GCModeller\src\workbench\pkg");

require(GCModeller);

imports "ptf" from "annotationKit";
imports "uniprot" from "seqtoolkit";

uniprot = open.uniprot("F:\Downloads\uniprot-compressed_true_download_true_format_xml_query__28_28taxonom-2022.09.30-08.22.32.52.xml");

cache.ptf(uniprot, file = "F:\Downloads\uniprot.dat", hds.stream = TRUE);