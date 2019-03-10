#Region "Microsoft.VisualBasic::915f96fab2583b025186972bba28f068, visualize\Phylip\Evolview\PhyloNode.vb"

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

    '     Class PhyloNode
    ' 
    '         Properties: AdditionalAttributs, BootStrap, BranchLength, BranchLengthToRoot, Descendents
    '                     Description, DistanceToRoot, ID, InternalID, IsFork
    '                     IsLeaf, IsRoot, LevelHorizontal, LevelVertical, LevelVerticalSlanted
    '                     MaxDistanceToTip, maxLeafVerticalLevel, minLeafVerticalLevel, NodeComplete, NumberOfDescendents
    '                     NumberOfLeafDescendents, Parent, Processed
    ' 
    '         Function: Clone, getBranchColorByColorsetID, getColorByDatasetID, getColorByDataTypeAndID, getLeafBKColorByColorsetID
    '                   getLeafColorByColorsetID, InternalCountLeafDescendents, RemoveFromParent, SetProcessed, ToString
    ' 
    '         Sub: addColorByDataTypeAndID, AddColorToBranch, addColorToLeaf, addColorToLeafBK, addColorWithID
    '              AddDescendent, removeAllBranchColors, RemoveAllDescendents, removeAllLeafBKColors, removeAllLeafColors
    '              removeBranchColorByColorsetID, removeColorByDataTypeAndID, RemoveDescendent, removeLeafBKColorByColorsetID, removeLeafColorByColorsetID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Collections.Generic

Namespace Evolview

    Public Class PhyloNode

#Region "* variables I need to describe a tree node"

        ''' <summary>
        ''' multiple (>= two) descendents are allowed
        ''' </summary>
        ''' <remarks></remarks>
        Private __descendentsList As List(Of PhyloNode) = New List(Of PhyloNode)()

        ''' <summary>
        ''' total branch length from current node to root -  max_distance_to_tip (height) is the max branchlength to get to the tip;
        ''' </summary>
        ''' <remarks></remarks>
        Private Max_Distance_To_Tip As Single = -1, Branchlength_To_Root As Single = -1

        ''' <summary>
        ''' May 20, 2011; check if current node is processed; used to fix a bug
        ''' </summary>
        ''' <remarks></remarks>
        Private _Processed As Boolean = False
        Private _BranchColors As New Dictionary(Of String, String)(), _LeafColors As New Dictionary(Of String, String)(), _LeafBackgroundColors As New Dictionary(Of String, String)(), _Colors As New Dictionary(Of String, String)()

        '
        '		 * jan 7, 2011; I need this to plot SLANTED_CLADOGRAM_MIDDLE;
        '		 *   in this mode, the position of internal node
        '		 *       = min_leaf_vertical_level + (max_leaf_vertical_level - min_leaf_vertical_level) / 2
        '		 *   while the leaf node has the same with level_vertical
        '

        Private Level_Vertical_Slanted As Single = 0
        '
        '		 * jan 7, 2011; plot SLANTED_CLADOGRAM_NORMAL; similar to dendroscope
        '		 * in this mode, for each internal node, I need :
        '		     min_leaf_vertical_level and max_leaf_vertical_leve
        '

        Private Min_Leaf_Vertical_Level As Single = 0, Max_Leaf_Vertical_Level As Single = 0
