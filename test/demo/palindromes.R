imports "bioseq.patterns" from "seqtoolkit.dll";

let nt as string = "ATTGCCGTTAATTGCCGTTAATTGCCGTTACGTTACGTTACGTTAATTGCCGTTAACGTTATTGCCGTTACGTTACGTTACGTTACGTTACGTTAATTGCATTGCATTGCATTGCATTGCCGTTAAATTGCTTGCCGTTAATTGCATTGCATTGCCGTTACGTTA";

print("Nt for test:");
print(nt);
print(`Nt sequence have ${nchar(nt)} bases.`);

# ATTGC CGTTA
print("Test for palindrome mirror sites:");
print(palindrome.mirror(nt, "ATTGC"));