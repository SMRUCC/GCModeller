require(GCModeller);

let t0 = now();
let list = extract_reactions(); 

print(now() - t0);
print(length(list));

write.msgpack(list, file.path(@dir, "reactions.msgpack"));