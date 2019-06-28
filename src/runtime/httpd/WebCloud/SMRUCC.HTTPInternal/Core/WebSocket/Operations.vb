Namespace Core.WebSocket

    Public Enum Operations As Integer
        ''' <summary>
        ''' Text Data Sent From Client
        ''' </summary>
        TextRecieved = &H1
        ''' <summary>
        ''' Binary Data Sent From Client 
        ''' </summary>
        BinaryRecieved = &H2
        ''' <summary>
        ''' Ping Sent From Client 
        ''' </summary>
        Ping = &H9
        ''' <summary>
        ''' Pong Sent From Client 
        ''' </summary>
        Pong = &HA
    End Enum
End Namespace