Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Module TreeParser

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Deflate(htext As BriteHText, entryIDPattern$) As IEnumerable(Of BriteTerm)
            Return htext.deflateInternal(entryIDPattern).IteratesALL
        End Function

        <Extension>
        Private Function deflateInternal(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of CompoundBrite))
            Select Case model.degree
                Case "C"c : Return model.deflate_C(entryIDPattern)
                Case "D"c : Return model.deflate_D(entryIDPattern)
                Case "E"c : Return model.deflate_E(entryIDPattern)
                Case Else
                    Throw New NotImplementedException(model.degree)
            End Select
        End Function

        <Extension>
        Private Iterator Function deflate_C(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of CompoundBrite))
            For Each [class] As BriteHText In model.categoryItems
                If [class].categoryItems.IsNullOrEmpty Then
                    If [class].classLabel.StartsWith(entryIDPattern, RegexICSng) Then

                    End If

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
        End Function

        <Extension>
        Private Iterator Function deflate_D(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of CompoundBrite))
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
        End Function

        <Extension>
        Private Iterator Function deflate_E(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of CompoundBrite))
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
        End Function
    End Module
End Namespace