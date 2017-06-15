#Region "Microsoft.VisualBasic::3f673e74c955ccb21817e240fc06e6a6, ..\core\Bio.Assembly\Assembly\Uniprot\XML\Model\Data.vb"

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

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    Public Class sequence
        <XmlAttribute> Public Property length As Integer
        <XmlAttribute> Public Property mass As String
        <XmlAttribute> Public Property checksum As String
        <XmlAttribute> Public Property modified As String
        <XmlAttribute> Public Property version As String

        <XmlText> Public Property sequence As String

        Public Overrides Function ToString() As String
            Return sequence
        End Function
    End Class

    Public Class gene

        <XmlElement("name")> Public Property names As value()
            Get
                Return table.Values _
                    .IteratesALL _
                    .ToArray
            End Get
            Set(value As value())
                If value.IsNullOrEmpty Then
                    table = New Dictionary(Of String, value())
                Else
                    ' 会有多种重复的类型
                    table = value _
                        .GroupBy(Function(name) name.type) _
                        .ToDictionary(Function(n) n.Key,
                                      Function(g) g.ToArray)
                End If
            End Set
        End Property

        Dim table As Dictionary(Of String, value())

        Default Public ReadOnly Property IDs(type$) As String()
            Get
                If table.ContainsKey(type) Then
                    Return table(type).ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Function HaveKey(type$) As Boolean
            Return table.ContainsKey(type)
        End Function

        ''' <summary>
        ''' (primary) 基因名称
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Primary As String()
            Get
                If table.ContainsKey("primary") Then
                    Return table("primary").ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' (ORF) 基因编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ORF As String()
            Get
                ' ORF 和 locus编号的含义是一样的

                If table.ContainsKey("ORF") Then
                    Return table("ORF").ValueArray
                ElseIf table.ContainsKey("ordered locus") Then
                    Return table("ordered locus").ValueArray
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class organism

        <XmlAttribute> Public Property evidence As String
        <XmlElement("name")> Public Property names As value()
            Get
                Return namesData.Values.ToArray
            End Get
            Set(value As value())
                If value Is Nothing Then
                    _namesData = New Dictionary(Of String, value)
                Else
                    _namesData = value.ToDictionary(Function(x) x.type)
                End If
            End Set
        End Property

        Public Property dbReference As value
        Public Property lineage As lineage

        <XmlIgnore>
        Public ReadOnly Property namesData As Dictionary(Of String, value)

        <XmlIgnore>
        Public ReadOnly Property scientificName As String
            Get
                If namesData.ContainsKey("scientific") Then
                    Return namesData("scientific").value
                Else
                    Return ""
                End If
            End Get
        End Property

        <XmlIgnore>
        Public ReadOnly Property commonName As String
            Get
                If namesData.ContainsKey("common") Then
                    Return namesData("common").value
                Else
                    Return ""
                End If
            End Get
        End Property
    End Class

    Public Class lineage
        <XmlElement("taxon")> Public Property taxonlist As String()
    End Class

    Public Class protein

        Public Property recommendedName As recommendedName
        Public Property submittedName As recommendedName

        Public ReadOnly Property FullName As String
            Get
                If recommendedName Is Nothing OrElse recommendedName.fullName Is Nothing Then
                    If submittedName Is Nothing OrElse submittedName.fullName Is Nothing Then
                        Return Nothing
                    Else
                        Return submittedName.fullName.value
                    End If
                Else
                    Return recommendedName.fullName.value
                End If
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Get by types using <see cref="Takes"/> extensions
    ''' </summary>
    Public Class feature : Implements INamedValue

        <XmlAttribute> Public Property type As String Implements INamedValue.Key
        <XmlAttribute> Public Property evidence As String
        <XmlAttribute> Public Property description As String
        <XmlText> Public Property value As String
        Public Property location As location
    End Class

    Public Class location
        ''' <summary>
        ''' 片段的起点位置
        ''' </summary>
        ''' <returns></returns>
        Public Property begin As position
        ''' <summary>
        ''' 片段的结束位置
        ''' </summary>
        ''' <returns></returns>
        Public Property [end] As position
        ''' <summary>
        ''' 单个位点的位置
        ''' </summary>
        ''' <returns></returns>
        Public Property position As position
    End Class

    ''' <summary>
    ''' 序列上面的某一个位点位置
    ''' </summary>
    Public Class position

        <XmlAttribute>
        Public Property position As Integer

        Public Overrides Function ToString() As String
            Return position
        End Function
    End Class

    Public Class recommendedName

        Public Property fullName As value

        <XmlElement("shortName")>
        Public Property shortNames As value()
        ''' <summary>
        ''' 一个蛋白可能会有多个EC编号
        ''' </summary>
        ''' <returns></returns>
        <XmlElement("ecNumber")>
        Public Property ecNumber As value()
    End Class

    Public Class value
        Implements Value(Of String).IValueOf

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String
        <XmlAttribute> Public Property description As String
        <XmlAttribute> Public Property id As String
        <XmlText> Public Property value As String Implements Value(Of String).IValueOf.value

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class dbReference

        ''' <summary>
        ''' 外部数据库的名称
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As String
        ''' <summary>
        ''' 外部数据库的编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        <XmlElement("property")>
        Public Property properties As [property]()
        Public Property molecule As molecule

    End Class

    Public Structure molecule

        <XmlAttribute>
        Public Property id As String

        Public Overrides Function ToString() As String
            Return id
        End Function
    End Structure

    Public Class [property]

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property value As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
