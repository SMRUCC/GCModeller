#Region "Microsoft.VisualBasic::36c177ae9bca4bf2fd550df54a82daf4, ..\GCModeller\models\Networks\Microbiome\UniProt\TaxonomyRef.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' Combine the UniProt taxonomy information with the KEGG orthology reference.
''' </summary>
Public Class TaxonomyRef : Implements IKeyedEntity(Of String)

    ''' <summary>
    ''' The NCBI taxonomy id
    ''' </summary>
    ''' <returns></returns>
    ''' 
    <XmlAttribute("ncbi_taxon_id")>
    Public Property TaxonID As String Implements IKeyedEntity(Of String).Key
    Public Property organism As organism
    Public Property genome As OrthologyTerms
    ''' <summary>
    ''' 具有KEGG直系同源注释结果的基因的数量和该基因组内的总基因数量的比值结果
    ''' </summary>
    ''' <returns></returns>
    <XmlAttribute>
    Public Property Coverage As Double

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

    Public Overrides Function ToString() As String
        Return $"[{TaxonID}] {organism.scientificName}"
    End Function
End Class
