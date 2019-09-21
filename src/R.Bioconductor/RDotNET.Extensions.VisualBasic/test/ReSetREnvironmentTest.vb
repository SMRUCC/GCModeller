#Region "Microsoft.VisualBasic::d1c1bb5b633be7db15940f29f9266825, RDotNET.Extensions.VisualBasic\test\ReSetREnvironmentTest.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module ReSetREnvironmentTest
    ' 
    '     Sub: existsTest, Main
    ' 
    ' /********************************************************************************/

#End Region

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

