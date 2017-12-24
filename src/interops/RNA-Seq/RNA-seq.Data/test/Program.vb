#Region "Microsoft.VisualBasic::2ff0e9ae2d74c4ce67955568943ce588, ..\interops\RNA-Seq\RNA-seq.Data\test\Program.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports SMRUCC.genomics.SequenceModel.FQ

Module Program

    Sub Main()
        Dim q = FastQ.GetQualityOrder("@"c)
        q = FastQ.GetQualityOrder("5"c)

        Call Stream _
            .ReadAllLines("F:\2017-12-6-16s_test\test\T1-1_combined_R1.fastq") _
            .TrimLowQuality _
            .TrimShortReads(200) _
            .WriteFastQ("F:\2017-12-6-16s_test\test\T1-1_combined_R1.trim.fastq")

        Call Stream _
            .ReadAllLines("F:\2017-12-6-16s_test\test\T1-1_combined_R2.fastq") _
            .TrimLowQuality _
            .TrimShortReads(200) _
            .WriteFastQ("F:\2017-12-6-16s_test\test\T1-1_combined_R2.trim.fastq")

        Pause()
    End Sub
End Module

