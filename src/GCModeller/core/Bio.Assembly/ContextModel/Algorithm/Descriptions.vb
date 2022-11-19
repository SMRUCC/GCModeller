#Region "Microsoft.VisualBasic::7c1f93ca1b225257d2e34d2d6519da98, GCModeller\core\Bio.Assembly\ContextModel\Algorithm\Descriptions.vb"

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

    '   Total Lines: 149
    '    Code Lines: 89
    ' Comment Lines: 41
    '   Blank Lines: 19
    '     File Size: 6.55 KB


    '     Module LocationDescriptions
    ' 
    '         Function: ATGDistance, BlankSegment, GetATGDistance, (+2 Overloads) GetLociRelations, IsBlankSegment
    '                   LocationDescription
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports stdNum = System.Math

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
            Return String.Equals(Gene.Key, BLANK_VALUE) OrElse Gene.Location.FragmentSize = 0
        End Function

        Public Const BLANK_VALUE As String = "Blank"

        <Extension> Public Function BlankSegment(Of T As IGeneBrief)(Location As NucleotideLocation) As T
            Dim BlankData = Activator.CreateInstance(Of T)()
            BlankData.Feature = BLANK_VALUE
            BlankData.Product = BLANK_VALUE
            BlankData.Key = BLANK_VALUE
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
            Return GetLociRelations(gene.Location, nucl)
        End Function

        ''' <summary>
        ''' Get the loci relationship between the target gene and the specific feature loci.
        ''' </summary>
        ''' <param name="gene"></param>
        ''' <param name="nucl"></param>
        ''' <returns></returns>
        Public Function GetLociRelations(gene As NucleotideLocation, nucl As NucleotideLocation) As SegmentRelationships
            Dim r As SegmentRelationships = gene.GetRelationship(nucl)

            If r = SegmentRelationships.DownStream AndAlso
                gene.Strand = Strands.Reverse Then
                ' 反向的基因需要被特别注意，当目标片段处于下游的时候，该下游片段可能为该基因的启动子区
                Return SegmentRelationships.UpStream

            ElseIf r = SegmentRelationships.UpStream AndAlso
                gene.Strand = Strands.Reverse Then
                Return SegmentRelationships.DownStream

            ElseIf r = SegmentRelationships.UpStreamOverlap AndAlso
                gene.Strand = Strands.Reverse Then
                Return SegmentRelationships.DownStreamOverlap

            ElseIf r = SegmentRelationships.DownStreamOverlap AndAlso
                gene.Strand = Strands.Reverse Then
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
                Return stdNum.Abs(gene.Location.right - nucl.left)
            Else
                Return stdNum.Abs(gene.Location.left - nucl.right)
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
                Return loci.right - gene.Location.left
            ElseIf gene.Location.Strand = Strands.Reverse Then  '互补链方向的基因，则应该减去右边
                Return gene.Location.right - loci.left
            Else
                Return loci.left - gene.Location.left
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
                Return String.Format(DownStream, data.Key)
            ElseIf posi = SegmentRelationships.Equals Then
                Return String.Format(IsORF, data.Key)
            ElseIf posi = SegmentRelationships.Inside Then
                Return String.Format(Inside, data.Key)
            ElseIf posi = SegmentRelationships.DownStreamOverlap Then
                Return String.Format(OverloapsDownStream, data.Key)
            ElseIf posi = SegmentRelationships.UpStreamOverlap Then
                Return String.Format(OverlapsUpStream, data.Key)
            Else
                Return String.Format(PromoterRegion, data.Key)
            End If
        End Function
    End Module
End Namespace
