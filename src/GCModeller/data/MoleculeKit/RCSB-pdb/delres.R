let pdb_file = ?"--pdb" || stop("no input file");
let pdb_out = ?"--out" || pdb_file;
[@desc "atom input should be in format like: atom amino_acid, example as: ``HA3 GLY``, which could resolve 
the gromacs problem of: Atom HA3 in residue GLY 1 was not found in rtp entry NGLY with 9 atoms
while sorting atoms."]
let atom = ?"--atom" || stop("no atom data to deletes");

let txt = readLines(pdb_file);
let filter = instr(txt, atom) > 0;

print("atom data lines that going to be removed:");
print(which(filter));
print("preview of the data lines to be removed:");
print(txt[filter]);

writeLines(txt[!filter], con = pdb_out, sep = "\n");