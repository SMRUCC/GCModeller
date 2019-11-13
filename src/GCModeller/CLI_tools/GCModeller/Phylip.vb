#Region "Microsoft.VisualBasic::80a69ceb808b16a37e2f9416b6fec627, CLI_tools\GCModeller\Phylip.vb"

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

    ' Module Phylip
    ' 
    '     Function: __mapScore, __toQueryOverview, MPAlignmentAsTree
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Math
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.Views
Imports SMRUCC.genomics.Interops.Visualize.Phylip

Module Phylip

    Public Function MPAlignmentAsTree(aln As IEnumerable(Of MPCsvArchive)) As MatrixFile.Gendist
        Dim QueryGroup = From x As MPCsvArchive
                         In __mapScore(aln.ToArray)
                         Select x
                         Group x By x.QueryName Into Group
        Dim Overviews As New Overview With {
            .Queries = QueryGroup.Select(
                Function(x) __toQueryOverview(
                    x.QueryName,
                    x.Group.ToArray)).ToArray
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
        Dim scores = aln.Select(Function(x) x.Score)
        Dim mapps%() = scores.GenerateMapping(1000)

        For i As Integer = 0 To aln.Length - 1
            aln(i).Score = mapps(i)
            aln(i).FullScore = 1000
        Next

        Return aln
    End Function

    Private Function __toQueryOverview(name As String, hits As MPCsvArchive()) As BLASTOutput.Views.Query
        Dim query As New BLASTOutput.Views.Query With {
            .Id = name,
            .Hits = hits _
                .Select(Function(x) New BBH.BestHit With {
                    .HitName = x.HitName,
                    .identities = x.Similarity,
                    .QueryName = x.QueryName
                })
        }
        Return query
    End Function
End Module
