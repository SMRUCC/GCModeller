#Region "Microsoft.VisualBasic::d554db02f35196bc19f110c0fbebb36f, localblast\LocalBLAST\LocalBLAST\BlastOutput\Xml\Iteration.vb"

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

    '     Class Iteration
    ' 
    '         Properties: Hits, IterNum, QueryDef, QueryId, QueryLen
    '                     Stat
    ' 
    '         Function: BestHit, GrepHits, GrepQuery, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH
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
            If Not QueryDef.StringEmpty Then
                QueryDef = method(QueryDef)
            Else
                QueryDef = "Unknown"
            End If
            Return 0
        End Function

        Public Function BestHit(Optional identities_cutoff As Double = 0.15) As BestHit
            If Me.Hits.IsNullOrEmpty Then Return New BestHit With {
                .QueryName = Me.QueryDef,
                .HitName = HITS_NOT_FOUND
            }

            Dim LQuery = LinqAPI.DefaultFirst(Of Hit) <=
 _
                From hit As Hit
                In Me.Hits
                Where hit.Coverage > 0.5 AndAlso hit.Identities > identities_cutoff
                Select hit
                Order By hit.Gaps Ascending '

            If LQuery Is Nothing Then Return New BestHit With {
                .QueryName = Me.QueryDef,
                .HitName = HITS_NOT_FOUND
            }

            Return New BestHit With {
                .QueryName = Me.QueryDef,
                .HitName = LQuery.Id
            }
        End Function

        Public Function GrepHits(method As TextGrepMethod) As Integer
            If method Is Nothing OrElse Hits.IsNullOrEmpty Then
                Return 0
            Else
                Dim LQuery = LinqAPI.Exec(Of Integer) <=
                    From Hit As Hit
                    In Hits.AsParallel
                    Select Hit.Grep(method) '

                Return LQuery.Length
            End If
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("> {0}|{1}", QueryId, QueryDef)
        End Function
    End Class
End Namespace
