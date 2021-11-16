imports "operon" from "comparative_genomics";

print(head(as.data.frame(known_operons()), n = 100));