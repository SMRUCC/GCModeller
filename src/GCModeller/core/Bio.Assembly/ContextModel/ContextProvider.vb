Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.ComponentModel
Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace ContextModel

    ''' <summary>
    ''' 基因组上下文计算工具，一般使用<see cref="PTT"/>或者<see cref="GFF"/>文件作为数据源.
    ''' 
    ''' ```vbnet
    ''' Dim PTT As <see cref="PTT"/> = TabularFormat.PTT.Load("G:\Xanthomonas_campestris_8004_uid15\CP000050.ptt")
    ''' Dim genome As New <see cref="GenomeContextProvider"/>(Of GeneBrief)(PTT)
    ''' Dim loci As New <see cref="NucleotideLocation"/>(3834400, 3834450) ' XC_3200, XC_3199, KEGG测试成功
    ''' Dim rels = genome.GetAroundRelated(loci, False)
    ''' 
    ''' rels = genome.GetAroundRelated(loci, True)
    ''' ```
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class GenomeContextProvider(Of T As IGeneBrief)

        ReadOnly _forwards As OrderSelector(Of IntTag(Of T))
        ReadOnly _reversed As OrderSelector(Of IntTag(Of T))

        Sub New(genome As IGenomicsContextProvider(Of T))
            Call Me.New(
                genome.GetStrandFeatures(Strands.Forward),
                genome.GetStrandFeatures(Strands.Reverse))
        End Sub

        Sub New(forwards As IEnumerable(Of T), reversed As IEnumerable(Of T))
            _forwards = IntTag(Of T).OrderSelector(forwards, Function(x) x.Location.Left, True)
            _reversed = IntTag(Of T).OrderSelector(reversed, Function(x) x.Location.Right, False)
        End Sub

        Sub New(source As T())
            Call Me.New(
                (From x As T In source Where x.Location.Strand = Strands.Forward Select x),
                (From x As T In source Where x.Location.Strand = Strands.Reverse Select x))
        End Sub

        ''' <summary>
        ''' 获取某一个指定的位点在基因组之中的内部反向的基因的集合
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <param name="Strand"></param>
        ''' <returns></returns>
        Public Function GetInnerAntisense(source As IEnumerable(Of T),
                                          LociStart As Integer,
                                          LociEnds As Integer,
                                          Strand As Strands) As T()

            Dim Raw As Relationship(Of T)() = GetRelatedGenes(source, LociStart, LociEnds, 0)
            Dim LQuery = (From obj In Raw
                          Where obj.Relation = SegmentRelationships.Inside AndAlso '只需要在内部并且和指定的链的方向反向的对象就可以了
                              (Strand <> obj.Gene.Location.Strand AndAlso
                              obj.Gene.Location.Strand <> Strands.Unknown)
                          Select obj.Gene).ToArray
            Return LQuery
        End Function

        ''' <summary>
        ''' Gets the related genes on a specific loci site location.(函数获取某一个给定的位点附近的所有的有关联的基因对象。
        ''' 请注意，这个函数仅仅是依靠于两个位点之间的相互位置关系来判断的，
        ''' 并没有判断链的方向，假若需要判断链的方向，请在调用本函数之前就将参数<paramref name="datasource"/>按照链的方向筛选出来)
        ''' </summary>
        ''' <param name="DataSource"></param>
        ''' <param name="LociStart"></param>
        ''' <param name="LociEnds"></param>
        ''' <param name="ATGDistance"></param>
        ''' <returns>请注意，函数所返回的列表之中包含有不同的关系！</returns>
        ''' <remarks></remarks>
        Public Function GetRelatedGenes(DataSource As IEnumerable(Of GeneBrief),
                                        LociStart As Integer,
                                        LociEnds As Integer,
                                        Optional ATGDistance As Integer = 500) As Relationship(Of GeneBrief)()

            Return GetRelatedGenes(DataSource, LociStart, LociEnds, ATGDistance)
        End Function

        ''' <summary>
        ''' Gets the related genes on a specific loci site location.(函数获取某一个给定的位点附近的所有的有关联的基因对象。
        ''' 请注意，这个函数仅仅是依靠于两个位点之间的相互位置关系来判断的，
        ''' 并没有判断链的方向，假若需要判断链的方向，请在调用本函数之前就将参数<paramref name="source"/>按照链的方向筛选出来)
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="start"></param>
        ''' <param name="ends"></param>
        ''' <param name="ATGDist"></param>
        ''' <returns>请注意，函数所返回的列表之中包含有不同的关系！</returns>
        ''' <remarks></remarks>
        Public Shared Function GetRelatedGenes(source As IEnumerable(Of T),
                                               start As Integer,
                                               ends As Integer,
                                               Optional ATGDist As Integer = 500,
                                               Optional stranded As Boolean = True) As Relationship(Of T)()

            Dim ntSite As New NucleotideLocation(start, ends, Strands.Unknown)
            Dim provider As New GenomeContextProvider(Of T)(source.ToArray)
            Dim relates As Relationship(Of T)() =
                provider.GetAroundRelated(ntSite, stranded, ATGDist)

            Return relates
        End Function

        ''' <summary>
        ''' <see cref="SegmentRelationships.UpStreamOverlap"/> and 
        ''' <see cref="SegmentRelationships.UpStream"/>
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="Loci"></param>
        ''' <param name="ATGDistance"></param>
        ''' <returns></returns>
        Public Shared Function GetRelatedUpstream(source As IEnumerable(Of T), Loci As NucleotideLocation, Optional ATGDistance As Integer = 2000) As Relationship(Of T)()
            Dim LociDelegate = New RelationDelegate(Of T) With {
                .dataSource = source,
                .loci = Loci.Normalization
            }
            Dim UpStreams As New KeyValuePair(Of SegmentRelationships, T())(
               SegmentRelationships.UpStream,
                LociDelegate.GetRelation(SegmentRelationships.UpStream, ATGDistance))
            Dim UpStreamOverlaps As New KeyValuePair(Of SegmentRelationships, T())(
               SegmentRelationships.UpStreamOverlap,
                LociDelegate.GetRelation(SegmentRelationships.UpStreamOverlap, ATGDistance))
            Dim array = {UpStreams, UpStreamOverlaps}
            Dim data0 = array.ToArray(Function(x) x.Value.ToArray(Function(g) New Relationship(Of T)(g, x.Key))).MatrixToList
            Return data0.ToArray
        End Function

        Public Shared Function GetRelatedGenes(source As IEnumerable(Of T),
                                               loci As NucleotideLocation,
                                               relation As SegmentRelationships,
                                               Optional dist As Integer = 500,
                                               Optional stranded As Boolean = True) As T()
            Dim provider As New GenomeContextProvider(Of T)(source.ToArray)
            Dim relates As Relationship(Of T)() =
                provider.GetAroundRelated(loci, stranded, dist)

            Return LinqAPI.Exec(Of T) <= From x As Relationship(Of T)
                                         In relates
                                         Where x.Relation = relation
                                         Select x.Gene
        End Function

        ''' <summary>
        ''' Gets the stranded gene object data source.
        ''' </summary>
        ''' <param name="strand"></param>
        ''' <returns></returns>
        Public Function GetSource(strand As Strands) As OrderSelector(Of IntTag(Of T))
            If strand = Strands.Forward Then
                Return _forwards
            ElseIf strand = Strands.Reverse Then
                Return _reversed
            Else
                Throw New NotImplementedException(strand.ToString & " " & GetType(Strands).FullName)
            End If
        End Function

        ''' <summary>
        ''' Creates the anonymous function pointer for the relationship <see cref="GetAroundRelated"/>
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="stranded"></param>
        ''' <returns></returns>
        Private Function __delegate(loci As NucleotideLocation, stranded As Boolean) As Func(Of Strands, Integer, T())
            Dim strand As Strands = loci.Strand

            If stranded Then
                Return AddressOf New RelationDelegate(Of T) With {
                    .dataSource = GetSource(strand),
                    .loci = loci.Normalization
                }.GetRelation
            Else
                Dim asc As New RelationDelegate(Of T) With {
                    .dataSource = _forwards,
                    .loci = loci.Normalization
                }
                Dim desc As New RelationDelegate(Of T) With {
                    .dataSource = _reversed,
                    .loci = loci
                }
                Return Function(relType, dist) desc.GetRelation(relType, dist) + (MakeList(Of T)() <= asc.GetRelation(relType, dist))
            End If
        End Function

        ''' <summary>
        ''' Gets the related genes on a specific loci site location.(函数获取某一个给定的位点附近的所有的有关联的基因对象。
        ''' 请注意，这个函数仅仅是依靠于两个位点之间的相互位置关系来判断的，
        ''' 假若这个参数<param name="stranded"></param>为真，假若需要判断链的方向)
        ''' </summary>
        ''' <param name="lociDist"></param>
        ''' <returns>请注意，函数所返回的列表之中包含有不同的关系！</returns>
        ''' <remarks></remarks>
        Public Function GetAroundRelated(loci As NucleotideLocation, Optional stranded As Boolean = True, Optional lociDist As Integer = 500) As Relationship(Of T)()
            Dim GetRelation = __delegate(loci, stranded)
            Dim foundTEMP As T()
            Dim lstRelated As New List(Of Relationship(Of T))

            foundTEMP = GetRelation(SegmentRelationships.UpStream, lociDist)  ' 获取ATG距离小于阈值的所有基因
            'foundTEMP = (From gene As T
            '             In foundTEMP
            '             Where Math.Abs(GetATGDistance(loci, gene)) <= lociDist
            '             Select gene).ToArray

            If Not foundTEMP.IsNullOrEmpty Then
                lstRelated += From gene As T
                              In foundTEMP
                              Select New Relationship(Of T)(gene, SegmentRelationships.UpStream)
            End If

            For Each relationShip In New SegmentRelationships() {
 _
               SegmentRelationships.Equals,
               SegmentRelationships.Inside,
               SegmentRelationships.DownStreamOverlap,
               SegmentRelationships.UpStreamOverlap,
               SegmentRelationships.Cover
            }

                foundTEMP = GetRelation(relationShip, 3 * lociDist)

                If Not foundTEMP.IsNullOrEmpty Then
                    lstRelated += From gene As T
                                  In foundTEMP
                                  Select New Relationship(Of T)(gene, relationShip)
                End If
            Next

            Dim DownStreamGenes = GetRelation(SegmentRelationships.DownStream, 3 * lociDist)
            Dim Dwsrt As T() =
                LinqAPI.Exec(Of T) <= From gene As T
                                      In DownStreamGenes
                                      Let Distance As Integer = LocationDescriptions.ATGDistance(gene, loci)
                                      Where Distance <= lociDist
                                      Select gene

            If Not Dwsrt.IsNullOrEmpty Then
                lstRelated += From gene As T
                              In foundTEMP
                              Select New Relationship(Of T)(gene, SegmentRelationships.DownStream)
            End If

            Return lstRelated.ToArray  '只返回下游的第一个基因
        End Function
    End Class
End Namespace