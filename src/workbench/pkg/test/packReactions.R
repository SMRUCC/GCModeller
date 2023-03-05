require(GCModeller);
require(HDS);

imports "repository" from "kegg_kit";

let raw = HDS::openStream("D:\GCModeller\src\workbench\pkg\test\kegg_reactions.db", readonly = TRUE);
let files = raw |> files(dir = "/reactions/", excludes_dir = TRUE);
let reactions = files 
|> lapply(f -> HDS::getData(raw, f) |> loadXml(typeof = "kegg_reaction"))
|> unlist() 
|> as.vector()
;

reactions
|> write.msgpack(
	file = "D:\GCModeller\src\workbench\pkg\data\kegg\reactions.msgpack"
);