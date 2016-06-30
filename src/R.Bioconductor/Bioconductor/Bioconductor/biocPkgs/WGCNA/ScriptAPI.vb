Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes
Imports RDotNET.Extensions.VisualBasic

Namespace WGCNA

    <PackageNamespace("bioc.WGCNA")>
    Public Module ScriptAPI

        <ExportAPI("WGCNA")>
        Public Function BuildScript() As WGCNA.App.WGCNA
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' allowWGCNAThreads enables parallel calculation within the compiled code in WGCNA, principally for calculation of correlations in the presence of missing data. This function is now deprecated; use enableWGCNAThreads instead.
        ''' </summary>
        ''' <param name="nThreads">
        ''' Number of threads to allow. If not given, the number of processors online (as reported by system configuration) will be used. 
        ''' There appear to be some cases where the automatically-determined number is wrong; please check the output to see that the number of threads makes sense. 
        ''' Except for testing and/or torturing your system, the number of threads should be no more than the number of actual processors/cores.
        ''' </param>
        ''' <returns>allowWGCNAThreads, enableWGCNAThreads, and disableWGCNAThreads return the maximum number of threads WGCNA calculations will be allowed to use.</returns>
        Public Function allowWGCNAThreads(Optional nThreads As String = NULL) As String
            Return $"allowWGCNAThreads(nThreads = {nThreads})"
        End Function

        ''' <summary>
        ''' enableWGCNAThreads enables parallel calculations within user-level R functions as well as within the compiled code, and registers an appropriate parallel calculation back-end for the operating system/platform.
        ''' </summary>
        ''' <param name="nThreads">
        ''' Number of threads to allow. If not given, the number of processors online (as reported by system configuration) will be used. 
        ''' There appear to be some cases where the automatically-determined number is wrong; please check the output to see that the number of threads makes sense. 
        ''' Except for testing and/or torturing your system, the number of threads should be no more than the number of actual processors/cores.
        ''' </param>
        ''' <returns>allowWGCNAThreads, enableWGCNAThreads, and disableWGCNAThreads return the maximum number of threads WGCNA calculations will be allowed to use.</returns>
        Public Function enableWGCNAThreads(Optional nThreads As String = NULL) As String
            Return $"enableWGCNAThreads(nThreads = {nThreads})"
        End Function

        ''' <summary>
        ''' disableWGCNAThreads disables parallel processing.
        ''' </summary>
        ''' <returns>allowWGCNAThreads, enableWGCNAThreads, and disableWGCNAThreads return the maximum number of threads WGCNA calculations will be allowed to use.</returns>
        Public Function disableWGCNAThreads() As String
            Return "disableWGCNAThreads()"
        End Function

        ''' <summary>
        ''' WGCNAnThreads returns the number of threads (parallel processes) that WGCNA is currently configured to run with.
        ''' </summary>
        ''' <returns>allowWGCNAThreads, enableWGCNAThreads, and disableWGCNAThreads return the maximum number of threads WGCNA calculations will be allowed to use.</returns>
        Public Function WGCNAnThreads() As String
            Return "WGCNAnThreads()"
        End Function
    End Module
End Namespace
