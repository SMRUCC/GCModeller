#Region "Microsoft.VisualBasic::fc0e014b6e34e5ec62c7587294b49989, GCModeller\models\Networks\Microbiome\UniProt\TaxonomyRef.vb"

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

    '   Total Lines: 112
    '    Code Lines: 70
    ' Comment Lines: 24
    '   Blank Lines: 18
    '     File Size: 3.19 KB


    ' Class TaxonomyRef
    ' 
    '     Properties: coverage, genome, KOTerms, numberOfGenes, organism
    '                 subcellular_components, taxonID, TaxonomyString
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: ToString
    ' 
    ' Class SubCellularLocation
    ' 
    '     Properties: locations
    ' 
    '     Function: getCollection, getSize
    ' 
    ' Class Location
    ' 
    '     Properties: name, proteins
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Metagenomics
Imports SMRUCC.genomics.Model.Network.Microbiome

''' <summary>
''' Combine the UniProt taxonomy information with the KEGG orthology reference.
''' </summary>
Public Class TaxonomyRef : Inherits XmlDataModel
    Implements IKeyedEntity(Of String)

    ''' <summary>
    ''' The NCBI taxonomy id
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <XmlElement("ncbi_taxon_id")>
    Public Property taxonID As String Implements IKeyedEntity(Of String).Key

    ''' <summary>
    ''' 具有KEGG直系同源注释结果的基因的数量和该基因组内的总基因数量的比值结果
    ''' </summary>
    ''' <returns></returns>
    <XmlElement>
    Public Property coverage As Double
    Public Property numberOfGenes As Integer
    Public Property organism As organism
    Public Property genome As OrthologyTerms
    Public Property subcellular_components As SubCellularLocation

    Dim ts As Taxonomy

    Public ReadOnly Property TaxonomyString As Taxonomy
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            ' IsFalse not equals not operator??
            If ts Then
                ' Do Nothing
            Else
                ts = New Taxonomy(organism.lineage.taxonlist)
            End If

            Return ts
        End Get
    End Property

    Public ReadOnly Property KOTerms As String()
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return genome _
                .Terms _
                .SafeQuery _
                .Select(Function(t) t.name) _
                .ToArray
        End Get
    End Property

    <XmlNamespaceDeclarations()>
    Public xmlnsImports As XmlSerializerNamespaces

    Public Sub New()
        xmlnsImports = New XmlSerializerNamespaces
        xmlnsImports.Add("KO", OrthologyTerms.Xmlns)
    End Sub

    Public Overrides Function ToString() As String
        Return $"[{taxonID}] {organism.scientificName}"
    End Function
End Class

Public Class SubCellularLocation : Inherits ListOf(Of Location)

    <XmlElement>
    Public Property locations As Location()

    Protected Overrides Function getSize() As Integer
        Return locations.Length
    End Function

    Protected Overrides Function getCollection() As IEnumerable(Of Location)
        Return locations
    End Function
End Class

Public Class Location

    ''' <summary>
    ''' The location name
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <XmlAttribute>
    Public Property name As String
    ''' <summary>
    ''' The protein id list
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <XmlElement("protein")>
    Public Property proteins As NamedValue()

    Public Overrides Function ToString() As String
        Return name
    End Function

End Class
