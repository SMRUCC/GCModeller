#' A helper function for make conversion from the ec number list to ko id
#' 
#' @param ec_number a character vector of the ec number.
#' 
#' @return a dataframe object that mapping the ec number to the corresponding ko id, this dataframe object contains two data fields:
#' 
#' 1. ec_number: the enzyme number from the parameter input
#' 2. ko: the kegg ko id that the enzyme mapping to
#' 3. symbol: the gene name of the enzyme
#' 4. name: the gene function, or the full name
#' 
const ecnumber_to_ko = function(ec_number) {
    imports "brite" from "kegg_kit";

    let brite = brite::parse("ko00001");
    let df = as.data.frame(brite.as.table(brite));
    
    print(df, max.print = 6);
    str(df);

    stop();
}