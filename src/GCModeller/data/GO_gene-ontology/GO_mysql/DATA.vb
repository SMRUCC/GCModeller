#Region "Microsoft.VisualBasic::c93db72daa36a18c58e4e30edadbb495, data\GO_gene-ontology\GO_mysql\DATA.vb"

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

' Module DATA
' 
'     Function: DumpMySQL, ImportsMySQL
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL
Imports Oracle.LinuxCompatibility.MySQL.Scripting
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry.IO.Models

Public Module DATA

    ''' <summary>
    ''' 从``*.obo``格式的GO数据库文件之中导入为mysql数据库数据
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <returns></returns>
    <Extension> Public Function ImportsMySQL(obo As OBOFile) As Dictionary(Of String, MySQLTable())
        Dim namespaces As New Dictionary(Of String, kb_go.term_namespace)
        Dim relationNames As Dictionary(Of String, kb_go.relation_names)
        Dim go_terms As New Dictionary(Of String, kb_go.go_terms)
        Dim dag As New List(Of kb_go.dag_relationship)
        Dim xrefList As New List(Of kb_go.xref)
        Dim synonymNames As New List(Of kb_go.term_synonym)
        Dim altIDs As New List(Of kb_go.alt_id)
        Dim synonymID As i32 = 0

        With namespaces
            !cellular_component = New kb_go.term_namespace With {.id = Ontologies.CellularComponent, .namespace = "cellular_component"}
            !biological_process = New kb_go.term_namespace With {.id = Ontologies.BiologicalProcess, .namespace = "biological_process"}
            !molecular_function = New kb_go.term_namespace With {.id = Ontologies.MolecularFunction, .namespace = "molecular_function"}
        End With

        relationNames = Enums(Of OntologyRelations) _
            .ToDictionary(Function(rel) rel.ToString,
                          Function(rel)
                              Return New kb_go.relation_names With {
                                  .id = rel,
                                  .name = rel.ToString
                              }
                          End Function)

        For Each term As Term In obo.EnumerateGOTerms
            Dim id& = term.id.Split(":"c).Last
            Dim dagNode As TermNode = term.ConstructNode()

            go_terms(term.id) = New kb_go.go_terms With {
                .id = id,
                .term = term.id,
                .comment = term.comment.MySqlEscaping,
                .def = term.def.MySqlEscaping,
                .is_obsolete = term.is_obsolete.ParseBoolean,
                .name = term.name.MySqlEscaping,
                .namespace = term.namespace,
                .namespace_id = namespaces(term.namespace).id
            }

            If Not term.alt_id.IsNullOrEmpty Then
                altIDs += From alid As String
                          In term.alt_id
                          Let id2 As Long = alid.Split(":"c).Last
                          Select New kb_go.alt_id With {
                              .alt_id = id2,
                              .id = id,
                              .name = term.name.MySqlEscaping
                          }
            End If

            If Not dagNode.is_a.IsNullOrEmpty Then
                Dim term_id&

                For Each assert As is_a In dagNode.is_a
                    term_id = assert.term_id.Split(":"c).Last
                    dag += New kb_go.dag_relationship With {
                        .id = id,
                        .term_id = term_id,
                        .name = assert.name.MySqlEscaping,
                        .relationship = NameOf(is_a),
                        .relationship_id = relationNames(.relationship).id
                    }
                Next
            End If
            If Not dagNode.relationship.IsNullOrEmpty Then
                For Each rel In dagNode.relationship
                    dag += New kb_go.dag_relationship With {
                        .id = id,
                        .relationship = rel.type.ToString,
                        .relationship_id = rel.type,
                        .name = rel.parentName.MySqlEscaping,
                        .term_id = rel.parent.Value
                    }
                Next
            End If

            If Not dagNode.xref.IsNullOrEmpty Then
                xrefList += From ref As NamedValue(Of String)
                            In dagNode.xref
                            Select New kb_go.xref With {
                                .xref = ref.Name,
                                .comment = ref.Description.MySqlEscaping,
                                .external_id = ref.Value.MySqlEscaping,
                                .go_id = id
                            }
            End If

            If Not dagNode.synonym.IsNullOrEmpty Then
                synonymNames += From name As synonym
                                In dagNode.synonym
                                Let obj As String = name.synonym.Description
                                Select New kb_go.term_synonym With {
                                    .id = ++synonymID,
                                    .synonym = name.name.MySqlEscaping,
                                    .term_id = id,
                                    .object = obj,
                                    .type = name.type
                                }
            End If
        Next

        Dim GO As New Dictionary(Of String, MySQLTable())

        GO(NameOf(kb_go.alt_id)) = altIDs
        GO(NameOf(kb_go.dag_relationship)) = dag
        GO(NameOf(kb_go.go_terms)) = go_terms.Values.ToArray
        GO(NameOf(kb_go.relation_names)) = relationNames.Values.ToArray
        GO(NameOf(kb_go.term_namespace)) = namespaces.Values.ToArray
        GO(NameOf(kb_go.term_synonym)) = synonymNames
        GO(NameOf(kb_go.xref)) = xrefList

        Return GO
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <param name="saveSQL$">The file path of the saved sql file.</param>
    ''' <returns></returns>
    <Extension>
    Public Function DumpMySQL(obo As OBOFile, saveSQL$) As Boolean
        Using writer As StreamWriter = saveSQL.OpenWriter
            With obo.ImportsMySQL.Values
                Call writer.DumpMySQL(.ToArray)
            End With

            Return True
        End Using
    End Function
End Module
