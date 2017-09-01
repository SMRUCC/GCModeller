Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL
Imports SMRUCC.genomics.Data.GeneOntology.DAG
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module DATA

    ''' <summary>
    ''' 从``*.obo``格式的GO数据库文件之中导入为mysql数据库数据
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <returns></returns>
    <Extension> Public Function ImportsMySQL(obo As OBOFile) As Dictionary(Of String, SQLTable())
        Dim namespaces As New Dictionary(Of String, kb_go.term_namespace)
        Dim relationNames As New Dictionary(Of String, kb_go.relation_names)
        Dim go_terms As New Dictionary(Of String, kb_go.go_terms)
        Dim dag As New List(Of kb_go.dag_relationship)
        Dim xrefList As New List(Of kb_go.xref)
        Dim synonymNames As New List(Of kb_go.term_synonym)

        With namespaces
            !cellular_component = New kb_go.term_namespace With {.id = Ontologies.CellularComponent, .namespace = "cellular_component"}
            !biological_process = New kb_go.term_namespace With {.id = Ontologies.BiologicalProcess, .namespace = "biological_process"}
            !molecular_function = New kb_go.term_namespace With {.id = Ontologies.MolecularFunction, .namespace = "molecular_function"}
        End With

        With relationNames
            !is_a = New kb_go.relation_names With {
                .id = 0,
                .name = NameOf(is_a)
            }
        End With

        Dim relsID = Function(name$)
                         If Not relationNames.ContainsKey(name) Then
                             relationNames(name) = New kb_go.relation_names With {
                                 .id = relationNames.Count,
                                 .name = name
                             }
                         End If

                         Return relationNames(name).id
                     End Function

        For Each term As Term In obo.EnumerateGOTerms
            Dim id& = term.id.Split(":"c).Last
            Dim dagNode As TermNode = term.ConstructNode()

            go_terms(term.id) = New kb_go.go_terms With {
                .id = id,
                .term = term.id,
                .comment = term.comment,
                .def = term.def,
                .is_obsolete = term.is_obsolete.ParseBoolean,
                .name = term.name,
                .namespace = term.namespace,
                .namespace_id = namespaces(term.namespace).id
            }

            If Not term.alt_id.IsNullOrEmpty Then
                For Each alid In term.alt_id

                Next
            End If

            If Not dagNode.is_a.IsNullOrEmpty Then
                Dim term_id&

                For Each assert As is_a In dagNode.is_a
                    term_id = assert.term_id.Split(":"c).Last
                    dag += New kb_go.dag_relationship With {
                        .id = id,
                        .term_id = term_id,
                        .name = assert.name,
                        .relationship = NameOf(is_a),
                        .relationship_id = relsID(.relationship)
                    }
                Next
            End If

            If Not dagNode.xref.IsNullOrEmpty Then
                xrefList += From ref As NamedValue(Of String)
                            In dagNode.xref
                            Select New kb_go.xref With {
                                .xref = ref.Name,
                                .comment = ref.Description,
                                .external_id = ref.Value,
                                .go_id = id
                            }
            End If
        Next
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
            With obo.ImportsMySQL
                Call writer.DumpMySQL(.Values.ToArray)
            End With

            Return True
        End Using
    End Function
End Module
