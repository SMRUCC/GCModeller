#Region "Microsoft.VisualBasic::ec8619daa79a6a7f15ef23f450c29d63, Test\Module1.vb"

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

    ' Module Module1
    ' 
    '     Sub: engineTest, Main, polysat
    ' 
    ' /********************************************************************************/

#End Region

' Imports RDotNET.Extensions.Bioinformatics
Imports RDotNet.Extensions.VisualBasic.API
Imports RDotNet.Extensions.VisualBasic
Imports Microsoft.VisualBasic.Serialization.JSON

Module Module1

    Sub Main()

        Call engineTest()
    End Sub

    Sub engineTest()

        Dim a#() = {1.59, 3.963, 6.555, 2.365, 3, 4.233}
        Dim b = 0R.Replicate(a.Length).ToArray

        Dim pvalue = stats.Ttest(a, b, varEqual:=True)

        Call pvalue.GetJson.__DEBUG_ECHO


        Pause()
    End Sub

    Sub polysat()
        'require("polysat")

        'Dim oooo = polysat.calcFst(polysat.myfreq)

        'Dim nn = RDotNet.Extensions.Bioinformatics.adegenet.genind.nancycats

        'Dim nnx As var = 44

        'nnx = {1, 2, 3, 4, 5}


        'Pause()
    End Sub
End Module
