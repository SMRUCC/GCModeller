imports "gseakit.background" from "gseakit.dll";

let save = is.empty(?"--save") ? `${?"--background"}.KO.csv` : [?"--save"];

[?"--background"]
:> read.background
:> KO.table
:> write.csv( file = save);
