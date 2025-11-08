const download_proteins = function(q, tax_id = NULL) {
    imports "UniProt" from "annotationKit";

    let result = UniProt::rest_query(q, tax_id);
    
}