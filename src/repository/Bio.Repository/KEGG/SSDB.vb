#Region "Microsoft.VisualBasic::cb97d55691fe4bc0fd4a319cef833fdf, Bio.Repository\KEGG\SSDB.vb"

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

    '   Total Lines: 70
    '    Code Lines: 44 (62.86%)
    ' Comment Lines: 14 (20.00%)
    '    - Xml Docs: 64.29%
    ' 
    '   Blank Lines: 12 (17.14%)
    '     File Size: 2.69 KB


    ' Module SSDB
    ' 
    '     Sub: CutSequence_Upstream
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module SSDB

    ''' <summary>
    ''' 这个API只适合小批量数据的获取
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="geneIDs"></param>
    ''' <param name="len%"></param>
    ''' <param name="save$"></param>
    ''' <param name="code$"></param>
    ''' <param name="[overrides]"></param>
    <Extension>
    Public Sub CutSequence_Upstream(genome As PTT, geneIDs As IEnumerable(Of String), len%, save$, code$, Optional [overrides] As Boolean = False)
        Dim genes = genome.ToDictionary
        Dim cuts As New FastaFile(save, throwEx:=False)
        Dim titles As New Index(Of String)(cuts.Select(Function(f) f.Title))

        Using write As IO.StreamWriter = save.OpenWriter(Encodings.ASCII)
            For Each fa In cuts
                Call write.WriteLine(fa.GenerateDocument(60))
            Next

            For Each id$ In geneIDs
                If Not genes.ContainsKey(id) Then
                    Continue For
                End If

                Dim loci = genes(id).Location
                Dim region As NucleotideLocation

                With loci.Normalization

                    ' 必须要偏移一个碱基，否则ATG之中的A就会被添加进来了，做motif的时候这个总是会出现的A会导致会产生不存在的motif
                    If loci.Strand = Strands.Reverse Then
                        region = New NucleotideLocation(.Right + 1, .Right + len, .Strand) ' ATG 向右平移
                    Else
                        region = New NucleotideLocation(.Left - len, .Left - 1, .Strand)
                    End If
                End With

                Dim title$ = id & " " & region.ToString

                If titles(title) > -1 AndAlso Not [overrides] Then
                    Call $"Skip existed {title}...".debug
                    Continue For
                End If

                Dim seq As FastaSeq '=
                'SMRUCC.genomics.Assembly.KEGG.DBG.API.CutSequence(
                'region,
                'org:=code,
                'vector:=loci.Strand)

                seq.Headers = {title}

                Call write.WriteLine(seq.GenerateDocument(60))
                Call Thread.Sleep(1500)
            Next
        End Using
    End Sub
End Module
