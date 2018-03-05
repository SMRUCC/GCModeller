#Region "Microsoft.VisualBasic::20110e2404dad3a09b7139f110954504, Test\Module1.vb"

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
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.Bioinformatics
Imports RDotNET.Extensions.VisualBasic.API
Imports RDotNET.Extensions.VisualBasic

Module Module1

    Sub Main()

        require("polysat")

        Dim oooo = polysat.calcFst(polysat.myfreq)

        Dim nn = RDotNET.Extensions.Bioinformatics.adegenet.genind.nancycats

        Dim nnx As var = 44

        nnx = {1, 2, 3, 4, 5}


        Pause()
    End Sub

End Module
