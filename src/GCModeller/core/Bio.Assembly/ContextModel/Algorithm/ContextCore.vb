#Region "Microsoft.VisualBasic::f699d44a77c5d632cb7099cd3e68a8b6, GCModeller\core\Bio.Assembly\ContextModel\Algorithm\ContextCore.vb"

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

    '   Total Lines: 72
    '    Code Lines: 48
    ' Comment Lines: 17
    '   Blank Lines: 7
    '     File Size: 3.13 KB


    '     Structure RelationDelegate
    ' 
    '         Function: GetATGRelation, GetRelation
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.Loci
Imports stdNum = System.Math

Namespace ContextModel

    ''' <summary>
    ''' The working core of the genomics context provider.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Structure RelationDelegate(Of T As IGeneBrief)

        Dim dataSource As OrderSelector(Of IntTag(Of T))
        Dim loci As NucleotideLocation
        Dim parallel As Boolean

        ''' <summary>
        ''' 为了提高上下文的搜索效率，只在附近的位置搜索
        ''' 对于正向，是从小到大排序的
        ''' </summary>
        ''' <param name="relType"></param>
        ''' <param name="dist"></param>
        ''' <returns></returns>
        Public Function GetRelation(relType As SegmentRelationships, dist As Integer) As T()
            Dim loci As NucleotideLocation = Me.loci
            Dim result As IEnumerable(Of IntTag(Of T)) = Nothing

            If Not dataSource.Desc Then ' 目标序列是升序排序的
                Dim n As Integer = loci.Right + dist
                result = dataSource.SelectUntilGreaterThan(n)
            Else ' 目标序列是降序排序的
                Dim n As Integer = loci.Left - dist
                result = dataSource.SelectUntilLessThan(n)
            End If

            Dim genes As T() =
                If(parallel,
                (LinqAPI.Exec(Of T) <= From gene As T
                                       In result.Select(Function(x) x.x).AsParallel
                                       Let Relation As SegmentRelationships =
                                           GetLociRelations(gene, loci)
                                       Where Relation = relType
                                       Select gene),
                 LinqAPI.Exec(Of T) <= From gene As T
                                       In result.Select(Function(x) x.x)
                                       Let Relation As SegmentRelationships =
                                           GetLociRelations(gene, loci)
                                       Where Relation = relType
                                       Select gene)
            Return genes
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="relType"></param>
        ''' <param name="ATGdist"></param>
        ''' <returns></returns>
        Public Function GetATGRelation(relType As SegmentRelationships, ATGdist As Integer) As T()
            Dim genes As T() = GetRelation(relType, ATGdist)
            Dim loci As NucleotideLocation = Me.loci
            Dim LQuery As T() =
                LinqAPI.Exec(Of T) <= From gene As T
                                      In genes
                                      Where stdNum.Abs(GetATGDistance(loci, gene)) <= ATGdist
                                      Select gene '获取ATG距离小于阈值的所有基因
            Return LQuery
        End Function
    End Structure
End Namespace
