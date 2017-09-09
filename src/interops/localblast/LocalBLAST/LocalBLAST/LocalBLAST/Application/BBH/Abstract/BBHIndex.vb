#Region "Microsoft.VisualBasic::37f0878693aff92f491195a607fc5f5e, ..\localblast\LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Abstract\BBHIndex.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
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

Imports Microsoft.VisualBasic.ComponentModel.KeyValuePair
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace LocalBLAST.Application.BBH.Abstract

    ''' <summary>
    ''' 可以使用这个对象来表述<see cref="I_BlastQueryHit"/>的所有派生类
    ''' </summary>
    Public Class BBHIndex : Inherits I_BlastQueryHit
        Implements IKeyValuePair
        Implements IQueryHits

        Public Property identities As Double Implements IQueryHits.identities

        ''' <summary>
        ''' 动态属性
        ''' </summary>
        ''' <returns></returns>
        <Meta(GetType(String))>
        Public Property Properties As Dictionary(Of String, String)
            Get
                If _Properties Is Nothing Then
                    _Properties = New Dictionary(Of String, String)
                End If
                Return _Properties
            End Get
            Set(value As Dictionary(Of String, String))
                _Properties = value
            End Set
        End Property

        Dim _Properties As Dictionary(Of String, String)

        ''' <summary>
        ''' 请注意这个属性进行字典的读取的时候，假若不存在，则会返回空字符串，不会报错
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Default Public Property [Property](Name As String) As String
            Get
                If Properties.ContainsKey(Name) Then
                    Return Properties(Name)
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                If Properties.ContainsKey(Name) Then
                    Call Properties.Remove(Name)
                End If
                Properties.Add(Name, value)
            End Set
        End Property

        <Ignored> Public Property Positive As Double
            Get
                Dim p As String = [Property]("Positive")
                If String.IsNullOrEmpty(p) Then
                    p = [Property]("positive")
                End If
                Return Val(p)
            End Get
            Set(value As Double)
                [Property]("Positive") = CStr(value)
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        ''' <summary>
        ''' 从bbh结果里面构建出比对信息的哈希表
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="hitsHash">Using <see cref="BBHIndex.HitName"/> as hash key? Default is using <see cref="BBHIndex.QueryName"/></param>
        ''' <param name="trim">这个函数里面默认是消除了KEGG的物种简写代码的</param>
        ''' <returns></returns>
        Public Shared Function BuildHitsHash(source As IEnumerable(Of BBHIndex),
                                             Optional hitsHash As Boolean = False,
                                             Optional trim As Boolean = True) As Dictionary(Of String, String())
            Dim LQuery As IEnumerable(Of KeyValuePair(Of String, String))

            If trim Then
                LQuery = (From x As BBHIndex
                          In source
                          Where x.Matched
                          Select New KeyValuePair(Of String, String)(x.QueryName.Split(":"c).Last, x.HitName.Split(":"c).Last))
            Else
                LQuery = (From x As BBHIndex
                          In source
                          Select New KeyValuePair(Of String, String)(x.QueryName.Split(":"c).Last, x.HitName.Split(":"c).Last))
            End If

            If hitsHash Then
                Return (From x In LQuery
                        Select x
                        Group x By x.Value Into Group) _
                             .ToDictionary(Function(x) x.Value,
                                           Function(x) (From o In x.Group Select o.Key Distinct).ToArray)
            Else
                Return (From x In LQuery
                        Select x
                        Group x By x.Key Into Group) _
                             .ToDictionary(Function(x) x.Key,
                                           Function(x) (From o In x.Group Select o.Value Distinct).ToArray)
            End If
        End Function
    End Class
End Namespace
