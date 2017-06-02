Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.Metagenome.gast

''' <summary>
''' ``Rank Abundance``曲线
''' </summary>
Public Module RankAbundance

    ''' <summary>
    ''' ``Rank-abundance``曲线是分析多样性的一种方式。构建方法是统计单一样品中，每一个OTU 所含的序列数，
    ''' 将OTUs 按丰度（所含有的序列条数）由大到小等级排序，再以OTU 等级为横坐标，以每个OTU 中所含的序列
    ''' 数（也可用OTU 中序列数的相对百分含量）为纵坐标做图。
    ''' ``Rank-abundance``曲线可用来解释多样性的两个方面，即物种丰度和物种均匀度。在水平方向，物种的丰度
    ''' 由曲线的宽度来反映，物种的丰度越高，曲线在横轴上的范围越大；曲线的形状（平滑程度）反映了样品中物种
    ''' 的均度，曲线越平缓，物种分布越均匀。
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' > Scott T Bates, Jose C Clemente, et al. Global biogeography of highly diverse protistan communities in soil. The ISME Journal (2013) 7, 652–659; doi:10.1038/ismej.2012.147.
    ''' </remarks>
    <Extension> Public Function Plot(otus As IEnumerable(Of Names))

    End Function
End Module
