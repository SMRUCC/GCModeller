#Region "Microsoft.VisualBasic::723f060fad891cab69399e8759787580, ..\GCModeller\analysis\SequenceToolkit\SmithWaterman\Extension\HSP.vb"

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

Imports System.Linq
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Text.LevenshteinDistance

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
            Return Math.Abs(ToA - FromA)
        End Get
    End Property

    ''' <summary>
    ''' length of the hit fragment size
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LengthHit As Integer
        Get
            Return Math.Abs(ToB - FromB)
        End Get
    End Property

    Public Shared Function CreateObject(match As Match, query As String, subject As String) As HSP
        Dim queryp = Mid(query, match.FromA, match.ToA - match.FromA)
        Dim subjectp = Mid(subject, match.FromB, match.ToB - match.FromB)

        Return New HSP With {
            .FromA = match.FromA,
            .FromB = match.FromB,
            .ToA = match.ToA,
            .ToB = match.ToB,
            .QueryLength = query.Length,
            .SubjectLength = subject.Length,
            .Score = match.Score,
            .Query = queryp,
            .Subject = subjectp
        }
    End Function

    Public Shared Function CreateHSP(Of T)(sw As GSW(Of T), asChar As ToChar(Of T), ByRef best As HSP, cutoff As Double) As HSP()
        Dim query As String = New String(sw.query.ToArray(Function(x) asChar(x)))
        Dim subject As String = New String(sw.subject.ToArray(Function(x) asChar(x)))
        Dim matches = sw.Matches(cutoff).ToList
        Dim hsp = matches.ToArray(Function(x) CreateObject(x, query, subject))

        Try
            Dim lstb = SimpleChaining.chaining(hsp.ToArray(Function(x) x.As(Of Match)).ToList, False)
            lstb = (From x In lstb Select x Order By x.Score Descending).ToList
            If Not lstb.IsNullOrEmpty Then
                best = CreateObject(lstb.FirstOrDefault, query, subject)
            End If
        Catch ex As Exception

        End Try

        Return hsp
    End Function

End Class

