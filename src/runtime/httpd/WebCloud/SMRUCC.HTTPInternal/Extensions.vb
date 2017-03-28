Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports SMRUCC.WebCloud.HTTPInternal.AppEngine.APIMethods.Arguments

Public Module Extensions

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path$">The file path of the local file that will be transfer to the client browser.</param>
    ''' <param name="MIMEtype$"><see cref="MIME"/></param>
    ''' <param name="out"></param>
    ''' <param name="buffer_size%"></param>
    <Extension>
    Public Sub TransferBinary(path$, MIMEtype$, ByRef out As HttpResponse, Optional buffer_size% = 4096)
        Dim buffer As Byte() = New Byte(buffer_size) {}

        Using reader As New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            Call out.WriteHeader(MIMEtype, reader.Length)

            Do While reader.Position <= reader.Length
                Call reader.Read(buffer, Scan0, buffer.Length)
                Call out.Write(buffer)
            Loop
        End Using
    End Sub
End Module
