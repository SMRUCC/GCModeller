imports "gseakit.DEG_sample" from "gseakit.dll";

let sampleInfo <- [file = ?"--sampleInfo"] 
  :> read.sampleinfo 
  :> as.object 
  :> groupBy(sample => sample$sample_group);

# for(sample in sampleInfo) {
#     print(sample);
# }

let control.label as string = "normal";
let normal = sampleInfo :> first(x => x$key == control.label);

print(normal);

# split file test
sampleInfo 
:> which(x -> x$key != control.label) 
:> projectAs(group -> group$group :> write.sampleinfo( file = `${?"--sampleInfo"}.group=${group$key}.csv`));

for(group in sampleInfo :> which(x -> x$key != control.label)) {
   # print(group);
}