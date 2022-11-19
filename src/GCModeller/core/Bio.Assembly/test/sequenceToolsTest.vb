#Region "Microsoft.VisualBasic::2542e098f9a63de60ea2660649edd7d1, GCModeller\core\Bio.Assembly\Test\sequenceToolsTest.vb"

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


    ' Code Statistics:

    '   Total Lines: 16
    '    Code Lines: 14
    ' Comment Lines: 0
    '   Blank Lines: 2
    '     File Size: 744 B


    ' Module sequenceToolsTest
    ' 
    '     Sub: Main1
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Module sequenceToolsTest
    Sub Main1()
        Dim nt As New FastaSeq With {.Headers = {"ABC"}, .SequenceData = "abcdefghijklmnopqrstuvwxyz"}
        Dim cut = nt.CutSequenceLinear(New Location With {.left = 1, .right = 7})
        Dim cut2 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 25})
        Dim cut3 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 26})
        Dim cut4 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 30})
        Dim cut5 = nt.CutSequenceLinear(New Location With {.left = 30, .right = 32})

        Pause()
    End Sub
End Module
