#Region "Microsoft.VisualBasic::9c02d9e75b8a2a4aeaa3dc174469e889, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\BriteHEntry\CategoryEntry\Organism.vb"

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

    '   Total Lines: 96
    '    Code Lines: 73
    ' Comment Lines: 10
    '   Blank Lines: 13
    '     File Size: 3.28 KB


    '     Module Organism
    ' 
    '         Function: (+2 Overloads) FillTaxonomyTable, GetBacteriaList, GetResource, IsSpeciesLevel
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Metagenomics

Namespace Assembly.KEGG.DBGET.BriteHEntry

    ''' <summary>
    ''' ### br08601
    ''' 
    ''' > http://www.kegg.jp/kegg-bin/download_htext?htext=br08601.keg&amp;format=htext&amp;filedir=
    ''' </summary>
    Public Module Organism

        Public Const HtextKey$ = "br08601"

        ''' <summary>
        ''' 从卫星资源程序集之中加载数据库数据
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Load resource using <see cref="ResourcesSatellite"/></remarks>
        Public Function GetResource() As htext
            Dim res$ = GetType(Organism) _
                .Assembly _
                .ResourcesSatellite() _
                .GetString(Organism.HtextKey)
            Dim htext As htext = htext.StreamParser(res)
            Return htext
        End Function

        <Extension>
        Public Function GetBacteriaList(resource As htext) As Taxonomy()
            Dim bacteriaAll As BriteHText = resource _
                .Hierarchical _
                .categoryItems _
                .Takes("Prokaryotes\s+\(\d+\)") _
                .First _
                .categoryItems _
                .Takes("Bacteria\s+\(\d+\)") _
                .First
            Dim table = bacteriaAll _
                .EnumerateEntries _
                .Where(AddressOf IsSpeciesLevel) _
                .FillTaxonomyTable

            Return table
        End Function

        <Extension>
        Public Function FillTaxonomyTable(OrgCodes As IEnumerable(Of BriteHText)) As Taxonomy()
            Dim out As New List(Of Taxonomy)
            Dim levels As New Dictionary(Of Char, String)
            Dim sp As String, hc As BriteHText
            Dim name$

            For Each code As BriteHText In OrgCodes
                hc = code

                Do While hc.CategoryLevel <> "/"c
                    hc = hc.parent
                    levels(hc.CategoryLevel) = hc.classLabel
                Loop

                name = code.classLabel
                sp = name
                sp = sp.Split.First
                name = Mid(name, sp.Length + 1).Trim
                out += New Taxonomy With {
                    .scientificName = name,
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

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function IsSpeciesLevel(htext As BriteHText) As Boolean
            Return htext.CategoryLevel = "E"c
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension> Public Function FillTaxonomyTable(organisms As htext) As Taxonomy()
            Return organisms _
                .Hierarchical _
                .EnumerateEntries _
                .Where(AddressOf IsSpeciesLevel) _
                .FillTaxonomyTable
        End Function
    End Module
End Namespace
