Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RServer = RDotNET.Extensions.VisualBasic.RSystem

Module ReSetREnvironmentTest

    Sub Main()

        Call App.CurrentDirectory.__DEBUG_ECHO

        Using RServer.R
            With RServer.R
                Dim x As var = 999
                Dim y As var = False

                !d = base.dataframe(!hello = x, !world = y)

                Call base.print(!d)

                Call App.CurrentDirectory.__DEBUG_ECHO
            End With
        End Using

        Call App.CurrentDirectory.__DEBUG_ECHO

        Call RServer.TryInit()

        Call base.print("d")

        Pause()
    End Sub
End Module
