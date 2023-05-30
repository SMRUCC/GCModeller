imports "http" from "webKit";

const eutils = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi";

const url.search_taxonomy = function(name) {
    `${eutils}?db=taxonomy&term=${urlencode(name)}%5borganism%5d`;
}

const taxonomy_search = function(name) {
    const url  = url.search_taxonomy(name);
    const list = REnv::getHtml(url, interval = 0);

    stop(list);
}