#Region "Microsoft.VisualBasic::1c1a4ea5f3792ac7fdbf28542d714f89, DataMySql\kb_UniProtKB\MySqlImports.vb"

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

    '     Module MySqlImports
    ' 
    '         Function: DumpMySQL, DumpMySQLProject, ImportsUniProtKB, PopulateData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Scripting
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology

Namespace kb_UniProtKB

    Public Module MySqlImports

        ''' <summary>
        ''' Create mysql database dump from <see cref="UniProtXML"/> database
        ''' </summary>
        ''' <param name="uniprot">
        ''' For imports a ultra large size XML database, using linq method <see cref="UniProtXML.EnumerateEntries"/>
        ''' </param>
        ''' <returns></returns>
        <Extension>
        Public Function ImportsUniProtKB(uniprot As IEnumerable(Of entry)) As Dictionary(Of String, MySQLTable())
            Dim hashCodes As New List(Of MySQLTable)
            Dim altIDs As New List(Of MySQLTable)

            Dim proteinFunctions As New List(Of MySQLTable)
            Dim alternativeNames As New List(Of MySQLTable)
            Dim GOfunctions As New List(Of MySQLTable)
            Dim KOfunctions As New List(Of MySQLTable)
            Dim geneNames As New List(Of MySQLTable)

            Dim peoples As New List(Of MySQLTable)
            Dim citation As New List(Of MySQLTable)
            Dim jobs As New List(Of MySQLTable)
            Dim keywords As New List(Of MySQLTable)
            Dim proteinKeywords As New List(Of MySQLTable)
            Dim proteinReferences As New List(Of MySQLTable)
            Dim scopes As New List(Of MySQLTable)
            Dim reference_scopes As New List(Of MySQLTable)

            Dim featureSites As New List(Of MySQLTable)
            Dim featureRegions As New List(Of MySQLTable)
            Dim featureVariations As New List(Of MySQLTable)
            Dim featureTypes As New List(Of MySQLTable)

            Dim organism As New List(Of MySQLTable)
            Dim proteome As New List(Of MySQLTable)

            Dim tissues As New List(Of MySQLTable)
            Dim proteinTissueLocations As New List(Of MySQLTable)

            Dim subcellularLocations As New List(Of MySQLTable)
            Dim locations As New List(Of MySQLTable)
            Dim topologies As New List(Of MySQLTable)

            Dim x As MySQLTable

            For Each x In uniprot.PopulateData
                Select Case x.GetType.Name
                    Case NameOf(mysql.alt_id)
                        altIDs += x
                    Case NameOf(mysql.feature_site_variation)
                        featureVariations += x
                    Case NameOf(mysql.feature_types)
                        featureTypes += x
                    Case NameOf(mysql.gene_info)
                        geneNames += x
                    Case NameOf(mysql.hashcode_scopes)
                        scopes += x
                    Case NameOf(mysql.hash_table)
                        hashCodes += x
                    Case NameOf(mysql.keywords)
                        keywords += x
                    Case NameOf(mysql.literature)
                        citation += x
                    Case NameOf(mysql.location_id)
                        locations += x
                    Case NameOf(mysql.organism_code)
                        organism += x
                    Case NameOf(mysql.organism_proteome)
                        proteome += x
                    Case NameOf(mysql.peoples)
                        peoples += x
                    Case NameOf(mysql.protein_alternative_name)
                        alternativeNames += x
                    Case NameOf(mysql.protein_feature_regions)
                        featureRegions += x
                    Case NameOf(mysql.protein_feature_site)
                        featureSites += x
                    Case NameOf(mysql.protein_functions)
                        proteinFunctions += x
                    Case NameOf(mysql.protein_go)
                        GOfunctions += x
                    Case NameOf(mysql.protein_keywords)
                        proteinKeywords += x
                    Case NameOf(mysql.protein_ko)
                        KOfunctions += x
                    Case NameOf(mysql.protein_reference)
                        proteinReferences += x
                    Case NameOf(mysql.protein_reference_scopes)
                        reference_scopes += x
                    Case NameOf(mysql.protein_subcellular_location)
                        subcellularLocations += x
                    Case NameOf(mysql.research_jobs)
                        jobs += x
                    Case NameOf(mysql.tissue_code)
                        tissues += x
                    Case NameOf(mysql.tissue_locations)
                        proteinTissueLocations += x
                    Case NameOf(mysql.topology_id)
                        topologies += x
                    Case Else
                        Throw New NotSupportedException(x.GetType.Name)
                End Select
            Next

            With New Dictionary(Of String, MySQLTable())

                .ByRef(NameOf(mysql.hash_table)) = hashCodes
                .ByRef(NameOf(mysql.protein_functions)) = proteinFunctions
                .ByRef(NameOf(mysql.alt_id)) = altIDs
                .ByRef(NameOf(mysql.protein_go)) = GOfunctions
                .ByRef(NameOf(mysql.protein_ko)) = KOfunctions
                .ByRef(NameOf(mysql.protein_alternative_name)) = alternativeNames
                .ByRef(NameOf(mysql.gene_info)) = geneNames

                .ByRef(NameOf(mysql.peoples)) = peoples
                .ByRef(NameOf(mysql.literature)) = citation
                .ByRef(NameOf(mysql.research_jobs)) = jobs
                .ByRef(NameOf(mysql.keywords)) = keywords
                .ByRef(NameOf(mysql.protein_keywords)) = proteinKeywords

                .ByRef(NameOf(mysql.protein_reference)) = proteinReferences
                .ByRef(NameOf(mysql.hashcode_scopes)) = scopes
                .ByRef(NameOf(mysql.protein_reference_scopes)) = reference_scopes

                .ByRef(NameOf(mysql.protein_feature_site)) = featureSites
                .ByRef(NameOf(mysql.protein_feature_regions)) = featureRegions
                .ByRef(NameOf(mysql.feature_site_variation)) = featureVariations
                .ByRef(NameOf(mysql.feature_types)) = featureTypes

                .ByRef(NameOf(mysql.organism_code)) = organism
                .ByRef(NameOf(mysql.organism_proteome)) = proteome

                .ByRef(NameOf(mysql.tissue_code)) = tissues
                .ByRef(NameOf(mysql.tissue_locations)) = proteinTissueLocations

                .ByRef(NameOf(mysql.protein_subcellular_location)) = subcellularLocations
                .ByRef(NameOf(mysql.location_id)) = locations
                .ByRef(NameOf(mysql.topology_id)) = topologies

                Return .ByRef
            End With
        End Function

        ''' <summary>
        ''' For a ultra large size datatset export solution.
        ''' </summary>
        ''' <param name="uniprot"></param>
        ''' <returns></returns>
        <Extension>
        Public Iterator Function PopulateData(uniprot As IEnumerable(Of entry)) As IEnumerable(Of MySQLTable)
            Dim hashCodes As New Dictionary(Of String, Long)
            Dim altNameId As i32 = 1
            Dim peoples As New Dictionary(Of String, Long)
            Dim citations As New Dictionary(Of String, Long)
            Dim proteinReferences As i32 = 1
            Dim scopes As New Dictionary(Of String, Long)
            Dim keywords As New Dictionary(Of String, Long)
            Dim featureTypes As New Dictionary(Of String, Long)
            Dim featureSites As i32 = 1
            Dim featureRegions As i32 = 1
            Dim organism As New Dictionary(Of String, Long)
            Dim tissues As New Dictionary(Of String, Long)
            Dim topologies As New Dictionary(Of String, Long)
            Dim locations As New Dictionary(Of String, Long)
            Dim subCellularLocations As i32 = 1
            Dim peopleJobs As New Index(Of String)
            Dim uniqueTissueLocations As New Index(Of String)
            Dim uniqueKey$

            For Each protein As entry In uniprot
                Dim uniprotID$ = protein.accessions.First
                Dim hashcode&

