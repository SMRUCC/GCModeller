#Region "Microsoft.VisualBasic::76ed6690602461ba99c937af1227a908, Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Compounds\TextModel.vb"

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

    '     Module CompoundTextModel
    ' 
    '         Function: Build, buildInternal
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
