imports "http" from "webKit";
imports ["Html", "graphquery"] from "webKit";

const eutils = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi";
const taxonomy_query = "

ncbi_taxid css('eSearchResult', *)
           | css('IdList', *)
           | css('Id', *)
           [
               text()
           ]
";

const url.search_taxonomy = function(name) {
    `${eutils}?db=taxonomy&term=${urlencode(name)}%5borganism%5d`;
}

const taxonomy_search = function(name) {
    const url   = url.search_taxonomy(name);
    const xml   = REnv::getHtml(url, interval = 0);
    const list  = Html::parse(xml); 
    const taxid = graphquery::query(
        document   = list, 
        graphquery = graphquery::parseQuery(taxonomy_query)
    );

    print("get ncbi taxonomy id mapping for:");
    print(name);
    print(taxid);

    taxid;
}