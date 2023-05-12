imports "jQuery" from "webKit";

#' Query the taxonomic list from RegPrecise database
#' 
const list.taxonomic_group = function() {
    const page = jQuery::load('https://regprecise.lbl.gov/collections_tax.jsp');
    const stattbl = page[".stattbl"];
}