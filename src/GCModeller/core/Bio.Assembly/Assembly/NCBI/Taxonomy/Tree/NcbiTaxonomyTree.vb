#Region "Microsoft.VisualBasic::eab3dab5216d408dee57cfb1e75a9fb7, core\Bio.Assembly\Assembly\NCBI\Taxonomy\Tree\NcbiTaxonomyTree.vb"

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

'   Total Lines: 568
'    Code Lines: 250 (44.01%)
' Comment Lines: 245 (43.13%)
'    - Xml Docs: 52.24%
' 
'   Blank Lines: 73 (12.85%)
'     File Size: 24.07 KB


'     Class NcbiTaxonomyTree
' 
'         Properties: Taxonomy
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: __ascendantsWithRanksAndNames, __descendants, __preorderTraversal, __preorderTraversalOnlyLeaves, flatten
'                   (+2 Overloads) GetAscendantsWithRanksAndNames, GetChildren, GetDescendants, GetDescendantsWithRanksAndNames, GetLeaves
'                   GetLeavesWithRanksAndNames, GetName, GetParent, GetRank, GetTaxidsAtRank
'                   preorderTraversal
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports MapNode = System.Collections.Generic.KeyValuePair(Of String, SMRUCC.genomics.Assembly.NCBI.Taxonomy.TaxonomyNode)

