#Region "Microsoft.VisualBasic::03997a5ca56883cdbb3a0d878d27cebe, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Standard\BestHit.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xieguigang (xie.guigang@live.com)
'       xie (genetics@smrucc.org)
' 
' Copyright (c) 2016 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Namespace LocalBLAST.BLASTOutput.Legacy

    Partial Class BLASTOutput

        ''' <summary>
        ''' 获取本日志文件中的最好的序列比对匹配结果
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ExportBestHit(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Dim LQuery = From Query In Me.Queries.AsParallel
                         Let BestHit = GetBestHit(Query, identities_cutoff)
                         Let Row = GenerateRow(Query, BestHit)
                         Select Row
                         Order By Row.QueryName Ascending '
            Return LQuery.ToArray
        End Function

        Private Shared Function GenerateRow(Query As Query, BestHit As Hit) As LocalBLAST.Application.BBH.BestHit
            If BestHit Is Nothing Then
                Return New LocalBLAST.Application.BBH.BestHit With {.QueryName = Query.QueryName, .HitName = HITS_NOT_FOUND, .query_length = Query.Length}
            Else
                Dim Score As LocalBLAST.BLASTOutput.ComponentModel.Score = BestHit.Score
                Return New LocalBLAST.Application.BBH.BestHit With {
                    .QueryName = Query.QueryName, .HitName = BestHit.Name, .query_length = Query.Length, .hit_length = BestHit.Length, .Score = Score.Score, .evalue = Score.Expect,
                    .identities = Score.Identities.Value, .Positive = Score.Positives.Value, .length_hsp = Score.Gaps.Value}
            End If
        End Function

        Private Shared Function GetBestHit(Query As Query, identities As Double) As Hit
            If Query.Hits Is Nothing OrElse Query.Hits.Count = 0 Then
                Return Nothing
            Else
                Dim LQuery = From Hit In Query.Hits
                             Where (Hit.Score.Identities > identities And Query.Length / Hit.Length > 0.5)
                             Select Hit
                             Order By Hit.Score.Expect Ascending '
                Dim Result = LQuery.ToArray
                If Result.Count > 0 Then
                    Return Result.First '找到了最佳的匹配
                Else
                    Return Nothing '在匹配项中没有找到任何符合条件的匹配项
                End If
            End If
        End Function
    End Class
End Namespace
