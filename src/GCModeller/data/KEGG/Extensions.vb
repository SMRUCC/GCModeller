Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Math.HashMaps

Public Module Extensions

    Public Function LocusTagGuid(sp$, locusTag$) As Long
        Dim KEGG$ = $"{sp}:{locusTag}"
        Dim hash& = JenkinsHash.hash(KEGG)
        Return hash
    End Function
End Module
