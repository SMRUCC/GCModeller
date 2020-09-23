#Region "Microsoft.VisualBasic::f340aa28bcec45a41c5285d13ce51904, analysis\ProteinTools\ProteinTools.Family\FileSystem\TblModel.vb"

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

    '     Class Family
    ' 
    '         Properties: Descriptions, Domains, Family, FamilyLike, PfamString
    '                     Trace
    ' 
    '         Function: __trace, GetTrace, IsThisFamily, ToString
    '         Enum MatchStates
    ' 
    '             MPMatch, TraceMatch
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: __matchTraceDef, (+2 Overloads) CreateObject, GetName
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Linq.JoinExtensions
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Xfam
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment

Namespace FileSystem

    Public Class Family

        <XmlAttribute> Public Property Family As String
        Public Property Domains As String()
        <XmlElement> Public Property PfamString As PfamString()
            Get
                Return _pfamString
            End Get
            Set(value As PfamString())
                _pfamString = value
                _trace = Nothing
            End Set
        End Property

        Dim _pfamString As PfamString()
        Dim _trace As String()

        ''' <summary>
        ''' 与<see cref="Domains"/>不同的是，这个超过60的蛋白质拥有这个结构域其就会被记录在这里
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>全是小写的</remarks>
        Public ReadOnly Property Trace As String()
            Get
                If _trace Is Nothing Then
                    If _pfamString.IsNullOrEmpty Then
                        _trace = New String() {}
                    Else
                        _trace = __trace(_pfamString)
                    End If
                End If

                Return _trace
            End Get
        End Property

        Public Property Descriptions As KeyValuePair()

        Public Shared Function GetTrace(Family As Family) As String()
            Return __trace(Family.PfamString)
        End Function

        Private Shared Function __trace(pfam As PfamString()) As String()
            Dim Pfams As String() = pfam.Select(Function(x) x.PfamString.Split("+"c).Select(Function(pf) pf.Split("("c).First)).ToVector
            Dim Groups = (From x As String In Pfams Select x Group x By x.ToLower Into Group).ToArray
            Dim l As Integer = pfam.Length * 0.95
            Dim lst As String() = (From pf In Groups
                                   Let co As Integer = pf.Group.Count
                                   Where co >= l
                                   Select co, pf.ToLower
                                   Order By co Descending).ToArray.Take(3).Select(Function(x) x.ToLower)
            Return lst
        End Function

        Public ReadOnly Property FamilyLike As Boolean
            Get
                Return Family.First = "*"c
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Family
        End Function

        ''' <summary>
        ''' 分类注释的核心函数
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="threshold"></param>
        ''' <param name="highlyThreshold"></param>
        ''' <returns></returns>
        Public Function IsThisFamily(query As Pfam.PfamString.PfamString,
                                     threshold As Double,
                                     highlyThreshold As Double,
                                     accept As Integer,
                                     Optional strict As Boolean = True) As MatchStates
            Dim accu As Integer

            For Each subject As FileSystem.PfamString In PfamString
                Dim alignment As LevAlign =
                    Pfam.ProteinDomainArchitecture.MPAlignment.Algorithm.PfamStringEquals(
                        query, subject.AsPfamString, highlyThreshold)

                If alignment.MatchSimilarity > threshold Then  ' 结构上面的相似度大于阈值，则累积加1
                    accu += 1
                    If accu >= accept Then
                        Return MatchStates.MPMatch
                    End If
                End If
            Next

            ' 假若没有满足，但是数据库之中的记录全部比对上了，则肯定是
            If accu = PfamString.Length Then
                Return MatchStates.MPMatch
            Else
                If strict Then
                    Return MatchStates.NotMatch
                End If
            End If

            Return __matchTraceDef(query)
        End Function

        Public Enum MatchStates As Integer
            ''' <summary>
            ''' 不是这个家族的
            ''' </summary>
            NotMatch = 0
            ''' <summary>
            ''' 完全匹配上
            ''' </summary>
            MPMatch
            ''' <summary>
            ''' 只匹配上一个家族之中的结构域
            ''' </summary>
            TraceMatch
        End Enum

        Public Function GetName(result As MatchStates) As String
            If Me.FamilyLike AndAlso result <> MatchStates.NotMatch Then
                Return Mid(Me.Family, 2) & "-like"
            End If

            Select Case result
                Case MatchStates.NotMatch : Return ""
                Case MatchStates.MPMatch : Return Me.Family
                Case MatchStates.TraceMatch : Return Me.Family & "-like"
                Case Else
                    Return ""
            End Select
        End Function

        ''' <summary>
        ''' 只要是有该结构域的就算是该家族的蛋白质？？
        ''' </summary>
        ''' <remarks>
        ''' 虽然在MPAlignment比对操作的时候也会知道有至少一个结构域被比对上，但是并不确定那个结构域是否为家族特有的结构域所以在MP比对之中无法进行判断，所以需要在这里进行判断
        ''' </remarks>
        ''' <param name="query"></param>
        ''' <returns></returns>
        Private Function __matchTraceDef(query As Pfam.PfamString.PfamString) As MatchStates
            Dim queryDomains As String() = query.PfamString.Select(Function(x) x.Split("("c).First.ToLower)
            ' 查看是否有交集
            Dim LQuery As Integer = (From x As String
                                     In queryDomains
                                     Where Array.IndexOf(Trace, x) > -1
                                     Select 100).FirstOrDefault
            If LQuery > 0 Then
                Return MatchStates.TraceMatch
            Else
                Return MatchStates.NotMatch
            End If
        End Function

        Public Shared Function CreateObject(Name As String, proteins As PfamString(), Optional describ As KeyValuePair() = Nothing) As Family
            Dim Domains As String() = proteins _
                .Select(Function(x) x.Domains) _
                .IteratesALL _
                .Select(Function(x) x.Split(":"c).First) _
                .Distinct _
                .ToArray
            Return New Family With {
                .Family = Name,
                .Domains = Domains,
                .PfamString = proteins.OrderBy(Function(x) x.PfamString).ToArray,
                .Descriptions = describ
            }
        End Function

        Public Shared Function CreateObject(Name As String, proteins As Pfam.PfamString.PfamString()) As Family
            Dim prot As PfamString() = proteins _
                .Where(Function(x)
                           Return Not x.PfamString.IsNullOrEmpty
                       End Function) _
                .Select(Function(x)
                            Return FileSystem.PfamString.CreateObject(x)
                        End Function) _
                .ToArray

            Return CreateObject(Name, prot)
        End Function
    End Class
End Namespace
