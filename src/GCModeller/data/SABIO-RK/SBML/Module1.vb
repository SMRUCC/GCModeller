Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.MIME.application.rdf_xml

Namespace SBML

    Module Module1

        <Extension>
        Private Iterator Function getIdentifiers(react As SBMLReaction) As IEnumerable(Of NamedValue(Of String))
            Dim annotation = react.annotation.RDF.description

            If Not annotation.is.IsNullOrEmpty Then
                For Each ref In annotation.is
                    Yield ref.Bag _
                        .list(Scan0) _
                        .getIdentifier("is")
                Next
            End If

            If Not annotation.isVersionOf Is Nothing Then
                Yield annotation.isVersionOf _
                    .Bag _
                    .list(Scan0) _
                    .getIdentifier(NameOf(annotation.isVersionOf))
            End If
        End Function

        <Extension>
        Public Function getIdentifier(ref As li, Optional type$ = "is") As NamedValue(Of String)
            Dim tokens = ref.resource.Split("/"c).AsList

            Return New NamedValue(Of String) With {
                .Description = type,
                .Name = tokens(-2),
                .Value = tokens(-1)
            }
        End Function
    End Module
End Namespace