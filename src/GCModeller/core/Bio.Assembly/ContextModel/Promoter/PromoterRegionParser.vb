#Region "Microsoft.VisualBasic::744aef6eb5f85d0da573832c16abebef, ..\GCModeller\core\Bio.Assembly\ContextModel\PromoterRegionParser.vb"

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

        Public Shared ReadOnly Property PrefixLength As IReadOnlyList(Of Integer) = {100, 150, 200, 250, 300, 400, 500}

        Public ReadOnly Property PromoterRegions As IntegerTagged(Of Dictionary(Of String, FastaSeq))()
        ReadOnly lengthIndex As New Index(Of Integer)(PrefixLength)

        ''' <summary>
        ''' 基因组的Fasta核酸序列
        ''' </summary>
        ''' <param name="nt">全基因组序列</param>
        ''' <remarks></remarks>
        Sub New(nt As FastaSeq, PTT As PTT)
            Dim genome As IPolymerSequenceModel = nt
            Dim regions(PrefixLength.GetLength - 1) As IntegerTagged(Of Dictionary(Of String, FastaSeq))
            Dim i As int = 0

            genome.SequenceData = genome.SequenceData.ToUpper

            For Each l% In PrefixLength
                regions(++i) = New IntegerTagged(Of Dictionary(Of String, FastaSeq)) With {
                    .Tag = l,
                    .Value = CreateObject(l, PTT, genome)
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

        ''' <summary>
        ''' 解析出所有基因前面的序列片段
        ''' </summary>
        ''' <param name="Length"></param>
        ''' <param name="PTT"></param>
        ''' <param name="nt"></param>
        ''' <returns></returns>
        Private Shared Function CreateObject(length As Integer, PTT As PTT, nt As IPolymerSequenceModel) As Dictionary(Of String, FASTA.FastaSeq)
            Dim parser = From gene As ComponentModels.GeneBrief
                         In PTT.GeneObjects.AsParallel
                         Let upstream = gene.GetUpstreamSeq(nt, length)
                         Select gene.Synonym,
                             promoter = upstream
            Dim table = parser.ToDictionary(Function(g) g.Synonym, Function(g) g.promoter)
            Return table
        End Function

        Public Function GetRegionCollectionByLength(l%) As Dictionary(Of String, FastaSeq)
            Dim i As Integer = Me.lengthIndex.IndexOf(l)
            Return Me.PromoterRegions(i%)
        End Function

        Public Function GetSequenceById(lstId As IEnumerable(Of String), <Parameter("Len")> Length As Integer) As FASTA.FastaFile
            Return GetSequenceById(Me, lstId, Length)
        End Function

        Shared ReadOnly default150 As DefaultValue(Of Integer) = 150.AsDefault(Function(value) Not ContainsLength(length:=DirectCast(value, Integer)))

        ''' <summary>
        ''' Get parsed sequence by a given id list.
        ''' </summary>
        ''' <param name="parser"></param>
        ''' <param name="geneIDs"></param>
        ''' <param name="length"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExportAPI("Get.Sequence.By.Id")>
        Public Shared Function GetSequenceById(parser As PromoterRegionParser, geneIDs As IEnumerable(Of String), Optional length% = 150) As FastaFile
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
            If Array.IndexOf(PrefixLength, length) = -1 Then
                Call $"The promoter region length {length} is not valid, using default value is 150bp.".__DEBUG_ECHO
            Else
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
