#Region "Microsoft.VisualBasic::73e16bb187697fae5299a8b14fdcdff8, analysis\SequenceToolkit\SmithWaterman\Extension\HSP.vb"

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

    ' Class HSP
    ' 
    '     Properties: LengthHit, LengthQuery, Query, QueryLength, Subject
    '                 SubjectLength
    ' 
    '     Function: CreateHSP, CreateObject
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein.LevenshteinDistance
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq.Extensions

Public Class HSP : Inherits Match

    Public Property Query As String
    Public Property Subject As String
    <XmlAttribute> Public Property QueryLength As Integer
    <XmlAttribute> Public Property SubjectLength As Integer

    ''' <summary>
    ''' length of the query fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthQuery As Integer
        Get
            Return Math.Abs(toA - fromA)
        End Get
    End Property

    ''' <summary>
    ''' length of the hit fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthHit As Integer
        Get
            Return Math.Abs(toB - fromB)
        End Get
    End Property

    Public Shared Function CreateObject(match As Match, query As String, subject As String) As HSP
        Dim queryp = Mid(query, match.fromA, match.toA - match.fromA)
        Dim subjectp = Mid(subject, match.fromB, match.toB - match.fromB)

        Return New HSP With {
            .fromA = match.fromA,
            .fromB = match.fromB,
            .toA = match.toA,
            .toB = match.toB,
            .QueryLength = query.Length,
            .SubjectLength = subject.Length,
            .score = match.score,
            .Query = queryp,
            .Subject = subjectp
        }
    End Function

    Public Shared Function CreateHSP(Of T)(sw As GSW(Of T), asChar As ToChar(Of T), ByRef best As HSP, cutoff As Double) As HSP()
        Dim query As String = sw.query.Select(Function(x) asChar(x)).CharString
        Dim subject As String = sw.subject.Select(Function(x) asChar(x)).CharString
        Dim matches = sw.Matches(cutoff).AsList
        Dim hsp As HSP() = matches _
            .Select(Function(x) CreateObject(x, query, subject)) _
            .ToArray

        Try
            Dim lstb = SimpleChaining.Chaining(hsp.Select(Function(x) DirectCast(x, Match)).AsList, False)
            lstb = (From x In lstb Select x Order By x.score Descending).AsList
            If Not lstb.IsNullOrEmpty Then
                best = CreateObject(lstb.FirstOrDefault, query, subject)
            End If
        Catch ex As Exception

        End Try

        Return hsp
    End Function
End Class
