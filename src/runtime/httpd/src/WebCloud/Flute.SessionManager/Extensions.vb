
Imports System.Runtime.CompilerServices
Imports Flute.Http.Configurations

<HideModuleName>
Public Module Extensions

    Public Function Open(session_store As String, ssid As String) As SessionFile
        Dim dir As String = $"{session_store}/{ssid.Substring(ssid.Length - 2, 2)}/{ssid.Substring(ssid.Length - 4, 3)}"
        Dim keyfile As String = $"{dir}/{ssid}.keys"
        Dim datafile As String = $"{dir}/{ssid}"
        Dim file As New SessionFile(keyfile, datafile)

        Return file
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Open(ssid As String, config As Configuration) As SessionFile
        Return SessionManager.Open(config.session.session_store, ssid)
    End Function

End Module
