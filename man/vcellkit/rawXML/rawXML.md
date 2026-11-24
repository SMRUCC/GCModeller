# rawXML

the virtual cell raw data
> the combination of the stream frame data in the rawpack file:
>  
>  mass profile:
>  
>  + transcriptome -> mass_profile
>  + proteome -> mass_profile
>  + metabolome -> mass_profile
>  
>  flux profile:
>  
>  + transcriptome -> activity
>  + proteome -> activity
>  + metabolome -> flux_size

+ [open.vcellPack](rawXML/open.vcellPack.1) open the simulation data storage driver
+ [open.vcellXml](rawXML/open.vcellXml.1) open gcXML raw data file for read/write
+ [frame.index](rawXML/frame.index.1) [debug api]
+ [entity.names](rawXML/entity.names.1) 
+ [frame.matrix](rawXML/frame.matrix.1) get a frame matrix for compares between different samples.
+ [time.frames](rawXML/time.frames.1) Get a sample matrix data in a timeline.