#End Region

        Public Overrides Function ToString() As String
            If String.IsNullOrEmpty(ID) Then
                Return If(IsRoot, "(ROOT) ", "") & InternalID & "   " & BranchLength
            Else
                Return If(IsRoot, "(ROOT) ", "") & ID & "   " & BranchLength
            End If
        End Function

        ''' <summary>
        ''' ˮƽ������������λ��
        '''
        '''  ' April 8, 2011; NOTE: use this instead of branchcolors, leafcolors and leafbackground colors ...
        '''
        '''		 * level_vertical is the relative vertical level from the top, start with 1;
        '''		 *   for example, given a tree like the following:
        '''		 *     |----- A 1
        '''		 *  ---| C    |--- E 2
        '''		 *     |------| B 2.5      a sample tree
        '''		 *     	      |----F 3
        '''		 *
        '''		 *   A is level 1, E is level 2, F is 3, B is 2.5 (2+3)/2; C is (2.5 + 1) / 2 = 1.725
        '''		 *
        '''		 * level_horizontal is the relative horizontal level from the right, start with 1; therefore all leaves are at level 1
        '''		 *   in this sample tree, AEF are at level 1, B is 2, C is 3
        '''		 *
        '''		 * here I use two integers to store the two levels
        '''		 *
        '''		 * >>>> how levels are assigned >>>>
        '''		 *   vertical levels: in a tree like ((A,B), (C,D)), levels for ABCD are 1,2,3,4 respectively; internal levels are assigned using a function: ""
        '''		 *   horizontal levels: all leaf are assigned with 1, internal levels are assigned using a function: ""
        '''		 *
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LevelHorizontal As Single

        ''' <summary>
        ''' ��ֱ������������λ��
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LevelVertical As Single

        ''' <summary>
        ''' jan 7, 2011; if current node is leaf, level_vertical_slanted = level_vertical
        ''' so there is not necessary to assign level_vertical_slanted to leaf node;
        ''' for slanted middle mode;
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property LevelVerticalSlanted() As Single
            Get
                Return If(Me.IsLeaf, LevelVertical, Level_Vertical_Slanted)
            End Get
            Set(value As Single)
                Level_Vertical_Slanted = value
            End Set
        End Property

        ''' <summary>
        ''' jan 7, 2011; if leaf node, min_leaf_vertical_level and max_leaf_vertical_level = level_vertical
        ''' for slanted normal mode
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property minLeafVerticalLevel() As Single
            Get
                Return If(Me.IsLeaf, LevelVertical, Min_Leaf_Vertical_Level)
            End Get
            Set(value As Single)
                Min_Leaf_Vertical_Level = value
            End Set
        End Property

        Public Property maxLeafVerticalLevel() As Single
            Get
                Return If(Me.IsLeaf, LevelVertical, Max_Leaf_Vertical_Level)
            End Get
            Set(value As Single)
                Max_Leaf_Vertical_Level = value
            End Set
        End Property

        ''' <summary>
        ''' id is actually a lable that will be printed
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ID As String
        Public Property InternalID As String
        Public Property Description As String
        Public Property BranchLength As Single
        Public Property BootStrap As Single
        Public Property Parent As PhyloNode

        Public ReadOnly Property IsFork As Boolean
            Get
                Return Not IsRoot AndAlso Not IsLeaf AndAlso String.IsNullOrEmpty(ID)
            End Get
        End Property

        ''' <summary>
        ''' The descendents of current node.(��ǰ�ڵ�ĺ���ڵ�)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Descendents As List(Of PhyloNode)
            Get
                Return __descendentsList
            End Get
            Set(value As List(Of PhyloNode))
                __descendentsList = value
            End Set
        End Property

        Public Sub AddDescendent(NewDescendent As PhyloNode)
            If __descendentsList.Contains(NewDescendent) = False Then
                Call __descendentsList.Add(NewDescendent)
                ' also change the parent automatically
                NewDescendent.Parent = Me
            End If
        End Sub

        Public Sub RemoveDescendent(descendent As PhyloNode)
            If __descendentsList.Contains(descendent) = True Then
                Dim idx As Integer = __descendentsList.IndexOf(descendent)
                __descendentsList.RemoveAt(idx)
            End If
        End Sub

        ' -- remove from parent; oct 26, 2013 --
        Public Function RemoveFromParent() As PhyloNode
            Dim Parent As PhyloNode = Me.Parent

            If Parent IsNot Nothing Then
                Parent.RemoveDescendent(Me)
            End If

            Return Parent
        End Function

        Public ReadOnly Property NumberOfDescendents() As Integer
            Get
                Return __descendentsList.Count
            End Get
        End Property

        Public ReadOnly Property NumberOfLeafDescendents() As Integer
            Get
                Dim NumOfLeafDescendents As Integer = 0

                For Each Node As PhyloNode In Me.__descendentsList
                    If Node.IsLeaf Then
                        NumOfLeafDescendents += 1
                    Else
                        NumOfLeafDescendents += InternalCountLeafDescendents(Node)
                    End If
                Next

                Return NumOfLeafDescendents
            End Get
        End Property

        Private Function InternalCountLeafDescendents(Node As PhyloNode) As Integer
            Dim Count As Integer = 0

            For Each DNode As PhyloNode In Node.Descendents
                If DNode.IsLeaf Then
                    Count += 1
                Else
                    Count += InternalCountLeafDescendents(DNode)
                End If
            Next

            Return Count
        End Function

        Public Property IsLeaf As Boolean
        Public Property IsRoot As Boolean

        Public Property MaxDistanceToTip As Single
            Get
                Return Max_Distance_To_Tip
            End Get
            Set(value As Single)
                Max_Distance_To_Tip = value
            End Set
        End Property


        Public Property BranchLengthToRoot As Single
            Get
                Return Branchlength_To_Root
            End Get
            Set(value As Single)
                Branchlength_To_Root = value
            End Set
        End Property

        Public ReadOnly Property NodeComplete As Boolean
            Get
                Dim IsComplete As Boolean = True

                If BranchLength <= 0 OrElse _ID.Trim.Length = 0 OrElse (_Parent Is Nothing AndAlso __descendentsList.Count = 0) Then
                    IsComplete = False
                End If

                Return IsComplete
            End Get
        End Property

        ''' <summary>
        ''' make a new copy of current node
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Clone() As PhyloNode
            Dim NewNode As New PhyloNode()

            NewNode.BootStrap = BootStrap
            NewNode.BranchLength = BranchLength
            NewNode.Descendents = Descendents
            ' this isn't clone yet
            NewNode.Description = Description
            NewNode.ID = ID
            NewNode.InternalID = InternalID
            NewNode.Parent = _Parent
            ' this isn't clone yet
            Return NewNode
        End Function

        ''' <summary>
        ''' * branch colors
        ''' </summary>
        ''' <param name="colorsetID"></param>
        ''' <param name="color"></param>
        ''' <remarks></remarks>
        Public Sub AddColorToBranch(colorsetID As String, color As String)
            Call Me._BranchColors.Add(colorsetID, color)
        End Sub

        Public Function getBranchColorByColorsetID(colorsetID As String) As String
            Return If(Me._BranchColors.ContainsKey(colorsetID), Me._BranchColors(colorsetID), "black")
        End Function

        Public Sub removeBranchColorByColorsetID(colorsetID As String)
            Me._BranchColors.Remove(colorsetID)
        End Sub

        Public Sub removeAllBranchColors()
            Me._BranchColors.Clear()
        End Sub

        ''' <summary>
        ''' * leaf colors
        ''' </summary>
        ''' <param name="colorsetID"></param>
        ''' <param name="color"></param>
        ''' <remarks></remarks>
        Public Sub addColorToLeaf(colorsetID As String, color As String)
            Call Me._LeafColors.Add(colorsetID, color)
        End Sub

        Public Function getLeafColorByColorsetID(colorsetID As String) As String
            Return If(Me._LeafColors.ContainsKey(colorsetID), Me._LeafColors(colorsetID), "black")
        End Function

        Public Sub removeLeafColorByColorsetID(colorsetID As String)
            Call Me._LeafColors.Remove(colorsetID)
        End Sub

        Public Sub removeAllLeafColors()
            Me._LeafColors.Clear()
        End Sub

        '
        '		 * leaf background colors
        '

        Public Sub addColorToLeafBK(colorsetID As String, color As String)
            Me._LeafBackgroundColors.Add(colorsetID, color)
        End Sub

        Public Function getLeafBKColorByColorsetID(colorsetID As String) As String
            Return If(Me._LeafBackgroundColors.ContainsKey(colorsetID), Me._LeafBackgroundColors(colorsetID), "white")
            ' March 21, 2011; change default color from 'black' to 'white'
        End Function

        Public Sub removeLeafBKColorByColorsetID(colorsetID As String)
            Me._LeafBackgroundColors.Remove(colorsetID)
        End Sub

        Public Sub removeAllLeafBKColors()
            Me._LeafBackgroundColors.Clear()
        End Sub

        Public Function getColorByDatasetID(id As String) As String
            Return If(Me._Colors.ContainsKey(id), Me._Colors(id), "")
        End Function

        Public Sub addColorWithID(datasetID As String, color As String)
            Me._Colors.Add(datasetID, color)
        End Sub
        ' April 8, 2011
        Public Function getColorByDataTypeAndID(type As String, id As String) As String
            Dim col As String = ""
            If type.ToUpper() = "branch".ToUpper() Then
                col = Me.getBranchColorByColorsetID(id)
            ElseIf type.ToUpper() = "leaf".ToUpper() Then
                col = Me.getLeafColorByColorsetID(id)
            ElseIf type.ToUpper() = "leafbk".ToUpper() Then
                col = Me.getLeafBKColorByColorsetID(id)
            End If

            Return col
        End Function
        ' April 8, 2011;
        Public Sub addColorByDataTypeAndID(type As TreeDecoType, id As String, col As String)
            If type.Equals(TreeDecoType.BRANCHCOLOR) Then
                Me.AddColorToBranch(id, col)
            ElseIf type.Equals(TreeDecoType.LEAFCOLOR) Then
                Me.addColorToLeaf(id, col)
            ElseIf type.Equals(TreeDecoType.LEAFBKCOLOR) Then
                Me.addColorToLeafBK(id, col)
            End If
        End Sub
        ' April 8, 2011;
        Public Sub removeColorByDataTypeAndID(type As TreeDecoType, id As String)
            If type.Equals(TreeDecoType.BRANCHCOLOR) Then
                Me.removeBranchColorByColorsetID(id)
            ElseIf type.Equals(TreeDecoType.LEAFCOLOR) Then
                Me.removeLeafColorByColorsetID(id)
            ElseIf type.Equals(TreeDecoType.LEAFBKCOLOR) Then
                Me.removeLeafBKColorByColorsetID(id)
            End If
        End Sub

        Public Function SetProcessed(b As Boolean) As Boolean
            Me._Processed = b
            Return Me._Processed
        End Function

        Public ReadOnly Property Processed As Boolean
            Get
                Return Me._Processed
            End Get
        End Property

        Public Property AdditionalAttributs() As Dictionary(Of String, String)

        Public ReadOnly Property DistanceToRoot() As Integer
            Get
                Dim distance_to_root As Integer = 0
                If Not Me.IsRoot Then
                    distance_to_root += 1
                    Dim node As PhyloNode = Me.Parent
                    While node IsNot Nothing AndAlso Not node.IsRoot
                        distance_to_root += 1
                        node = node.Parent
                    End While
                End If
                Return distance_to_root
            End Get
        End Property

        Public Sub RemoveAllDescendents()
            Call Me.__descendentsList.Clear()
        End Sub
    End Class
End Namespace
