Imports LANS.SystemsBiology.Assembly.NCBI
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports LANS.SystemsBiology.ComponentModel.Loci.NucleotideLocation
Imports LANS.SystemsBiology.ContextModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions

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
        ''' 这个构造函数已经使用<see cref="ComponentModel.Loci.NucleotideLocation.Copy()"/>函数从数据源<paramref name="mappinglocation"/>进行复制
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