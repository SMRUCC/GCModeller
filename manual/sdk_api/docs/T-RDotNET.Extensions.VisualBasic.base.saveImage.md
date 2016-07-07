---
title: saveImage
---

# saveImage
_namespace: [RDotNET.Extensions.VisualBasic.base](N-RDotNET.Extensions.VisualBasic.base.html)_

save.image() is just a short-cut for ‘save my current workspace’, i.e., save(list = ls(all.names = TRUE), file = ".RData", envir = .GlobalEnv). It is also what happens with q("yes").




### Properties

#### ascii
if TRUE, an ASCII representation of the data is written. The default value of ascii is FALSE which leads to a binary file being written. 
 If NA and version >= 2, a different ASCII representation is used which writes double/complex numbers as binary fractions.
#### compress
logical or character string specifying whether saving to a named file is to use compression. TRUE corresponds to gzip compression, and character strings "gzip", "bzip2" or "xz" specify the type of compression. 
 Ignored when file is a connection and for workspace format version 1.
#### file
a (writable binary-mode) connection or the name of the file where the data will be saved (when tilde expansion is done). Must be a file name for save.image or version = 1.
#### safe
logical. If TRUE, a temporary file is used for creating the saved workspace. The temporary file is renamed to file if the save succeeds. 
 This preserves an existing workspace file if the save fails, but at the cost of using extra disk space during the save.
#### version
the workspace format version to use. NULL specifies the current default format. The version used from R 0.99.0 to R 1.3.1 was version 1. The default format as from R 1.4.0 is version 2.
