require(GCModeller);

imports "mesh" from "kb";

let mesh = read.mesh_tree(system.file("data/mtrees2024.txt", package = "GCModeller"));
let model  = mesh |> mesh_background(category = "D");

model 
|> xml 
|> writeLines(con = file.path(@dir, "mesh_compound.xml"))
;