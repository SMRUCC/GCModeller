Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically.Seeding

    ''' <summary>
    ''' 生成序列上面的Feature位点计算的种子
    ''' </summary>
    Public Class SeedBox

        ReadOnly __seq As I_PolymerSequenceModel
        ReadOnly __chars As Char()

        Sub New(seq As I_PolymerSequenceModel)
            __seq = seq
            __chars = seq.SequenceData _
                .ToArray _
                .Distinct _
                .ToArray
        End Sub

        Public Iterator Function PopulateSeeds(min%, max%) As IEnumerable(Of Seed())
            Dim base As List(Of String) = __trimAvaliable(Seeds.InitializeSeeds(__chars, min))

            Yield base.ToArray(Function(s) New Seed(s))

            For len As Integer = min To max
                base = base.ExtendSequence(__chars)
                base = __trimAvaliable(base)

                Yield base.ToArray(Function(s) New Seed(s))
            Next
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="seeds$">序列种子文件的文件路径</param>
        ''' <param name="seq"></param>
        ''' <param name="min%"></param>
        ''' <param name="max%"></param>
        ''' <returns></returns>
        Public Shared Iterator Function PopulateSeedsFromSeedsFile(seeds$, seq As I_PolymerSequenceModel, min%, max%) As IEnumerable(Of Seed())
            Dim data As SeedData = SeedData.Load(seeds)
            Dim lg = From seed As String
                     In data.Seeds
                     Let len As Integer = seed.Length
                     Where len >= min AndAlso len <= max
                     Select len, seed
                     Group By len Into Group
                     Order By len Ascending

            For Each pack In lg
                Dim avaliable = pack.Group.Where(Function(s) seq.SequenceData.IndexOf(s.seed) > -1)
                Dim out As Seed() = avaliable.ToArray(Function(s) New Seed(s.seed))

                Yield out
            Next
        End Function

        Public Shared Iterator Function PopulateSeedsFromSequence(seq As I_PolymerSequenceModel, min%, max%) As IEnumerable(Of Seed())
            Dim box As New SeedBox(seq)

            For Each pack In box.PopulateSeeds(min, max)
                Yield pack
            Next
        End Function

        Private Function __trimAvaliable(seeds As IEnumerable(Of String)) As List(Of String)
            Dim out As New List(Of String)(seeds.Where(Function(s) __seq.SequenceData.IndexOf(s) > -1))
            Return out
        End Function

        Public Overrides Function ToString() As String
            Return __seq.ToString
        End Function
    End Class
End Namespace