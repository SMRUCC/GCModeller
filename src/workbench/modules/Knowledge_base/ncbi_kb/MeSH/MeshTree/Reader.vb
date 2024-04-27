#Region "Microsoft.VisualBasic::e31b544a6f635e954e2793340b83d941, G:/GCModeller/src/workbench/modules/Knowledge_base/ncbi_kb//MeSH/MeshTree/Reader.vb"

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


    ' Code Statistics:

    '   Total Lines: 82
    '    Code Lines: 65
    ' Comment Lines: 2
    '   Blank Lines: 15
    '     File Size: 2.92 KB


    '     Module Reader
    ' 
    '         Function: ParseCategory, (+3 Overloads) ParseTree, (+2 Overloads) ReadTerms
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Values

Namespace MeSH.Tree

    Public Module Reader

        Public Function ParseCategory(tree As String) As MeshCategory
            Static category_chars As Dictionary(Of Char, MeshCategory) = Enums(Of MeshCategory)() _
                .ToDictionary(Function(c)
                                  Return MeshCategory.C.ToString.First
                              End Function)

            Return category_chars(tree.ToUpper.First)
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ReadTerms(s As Stream) As IEnumerable(Of Term)
            Return ReadTerms(New StreamReader(s))
        End Function

        Public Function ParseTree(file As String) As Tree(Of Term)
            Using s As Stream = file.OpenReadonly
                Return ParseTree(New StreamReader(s))
            End Using
        End Function

        Public Function ParseTree(file As Stream) As Tree(Of Term)
            Return ParseTree(New StreamReader(file))
        End Function

        Private Iterator Function ReadTerms(file As StreamReader) As IEnumerable(Of Term)
            Dim line As Value(Of String) = ""
            Dim str As String()
            Dim term As Term

            Do While Not (line = file.ReadLine) Is Nothing
                str = line.Split(";"c)
                term = New Term With {
                    .term = str(Scan0),
                    .tree = str(1).Split("."c)
                }

                Yield term
            Loop
        End Function

        Public Function ParseTree(file As StreamReader) As Tree(Of Term)
            Dim tree As New Tree(Of Term) With {
                .Data = New Term With {.term = "/", .tree = {}},
                .Childs = New Dictionary(Of String, Tree(Of Term)),
                .label = "NCBI MeSH"
            }
            Dim node As Tree(Of Term)

            For Each term As Term In ReadTerms(file)
                ' reset the node to tree root
                node = tree
                ' walk the tree path
                For Each lv As String In term.tree
                    If Not node.Childs.ContainsKey(lv) Then
                        node.Childs.Add(lv, New Tree(Of Term) With {
                            .Childs = New Dictionary(Of String, Tree(Of Term)),
                            .label = lv,
                            .Parent = node
                        })
                    End If

                    node = node.Childs(lv)
                Next

                node.Data = term
                node.label = term.term
            Next

            Return tree
        End Function
    End Module
End Namespace
