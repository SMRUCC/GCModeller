#Region "Microsoft.VisualBasic::0538b66ce0be1bfd98e7952ac18cd363, analysis\SequenceToolkit\MotifFinder\test\Program.vb"

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

    '   Total Lines: 14
    '    Code Lines: 12 (85.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 2 (14.29%)
    '     File Size: 583 B


    ' Module Program
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports System
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim data As FastaFile = FastaFile.LoadNucleotideData("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Staphylococcaceae_LexA___Staphylococcaceae.fasta")
        Dim avgLen As Integer = data.Average(Function(seq) seq.Length) - 1
        Dim gibbs As New GibbsSampler(data, motifLength:=avgLen)
        Dim motif As MSAMotif = gibbs.find

        Pause()
    End Sub
End Module
