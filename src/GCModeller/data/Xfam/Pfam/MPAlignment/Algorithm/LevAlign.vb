#Region "Microsoft.VisualBasic::0b38265d5925e5a1d233513ee806feed, data\Xfam\Pfam\MPAlignment\Algorithm\LevAlign.vb"

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

    '     Class LevAlign
    ' 
    '         Properties: Codes, DomainCodes, LengthDelta, Query, QueryPfam
    '                     StructMatched, Subject, SubjectPfam
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: __asChar, __getReference, __getSubject, __innerInsert, ToRow
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace ProteinDomainArchitecture.MPAlignment

    ''' <summary>
    ''' 这个比对是做结构域对其的
    ''' </summary>
    Public Class LevAlign : Inherits DistResult

        Public ReadOnly Property DomainCodes As IReadOnlyDictionary(Of String, Char)

        Public Property Codes As KeyValuePairObject(Of String, Char)()
            Get
                Return DomainCodes.Select(Function(row) New KeyValuePairObject(Of String, Char)(row)).ToArray
            End Get
            Set(value As KeyValuePairObject(Of String, Char)())
                _DomainCodes = value.ToDictionary(Function(row) row.Key, Function(row) row.Value)
            End Set
        End Property

        Public Property Query As String
        Public Property Subject As String

        Public Property QueryPfam As PfamString.PfamString
        Public Property SubjectPfam As PfamString.PfamString

        Public ReadOnly Property LengthDelta As Double
            Get
                Dim ps As Double() = {QueryPfam.Length, SubjectPfam.Length}
                Return 1 - ps.Min / ps.Max
            End Get
        End Property

        Sub New()
        End Sub

        'Const ASCII As String = "abcdefghijklmnopqrstuvwxyz0123456789/*-+.?<>,;:[]{}()=|\`~!@#$%^&"

        Const A As Integer = Asc("A"c)

        Private Function __asChar(x As ProteinModel.DomainObject) As Char
            Return DomainCodes(x.Name)
        End Function

        ''' <summary>
        ''' 结构域是否是完全匹配上的
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property StructMatched As Boolean
            Get
                If QueryPfam.PfamString.IsNullOrEmpty OrElse
                    SubjectPfam.PfamString.IsNullOrEmpty Then
                    Return False
                Else
                    Return NumMatches = QueryPfam.PfamString.Length AndAlso
                        NumMatches = SubjectPfam.PfamString.Length
                End If
            End Get
        End Property

        ''' <summary>
        ''' 由于是需要对需要注释的基因组里面的未知功能的蛋白质的生物学功能进行推测，所以这里的Reference是数据库之中的蛋白质
        ''' </summary>
        ''' <param name="prot_a"></param>
        ''' <param name="prot_b"></param>
        Sub New(prot_a As PfamString.PfamString, prot_b As PfamString.PfamString, equals As DomainEquals)
            Dim domainA As ProteinModel.DomainObject() = prot_a.GetDomainData(False).OrderBy(Function(x) x.Position.left).ThenBy(Function(x) x.Name).ToArray
            Dim domainB As ProteinModel.DomainObject() = prot_b.GetDomainData(False).OrderBy(Function(x) x.Position.left).ThenBy(Function(x) x.Name).ToArray

            ' 首先进行编码工作
            DomainCodes = (From domain In domainA.Join(domainB)
                           Select domain
                           Group domain By domain.Name Into Group) _
                                    .Select(Function(name, idx) New With {
                                        .ch = ChrW(idx + LevAlign.A),
                                        .Name = name.Name}) _
                                            .ToDictionary(Function(name) name.Name,
                                                          Function(name) name.ch)

            Dim align As DistResult = LevenshteinDistance.ComputeDistance(domainA, domainB, AddressOf equals.Equals, AddressOf __asChar)

            If align Is Nothing Then  ' 完全比对不上的
                Dim result As AlignmentOutput = Algorithm.AltEquals(prot_a, prot_b, equals.__high_Scoring_thresholds)
                Dim distTable As Double(,) = New Double(domainA.Length, domainB.Length) {}

                Me.DistEdits = result.AlignmentResult.Edits
                Me.DistTable = distTable.ToVectorList.Select(Function(x) New ArrayRow(x)).ToArray
                Me.Matches = DistEdits
                Me.DistTable(domainA.Length)(domainB.Length) = result.FullScore - result.Score
            Else
                Call align.CopyTo(Me)
            End If

            Me.Reference = prot_a.ProteinId
            Me.Hypotheses = prot_b.ProteinId
            Me.Query = New String(domainA.Select(AddressOf __asChar).ToArray)
            Me.Subject = New String(domainB.Select(AddressOf __asChar).ToArray)
            Me.QueryPfam = prot_a
            Me.SubjectPfam = prot_b
        End Sub

        Protected Overrides Function __innerInsert() As String
            Dim str As StringBuilder = New StringBuilder(1024)
            Call str.AppendLine($"<tr><td>Query-Pfam: </td><td> {QueryPfam}</td></tr>")
            Call str.AppendLine($"<tr><td>Subject-Pfam: </td><td> {SubjectPfam}</td></tr>")
            Return str.ToString
        End Function

        Protected Overrides Function __getReference() As String
            Return Query
        End Function

        Protected Overrides Function __getSubject() As String
            Return Subject
        End Function

        Public Function ToRow() As MPCsvArchive
            Return MPCsvArchive.CreateObject(Me)
        End Function
    End Class
End Namespace