Namespace Assembly.NCBI.Taxonomy

    ''' <summary>
    ''' Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and 
    ''' ``names.dmp`` files 
    ''' 
    ''' ```json
    ''' { Taxid   : namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
    ''' ```
    ''' 
    ''' + https://www.biostars.org/p/13452/
    ''' + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
    '''
    ''' </summary>
    ''' <remarks>
    ''' ### NCBI_taxonomy_tree
    '''
    ''' > https://github.com/frallain/NCBI_taxonomy_tree
    ''' 
    ''' The NCBI Taxonomy database Is a curated Set Of names And classifications For all Of the organisms that are 
    ''' represented In GenBank (http://www.ncbi.nlm.nih.gov/Taxonomy/taxonomyhome.html/). It can be accessed 
    ''' via http://www.ncbi.nlm.nih.gov/Taxonomy/Browser/wwwtax.cgi Or it can be downloaded from 
    ''' ftp://ftp.ncbi.nih.gov/pub/taxonomy/ in the form of 2 files : ``nodes.dmp`` for the structure of the tree 
    ''' And ``names.dmp`` for the names of the different nodes.
    '''
    ''' Here I make available my In-memory mapping Of the NCBI taxonomy : a Python 2.7 Class that maps the ``names.dmp`` 
    ''' And ``nodes.dmp`` files In a Python dictionnary which can be used To retrieve lineages, descendants, etc ...
    ''' </remarks>
    Public Class NcbiTaxonomyTree

        ''' <summary>
        ''' 这个列表是从小到大进行排序的
        ''' 
        ''' + species
        ''' + genus
        ''' + family
        ''' + order
        ''' + class
        ''' + phylum
        ''' + superkingdom
        ''' </summary>
        Public Shared ReadOnly stdranks As Index(Of String) = {
            "species",
            "genus",
            "family",
            "order",
            "class",
            "phylum",
            "superkingdom"
        }

        Public Const species As String = NameOf(species),
            genus As String = NameOf(genus),
            family As String = NameOf(family),
            order As String = NameOf(order),
            [class] As String = NameOf([class]),
            phylum As String = NameOf(phylum),
            superkingdom As String = NameOf(superkingdom)

        ''' <summary>
        ''' ``{taxid -> taxonomy_node}``
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Taxonomy As New Dictionary(Of String, TaxonomyNode)

        ''' <summary>
        ''' 当<paramref name="taxid"/>不存在的时候，这个只读属性会返回空值
        ''' </summary>
        ''' <param name="taxid%"></param>
        ''' <returns></returns>
        Default Public ReadOnly Property GetNode(taxid%) As TaxonomyNode
            Get
                Dim key As String = taxid.ToString

                If Not Taxonomy.ContainsKey(key) Then
                    Return Nothing
                Else
                    Return _Taxonomy(key)
                End If
            End Get
        End Property

        ''' <summary>
        ''' Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and 
        ''' ``names.dmp`` files 
        ''' 
        ''' ```json
        ''' { Taxid    namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
        ''' ```
        ''' 
        ''' + https://www.biostars.org/p/13452/
        ''' + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
        ''' 
        ''' </summary>
        Sub New(directory As String)
            Call Me.New(directory & "/nodes.dmp", directory & "/names.dmp")
        End Sub

        ''' <summary>
        ''' Builds the following dictionnary from NCBI taxonomy ``nodes.dmp`` and 
        ''' ``names.dmp`` files 
        ''' 
        ''' ```json
        ''' { Taxid    namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
        ''' ```
        ''' 
        ''' + https://www.biostars.org/p/13452/
        ''' + https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
        ''' 
        ''' </summary>
        ''' <param name="nodes"></param>
        ''' <param name="names"></param>
        Sub New(nodes$, names$)
            If (Not nodes.FileExists) OrElse (Not names.FileExists) Then
                Throw New Exception("Missing file ""node.dmp"" or ""names.dmp"".")
            Else
                Call "NcbiTaxonomyTree building ...".debug
                Call DmpFileReader.loadTree(names, nodes, Taxonomy)
                Call "NcbiTaxonomyTree built".debug
            End If
        End Sub

        ''' <summary>
        ''' get parent node of current node its taxid
        ''' </summary>
        ''' <param name="taxid"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetParent(taxid As Integer) As TaxonomyNode
            Return Taxonomy(taxid.ToString)
        End Function

        ''' <summary>
        ''' Returns parent id
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <returns></returns>
        Public Function GetParents(ParamArray taxids As Integer()) As Dictionary(Of Integer, String)
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getParent([28384, 131567])
            '    {28384: 1, 131567: 1}
            '"""
            Dim result As New Dictionary(Of Integer, String)

            For Each taxid As Integer In taxids
                result(key:=taxid) = Taxonomy(taxid.ToString).parent
            Next

            Return result
        End Function

        ''' <summary>
        ''' 获取所给定的taxid的分类等级列表
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <returns></returns>
        Public Function GetRank(ParamArray taxids As Integer()) As Dictionary(Of Integer, String)
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getRank([28384, 131567])
            '    {28384: 'no rank', 131567: 'no rank'}
            '"""
            Dim result As New Dictionary(Of Integer, String)

            For Each taxid As Integer In taxids
                result(key:=taxid) = Taxonomy(taxid.ToString).rank
            Next

            Return result
        End Function

        Public Function GetChildren(ParamArray taxids As Integer()) As Dictionary(Of Integer, Integer())
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getChildren([28384, 131567])
            '    {28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
            '"""
            Dim result As New Dictionary(Of Integer, Integer())

            For Each taxid As Integer In taxids
                result(key:=taxid) = Taxonomy(taxid.ToString).children _
                    .Select(AddressOf Integer.Parse) _
                    .ToArray
            Next

            Return result
        End Function

        Public Function GetName(ParamArray taxids As Integer()) As Dictionary(Of Integer, String)
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getName([28384, 131567])
            '    {28384: 'other sequences', 131567: 'cellular organisms'}
            '"""
            Dim result As New Dictionary(Of Integer, String)

            For Each taxid In taxids
                result(key:=taxid) = Taxonomy(taxid.ToString).name
            Next

            Return result
        End Function

        ''' <summary>
        ''' 使用这个函数得到物种的具体分类，返回来的数据是从小到大排列的
        ''' </summary>
        ''' <param name="taxid">ncbi taxonomy数据库中的物种id</param>
        ''' <param name="only_std_ranks">是否仅返回标准分类层级</param>
        ''' <returns>目标物种在NCBI taxonomy物种分类树上的分类路径节点向量</returns>
        ''' <remarks>
        ''' 请注意：返回来的数据是从小到大排列的
        ''' </remarks>
        Public Function GetAscendantsWithRanksAndNames(taxid As Integer, Optional only_std_ranks As Boolean = False) As TaxonomyNode()
            Dim key As String = taxid.ToString

            If Not Taxonomy.ContainsKey(key) Then
                Return {}
            Else
                Return ascendantsWithRanksAndNames(key, only_std_ranks)
            End If
        End Function

        ''' <summary>
        ''' 使用这个函数得到物种的具体分类，返回来的数据是从小到大排列的
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <param name="only_std_ranks"></param>
        ''' <returns></returns>
        Public Function GetAscendantsWithRanksAndNames(taxids As IEnumerable(Of Integer), Optional only_std_ranks As Boolean = False) As Dictionary(Of Integer, TaxonomyNode())
            '""" 
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getAscendantsWithRanksAndNames([1,562]) # doctest: +NORMALIZE_WHITESPACE
            '    {1: [Node(taxid=1, rank='no rank', name='root')],
            '     562: [Node(taxid=562, rank='species', name='Escherichia coli'),
            '      Node(taxid=561, rank='genus', name='Escherichia'),
            '      Node(taxid=543, rank='family', name='Enterobacteriaceae'),
            '      Node(taxid=91347, rank='order', name='Enterobacteriales'),
            '      Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
            '      Node(taxid=1224, rank='phylum', name='Proteobacteria'),
            '      Node(taxid=2, rank='superkingdom', name='Bacteria'),
            '      Node(taxid=131567, rank='no rank', name='cellular organisms'),
            '      Node(taxid=1, rank='no rank', name='root')]}
            '    >>> tree.getAscendantsWithRanksAndNames([562], only_std_ranks=True) # doctest: +NORMALIZE_WHITESPACE
            '    {562: [Node(taxid=562, rank='species', name='Escherichia coli'),
            '      Node(taxid=561, rank='genus', name='Escherichia'),
            '      Node(taxid=543, rank='family', name='Enterobacteriaceae'),
            '      Node(taxid=91347, rank='order', name='Enterobacteriales'),
            '      Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
            '      Node(taxid=1224, rank='phylum', name='Proteobacteria'),
            '      Node(taxid=2, rank='superkingdom', name='Bacteria')]}
            '"""
            Dim result As New Dictionary(Of Integer, TaxonomyNode())

            For Each taxid In taxids
                If Not Taxonomy.ContainsKey(taxid.ToString) Then
                    result(key:=taxid) = {}
                    Call $"Missing taxid {taxid}!".debug
                Else
                    result(key:=taxid) = ascendantsWithRanksAndNames(taxid, only_std_ranks)
                End If
            Next
            Return result
        End Function

        Private Function ascendantsWithRanksAndNames(taxid As String, only_std_ranks As Boolean) As TaxonomyNode()
            Dim lineage As New List(Of TaxonomyNode) From {
                New TaxonomyNode With {
                    .taxid = taxid,
                    .rank = _Taxonomy(taxid).rank,
                    .name = _Taxonomy(taxid).name
                }
            }

            Do While _Taxonomy(taxid).parent IsNot Nothing
                taxid = _Taxonomy(taxid).parent
                lineage += New TaxonomyNode With {
                    .taxid = taxid,
                    .rank = _Taxonomy(taxid).rank,
                    .name = _Taxonomy(taxid).name
                }
            Loop

            If only_std_ranks Then
                Dim std_lineage = LinqAPI.MakeList(Of TaxonomyNode) _
                                                                    _
                    () <= From lvl As TaxonomyNode
                          In lineage
                          Where lvl.rank Like stdranks
                          Select lvl

                Dim lastlevel As Integer = 0

                If lineage(lastlevel).rank = "no rank" Then
                    std_lineage.Insert(0, lineage(lastlevel))
                End If

                lineage = std_lineage
            End If

            Return lineage
        End Function

        Private Function descendants(taxid As String) As IEnumerable(Of String)
            '""" 
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree._getDescendants(208962) # doctest: +NORMALIZE_WHITESPACE
            '    [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]
            '"""
            Dim children = Taxonomy(taxid).children
            Dim result As New List(Of String)

            If Not children.IsNullOrEmpty Then

                result = LinqAPI.MakeList(Of String) _
                                                     _
                    () <= From child As String
                          In children
                          Select descendants(child)

                result.Insert(0, taxid)

            Else
                result += taxid
            End If

            Return result
        End Function

        ''' <summary>
        ''' Returns all the descendant taxids from a branch/clade 
        ''' of a list of taxids : all nodes (leaves or not) of the 
        ''' tree are returned including the original one.
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <returns></returns>
        Public Function GetDescendants(ParamArray taxids As Integer()) As Dictionary(Of Integer, Integer())
            '""" Returns all the descendant taxids from a branch/clade 
            '    of a list of taxids : all nodes (leaves or not) of the 
            '    tree are returned including the original one.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxid2descendants = tree.getDescendants([208962,566])
            '    >>> taxid2descendants == {566: [566, 1115515], 208962: [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]}
            '    True
            '"""
            Dim result As New Dictionary(Of Integer, Integer())

            For Each taxid As Integer In taxids
                result(key:=taxid) = flatten(descendants(taxid.ToString)).ToArray(Of Integer)
            Next

            Return result
        End Function

        ''' <summary>
        ''' Returns the ordered list of the descendants with their respective ranks and names for a LIST of taxids.
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <returns></returns>
        Public Function GetDescendantsWithRanksAndNames(ParamArray taxids As Integer()) As Dictionary(Of Integer, TaxonomyNode())
            '""" Returns the ordered list of the descendants with their respective ranks and names for a LIST of taxids.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxid2descendants = tree.getDescendantsWithRanksAndNames([566]) # doctest: +NORMALIZE_WHITESPACE
            '    >>> taxid2descendants[566][1].taxid 
            '    1115515
            '    >>> taxid2descendants[566][1].rank 
            '    'no rank'
            '    >>> taxid2descendants[566][1].name 
            '    'Escherichia vulneris NBRC 102420'
            '"""
            '    Node = namedtuple('Node', ['taxid', 'rank', 'name'])
            Dim result As New Dictionary(Of Integer, TaxonomyNode())

            For Each taxid In taxids
                result(key:=taxid) = LinqAPI.Exec(Of TaxonomyNode) <=
                                                                     _
                    From descendant As String
                    In descendants(taxid)
                    Select New TaxonomyNode With {
                        .taxid = descendant,
                        .rank = Taxonomy(descendant).rank,
                        .name = Taxonomy(descendant).name
                    }
            Next

            Return result
        End Function

        ''' <summary>
        ''' Returns all the descendant taxids that are leaves of the tree from 
        ''' a branch/clade determined by ONE taxid.
        ''' </summary>
        ''' <param name="taxid"></param>
        ''' <returns></returns>
        Public Function GetLeaves(taxid As Integer) As Integer()
            '""" Returns all the descendant taxids that are leaves of the tree from 
            '    a branch/clade determined by ONE taxid.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxids_leaves_entire_tree = tree.getLeaves(1)
            '    >>> len(taxids_leaves_entire_tree)
            '    1184218
            '    >>> taxids_leaves_escherichia_genus = tree.getLeaves(561)
            '    >>> len(taxids_leaves_escherichia_genus)
            '    3382
            '"""
            Dim children = Taxonomy(taxid.ToString).children

            If children.IsNullOrEmpty Then
                Return {taxid} ' # In case of the taxid has no child
            End If

            Dim out = LinqAPI.Exec(Of Integer) _
                                               _
                () <= From child As String
                      In children'.AsParallel
                      Select GetLeaves(child) ' Else taxid

            Return out
        End Function

        ''' <summary>
        ''' Returns all the descendant taxids that are leaves of the tree from 
        ''' a branch/clade determined by ONE taxid.
        ''' </summary>
        ''' <param name="taxid"></param>
        ''' <returns></returns>
        Public Function GetLeavesWithRanksAndNames(taxid As Integer) As TaxonomyNode()
            '""" Returns all the descendant taxids that are leaves of the tree from 
            '    a branch/clade determined by ONE taxid.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxids_leaves_entire_tree = tree.getLeavesWithRanksAndNames(561)
            '    >>> taxids_leaves_entire_tree[0]
            '    Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
            '"""
            '   Node = namedtuple('Node', ['taxid', 'rank', 'name'])                            
            Dim result As TaxonomyNode() = LinqAPI.Exec(Of TaxonomyNode) <=
                                                                           _
                From leaf As Integer
                In GetLeaves(taxid)
                Let leaf_key As String = leaf.ToString
                Select New TaxonomyNode With {
                    .taxid = leaf,
                    .rank = Taxonomy(leaf_key).rank,
                    .name = Taxonomy(leaf_key).name
                }

            Return result
        End Function

        ''' <summary>
        ''' Returns all the taxids that are at a specified rank: 
        ''' 
        ''' + standard ranks: 
        '''   ``species, genus, family, order, class, phylum, superkingdom.``
        ''' + non-standard ranks: 
        '''   ``forma, varietas, subspecies, species group, subtribe, tribe, subclass, kingdom.``
        '''   
        ''' </summary>
        ''' <param name="rank"></param>
        ''' <returns></returns>
        Public Function GetTaxidsAtRank(rank As String) As Integer()
            '""" Returns all the taxids that are at a specified rank : 
            '    standard ranks : species, genus, family, order, class, phylum,
            '        superkingdom.
            '    non-standard ranks : forma, varietas, subspecies, species group, 
            '        subtribe, tribe, subclass, kingdom.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getTaxidsAtRank('superkingdom')
            '    [2, 2157, 2759, 10239, 12884]
            '""" 
            Dim LQuery = LinqAPI.Exec(Of MapNode) _
                                                  _
                () <= From node As MapNode
                      In Taxonomy.AsParallel
                      Where node.Value.rank = rank
                      Select node

            Dim taxid = LQuery.Select(Function(x) Integer.Parse(x.Key)).ToArray
            Return taxid
        End Function

        ''' <summary>
        ''' Prefix (Preorder) visit of the tree: https://en.wikipedia.org/wiki/Tree_traversal
        ''' </summary>
        ''' <param name="taxid"></param>
        ''' <param name="only_leaves"></param>
        ''' <returns></returns>
        Public Function preorderTraversal(taxid As Integer, only_leaves As Boolean) As Integer()
            '""" Prefix (Preorder) visit of the tree
            '    https://en.wikipedia.org/wiki/Tree_traversal
            '"""

            Dim _preorderTraversal As Func(Of Integer, Integer())

            If only_leaves Then
                _preorderTraversal = AddressOf preorderTraversalOnlyLeaves
            Else
                _preorderTraversal = AddressOf preorderTraversal
            End If

            Return _preorderTraversal(taxid)
        End Function

        Private Function preorderTraversal(taxid As Integer) As Integer()
            Dim children = Taxonomy(key:=taxid.ToString).children
            Dim result As Integer()

            If children IsNot Nothing Then
                result = LinqAPI.Exec(Of Integer) _
                                                  _
                    () <= From child As String
                          In children
                          Select preorderTraversal(Integer.Parse(child)) ', taxid )

                result.Add(taxid)
            Else
                result = {taxid}
            End If

            Return result
        End Function

        Private Function preorderTraversalOnlyLeaves(taxid As Integer) As Integer()
            Dim children = Taxonomy(key:=taxid.ToString).children

            If children.IsNullOrEmpty Then
                Return {taxid}
            End If

            Dim result%() = LinqAPI.Exec(Of Integer) _
                                                     _
                () <= From child As String
                      In children.AsParallel
                      Select preorderTraversalOnlyLeaves(Integer.Parse(child)) 'for] if children else taxid

            Return result
        End Function

        ''' <summary>
        ''' ```
        ''' >>> flatten([1 , [2, 2], [2, [3, 3, 3]]]) 
        ''' [1, 2, 2, 2, 3, 3, 3]
        ''' ```
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        Public Shared Function flatten(seq As IEnumerable) As IEnumerable
            ' """
            ' >>> flatten([1 , [2, 2], [2, [3, 3, 3]]]) 
            ' [1, 2, 2, 2, 3, 3, 3]
            ' """ 
            ' # flatten fonction from C:\Python26\Lib\compiler\ast.py, 
            ' # compiler Is deprecated In py2.6
            Dim l As New List(Of Object)

            For Each elt As Object In seq
                Dim t As Type = elt.GetType

                If Array.IndexOf(t.GetInterfaces, GetType(IEnumerable)) > -1 Then
                    For Each elt2 In flatten(DirectCast(elt, IEnumerable))
                        l.Add(elt2)
                    Next
                Else
                    l.Add(elt)
                End If
            Next

            Return l
        End Function
    End Class
End Namespace
