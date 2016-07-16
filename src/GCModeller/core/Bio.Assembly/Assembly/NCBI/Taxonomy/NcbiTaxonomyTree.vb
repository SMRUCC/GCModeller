Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Python
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI

    Public Class TaxonNode
        Public Property taxid As Integer
        Public Property name As String
        Public Property rank As String
        Public Property parent As String
        Public Property children As List(Of Integer)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

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
    ''' https://github.com/frallain/NCBI_taxonomy_tree
    ''' 
    ''' #### NCBI_taxonomy_tree
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
    Public Class NcbiTaxonomyTree : Inherits ClassObject

        ''' <summary>
        ''' + species
        ''' + genus
        ''' + family
        ''' + order
        ''' + class
        ''' + phylum
        ''' + superkingdom
        ''' </summary>
        Private Shared ReadOnly stdranks As New List(Of String) From {
            "species",
            "genus",
            "family",
            "order",
            "class",
            "phylum",
            "superkingdom"
        }

        Public ReadOnly Property Taxonomy As New Dictionary(Of Integer, TaxonNode)

        Const sciNdeli As String = "scientific name"

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
        ''' <param name="nodes_filename"></param>
        ''' <param name="names_filename"></param>
        Sub New(nodes_filename As String, names_filename As String)
            If (Not nodes_filename.FileExists) OrElse (Not names_filename.FileExists) Then
                Throw New Exception
            End If

            Call "NcbiTaxonomyTree building ...".__DEBUG_ECHO

            Dim taxid2name As New Dictionary(Of Integer, String)
            Dim taxid As Integer

            Call $"{names_filename.ToFileURL} parsing ...".__DEBUG_ECHO

            For Each line In names_filename.IterateAllLines
                Dim lineToken As String() = line.Replace(vbTab, "").Split("|"c)
                If lineToken(3).TextEquals(sciNdeli) Then
                    taxid = CInt(lineToken(0))
                    taxid2name(taxid) = lineToken(1)
                End If
            Next

            Call "names.dmp parsed".__DEBUG_ECHO
            Call $"{nodes_filename} parsing ...".__DEBUG_ECHO

            For Each line As String In nodes_filename.IterateAllLines
                Dim lineTokens As String() = line.Replace(vbTab, "").Split("|"c)
                'LinqAPI.Exec(Of String) <= From elt As String
                '                           In line.Split("|"c)
                '                           Select New String(elt.slice(0, 2).ToArray)
                taxid = CInt(lineTokens(0))
                Dim parent_taxid As Integer = CInt(lineTokens(1))

                If Taxonomy.ContainsKey(taxid) Then ': # 18204/1308852
                    Dim x As TaxonNode = Taxonomy(taxid)
                    x.rank = lineTokens(2)
                    x.parent = parent_taxid
                    Taxonomy(taxid) = x ' dic(taxid).Replace(rank = line[2][1:             -1], parent=parent_taxid)
                Else ':           # 1290648/1308852
                    Taxonomy(taxid) = New TaxonNode With {
                        .name = taxid2name(taxid),
                        .rank = lineTokens(2),
                        .parent = parent_taxid,
                        .children = New List(Of Integer)
                    }
                    Call taxid2name.Remove(taxid)
                End If

                If Taxonomy.ContainsKey(parent_taxid) Then
                    Taxonomy(parent_taxid).children.Add(taxid)
                Else
                    Taxonomy(parent_taxid) = New TaxonNode With {
                        .name = taxid2name(parent_taxid),
                        .rank = Nothing,
                        .parent = Nothing,
                        .children = New List(Of Integer)({taxid})
                    }
                    Call taxid2name.Remove(parent_taxid)
                End If
            Next

            Call "nodes.dmp parsed".__DEBUG_ECHO

            '# To avoid infinite Loop
            Dim root_children = Taxonomy(1).children
            root_children.Remove(1)
            Dim xx = Taxonomy(1)
            xx.parent = Nothing
            xx.children = root_children
            Taxonomy(1) = xx 'dic(1)._replace(parent = None, children = root_children)

            Call "NcbiTaxonomyTree built".__DEBUG_ECHO
        End Sub

        Public Function GetParent(ParamArray taxids As Integer()) As Dictionary(Of Integer, String)
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getParent([28384, 131567])
            '    {28384: 1, 131567: 1}
            '"""
            Dim result As New Dictionary(Of Integer, String)

            For Each taxid As Integer In taxids
                result(taxid) = Taxonomy(taxid).parent
            Next

            Return result
        End Function

        Public Function GetRank(ParamArray taxids As Integer()) As Dictionary(Of Integer, String)
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getRank([28384, 131567])
            '    {28384: 'no rank', 131567: 'no rank'}
            '"""
            Dim result As New Dictionary(Of Integer, String)

            For Each taxid As Integer In taxids
                result(taxid) = Taxonomy(taxid).rank
            Next

            Return result
        End Function

        Public Function GetChildren(ParamArray taxids As Integer()) As Dictionary(Of Integer, List(Of Integer))
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getChildren([28384, 131567])
            '    {28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
            '"""
            Dim result As New Dictionary(Of Integer, List(Of Integer))

            For Each taxid As Integer In taxids
                result(taxid) = Taxonomy(taxid).children
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
                result(taxid) = Taxonomy(taxid).name
            Next

            Return result
        End Function

        Public Function GetAscendantsWithRanksAndNames(taxids As IEnumerable(Of Integer), Optional only_std_ranks As Boolean = False) As Dictionary(Of Integer, TaxonNode())
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
            Dim result As New Dictionary(Of Integer, TaxonNode())

            For Each taxid In taxids
                result(taxid) = __ascendantsWithRanksAndNames(taxid, only_std_ranks)
            Next
            Return result
        End Function

        Private Function __ascendantsWithRanksAndNames(taxid As Integer, only_std_ranks As Boolean) As TaxonNode()
            Dim lineage As New List(Of TaxonNode) From {
                New TaxonNode With {
                    .taxid = taxid,
                    .rank = Taxonomy(taxid).rank,
                    .name = Taxonomy(taxid).name
                }
            }

            Do While Taxonomy(taxid).parent IsNot Nothing
                taxid = Taxonomy(taxid).parent
                lineage += New TaxonNode With {
                    .taxid = taxid,
                    .rank = Taxonomy(taxid).rank,
                    .name = Taxonomy(taxid).name
                }
            Loop

            If only_std_ranks Then
                Dim std_lineage = LinqAPI.MakeList(Of TaxonNode) <=
 _
                    From lvl As TaxonNode
                    In lineage
                    Where stdranks.IndexOf(lvl.rank) > -1
                    Select lvl

                Dim lastlevel As Integer = 0

                If lineage(lastlevel).rank = "no rank" Then
                    std_lineage.Insert(0, lineage(lastlevel))
                End If

                lineage = std_lineage
            End If

            Return lineage
        End Function

        Public Function __descendants(taxid As Integer) As IEnumerable(Of Integer)
            '""" 
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree._getDescendants(208962) # doctest: +NORMALIZE_WHITESPACE
            '    [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]
            '"""
            Dim children = Taxonomy(taxid).children
            Dim result As New List(Of Integer)

            If Not children.IsNullOrEmpty Then

                result =
                    LinqAPI.MakeList(Of Integer) <=
                    From child As Integer
                    In children
                    Select __descendants(child)
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

            For Each taxid In taxids
                result(taxid) = flatten(__descendants(taxid)).ToArray(Of Integer)
            Next

            Return result
        End Function

        ''' <summary>
        ''' Returns the ordered list of the descendants with their respective ranks and names for a LIST of taxids.
        ''' </summary>
        ''' <param name="taxids"></param>
        ''' <returns></returns>
        Public Function GetDescendantsWithRanksAndNames(ParamArray taxids As Integer()) As Dictionary(Of Integer, TaxonNode())
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
            Dim result As New Dictionary(Of Integer, TaxonNode())

            For Each taxid In taxids
                result(taxid) = LinqAPI.Exec(Of TaxonNode) <=
 _
                    From descendant As Integer
                    In __descendants(taxid)
                    Select New TaxonNode With {
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
            Dim children As IEnumerable(Of Integer) = Taxonomy(taxid).children

            If children.IsNullOrEmpty Then
                Return {taxid} ' # In case of the taxid has no child
            End If

            Dim out = LinqAPI.Exec(Of Integer) <=
                From child As Integer
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
        Public Function GetLeavesWithRanksAndNames(taxid As Integer) As TaxonNode()
            '""" Returns all the descendant taxids that are leaves of the tree from 
            '    a branch/clade determined by ONE taxid.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxids_leaves_entire_tree = tree.getLeavesWithRanksAndNames(561)
            '    >>> taxids_leaves_entire_tree[0]
            '    Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
            '"""
            '   Node = namedtuple('Node', ['taxid', 'rank', 'name'])                            
            Dim result As TaxonNode() = LinqAPI.Exec(Of TaxonNode) <=
 _
                From leaf As Integer
                In GetLeaves(taxid)
                Select New TaxonNode With {
                    .taxid = leaf,
                    .rank = Taxonomy(leaf).rank,
                    .name = Taxonomy(leaf).name
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
            Dim LQuery = LinqAPI.Exec(Of
                KeyValuePair(Of Integer, TaxonNode)) <=
 _
                From node As KeyValuePair(Of Integer, TaxonNode)
                In Taxonomy
                Where node.Value.rank = rank
                Select node

            Dim out = LQuery.ToArray(Function(x) x.Key)
            Return out
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
                _preorderTraversal = AddressOf __preorderTraversalOnlyLeaves
            Else
                _preorderTraversal = AddressOf __preorderTraversal
            End If

            Return _preorderTraversal(taxid)
        End Function

        Private Function __preorderTraversal(taxid As Integer) As Integer()
            Dim children = Taxonomy(taxid).children
            Dim result As Integer()

            If children IsNot Nothing Then
                result = LinqAPI.Exec(Of Integer) <=
 _
                    From child As Integer
                    In children
                    Select __preorderTraversal(child) ', taxid )

                result.Add(taxid)
            Else
                result = {taxid}
            End If
            Return result
        End Function

        Private Function __preorderTraversalOnlyLeaves(taxid As Integer) As Integer()
            Dim children = Taxonomy(taxid).children

            If children.IsNullOrEmpty Then
                Return {taxid}
            End If

            Dim result As Integer() =
                LinqAPI.Exec(Of Integer) <= From child As Integer
                                            In children
                                            Select __preorderTraversalOnlyLeaves(child) 'for] if children else taxid
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