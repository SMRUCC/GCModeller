#Region "Microsoft.VisualBasic::87a1ee1ae2aff03f8ab1aefa62e485bf, ..\interops\localblast\LocalBLAST\LocalBLAST\BlastOutput\Xml\Iteration.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.IBlastOutput
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.XmlFile.Hits

Namespace LocalBLAST.BLASTOutput.XmlFile

    Public Class Iteration

        <XmlElement("Iteration_iter-num")> Public Property IterNum As String
        <XmlElement("Iteration_query-ID")> Public Property QueryId As String
        <XmlElement("Iteration_query-def")> Public Property QueryDef As String
        <XmlElement("Iteration_query-len")> Public Property QueryLen As String
        <XmlArray("Iteration_hits")> Public Property Hits As Hit()
        <XmlArray("Iteration_stat")> Public Property Stat As Statistics()

        Public Function GrepQuery(method As TextGrepMethod) As Integer
            If Not QueryDef.IsNullOrEmpty Then
                QueryDef = method(QueryDef)
            Else
                QueryDef = "Unknown"
            End If
            Return 0
        End Function

        Public Function BestHit(Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit
            If Me.Hits.IsNullOrEmpty Then
                Return New LocalBLAST.Application.BBH.BestHit With {.QueryName = Me.QueryDef, .HitName = HITS_NOT_FOUND}
            Else
                Dim LQuery = From Hit As Hit In Me.Hits
                             Where Hit.Coverage > 0.5 AndAlso Hit.Identities > identities_cutoff
                             Select Hit
                             Order By Hit.Gaps Ascending '
                Dim Result = LQuery.ToArray
                If Result.IsNullOrEmpty Then
                    Return New LocalBLAST.Application.BBH.BestHit With {.QueryName = Me.QueryDef, .HitName = HITS_NOT_FOUND}
                Else
                    Dim _BestHit = Result.First
                    Return New LocalBLAST.Application.BBH.BestHit With {.QueryName = Me.QueryDef, .HitName = _BestHit.Id}
                End If
            End If
        End Function

        Public Function GrepHits(method As TextGrepMethod) As Integer
            If method Is Nothing OrElse Hits Is Nothing OrElse Hits.Count = 0 Then
                Return 0
            Else
                Dim Query = From Hit As Hit In Hits.AsParallel Select Hit.Grep(method) '
                Return Query.ToArray.Length
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("> {0}|{1}", QueryId, QueryDef)
        End Function
    End Class
End Namespace
