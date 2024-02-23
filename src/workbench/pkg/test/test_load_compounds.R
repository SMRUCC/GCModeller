require(GCModeller);

let t0 = now();
let list = kegg_compounds(TRUE); 

print(now() - t0);
print(length(list));

write.msgpack(list, file.path(@dir, "compounds.msgpack"));