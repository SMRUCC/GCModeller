Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.foundation.OBO_Foundry

Public Module DATA

    ''' <summary>
    ''' 从``*.obo``格式的GO数据库文件之中导入为mysql数据库数据
    ''' </summary>
    ''' <param name="obo"></param>
    ''' <returns></returns>
    <Extension> Public Function ImportsMySQL(obo As OBOFile)
        Dim namespaces As New Dictionary(Of String, kb_go.term_namespace)
        Dim relationNames As New Dictionary(Of String, kb_go.relation_names)
        Dim go_terms As New Dictionary(Of String, kb_go.go_terms)
        Dim dag As New List(Of kb_go.dag_relationship)
        Dim xrefList As New List(Of kb_go.xref)
        Dim synonymNames As New List(Of kb_go.term_synonym)

        With namespaces
            !cellular_component = New kb_go.term_namespace With {.id = Ontologies.CellularComponent, .namespace = Ontologies.CellularComponent.Description}
            !biological_process = New kb_go.term_namespace With {.id = Ontologies.BiologicalProcess, .namespace = Ontologies.BiologicalProcess.Description}
            !molecular_function = New kb_go.term_namespace With {.id = Ontologies.MolecularFunction, .namespace = Ontologies.MolecularFunction.Description}
        End With

        For Each term As Term In obo.EnumerateGOTerms
            Dim id& = term.id.Split(":"c).Last

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

            If Not term.is_a.IsNullOrEmpty Then

            End If

            If Not term.xref.IsNullOrEmpty Then

            End If
        Next
    End Function
End Module
