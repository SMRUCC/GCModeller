## Build BIOM model for MEGAN

### Reads content profiling between samples

### Assign OTU samples with taxonomy information

About how to creates the metagenome taxonomy information for each OTU sequence, reference to this page: [Annotation of the taxonomy using GCModeller](./Metagenomics_taxonomy.md)

### Export BIOM using GCModeller

```bash
rna-seq ? /Export.Megan.BIOM
# Help for command '/Export.Megan.BIOM':
#
#   Information:
#   Usage:        /home/biostack/GCModeller/RNA-seq /Export.Megan.BIOM /in <relative.table.csv> [/out <out.json.biom>]
#   Example:      CLI usage example not found!

# Example as:
RNA-seq /Export.Megan.BIOM /in OTU-relative_samples.table.csv
```

### Data visualize using MEGAN
