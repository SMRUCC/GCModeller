let table <- read.csv("K:\20200226\TRN\1025_mapping.csv");

table[, "VF"] <- $"VF[=]\d"(table[, "Reference"]) :> sapply(r -> r[1]) :> as.character;
table[, "EG"] <- $"EG[=]\d"(table[, "Reference"]) :> sapply(r -> r[1]) :> as.character;

table
:> write.csv(file = "K:\20200226\TRN\1025_mapping.details.csv", row_names = FALSE) 
;