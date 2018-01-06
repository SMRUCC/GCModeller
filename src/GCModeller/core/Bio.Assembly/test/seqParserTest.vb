#Region "Microsoft.VisualBasic::32106ab4b081774cc6f5933c1fe741b0, ..\GCModeller\core\Bio.Assembly\test\seqParserTest.vb"

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

Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel

Module seqParserTest

    Sub Main()
        Dim gb As GBFF.File = GBFF.File.Load("J:\XCC\assembly\GCA_001705565.1_ASM170556v1\GCA_001705565.1_ASM170556v1_genomic.gbff")
        Dim nt = gb.Origin.ToFasta
        Dim genes = gb.GbffToPTT.GeneObjects.OrderBy(Function(g) g.Location.Left).ToArray

        For i As Integer = 0 To 9
            Dim gene = genes(i)
            Dim seq = nt.CutSequenceCircular(gene.Location - 50)
            Dim fa = seq.SimpleFasta(gene.Synonym)

            Call fa.SaveTo($"./{i}.fasta")
        Next
    End Sub
End Module

