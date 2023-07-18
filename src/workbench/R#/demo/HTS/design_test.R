imports "sampleInfo" from "phenotype_kit";

let new1 = sampleInfo::design(
    read.sampleinfo(`${@dir}/sampleInfo.csv`),
    C = C6 + C9,
    I = I56 + I59 + I86 + I89
);

print(as.data.frame(new1));