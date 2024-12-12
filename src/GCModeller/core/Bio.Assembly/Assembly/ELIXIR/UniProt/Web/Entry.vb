#Region "Microsoft.VisualBasic::2b016c17b772b5bfa649eed22fddfa41, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Entry.vb"

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

'   Total Lines: 18
'    Code Lines: 15 (83.33%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 3 (16.67%)
'     File Size: 572 B


'     Class Entry
' 
'         Properties: Entry, EntryName, GeneNames, Length, Organism
'                     ProteinNames, StatusReviewed
' 
'         Function: ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps

Namespace Assembly.Uniprot.Web

    ''' <summary>
    ''' tabular export of the protein list in tsv format
    ''' </summary>
    Public Class Entry

        Public Property Entry As String
        Public Property Reviewed As String
        <Column("Entry Name")> Public Property EntryName As String
        <Column("Protein names")> Public Property Protein_names As String
        <Column("Gene Names")> Public Property GeneNames As String
        Public Property Organism As String
        Public Property Length As String
        <Column("Gene Names (synonym)")> Public Property GeneNames_synonym As String
        <Column("Gene Names (ordered locus)")> Public Property GeneNames_ordered_locus As String
        <Column("Gene Names (ORF)")> Public Property GeneNames_ORF As String
        <Column("Gene Names (primary)")> Public Property GeneNames_primary As String
        <Column("EC number")> Public Property EC_number As String
        <Column("Catalytic activity")> Public Property Catalytic_activity As String
        <Column("Keyword ID")> Public Property KeywordID As String
        Public Property Keywords As String
        Public Property Annotation As String
        Public Property Caution As String
        <Column("Tissue specificity")> Public Property Tissue_specificity As String
        <Column("Gene Ontology (cellular component)")> Public Property GeneOntology_cellular_component As String
        Public Property Intramembrane As String
        <Column("Subcellular location [CC]")> Public Property Subcellular_location_CC As String
        Public Property Transmembrane As String
        <Column("Topological domain")> Public Property Topological_domain As String
        Public Property Reactome As String
        Public Property PlantReactome As String

        Public Overrides Function ToString() As String
            Return Entry.ToString
        End Function
    End Class
End Namespace
