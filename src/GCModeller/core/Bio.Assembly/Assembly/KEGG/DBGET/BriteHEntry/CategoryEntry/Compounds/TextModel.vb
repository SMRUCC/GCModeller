Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module CompoundTextModel

        <Extension>
        Public Function Build(model As BriteHText) As CompoundBrite()
            Dim list As New List(Of CompoundBrite)

            Select Case model.Degree

                Case "C"c
                    For Each [class] As BriteHText In model.CategoryItems

                        If [class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If

                            list += From htext As BriteHText
                                    In category.CategoryItems
                                    Select New CompoundBrite With {
                                        .class = [class].ClassLabel,
                                        .category = category.ClassLabel,
                                        .entry = New KeyValuePair With {
                                            .Key = htext.EntryId,
                                            .Value = htext.Description
                                        }
                                    }
                        Next
                    Next

                Case "D"c
                    For Each [class] As BriteHText In model.CategoryItems

                        If [class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.CategoryItems
                                If subCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If

                                list += From br As BriteHText
                                        In subCategory.CategoryItems
                                        Select New CompoundBrite With {
                                            .class = [class].ClassLabel,
                                            .category = category.ClassLabel,
                                            .subcategory = subCategory.ClassLabel,
                                            .entry = New KeyValuePair With {
                                                .Key = br.EntryId,
                                                .Value = br.Description
                                            }
                                        }
                            Next
                        Next
                    Next

                Case "E"c
                    For Each [class] As BriteHText In model.CategoryItems
                        If [class].CategoryItems.IsNullOrEmpty Then
                            Continue For
                        End If
                        For Each category As BriteHText In [class].CategoryItems
                            If category.CategoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.CategoryItems
                                If subCategory.CategoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If
                                For Each order As BriteHText In subCategory.CategoryItems
                                    If order.CategoryItems.IsNullOrEmpty Then
                                        Continue For
                                    End If

                                    list += From br As BriteHText
                                            In order.CategoryItems
                                            Select New CompoundBrite With {
                                                .class = [class].ClassLabel,
                                                .category = category.ClassLabel,
                                                .subcategory = subCategory.ClassLabel,
                                                .order = order.ClassLabel,
                                                .entry = New KeyValuePair With {
                                                    .Key = br.EntryId,
                                                    .Value = br.Description
                                                }
                                            }

                                Next
                            Next
                        Next
                    Next
            End Select

            Return list.ToArray
        End Function
    End Module
End Namespace