#Region "Microsoft.VisualBasic::1b3276e10e59df972c130d92cbcd81db, localblast\assembler\test\Module1.vb"

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

Imports SMRUCC.genomics.Interops.NCBI.Localblast.Assembler
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim fa = "D:\biodeep\1_combined_R1.fasta"
        Dim reads = FastaFile.LoadNucleotideData(fa).Select(Function(f)
                                                                f.SequenceData = Mid(f.SequenceData, 10, fa.Length - 20)
                                                                Return f
                                                            End Function).ToArray
        Dim contigs = Greedy.DeNovoAssembly(reads, identity:=0.7, similar:=0.4).ToArray

        Call New FastaFile(contigs).Save("D:\biodeep\1_combined_R1_contigs.fasta")

        'Dim fa = "E:\Resources\JCC1_combined_R1.fasta"
        'Dim contigs = Greedy.DeNovoAssembly(FastaFile.LoadNucleotideData(fa), identity:=0.1, similar:=0.05).ToArray

        'Call New FastaFile(contigs).Save("E:\Resources\JCC1_combined_R1_contigs.fasta")

        Pause()
    End Sub

End Module
