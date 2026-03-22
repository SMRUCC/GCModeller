#Region "Microsoft.VisualBasic::3e758b1a1d9bc050a92eac45db671223, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\Data.vb"

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

    '   Total Lines: 492
    '    Code Lines: 299 (60.77%)
    ' Comment Lines: 125 (25.41%)
    '    - Xml Docs: 96.80%
    ' 
    '   Blank Lines: 68 (13.82%)
    '     File Size: 17.02 KB


    '     Class sequence
    ' 
    '         Properties: checksum, length, mass, modified, sequence
    '                     version
    ' 
    '         Function: ToString
    ' 
    '     Class gene
    ' 
    '         Properties: names, ORF, Primary
    ' 
    '         Function: HaveKey, ToString
    ' 
    '     Class organism
    ' 
    '         Properties: commonName, dbReference, evidence, lineage, names
    '                     namesData, scientificName
    ' 
    '     Class lineage
    ' 
    '         Properties: taxonlist
    ' 
    '         Function: ToString
    ' 
    '     Class protein
    ' 
    '         Properties: alternativeNames, fullName, recommendedName, submittedName
    ' 
    '         Function: ToString
    ' 
    '     Enum featureTypes
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    '     Class feature
    ' 
    '         Properties: description, evidence, ligand, location, original
    '                     status, type, value, variation
    ' 
    '         Function: ToString
    ' 
    '     Class ligand
    ' 
    '         Properties: dbReference, label, name
    ' 
    '     Class location
    ' 
    '         Properties: [end], begin, IsRegion, IsSite, position
    ' 
    '         Function: ToString
    ' 
    '     Class position
    ' 
    '         Properties: position, status
    ' 
    '         Function: ToString
    ' 
    '     Class recommendedName
    ' 
    '         Properties: ecNumber, fullName, shortNames
    ' 
    '     Class value
    ' 
    '         Properties: description, evidence, id, type, value
    ' 
    '         Function: ToString
    ' 
    '     Class dbReference
    ' 
    '         Properties: id, molecule, properties, type
    ' 
    '         Function: hasDbReference, ToString
    ' 
    '     Structure molecule
    ' 
    '         Properties: id
    ' 
    '         Function: ToString
    ' 
    '     Class [property]
    ' 
    '         Properties: type, value
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.Uniprot.XML

    ''' <summary>
    ''' the protein sequence data
    ''' </summary>
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

    Public Class organism

        <XmlAttribute> Public Property evidence As String()
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

        <XmlElement>
        Public Property dbReference As value()
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

        <XmlElement("taxon")>
        Public Property taxonlist As String()

        Public Overrides Function ToString() As String
            Return taxonlist.GetJson
        End Function
    End Class

    ''' <summary>
    ''' Describes different types of sequence annotations.
    ''' Equivalent to the flat file FT-line.
    ''' Get by types using <see cref="Takes"/> extensions
    ''' </summary>
    Public Class feature : Implements INamedValue

        ''' <summary>
        ''' Describes the type of a sequence annotation.
        ''' Equivalent to the flat file FT feature keys, but using full terms instead 
        ''' of acronyms.
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property type As String Implements INamedValue.Key
        <XmlAttribute> Public Property evidence As String()
        <XmlAttribute> Public Property description As String
        ''' <summary>
        ''' string value could be one of these enumeration:
        ''' 
        ''' + by similarity
        ''' + probable
        ''' + potential
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property status As String

        <XmlText> Public Property value As String

        ''' <summary>
        ''' Describes the original sequence in annotations that describe natural 
        ''' or artifical sequence variations.
        ''' </summary>
        ''' <returns></returns>
        Public Property original As String
        ''' <summary>
        ''' Describes the variant sequence in annotations that describe natural 
        ''' or artifical sequence variations.
        ''' </summary>
        ''' <returns></returns>
        Public Property variation As String
        ''' <summary>
        ''' Describes the sequence coordinates of the annotation.
        ''' </summary>
        ''' <returns></returns>
        Public Property location As location
        Public Property ligand As ligand

        Public Overrides Function ToString() As String
            Return description
        End Function
    End Class

    Public Class ligand

        Public Property name As String
        Public Property label As String
        Public Property dbReference As dbReference

    End Class

    ''' <summary>
    ''' Describes a sequence location as either a range with a begin and end or as a 
    ''' position. The 'sequence' attribute is only used when the location is not on 
    ''' the canonical sequence displayed in the current entry.
    ''' </summary>
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

        Public ReadOnly Property IsRegion As Boolean
            Get
                Return Not (begin Is Nothing AndAlso [end] Is Nothing)
            End Get
        End Property

        Public ReadOnly Property IsSite As Boolean
            Get
                Return Not IsRegion
            End Get
        End Property

        Public Overrides Function ToString() As String
            If IsRegion Then
                Return $"[{begin}, {[end]}]"
            Else
                Return position.position
            End If
        End Function
    End Class

    ''' <summary>
    ''' 序列上面的某一个位点位置
    ''' </summary>
    Public Class position

        <XmlAttribute> Public Property position As Integer
        ''' <summary>
        ''' Value could be one of these enumeration:
        ''' 
        ''' + certain
        ''' + uncertain
        ''' + less than
        ''' + greater than
        ''' + unknown
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property status As String = "certain"

        Public Overrides Function ToString() As String
            Return position
        End Function

        Public Shared Narrowing Operator CType(pos As position) As Integer
            Return pos.position
        End Operator
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

    ''' <summary>
    ''' 一条值数据记录
    ''' </summary>
    Public Class value : Implements Value(Of String).IValueOf

        <XmlAttribute> Public Property type As String
        <XmlAttribute> Public Property evidence As String()
        <XmlAttribute> Public Property description As String
        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' 这条值对象的文本内容
        ''' </summary>
        ''' <returns></returns>
        <XmlText>
        Public Property value As String Implements Value(Of String).IValueOf.Value

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Narrowing Operator CType(val As value) As String
            Return val.value
        End Operator
    End Class

    ''' <summary>
    ''' Describes a molecule by name or unique identifier.
    ''' </summary>
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
