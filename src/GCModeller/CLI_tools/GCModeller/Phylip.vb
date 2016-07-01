Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Phylip
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports Microsoft.VisualBasic.Linq.Extensions

Module Phylip

    Public Function MPAlignmentAsTree(aln As IEnumerable(Of MPCsvArchive)) As MatrixFile.Gendist

        aln = __mapScore(aln.ToArray)

        Dim QueryGroup = (From x In aln Select x Group x By x.QueryName Into Group)
        Dim Overviews = New Overview With {
            .Queries = QueryGroup.ToArray(Function(x) __toQueryOverview(x.QueryName, x.Group.ToArray))
        }
        Dim MAT = SelfOverviewsMAT(Overviews)
        Dim Gendist = MatrixFile.Gendist.CreateMotifDistrMAT(MAT)
        Return Gendist
    End Function

    ''' <summary>
    ''' 有些相似度是负数的，计算进化树的矩阵会报错，这里需要mapping一下将负数消除
    ''' </summary>
    ''' <param name="aln"></param>
    ''' <returns></returns>
    Private Function __mapScore(aln As MPCsvArchive()) As MPCsvArchive()
        Dim scores = aln.ToArray(Function(x) x.Score)
        Dim mapps = scores.GenerateMapping(1000)

        For i As Integer = 0 To aln.Length - 1
            aln(i).Score = mapps(i)
            aln(i).FullScore = 1000
        Next

        Return aln
    End Function

    Private Function __toQueryOverview(name As String, hits As MPCsvArchive()) As BLASTOutput.Views.Query
        Dim query As New BLASTOutput.Views.Query With {
            .Id = name,
            .Hits = hits.ToArray(
                Function(x) New BBH.BestHit With {
                    .HitName = x.HitName,
                    .identities = x.Similarity,
                    .QueryName = x.QueryName})
        }
        Return query
    End Function
End Module
