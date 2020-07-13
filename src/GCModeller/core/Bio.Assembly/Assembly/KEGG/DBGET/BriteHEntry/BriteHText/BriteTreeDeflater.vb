#Region "Microsoft.VisualBasic::342093cde88e320f44fe862ab8a5b073, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\BriteHText\BriteTreeDeflater.vb"

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

    '     Module BriteTreeDeflater
    ' 
    '         Function: (+2 Overloads) Deflate, deflate_C, deflate_D, deflate_E, deflateInternal
    '                   incompleteTerm
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.BriteHEntry

    Public Module BriteTreeDeflater

        Const NA$ = "N/A"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Deflate(htext As htext, entryIDPattern$) As IEnumerable(Of BriteTerm)
            Return htext.Hierarchical.deflateInternal(entryIDPattern).IteratesALL
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Deflate(htext As BriteHText, entryIDPattern$) As IEnumerable(Of BriteTerm)
            Return htext.deflateInternal(entryIDPattern).IteratesALL
        End Function

        <Extension>
        Private Function deflateInternal(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of BriteTerm))
            Select Case model.degree
                Case "C"c : Return model.deflate_C(entryIDPattern)
                Case "D"c : Return model.deflate_D(entryIDPattern)
                Case "E"c : Return model.deflate_E(entryIDPattern)
                Case Else
                    Throw New NotImplementedException(model.degree)
            End Select
        End Function

        <Extension>
        Private Function incompleteTerm(possibleTerm As BriteHText, entryIDPattern$,
                                        Optional [class] As BriteHText = Nothing,
                                        Optional category As BriteHText = Nothing,
                                        Optional subCategory As BriteHText = Nothing,
                                        Optional order As BriteHText = Nothing) As BriteTerm()

            If Not possibleTerm.classLabel.StartsWith(entryIDPattern, RegexICSng) Then
                Return {}
            End If

            Dim termEntry = possibleTerm.classLabel.GetTagValue(" ", trim:=True)
            Dim term As New BriteTerm With {.entry = termEntry}

            term.class = If([class]?.classLabel, NA)
            term.category = If(category?.classLabel, NA)
            term.subcategory = If(subCategory?.classLabel, NA)
            term.order = If(order?.classLabel, NA)

            Return {term}
        End Function

        <Extension>
        Private Iterator Function deflate_C(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of BriteTerm))
            For Each [class] As BriteHText In model.categoryItems
                If [class].categoryItems.IsNullOrEmpty Then
                    Yield [class].incompleteTerm(entryIDPattern)
                    Continue For
                End If

                For Each category As BriteHText In [class].categoryItems
                    If category.categoryItems.IsNullOrEmpty Then
                        Yield category.incompleteTerm(entryIDPattern, [class])
                        Continue For
                    End If

                    Yield From htext As BriteHText
                          In category.categoryItems
                          Select New BriteTerm With {
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
        Private Iterator Function deflate_D(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of BriteTerm))
            For Each [class] As BriteHText In model.categoryItems
                If [class].categoryItems.IsNullOrEmpty Then
                    Yield [class].incompleteTerm(entryIDPattern)
                    Continue For
                End If

                For Each category As BriteHText In [class].categoryItems
                    If category.categoryItems.IsNullOrEmpty Then
                        Yield category.incompleteTerm(entryIDPattern, [class])
                        Continue For
                    End If
                    For Each subCategory As BriteHText In category.categoryItems
                        If subCategory.categoryItems.IsNullOrEmpty Then
                            Yield subCategory.incompleteTerm(entryIDPattern, [class], category)
                            Continue For
                        End If

                        Yield From br As BriteHText
                              In subCategory.categoryItems
                              Select New BriteTerm With {
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
        Private Iterator Function deflate_E(model As BriteHText, entryIDPattern$) As IEnumerable(Of IEnumerable(Of BriteTerm))
            For Each [class] As BriteHText In model.categoryItems
                If [class].categoryItems.IsNullOrEmpty Then
                    Yield [class].incompleteTerm(entryIDPattern)
                    Continue For
                End If
                For Each category As BriteHText In [class].categoryItems
                    If category.categoryItems.IsNullOrEmpty Then
                        Yield category.incompleteTerm(entryIDPattern, [class])
                        Continue For
                    End If
                    For Each subCategory As BriteHText In category.categoryItems
                        If subCategory.categoryItems.IsNullOrEmpty Then
                            Yield subCategory.incompleteTerm(entryIDPattern, [class], category)
                            Continue For
                        End If
                        For Each order As BriteHText In subCategory.categoryItems
                            If order.categoryItems.IsNullOrEmpty Then
                                Yield order.incompleteTerm(entryIDPattern, [class], category, subCategory)
                                Continue For
                            End If

                            Yield From br As BriteHText
                                  In order.categoryItems
                                  Select New BriteTerm With {
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
