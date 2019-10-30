Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Linq

Namespace CatalogProfiling

    Module Extensions

        <Extension>
        Public Function GetColors(profile As Dictionary(Of String, NamedValue(Of Double)()), colorSchema$) As ColorProfile
            If colorSchema.IsPattern("scale\(.+\)") Then
                colorSchema = colorSchema.GetStackValue("(", ")")
                Return New ValueScaleColorProfile(profile.Values.IteratesALL, colorSchema, 30)
            Else
                Dim category As New Dictionary(Of String, String)

                For Each profileGroup In profile
                    For Each term As NamedValue(Of Double) In profileGroup.Value
                        category(term.Name) = profileGroup.Key
                    Next
                Next

                Return New CategoryColorProfile(category, colorSchema)
            End If
        End Function

    End Module

End Namespace