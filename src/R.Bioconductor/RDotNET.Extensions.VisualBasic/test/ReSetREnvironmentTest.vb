Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.VisualBasic.API
Imports RServer = RDotNET.Extensions.VisualBasic.RSystem

Module ReSetREnvironmentTest

    Sub existsTest()

        Console.WriteLine(base.exists("hello"))

        Pause()

    End Sub

    Sub Main()

        Call existsTest()

        With RServer.R
            Dim x As var = 999
            Dim y As var = False

            !d = base.dataframe(!hello = x, !world = y)

            Call base.print("Resulted data frame:", [string]:=True)
            Call base.print(!d)

            Dim z As var = 9999.Replicate(100000).ToArray

            Call utils.memory_size.__DEBUG_ECHO

            Call .Reset()
        End With

        Call System.GC.Collect()

        With RServer.R

            Call base.print("After reset:", [string]:=True)
            Call utils.memory_size.__DEBUG_ECHO

            Pause()

            ' This statement will cause exception as the environment have been reset
            Call base.print("d")
        End With

        Pause()
    End Sub
End Module
