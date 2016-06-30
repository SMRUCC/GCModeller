Module Program

    Public Function Main() As Integer
        Using PbsServices As PbsHostServer = New PbsHostServer
            Return PbsServices.StartServices
        End Using
    End Function
End Module
