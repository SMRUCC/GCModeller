imports "repository" from "kegg_kit";

#' query data to gcmodeller object model
#'
#' @param url the resource url on remote server or local file path for debug
#'
const kegg_compound as function(url) {
  # parse the page text
  const keyValues = keyIndex(http_query(url, raw = FALSE));

  # str(keyValues);
  # stop();

  print("start to parse fields...");
  
  # parse fields
  const xref        = graphquery::query(document = Html::parse(keyValues$"Other DBs"),  graphquery = get_graph("graphquery/fields/dbLinks.graphquery"));
  const KCF_text    = graphquery::query(document = Html::parse(keyValues$"KCF data"),   graphquery = get_graph("graphquery/fields/KCFtext.graphquery"));
  const pathways    = graphquery::query(document = Html::parse(keyValues$"Pathway"),    graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const modules     = graphquery::query(document = Html::parse(keyValues$"Module"),     graphquery = get_graph("graphquery/fields/pathway_item.graphquery"));
  const reactionId  = graphquery::query(document = Html::parse(keyValues$"Reaction"),   graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));
  const commonNames = graphquery::query(document = Html::parse(keyValues$"Name"),       graphquery = get_graph("graphquery/fields/commonNames.graphquery"))
  |> strsplit("\r|\n")
  |> trim("; ")
  ;
  const id          = graphquery::query(document = Html::parse(keyValues$"Entry"),      graphquery = get_graph("graphquery/fields/simpleText.graphquery"));
  const formula     = graphquery::query(document = Html::parse(keyValues$"Formula"),    graphquery = get_graph("graphquery/fields/text.graphquery"));
  const exactMass   = graphquery::query(document = Html::parse(keyValues$"Exact mass"), graphquery = get_graph("graphquery/fields/text.graphquery"));
  const remarks     = graphquery::query(document = Html::parse(keyValues$"Remark"),     graphquery = get_graph("graphquery/fields/text.graphquery"));
  const EC_idlist   = graphquery::query(document = Html::parse(keyValues$"Enzyme"),     graphquery = get_graph("graphquery/fields/reactionLink.graphquery"));

  print(DBLinks(xref));
  print(pathwayList(pathways));
  print(pathwayList(modules));

  if (isNullString(id)) {
    NULL;
  } else {
    # create data model
    repository::compound(
      entry     = id,
      name      = commonNames[commonNames != ""],
      reaction  = reactionId[reactionId == $"R\d+"],
      enzyme    = EC_idlist[EC_idlist   == $"\d[.].+"],
      formula   = formula,
      exactMass = exactMass,
      remarks   = remarks,
      KCF       = KCF_text,
      DBLinks   = DBLinks(xref),
      pathway   = pathwayList(pathways),
      modules   = pathwayList(modules)
    );
  }
}

const pathwayList as function(list) {
  const id   = sapply(list, r -> r$id);
  const link = sapply(list, r -> r$link);
  const name = sapply(list, r -> r$name);

  data.frame(id, link, name);
}

const DBLinks as function(xref) {
  const db   = sapply(xref, r -> r$dbName);
  const id   = sapply(xref, r -> r$id);
  const link = sapply(xref, r -> r$link);

  data.frame(db, id, link);
}

