Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module CompoundTextModel

        <Extension>
        Public Iterator Function Build(model As BriteHText) As IEnumerable(Of CompoundBrite)
            For Each part As IEnumerable(Of CompoundBrite) In model.buildInternal
                For Each level As CompoundBrite In part
                    Yield level
                Next
            Next
        End Function

        <Extension>
        Private Iterator Function buildInternal(model As BriteHText) As IEnumerable(Of IEnumerable(Of CompoundBrite))
            Select Case model.degree

                Case "C"c
                    For Each [class] As BriteHText In model.categoryItems

                        If [class].categoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [class].categoryItems
                            If category.categoryItems.IsNullOrEmpty Then
                                Continue For
                            End If

                            Yield From htext As BriteHText
                                  In category.categoryItems
                                  Select New CompoundBrite With {
                                      .class = [class].classLabel,
                                      .category = category.classLabel,
                                      .entry = New KeyValuePair With {
                                           .Key = htext.entryID,
                                           .Value = htext.description
                                      }
                                  }
                        Next
                    Next

                Case "D"c
                    For Each [class] As BriteHText In model.categoryItems

                        If [class].categoryItems.IsNullOrEmpty Then
                            Continue For
                        End If

                        For Each category As BriteHText In [class].categoryItems
                            If category.categoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.categoryItems
                                If subCategory.categoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If

                                Yield From br As BriteHText
                                      In subCategory.categoryItems
                                      Select New CompoundBrite With {
                                           .class = [class].classLabel,
                                           .category = category.classLabel,
                                           .subcategory = subCategory.classLabel,
                                           .entry = New KeyValuePair With {
                                                .Key = br.entryID,
                                                .Value = br.description
                                           }
                                      }
                            Next
                        Next
                    Next

                Case "E"c
                    For Each [class] As BriteHText In model.categoryItems
                        If [class].categoryItems.IsNullOrEmpty Then
                            Continue For
                        End If
                        For Each category As BriteHText In [class].categoryItems
                            If category.categoryItems.IsNullOrEmpty Then
                                Continue For
                            End If
                            For Each subCategory As BriteHText In category.categoryItems
                                If subCategory.categoryItems.IsNullOrEmpty Then
                                    Continue For
                                End If
                                For Each order As BriteHText In subCategory.categoryItems
                                    If order.categoryItems.IsNullOrEmpty Then
                                        Continue For
                                    End If

                                    Yield From br As BriteHText
                                          In order.categoryItems
                                          Select New CompoundBrite With {
                                              .class = [class].classLabel,
                                              .category = category.classLabel,
                                              .subcategory = subCategory.classLabel,
                                              .order = order.classLabel,
                                              .entry = New KeyValuePair With {
                                                  .Key = br.entryID,
                                                  .Value = br.description
                                              }
                                          }

                                Next
                            Next
                        Next
                    Next
            End Select
        End Function
    End Module
End Namespace