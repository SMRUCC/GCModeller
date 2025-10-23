#Region "Microsoft.VisualBasic::da0fb94164e12ada8d21bfcbf950d1f3, core\Bio.Assembly\ContextModel\Promoter\Extensions.vb"

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

'   Total Lines: 83
'    Code Lines: 52 (62.65%)
' Comment Lines: 22 (26.51%)
'    - Xml Docs: 86.36%
' 
'   Blank Lines: 9 (10.84%)
'     File Size: 3.49 KB


'     Module Extensions
' 
'         Function: GetPrefixLengths, GetUpstreamSeq, headers, ParseUpstreamByLength
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace ContextModel.Promoter

    Public Module Extensions

        ''' <summary>
        ''' Read from <see cref="PrefixLength"/> members.
        ''' </summary>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetPrefixLengths() As IEnumerable(Of Integer)
            Return From L In GetType(PrefixLength).GetEnumValues Select CInt(L)
        End Function

        ''' <summary>
        ''' 解析出所有基因前面的序列片段
        ''' </summary>
        ''' <param name="context"></param>
        ''' <param name="nt"></param>
        ''' <param name="length%"></param>
        ''' <returns></returns>
        <Extension>
        Public Function ParseUpstreamByLength(context As PTT, nt As IPolymerSequenceModel, length%) As Dictionary(Of String, FastaSeq)
            Dim genes = context.GeneObjects
            Dim parser = From gene As GeneBrief
                         In genes.AsParallel
                         Let upstream = gene.GetUpstreamSeq(nt, length)
                         Select gene.Synonym,
                             promoter = upstream
            Dim table = parser.ToDictionary(Function(g) g.Synonym, Function(g) g.promoter)
            Return table
        End Function

        ''' <summary>
        ''' Get upstream nt sequence in a specific length for target gene.
        ''' (在这个函数之中，位点的计算的时候会有一个碱基的偏移量是因为为了不将起始密码子ATG之中的A包含在结果序列之中)
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="nt"></param>
        ''' <param name="len%"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function GetUpstreamSeq(gene As IGeneBrief, nt As IPolymerSequenceModel, len%) As FastaSeq
            Dim loci As NucleotideLocation = gene.Location

            With loci.Normalization()
                If .Strand = Strands.Forward Then
                    ' 正向序列是上游，无需额外处理
                    loci = New NucleotideLocation(.left - len, .left - 1)
                Else
                    ' 反向序列是下游，需要额外小心
                    loci = New NucleotideLocation(.right + 1, .right + len, ComplementStrand:=True)
                End If
            End With

            Dim site As SimpleSegment = nt.CutSequenceCircular(loci)
            Dim attrs$() = gene.headers(site)
            Dim promoter As New FastaSeq With {
                .Headers = attrs,
                .SequenceData = site.SequenceData
            }

            Return promoter
        End Function

        <Extension>
        Private Function headers(gene As IGeneBrief, site As SimpleSegment) As String()
            If gene.Product.StringEmpty Then
                Return {gene.Feature & " " & site.ID}
            Else
                Return {gene.Feature & " " & site.ID, gene.Product}
            End If
        End Function
    End Module
End Namespace
