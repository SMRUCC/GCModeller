Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

Module BackgroundHelper

    Public Function CastBackground(geneSet As Object, env As Environment) As [Variant](Of Background, Message)
        If geneSet Is Nothing Then
            Return RInternal.debug.stop("the required enrichment background model should not be nothing!", env)
        End If

        If TypeOf geneSet Is List Then
            Return backgroundFromList(DirectCast(geneSet, list), env)
        ElseIf TypeOf geneSet Is Background Then
            Return DirectCast(geneSet, Background)
        Else
            Return Message.InCompatibleType(GetType(Background), geneSet.GetType, env)
        End If
    End Function
End Module
