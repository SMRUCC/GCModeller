#Region "Microsoft.VisualBasic::61eedf8fdd3745e824f65c92d0a94db9, analysis\SequenceToolkit\MSA\test\Module1.vb"

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

Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim seq$() = "GATGTGGGGCCG
AAGTCCGAG
GATGTGCAG
CCGTCTAGCAGT
CCTGCTGCAG
CCTGTAGGAACAG".lTokens
        Dim matrix = "D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\MSA\Matrix.txt".ReadAllLines.Select(Function(l) l.Replace(" "c, "").ToArray).ToArray
        Dim msa = seq.MultipleAlignment(matrix)

        Call msa.ToFasta.Save("./msa.txt")
        Call Console.WriteLine(msa)

        Console.WriteLine(vbCrLf)

        msa = FastaFile.LoadNucleotideData("D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Xanthomonadales_MetR___Xanthomonadales.fasta").MultipleAlignment(matrix)

        Call msa.Print(15)

        Pause()
    End Sub
End Module

