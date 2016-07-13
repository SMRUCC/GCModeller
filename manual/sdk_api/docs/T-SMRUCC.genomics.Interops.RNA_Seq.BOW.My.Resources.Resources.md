---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Interops.RNA_Seq.BOW.My.Resources](N-SMRUCC.genomics.Interops.RNA_Seq.BOW.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### bcftools
Looks up a localized resource of type System.Byte[].
#### bgzip
Looks up a localized resource of type System.Byte[].
#### BWA
Looks up a localized resource of type System.Byte[].
#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### HTSeq_Count_Invoked
Looks up a localized string similar to Imports htseq-count

 Out.Dir <- {$Out.Dir}
 File.Sam <- {$File.Sam}
 Table <- htseq-count SAM $File.Sam Gff {$GFF} Mode {$Mode} RPKM {$RPKM}
 File.Sam <- basename $File.Sam

# Write Table Data to the specific data output directory.
$Table > $Out.Dir/$File.Sam.txt.
#### razip
Looks up a localized resource of type System.Byte[].
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
#### s2b
Looks up a localized string similar to samtools view -h -o out.sam in.bam.
#### samtools
Looks up a localized resource of type System.Byte[].
#### tabix
Looks up a localized resource of type System.Byte[].
