#Region "Microsoft.VisualBasic::dee3e552665b01713cf779ff5d553062, localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Blast+\2.6.0+\BlastX\BlastXHit.vb"

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

    '     Class BlastXHit
    ' 
    '         Properties: Frame
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: CreateObjects, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application

Namespace LocalBLAST.BLASTOutput.BlastPlus.BlastX

    Public Class BlastXHit : Inherits BBH.BestHit

        Public Property Frame As String

        Sub New()
        End Sub

        Sub New(query As Components.Query, hit As Components.HitFragment)
            Me.evalue = hit.Score.Expect
            Me.Frame = hit.ReadingFrameOffSet
            Me.hit_length = hit.HitLen
            Me.identities = hit.Score.Identities
            Me.length_hit = hit.SubjectLength
            Me.length_hsp = hit.SubjectLength
            Me.length_query = hit.QueryLoci.FragmentSize
            Me.Positive = hit.Score.Positives
            Me.QueryName = query.QueryName
            Me.query_length = query.QueryLength
            Me.Score = hit.Score.Score

            With hit.HitName.GetTagValue(" ", trim:=True)
                HitName = .Name
                description = .Value
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Function CreateObjects(query As Components.Query) As BlastXHit()
            Dim LQuery As BlastXHit() = LinqAPI.Exec(Of BlastXHit) _
 _
                () <= From x As Components.HitFragment
                      In query.Subjects _
                          .SafeQuery _
                          .Select(Function(s) s.Hits) _
                          .IteratesALL
                      Select New BlastXHit(query, x)

            Return LQuery
        End Function
    End Class
End Namespace
