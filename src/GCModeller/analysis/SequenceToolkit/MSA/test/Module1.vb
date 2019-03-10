﻿#Region "Microsoft.VisualBasic::d2c3b7e5a9261601d7c270efb83bc095, analysis\SequenceToolkit\MSA\test\Module1.vb"

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
    '     Sub: exceptionTest, Main
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()

        Call exceptionTest()

        Dim seq$() = "GATGTGGGGCCG
AAGTCCGAG
GATGTGCAG
CCGTCTAGCAGT
CCTGCTGCAG
CCTGTAGGAACAG".lTokens
        Dim matrix = "D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\MSA\Matrix.txt".ReadAllLines.Select(Function(l) l.Replace(" "c, "").ToArray).ToArray
        Dim msa = seq.MultipleAlignment(New ScoreMatrix With {.Matrix = matrix})

        Call msa.ToFasta.Save("./msa.txt")
        Call Console.WriteLine(msa)

        Console.WriteLine(vbCrLf)

        msa = FastaFile.LoadNucleotideData("D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Xanthomonadales_MetR___Xanthomonadales.fasta").MultipleAlignment(New ScoreMatrix With {.Matrix = matrix})

        Call msa.Print(15)

        Pause()
    End Sub

    Sub exceptionTest()

        Dim seq$() = "CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGACGCACCGC
CGCACGCGC
CGCACGCGC
CGCACGCGC
CGCACGCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGCACCCGC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CCGACGCACCGCAC
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCA
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGACGCACCGCACT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGCCT
CGCACCCGC
CGCACCCGC
CGCACCCGC
CGCACCCGC
CCGGCACACGCG
CCGGCACACGCG
CCGGCACACGCG
CCGGCACACGCG
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGCACTCGC
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA
CCGACGCACCGCA".lTokens
        Dim msa = seq.MultipleAlignment(ScoreMatrix.DefaultMatrix)

        Call msa.ToFasta.Save("./msa.txt")
        Call Console.WriteLine(msa)

        Pause()
    End Sub
End Module
