                #Region "Microsoft.VisualBasic::251271aa66be0058524faba54437287d, core\Bio.Assembly\Assembly\ELIXIR\UniProt\Web\Entry.vb"

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

    '   Total Lines: 86
    '    Code Lines: 64 (74.42%)
    ' Comment Lines: 6 (6.98%)
    '    - Xml Docs: 50.00%
    ' 
    '   Blank Lines: 16 (18.60%)
    '     File Size: 4.67 KB


    '     Class Entry
    ' 
    '         Properties: Annotation, Catalytic_activity, Caution, EC_number, Entry
    '                     EntryName, GeneNames, GeneNames_ordered_locus, GeneNames_ORF, GeneNames_primary
    '                     GeneNames_synonym, GeneOntology_cellular_component, Intramembrane, KeywordID, Keywords
    '                     Length, Organism, PlantReactome, Protein_names, Reactome
    '                     Reviewed, Subcellular_location_CC, Tissue_specificity, Topological_domain, Transmembrane
    ' 
    '         Function: ParseSubcellularLocation, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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

        Public Shared Iterator Function ParseSubcellularLocation(protein As Entry) As IEnumerable(Of NamedCollection(Of String))
            ' "SUBCELLULAR LOCATION: Cytoplasm {ECO:0000256|RuleBase:RU367123}. Golgi apparatus, trans-Golgi network membrane {ECO:0000256|ARBA:ARBA00004150, ECO:0000256|RuleBase:RU367123}; Peripheral membrane protein {ECO:0000256|ARBA:ARBA00004150, ECO:0000256|RuleBase:RU367123}. Endosome membrane {ECO:0000256|ARBA:ARBA00004481, ECO:0000256|RuleBase:RU367123}; Peripheral membrane protein {ECO:0000256|ARBA:ARBA00004481, ECO:0000256|RuleBase:RU367123}. Endoplasmic reticulum membrane {ECO:0000256|ARBA:ARBA00004406, ECO:0000256|RuleBase:RU367123}; Peripheral membrane protein {ECO:0000256|ARBA:ARBA00004406, ECO:0000256|RuleBase:RU367123}. Mitochondrion membrane {ECO:0000256|ARBA:ARBA00004318, ECO:0000256|RuleBase:RU367123}; Peripheral membrane protein {ECO:0000256|ARBA:ARBA00004318, ECO:0000256|RuleBase:RU367123}. Cytoplasmic vesicle, autophagosome {ECO:0000256|RuleBase:RU367123}. Nucleus {ECO:0000256|ARBA:ARBA00004123}."

            Dim list1 = Strings.Trim(protein.Subcellular_location_CC).Replace("SUBCELLULAR LOCATION:", "").Trim.StringSplit("[.;]\s*")
            Dim list2 = protein.GeneOntology_cellular_component.StringSplit("[;]\s*")

            For Each loc As String In list1
                If loc <> "" Then
                    Dim evidence = loc.Match("[{].+[}]")

                    ' invalid format?
                    If evidence = "" Then
                        Continue For
                    End If

                    Dim name As String = loc.Replace(evidence, "").Trim

                    If name = "" Then
                        Continue For
                    End If

                    Yield New NamedCollection(Of String)(name, evidence.GetStackValue("{", "}").StringSplit("[,]\s*"))
                End If
            Next

            For Each loc As String In list2
                If loc <> "" Then
                    Dim evidence = loc.Match("\[.+\]")

                    ' invalid format?
                    If evidence = "" Then
                        Continue For
                    End If

                    Dim name As String = loc.Replace(evidence, "").Trim

                    If name = "" Then
                        Continue For
                    End If

                    Yield New NamedCollection(Of String)(name, evidence.GetStackValue("[", "]").StringSplit("[;,]\s*"))
                End If
            Next
        End Function
    End Class
End Namespace
