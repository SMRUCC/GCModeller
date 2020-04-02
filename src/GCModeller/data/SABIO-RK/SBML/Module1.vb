Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace SBML

    Module Module1

        <Extension>
        Public Iterator Function getIdentifiers(react As SBMLReaction) As IEnumerable(Of NamedValue(Of String))
            Dim annotation = react.annotation.RDF.description
            Dim tokens As List(Of String)

            If Not annotation.is.IsNullOrEmpty Then
                For Each ref In annotation.is
                    tokens = ref.Bag.list(Scan0).resource.Split("/"c).AsList

                    Yield New NamedValue(Of String) With {
                        .Description = "is",
                        .Name = tokens(-2),
                        .Value = tokens(-1)
                    }
                Next
            End If

            If Not annotation.isVersionOf Is Nothing Then
                tokens = annotation.isVersionOf.Bag.list(Scan0).resource.Split("/"c).AsList

                Yield New NamedValue(Of String) With {
                    .Description = NameOf(annotation.isVersionOf),
                    .Name = tokens(-2),
                    .Value = tokens(-1)
                }
            End If
        End Function
    End Module
End Namespace