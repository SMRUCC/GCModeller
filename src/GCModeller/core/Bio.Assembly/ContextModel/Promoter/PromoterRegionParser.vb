#Region "Microsoft.VisualBasic::014db1ced01aebe645e2ae7e3a2fe1d3, GCModeller\core\Bio.Assembly\ContextModel\Promoter\PromoterRegionParser.vb"

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

    '   Total Lines: 109
    '    Code Lines: 70
    ' Comment Lines: 23
    '   Blank Lines: 16
    '     File Size: 4.48 KB


    '     Class PromoterRegionParser
    ' 
    '         Properties: PrefixLengths, PromoterRegions
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: ContainsLength, GetRegionCollectionByLength, (+2 Overloads) GetSequenceById
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace ContextModel.Promoter

    ''' <summary>
    ''' 直接从基因的启动子区选取序列数据以及外加操纵子的第一个基因的启动子序列
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    <Package("Parser.Gene.Promoter", Publisher:="xie.guigang@gmail.com")>
    Public Class PromoterRegionParser

        Public Shared ReadOnly Property PrefixLengths As IReadOnlyList(Of Integer) = GetPrefixLengths().ToArray

        Public ReadOnly Property PromoterRegions As IntegerTagged(Of Dictionary(Of String, FastaSeq))()
        ReadOnly lengthIndex As New Index(Of Integer)(PrefixLengths)

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="nt">全基因组序列</param>
        ''' <remarks></remarks>
        Sub New(nt As FastaSeq, PTT As PTT)
            Dim genome As IPolymerSequenceModel = nt
            Dim regions(PrefixLengths.GetLength - 1) As IntegerTagged(Of Dictionary(Of String, FastaSeq))
            Dim i As i32 = 0

            genome.SequenceData = genome.SequenceData.ToUpper

            For Each l% In PrefixLengths
                regions(++i) = New IntegerTagged(Of Dictionary(Of String, FastaSeq)) With {
                    .Tag = l,
                    .Value = PTT.ParseUpstreamByLength(genome, l)
                }
            Next

            PromoterRegions = regions
        End Sub

        Sub New(gbff As GBFF.File)
            Call Me.New(gbff.Origin.ToFasta, gbff.GbffToPTT(ORF:=True))
        End Sub

        Sub New(genome As PTTDbLoader)
            Call Me.New(genome.GenomeFasta, genome.ORF_PTT)
        End Sub

        Public Function GetRegionCollectionByLength(l%) As Dictionary(Of String, FastaSeq)
            Dim i% = lengthIndex.IndexOf(l)
            Return Me.PromoterRegions(i%)
        End Function

        Public Function GetSequenceById(geneIDs As IEnumerable(Of String), <Parameter("Len")> length%) As FastaFile
            Return GetSequenceById(Me, geneIDs, length)
        End Function

        Shared ReadOnly default150 As [Default](Of  Integer) = 150.AsDefault(Function(value) Not ContainsLength(length:=DirectCast(value, Integer)))

        ''' <summary>
        ''' Get parsed sequence by a given id list.
        ''' </summary>
        ''' <param name="parser"></param>
        ''' <param name="geneIDs"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Get.Sequence.By.Id")>
        Public Shared Function GetSequenceById(parser As PromoterRegionParser,
                                               geneIDs As IEnumerable(Of String),
                                               Optional length As PrefixLength = PrefixLength.L150) As FastaFile
            With geneIDs.Indexing
                Dim query = From fasta
                            In parser.GetRegionCollectionByLength(length Or default150)
                            Where .IndexOf(fasta.Key) > -1
                            Select fasta.Value
                Dim out As New FastaFile(query)
                Return out
            End With
        End Function

        ''' <summary>
        ''' If <paramref name="length"/> is 160, then it will be invalid, check if the input length is valids?
        ''' </summary>
        ''' <param name="length"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function ContainsLength(length As Integer) As Boolean
            If Array.IndexOf(PrefixLengths, length) = -1 Then
                Call $"The promoter region length {length} is not valid, using default value is 150bp.".__DEBUG_ECHO
            Else
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
