Imports System.Net.Sockets
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices.Plugin
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Language.Values

Namespace Core.WebSocket

    ''' <summary>
    ''' A static method should be marked with <see cref="PluginAttribute"/>
    ''' </summary>
    ''' <param name="tcp"></param>
    ''' <returns></returns>
    Public Delegate Function WebsocketActivator(tcp As TcpClient) As WsProcessor

    <HideModuleName> Public Module Extensions

        Public Function GetActivator(directory$, entry As String) As WebsocketActivator
            Dim activator As New Value(Of MethodInfo)

            For Each dll As String In ls - l - r - "*.dll" <= directory
                If Not (activator = Loader.GetPluginMethod(dll, entry)) Is Nothing Then
                    Return activator.CreateDelegate(GetType(WebsocketActivator))
                End If
            Next

            Return Nothing
        End Function
    End Module
End Namespace