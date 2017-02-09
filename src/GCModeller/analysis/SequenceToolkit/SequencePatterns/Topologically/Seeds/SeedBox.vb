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

        Private Function __trimAvaliable(seeds As IEnumerable(Of String)) As List(Of String)
            Dim out As New List(Of String)(seeds.Where(Function(s) __seq.SequenceData.IndexOf(s) > -1))
            Return out
        End Function

        Public Overrides Function ToString() As String
            Return __seq.ToString
        End Function
    End Class
End Namespace