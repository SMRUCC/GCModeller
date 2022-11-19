#Region "Microsoft.VisualBasic::9a41d2cc3c010bdbbe2138aa648a327a, GCModeller\core\Bio.Assembly\SequenceModel\NucleicAcid\Objects\Contig.vb"

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

    '   Total Lines: 90
    '    Code Lines: 60
    ' Comment Lines: 16
    '   Blank Lines: 14
    '     File Size: 3.63 KB


    '     Class Contig
    ' 
    '         Properties: Location, MappingLocation
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetRelatedGenes, GetRelatedUpstream, GetsATGDist, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat
Imports SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.ContextModel

Namespace SequenceModel.NucleotideModels

    ''' <summary>
    ''' 这个基础的模型对象只有在基因组上面的位置信息
    ''' </summary>
    Public MustInherit Class Contig
        Implements IContig

        Protected _MappingLocation As NucleotideLocation

        ''' <summary>
        ''' 在参考基因组上面的Mapping得到的位置，假若需要修改位置，假若害怕影响到原有的数据的话，则请复写这个属性然后使用复制的方法得到新的位点数据
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property MappingLocation(Optional reset As Boolean = False) As NucleotideLocation
            Get
                If reset Then
                    _MappingLocation = Nothing
                End If

                If _MappingLocation Is Nothing Then
                    _MappingLocation = __getMappingLoci()
                End If
                Return _MappingLocation
            End Get
        End Property

        Private Property Location As NucleotideLocation Implements IContig.Location
            Get
                Return _MappingLocation
            End Get
            Set(value As NucleotideLocation)
                _MappingLocation = value
            End Set
        End Property

        Protected MustOverride Function __getMappingLoci() As NucleotideLocation

        Public Overrides Function ToString() As String
            Return MappingLocation.ToString
        End Function

        Sub New()
        End Sub

        ''' <summary>
        ''' 这个构造函数已经使用<see cref="NucleotideLocation.Copy()"/>函数从数据源<paramref name="mappinglocation"/>进行复制
        ''' </summary>
        ''' <param name="MappingLocation"></param>
        Protected Sub New(MappingLocation As NucleotideLocation)
            Me._MappingLocation = MappingLocation.Copy
        End Sub

        Public Function GetRelatedGenes(PTT As PTT, loc As SegmentRelationships, Optional atgDist As Integer = 500) As GeneBrief()
            Dim found As Relationship(Of GeneBrief)() =
                PTT.GetRelatedGenes(MappingLocation, True)
            Dim gets As GeneBrief() =
                LinqAPI.Exec(Of GeneBrief) <= From x As Relationship(Of GeneBrief)
                                              In found
                                              Where loc.HasFlag(x.Relation)
                                              Select x.Gene
            Return gets
        End Function

        ''' <summary>
        ''' 会同时将上游以及上游重叠的基因都找出来
        ''' </summary>
        ''' <param name="PTT"></param>
        ''' <returns></returns>
        Public Function GetRelatedUpstream(PTT As PTT, ATGDist As Integer) As GeneBrief()
            Dim rel As SegmentRelationships = SegmentRelationships.UpStream + SegmentRelationships.UpStreamOverlap
            Return GetRelatedGenes(PTT, rel, ATGDist)
        End Function

        Public Function GetsATGDist(Gene As GeneBrief) As Integer
            Return GetATGDistance(MappingLocation, Gene)
        End Function

        Public Overloads Shared Narrowing Operator CType(contig As Contig) As NucleotideLocation
            Return contig.MappingLocation
        End Operator
    End Class
End Namespace
