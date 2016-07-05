#Region "Microsoft.VisualBasic::e8b849d3824fa270b4d76193fd6361c4, ..\GCModeller\core\Bio.Assembly\ContextModel\Descriptions.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
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
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel
Imports SMRUCC.genomics.ComponentModel.Loci
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic

Namespace ContextModel

    Public Module LocationDescriptions

        ''' <summary>
        ''' 判断本对象是否是由<see cref="BlankSegment"></see>方法所生成的空白片段
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function IsBlankSegment(Gene As IGeneBrief) As Boolean
            If Gene Is Nothing Then
                Return True
            End If
            Return String.Equals(Gene.Identifier, BLANK_VALUE) OrElse
                Gene.Location.FragmentSize = 0
        End Function

        Public Const BLANK_VALUE As String = "Blank"

        <Extension> Public Function BlankSegment(Of T As IGeneBrief)(Location As NucleotideLocation) As T
            Dim BlankData = Activator.CreateInstance(Of T)()
            BlankData.COG = BLANK_VALUE
            BlankData.Product = BLANK_VALUE
            BlankData.Identifier = BLANK_VALUE
            BlankData.Length = Location.FragmentSize
            BlankData.Location = Location
            Return BlankData
        End Function

        ''' <summary>
        ''' Get the loci relationship between the target gene and the specific feature loci.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="gene"></param>
        ''' <param name="nucl"></param>
        ''' <returns></returns>
        Public Function GetLociRelations(Of T As IGeneBrief)(gene As T, nucl As NucleotideLocation) As SegmentRelationships
            Dim r As SegmentRelationships = gene.Location.GetRelationship(nucl)

            If r = SegmentRelationships.DownStream AndAlso
                gene.Location.Strand = Strands.Reverse Then
                Return SegmentRelationships.UpStream  '反向的基因需要被特别注意，当目标片段处于下游的时候，该下游片段可能为该基因的启动子区

            ElseIf r = SegmentRelationships.UpStream AndAlso
                gene.Location.Strand = Strands.Reverse Then
                Return SegmentRelationships.DownStream

            ElseIf r = SegmentRelationships.UpStreamOverlap AndAlso
                gene.Location.Strand = Strands.Reverse Then
                Return SegmentRelationships.DownStreamOverlap

            ElseIf r = SegmentRelationships.DownStreamOverlap AndAlso
                gene.Location.Strand = Strands.Reverse Then
                Return SegmentRelationships.UpStreamOverlap

            Else
                Return r
            End If
        End Function

        ''' <summary>
        ''' Calculate the ATG distance between the target gene and the specific feature loci.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="gene"></param>
        ''' <param name="nucl"></param>
        ''' <returns></returns>
        Public Function ATGDistance(Of T As IGeneBrief)(gene As T, nucl As NucleotideLocation) As Integer
            Call nucl.Normalization()
            Call gene.Location.Normalization()

            If gene.Location.Strand = Strands.Forward Then
                Return Math.Abs(gene.Location.Right - nucl.Left)
            Else
                Return Math.Abs(gene.Location.Left - nucl.Right)
            End If
        End Function

        ''' <summary>
        ''' Calculates the ATG distance between the target gene and a loci segment on.(计算位点相对于某一个基因的ATG距离)
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="gene"></param>
        ''' <returns>总是计算最大的距离</returns>
        ''' <remarks></remarks>
        <Extension> Public Function GetATGDistance(loci As Location, gene As IGeneBrief) As Integer
            Call loci.Normalization()
            Call gene.Location.Normalization()

            If gene.Location.Strand = Strands.Forward Then '直接和左边相减
                Return loci.Right - gene.Location.Left
            ElseIf gene.Location.Strand = Strands.Reverse Then  '互补链方向的基因，则应该减去右边
                Return gene.Location.Right - loci.Left
            Else
                Return loci.Left - gene.Location.Left
            End If
        End Function

        Const Intergenic As String = "Intergenic region"
        Const DownStream As String = "In the downstream of [{0}] gene ORF."
        Const IsORF As String = "Is [{0}] gene ORF."
        Const Inside As String = "Inside the [{0}] gene ORF."
        Const OverloapsDownStream As String = "Overlap on down_stream with [{0}] gene ORF."
        Const OverlapsUpStream As String = "Overlap on up_stream with [{0}] gene ORF."
        Const PromoterRegion As String = "In the promoter region of [{0}] gene ORF."

        ''' <summary>
        ''' Gets the loci location description data.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="posi"></param>
        ''' <param name="data"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Extension> Public Function LocationDescription(Of T As IGeneBrief)(posi As SegmentRelationships, data As T) As String
            If IsBlankSegment(data) Then
                Return Intergenic

            ElseIf posi = SegmentRelationships.DownStream Then
                Return String.Format(DownStream, data.Identifier)
            ElseIf posi = SegmentRelationships.Equals Then
                Return String.Format(IsORF, data.Identifier)
            ElseIf posi = SegmentRelationships.Inside Then
                Return String.Format(Inside, data.Identifier)
            ElseIf posi = SegmentRelationships.DownStreamOverlap Then
                Return String.Format(OverloapsDownStream, data.Identifier)
            ElseIf posi = SegmentRelationships.UpStreamOverlap Then
                Return String.Format(OverlapsUpStream, data.Identifier)
            Else
                Return String.Format(PromoterRegion, data.Identifier)
            End If
        End Function
    End Module
End Namespace
