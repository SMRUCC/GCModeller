Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Python
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.NCBI

    Public Class Node
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

        Shared ReadOnly stdranks As IReadOnlyCollection(Of String) = {"species", "genus", "family", "order", "class", "phylum", "superkingdom"}

        Public ReadOnly Property dic As New Dictionary(Of Integer, Node)

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
            '  Log.info("NcbiTaxonomyTree building ...")
            '  Dim Node = namedtuple('Node', ['name', 'rank', 'parent', 'children'])
            Dim taxid2name As New Dictionary(Of Integer, String)
            '   Log.debug("names.dmp parsing ...")
            Dim taxid As Integer

            For Each line In names_filename.IterateAllLines
                Dim lineToken = line.Split("|"c)
                If lineToken(3) = "\tscientific name\t" Then
                    taxid = CInt(lineToken(0))
                    taxid2name(taxid) = New String(lineToken(1).slice(1, -1).ToArray)
                End If
            Next
            ' Log.debug("names.dmp parsed")

            '   Log.debug("nodes.dmp parsing ...")

            For Each line As String In nodes_filename.IterateAllLines
                Dim lineTokens = From elt In line.Split("|"c) Select New String(elt.slice(0, 3).ToArray)
                taxid = CInt(lineTokens(0))
                Dim parent_taxid = CInt(lineTokens(1))

                If dic.ContainsKey(taxid) Then ': # 18204/1308852
                    Dim x = dic(taxid)
                    x.rank = lineTokens(2).slice(1, -1).CharString
                    x.parent = parent_taxid
                    dic(taxid) = x ' dic(taxid).Replace(rank = line[2][1:             -1], parent=parent_taxid)
                Else ':           # 1290648/1308852
                    dic(taxid) = New Node With {
                        .name = taxid2name(taxid),
                        .rank = lineTokens(2).slice(1, -1).CharString,
                        .parent = parent_taxid,
                        .children = New List(Of Integer)
                    }
                    '    del taxid2name(taxid)
                End If

                Try ':         # 1290648/1308852
                    dic(parent_taxid).children.Add(taxid)
                Catch ex As Exception
                    '   except KeyError:         # 18204/1308852
                    dic(parent_taxid) = New Node With {.name = taxid2name(parent_taxid), .rank = Nothing, .parent = Nothing, .children = New List(Of Integer)({taxid})}
                    '     del taxid2name[parent_taxid]
                End Try
            Next

            ' Log.debug("nodes.dmp parsed")
            '# To avoid infinite Loop
            Dim root_children = dic(1).children
            root_children.Remove(1)
            Dim xx = dic(1)
            xx.parent = Nothing
            xx.children = root_children
            dic(1) = xx 'dic(1)._replace(parent = None, children = root_children)
            '  Log.info("NcbiTaxonomyTree built")
        End Sub

        Public Function getParent(taxids As IEnumerable(Of Integer))
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getParent([28384, 131567])
            '    {28384: 1, 131567: 1}
            '"""
            Dim result As New Dictionary(Of Integer, String)
            For Each taxid In taxids
                result(taxid) = dic(taxid).parent
            Next
            Return result
        End Function

        Public Function getRank(taxids As IEnumerable(Of Integer))
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getRank([28384, 131567])
            '    {28384: 'no rank', 131567: 'no rank'}
            '"""
            Dim result As New Dictionary(Of Integer, String)
            For Each taxid In taxids
                result(taxid) = dic(taxid).rank
            Next
            Return result
        End Function

        Public Function getChildren(taxids As IEnumerable(Of Integer))
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getChildren([28384, 131567])
            '    {28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
            '"""
            Dim result As New Dictionary(Of Integer, List(Of Integer))
            For Each taxid In taxids
                result(taxid) = dic(taxid).children
            Next
            Return result
        End Function

        Public Function getName(taxids As IEnumerable(Of Integer))
            '"""
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getName([28384, 131567])
            '    {28384: 'other sequences', 131567: 'cellular organisms'}
            '"""
            Dim result As New Dictionary(Of Integer, String)
            For Each taxid In taxids
                result(taxid) = dic(taxid).name
            Next

            Return result
        End Function

        Public Function getAscendantsWithRanksAndNames(taxids As IEnumerable(Of Integer), Optional only_std_ranks As Boolean = False)
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
            Dim _getAscendantsWithRanksAndNames =
                Function(taxid) ', only_std_ranks)
                    '  Node = NamedTuple('Node', ['taxid', 'rank', 'name'])
                    Dim lineage = {New Node With {.taxid = taxid,
                                                                      .rank = dic(taxid).rank,
                             .name = dic(taxid).name}}
                    Do While dic(taxid).parent IsNot Nothing
                        taxid = dic(taxid).parent
                        lineage.Append(New Node With {.taxid = taxid,
                                      .rank = dic(taxid).rank,
                                       .name = dic(taxid).name})
                    Loop
                    If only_std_ranks Then
                        Dim std_lineage = LinqAPI.MakeList(Of Node) <= From lvl In lineage Where stdranks.Contains(lvl.rank)
                        Dim lastlevel = 0
                        If lineage(lastlevel).rank = "no rank" Then
                            std_lineage.Insert(0, lineage(lastlevel))
                        End If
                        lineage = std_lineage
                    End If
                    Return lineage
                End Function

            Dim result As New Dictionary(Of Integer, Node())

            For Each taxid In taxids
                result(taxid) = _getAscendantsWithRanksAndNames(taxid)
            Next
            Return result
        End Function

        Public Function _getDescendants(taxid) As IEnumerable(Of Integer)
            '""" 
            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree._getDescendants(208962) # doctest: +NORMALIZE_WHITESPACE
            '    [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]
            '"""
            Dim children = dic(taxid).children
            Dim result

            If Not children.IsNullOrEmpty Then
                result = From child In children Select _getDescendants(child)
                result.insert(0, taxid)
            Else
                result = taxid
            End If
            Return result
        End Function
        Public Function getDescendants(taxids)
            '""" Returns all the descendant taxids from a branch/clade 
            '    of a list of taxids : all nodes (leaves or not) of the 
            '    tree are returned including the original one.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxid2descendants = tree.getDescendants([208962,566])
            '    >>> taxid2descendants == {566: [566, 1115515], 208962: [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]}
            '    True
            '"""
            Dim result = {}
            For Each taxid In taxids
                result(taxid) = flatten(_getDescendants(taxid))
            Next
            Return result
        End Function
        Public Function getDescendantsWithRanksAndNames(taxids)
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
            Dim result = {}
            For Each taxid In taxids
                result(taxid) = From descendant In _getDescendants(taxid) Select New Node With {.taxid = descendant,
                           .rank = dic(descendant).rank,
                           .name = dic(descendant).name}

            Next
            Return result
        End Function

        Public Function getLeaves(taxid As Integer) As IEnumerable
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

            Dim result = _getLeaves(taxid)

            If result Is Nothing Then  '    # In case of the taxid has no child
                result = [result]
            Else
                result = flatten(result)
            End If
            Return result
        End Function

        Private Function _getLeaves(taxid As Integer) As IEnumerable
            Dim children = dic(taxid).children
            Dim out = From child In children Where children IsNot Nothing Select _getLeaves(child) 'Else taxid
            Return out
        End Function

        Public Function getLeavesWithRanksAndNames(taxid)
            '""" Returns all the descendant taxids that are leaves of the tree from 
            '    a branch/clade determined by ONE taxid.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> taxids_leaves_entire_tree = tree.getLeavesWithRanksAndNames(561)
            '    >>> taxids_leaves_entire_tree[0]
            '    Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
            '"""
            '   Node = namedtuple('Node', ['taxid', 'rank', 'name'])                            
            Dim result = From leaf In getLeaves(taxid) Select New Node With {.taxid = leaf,
                   .rank = dic(leaf).rank,
               .name = dic(leaf).name}

            Return result
        End Function
        Public Function getTaxidsAtRank(rank)
            '""" Returns all the taxids that are at a specified rank : 
            '    standard ranks : species, genus, family, order, class, phylum,
            '        superkingdom.
            '    non-standard ranks : forma, varietas, subspecies, species group, 
            '        subtribe, tribe, subclass, kingdom.

            '    >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            '    >>> tree.getTaxidsAtRank('superkingdom')
            '    [2, 2157, 2759, 10239, 12884]
            '""" 
            Return From node In dic.Values Where node.rank = rank Select node
        End Function
        Public Function preorderTraversal(taxid As Integer, only_leaves As Boolean)
            '""" Prefix (Preorder) visit of the tree
            '    https://en.wikipedia.org/wiki/Tree_traversal
            '"""

            Dim _preorderTraversal

            If only_leaves Then
                _preorderTraversal = Function()
                                         Dim children = dic(taxid).children
                                         Dim result = From child In children Select _preorderTraversal(child) 'for] if children else taxid
                                         Return result
                                     End Function
            Else
                _preorderTraversal = Function()
                                         Dim children = dic(taxid).children
                                         Dim result
                                         If children IsNot Nothing Then
                                             result = From child In children Select _preorderTraversal(child) ', taxid )
                                         Else
                                             result = taxid
                                         End If
                                         Return result
                                     End Function
            End If
            Return _preorderTraversal(taxid)
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

            For Each elt In seq
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