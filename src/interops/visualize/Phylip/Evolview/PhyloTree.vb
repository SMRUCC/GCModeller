#Region "Microsoft.VisualBasic::615e1cbe364a6e068271d3fa271aa5a1, visualize\Phylip\Evolview\PhyloTree.vb"

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

    '     Class PhyloTree
    ' 
    '         Properties: AllLeafLabels, AllNodes, Description, errorMessage, FirstLeafNode
    '                     HasBranchLength, ID, InternalID, LastLeafNode, LeafNodes
    '                     maxHorizontalLevel, maxTotalBranchLengthFromRootToAnyTip, maxVerticalLevel, RootNode, treeDataValid
    '                     treeFormat, treeString
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: getAllAncestors, getAttributeValueByItemID, getExternalIDbyInternalID, getLCA, getNodeByID
    '                   getNumericValueForNamedItemFromAnXMLNode, hasBootstrapScores, MakeNewInternalNode, makeNewLeafNode, phyloNodeToString
    '                   reMakeEssentialVariables, ToString, toTreeString
    ' 
    '         Sub: CalcDistanceToRoot, CalcLevels, CalcMaxDistanceToTip, InternalReCalcLevels, InternalReDoID2Node
    '              newickParser, NewickParser, NexusParser, nhxParser, parseInforAndMakeNewLeafNode
    '              phyloXMLparserNodesIterator, reCalcDistanceToRoot, reCalcMaxDistanceToTip, rerootTree
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text

Imports RegExp = System.Text.RegularExpressions.Regex
Imports MatchResult = System.Text.RegularExpressions.MatchCollection

Imports Node = System.Xml.XmlNode
Imports Element = System.Xml.XmlElement
Imports NodeList = System.Xml.XmlNodeList

Imports Microsoft.VisualBasic

