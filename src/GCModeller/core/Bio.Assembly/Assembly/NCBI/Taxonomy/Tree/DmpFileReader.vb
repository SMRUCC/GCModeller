#Region "Microsoft.VisualBasic::b60c9edfee80f2c232748fa4b9317da9, core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\DmpFileReader.vb"

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

    '   Total Lines: 120
    '    Code Lines: 70 (58.33%)
    ' Comment Lines: 31 (25.83%)
    '    - Xml Docs: 16.13%
    ' 
    '   Blank Lines: 19 (15.83%)
    '     File Size: 5.11 KB


    '     Class DmpFileReader
    ' 
    '         Function: ParseNames
    ' 
    '         Sub: loadTree
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text

Namespace Assembly.NCBI.Taxonomy

    Friend Class DmpFileReader

        Const sciNdeli As String = "scientific name"

        Private Shared Function ParseNames(names As String) As Dictionary(Of String, String)
            Dim taxid2name As New Dictionary(Of String, String)
            Dim taxid As String
            Dim lineToken As String()

            Call $"{names.ToFileURL} parsing ...".debug

            For Each line As String In names.IterateAllLines
                lineToken = line.Replace(ASCII.TAB, "").Split("|"c)

                ' 读取名称数据
                ' 将taxid和scientific name之间进行一一对应
                If lineToken(3).TextEquals(sciNdeli) Then
                    taxid = lineToken(0)
                    taxid2name(taxid) = lineToken(1)
                End If
            Next

            Call "names.dmp parsed".debug

            Return taxid2name
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="names"></param>
        ''' <param name="nodes"></param>
        ''' <param name="taxonomy">pull the parsed tree data from this parameter</param>
        Public Shared Sub loadTree(names$, nodes$, ByRef taxonomy As Dictionary(Of String, TaxonomyNode))
            Dim taxid2name As Dictionary(Of String, String) = ParseNames(names)
            Dim taxid As String
            Dim parent_taxid As String
            Dim lineTokens$()

            Call $"{nodes} parsing ...".debug

            For Each line As String In nodes.IterateAllLines
                lineTokens = line.Replace(ASCII.TAB, "").Split("|"c)

                ' nodes.dmp
                ' ---------
                ' 
                ' this file represents taxonomy nodes. The description for each node includes 
                ' the following fields
                '
                '     tax_id                   -- node id in GenBank taxonomy database
                '     parent tax_id            -- parent node id in GenBank taxonomy database
                '     rank                     -- rank Of this node (superkingdom, kingdom, ...) 
                ' 	  embl code                -- locus-name prefix; Not unique
                '     division id                        -- see division.dmp file
                '     inherited div flag  (1 Or 0)		 -- 1 if node inherits division from parent
                ' 	  genetic code id				     -- see gencode.dmp file
                '     inherited GC  flag  (1 Or 0)		 -- 1 if node inherits genetic code from parent
                ' 	  mitochondrial genetic code id		 -- see gencode.dmp file
                '     inherited MGC flag  (1 Or 0)	  	 -- 1 if node inherits mitochondrial gencode from parent
                ' 	  GenBank hidden flag (1 Or 0)       -- 1 if name Is suppressed in GenBank entry lineage
                ' 	  hidden subtree root flag (1 Or 0)  -- 1 if this subtree has no sequence data yet
                ' 	  comments                           -- Free-Text comments And citations

                taxid = lineTokens(0)
                parent_taxid = lineTokens(1)

                ' : # 18204/1308852
                If taxonomy.ContainsKey(taxid) Then
                    With taxonomy(taxid)
                        .rank = lineTokens(2)
                        .parent = parent_taxid
                    End With
                Else
                    ' : # 1290648/1308852
                    taxonomy(taxid) = New TaxonomyNode With {
                        .name = taxid2name(taxid),
                        .rank = lineTokens(2),
                        .parent = parent_taxid,
                        .children = New List(Of String),
                        .taxid = taxid
                    }
                    Call taxid2name.Remove(taxid)
                End If

                If taxonomy.ContainsKey(parent_taxid) Then
                    taxonomy(parent_taxid).children.Add(taxid)
                Else
                    ' create a new parent node
                    taxonomy(parent_taxid) = New TaxonomyNode With {
                        .name = taxid2name(parent_taxid),
                        .rank = Nothing,
                        .parent = Nothing,
                        .children = New List(Of String) From {
                            taxid
                        },
                        .taxid = parent_taxid
                    }
                    Call taxid2name.Remove(parent_taxid)
                End If
            Next

            Call "nodes.dmp parsed".debug

            ' To avoid infinite Loop
            Dim root_children = taxonomy("1").children
            Call root_children.Remove("1")

            ' 20260128
            ' fix bug for bacteria
            taxonomy("2").rank = NcbiTaxonomyTree.superkingdom

            With taxonomy("1")
                .parent = Nothing
                .children = root_children
            End With
        End Sub
    End Class
End Namespace
