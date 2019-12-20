imports "bioseq.patterns" from "seqtoolkit.dll";

let nt as string = "ATTGCCGTTAATTGCATTGCATTGCCGTTAACGTTATTGCATTGCATTGCATTGCATTGCATTGCAATTGCTTGCATTGCATTGCATTGC";

print("Nt for test:");
print(nt);

# ATTGC CGTTA
print(palindrome.mirror(nt, "ATTGC"));