Namespace Evolview

    '
    '	 * >>>> bootstrap score support in newick format; normal + itol styles (input)
    '	 * >>>> dec 29, 2010: in newick format, bootstrap scores often take positions of
    '	 * internal node IDs, for example: (A:0.12,B:0.31)90:0.45; in which '90' is
    '	 * suppose to be a ID for internal node, which make things confusing
    '	 *
    '	 * this format is supported by many programs, including Dendroscope, phyml and
    '	 * phylip
    '	 *
    '	 * itol supports this format; however, it has its own implementation /
    '	 * extention, like the following: (A:0.12,B:0.31):0.45[90]; which is better in
    '	 * my oppinoin; however, this format is not supported by Dendroscope
    '	 *
    '	 *
    '	 * >>>> levels (horizontal and vertical); useful in drawing phylo-tree >>>>> dec
    '	 * 29, 2010: levels are assigned to leaf nodes while they are created; level
    '	 * values for internal nodes are calculated using two functions: "" (for
    '	 * horizontal) and "" (for vertical)
    '	 *
    '	 * >>>> add support to output tree in newick format >>>> dec 29, 2010:
    '	 *
    '	 * >>>>> add fail-save to parsing the results >>>>>>
    '	 *
    '	 * >>>> Feb 2, 2012: add support to ncbi exported phylip format check the tree
    '	 * label names before exporting
    '	 *
    '	 * >>>> April 4, 2013; fix a bug in parsing tree like : ((a:1,b):3,(c:1,(d:1,e:3):1):2);
    '	 * an older version thinks this tree has bootstrap values
    '	 *
    '	 * >>>> Sep 10, 2013; fix a but in parsing tree like : ((a,b)0.88,(c,d)0.99)0.99;
    '	 * this tree has bootstrap value, but not branch length (i think it's very strange)
    '	 *
    '	 * >>>> oct 19, 2013: fix a bug in parsing bootstrap value in nexus format like this: ... A:0.1)[&label=99]:0.2
    '	 *
    '	 * >>>> oct 25, 2013: implement reroot tree function; a tree can be rerooted at a leaf node as well as an internal node
    '	 *
    '	 *
    '

    Public NotInheritable Class PhyloTree

        ' NOTE: allNodes DO contain rootnode
        Private _AllNodes As New List(Of PhyloNode)(), _LeafNodes As New List(Of PhyloNode)()
        ' March 24, 2011
        Private _RootNode As PhyloNode = Nothing
        Private _TreeName As String = "", _InternalTreeStringData As String = "", _ErrorMessage As String = ""
        Private hasBootStrap As Boolean = False, isValidDataset As Boolean = True, isTreeEndWithSemiColon As Boolean = False
        Private _MaxVerticalLevel As Integer = 0, serial_internal_node As Integer = 1, serial_leaf_node As Integer = 0
        Private _hashID2Nodes As New Dictionary(Of String, PhyloNode)()
        ' Jan 22, 2011; 'ID' could be leaf node IDs or internal IDs
        Private _hsInternalID2ExternalID As New Dictionary(Of String, String)()
        Private _MaxRootToTipTotalBranchLength As Single = 0

        ' oct 20, 2013: some regular expression stuff
        Friend RegExpFloat As New System.Text.RegularExpressions.Regex("(\d+\.?\d*)")
        Friend MatchLeftBracket As New System.Text.RegularExpressions.Regex("\[")

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="treename">ϵͳ������������</param>
        ''' <param name="TreestrData">�������������ļ����ı�����</param>
        ''' <param name="format">�ļ���ʽ��nhx, newick, nexus��Ĭ�ϸ�ʽΪphylip��Ĭ�������ʽnewick</param>
        ''' <remarks></remarks>
        Sub New(TreeName As String, TreestrData As String, Format As String)
            TreestrData = TreestrData.Trim()

            Me._TreeName = TreeName
            Me._InternalTreeStringData = TreestrData
            Me._TreeFormat = Format

            '
            '			 * initiate rootnode and add it to allNodes
            '

            _RootNode = Me.MakeNewInternalNode("", "INT" & serial_internal_node, True, Nothing)

            If Format.Equals("newick") Then
                ' currently only newick trees are supported
                TreestrData = TreestrData.Replace(vbLf, "")

                ' first of all, check if the tree is valid
                Dim iBrackets As Integer = 0
                For Each c As Char In TreestrData.Trim().ToCharArray()
                    If c = "("c Then
                        iBrackets += 1
                    ElseIf c = ")"c Then
                        iBrackets -= 1
                    End If
                Next

                If iBrackets <> 0 Then
                    Me.isValidDataset = False
                    Me._ErrorMessage = "the numbers of '(' and ')' do not match, please check your tree!!"
                Else
                    newickParser(TreestrData, Nothing, Me._RootNode)
                End If
            ElseIf Format.ToUpper() = "nhx".ToUpper() Then
                TreestrData = TreestrData.Replace(vbLf, "")
                nhxParser(TreestrData)
            ElseIf Format.ToUpper() = "nexus".ToUpper() Then
                NexusParser(TreestrData)
                'ElseIf format.ToUpper() = "phyloXML".ToUpper() Then
                '                phyloXMLParser(treestr)
            End If

            '
            '			 * March 2, 2011; check if current tree data is valid
            '

            If Me.isValidDataset Then
                ' if it is still true
                If Me._AllNodes.Count < 3 Then
                    Me.isValidDataset = False
                    Me._ErrorMessage = "at least THREE nodes (two leaves and one internal node) are required"
                ElseIf Me.isTreeEndWithSemiColon = False Then
                    Me.isValidDataset = False
                    Me._ErrorMessage = "tree doesn't end with a ';', please check"

                    '
                    '					 * check intergrety of current tree a tree is valid if only : 1. all
                    '					 * nodes have valid/ non-null ID 2. all nodes have unique ID
                    '

                Else
                End If
            End If

            If Me.isValidDataset Then
                '
                '				 * after successfully loading the tree, calculate height (max
                '				 * distance to tip) and distance to root
                '

                HasBranchLength = Me.HasBranchLength()
                ' this goes first!!
                Me.reCalcDistanceToRoot()
                Me.reCalcMaxDistanceToTip()
                Me.InternalReCalcLevels()
                'actually this function can be intergrated with reCalcMaxDistanceToTip or reCalcDistanceToRoot; but I don't want the mix thing tegother

                Me.InternalReDoID2Node()
            End If
            ' if tree is valid
        End Sub

        Private Sub InternalReDoID2Node()
            Dim i As Integer = 1

            Call _hashID2Nodes.Clear()
            '
            '			 * at the end, go through all nodes and make a ID2Node hash
            '
            For Each Node As PhyloNode In Me._AllNodes
                Dim Node_ID As String = Node.ID
                ' NOTE: jan 25, 2011; rootNote usually doesn't have any ID
                Dim InternalNode_ID As String = Node.InternalID

                If String.IsNullOrEmpty(Node_ID) Then
                    Node_ID = InternalNode_ID ' internalID cannot be empty
                End If

                If Me._hashID2Nodes.ContainsKey(Node_ID) Then
                    Node_ID = Node_ID & "_" & i
                    i += 1
                    Node.ID = Node_ID
                End If

                Call Me._hashID2Nodes.Add(Node_ID, Node)
            Next
        End Sub

        ''' <summary>
        '''		 * >>>>>>>> >>>>>>>>> some important methods; I set them 'protected/private'
        '''		 * so that they are invisible to users
        '''
        '''
        '''			 * calculate horizontal and vertical levels for internal nodes, vertical
        '''			 * level = (min(levels of all descendents) + max(levels of all
        '''			 * descendents)) / 2; horizontal level = max(levels of all descendents)
        '''			 * + 1; therefore root has the max horizontal level
        '''			 *
        '''			 * jan 7, 2011; add level_vertical_slanted = (min(levels of all
        '''			 * descendents) + (max(levels of all descendents)) - min) / 2;
        '''
        '''
        '''
        '''			 * parameters are: node, array ref to hold horizontal levels of all its
        '''			 * descendents array ref to hold vertical levels of all its descendents
        '''			 * array ref to hold horizontal levels of its parent array ref to hold
        '''			 * vertical levels of its parent array ref to hold vertical levels of
        '''			 * all its leaf descendents of the parent node array ref to hold
        '''			 * vertical levels of all its leaf descendents of the current node
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub InternalReCalcLevels()
            Call CalcLevels(_RootNode, New List(Of Single)(), New List(Of Single)(), New List(Of Single)(), New List(Of Single)(), New List(Of Single)(), New List(Of Single)())
        End Sub

        ''' <summary>
        '''
        '''			 * recalculate distance_to_root for each node; distance to root is the
        '''			 * total branch length from a given node to the root I use a nested
        '''			 * function to do the calculation
        '''			 *
        '''			 * NOTE: this program will continue if only there is valid branchlength
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub reCalcDistanceToRoot()
            If Me.HasBranchLength Then
                Call CalcDistanceToRoot(_RootNode, 0)
            End If
        End Sub

        ''' <summary>
        '''
        '''			 * recalculate height for each node; height is the max branchlength to
        '''			 * get to the tip; start with leaf nodes; calculate accumulative branch
        '''			 * length from it to internal nodes
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub reCalcMaxDistanceToTip()
            If Me.HasBranchLength = False Then
                Return
            End If

            For Each pnode As PhyloNode In _AllNodes
                If pnode.IsLeaf = True Then
                    ' start with leaf node only
                    CalcMaxDistanceToTip(pnode, 0)
                End If
            Next
        End Sub

        Private Function makeNewLeafNode(id As String, branch_length As Single, parentnode As PhyloNode, level_vertical As Integer) As PhyloNode
            Dim leafnode As New PhyloNode()
            leafnode.IsLeaf = True
            leafnode.IsRoot = False
            leafnode.ID = id
            leafnode.BranchLength = branch_length

            '
            '			 * level_vertical actually is the serial ID of leaf node; I use
            '			 * LEF_serial as internal ID of leaf nodes; Jan 14, 2011
            '

            Dim internalid As String = "LEF_" & level_vertical
            leafnode.InternalID = internalid

            '
            '			 * horizontal and vertical levels; dec 29, 2010
            '

            leafnode.LevelHorizontal = 1
            ' all leaf nodes have a horizontal level of 1
            leafnode.LevelVertical = level_vertical
            ' leaf nodes have integer vertical levels starting with 1
            '
            '			 * parental and descendental relationships
            '

            leafnode.Parent = parentnode
            parentnode.AddDescendent(leafnode)

            '
            '			 * add new leaf node
            '

            Me._LeafNodes.Add(leafnode)
            Me._AllNodes.Add(leafnode)
            Me._hsInternalID2ExternalID.Add(internalid, id)

            _MaxVerticalLevel = serial_leaf_node
            ' dec 30, 2010
            Return leafnode
        End Function

        ''' <summary>
        ''' Dec 5, 2011; can be used to make rootnode
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="internal_id"></param>
        ''' <param name="isroot"></param>
        ''' <param name="parentnode"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function MakeNewInternalNode(id As String, internal_id As String, isroot As Boolean, parentnode As PhyloNode) As PhyloNode
            Dim NewNodeObject As New PhyloNode()
            NewNodeObject.InternalID = internal_id
            NewNodeObject.IsRoot = isroot
            NewNodeObject.IsLeaf = False
            NewNodeObject.ID = id

            'System.out.println(id + " = " + internal_id);

            ' dec 5, 2011
            If Not isroot AndAlso parentnode IsNot Nothing Then
                NewNodeObject.Parent = parentnode
                parentnode.AddDescendent(NewNodeObject)
            End If

            Me._AllNodes.Add(NewNodeObject)
            Me._hsInternalID2ExternalID.Add(internal_id, id)
            Return NewNodeObject
        End Function

        '
        '		 * a function calls itself during runing is quite dangerous ...
        '

        Protected Friend Sub CalcDistanceToRoot(node As PhyloNode, accumulated_distance As Single)
            Dim branchlength As Single = If(node.IsRoot, 0, node.BranchLength)
            ' Feb 28, 2011; bug fix
            accumulated_distance += branchlength
            node.BranchLengthToRoot = accumulated_distance

            If node.IsLeaf = True Then
                Return
            End If

            For Each descendent_node As PhyloNode In node.Descendents
                CalcDistanceToRoot(descendent_node, accumulated_distance)
            Next
        End Sub

        '
        Protected Friend Sub CalcMaxDistanceToTip(pnode As PhyloNode, accumulated_height As Single)
            If pnode.MaxDistanceToTip < accumulated_height Then
                pnode.MaxDistanceToTip = accumulated_height
            End If

            If pnode.IsRoot = True OrElse pnode.Parent Is Nothing Then
                ' quite if only current node is root or it has no parent
                Return
            End If

            accumulated_height += pnode.BranchLength

            CalcMaxDistanceToTip(pnode.Parent, accumulated_height)

            Me._MaxRootToTipTotalBranchLength = accumulated_height
        End Sub

        ' created: dec 29, 2010; last modified: dec 29, 2010; version = 1.0;
        ' revised jan 7, 2011; ver = 1.1; add level_vertical_slanted (for slanded middle mode);
        ' also added min/max leaf vertical level for each internal node, for slanded normal mode;
        Protected Friend Sub CalcLevels(node As PhyloNode, horiz As List(Of Single), verti As List(Of Single), horiz_parent As List(Of Single), verti_parent As List(Of Single), verti_leaf_only_parent As List(Of Single), verti_leaf_only_current As List(Of Single))
            ' do nothing is current node is leaf
            If node.IsLeaf = True Then
                ' do nothing
                Return
            End If

            ' if current node is an internal node; go through its descendents
            For Each descendent As PhyloNode In node.Descendents
                If descendent.IsLeaf = True Then
                    horiz.Add(descendent.LevelHorizontal)
                    verti.Add(descendent.LevelVertical)

                    ' jan 7, 2011
                    verti_leaf_only_current.Add(descendent.LevelVertical)
                Else
                    '
                    '					 * call this function itself, be careful always;
                    '

                    ' call this function itself
                    CalcLevels(descendent, New List(Of Single)(), New List(Of Single)(), horiz, verti, verti_leaf_only_current, New List(Of Single)())
                End If
            Next

            '
            '			 * now we've obtained horizontal and vertical levels for all
            '			 * descendents, I'll do: a. calculate and save vertical and horizontal
            '			 * level for current node b. return the two values to the parent node,
            '			 * because these values are necessary to calculate corresponding levels
            '			 * of the parent.
            '

            ' calculate vertical level for current node and add the value to its parent node
            Dim current_vertical_level As Single = ((verti.Min) + (verti.Max)) / 2
            node.LevelVertical = current_vertical_level
            verti_parent.Add(current_vertical_level)

            ' calculate horizontal level for current node and add the value to its parent node
            Dim current_horizontal_level As Single = (horiz.Max) + 1
            node.LevelHorizontal = current_horizontal_level
            horiz_parent.Add(current_horizontal_level)

            ' jan 7, 2011; level_vertical_slanted; and min/max leaf vertical level for each internal nodes
            Dim min_leaf_vertial_level As Single = (verti_leaf_only_current.Min)
            Dim max_leaf_vertial_level As Single = (verti_leaf_only_current.Max)

            node.minLeafVerticalLevel = min_leaf_vertial_level
            node.maxLeafVerticalLevel = max_leaf_vertial_level

            node.LevelVerticalSlanted = (min_leaf_vertial_level + max_leaf_vertial_level) / 2

            verti_leaf_only_parent.Add(min_leaf_vertial_level)
            verti_leaf_only_parent.Add(max_leaf_vertial_level)
        End Sub
        ' <<<<<<<< <<<<<<<<<

        '
        '		 * >>>>>>> public methods >>>>>>>>> some boring /public methods (get/set
        '		 * pairs) so I put them at the bottom ... ...
        '		 *
        '

        Public Property ID As String
            Get
                Return _TreeName
            End Get
            Set(value As String)
                _TreeName = value
            End Set
        End Property

        Public Property InternalID As String
        Public Property Description As String

        Public ReadOnly Property RootNode() As PhyloNode
            Get
                Return _RootNode
            End Get
        End Property

        Public Function hasBootstrapScores() As Boolean
            Return hasBootStrap
        End Function

        Public ReadOnly Property maxVerticalLevel() As Integer
            Get
                Return _MaxVerticalLevel
            End Get
        End Property

        Public ReadOnly Property maxHorizontalLevel() As Single
            Get
                Return _RootNode.LevelHorizontal
            End Get
        End Property

        ' Jan 22, 2011;
        Public Function getNodeByID(node_id As String) As PhyloNode
            ' Note: node_id could be internal AND real IDs
            If Me._hashID2Nodes.ContainsKey(node_id) Then
                Return Me._hashID2Nodes(node_id)
            Else
                Return Nothing
            End If
        End Function

        ' Jan 22, 2011
        Public Function getAllAncestors(node As PhyloNode) As List(Of PhyloNode)
            Dim ancestorNodes As New List(Of PhyloNode)()

            While node.IsRoot = False
                ancestorNodes.Add(node.Parent)

                node = node.Parent
            End While

            Return ancestorNodes
        End Function

        '
        '		 * has branch length? this is only true if all nodes (except root) have
        '		 * branch length dec 29, 2010
        '
        Dim _HasBranchLength As Boolean = False

        Public Property HasBranchLength() As Boolean
            Get
                For Each node As PhyloNode In Me.AllNodes
                    If node.IsRoot = False Then
                        If node.BranchLength > 0 Then
                            _HasBranchLength = True
                            Exit For
                        End If
                    End If
                Next
                ' end of for
                Return _HasBranchLength
            End Get
            Set(value As Boolean)
                _HasBranchLength = value
            End Set
        End Property

        ''' <summary>
        ''' *******************************************************
        ''' leaf nodes and leaf node names.
        ''' *******************************************************
        ''' </summary>
        Public ReadOnly Property LeafNodes() As List(Of PhyloNode)
            Get
                Return Me._LeafNodes
            End Get
        End Property

        '
        '		 * May 13, 2012
        '

        Public ReadOnly Property AllLeafLabels() As String()
            Get
                Dim LQuery = (From pn As PhyloNode In LeafNodes Select pn.ID).ToArray
                Return LQuery
            End Get
        End Property
        ' May 13, 2012
        Public ReadOnly Property FirstLeafNode() As PhyloNode
            Get
                Return If(Me._LeafNodes.Count > 0, Me._LeafNodes(0), Nothing)
            End Get
        End Property

        Public ReadOnly Property LastLeafNode() As PhyloNode
            Get
                Return If(Me._LeafNodes.Count > 0, Me._LeafNodes(Me._LeafNodes.Count - 1), Nothing)
            End Get
        End Property

        ''' <summary>
        ''' *******************************************************
        ''' all nodes *******************************************************
        ''' </summary>
        Public ReadOnly Property AllNodes() As List(Of PhyloNode)
            Get
                Return _AllNodes
            End Get
        End Property

        '
        '		 * >>>> some non-get/set public functions that are more interesting
        '		 * Oct 24, 2013 --
        '		 * reroot tree at a given node, the latter can be a leaf or internal node
        '		 * if a leaf node is given, then the leaf node will be used as a outgroup --
        '

        Public Sub rerootTree(inode As PhyloNode)
            Console.WriteLine(" ----- reroot tree starts ------")
            ' first, check if this leaf connects directly to root
            ' cannot!!!!
            If inode.IsRoot OrElse inode.Parent.IsRoot Then
            Else
                ' make a new root
                Dim newroot As New PhyloNode()
                newroot.IsRoot = True
                newroot.IsLeaf = False
                newroot.ID = ""
                Me.serial_internal_node += 1
                Dim internalID As String = "INT" & serial_internal_node
                newroot.InternalID = internalID
                System.Console.WriteLine(vbTab & "make a new root with ID : " & internalID)

                ' get the parent node of current leaf
                Dim pnode__1 As PhyloNode = inode.RemoveFromParent()

                ' step 1: ----
                ' set this new root as parent of current node
                ' add current leaf to this new root;
                newroot.AddDescendent(inode)
                ' the parent will be set automatically ...

                ' make a new internal node, add it to the new root
                Dim newINode As New PhyloNode()
                Me.serial_internal_node += 1
                Dim newInternalNodeID As String = "INT" & serial_internal_node
                newINode.InternalID = newInternalNodeID
                newINode.ID = ""
                newroot.AddDescendent(newINode)
                ' the parent will be set automatically ...

                '  if this tree has branch length
                '  two senarios:
                '  1. if current node is a leaf, then the leaf take 95% of the branch length, and the new internal node take 5%
                '  2. if current node is an internal node, split it 60% 40%
                If Me.HasBranchLength Then
                    Dim branchlength As Single = inode.BranchLength
                    If inode.IsLeaf Then
                        inode.BranchLength = branchlength * 0.95F
                        newINode.BranchLength = branchlength * 0.05F
                    Else
                        inode.BranchLength = branchlength * 0.6F
                        newINode.BranchLength = branchlength * 0.4F
                    End If
                End If

                '  step 3: from the parent node of current node 'pnode', go all the way up to root
                '  change the parent / descendent relationship
                Dim cNode As PhyloNode = newINode
                ' note: here pnode cannot be root, so it has to have a valid parent
                Dim pNode__2 As PhyloNode = pnode__1
                '...
                While Not pNode__2.IsRoot
                    ' remove all daughter nodes from current node
                    cNode.RemoveAllDescendents()

                    ' remove pNode from its parent
                    Dim ppNode As PhyloNode = pNode__2.RemoveFromParent()

                    ' make ppNode a daughter node of current node
                    cNode.AddDescendent(pNode__2)

                    System.Console.WriteLine("   --> " & cNode.InternalID)
                    ' and also add all the daughter nodes of its parent as its daughters
                    For Each node As PhyloNode In pNode__2.Descendents
                        cNode.AddDescendent(node)
                    Next

                    ' now:
                    cNode = pNode__2
                    pNode__2 = ppNode
                End While

                ' step 4 : now pNode is the root and cNode is the node that connects directly to the old root
                ' this step will delete the old root, and add other descendent nodes
                ' to 'cNode'
                ' first of all, remove cNode from its pare
                pNode__2 = cNode.RemoveFromParent()
                ' note this will return the new parent of cNote, not the old root
                ' also note that the old rootNode no longer has cNode ...
                For Each node As PhyloNode In Me._RootNode.Descendents
                    pNode__2.AddDescendent(node)
                    If Me.HasBranchLength Then
                        node.BranchLength = node.BranchLength + cNode.BranchLength
                    End If
                Next

                ' !!! important, this has to be done at the near end of the function, otherwise you will lose all
                ' nodes at the other side of the root node...
                _RootNode = newroot

                ' step 5 : final steps
                ' remake all essential variables ...
                Me._AllNodes.Clear()
                Me._LeafNodes.Clear()
                Me._hashID2Nodes.Clear()
                Me._hsInternalID2ExternalID.Clear()
                Me.reMakeEssentialVariables(_RootNode, 0)
                ' oct 25, 2013
                ' recaclulate some other variables
                Me.reCalcDistanceToRoot()
                Me.reCalcMaxDistanceToTip()
                Me.InternalReCalcLevels()
                'this.reDoID2Node();

                Console.WriteLine("-------- reroot tree done ---------")
            End If
        End Sub
        ' function root
        ''' <summary>
        ''' Oct 25, 2013; this is a recursive function
        ''' the four global variables will be changed in this function:
        ''' allNodes, leafNodes, hashID2Nodes, hsInternalID2externalID
        '''
        ''' also fix the parent and descendent relationships
        ''' </summary>
        ''' <param name="node"> </param>
        Private Function reMakeEssentialVariables(node As PhyloNode, leafVerticalLevel As Integer) As Integer
            ' reDoID2Node () ...
            Dim node_id As String = node.ID
            ' NOTE: jan 25, 2011; rootNote usually doesn't have any ID
            Dim internalnode_id As String = node.InternalID

            If node_id IsNot Nothing AndAlso node_id.Trim().Length > 0 Then
                Me._hashID2Nodes.Add(node_id, node)
            End If

            ' internalID cannot be empty
            If internalnode_id.Trim().Length > 0 Then
                Me._hashID2Nodes.Add(internalnode_id, node)
            End If

            If node.IsLeaf Then

                ' internal ID to external id; for leaf nodes only
                Me._hsInternalID2ExternalID.Add(internalnode_id, node_id)

                ' add current node to leafNodes
                Me._LeafNodes.Add(node)
                leafVerticalLevel += 1
                node.LevelVertical = leafVerticalLevel
            Else

                For Each dnode As PhyloNode In node.Descendents
                    'dnode.setParent( node );
                    leafVerticalLevel = reMakeEssentialVariables(dnode, leafVerticalLevel)
                Next
            End If

            ' add all nodes to allnodes, including root node
            Me._AllNodes.Add(node)

            Return leafVerticalLevel
        End Function

        '
        '		 * March 2, 2011;
        '

        Public ReadOnly Property treeDataValid() As Boolean
            Get
                Return Me.isValidDataset
            End Get
        End Property

        Public ReadOnly Property errorMessage() As String
            Get
                Return Me._ErrorMessage
            End Get
        End Property

        Public ReadOnly Property maxTotalBranchLengthFromRootToAnyTip() As Single
            Get
                Return Me._MaxRootToTipTotalBranchLength
            End Get
        End Property

        '
        '		 * get last common ancestor given any leaf-nodes (internal, non-internal Jan
        '		 * 22, 2011 getLCA works for any nodes, leaf or non-leaf; March 17 2011;
        '

        Public Function getLCA(leaf_ids As List(Of String)) As PhyloNode

            '
            '			 * first of all, get a list of valid nodes and their ancestors
            '			 * corresponding to leaf_ids
            '

            Dim leaf_nodes As New List(Of PhyloNode)()
            Dim ancestors As New List(Of List(Of PhyloNode))()
            For Each leaf_id As String In leaf_ids
                Dim leaf_node As PhyloNode = Me.getNodeByID(leaf_id)
                If leaf_node IsNot Nothing Then
                    ancestors.Add(Me.getAllAncestors(leaf_node))
                    leaf_nodes.Add(leaf_node)
                End If
            Next

            '
            Dim lca As PhyloNode = Nothing

            If leaf_nodes.Count = 1 Then
                lca = leaf_nodes(0)
                ' do nothing
            ElseIf leaf_nodes.Count = 0 Then
            Else

                For Each ancestor As PhyloNode In ancestors(0)
                    Dim isLCA As Boolean = True
                    For idx As Integer = 1 To ancestors.Count - 1
                        If ancestors(idx).Contains(ancestor) = False Then
                            isLCA = False
                        End If
                    Next

                    If isLCA = True Then
                        lca = ancestor
                        Exit For

                    End If
                    ' for ancesters
                Next
            End If
            ' leaf_nodes.size
            Return lca
        End Function
        ' get lca
        '
        '		 * >>>> to string in Newick| nexus | nhx format >>>>> three parameters are
        '		 * required: 1. show/ hide bootstrap score, if available 2. show/ hide
        '		 * internal id (if not 3, it's not possible to show both internal ID and
        '		 * bootstrap score at the same time) 3. itol-compatible style bootstrap
        '		 * scores
        '		 *
        '		 * note: if the tree has branch lengths, they will appear in the tree
        '

        ' created on dec 29, 2010; last modified: dec 29, 2010; version = 1.0; by Weihua Chen
        Public Function toTreeString(treeFormat As String, showBootstrap As Boolean, showInternalID As Boolean, writeItolFormat As Boolean) As String
            '
            '			 * check the following parameters first showBootstrap showInternalID
            '			 * showBranchlength writeItolFormat; note writeItolFormat is true only
            '			 * if showBootstrap == true
            '

            Dim showBranchlength As Boolean = Me.HasBranchLength
            'will show branch length if the tree has bl-values
            If showBootstrap = True AndAlso Me.hasBootStrap = False Then
                showBootstrap = False
            End If

            If showBootstrap = False OrElse showBranchlength = False OrElse treeFormat.ToUpper() = "nexus".ToUpper() Then
                ' itol style requires both bootstrap AND branchlength
                writeItolFormat = False
            End If

            If showBootstrap = True AndAlso writeItolFormat = False Then
                showInternalID = False
            End If

            '
            '			 * Dec 5, 2011; prepare translate
            '

            Dim hashTranslate As Dictionary(Of String, String) = Nothing
            If treeFormat.ToUpper() = "nexus".ToUpper() Then
                hashTranslate = New Dictionary(Of String, String)()
                Dim serial As Integer = 0
                For Each leafnode As PhyloNode In Me._LeafNodes
                    serial += 1
                    hashTranslate.Add(leafnode.ID, Convert.ToString(serial))
                Next
            End If

            '
            '			 * >>>> how to write a tree to string? dec 29, 2010 >>>>> basically,
            '			 * there are three steps: 1. for each internal node, find all its
            '			 * descendents, join them with ',', and surround the joint string with
            '			 * () 2. append ID/internalID/boostrap/branchlength of the internal node
            '			 * to the string 3. repeat 1&2 until the internal node is root
            '

            ' let's start with a simple tree
            Dim treestr_newick As String = phyloNodeToString(Me._RootNode, New List(Of String)(), New List(Of String)(), showBootstrap, showInternalID, showBranchlength,
                writeItolFormat, treeFormat, hashTranslate)

            If treeFormat.ToUpper() = "nexus".ToUpper() Then
                Dim treestr As New StringBuilder()
                treestr.Append("#NEXUS" & vbLf & vbLf).Append("Begin trees; [Treefile created using EViewer]" & vbLf).Append(vbTab & "Translate" & vbLf)
                Dim serial As Integer = 0
                For Each leafnode As PhyloNode In Me._LeafNodes
                    serial += 1
                    If leafnode.ID.Contains(" ") Then
                        treestr.Append(vbTab & vbTab).Append(serial).Append(" '").Append(leafnode.ID).Append("'")
                    Else
                        treestr.Append(vbTab & vbTab).Append(serial).Append(" ").Append(leafnode.ID)
                    End If

                    If serial < Me._LeafNodes.Count Then
                        treestr.Append(",")
                    End If
                    treestr.AppendLine()
                Next
                treestr.Append(";").Append(vbLf)
                treestr.Append(vbTab & " tree tree_1 = [&R] ").Append(treestr_newick).Append(vbLf)
                treestr.Append("End;").Append(vbLf)

                treestr_newick = treestr.ToString()
            End If

            Return treestr_newick
        End Function

        Public ReadOnly Property treeString() As String
            Get
                Return Me._InternalTreeStringData
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _InternalTreeStringData
        End Function

        ' Dec 5, 2011; to export to other tree formats, including nhx and nexus
        Protected Friend Function phyloNodeToString(node As PhyloNode, for_descendents As List(Of String), for_parent As List(Of String), showBootstrap As Boolean, showInternalID As Boolean, showBranchlength As Boolean,
            writeItolFormat As Boolean, treeFormat As String, hashTranslate As Dictionary(Of String, String)) As String

            ' do nothing if current node is leaf
            If node.IsLeaf = True Then
                ' do nothing
                Return Nothing
            End If

            ' if current node is an internal node; go through its descendents
            For Each descendent As PhyloNode In node.Descendents
                If descendent.IsLeaf = True Then
                    '
                    '					 * assemble a leaf string: ID:branchlength, in which
                    '					 * ':branchlength' is depending on showBranchlength and add it
                    '					 * to 'for_descendents'
                    '

                    ' leaf id
                    Dim str_leaf As String = descendent.ID
                    If treeFormat.ToUpper() = "nexus".ToUpper() AndAlso hashTranslate IsNot Nothing AndAlso hashTranslate.ContainsKey(str_leaf) Then
                        str_leaf = hashTranslate(str_leaf)
                    ElseIf str_leaf.Contains(" ") Then
                        ' Feb 2, 2012
                        str_leaf = "'" & str_leaf & "'"
                    End If

                    ' leaf branch length, if available
                    If showBranchlength = True Then
                        ' showBranchlength is true means there is branch-length data for all nodes
                        str_leaf += ":" & Convert.ToString(descendent.BranchLength)
                    End If

                    ' nhx string, if available; not implemented

                    for_descendents.Add(str_leaf)
                Else
                    '
                    '					 * call this function itself, be careful always;
                    '

                    ' call this function itself
                    phyloNodeToString(descendent, New List(Of String)(), for_descendents, showBootstrap, showInternalID, showBranchlength,
                        writeItolFormat, treeFormat, hashTranslate)
                End If
            Next

            '
            '			 * now we've obtained string representation of all descendents for
            '			 * current node, I'll a. assemble string representation for this
            '			 * (internal) node b. return the string to the parent node, it's needed
            '			 * to assemble string representation for the parent node
            '

            ' assemble string representation for current internal node;
            ' normally, one internal node should at least have two+ descendents,
            '   but in itol, internal node without descendents is supported; I'll deal with this latter
            Dim str_internal_node As String = ""

            Dim size_of_for_descendents As Integer = for_descendents.Count
            For i As Integer = 0 To size_of_for_descendents - 1
                str_internal_node += for_descendents(i)
                If i < (size_of_for_descendents - 1) Then
                    str_internal_node += ","
                End If
            Next
            str_internal_node = "(" & str_internal_node & ")"

            '
            '			 * get annotation to current internal node
            '

            Dim internalnode_id As String = If(node.ID.Length > 0, node.ID, node.InternalID)
            If internalnode_id.Contains(" ") Then
                ' Feb 2, 2012
                internalnode_id = "'" & internalnode_id & "'"
            End If

            Dim str_anno As String = ""

            If treeFormat.ToUpper() = "nhx".ToUpper() Then
                Dim nhxAttrs As New Dictionary(Of String, String)()
                If showBootstrap Then
                    nhxAttrs.Add("B", Convert.ToString(node.BootStrap))
                End If
                If showInternalID Then
                    nhxAttrs.Add("ND", node.InternalID)
                End If

                ' now summarize --
                If showBranchlength Then
                    str_anno += ":" & node.BranchLength
                End If
                If Not nhxAttrs.Count = 0 Then
                    str_anno += "[&&NHX"
                    For Each kv As KeyValuePair(Of String, String) In nhxAttrs
                        str_anno += ":" & kv.Key & "=" & kv.Value
                    Next
                    str_anno += "]"
                End If
            ElseIf treeFormat.ToUpper() = "newick".ToUpper() Then
                If writeItolFormat = True Then
                    str_anno += ":" & node.BranchLength & "[" & node.BootStrap & "]"
                    If showInternalID = True Then
                        str_anno = internalnode_id & str_anno
                    End If
                Else
                    If showBootstrap = True Then
                        str_anno = Convert.ToString(node.BootStrap)
                    End If

                    If showInternalID = True Then
                        str_anno = internalnode_id
                    End If

                    If showBranchlength = True Then
                        str_anno += ":" & node.BranchLength
                    End If
                End If
            ElseIf treeFormat.ToUpper() = "nexus".ToUpper() Then
                If showBranchlength = True Then
                    str_anno += ":" & node.BranchLength
                End If

                If showInternalID = True Then
                    str_anno += "[" & internalnode_id & "]"
                End If
            End If

            ' append the annotation to the ID of internal node
            str_internal_node += str_anno

            If node.IsRoot = True Then
                str_internal_node += ";"c
                Return str_internal_node
            Else
                for_parent.Add(str_internal_node)
                Return Nothing
            End If
            ' end of node.isRoot
        End Function
        ' end of function phyloNodeToString
        ''' <summary>
        ''' created: Oct 20, 2013 : a better and easier to maintain parser for newick and nexus trees
        ''' NOTE: this is a recursive function </summary>
        ''' <param name="inputstr"> : input tree string </param>
        ''' <param name="hashTranslate"> : aliases for lead nodes (for nexsus format) </param>
        ''' <param name="iNode"> : current internal node; == rootNode the first time 'newickParser' is called  </param>
        Private Sub newickParser(inputstr As String, hashTranslate As Dictionary(Of String, String), iNode As PhyloNode)
            inputstr = inputstr.Trim()

            ' NOTE: the input string should look like this: (A,B,(C,D)E)F
            ' the first char has to be (
            ' first, get what's between the first and the last Parentheses, and what's after the last right Parentheses
            ' for example, your tree : (A,B,(C,D)E)F will be split into two parts:
            '   A,B,(C,D)E = ...
            '   F = tail string
            Dim tailString As String = ""

            If Not inputstr.Length = 0 Then
                ' remove trailing ';'
                While inputstr.EndsWith(";")
                    Me.isTreeEndWithSemiColon = True
                    ' is this really necessary???
                    inputstr = inputstr.Substring(0, inputstr.Length - 1)
                End While
                For idx As Integer = inputstr.Length - 1 To 0 Step -1
                    If inputstr(idx) = ")"c Then
                        tailString = inputstr.Substring(idx + 1)

                        ' change input str from (A,B,(C,D)E)F to A,B,(C,D)E
                        inputstr = inputstr.Substring(1, idx - 1)
                        ' !!!!!
                        Exit For
                    End If
                Next
            End If
            ' if string is not empty
            ' parse the tail string and get information for current internal node
            ' possibilities
            ' 1. nothing ...
            ' 2. bootstrap only: )99, or )0.99
            ' 3. branch length only: ):0.456
            ' 4. both bootstrap and branch length )99:0.456
            ' 5. internal ID : )str
            ' 6. internal ID with branch length : )str:0.456
            ' 7. itol style, internalID, bootstrap and branch length: )internalID:0.2[95]
            ' 8. nexus style bootstrap: )[&label=99]:0.456
            If Not tailString.Length = 0 Then
                ' first of all, split string into two parts by ':'
                Dim parts As String() = tailString.StringSplit(":", True)

                '  deal with the first part
                '  first, check case 8. Square brackets [&label=99]
                If parts.Length >= 1 Then
                    Dim part1 As String = parts(0)
                    If Not part1.Length = 0 Then
                        ' if not case 3
                        If MatchLeftBracket.Match(part1) IsNot Nothing Then
                            ' get the float number from a string like this: [&label=99]
                            Dim m As MatchResult = RegExpFloat.Matches(part1)
                            If (m IsNot Nothing) Then
                                Dim bootstrap As Single = Val(m.Item(0).Value)
                                iNode.BootStrap = bootstrap
                                Me.hasBootStrap = True
                            End If
                        Else
                            ' check if it is a string or numeric
                            Try
                                Dim bootstrap As Single = Val(part1)
                                ' if success; case 2, 4
                                iNode.BootStrap = bootstrap
                                Me.hasBootStrap = True
                            Catch
                                ' if fail, i assume the string is internal note name; case 5, 7
                                iNode.InternalID = part1
                            End Try
                        End If
                        ' if part 1 is not empty
                    End If
                End If

                ' if there is a part 2 and it's not empty --
                If parts.Length >= 2 Then
                    Dim part2 As String = parts(1)
                    If Not part2.Length = 0 Then
                        If MatchLeftBracket.Match(part2) IsNot Nothing Then
                            ' if it is the itol style

                            ' split it into two parts by '[', the first part should contain the branch lenth, while the second contains the bootstrap
                            ' of cource, both could be empty,
                            ' valid inputs are:
                            ' :[] - both are empty,
                            ' :[99]
                            ' :0.456[]
                            ' :0.456[99]
                            ' NOTE: bootstrap value can also be float number
                            Dim iparts As String() = part2.StringSplit("\[", True)
                            If iparts.Length > 0 Then
                                ' the first part: branch length
                                Dim ipart1 As String = iparts(0)
                                Dim m As MatchResult = RegExpFloat.Matches(ipart1)
                                If (m IsNot Nothing) Then
                                    Dim branchlength As Single = Val(m.Item(0).Value)
                                    iNode.BranchLength = branchlength
                                    Me.HasBranchLength = True
                                End If
                                ' branch length
                                ' the second part, bootstrap
                                If iparts.Length >= 2 Then
                                    Dim ipart2 As String = iparts(1)
                                    Dim m2 As MatchResult = RegExpFloat.Matches(ipart2)
                                    If (m IsNot Nothing) Then
                                        Dim bootstrap As Single = Val(m2.Item(0).Value)
                                        iNode.BootStrap = bootstrap
                                        Me.hasBootStrap = True
                                    End If
                                    ' bootstrap
                                End If
                            End If
                        Else
                            ' parse branch length value; case 3,4,6,8
                            Try
                                Dim branchlength As Single = Val(part2)
                                ' if success; case 2, 4
                                iNode.BranchLength = branchlength
                                ' do nothing
                            Catch
                            End Try
                            ' if ... else ...
                        End If
                        ' if part2 is not empty
                    End If
                    ' if part2 is there
                End If
            End If
            ' if the string4internalNode string is not empty

            ' now go through what's between the parentheses and get the leaf nodes
            '   (A,B,(C,D)E)F = original tree
            '   A,B,(C,D)E = the part the following codes will deal with
            If Not inputstr.Length = 0 Then

                ' split current input string into substrings, each is a daughtor node of current internal node
                ' if your input string is like this: A,B,(C,D)E
                ' it will be split into the following three daughter strings:
                '  A
                '  B
                '  (C,D)E
                ' accordingly, three daughter nodes will be created, two are leaf nodes and one is an internal node
                Dim brackets As Integer = 0, leftParenthesis As Integer = 0, commas As Integer = 0
                Dim sb As New StringBuilder()
                For Each c As Char In inputstr.ToCharArray()
                    If (c = ","c OrElse c = ")"c) AndAlso brackets = 0 Then
                        ' ',' usually indicates the end of an node; is || c == ')' really necessary ???
                        ' make daugher nodes
                        Dim daughter = sb.ToString()
                        If leftParenthesis > 0 AndAlso commas > 0 Then
                            serial_internal_node += 1
                            'System.out.println( serial_internal_node + "  " + daughter );
                            newickParser(daughter, hashTranslate, MakeNewInternalNode("", "INT" & serial_internal_node, False, iNode))
                        Else
                            ' a leaf daughter
                            serial_leaf_node += 1
                            ' parse information for current daughter node
                            parseInforAndMakeNewLeafNode(daughter, hashTranslate, iNode)
                        End If

                        ' reset some variables
                        sb = New StringBuilder()
                        leftParenthesis = 0
                    Else
                        sb.Append(c)
                        ' ',' will not be recored
                        If c = ","c Then
                            commas += 1
                        End If
                    End If

                    '  brackets is used to find the contents between a pair of matching ()s
                    '  how does this work???
                    '
                    '  here is how the value of brackets changes if your input string is like this :
                    '  (A,B,(C,D)E)F
                    '  1    2   1 0 # value of brackets ...
                    '  +    +   - - # operation
                    '  ^          ^ # contents between these two () will be extracted = A,B,(C,D)E
                    '
                    '  ---
                    '  variable 'leftParenthesis' is used to indicate whether current daughter node is likely a internal node;
                    '  however, this alone cannot garrentee this, because the name of a leaf node may contain Parenthesis
                    '  therefore I use 'leftParenthesis' and 'commas' together to indicate an internal node
                    If c = "("c Then
                        brackets += 1
                        leftParenthesis += 1
                    ElseIf c = ")"c Then
                        brackets -= 1
                    End If
                Next
                ' deal with the last daughter
                Dim LastDaughter As String = sb.ToString()
                If leftParenthesis > 0 AndAlso commas > 0 Then
                    serial_internal_node += 1

                    'System.out.println( serial_internal_node + "  " + daughter );
                    newickParser(LastDaughter, hashTranslate, MakeNewInternalNode("", "INT" & serial_internal_node, False, iNode))
                Else
                    ' a leaf daughter
                    serial_leaf_node += 1
                    ' parse information for current daughter node
                    parseInforAndMakeNewLeafNode(LastDaughter, hashTranslate, iNode)

                End If
            End If
            ' new recursive parser
        End Sub
        ' end of current function
        ''' <summary>
        ''' created on Oct 20, 2013
        ''' input: the leafstr to be parsed, the internal node the leaf node has to be added to
        ''' </summary>
        Private Sub parseInforAndMakeNewLeafNode(leafstr As String, hashTranslate As Dictionary(Of String, String), iNode As PhyloNode)
            leafstr = leafstr.Trim()

            ' parse a leaf node,
            ' possibilities are:
            ' 1. ,, - leaf node is not named (???)
            ' 2. A  - named leaf node
            ' 3. :0.1 - unamed leaf node with branch length
            ' 4. A:0.1 - named leaf node with branch length
            If leafstr.Length = 0 Then
                ' case 1
                makeNewLeafNode("", 0, iNode, serial_leaf_node)
            Else
                ' split it into two parts
                Dim parts As String() = leafstr.StringSplit(":", True)
                Dim branchlength As Single = 0
                ' first do part2, check if it has branch length
                If parts.Length >= 2 Then
                    Dim part2 As String = parts(1)
                    If Not part2.Length = 0 Then
                        Try
                            branchlength = Convert.ToSingle(part2)
                            Me.HasBranchLength = True
                            ' do nothing
                        Catch
                        End Try
                    End If
                End If

                ' now deal with part 1, two possibilities: named / unamed leaf node
                Dim part1 As String = parts(0)
                If part1.Length = 0 Then
                    makeNewLeafNode("", branchlength, iNode, serial_leaf_node)
                Else
                    Dim leafNodeName As String = part1.Replace("'", "").Replace("""", "")
                    leafNodeName = If((hashTranslate IsNot Nothing AndAlso hashTranslate.ContainsKey(leafNodeName)), hashTranslate(leafNodeName), leafNodeName)
                    makeNewLeafNode(leafNodeName, branchlength, iNode, serial_leaf_node)
                End If
            End If
        End Sub
        ' end of parseInforAndMakeNewLeafNode
        ' moved here on Nov 28, 2011;
        ''' <summary>
        ''' April 4, 2013;
        ''' bug fix; tree like this ((a:1,b):3,(c:1,(d:1,e:3):1):2); causes bootstrap value == true
        '''
        ''' Sep 10, 2013 : bug fix, tree with bootstrap but no branch length : ((a,b)0.88,(c,d)0.99)0.99;
        '''
        ''' Oct 19, 2013: nexus tree with bootstrap scores like this:
        '''
        ''' </summary>
        Private Sub NewickParser(treestr As String, hashTranslate As Dictionary(Of String, String))
            Dim name_str_backup As String = "", current_name_str As String = ""
            Dim current_internal_node As PhyloNode = _RootNode
            ' currrent internal node; set to rootnode
            Dim previous_chr As Char = " "c

            Dim isBranchLength As Boolean = False, isFirstLeftParenthesis As Boolean = True, isRightparenthesis As Boolean = False

            For idx As Integer = 0 To treestr.Length - 1
                Dim c As Char = treestr(idx)
                If c = "("c Then
                    '
                    '					 * possibilities: a. c is NOT the first '(', create a new
                    '					 * internal node b. c is immediately after a ',', create a new
                    '					 * internal node c. c is the first 'C', do nothing
                    '

                    If isFirstLeftParenthesis = False Then
                        ' a or b
                        '
                        '						* if not the first '(', the previous letter MUST BE ',' or
                        '						* '('
                        '

                        If previous_chr <> "("c AndAlso previous_chr <> ","c Then
                            Me._ErrorMessage = " pos:" & (idx - 1) & " '(' or ',' is expected, got '" & previous_chr & "'"
                            isValidDataset = False
                        Else
                            '
                            '							 * make a new internal node NOTE: the ID of the internal
                            '							 * node will be assigned later
                            '

                            serial_internal_node += 1
                            current_internal_node = MakeNewInternalNode("", "INT" & serial_internal_node, False, current_internal_node)
                        End If
                    Else
                        ' if true, set it to false
                        isFirstLeftParenthesis = False
                    End If

                    '
                    '					 * reset variables
                    '

                    isRightparenthesis = False

                    current_name_str = ""

                    name_str_backup = ""
                ElseIf c = "'"c Then
                    ' Feb 2, 2012: add support to phylip format
                    While True
                        idx += 1
                        Dim ch As Char = treestr(idx)
                        If ch = "'"c Then
                            ' matching
                            ' exit while loop
                            Exit While
                        Else
                            ' append it to current_name_str if ch is not '''
                            current_name_str += ch
                        End If

                    End While
                ElseIf c = ":"c Then
                    '
                    '					 * ':' indicates current tree has branch length
                    '

                    isBranchLength = True

                    If isRightparenthesis Then
                        Dim bootstrap As Single = -1
                        '
                        '						 * >>>> if NOT d or e >>>>> in ')name:0.01', if name is
                        '						 * numeric, it's bootstrap score, otherwise, it's id of
                        '						 * current internal node
                        '


                        If current_name_str.Trim().Length > 0 Then
                            Try
                                bootstrap = CSng(Convert.ToSingle(current_name_str.Trim()))
                            Catch
                                bootstrap = -1
                            End Try
                        End If

                        ' System.out.println("  bootstrap is : " + bootstrap + "; current name str is : " + current_name_str);

                        If bootstrap = -1 Then
                            current_internal_node.ID = current_name_str.Trim()
                        Else
                            current_internal_node.BootStrap = bootstrap
                            hasBootStrap = True
                        End If
                    End If

                    '
                    '					 * back up current name string and reset it
                    '

                    name_str_backup = current_name_str

                    current_name_str = ""
                ElseIf c = ","c OrElse c = ";"c OrElse c = ")"c OrElse c = "["c Then
                    '
                    '					 * get branch length from current name string
                    '

                    Dim branch_length As Single = 0
                    Try
                        branch_length = CSng(Convert.ToSingle(current_name_str.Trim()))
                    Catch
                        branch_length = 0
                    End Try

                    If isBranchLength = True Then
                        '
                        '						 * copy the value from backup
                        '						 *
                        '						 * *************************************************************
                        '						 * May 11, 2012 bug fix: copy the contents of a string is
                        '						 * only it's not empty
                        '

                        If Not name_str_backup.Length = 0 Then
                            current_name_str = name_str_backup
                        End If
                    End If

                    '
                    '					 * >>>>> possibilities: >>>>> a. ,name:0.01, or ,name:0.01)
                    '					 * create a new leaf node b. )name:0.01, or )name:0.01) save ID
                    '					 * to current internal node and set parent as current internal
                    '					 * node c. )name:0.01; save ID to internal node and end for loop
                    '					 * >>> dec 29, 2010; itol-style bootstrap >>> d.
                    '					 * )name:0.01[100.00]; in this case, name is the id of current
                    '					 * internal node, '0.01' is branch-length and 100.00 is the
                    '					 * bootstrap-score
                    '					 *
                    '					 * >>> Feb 2, 2012: nexus format with internal IDs, like: e.
                    '					 * ):0.01['internal id']
                    '					 *
                    '					 * Sep 10, 2013: new possibility:  f )0.01  with bootstrap value, but no branch length
                    '					 *
                    '


                    ' if internal node
                    If isRightparenthesis = True Then

                        '
                        '						 * possibilities b & c means the information is for internal
                        '						 * node; in this case, it will save the infor to current
                        '						 * internal node and change current internal node to its
                        '						 * parent if current internal node isn't the root
                        '						 *
                        '						 * dec 29, 2010; add bootstrap support in cases of b or c,
                        '						 * the 'name' part in 'name:0.01' could be the name of the
                        '						 * internal node or bootstrap score; the only way to
                        '						 * distinguish one from another is: if name is numeric
                        '						 * (integer or floot), it's bootstrap score otherwise, it's
                        '						 * name of the internal node
                        '						 *
                        '						 * NOTE: only internal node should have bootstrap score
                        '


                        Dim bootstrap As Single = -1

                        If c = "["c Then

                            ' first of all, get the string between []
                            ' ignore '
                            Dim string_between_parenthesis As String = ""
                            While True
                                idx += 1
                                Dim ch As Char = treestr(idx)
                                If ch = "]"c Then
                                    idx += 1
                                    c = treestr(idx)
                                    ' get the char next to ']' and assign it to c
                                    ' exit while loop
                                    Exit While
                                End If

                                If ch <> "'"c Then
                                    string_between_parenthesis += ch
                                End If
                            End While

                            ' check if cases d (itol style bootstrap) or e (internal id)
                            ' Oct 19, 2013: nexus style bootstrap value ... A:0.1)[&label=99]:0.2
                            If string_between_parenthesis.Trim().Length > 0 Then
                                Try
                                    '
                                    '									 * >>>> if itol style >>>> get the string
                                    '									 * between a [ and ]; and assign the char next
                                    '									 * to ']' to 'c' in cases like
                                    '									 * ')name:0.01[100.00]', name (current_name_str)
                                    '									 * is id for internal node 0.01 is branch
                                    '									 * length, 100.00 is bootstrap
                                    '

                                    Dim regExp__1 As RegExp = New RegularExpressions.Regex("(\d+\.?\d*)")
                                    Dim m As MatchResult = regExp__1.Matches(string_between_parenthesis)
                                    If (m IsNot Nothing) Then
                                        bootstrap = Convert.ToSingle(m.Item(0))
                                        hasBootStrap = True
                                    End If
                                    '
                                    'bootstrap = JSFuncs.extractFloatNumberFromString(string_between_parenthesis);
                                    'hasBootStrap = true;

                                    'bootstrap = Float.valueOf(string_between_parenthesis.trim()).floatValue();
                                    'hasBootStrap = true;

                                    '
                                    '									 * set values for bootstrap and id for current
                                    '									 * internal node
                                    '

                                    current_internal_node.ID = current_name_str.Trim()

                                    current_internal_node.BootStrap = bootstrap
                                Catch
                                    bootstrap = -1
                                    '
                                    current_internal_node.ID = string_between_parenthesis.Trim()
                                End Try
                            End If
                        Else

                            ' the following was moved to if( c == ':' );
                            '                        /*
                            '                         * >>>> if NOT d or e >>>>> in ')name:0.01', if name is
                            '                         * numeric, it's bootstrap score, otherwise, it's id of
                            '                         * current internal node
                            '                         */
                            '
                            '                        if (current_name_str.trim().length() > 0) {
                            '                            try {
                            '                                bootstrap = Float.valueOf(current_name_str.trim()).floatValue();
                            '                            } catch (NumberFormatException nfe) {
                            '                                bootstrap = -1;
                            '                            }
                            '                        }
                            '
                            '                        System.out.println("  bootstrap is : " + bootstrap + "; current name str is : " + current_name_str);
                            '
                            '                        if (bootstrap == -1) {
                            '                            current_internal_node.setID(current_name_str.trim());
                            '                        } else {
                            '                            current_internal_node.setBootStrap(bootstrap);
                            '                            hasBootStrap = true;
                            '                        }

                            ' Sep 10, 2013 --
                            ' if branch length does not exist, then the name is
                            If Not isBranchLength And branch_length <> 0 Then
                                current_internal_node.BootStrap = branch_length
                                Me.hasBootStrap = True
                            End If
                        End If
                        ' end of 'if c == '['
                        '
                        '						 * set branch length
                        '						 * Sep 10, 2013; make sure isBranchLength is True --
                        '

                        If branch_length > 0 And isBranchLength Then
                            current_internal_node.BranchLength = branch_length
                        End If

                        '
                        '						 * if current node is internal, move one level up
                        '

                        If current_internal_node.IsRoot = False Then
                            ' change current_internal_node
                            current_internal_node = current_internal_node.Parent
                        End If
                    Else
                        '
                        '						 * in case of 'a' -- the first '(',  make a new leaf node and add it to
                        '						 * 'allNodes'
                        '

                        serial_leaf_node += 1

                        ' Dec 1, 2011; translate leaf name --
                        Dim leaf_name_str As String = current_name_str
                        If hashTranslate IsNot Nothing AndAlso hashTranslate.ContainsKey(current_name_str) Then
                            leaf_name_str = hashTranslate(current_name_str)
                        End If
                        makeNewLeafNode(leaf_name_str, branch_length, current_internal_node, serial_leaf_node)
                    End If
                    ' <<<<< <<<<<<

                    ' reset string
                    current_name_str = ""
                    name_str_backup = ""

                    ' reset values
                    'isBranchLength = false;

                    If c = ";"c Then
                        Me.isTreeEndWithSemiColon = True
                        ' exit?
                        Exit For
                    End If

                    If c = ")"c Then
                        isRightparenthesis = True
                    Else
                        ' important
                        isRightparenthesis = False

                    End If
                Else
                    ' append c to current string
                    current_name_str += c
                End If
                previous_chr = c
            Next
            ' for split tree string to char
        End Sub

        ''' <summary>
        '''     '
        '''		 * Nov 28, 2011; nhx format see here for more details:
        '''		 * http://phylosoft.org/NHX/ please note that using nhx is now discoraged;
        '''		 * use phyloXML instead
        '''		 *
        '''		 * nhx format shares certain similarities with newick, so sode codes were
        '''		 * copied from the newick parser
        '''		 *
        '''		 * a typical nhx tree would look like:
        '''		 *
        '''		 * (((ADH2:0.1[&amp;&amp;NHX:S=human:E=1.1.1.1],
        '''		 * ADH1:0.11[&amp;&amp;NHX:S=human:E=1.1.1.1]):0.05[&amp;&amp;NHX:S=Primates:E=1.1.1.1:D=Y:B=100],
        '''		 * ADHY:0.1[&amp;&amp;NHX:S=nematode:E=1.1.1.1],ADHX:0.12[&amp;&amp;NHX:S=insect:E=1.1.1.1]):0.1[&amp;&amp;NHX:S=Metazoa:E=1.1.1.1:D=N],
        '''		 * (ADH4:0.09[&amp;&amp;NHX:S=yeast:E=1.1.1.1],ADH3:0.13[&amp;&amp;NHX:S=yeast:E=1.1.1.1],
        '''		 * ADH2:0.12[&amp;&amp;NHX:S=yeast:E=1.1.1.1],
        '''		 * ADH1:0.11[&amp;&amp;NHX:S=yeast:E=1.1.1.1]):0.1
        '''		 * [&amp;&amp;NHX:S=Fungi])[&amp;&amp;NHX:E=1.1.1.1:D=N];
        '''
        ''' </summary>
        ''' <param name="treestr"></param>
        ''' <remarks></remarks>
        Private Sub nhxParser(treestr As String)

            Dim name_str_backup As String = "", current_name_str As String = ""
            Dim current_internal_node As PhyloNode = _RootNode
            ' currrent internal node; set to rootnode
            Dim previous_chr As Char = " "c

            Dim isBranchLength As Boolean = False, isFirstLeftParenthesis As Boolean = True, isRightparenthesis As Boolean = False

            For idx As Integer = 0 To treestr.Length - 1
                Dim c As Char = treestr(idx)
                If c = "("c Then
                    '
                    '					 * possibilities: a. c is NOT the first '(', create a new
                    '					 * internal node b. c is immediately after a ',', create a new
                    '					 * internal node c. c is the first 'C', do nothing
                    '

                    If isFirstLeftParenthesis = False Then
                        ' a or b
                        '
                        '						 * if not the first '(', the previous letter MUST BE ',' or
                        '						 * '('
                        '

                        If previous_chr <> "("c AndAlso previous_chr <> ","c Then
                            Me._ErrorMessage = " pos:" & (idx - 1) & " '(' or ',' is expected, got '" & previous_chr & "'"
                            isValidDataset = False
                        Else
                            '
                            '							 * make a new internal node
                            '

                            serial_internal_node += 1
                            current_internal_node = MakeNewInternalNode("", "INT" & serial_internal_node, False, current_internal_node)
                        End If
                    Else
                        ' if true, set it to false
                        isFirstLeftParenthesis = False
                    End If

                    '
                    '					 * reset variables
                    '

                    isRightparenthesis = False

                    current_name_str = ""
                    name_str_backup = ""
                ElseIf c = ":"c Then
                    '
                    '					 * ':' indicates current tree has branch length
                    '

                    isBranchLength = True

                    '
                    '					 * back up current name string and reset it
                    '

                    name_str_backup = current_name_str

                    current_name_str = ""
                ElseIf c = ","c OrElse c = ";"c OrElse c = ")"c OrElse c = "["c Then
                    '
                    '					 * get branch length from current name string
                    '

                    Dim branch_length As Single = 0

                    If isBranchLength = True Then
                        Try
                            branch_length = CSng(Convert.ToSingle(current_name_str.Trim()))
                        Catch
                            branch_length = 0
                        End Try

                        '
                        '						 * copy the value from backup
                        '

                        current_name_str = name_str_backup
                    End If

                    '
                    '					 * >>>>> possibilities: >>>>> a. ,name:0.01, or ,name:0.01)
                    '					 * create a new leaf node b. )name:0.01, or )name:0.01) save ID
                    '					 * to current internal node and set parent as current internal
                    '					 * node c. )name:0.01; save ID to internal node and end for loop
                    '


                    ' in case of a: name is a leaf node; make a new leaf node and add it to 'allNodes'
                    If Not isRightparenthesis Then
                        serial_leaf_node += 1
                        makeNewLeafNode(If(isBranchLength, name_str_backup, current_name_str), branch_length, current_internal_node, serial_leaf_node)
                    Else
                        ' in cases of b:
                        '
                        '						 * set branch length
                        '

                        current_internal_node.ID = current_name_str.Trim()

                        If branch_length > 0 Then
                            current_internal_node.BranchLength = branch_length
                        End If

                        '
                        '						 * if current node is internal, move one level up
                        '

                        If current_internal_node.IsRoot = False Then
                            ' change current_internal_node
                            current_internal_node = current_internal_node.Parent
                        End If
                    End If

                    '
                    If c = "["c Then
                        '
                        '						 * nhx style extra information -- just add the information
                        '						 * to the last node, (leaf or internal)
                        '

                        Dim first_colon As Boolean = False
                        Dim pos_first_colon As Integer = 0
                        Dim nhx_string As New StringBuilder()

                        While True
                            idx += 1
                            Dim ch As Char = treestr(idx)
                            If ch = ":"c AndAlso Not first_colon Then
                                first_colon = True
                                pos_first_colon = idx
                            End If

                            If ch = "]"c Then
                                idx += 1
                                c = treestr(idx)
                                ' get the char next to ']' and assign it to c
                                ' exit while loop
                                Exit While
                            End If

                            If first_colon AndAlso idx > pos_first_colon Then
                                nhx_string.Append(treestr(idx))
                            End If
                        End While

                        ' -- now parse nhx string --
                        Dim nhx As String = nhx_string.ToString()
                        If Not nhx.Length = 0 Then
                            Dim node As PhyloNode = Me._AllNodes(Me._AllNodes.Count - 1)
                            Dim nhxAttributes As New Dictionary(Of String, String)()
                            For Each keyvaluepair As String In Strings.Split(nhx, ":")
                                If InStr(keyvaluepair, "=") > 0 Then
                                    Dim keyvalue As List(Of String) = Strings.Split(keyvaluepair, "=").AsList

                                    Dim k As String = keyvalue(0)
                                    Dim v As String = keyvalue(1)

                                    ' set bootstrap to current node
                                    If k.ToUpper() = "B".ToUpper() Then
                                        Me.hasBootStrap = True
                                        Dim bootstrap As Single = 0
                                        Try
                                            bootstrap = CSng(Convert.ToSingle(current_name_str.Trim()))
                                        Catch
                                            bootstrap = 0
                                        End Try

                                        node.BootStrap = bootstrap
                                    Else
                                        nhxAttributes.Add(k, v)
                                    End If
                                End If
                            Next
                            node.AdditionalAttributs = nhxAttributes
                        End If
                        ' if not empty
                        ' in cases like: )[&&NHX:S=Eukaryota:D=N:B=0];
                        If c = ";"c Then
                            Me.isTreeEndWithSemiColon = True
                            ' exit?
                            Exit For
                        End If
                    End If
                    ' if c== '['
                    ' reset string
                    current_name_str = ""
                    name_str_backup = ""

                    ' reset values
                    'isBranchLength = false;

                    If c = ";"c Then
                        Me.isTreeEndWithSemiColon = True
                        ' exit?
                        Exit For
                    End If

                    If c = ")"c Then
                        isRightparenthesis = True
                    Else
                        ' important
                        isRightparenthesis = False
                    End If
                ElseIf c <> " "c Then
                    ' append c to current string
                    current_name_str += c
                End If
                ' Nov 07, 2011; blank letters are now allowed
                'if (c != ' ') {
                ' keep track of previous char; blank letters are omitted
                '}
                previous_chr = c
            Next
            ' for split tree string to char
        End Sub

        ''' <summary>
        '''
        '''		 * Dec 1-2, 2011; nexus format; note only the tree part will be processed;
        '''		 * other data will be ignored see :
        '''		 * http://molecularevolution.org/resources/treeformats for more details
        '''		 *
        '''		 * a typical nexsus tree looks like: #nexus ... begin trees; translate 1
        '''		 * Ephedra, 2 Gnetum, 3 Welwitschia, 4 Ginkgo, 5 Pinus ; tree one = [&amp;U]
        '''		 * (1,2,(3,(4,5)); tree two = [&amp;U] (1,3,(5,(2,4)); end;
        '''
        ''' </summary>
        ''' <param name="treestr"></param>
        ''' <remarks></remarks>
        Private Sub NexusParser(treestr As String)
            Dim hashTranslate As New Dictionary(Of String, String)()
            Dim bTranslate As Boolean = False
            Dim bBeginTrees As Boolean = False

            'System.out.println("======== parsing nexus format =============\nyour input is : " + treestr);

            Dim line_num As Integer = 0
            For Each line As String In Strings.Split(treestr, vbLf)

                line = line.Trim()
                line_num += 1
                'System.out.println(line_num + ": " + line);

                If line_num = 1 AndAlso Not line.ToUpper() = "#NEXUS".ToUpper() Then
                    Me._ErrorMessage = "input file doesn't start with #NEXUS!!"
                    isValidDataset = False
                    Exit For
                ElseIf InStr(line.ToLower(), "Begin trees".ToLower()) > 0 Then
                    bBeginTrees = True
                ElseIf InStr(line.ToLower(), "Begin".ToLower()) > 0 AndAlso bBeginTrees Then
                    bBeginTrees = False
                ElseIf line.ToUpper() = "Translate".ToUpper() Then
                    bTranslate = True
                ElseIf line.Equals(";") Then
                    bTranslate = False
                ElseIf bBeginTrees AndAlso bTranslate Then
                    ' split the translate part:
                    Dim parts As List(Of String) = line.Split.AsList
                    If parts.Count >= 2 Then
                        Dim trans_str As String = ""
                        For i As Integer = 1 To parts.Count - 1
                            If i > 1 Then
                                trans_str += " "
                            End If
                            trans_str += parts(i).Replace("'", "").Replace(",", "")
                        Next
                        hashTranslate.Add(parts(0), trans_str)
                    End If
                ElseIf bBeginTrees AndAlso Not bTranslate AndAlso InStr(line.ToLower(), "tree".ToLower()) > 0 Then
                    Dim substring_treestr As String = line.Substring(line.IndexOf("("))

                    ' first of all, check if the tree is valid
                    Dim iBrackets As Integer = 0
                    For Each c As Char In substring_treestr.Trim().ToCharArray()
                        If c = "("c Then
                            iBrackets += 1
                        ElseIf c = ")"c Then
                            iBrackets -= 1
                        End If
                    Next

                    If iBrackets <> 0 Then
                        Me.isValidDataset = False
                        Me._ErrorMessage = "the numbers of '(' and ')' do not match, please check your tree!!"
                    Else
                        newickParser(substring_treestr, hashTranslate, Me._RootNode)
                    End If
                    ' only the first tree will be taken
                    Exit For
                End If
            Next
            ' for each line
        End Sub
        ' nexusParser
        '
        '		 * Dec 2, 2011 -- see
        '		 * http://code.google.com/p/google-web-toolkit-doc-1-5/wiki/DevGuideXML for
        '		 * details about how GWT deal with XML string
        '

        'Private Sub phyloXMLParser(treestrXML As String)
        '	Try
        '		Dim treeDom As Document = XMLParser.parse(treestrXML)
        '		If treeDom.getElementsByTagName("phylogeny").length > 0 Then
        '			Me.isTreeEndWithSemiColon = True
        '			'?????
        '			Dim phylogeny As Node = treeDom.getElementsByTagName("phylogeny").item(0)

        '			Dim childnodes As NodeList = phylogeny.childNodes
        '			If phylogeny.attributes.getNamedItem("rooted") IsNot Nothing AndAlso phylogeny.attributes.getNamedItem("rooted").nodeValue.equalsIgnoreCase("true") Then
        '				Console.WriteLine(" =================== rooted tree =================== ")
        '				For i As Integer = 0 To childnodes.length - 1
        '					Dim cnode As Node = childnodes.item(i)
        '					If cnode.nodeName.equalsIgnoreCase("clade") Then
        '						childnodes = cnode.childNodes
        '						Exit For
        '					End If
        '				Next
        '			End If

        '			phyloXMLparserNodesIterator(rootNode_Renamed, childnodes)
        '		Else
        '			Me.m_errormessage = "no 'phylogeny' tag found in your xml file, check your input"
        '			Me.isValidDataset = False
        '		End If
        '          Catch e As Exception
        '              Me.m_errormessage = e.Message
        '              Me.isValidDataset = False
        '	End Try
        'End Sub

        ' Dec 2, 2011
        Private Sub phyloXMLparserNodesIterator(parentnode As PhyloNode, nodes As Xml.XmlNodeList)
            '
            For i As Integer = 0 To nodes.Count - 1
                Dim node = nodes.Item(i)
                If String.Equals(node.Name, "clade", StringComparison.OrdinalIgnoreCase) Then
                    Dim currentNode As PhyloNode = Nothing

                    ' check leaf node or not
                    If DirectCast(node, Element).GetElementsByTagName("clade").Count > 0 Then
                        ' make a new internal node
                        serial_internal_node += 1

                        ' get id for internal node
                        Dim iid As String = getAttributeValueByItemID(node, "name")
                        currentNode = MakeNewInternalNode(iid, "INT" & serial_internal_node, False, parentnode)

                        ' get confidance / bootstrap
                        Dim bootstrap As Single = getNumericValueForNamedItemFromAnXMLNode(node, "confidence")
                        If bootstrap > -1 Then
                            Me.hasBootStrap = True
                            currentNode.BootStrap = bootstrap
                        End If

                        ' at the end, call this function itself --
                        phyloXMLparserNodesIterator(currentNode, node.ChildNodes)
                    Else
                        ' make a new leaf node
                        serial_leaf_node += 1
                        currentNode = makeNewLeafNode(DirectCast(node, Element).GetElementsByTagName("name").Item(0).FirstChild.Value, 0, parentnode, serial_leaf_node)
                    End If

                    '
                    '					 * get branch length, if possible case a: branch lenth as inline
                    '					 * attributes
                    '					 *
                    '					 * <clade branch_length="0.06">
                    '					 *
                    '					 * case b: branch length as child nodes
                    '					 *
                    '					 * <clade> <branch_length>0.07466</branch_length> <confidence
                    '					 * type="unknown">32.0</confidence> ... <clade>
                    '					 *
                    '					 * first of all, deal with case a:
                    '					 *
                    '					 * == last updated: Jan 09, 2012 ==
                    '

                    Dim branchlength As Single = getNumericValueForNamedItemFromAnXMLNode(node, "branch_length")
                    If branchlength > -1 Then
                        Me.HasBranchLength = True
                        currentNode.branchLength = branchlength
                        ' April 20, 2012
                    End If
                    ' if nodename is 'clade'
                End If
            Next
        End Sub

        Private Function getAttributeValueByItemID(node As Node, itemid As String) As String
            Dim value As String = ""

            ' get confidence; Jan 09, 2012
            If node.Attributes.GetNamedItem(itemid) IsNot Nothing Then
                value = node.Attributes.GetNamedItem(itemid).Value
            Else
                Dim dnodes = node.ChildNodes
                For j As Integer = 0 To dnodes.Count - 1
                    Dim dnode As Node = dnodes.Item(j)
                    If String.Equals(dnode.Name, itemid, StringComparison.OrdinalIgnoreCase) Then
                        value = dnode.FirstChild.ToString()
                    End If
                    ' then case b
                Next
            End If
            ' Jan 09, 2012
            Return (value)
        End Function

        ' -- April 20, 2012 --
        Private Function getNumericValueForNamedItemFromAnXMLNode(node As Xml.XmlNode, itemName As String) As Single
            Dim numericvalue As Single = -1
            If node.Attributes.GetNamedItem(itemName) IsNot Nothing Then
                Try

                    numericvalue = Convert.ToSingle(node.Attributes.GetNamedItem(itemName).Value)
                Catch
                End Try
            Else
                Dim dnodes As NodeList = node.ChildNodes
                For j As Integer = 0 To dnodes.Count - 1
                    Dim dnode As Node = dnodes.Item(j)
                    If String.Equals(dnode.Name, itemName, StringComparison.OrdinalIgnoreCase) Then
                        Try
                            numericvalue = Convert.ToSingle(dnode.FirstChild.ToString())
                        Catch
                        End Try
                    End If
                    ' then case b
                Next
            End If
            ' Jan 09, 2012
            Return numericvalue
        End Function
        ' April 20, 2012
        ' May 13, 2012
        Public Function getExternalIDbyInternalID(internal_id As String) As String
            Return If(Me._hsInternalID2ExternalID.ContainsKey(internal_id), Me._hsInternalID2ExternalID(internal_id), "")
        End Function

        ''' <returns> the treeFormat </returns>
        Public ReadOnly Property treeFormat() As String

    End Class
End Namespace
