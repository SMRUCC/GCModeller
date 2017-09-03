#Region "Microsoft.VisualBasic::f17a86a88246327317f18cb594d6122f, ..\repository\DataMySql\kb_UniProtKB\MySqlImports.vb"

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

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology

Namespace kb_UniProtKB

    Public Module MySqlImports

        ''' <summary>
        ''' Create mysql database dump from <see cref="UniProtXML"/> database
        ''' </summary>
        ''' <param name="uniprot">
        ''' For imports a ultra large size XML database, using linq method <see cref="UniProtXML.EnumerateEntries(String)"/>
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ImportsUniProtKB(uniprot As IEnumerable(Of entry)) As Dictionary(Of String, SQLTable())
            Dim hashCodes As New Dictionary(Of String, mysql.hash_table)
            Dim altIDs As New List(Of mysql.alt_id)

            Dim proteinFunctions As New List(Of mysql.protein_functions)
            Dim GOfunctions As New List(Of mysql.protein_go)
            Dim KOfunctions As New List(Of mysql.protein_ko)

            Dim peoples As New Dictionary(Of String, mysql.peoples)
            Dim citation As New Dictionary(Of String, mysql.literature)
            Dim jobs As New List(Of mysql.research_jobs)
            Dim keywords As New Dictionary(Of String, mysql.keywords)
            Dim proteinKeywords As New List(Of mysql.protein_keywords)
            Dim proteinReferences As New List(Of mysql.protein_reference)

            Dim featureSites As New List(Of mysql.protein_feature_site)
            Dim featureRegions As New List(Of mysql.protein_feature_regions)
            Dim featureVariations As New List(Of mysql.feature_site_variation)
            Dim featureTypes As New Dictionary(Of String, mysql.feature_types)

            Dim organism As New Dictionary(Of String, mysql.organism_code)
            Dim proteome As New List(Of mysql.organism_proteome)

            Dim tissues As New Dictionary(Of String, mysql.tissue_code)
            Dim proteinTissueLocations As New List(Of mysql.tissue_locations)

            For Each protein As entry In uniprot
                Dim uniprotID$ = protein.accessions.First
                Dim hashcode&

#Region "UniProt ID"
                For Each acc As String In protein.accessions
                    If Not hashCodes.ContainsKey(acc) Then
                        Call hashCodes.Add(
                            acc, New mysql.hash_table With {
                                .hash_code = hashCodes.Count,
                                .name = protein.name,
                                .uniprot_id = acc
                            })

                        If uniprotID = acc Then
                            hashcode = hashCodes(uniprotID).hash_code
                        End If
                    End If

                    If Not acc = uniprotID Then
                        altIDs += New mysql.alt_id With {
                            .alt_id = hashCodes(acc).hash_code,
                            .name = protein.name,
                            .primary_hashcode = hashcode,
                            .uniprot_id = acc
                        }
                    End If
                Next
#End Region
#Region "Protein Functions"
                Dim recommendedName = protein.protein.recommendedName
                Dim fullName$ = recommendedName _
                    .fullName _
                    .value _
                    .MySqlEscaping

                proteinFunctions += New mysql.protein_functions With {
                    .full_name = fullName,
                    .function = protein.comments _
                        .Where(Function(c) c.type = "function") _
                        .FirstOrDefault _
                       ?.text.value,
                    .hash_code = hashcode,
                    .name = protein.name,
                    .uniprot_id = uniprotID,
                    .short_name1 = recommendedName.shortNames.ElementAtOrDefault(0)?.value.MySqlEscaping,
                    .short_name2 = recommendedName.shortNames.ElementAtOrDefault(1)?.value.MySqlEscaping,
                    .short_name3 = recommendedName.shortNames.ElementAtOrDefault(2)?.value.MySqlEscaping
                }

                GOfunctions += protein.Xrefs _
                    .TryGetValue("GO") _
                   ?.Select(Function(go)
                                Dim term$ = go.properties _
                                    .Where(Function([property]) [property].type = "term") _
                                    .FirstOrDefault _
                                   ?.value
                                Dim [namespace] As Ontologies

                                Select Case term.Split(":"c).First
                                    Case "C"
                                        [namespace] = Ontologies.CellularComponent
                                    Case "P"
                                        [namespace] = Ontologies.BiologicalProcess
                                    Case "F"
                                        [namespace] = Ontologies.MolecularFunction
                                    Case Else
                                        Throw New InvalidDataException(term)
                                End Select

                                Return New mysql.protein_go With {
                                    .go_id = go.id.Split(":"c).Last,
                                    .hash_code = hashcode,
                                    .namespace = OntologyNamespaces([namespace]),
                                    .namespace_id = [namespace],
                                    .uniprot_id = uniprotID,
                                    .GO_term = go.id,
                                    .term_name = term.MySqlEscaping
                                }
                            End Function)
                KOfunctions += protein.Xrefs _
                    .TryGetValue("KO") _
                   ?.Select(Function(ko)
                                Return New mysql.protein_ko With {
                                    .hash_code = hashcode,
                                    .KO = ko.id.Match("\d+"),
                                    .uniprot_id = uniprotID
                                }
                            End Function)
#End Region
#Region "literature works"
                For Each ref As reference In protein.references
                    For Each name As person In ref.citation.authorList
                        Dim personName$ = name.name.MySqlEscaping

                        If Not peoples.ContainsKey(personName) Then
                            Call peoples.Add(
                                personName, New mysql.peoples With {
                                    .name = personName,
                                    .uid = peoples.Count
                                })
                        End If
                    Next

                    Dim cite As citation = ref.citation
                    Dim citeTitle$ = If(cite.title, uniprotID & ": " & cite.type).MySqlEscaping

                    If Not citation.ContainsKey(citeTitle) Then
                        Dim jobID&
                        Dim doi$ = cite.dbReferences _
                            .SafeQuery _
                            .Where(Function(r) r.type = "DOI") _
                            .FirstOrDefault _
                           ?.id
                        Dim pubmed = cite.dbReferences _
                            .SafeQuery _
                            .Where(Function(r) r.type = "PubMed") _
                            .FirstOrDefault _
                           ?.id

                        Call citation.Add(
                            citeTitle, New mysql.literature With {
                                .date = cite.date,
                                .db = cite.db,
                                .journal = cite.name,
                                .volume = cite.volume,
                                .title = citeTitle,
                                .type = cite.type,
                                .uid = citation.Count,
                                .pages = $"{cite.first} - {cite.last}",
                                .doi = doi,
                                .pubmed = pubmed
                            })

                        jobID = citation(citeTitle).uid
                        jobs += From people As person
                                In cite.authorList.SafeQuery
                                Select New mysql.research_jobs With {
                                    .literature_id = jobID,
                                    .literature_title = citeTitle,
                                    .people_name = people.name.MySqlEscaping,
                                    .person = peoples(.people_name).uid
                                }
                    End If

                    proteinReferences += New mysql.protein_reference With {.hash_code = hashcode, .reference_id = citation(citeTitle).uid, .scope = ref.scope, .uniprot_id = uniprotID}
                Next

                For Each keyword In protein.keywords.SafeQuery
                    Dim word$ = keyword.value.MySqlEscaping
                    Dim id& = keyword.id.Split("-"c).Last

                    If Not keywords.ContainsKey(word) Then
                        Call keywords.Add(
                            word, New mysql.keywords With {
                                .keyword = word,
                                .uid = id
                            })
                    End If

                    proteinKeywords += New mysql.protein_keywords With {
                        .keyword = word,
                        .hash_code = hashcode,
                        .keyword_id = id,
                        .uniprot_id = uniprotID
                    }
                Next
#End Region
#Region "feature sites"
                For Each feature As feature In protein.features.SafeQuery
                    If Not featureTypes.ContainsKey(feature.type) Then
                        Call featureTypes.Add(
                            feature.type, New mysql.feature_types With {
                                .type_name = feature.type,
                                .uid = featureTypes.Count
                            })
                    End If

                    If feature.location.IsSite Then
                        Dim featureID& = featureSites.Count

                        featureSites += New mysql.protein_feature_site With {
                            .description = feature.description.MySqlEscaping,
                            .hash_code = hashcode,
                            .position = feature.location.position.position,
                            .type = feature.type,
                            .type_id = featureTypes(feature.type).uid,
                            .uid = featureID,
                            .uniprot_id = uniprotID
                        }
                        If Not (feature.original.StringEmpty AndAlso feature.variation.StringEmpty) Then
                            featureVariations += New mysql.feature_site_variation With {
                                .hash_code = hashcode,
                                .original = feature.original,
                                .position = feature.location.position.position,
                                .uid = featureID,
                                .uniprot_id = uniprotID,
                                .variation = feature.variation
                            }
                        End If
                    Else
                        Dim featureID& = featureRegions.Count

                        featureRegions += New mysql.protein_feature_regions With {
                            .begin = feature.location.begin.position,
                            .description = feature.description.MySqlEscaping,
                            .end = feature.location.end.position,
                            .hash_code = hashcode,
                            .type = feature.type,
                            .type_id = featureTypes(feature.type).uid,
                            .uniprot_id = uniprotID,
                            .uid = featureRegions.Count
                        }
                    End If
                Next
#End Region
#Region "organism info"
                Dim organismScientificName$ = protein.organism _
                    .names _
                    .Where(Function(t) t.type = "scientific") _
                    .First _
                    .value

                If Not organism.ContainsKey(organismScientificName) Then
                    Call organism.Add(
                        organismScientificName, New mysql.organism_code With {
                            .organism_name = organismScientificName,
                            .uid = protein.organism.dbReference.id
                        })
                End If

                proteome += New mysql.organism_proteome With {
                    .gene_name = protein.name,
                    .id_hashcode = hashcode,
                    .org_id = organism(organismScientificName).uid,
                    .uniprot_id = uniprotID
                }
#End Region
#Region "tissue locations"
                Dim tissue$

                For Each ref As reference In protein.references
                    If ref.source Is Nothing OrElse ref.source.tissues.IsNullOrEmpty Then
                        Continue For
                    End If
                    For Each name In ref.source.tissues
                        tissue = organismScientificName & "+" & name

                        If Not tissues.ContainsKey(tissue) Then
                            Call tissues.Add(
                                tissue, New mysql.tissue_code With {
                                    .organism = organismScientificName,
                                    .org_id = organism(organismScientificName).uid,
                                    .tissue_name = name,
                                    .uid = tissues.Count
                                })
                        End If

                        proteinTissueLocations += New mysql.tissue_locations With {
                            .hash_code = hashcode,
                            .name = protein.name,
                            .tissue_id = tissues(tissue).uid,
                            .tissue_name = name,
                            .uniprot_id = uniprotID
                        }
                    Next
                Next
#End Region
            Next

            Dim mysqlTables As New Dictionary(Of String, SQLTable())

            mysqlTables(NameOf(mysql.hash_table)) = hashCodes.Values.ToArray
            mysqlTables(NameOf(mysql.protein_functions)) = proteinFunctions
            mysqlTables(NameOf(mysql.alt_id)) = altIDs
            mysqlTables(NameOf(mysql.protein_go)) = GOfunctions
            mysqlTables(NameOf(mysql.protein_ko)) = KOfunctions

            mysqlTables(NameOf(mysql.peoples)) = peoples.Values.ToArray
            mysqlTables(NameOf(mysql.literature)) = citation.Values.ToArray
            mysqlTables(NameOf(mysql.research_jobs)) = jobs
            mysqlTables(NameOf(mysql.keywords)) = keywords.Values.ToArray
            mysqlTables(NameOf(mysql.protein_keywords)) = proteinKeywords

            mysqlTables(NameOf(mysql.protein_feature_site)) = featureSites
            mysqlTables(NameOf(mysql.protein_feature_regions)) = featureRegions
            mysqlTables(NameOf(mysql.feature_site_variation)) = featureVariations
            mysqlTables(NameOf(mysql.feature_types)) = featureTypes.Values.ToArray

            mysqlTables(NameOf(mysql.organism_code)) = organism.Values.ToArray
            mysqlTables(NameOf(mysql.organism_proteome)) = proteome

            mysqlTables(NameOf(mysql.tissue_code)) = tissues.Values.ToArray
            mysqlTables(NameOf(mysql.tissue_locations)) = proteinTissueLocations

            Return mysqlTables
        End Function

        ''' <summary>
        ''' Write SQL dumps from uniprot XML database.
        ''' </summary>
        ''' <param name="uniprot"></param>
        ''' <param name="savedSQL$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function DumpMySQL(uniprot As IEnumerable(Of entry), savedSQL$) As Boolean
            Using writer As StreamWriter = savedSQL.OpenWriter
                With uniprot.ImportsUniProtKB.Values
                    Call writer.DumpMySQL(.ToArray)
                End With

                Return True
            End Using
        End Function
    End Module
End Namespace
