Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RServer = RDotNET.Extensions.VisualBasic.RSystem

Module ReSetREnvironmentTest

    Sub Main()
        Using RServer.R
            With RServer.R
                Dim x As var = 999
                Dim y As var = False

                !d = base.dataframe(!hello = x, !world = y)

                Call base.print(!d)
            End With
        End Using



        Pause()
    End Sub
End Module
