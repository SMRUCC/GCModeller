#Region "Microsoft.VisualBasic::60ac0e3def18ec3290c320e8f056a6af, G:/GCModeller/src/GCModeller/data/Xfam/Pfam//Pipeline/LocalBlast/BlastOutputParser.vb"

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


    ' Code Statistics:

    '   Total Lines: 107
    '    Code Lines: 75
    ' Comment Lines: 22
    '   Blank Lines: 10
    '     File Size: 4.86 KB


    '     Module BlastOutputParser
    ' 
    '         Function: ParseDomainQuery, ParseProteinQuery
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.ComponentModel

Namespace Pipeline.LocalBlast

    Public Module BlastOutputParser

        ''' <summary>
        ''' PfamA as query, alignment with protein sequence as subjects
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个方向的比较结果比较准确，但是后续会面临一个数据量比较大的按照蛋白编号分组的问题出现
        ''' 在这里一个query就是一个pfam结构域
        ''' </remarks>
        <Extension>
        Public Iterator Function ParseDomainQuery(query As Query) As IEnumerable(Of PfamHit)
            Dim pfamHit$ = query.QueryName
            ' 因为比对的方向是pfam vs protein
            ' 所以subject location是pfam在目标蛋白序列上的位置
            Dim location As Location
            Dim score As Score

            For Each hit As SubjectHit In query.SubjectHits.SafeQuery
                Dim queryName = hit.Name.GetTagValue(, trim:=True)
                Dim queryId = queryName.Name
                Dim queryDescribe = queryName.Value

                For Each fragment As FragmentHit In DirectCast(hit, BlastpSubjectHit).FragmentHits
                    score = fragment.Score
                    location = New Location(
                        fragment.Hsp.Select(Function(hsp) hsp.Subject.Left).Min,
                        fragment.Hsp.Select(Function(hsp) hsp.Subject.Right).Max
                    )

                    Yield New PfamHit With {
                        .description = queryDescribe,
                        .HitName = pfamHit,
                        .QueryName = queryId,
                        .query_length = hit.Length,
                        .hit_length = query.QueryLength,
                        .length_hit = fragment.LengthQuery,
                        .length_query = fragment.LengthHit,
                        .length_hsp = score.Gaps.Denominator,
                        .evalue = score.Expect,
                        .identities = score.Identities,
                        .positive = score.Positives,
                        .score = score.Score,
                        .start = location.left,
                        .ends = location.right
                    }
                Next
            Next
        End Function

        ''' <summary>
        ''' The protein sequence as query input, alignment with pfamA domain sequence as subjects.
        ''' </summary>
        ''' <param name="query"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这个只需要解析query就好了，不需要做group就可以构建出一条蛋白的结构域注释结果
        ''' 在这里，一个hit，就是一个pfam结构域
        ''' </remarks>
        <Extension>
        Public Iterator Function ParseProteinQuery(query As Query) As IEnumerable(Of PfamHit)
            Dim queryName = query.QueryName.GetTagValue(, trim:=True)
            Dim queryId = queryName.Name
            Dim queryDescribe = queryName.Value
            Dim location As Location
            Dim pfamHit As PfamHit
            Dim score As Score

            For Each hit As SubjectHit In query.SubjectHits.SafeQuery
                ' 因为比对的方向是protein vs pfam
                ' 所以query location是pfam在目标蛋白序列上的位置
                location = hit.QueryLocation

                For Each fragment As FragmentHit In DirectCast(hit, BlastpSubjectHit).FragmentHits
                    score = fragment.Score
                    pfamHit = New PfamHit With {
                        .description = queryDescribe,
                        .HitName = hit.Name,
                        .evalue = score.Expect,
                        .QueryName = queryId,
                        .score = score.Score,
                        .query_length = query.QueryLength,
                        .length_query = hit.LengthQuery,
                        .hit_length = hit.Length,
                        .length_hit = hit.LengthHit,
                        .length_hsp = score.Gaps.Denominator,
                        .identities = score.Identities,
                        .positive = score.Positives,
                        .start = location.left,
                        .ends = location.right
                    }

                    Yield pfamHit
                Next
            Next
        End Function
    End Module
End Namespace
