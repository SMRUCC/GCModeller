Imports LANS.SystemsBiology.AnalysisTools.ProteinTools.Family

Module Program

    Public Function Main() As Integer
#If DEBUG Then
        Call KEGG.ParsingFamilyDef("DNA-binding transcriptional repressor FabR (A)").__DEBUG_ECHO
#End If
        Return GetType(CLI).RunCLI(App.CommandLine)
    End Function
End Module