#Region "UniProt ID"
                For Each acc As String In protein.accessions
                    If Not hashCodes.ContainsKey(acc) Then
                        Call hashCodes.Add(acc, hashCodes.Count)

                        Yield New mysql.hash_table With {
                            .hash_code = hashCodes(acc),
                            .name = protein.name,
                            .uniprot_id = acc
                        }

                        If uniprotID = acc Then
                            hashcode = hashCodes(uniprotID) ' .hash_code
                        End If
                    End If

                    If Not acc = uniprotID Then
                        Yield New mysql.alt_id With {
                            .alt_id = hashCodes(acc),' .hash_code,
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
                Dim getRecommendedShortName =
                    Function(i As Integer)
                        Return recommendedName.shortNames _
                            .ElementAtOrDefault(i) _
                           ?.value _
                            .MySqlEscaping
                    End Function

                Yield New mysql.protein_functions With {
                    .full_name = fullName.MySqlEscaping,
                    .function = protein.comments _
                        .Where(Function(c) c.type = "function") _
                        .FirstOrDefault _
                        ?.text _
                        ?.value _
                        .MySqlEscaping,
                    .hash_code = hashcode,
                    .name = protein.name,
                    .uniprot_id = uniprotID,
                    .short_name1 = getRecommendedShortName(0).MySqlEscaping,
                    .short_name2 = getRecommendedShortName(1).MySqlEscaping,
                    .short_name3 = getRecommendedShortName(2).MySqlEscaping
                }

                For Each altName As mysql.protein_alternative_name In
                    From alt As recommendedName
                    In protein _
                        .protein _
                        .alternativeNames _
                        .SafeQuery
                    Let altFullName = alt.fullName _
                        ?.value _
                         .MySqlEscaping
                    Let getShortName = Function(index As Integer)
                                           Return alt.shortNames _
                                               .ElementAtOrDefault(index) _
                                              ?.value _
                                               .MySqlEscaping
                                       End Function
                    Select New mysql.protein_alternative_name With {
                        .fullName = altFullName.MySqlEscaping,
                        .hash_code = hashcode,
                        .name = protein.name,
                        .uniprot_id = uniprotID,
                        .shortName1 = getShortName(0).MySqlEscaping,
                        .shortName2 = getShortName(1).MySqlEscaping,
                        .shortName3 = getShortName(2).MySqlEscaping,
                        .shortName4 = getShortName(3).MySqlEscaping,
                        .shortName5 = getShortName(4).MySqlEscaping,
                        .uid = ++altNameId
                    }

                    Yield altName
                Next

                For Each go As dbReference In protein.Xrefs.TryGetValue("GO").SafeQuery
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

                    Dim go_class As New mysql.protein_go With {
                        .go_id = go.id.Split(":"c).Last,
                        .hash_code = hashcode,
                        .namespace = OntologyNamespaces([namespace]),
                        .namespace_id = [namespace],
                        .uniprot_id = uniprotID,
                        .GO_term = go.id,
                        .term_name = term.MySqlEscaping
                    }

                    Yield go_class
                Next

                For Each ko As dbReference In protein.Xrefs _
                    .TryGetValue("KO") _
                    .SafeQuery

                    Dim ko_class As New mysql.protein_ko With {
                        .hash_code = hashcode,
                        .KO = ko.id.Match("\d+"),
                        .uniprot_id = uniprotID
                    }

                    Yield ko_class
                Next

                If Not protein.gene Is Nothing Then
                    Dim gene As gene = protein.gene
                    Dim synNames = gene.IDs("synonym")

                    Yield New mysql.gene_info With {
                        .gene_name = gene.Primary?.FirstOrDefault.MySqlEscaping,
                        .hash_code = hashcode,
                        .ORF = gene.ORF?.FirstOrDefault.MySqlEscaping,
                        .uniprot_id = uniprotID,
                        .synonym1 = synNames.ElementAtOrDefault(0).MySqlEscaping,
                        .synonym2 = synNames.ElementAtOrDefault(1).MySqlEscaping,
                        .synonym3 = synNames.ElementAtOrDefault(2).MySqlEscaping
                    }
                End If
#End Region
#Region "literature works"
                For Each ref As reference In protein.references
                    For Each name As person In ref.citation.authorList
                        Dim personName$ = name.name.MySqlEscaping

                        If Not peoples.ContainsKey(personName) Then
                            Call peoples.Add(personName, peoples.Count)

                            Yield New mysql.peoples With {
                                .name = personName,
                                .uid = peoples(personName)
                            }
                        End If
                    Next

                    Dim cite As citation = ref.citation
                    Dim citeTitle$ = If(cite.title, uniprotID & ": " & cite.type).MySqlEscaping

                    If Not citations.ContainsKey(citeTitle) Then
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

                        Call citations.Add(citeTitle, citations.Count)

                        Yield New mysql.literature With {
                            .date = cite.date.MySqlEscaping,
                            .db = cite.db.MySqlEscaping,
                            .journal = cite.name.MySqlEscaping,
                            .volume = cite.volume.MySqlEscaping,
                            .title = cite.title.MySqlEscaping,
                            .type = cite.type.MySqlEscaping,
                            .uid = citations(citeTitle),
                            .pages = $"{cite.first} - {cite.last}",
                            .doi = doi.MySqlEscaping,
                            .pubmed = pubmed.MySqlEscaping
                        }

                        jobID = citations(citeTitle) ' .uid

                        For Each job As mysql.research_jobs In
                            From people As person
                            In cite _
                                .authorList _
                                .SafeQuery
                            Let name = people.name.MySqlEscaping
                            Let personID = peoples(name)
                            Select New mysql.research_jobs With {
                                .literature_id = jobID,
                                .literature_title = cite.title.MySqlEscaping,
                                .people_name = name,
                                .person = personID  ' .uid
                            }

                            uniqueKey = $"{job.literature_id} - {job.person}"

                            If peopleJobs.IndexOf(uniqueKey) > -1 Then
                                Continue For
                            Else
                                peopleJobs.Add(uniqueKey)
                            End If

                            Yield job
                        Next
                    End If

                    Dim refID& = ++proteinReferences

                    Yield New mysql.protein_reference With {
                        .hash_code = hashcode,
                        .reference_id = citations(citeTitle),' .uid,
                        .uniprot_id = uniprotID,
                        .uid = refID,
                        .literature_title = cite.title.MySqlEscaping
                    }

                    For Each scope As String In ref.scope _
                        .SafeQuery _
                        .Select(AddressOf MySqlEscaping)

                        If Not scopes.ContainsKey(scope) Then
                            Call scopes.Add(scope, scopes.Count)

                            Yield New mysql.hashcode_scopes With {
                                .scope = scope,
                                .uid = scopes(scope)
                            }
                        End If

                        Yield New mysql.protein_reference_scopes With {
                            .scope = scope,
                            .scope_id = scopes(scope),' .uid,
                            .uid = refID,
                            .uniprot_hashcode = hashcode,
                            .uniprot_id = uniprotID
                        }
                    Next
                Next

                For Each keyword In protein.keywords.SafeQuery
                    Dim word$ = keyword.value.MySqlEscaping
                    Dim id& = keyword.id.Split("-"c).Last

                    If Not keywords.ContainsKey(word) Then
                        Call keywords.Add(word, id)

                        Yield New mysql.keywords With {
                            .keyword = word,
                            .uid = id
                        }
                    End If

                    Yield New mysql.protein_keywords With {
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
                        Call featureTypes.Add(feature.type, featureTypes.Count)

                        Yield New mysql.feature_types With {
                            .type_name = feature.type,
                            .uid = featureTypes(feature.type)
                        }
                    End If

                    If feature.location.IsSite Then
                        Dim featureID& = ++featureSites

                        Yield New mysql.protein_feature_site With {
                            .description = feature.description.MySqlEscaping,
                            .hash_code = hashcode,
                            .position = feature.location.position.position,
                            .type = feature.type,
                            .type_id = featureTypes(feature.type),' .uid,
                            .uid = featureID,
                            .uniprot_id = uniprotID
                        }

                        If Not (feature.original.StringEmpty AndAlso feature.variation.StringEmpty) Then
                            Yield New mysql.feature_site_variation With {
                                .hash_code = hashcode,
                                .original = feature.original,
                                .position = feature.location.position.position,
                                .uid = featureID,
                                .uniprot_id = uniprotID,
                                .variation = feature.variation
                            }
                        End If
                    Else
                        Yield New mysql.protein_feature_regions With {
                            .begin = feature.location.begin.position,
                            .description = feature.description.MySqlEscaping,
                            .end = feature.location.end.position,
                            .hash_code = hashcode,
                            .type = feature.type,
                            .type_id = featureTypes(feature.type),' .uid,
                            .uniprot_id = uniprotID,
                            .uid = ++featureRegions
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
                    Dim ncbi_id As String = protein.NCBITaxonomyId

                    If Not ncbi_id.StringEmpty Then
                        Call organism.Add(organismScientificName, Long.Parse(ncbi_id))

                        Yield New mysql.organism_code With {
                            .organism_name = organismScientificName.MySqlEscaping,
                            .uid = organism(organismScientificName)
                        }
                    End If
                End If

                Dim proteomesInfo As dbReference = protein _
                    .dbReferences _
                    .Where(Function(r) r.type = "Proteomes") _
                    .FirstOrDefault
                Dim chr$ = If(
                    proteomesInfo Is Nothing,
                    "",
                    proteomesInfo.properties _
                        .SafeQuery _
                        .Where(Function(p) p.type = "component") _
                        .FirstOrDefault _
                       ?.value)

                Yield New mysql.organism_proteome With {
                    .gene_name = protein.name,
                    .id_hashcode = hashcode,
                    .org_id = organism(organismScientificName),' .uid,
                    .uniprot_id = uniprotID,
                    .proteomes_id = If(proteomesInfo Is Nothing, "", proteomesInfo.id),
                    .component = chr.MySqlEscaping
                }
#End Region
#Region "tissue locations"
                Dim tissue$
                Dim tissueID&

                For Each ref As reference In protein.references
                    If ref.source Is Nothing OrElse ref.source.tissues.IsNullOrEmpty Then
                        Continue For
                    End If

                    For Each name In ref.source.tissues
                        tissue = organismScientificName & "+" & name

                        If Not tissues.ContainsKey(tissue) Then
                            Call tissues.Add(tissue, tissues.Count)

                            Yield New mysql.tissue_code With {
                                .organism = organismScientificName.MySqlEscaping,
                                .org_id = organism(organismScientificName),' .uid,
                                .tissue_name = name,
                                .uid = tissues(tissue)
                            }
                        End If

                        tissueID = tissues(tissue)
                        uniqueKey = $"{hashcode} - {tissueID}"

                        If uniqueTissueLocations.IndexOf(uniqueKey) > -1 Then
                            Continue For
                        Else
                            uniqueTissueLocations.Add(uniqueKey)
                        End If

                        Yield New mysql.tissue_locations With {
                            .hash_code = hashcode,
                            .name = protein.name.MySqlEscaping,
                            .tissue_id = tissueID,' .uid,
                            .tissue_name = name.MySqlEscaping,
                            .uniprot_id = uniprotID
                        }
                    Next
                Next
#End Region
#Region "subcellular locations"
                Dim subcellular_locations = protein _
                    .comments _
                    .Where(Function(c) c.type = "subcellular location") _
                    .ToArray
                Dim topologyName$

                For Each sublocation As subcellularLocation In subcellular_locations _
                    .Select(Function(c) c.subcellularLocations) _
                    .IteratesALL

                    If Not sublocation.topology Is Nothing Then
                        topologyName = sublocation.topology.value.MySqlEscaping

                        If Not topologies.ContainsKey(topologyName) Then
                            Call topologies.Add(
                                topologyName, topologies.Count)

                            Yield New mysql.topology_id With {
                                .name = topologyName,
                                .uid = topologies(.name)
                            }
                        End If
                    End If

                    For Each name In sublocation.locations
                        If Not locations.ContainsKey(name.value) Then
                            Call locations.Add(name.value, locations.Count)

                            Yield New mysql.location_id With {
                                .name = name.value,
                                .uid = locations(.name)
                            }
                        End If

                        Yield New mysql.protein_subcellular_location With {
                            .hash_code = hashcode,
                            .location = name.value,
                            .location_id = locations(name.value),' .uid,
                            .topology = sublocation.topology _
                                ?.value _
                                    .MySqlEscaping,
                            .topology_id = If(
                                sublocation.topology Is Nothing,
                                -1,
                                topologies(.topology)),' .uid),
                            .uniprot_id = uniprotID,
                            .uid = ++subCellularLocations
                        }
                    Next
                Next
#End Region
            Next
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

        <Extension>
        Public Function DumpMySQLProject(uniprot As IEnumerable(Of entry), EXPORT$) As Boolean
            Try
                Call uniprot.PopulateData.ProjectDumping(EXPORT,, singleTransaction:=True)
            Catch ex As Exception
                ex = New Exception(EXPORT, ex)
                Throw ex
            End Try

            Return True
        End Function
    End Module
End Namespace
