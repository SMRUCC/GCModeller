Imports LANS.SystemsBiology.ComponentModel.Loci
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.ComponentModel.Loci.Abstract
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.ComponentModels
Imports LANS.SystemsBiology.Toolkits.RNA_Seq.BOW.DocumentFormat.SAM.DocumentElements
Imports LANS.SystemsBiology.ContextModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language
Imports LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat

<[Namespace]("TSSs.Analysis.ReadsView")>
Public Module Reads

    Public Class ReadsGroupView : Implements LANS.SystemsBiology.ComponentModel.Loci.Abstract.ILocationNucleotideSegment

        Dim _FLAG As Integer

        Public Property FLAG As Integer
            Get
                Return _FLAG
            End Get
            Set(value As Integer)
                _FLAG = value
                _BitwiseFLAGS = ComputeBitFLAGS(value)
            End Set
        End Property
        Public Property CIGAR As String
        Public Property MAPQ As Integer
        Public Property POS As Integer
        Public Property PNEXT As Integer
        Public Property NumberOfReads As Integer

        Public ReadOnly Property BitwiseFLAGS As BitFLAGS()

        Public ReadOnly Property BitwiseFLAGSDescription As String
            Get
                Return GetBitFLAGDescriptions(Me.BitwiseFLAGS)
            End Get
        End Property

        Public Function Copy() As ReadsGroupView
            Return New ReadsGroupView With {.CIGAR = Me.CIGAR,
               .FLAG = Me.FLAG,
               .MAPQ = Me.MAPQ,
               .NumberOfReads = Me.NumberOfReads,
               .PNEXT = Me.PNEXT,
               .POS = Me.POS}
        End Function

        Public Property AssociatedGene As String
        Public Property GeneLocation As String
        Public Property Position As String

        Public ReadOnly Property Strand As Strands Implements ILocationNucleotideSegment.Strand
            Get
                If Array.IndexOf(Me.BitwiseFLAGS, BitFLAGS.Bit0x10) > -1 Then
                    Return Strands.Reverse
                ElseIf Array.IndexOf(Me.BitwiseFLAGS, BitFLAGS.Bit0x4) > -1 Then
                    Return Strands.Unknown
                Else
                    Return Strands.Forward
                End If
            End Get
        End Property

        Public ReadOnly Property UniqueId As String Implements ILocationSegment.UniqueId
            Get
                Return $"{POS}>>{PNEXT}({FLAG})"
            End Get
        End Property

        Public ReadOnly Property Location As Location Implements ILocationSegment.Location
            Get
                Return New NucleotideLocation(POS, PNEXT, Me.Strand)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{POS}  >>  { Me.BitwiseFLAGSDescription }"
        End Function
    End Class

    Public Class GeneAssociationView
        Public Property FLAG As Integer
        Public Property CIGAR As String
        Public Property MAPQ As String
        Public Property POS As Integer
        Public Property PNEXT As Integer
        Public Property NumberOfReads As Integer
        Public Property AssociatedGene As String
        <Column("Position")> Public Property getPosition As String

        Public Overrides Function ToString() As String
            Return $"{AssociatedGene}/  {getPosition}"
        End Function

        Public ReadOnly Property Position As LANS.SystemsBiology.ComponentModel.Loci.SegmentRelationships
            Get
                Select Case getPosition
                    Case "", SegmentRelationships.Blank.ToString
                        Return SegmentRelationships.Blank
                    Case SegmentRelationships.Cover.ToString
                        Return SegmentRelationships.Cover
                    Case SegmentRelationships.DownStream.ToString
                        Return SegmentRelationships.DownStream
                    Case SegmentRelationships.DownStreamOverlap.ToString
                        Return SegmentRelationships.DownStreamOverlap
                    Case SegmentRelationships.Equals.ToString
                        Return SegmentRelationships.Equals
                    Case SegmentRelationships.Inside.ToString
                        Return SegmentRelationships.Inside
                    Case SegmentRelationships.UpStream.ToString
                        Return SegmentRelationships.UpStream
                    Case SegmentRelationships.UpStreamOverlap.ToString
                        Return SegmentRelationships.UpStreamOverlap
                    Case Else
                        Return SegmentRelationships.Blank
                End Select
            End Get
        End Property
    End Class

    <ExportAPI("Read.Csv.GeneAssociation")>
    Public Function LoadGeneAssiciation(CSv As String) As GeneAssociationView()
        Return CSv.LoadCsv(Of GeneAssociationView)(False).ToArray
    End Function

    <ExportAPI("Read.Csv.ReadsView")>
    Public Function LoadReadsView(CSV As String) As ReadsGroupView()
        Return CSV.LoadCsv(Of ReadsGroupView)(False).ToArray
    End Function

    <ExportAPI("Join", Info:="Merge two reads view data file.")>
    Public Function Join(a As String, b As String) As ReadsGroupView()
        Dim Csv = a.LoadCsv(Of ReadsGroupView)(False)
        Call Csv.AddRange(b.LoadCsv(Of ReadsGroupView)(False))
        Dim GroupLQuery = (From item In Csv Select item Group item By item.POS Into Group).ToArray      '进行Group操作
        Csv = (From item In GroupLQuery Select Join(item.Group.ToArray)).ToList
        Return Csv.ToArray
    End Function

    Private Function Join(data As ReadsGroupView()) As ReadsGroupView
        If data.Count = 1 Then
            Return data.First
        End If

        Dim LQuery = (From obj In data Select obj.NumberOfReads).Sum
        Dim out = data.First
        out.NumberOfReads = LQuery
        Return out
    End Function

    <ExportAPI("Write.Csv.ReadsView")>
    Public Function Save(data As Generic.IEnumerable(Of ReadsGroupView), SaveTo As String) As Boolean
        Return data.SaveTo(SaveTo, False)
    End Function

    ''' <summary>
    ''' If the distance of two TSS positions differed by less than 3 (Default <param name="offset"></param> is 3 bp) nt, they were treated as a single TSS And merged.
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    <ExportAPI("Contigs.Merge", Info:="If the distance of two TSS positions differed by less than 3 nt, they were treated as a single TSS And merged.")>
    Public Function MergeContigs(data As Generic.IEnumerable(Of ReadsGroupView), Optional offset As Integer = 3) As ReadsGroupView()
        '首先按照从小到大进行排序操作
        Dim Order = (From item In data Select item Order By item.POS Ascending).ToList
        Dim p As Integer

        Do While p < Order.Count - 1
            Dim current = Order(p)
            Call p.MoveNext

            Dim Next_P As Integer = p

            If Next_P = Order.Count Then
                Exit Do
            End If

            Dim [Next] = Order(Next_P)

            Do While [Next].POS - current.POS <= 3
                Call Order.RemoveAt(Next_P)
                current.NumberOfReads += [Next].NumberOfReads

                If Next_P = Order.Count Then
                    Exit Do
                End If

                [Next] = Order(Next_P)
                Call Console.Write("*")
            Loop

        Loop

        Return Order.ToArray
    End Function

    <ExportAPI("Gene.Association")>
    Public Function GeneAssociation(Reads As Generic.IEnumerable(Of ReadsGroupView), PTT As PTT) As ReadsGroupView()
        Dim ForwardGenes = (From Gene In PTT.GeneObjects Where Gene.Location.Strand = Strands.Forward Select Gene).ToArray
        Dim ReversedGenes = (From Gene In PTT.GeneObjects Where Gene.Location.Strand = Strands.Reverse Select Gene).ToArray
        Dim GetAssociationGenes = (From Segment As ReadsGroupView In Reads.AsParallel
                                   Let Source = If(Segment.Strand = Strands.Forward, ForwardGenes, ReversedGenes)
                                   Select Segment,
                                       RelatedGenes = GetRelatedGenes(Source, Segment)).ToArray
        Dim setValue As New SetValue(Of ReadsGroupView)
        Dim LQuery As List(Of ReadsGroupView) =
            LinqAPI.MakeList(Of ReadsGroupView) <= From x
                                                   In GetAssociationGenes.AsParallel
                                                   Select From RelatedGene In x.RelatedGenes.AsParallel
                                                          Let View = x.Segment.Copy
                                                          Select setValue _
                                                              .InvokeSetValue(View, NameOf(ReadsGroupView.AssociatedGene), RelatedGene.Gene.Synonym) _
                                                              .InvokeSet(NameOf(ReadsGroupView.Position), RelatedGene.Relation.ToString) _
                                                              .InvokeSet(NameOf(ReadsGroupView.GeneLocation), RelatedGene.Gene.Location.ToString).obj
        Call LQuery.AddRange((From obj In GetAssociationGenes.AsParallel Where obj.RelatedGenes.IsNullOrEmpty Select obj.Segment).ToArray)
        Return LQuery.ToArray
    End Function

    <ExportAPI("Gene.Association")>
    Public Function GeneAssociation(SourceDir As String, PTT As LANS.SystemsBiology.Assembly.NCBI.GenBank.TabularFormat.PTT, Optional Export As String = "") As Boolean
        If String.IsNullOrEmpty(Export) Then
            Export = SourceDir
        End If

        Dim Files = (From path In SourceDir.LoadSourceEntryList({"*.csv"}) Select name = path.Key, data = path.Value.LoadCsv(Of ReadsGroupView)(False)).ToArray
        Dim LQuery = (From data In Files Select data.name, viewData = GeneAssociation(data.data, PTT)).ToArray

        For Each dataFile In LQuery
            Dim FileName As String = $"{Export}/{dataFile.name}_gene_associations.csv"
            Call dataFile.viewData.SaveTo(FileName, False)
            Call Console.Write(".")
        Next

        Return True
    End Function

    Private Function GetRelatedGenes(Genes As GeneBrief(), Read As ReadsGroupView) As Relationship(Of GeneBrief)()
        'Dim Loci As Location = Read.Location
        'Dim relates = Genes.GetRelatedGenes(Loci.Left, Loci.Right)
        'Return relates
        Throw New NotImplementedException
    End Function

End Module
