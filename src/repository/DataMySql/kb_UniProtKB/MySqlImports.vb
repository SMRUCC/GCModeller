'#Region "Microsoft.VisualBasic::a7eed1c155e0bca688845465caa65348, ..\GCModeller\analysis\annoTools\DataMySql\UniprotSprot\DbTools.vb"

'' Author:
'' 
''       asuka (amethyst.asuka@gcmodeller.org)
''       xieguigang (xie.guigang@live.com)
''       xie (genetics@smrucc.org)
'' 
'' Copyright (c) 2016 GPL3 Licensed
'' 
'' 
'' GNU GENERAL PUBLIC LICENSE (GPL3)
'' 
'' This program is free software: you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation, either version 3 of the License, or
'' (at your option) any later version.
'' 
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
'' 
'' You should have received a copy of the GNU General Public License
'' along with this program. If not, see <http://www.gnu.org/licenses/>.

'#End Region

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
            Dim proteinFunctions As New List(Of mysql.protein_functions)
            Dim altIDs As New List(Of mysql.alt_id)
            Dim GOfunctions As New List(Of mysql.protein_go)
            Dim KOfunctions As New List(Of mysql.protein_ko)
            Dim peoples As New Dictionary(Of String, mysql.peoples)
            Dim citation As New Dictionary(Of String, mysql.literature)
            Dim jobs As New List(Of mysql.research_jobs)

            For Each entry As entry In uniprot
                Dim uniprotID$ = entry.accessions.First
                Dim hashcode&

                For Each acc As String In entry.accessions
                    If Not hashCodes.ContainsKey(acc) Then
                        Call hashCodes.Add(
                            acc, New mysql.hash_table With {
                                .hash_code = hashCodes.Count,
                                .name = entry.name,
                                .uniprot_id = acc
                            })

                        If uniprotID = acc Then
                            hashcode = hashCodes(uniprotID).hash_code
                        End If
                    End If

                    If Not acc = uniprotID Then
                        altIDs += New mysql.alt_id With {
                            .alt_id = hashCodes(acc).hash_code,
                            .name = entry.name,
                            .primary_hashcode = hashcode,
                            .uniprot_id = uniprotID
                        }
                    End If
                Next

                Dim recommendedName = entry.protein.recommendedName
                Dim fullName$ = recommendedName _
                    .fullName _
                    .value _
                    .MySqlEscaping

                proteinFunctions += New mysql.protein_functions With {
                    .full_name = fullName,
                    .function = entry.comments _
                        .Where(Function(c) c.type = "function") _
                        .FirstOrDefault _
                       ?.text.value,
                    .hash_code = hashcode,
                    .name = entry.name,
                    .uniprot_id = uniprotID,
                    .short_name1 = recommendedName.shortNames.ElementAtOrDefault(0)?.value.MySqlEscaping,
                    .short_name2 = recommendedName.shortNames.ElementAtOrDefault(1)?.value.MySqlEscaping,
                    .short_name3 = recommendedName.shortNames.ElementAtOrDefault(2)?.value.MySqlEscaping
                }

                GOfunctions += entry.Xrefs _
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
                                    .uniprot_id = uniprotID
                                }
                            End Function)
                KOfunctions += entry.Xrefs _
                    .TryGetValue("KO") _
                   ?.Select(Function(ko)
                                Return New mysql.protein_ko With {
                                    .hash_code = hashcode,
                                    .KO = ko.id.Match("\d+"),
                                    .uniprot_id = uniprotID
                                }
                            End Function)

                For Each ref As reference In entry.references
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
                Next
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