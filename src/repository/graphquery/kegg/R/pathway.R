imports "repository" from "kegg_kit";

#' Query of kegg pathway
#'
#' @param url the resource url on remote server or local file path for debug
#'
const kegg_pathway = function(url, debug = FALSE) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));
  const getDocument = function(keyName) {
    Html::parse(keyValues[[keyName]]);
  }

  const id         = graphquery::query(document = Html::parse(keyValues$"Entry"),       graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const commonName = graphquery::query(document = Html::parse(keyValues$"Name"),        graphquery = get_graph("graphquery/fields/text.graphquery"));
  const modules    = graphquery::query(document = Html::parse(keyValues$"Module"),      graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const info       = graphquery::query(document = Html::parse(keyValues$"Description"), graphquery = get_graph("graphquery/fields/text.graphquery"));
  const xref       = graphquery::query(document = Html::parse(keyValues$"Other DBs"),   graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));
  const KO_pathway = graphquery::query(document = Html::parse(keyValues$"KO pathway"),  graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
  const compounds  = graphquery::query(document = Html::parse(keyValues$"Compound"),    graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const drugs      = graphquery::query(document = Html::parse(keyValues$"Drug"),        graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const organism   = graphquery::query(document = Html::parse(keyValues$"Organism"),    graphquery = get_graph("graphquery/fields/text.graphquery"));
  const genes      = graphquery::query(document = Html::parse(keyValues$"Gene"),        graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const disease    = graphquery::query(document = Html::parse(keyValues$"Disease"),     graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const references = literature(
    reference = keyValues$Reference,
    authors   = keyValues$Authors,
    title     = keyValues$Title,
    journal   = keyValues$Journal
  );

  if (debug) {
    print(pathwayList(modules));
    print(DBLinks(xref));
    print(references);
    print(pathwayList(compounds));
    print(pathwayList(drugs));
    print(pathwayList(genes));
    print(pathwayList(disease));
  }

  if (isNullString(id)) {
    NULL;
  } else {
    repository::pathway(
      id          = id,
      name        = commonName,
      modules     = pathwayList(modules),
      description = info,
      DBLinks     = DBLinks(xref),
      KO_pathway  = KO_pathway,
      references  = references,
      compounds   = pathwayList(compounds),
      drugs       = pathwayList(drugs),
      organism    = parseKeggCode(organism),
      genes       = pathwayList(genes),
      disease     = pathwayList(disease)
    );
  }
}

#' Parse organism kegg code
#'
#' @return this function returns a list with ``code`` and ``name``.
#'
const parseKeggCode = function(name) {
  if (name == "") {
    list();
  } else {
    let kegg_code = $"\[.+:[a-z]{3,}\]"(name);

    kegg_code = substr(kegg_code, 2, nchar(kegg_code) - 1);
    kegg_code = strsplit(kegg_code, ":")[2];

    list(
      code = kegg_code,
      name = name
    );
  }
}

#' Create a literature dataframe
#'
const literature = function(reference, authors, title, journal) {
  const text_query = get_graph("graphquery/fields/text.graphquery");
  const getText = function(str) {
    graphquery::query(
      document   = Html::parse(str),
      graphquery = text_query
    )
    ;
  }

  data.frame(
    reference = sapply(reference, getText),
    authors   = sapply(authors,   getText),
    title     = sapply(title,     getText),
    journal   = sapply(journal,   getText)
  );
}
