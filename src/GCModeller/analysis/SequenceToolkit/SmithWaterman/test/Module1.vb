#Region "Microsoft.VisualBasic::3783df3b00ba977412ea6bb65bfa9d8f, analysis\SequenceToolkit\SmithWaterman\test\Module1.vb"

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

Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim s1 As New FastaToken("ATGCCCCCCCCCCTGGGAAAAAAAATGCCCACCCCTTTAA", "1")
        Dim s2 As New FastaToken("CCCTGGGAAAAAAAATGCCCCTGGGAAATCCTTTAAAAA", "2")
        Dim align As New SmithWaterman(s1, s2)
        Dim result = align.GetOutput(0.6, 6)

        Pause()
    End Sub
End Module
