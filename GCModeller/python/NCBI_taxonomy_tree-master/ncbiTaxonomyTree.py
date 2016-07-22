#!/bin/env python2.7
# encoding: utf-8
# from __future__ import print_function
from __future__ import division

import os
import sys
from collections import defaultdict
from collections import Iterable
from collections import namedtuple
import logging
log = logging.getLogger(os.path.basename(__file__))
# log.disabled = True


def flatten(seq):
    """
    >>> flatten([1 , [2, 2], [2, [3, 3, 3]]]) 
    [1, 2, 2, 2, 3, 3, 3]
    """ 
    # flatten fonction from C:\Python26\Lib\compiler\ast.py, 
    # compiler is deprecated in py2.6
    l = []
    for elt in seq:
        t = type(elt)
        if t is tuple or t is list or t is set:
            for elt2 in flatten(elt):
                l.append(elt2)
        else:
            l.append(elt)
    return l

class NcbiTaxonomyTree(object):

    def __init__(self, nodes_filename=None, names_filename=None):
        """ Builds the following dictionnary from NCBI taxonomy nodes.dmp and 
        names.dmp files :
        { Taxid   : namedtuple('Node', ['name', 'rank', 'parent', 'children'] }
        https://www.biostars.org/p/13452/
        https://pythonhosted.org/ete2/tutorial/tutorial_ncbitaxonomy.html
        """
        self.standard_ranks = stdranks = ['species','genus','family','order','class','phylum','superkingdom']
        if nodes_filename and names_filename:
            log.info("NcbiTaxonomyTree building ...")
            Node = namedtuple('Node', ['name', 'rank', 'parent', 'children'])
            taxid2name = {}
            log.debug("names.dmp parsing ...")
            with open(names_filename) as names_file:
                for line in names_file:
                    line = [elt for elt in line.split('|')]
                    if line[3] == "\tscientific name\t":
                        taxid = int(line[0])
                        taxid2name[taxid] = line[1][1:-1]
            log.debug("names.dmp parsed")

            log.debug("nodes.dmp parsing ...")
            self.dic = {}
            with open(nodes_filename) as nodes_file:
                for line in nodes_file:
                    line = [elt for elt in line.split('|')][:3]
                    taxid = int(line[0])
                    parent_taxid = int(line[1])

                    if taxid in self.dic: # 18204/1308852
                        self.dic[taxid] = self.dic[taxid]._replace(rank=line[2][1:-1], parent=parent_taxid)
                    else: # 1290648/1308852
                        self.dic[taxid] = Node(name=taxid2name[taxid], rank=line[2][1:-1], parent=parent_taxid, children=[])
                        del taxid2name[taxid]

                    try: # 1290648/1308852
                        self.dic[parent_taxid].children.append(taxid)
                    except KeyError: # 18204/1308852
                        self.dic[parent_taxid] = Node(name=taxid2name[parent_taxid], rank=None, parent=None, children=[taxid])
                        del taxid2name[parent_taxid]

            log.debug("nodes.dmp parsed")
            # to avoid infinite loop
            root_children = self.dic[1].children
            root_children.remove(1)
            self.dic[1] = self.dic[1]._replace(parent=None, children=root_children)
            log.info("NcbiTaxonomyTree built")

    def getParent(self, taxids):
        """
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getParent([28384, 131567])
            {28384: 1, 131567: 1}
        """
        result = {}
        for taxid in taxids:
            result[taxid] = self.dic[taxid].parent
        return result

    def getRank(self, taxids):
        """
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getRank([28384, 131567])
            {28384: 'no rank', 131567: 'no rank'}
        """
        result = {}
        for taxid in taxids:
            result[taxid] = self.dic[taxid].rank
        return result

    def getChildren(self, taxids):
        """
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getChildren([28384, 131567])
            {28384: [2387, 2673, 31896, 36549, 81077], 131567: [2, 2157, 2759]}
        """
        result = {}
        for taxid in taxids:
            result[taxid] = self.dic[taxid].children
        return result

    def getName(self, taxids):
        """
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getName([28384, 131567])
            {28384: 'other sequences', 131567: 'cellular organisms'}
        """
        result = {}
        for taxid in taxids:
            result[taxid] = self.dic[taxid].name
        return result


    def getAscendantsWithRanksAndNames(self, taxids, only_std_ranks=False):
        """ 
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getAscendantsWithRanksAndNames([1,562]) # doctest: +NORMALIZE_WHITESPACE
            {1: [Node(taxid=1, rank='no rank', name='root')],
             562: [Node(taxid=562, rank='species', name='Escherichia coli'),
              Node(taxid=561, rank='genus', name='Escherichia'),
              Node(taxid=543, rank='family', name='Enterobacteriaceae'),
              Node(taxid=91347, rank='order', name='Enterobacteriales'),
              Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
              Node(taxid=1224, rank='phylum', name='Proteobacteria'),
              Node(taxid=2, rank='superkingdom', name='Bacteria'),
              Node(taxid=131567, rank='no rank', name='cellular organisms'),
              Node(taxid=1, rank='no rank', name='root')]}
            >>> tree.getAscendantsWithRanksAndNames([562], only_std_ranks=True) # doctest: +NORMALIZE_WHITESPACE
            {562: [Node(taxid=562, rank='species', name='Escherichia coli'),
              Node(taxid=561, rank='genus', name='Escherichia'),
              Node(taxid=543, rank='family', name='Enterobacteriaceae'),
              Node(taxid=91347, rank='order', name='Enterobacteriales'),
              Node(taxid=1236, rank='class', name='Gammaproteobacteria'),
              Node(taxid=1224, rank='phylum', name='Proteobacteria'),
              Node(taxid=2, rank='superkingdom', name='Bacteria')]}
        """
        def _getAscendantsWithRanksAndNames(taxid, only_std_ranks):
            Node = namedtuple('Node', ['taxid', 'rank', 'name'])
            lineage = [Node(taxid=taxid, 
                                rank=self.dic[taxid].rank, 
                                name=self.dic[taxid].name)]
            while self.dic[taxid].parent != None:
                taxid = self.dic[taxid].parent
                lineage.append(Node(taxid=taxid, 
                                        rank=self.dic[taxid].rank, 
                                        name=self.dic[taxid].name))
            if only_std_ranks:
                std_lineage = [lvl for lvl in lineage if lvl.rank in self.standard_ranks]
                lastlevel = 0
                if lineage[lastlevel].rank == 'no rank':
                    std_lineage.insert(0, lineage[lastlevel])
                lineage = std_lineage
            return lineage

        result = {}
        for taxid in taxids:
            result[taxid] = _getAscendantsWithRanksAndNames(taxid, only_std_ranks)
        return result

    def _getDescendants(self, taxid):
        """ 
            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree._getDescendants(208962) # doctest: +NORMALIZE_WHITESPACE
            [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]
        """
        children = self.dic[taxid].children
        if children:
            result = [ self._getDescendants(child) for child in children] 
            result.insert(0, taxid)
        else:
            result = taxid
        return result

    def getDescendants(self, taxids): 
        """ Returns all the descendant taxids from a branch/clade 
            of a list of taxids : all nodes (leaves or not) of the 
            tree are returned including the original one.

            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> taxid2descendants = tree.getDescendants([208962,566])
            >>> taxid2descendants == {566: [566, 1115515], 208962: [208962, 502347, 550692, 550693, 909209, 910238, 1115511, 1440052]}
            True
        """
        result = {}
        for taxid in taxids:
            result[taxid] = flatten(self._getDescendants(taxid))
        return result

    def getDescendantsWithRanksAndNames(self, taxids):
        """ Returns the ordered list of the descendants with their respective ranks and names for a LIST of taxids.

            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> taxid2descendants = tree.getDescendantsWithRanksAndNames([566]) # doctest: +NORMALIZE_WHITESPACE
            >>> taxid2descendants[566][1].taxid 
            1115515
            >>> taxid2descendants[566][1].rank 
            'no rank'
            >>> taxid2descendants[566][1].name 
            'Escherichia vulneris NBRC 102420'
        """
        Node = namedtuple('Node', ['taxid', 'rank', 'name'])
        result = {}
        for taxid in taxids:
            result[taxid] = [Node(taxid=descendant, 
                                rank=self.dic[descendant].rank, 
                                name=self.dic[descendant].name) 
                    for descendant in self._getDescendants(taxid)] 
        return result

    def getLeaves(self, taxid): 
        """ Returns all the descendant taxids that are leaves of the tree from 
            a branch/clade determined by ONE taxid.

            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> taxids_leaves_entire_tree = tree.getLeaves(1)
            >>> len(taxids_leaves_entire_tree)
            1184218
            >>> taxids_leaves_escherichia_genus = tree.getLeaves(561)
            >>> len(taxids_leaves_escherichia_genus)
            3382
        """
        def _getLeaves(taxid):
            children = self.dic[taxid].children
            result = [_getLeaves(child) for child in children] if children else taxid
            return result
        result = _getLeaves(taxid)

        if not isinstance(result,Iterable): # In case of the taxid has no child
            result = [result]
        else:
            result = flatten(result)
        return result

    def getLeavesWithRanksAndNames(self, taxid): 
        """ Returns all the descendant taxids that are leaves of the tree from 
            a branch/clade determined by ONE taxid.

            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> taxids_leaves_entire_tree = tree.getLeavesWithRanksAndNames(561)
            >>> taxids_leaves_entire_tree[0]
            Node(taxid=1266749, rank='no rank', name='Escherichia coli B1C1')
        """
        Node = namedtuple('Node', ['taxid', 'rank', 'name'])                            
        result = [Node(taxid=leaf, 
                        rank=self.dic[leaf].rank, 
                        name=self.dic[leaf].name) 
                    for leaf in self.getLeaves(taxid)] 
        return result

    def getTaxidsAtRank(self, rank):
        """ Returns all the taxids that are at a specified rank : 
            standard ranks : species, genus, family, order, class, phylum,
                superkingdom.
            non-standard ranks : forma, varietas, subspecies, species group, 
                subtribe, tribe, subclass, kingdom.

            >>> tree = NcbiTaxonomyTree(nodes_filename="nodes.dmp", names_filename="names.dmp")
            >>> tree.getTaxidsAtRank('superkingdom')
            [2, 2157, 2759, 10239, 12884]
        """ 
        return [taxid for taxid,node in self.dic.iteritems() if node.rank == rank]

    def preorderTraversal(self, taxid, only_leaves):
        """ Prefix (Preorder) visit of the tree
            https://en.wikipedia.org/wiki/Tree_traversal
        """
        if only_leaves:
            def _preorderTraversal(taxid):
                children = self.dic[taxid].children
                result = [_preorderTraversal(child) for child in children] if children else taxid
                return result
        else:
            def _preorderTraversal(taxid):
                children = self.dic[taxid].children
                if children:
                    result = ([_preorderTraversal(child) for child in children] , taxid )
                else:
                    result = taxid
                return result
        return _preorderTraversal(taxid)
     

if __name__ == "__main__":

    log.disabled = True
    log.setLevel(logging.DEBUG)
    formatter = logging.Formatter('%(asctime)s %(name)-12s %(levelname)-8s %(module)-8s l.%(lineno)-3d : %(message)s')
    steam_handler = logging.StreamHandler(sys.stdout)
    steam_handler.setLevel(logging.DEBUG)
    formatter2 = logging.Formatter('%(asctime)s %(levelname)-8s l.%(lineno)-3d : %(message)s')
    steam_handler.setFormatter(formatter2)
    log.addHandler(steam_handler)
    # file_handler = logging.FileHandler(os.path.dirname(mgffilename_in) + os.sep + os.path.basename(mgffilename_in) + ".log", mode='wb', encoding=None, delay=0)
    # file_handler.setLevel(logging.DEBUG)
    # file_handler.setFormatter(formatter)
    # log.addHandler(file_handler)

    import tarfile
    with tarfile.open("names+nodes_test.tar.gz", 'r:gz') as tfile:
        tfile.extractall('.')

    import doctest
    doctest.testmod(verbose=True)

    