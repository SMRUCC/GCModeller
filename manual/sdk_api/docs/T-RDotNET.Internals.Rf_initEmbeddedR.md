---
title: Rf_initEmbeddedR
---

# Rf_initEmbeddedR
_namespace: [RDotNET.Internals](N-RDotNET.Internals.html)_

Initialise R for embedding

> 
>  '
>  int Rf_initEmbeddedR(int argc, char **argv)
> {
>     Rf_initialize_R(argc, argv);
>    // R_Interactive is set to true in unix Rembedded.c, not gnuwin
>     R_Interactive = TRUE;  /* Rf_initialize_R set this based on isatty */
>     setup_Rmainloop();
>     return(1);
> }
>  '
>  



