#Region "Microsoft.VisualBasic::0ca211edee1e4e8552b42cdcd8c1d81e, analysis\SequenceToolkit\SequencePatterns\Topologically\Seeds\SeedBox.vb"

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

    '     Class SeedBox
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: PopulateSeeds, PopulateSeedsFromSeedsFile, PopulateSeedsFromSequence, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel

Namespace Topologically.Seeding

    ''' <summary>
    ''' 生成序列上面的Feature位点计算的种子
    ''' </summary>
    Public Class SeedBox

        ReadOnly __seq As IPolymerSequenceModel
        ReadOnly __chars As Char()

        ''' <summary>
        ''' 会将``*``和``-``这些缺口的符号是需要被过滤掉的
        ''' </summary>
        ''' <param name="seq"></param>
        Sub New(seq As IPolymerSequenceModel)
            ' 获取所有的残基的符号
            __chars = seq.SequenceData _
                .ToArray _
                .Distinct _
                .Where(Function(c) c <> "*" AndAlso c <> "-") _
                .ToArray
            __seq = seq
        End Sub

        Public Iterator Function PopulateSeeds(min%, max%) As IEnumerable(Of Seed())
            Dim base As List(Of String) = __seq _
                .SequenceData _
                .PopulateExistsSeeds(Seeds.InitializeSeeds(__chars, min)) _
                .AsList

            Yield base.Select(Function(s) New Seed(s)).ToArray

            For len As Integer = min To max
                base = base.ExtendSequence(__chars)
                base = __seq.SequenceData.PopulateExistsSeeds(base).AsList

                Yield base.Select(Function(s) New Seed(s)).ToArray
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
        Public Shared Iterator Function PopulateSeedsFromSeedsFile(seeds$, seq As IPolymerSequenceModel, min%, max%) As IEnumerable(Of Seed())
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
                Dim out As Seed() = avaliable.Select(Function(s) New Seed(s.seed)).ToArray

                Yield out
            Next
        End Function

        Public Shared Iterator Function PopulateSeedsFromSequence(seq As IPolymerSequenceModel, min%, max%) As IEnumerable(Of Seed())
            Dim box As New SeedBox(seq)

            For Each pack In box.PopulateSeeds(min, max)
                Yield pack
            Next
        End Function

        Public Overrides Function ToString() As String
            Return __seq.ToString
        End Function
    End Class
End Namespace
