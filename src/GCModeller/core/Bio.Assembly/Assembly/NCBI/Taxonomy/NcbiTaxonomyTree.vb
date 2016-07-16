Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic

Namespace Assembly.NCBI

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
            Dim taxid2name As New Dictionary(Of Integer, String())
            '   Log.debug("names.dmp parsing ...")

            For Each line In names_filename.IterateAllLines
                Dim lineToken = line.Split("|"c)
                If line(3) = "\tscientific name\t" Then
                    Dim taxid = CInt(lineToken(0))
                    taxid2name(taxid) = lineToken(1)(1:          -1)
                    End If
            Next
            ' Log.debug("names.dmp parsed")

            '   Log.debug("nodes.dmp parsing ...")
            Dim dic = {}

            For Each line As String In nodes_filename.IterateAllLines
                line = [elt for elt in line.split('|')][:3]
                    taxid = Int(line[0])
                parent_taxid = Int(line[1])

                If taxid Then In self.dic: # 18204/1308852
                        self.dic[taxid] = self.dic[taxid]._replace(rank=line[2][1:      -1], parent=parent_taxid)
                    Else :       # 1290648/1308852
                        self.dic[taxid] = Node(name = taxid2name[taxid], rank = line[2][1:      -1], parent=parent_taxid, children=[])
                        del taxid2name[taxid]

                    Try :       # 1290648/1308852
                        self.dic[parent_taxid].children.append(taxid)
                    except KeyError:       # 18204/1308852
                        self.dic[parent_taxid] = Node(name = taxid2name[parent_taxid], rank = None, parent = None, children = [taxid])
                        del taxid2name[parent_taxid]
                    Next

            Log.debug("nodes.dmp parsed")
# To avoid infinite Loop
            root_children = self.dic[1].children
            root_children.remove(1)
            self.dic[1] = self.dic[1]._replace(parent=None, children=root_children)
            Log.info("NcbiTaxonomyTree built")
        End Sub

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