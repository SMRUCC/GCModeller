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

            For Each entry As entry In uniprot
                Dim uniprotID$ = entry.accessions.First
                Dim hashcode&

                For Each acc As String In entry.accessions
                    If Not hashCodes.ContainsKey(acc) Then
                        hashCodes.Add(acc, New mysql.hash_table With {.hash_code = hashCodes.Count, .name = entry.name, .uniprot_id = acc})
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

                For Each ref As reference In entry.reference

                Next
            Next

                Dim mysqlTables As New Dictionary(Of String, SQLTable())

            mysqlTables(NameOf(mysql.hash_table)) = hashCodes.Values.ToArray
            mysqlTables(NameOf(mysql.protein_functions)) = proteinFunctions
            mysqlTables(NameOf(mysql.alt_id)) = altIDs
            mysqlTables(NameOf(mysql.protein_go)) = GOfunctions
            mysqlTables(NameOf(mysql.protein_ko)) = KOfunctions
            mysqlTables(NameOf(mysql.peoples)) = peoples.Values.ToArray

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