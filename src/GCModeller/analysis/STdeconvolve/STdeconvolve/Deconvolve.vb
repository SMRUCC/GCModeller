﻿Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Public Class Deconvolve

    ''' <summary>
    ''' each pixel is defined as a mixture of 𝐾 cell types 
    ''' represented As a multinomial distribution Of cell-type 
    ''' probabilities
    ''' </summary>
    ''' <returns></returns>
    Public Property theta As Matrix

    ''' <summary>
    ''' each cell-type Is defined as a probability distribution 
    ''' over the genes (𝛽) present in the ST dataset.
    ''' </summary>
    ''' <returns></returns>
    Public Property topicMap As Dictionary(Of String, Double)()

End Class
