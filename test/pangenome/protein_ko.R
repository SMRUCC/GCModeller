require(GCModeller);

imports "annotation.workflow" from "seqtoolkit";
imports "annotation.terms" from "seqtoolkit";

let dir = ?"--dir" || stop("no data source provided!");
let proteins = read_m8(file.path(dir, "ko.txt"), parseHitId = 1) |> assign_terms();

write.csv(proteins , file = file.path(dir, "kegg.csv"));
