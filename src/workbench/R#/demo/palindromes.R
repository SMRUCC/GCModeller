imports "bioseq.patterns" from "seqtoolkit.dll";

let nt as string = "ATTGCCGTTAATTGCCGTTAATTGCCGTTACGTTACGTTACGTTAATTGCCGTTAACGTTATTGCCGTTACGTTACGTTACGTTACGTTACGTTAATTGCATTGCATTGCATTGCATTGCCGTTAAATTGCTTGCCGTTAATTGCATTGCATTGCCGTTACGTTA";

print("Nt for test:");
print(nt);
print(sprintf("Nt sequence have %s bases.", nchar(nt)));

# ATTGC CGTTA
print(palindrome.mirror(nt, "ATTGC"));