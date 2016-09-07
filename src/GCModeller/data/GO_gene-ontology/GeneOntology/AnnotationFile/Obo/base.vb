Imports SMRUCC.genomics.foundation.OBO_Foundry

Namespace OBO

    Public MustInherit Class base

        ''' <summary>
        ''' Every term has a term name—e.g. mitochondrion, glucose transport, amino acid binding—and a unique zero-padded seven digit 
        ''' identifier (often called the term accession or term accession number) prefixed by GO:, e.g. GO:0005125 or GO:0060092. 
        ''' The numerical portion of the ID has no inherent meaning or relation to the position of the term in the ontologies. 
        ''' Ranges of GO IDs are assigned to individual ontology editors or editing groups, and can thus be used to trace who added the term.
        ''' </summary>
        ''' <returns></returns>
        <Field("id")> Public Property id As String
        <Field("name")> Public Property name As String
        ''' <summary>
        ''' Denotes which of the three sub-ontologies—cellular component, biological process or molecular function—the term belongs to.
        ''' </summary>
        ''' <returns></returns>
        <Field("namespace")> Public Property [namespace] As String
    End Class
End Namespace