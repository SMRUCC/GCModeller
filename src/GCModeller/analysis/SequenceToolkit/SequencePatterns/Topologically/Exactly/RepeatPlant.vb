Imports SMRUCC.genomics.ComponentModel.Loci.Abstract
Imports SMRUCC.genomics.SequenceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Topologically

    Public Class RepeatsLoci : Implements ILoci

        Public Property RepeatLoci As String
        <Column("Loci.Left")> Public Property LociLeft As Integer Implements ILoci.Left

        Friend Overridable Function __hash() As String
            Return Me.RepeatLoci & CStr(LociLeft)
        End Function
    End Class

    Public Class RevRepeatsLoci : Inherits RepeatsLoci
        <Column("rev-Repeats")> Public Property RevRepeats As String
        <Column("rev-Loci.Left")> Public Property RevLociLeft As Integer

        Friend Overrides Function __hash() As String
            Return MyBase.__hash() & Me.RevRepeats & CStr(Me.RevLociLeft)
        End Function
    End Class

    Public Class RepeatsView : Implements ILoci
        Implements I_PolymerSequenceModel

        Public Property Left As Integer Implements ILoci.Left
        Public Property SequenceData As String Implements I_PolymerSequenceModel.SequenceData
        Public Property Locis As Integer()
        Public ReadOnly Property Length As Integer
            Get
                Return Len(SequenceData)
            End Get
        End Property

        Public Overridable Function LociProvider() As Integer()
            Return Locis
        End Function

        Public Shared Function TrimView(data As Generic.IEnumerable(Of RepeatsLoci)) As RepeatsView()
            Dim LQuery = (From loci As RepeatsLoci
                          In data
                          Select loci
                          Group loci By loci.RepeatLoci Into Group).ToArray
            Dim views = (From loci In LQuery
                         Let pos As Integer() = loci.Group.ToArray(Function(site) CInt(site.LociLeft))
                         Select New RepeatsView With {
                             .SequenceData = loci.RepeatLoci,
                             .Left = pos.Min,
                             .Locis = pos}).ToArray
            Return views
        End Function

        Public Overrides Function ToString() As String
            Return $"{SequenceData}  @{Left}, {Hot}"
        End Function

        ''' <summary>
        ''' 返回的是基因组上面的每一个位点的热度的列表
        ''' </summary>
        ''' <typeparam name="TView"></typeparam>
        ''' <param name="data"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        Public Shared Function ToVector(Of TView As RepeatsView)(data As IEnumerable(Of TView), size As Long) As Double()
            Dim Extract = (From obj As TView
                           In data
                           Where Not obj Is Nothing
                           Select (From site As Integer
                                   In obj.LociProvider
                                   Select site,
                                       obj.Hot).ToArray).MatrixToList
            Dim src = (From t In (From site In Extract
                                  Select site
                                  Group site By site.site Into Group)
                       Select t.site,
                           Group = t.Group.ToArray
                       Order By site Ascending).ToArray
            Dim LQuery As Dictionary(Of Integer, Double) =
                src.ToDictionary(
                Function(site) site.site,
                Function(site) site.Group.ToArray(Function(loci) loci.Hot).Sum)
            Dim vector As Double() = size.ToArray(Function(idx) LQuery.TryGetValue(idx, [default]:=0))
            Return vector
        End Function

        ''' <summary>
        ''' 平均距离越小，则热度越高
        ''' 位点越多，热度越高
        ''' 片段越长，热度越高
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property Hot As Double
            Get
                Dim ordLocis = (From n As Integer
                                In Me.LociProvider
                                Select n
                                Order By n Ascending).ToArray.CreateSlideWindows(2)
                Dim avgDist As Double = ordLocis.ToArray(
                    Function(loci) _
                        If(loci.Elements.IsNullOrEmpty OrElse
                        loci.Elements.Length = 1, 1,
                        loci.Elements.Last - loci.Elements.First)).Average
                avgDist = Len(SequenceData) / avgDist  ' 表达式的含义： 片段越长，热度越高，  平均距离越短，热度越高
                Dim lociCounts As Double = LociProvider.Length
                lociCounts = lociCounts / 10
                Return avgDist + lociCounts
            End Get
        End Property

        Public Overridable ReadOnly Property RepeatsNumber As Integer
            Get
                Return Me.LociProvider.Length
            End Get
        End Property
    End Class

    Public Class RevRepeatsView : Inherits RepeatsView
        Implements ILoci
        Implements I_PolymerSequenceModel

        Public Property RevLocis As Integer()
        Public Property RevSegment As String

        Public Overrides Function LociProvider() As Integer()
            Return RevLocis
        End Function

        Public Overloads Shared Function TrimView(data As Generic.IEnumerable(Of RevRepeats)) As RevRepeatsView()
            Dim LQuery As RevRepeatsView() = data.ToArray(
                Function(loci) _
                    New RevRepeatsView With {
                        .Left = loci.Locations.Min,
                        .Locis = loci.RepeatLoci,
                        .RevLocis = loci.Locations,
                        .RevSegment = loci.RevSegment,
                        .SequenceData = loci.SequenceData
                    })
            Return LQuery
        End Function

        Public Overrides ReadOnly Property RepeatsNumber As Integer
            Get
                Return RevLocis.Length
            End Get
        End Property

        Public Overrides ReadOnly Property Hot As Double
            Get
                Dim loci = (From n As Integer
                            In RevLocis
                            Select n
                            Order By n Ascending).ToArray.CreateSlideWindows(2)
                Dim avgDist As Double = loci.ToArray(
                    Function(lo) _
                        If(lo.Elements.IsNullOrEmpty OrElse
                        lo.Elements.Length = 1,
                        1, lo.Elements.Last - lo.Elements.First)).Average
                Return MyBase.Hot + Len(RevSegment) / avgDist
            End Get
        End Property
    End Class
End Namespace