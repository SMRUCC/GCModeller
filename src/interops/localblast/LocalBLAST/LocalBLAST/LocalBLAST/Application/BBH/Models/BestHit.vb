#Region "Microsoft.VisualBasic::9d8d88542f909453fabff8b545f129ff, localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Models\BestHit.vb"

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

'   Total Lines: 95
'    Code Lines: 64 (67.37%)
' Comment Lines: 20 (21.05%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 11 (11.58%)
'     File Size: 4.29 KB


'     Class BestHit
' 
'         Properties: coverage, description, evalue, hit_length, identities
'                     length_hit, length_hsp, length_query, positive, query_length
'                     SBHScore, score
' 
'         Function: FindByQueryName, IsMatchedBesthit, IsNullOrEmpty, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models.KeyValuePair
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH.Abstract
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace LocalBLAST.Application.BBH

    ''' <summary>
    ''' A single direction best hit.
    ''' (单向的最佳比对结果)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BestHit : Inherits I_BlastQueryHit
        Implements IKeyValuePair, IQueryHits

        <Column("query_length")> Public Property query_length As Integer
        <Column("hit_length")> Public Property hit_length As Integer
        ''' <summary>
        ''' usually be the blast raw bit score
        ''' </summary>
        ''' <returns></returns>
        <Column("score")> Public Property score As Double
        <Column("e-value")> Public Property evalue As Double
        <Column("identities")> Public Property identities As Double Implements IQueryHits.identities
        <Column("positive")> Public Property positive As Double
        <Column("length_hit")> Public Property length_hit As Integer
        <Column("length_query")> Public Property length_query As Integer
        <Column("length_hsp")> Public Property length_hsp As Integer

        ''' <summary>
        ''' 这个覆盖度是相对于query作为未知功能的全长protein序列而言的。
        ''' <see cref="length_query"/> / <see cref="query_length"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property coverage As Double
            Get
                Return Me.length_query / Me.query_length
            End Get
        End Property

        Public ReadOnly Property SBHScore As Double
            Get
                Return identities * coverage
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(copy As BestHit)
            With copy
                Me.QueryName = .QueryName
                Me.HitName = .HitName
                Me.description = .description
                Me.query_length = .query_length
                Me.hit_length = hit_length
                Me.score = .score
                Me.evalue = .evalue
                Me.identities = .identities
                Me.positive = .positive
                Me.length_hit = .length_hit
                Me.length_query = .length_query
                Me.length_hsp = .length_hsp
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("{0} vs. {1}; E-value:={2}", QueryName, HitName, evalue)
        End Function

        ''' <summary>
        ''' <see cref="Besthit.coverage"/> >= <paramref name="coverage"/> AndAlso <see cref="Besthit.identities"/> >= <paramref name="identities"/> 
        ''' </summary>
        ''' <param name="identities"></param>
        ''' <param name="coverage"></param>
        ''' <returns></returns>
        Public Function IsMatchedBesthit(Optional identities As Double = 0.15, Optional coverage As Double = 0.5) As Boolean
            If length_query = 0 Then
                Return False
            End If
            Return Me.coverage >= coverage AndAlso Me.identities >= identities
        End Function

        Public Shared Function FindByQueryName(QueryName As String, source As IEnumerable(Of BestHit)) As BestHit()
            Dim LQuery As BestHit() =
                LinqAPI.Exec(Of BestHit) <= From bh As BestHit
                                            In source
                                            Where String.Equals(QueryName, bh.QueryName)
                                            Select bh
            Return LQuery
        End Function

        Public Shared Function IsNullOrEmpty(Of T As BestHit)(data As IEnumerable(Of T), Optional trimSelfAligned As Boolean = False) As Boolean
            If data.IsNullOrEmpty Then
                Return True
            End If

            If Not trimSelfAligned Then
                Dim LQuery = (From hit As T In data.AsParallel
                              Where Not String.Equals(hit.HitName, IBlastOutput.HITS_NOT_FOUND)
                              Select hit).FirstOrDefault
                Return LQuery Is Nothing
            Else
                Dim LQuery = (From item As T In data.AsParallel
                              Where Not String.IsNullOrEmpty(item.QueryName) AndAlso
                              String.Equals(item.HitName, item.QueryName)
                              Select item).FirstOrDefault
                Return LQuery Is Nothing
            End If
        End Function
    End Class
End Namespace
