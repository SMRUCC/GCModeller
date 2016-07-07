---
title: save
---

# save
_namespace: [RDotNET.Extensions.VisualBasic.base](N-RDotNET.Extensions.VisualBasic.base.html)_

save writes an external representation of R objects to the specified file. The objects can be read back from the file at a later date by using the function load or attach (or data in some cases).

> 
>  The names of the objects specified either as symbols (or character strings) in ... or as a character vector in list are used to look up the objects from environment envir. By default promises are evaluated, but if eval.promises = FALSE promises are saved (together with their evaluation environments). (Promises embedded in objects are always saved unevaluated.)
> 
>  All R platforms use the XDR (bigendian) representation Of C ints And doubles In binary save-d files, And these are portable across all R platforms.
>  ASCII saves used To be useful For moving data between platforms but are now mainly Of historical interest. They can be more compact than binary saves where compression Is Not used, but are almost always slower To both read And write: binary saves compress much better than ASCII ones. Further, Decimal ASCII saves may Not restore Double/complex values exactly, And what value Is restored may depend On the R platform.
>  Default values For the ascii, compress, safe And version arguments can be modified With the "save.defaults" Option (used both by save And save.image), see also the 'Examples’ section. If a "save.image.defaults" option is set it is used in preference to "save.defaults" for function save.image (which allows this to have different defaults). In addition, compression_level can be part of the "save.defaults" option.
>  A connection that Is Not already open will be opened In mode "wb". Supplying a connection which Is open And Not In binary mode gives an Error.
>  



### Properties

#### ascii
if TRUE, an ASCII representation of the data is written. The default value of ascii is FALSE which leads to a binary file being written. If NA and version >= 2, a different ASCII representation is used which writes double/complex numbers as binary fractions.
#### compress
logical or character string specifying whether saving to a named file is to use compression. TRUE corresponds to gzip compression, and character strings "gzip", "bzip2" or "xz" specify the type of compression. Ignored when file is a connection and for workspace format version 1.
#### compression_level
integer: the level of compression to be used. Defaults to 6 for gzip compression and to 9 for bzip2 or xz compression.
#### envir
environment to search for objects to be saved.
#### evalPromises
logical: should objects which are promises be forced before saving?
#### file
a (writable binary-mode) connection or the name of the file where the data will be saved (when tilde expansion is done). Must be a file name for save.image or version = 1.
#### Func
Save R Objects
#### list
A character vector containing the names of objects to be saved.
#### precheck
logical: should the existence Of the objects be checked before starting To save (And In particular before opening the file/connection)? Does Not apply To version 1 saves.
#### version
the workspace format version to use. NULL specifies the current default format. The version used from R 0.99.0 to R 1.3.1 was version 1. The default format as from R 1.4.0 is version 2.
#### x
the names of the objects to be saved (as symbols or character strings).
