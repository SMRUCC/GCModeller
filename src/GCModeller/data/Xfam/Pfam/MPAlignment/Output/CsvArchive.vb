#Region "Microsoft.VisualBasic::664ea2fb68b7453bc6b6a4b88ac37189, data\Xfam\Pfam\MPAlignment\Output\CsvArchive.vb"

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
    '    Code Lines: 65 (60.75%)
    ' Comment Lines: 32 (29.91%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (9.35%)
    '     File Size: 4.17 KB


    '     Class MPCsvArchive
    ' 
    '         Properties: Description, Distance, FullScore, LengthDelta, LevMatch
    '                     LevScore, MatchDomains, QueryLength, QueryPfamString, Score
    '                     Similarity, StructMatched, SubjectPfamString
    ' 
    '         Function: (+2 Overloads) CreateObject, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' CSV格式的MPAlignment结果数据记录
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MPCsvArchive : Inherits I_BlastQueryHit

        ''' <summary>
        ''' Protein sequence length.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property QueryLength As Integer

        <Column("query.pfam-string")> Public Property QueryPfamString As String
        <Column("subject.pfam-string")> Public Property SubjectPfamString As String

        ''' <summary>
        ''' MPScore
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Score As Double
        Public Property FullScore As Double

        ''' <summary>
        ''' <see cref="Score"></see>/<see cref="FullScore"></see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Similarity As Double
            Get
                Return Score / FullScore
            End Get
        End Property

        Public Property Distance As Double
        Public Property LevScore As Double
        Public Property LevMatch As String
        Public Property LengthDelta As Double
        Public ReadOnly Property MatchDomains As Integer
            Get
                If String.IsNullOrEmpty(LevMatch) Then
                    Return 0
                End If
                Dim n As Integer = (From ch As Char In LevMatch Where ch <> "-"c Select ch).Count
                Return n
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 结构域是否是完全匹配上的
        ''' </summary>
        ''' <returns></returns>
        Public Property StructMatched As Boolean

        Public Shared Function CreateObject(output As LevAlign) As MPCsvArchive
            Return New MPCsvArchive With {
                .HitName = output.SubjectPfam.ProteinId,
                .QueryName = output.QueryPfam.ProteinId,
                .SubjectPfamString = output.SubjectPfam.get__PfamString,
                .QueryPfamString = output.QueryPfam.get__PfamString,
                .Score = output.Score * (1 - output.LengthDelta),
                .Distance = output.Distance,
                .LevMatch = output.Matches,
                .QueryLength = output.QueryPfam.Length,
                .Description = output.QueryPfam.Description,
                .LevScore = output.Score,
                .LengthDelta = output.LengthDelta,
                .FullScore = 1,
                .StructMatched = output.StructMatched
            }
        End Function

        Public Shared Function CreateObject(output As AlignmentOutput) As MPCsvArchive
            Return New MPCsvArchive With {
                .FullScore = output.FullScore,
                .HitName = output.ProteinSbjct.ProteinId,
                .LengthDelta = output.LengthDelta,
                .Description = output.ProteinQuery.Description,
                .QueryLength = output.ProteinQuery.Length,
                .QueryName = output.ProteinQuery.ProteinId,
                .QueryPfamString = output.ProteinQuery.get__PfamString,
                .Score = output.Score,
                .SubjectPfamString = output.ProteinSbjct.get__PfamString
            }
        End Function
    End Class
End Namespace
