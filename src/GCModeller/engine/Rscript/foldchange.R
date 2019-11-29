imports "gseakit.DEG_sample" from "gseakit.dll";

let sampleInfo <- [file = ?"--sampleInfo"] 
  :> read.sampleinfo 
  :> as.object 
  :> groupBy(sample => sample$sample_group);

# load experiment result data
let data <- [file = ?"--data"] :> read.dataframe(mode = "numeric");

# for(sample in sampleInfo) {
#     print(sample);
# }

print("Try to parse normal controls:");

let control.label as string = "normal";
let normal = sampleInfo :> first(x => x$key == control.label);
let normalSample = data :> dataset.project(cols = normal :> projectAs(x -> x$ID)  );

normalSample :> write.csv( file = `${?"--data"}.${control.label}.csv`);

print(normal);

# split file test
# sampleInfo 
# :> which(x -> x$key != control.label) 
# :> projectAs(group -> group$group :> write.sampleinfo( file = `${?"--sampleInfo"}.group=${group$key}.csv`));

cat("\n\n");

for(group in sampleInfo :> which(x -> x$key != control.label)) {
   print(`   ${group$key}`);
}