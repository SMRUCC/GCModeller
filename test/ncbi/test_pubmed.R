require(GCModeller);

imports "pubmed" from "kb";

let xml = readText(file.path(@dir, "test_pubmed.xml"));
let article = pubmed::article(xml, xml = TRUE);

str(as.list(article));