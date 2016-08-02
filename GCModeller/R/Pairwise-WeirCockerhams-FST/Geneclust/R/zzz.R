.First.lib <- function (lib, pkg) 
{
  library.dynam("Geneclust",pkg, lib)
  cat('**********************************************************',fill=TRUE)
  cat('*   Geneclust 1.0.1 is loaded                            *',fill=TRUE)
  cat('*                                                        *',fill=TRUE)
  cat('*   Use help(Geneclust) for a quick overview.            *',fill=TRUE)
  cat('*                                                        *',fill=TRUE)
  cat('**********************************************************',fill=TRUE)
  require(spatial)

}


