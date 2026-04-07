#Region "Microsoft.VisualBasic::b645c2112f7cb05f1efde564def25b64, localblast\LocalBLAST\Pipeline\Exports.vb"

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

    '   Total Lines: 52
    '    Code Lines: 46 (88.46%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 6 (11.54%)
    '     File Size: 2.05 KB


    '     Module Exports
    ' 
    '         Function: ExportHitsResult, ReadSubjectHits
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput.BlastPlus
Imports SMRUCC.genomics.Interops.NCBI.Extensions.Tasks.Models

Namespace Pipeline

    Public Module Exports

        <Extension>
        Public Iterator Function ExportHitsResult(blast As IEnumerable(Of Query), Optional grepName As Func(Of String, NamedValue(Of String)) = Nothing) As IEnumerable(Of HitCollection)
            For Each query As Query In blast.SafeQuery
                Yield New HitCollection With {
                    .QueryName = query.QueryName,
                    .description = query.QueryName,
                    .hits = query.SubjectHits _
                        .ReadSubjectHits(grepName) _
                        .ToArray
                }
            Next
        End Function

        <Extension>
        Private Iterator Function ReadSubjectHits(subjects As IEnumerable(Of SubjectHit), grepName As Func(Of String, NamedValue(Of String))) As IEnumerable(Of Hit)
            Dim hitname As String
            Dim tag As String

            For Each subj As SubjectHit In subjects.SafeQuery
                If grepName Is Nothing Then
                    hitname = subj.Name
                    tag = hitname
                Else
                    With grepName(subj.Name)
                        tag = .Name
                        hitname = .Value
                    End With
                End If

                Yield New Hit With {
                    .hitName = hitname,
                    .identities = subj.Score.Identities,
                    .positive = subj.Score.Positives,
                    .tag = tag,
                    .evalue = subj.Score.Expect,
                    .gaps = subj.Score.Gaps,
                    .score = subj.Score.Score
                }
            Next
        End Function
    End Module
End Namespace
