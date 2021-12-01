#Region "Microsoft.VisualBasic::fa26e591bc4517e1762666d475c073ce, core\Bio.Assembly\Assembly\ELIXIR\UniProt\XML\Model\Data.vb"

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
    '         Properties: description, evidence, location, original, status
    '                     type, value, variation
    ' 
    '         Function: ToString
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
    '         Function: ToString
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

    ''' <summary>
    ''' Describes a gene.
    ''' Equivalent to the flat file GN-line.
    ''' </summary>
    Public Class gene

        ''' <summary>
        ''' Describes different types of gene designations.
        ''' Equivalent to the flat file GN-line.
        ''' </summary>
        ''' <returns></returns>
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
                                      Function(g)
                                          Return g.ToArray
                                      End Function)
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
    ''' Describes the names for the protein and parts thereof.
    ''' Equivalent to the flat file DE-line.
    ''' </summary>
    Public Class protein

        Public Property recommendedName As recommendedName
        Public Property submittedName As recommendedName

        <XmlElement("alternativeName")>
        Public Property alternativeNames As recommendedName()

        ''' <summary>
        ''' <see cref="recommendedName"/> -> <see cref="submittedName"/> -> <see cref="alternativeNames"/>
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property fullName As String
            Get
                If recommendedName Is Nothing OrElse recommendedName.fullName Is Nothing Then
                    If submittedName Is Nothing OrElse submittedName.fullName Is Nothing Then
                        Return alternativeNames.FirstOrDefault().fullName.value
                    Else
                        Return submittedName.fullName.value
                    End If
                Else
                    Return recommendedName.fullName.value
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return fullName
        End Function
    End Class

    ''' <summary>
    ''' Describes the type of a sequence annotation.
    ''' Equivalent to the flat file FT feature keys, but using full terms instead 
    ''' of acronyms. The string value enumeration of <see cref="feature.type"/>
    ''' property.
    ''' </summary>
    Public Enum featureTypes

        <Description("active site")> active_site
        <Description("binding site")> binding_site
        <Description("calcium-binding region")> calcium_binding_region
        <Description("chain")> chain
        <Description("coiled-coil region")> coiled_coil_region
        <Description("compositionally biased region")> compositionally_biased_region
        <Description("cross-link")> cross_link
        <Description("disulfide bond")> disulfide_bond
        <Description("DNA-binding region")> DNA_binding_region
        <Description("domain")> domain
        <Description("glycosylation site")> glycosylation_site
        <Description("helix")> helix
        <Description("initiator methionine")> initiator_methionine
        <Description("lipid moiety-binding region")> lipid_moiety_binding_region
        <Description("metal ion-binding site")> metal_ion_binding_site
        <Description("modified residue")> modified_residue
        <Description("mutagenesis site")> mutagenesis_site
        <Description("non-consecutive residues")> non_consecutive_residues
        <Description("non-terminal residue")> non_terminal_residue
        <Description("nucleotide phosphate-binding region")> nucleotide_phosphate_binding_region
        <Description("peptide")> peptide
        <Description("propeptide")> propeptide
        <Description("region of interest")> region_of_interest
        <Description("repeat")> repeat
        <Description("non-standard amino acid")> non_standard_amino_acid
        <Description("sequence conflict")> sequence_conflict
        <Description("sequence variant")> sequence_variant
        <Description("short sequence motif")> short_sequence_motif
        <Description("signal peptide")> signal_peptide
        <Description("site")> site
        <Description("splice variant")> splice_variant
        <Description("strand")> strand
        <Description("topological domain")> topological_domain
        <Description("transit peptide")> transit_peptide
        <Description("transmembrane region")> transmembrane_region
        <Description("turn")> turn
        <Description("unsure residue")> unsure_residue
        <Description("zinc finger region")> zinc_finger_region
        <Description("intramembrane region")> intramembrane_region

    End Enum

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
        <XmlAttribute> Public Property evidence As String
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

        Public Overrides Function ToString() As String
            Return description
        End Function
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
        <XmlAttribute> Public Property evidence As String
        <XmlAttribute> Public Property description As String
        <XmlAttribute> Public Property id As String

        ''' <summary>
        ''' 这条值对象的文本内容
        ''' </summary>
        ''' <returns></returns>
        <XmlText> Public Property value As String Implements Value(Of String).IValueOf.Value

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Shared Narrowing Operator CType(val As value) As String
            Return val.value
        End Operator
    End Class

    Public Class dbReference

        ''' <summary>
        ''' 外部数据库的名称
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' 对于RefSeq而言，RefSeq的编号是蛋白序列在NCBI数据库之中的编号，如果需要找到对应的核酸编号，
        ''' 则会需要通过<see cref="properties"/>列表之中的``nucleotide sequence ID``键值对来获取
        ''' </remarks>
        <XmlAttribute> Public Property type As String
        ''' <summary>
        ''' 外部数据库的编号
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property id As String
        <XmlElement("property")>
        Public Property properties As [property]()
        Public Property molecule As molecule

        Default Public ReadOnly Property PropertyValue(name$) As String
            Get
                Return properties _
                    .SafeQuery _
                    .Where(Function([property]) [property].type = name) _
                    .FirstOrDefault _
                   ?.value
            End Get
        End Property

        Public Function hasDbReference(dbName As String) As Boolean
            Return Not properties _
                .SafeQuery _
                .Where(Function([property])
                           Return [property].type = dbName
                       End Function) _
                .FirstOrDefault Is Nothing
        End Function

        Public Overrides Function ToString() As String
            Return $"[{type}] {id}"
        End Function

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
