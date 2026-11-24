require(GCModeller);

imports "annotation.workflow" from "seqtoolkit";
imports "annotation.terms" from "seqtoolkit";

let dir = ?"--dir" || stop("no data source provided!");
let proteins = read_m8(file.path(dir, "proteins.txt")) |> assign_terms();

write.csv(proteins , file = file.path(dir, "proteins.csv"));
