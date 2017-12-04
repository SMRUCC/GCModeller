#Region "Microsoft.VisualBasic::3ededfd62967d161cb7b8369c8cbd35f, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\Organism.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' br08601
    ''' </summary>
    Public Module Organism

        <Extension> Public Function FillTaxonomyTable(organisms As htext) As Taxonomy()
            Dim out As New List(Of Taxonomy)
            Dim levels As New Dictionary(Of Char, String)
            Dim h As BriteHText
            Dim sp$

            For Each htext As BriteHText In organisms _
                .Hierarchical _
                .EnumerateEntries _
                .Where(Function(hE) hE.CategoryLevel = "E"c)

                h = htext

                Do While h.CategoryLevel <> "/"c
                    h = h.Parent
                    levels(h.CategoryLevel) = h.ClassLabel
                Loop

                sp = htext.ClassLabel
                sp = sp.Split.First
                out += New Taxonomy With {
                    .scientificName = Mid(htext.ClassLabel, sp.Length + 1).Trim,
                    .species = sp,
                    .genus = levels.TryGetValue("D"c),
                    .family = levels.TryGetValue("C"c),
                    .order = levels.TryGetValue("B"c),
                    .class = levels.TryGetValue("A"c)
                }
                Call levels.Clear()
            Next

            Return out
        End Function
    End Module
End Namespace
