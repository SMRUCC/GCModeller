#Region "Microsoft.VisualBasic::04a461f8993eed6048b8af5b7d30969e, visualize\Phylip\Evolview\TreeSkeleton.vb"

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

    ' 
    ' /********************************************************************************/

#End Region

'Imports OMSVGPoint = System.Drawing.Point
'Imports System.Drawing

'Namespace Evolview

'    Public Class APosition

'        Private m_xy As OMSVGPoint
'        Private angle_Renamed As Single = 0.0F

'        ' constructors
'        Public Sub New()
'        End Sub

'        Public Sub New(newxy As OMSVGPoint, newangle As Single)
'            m_xy = newxy
'            angle_Renamed = newangle
'        End Sub

'        '
'        '			 * get-set pairs
'        '			 

'        Public Overridable Property xY() As OMSVGPoint
'            Get
'                Return m_xy
'            End Get
'            Set(value As OMSVGPoint)
'                m_xy = value
'            End Set
'        End Property


'        Public Overridable Property angle() As Single
'            Get
'                Return angle_Renamed
'            End Get
'            Set(value As Single)
'                angle_Renamed = value
'            End Set
'        End Property


'        Public Overridable Property x() As Single
'            Get
'                Return m_xy.X
'            End Get
'            Set(value As Single)
'                m_xy.X = value
'            End Set
'        End Property

'        Public Overridable Property y() As Single
'            Get
'                Return m_xy.Y
'            End Get
'            Set(value As Single)
'                m_xy.X = value
'            End Set
'        End Property

'        Public Overrides Function ToString() As String
'            Return angle & "   " & m_xy.ToString
'        End Function
'    End Class


'    '
'    '		 * April 8, 2011; clean up; done March 17, 2011; make tree skeleton a
'    '		 * seperate class
'    '		 *
'    '		 * this class takes care of tree skeleton + leaf label, colors of branch,
'    '		 * leaf and background of leaf it'll also update positions for plotting leaf
'    '		 * labels, branch lengths and bootstrap scores
'    '		 *
'    '		 

'    Public Class TreeSkeleton
'        '
'        '			 * branch / leaf label / leaf backgroud / colors
'        '			 


'        Private leafColorEnabled As Boolean = True, leafBKColorEnabled As Boolean = True, branchColorEnabled As Boolean = True, alignLeafLabels_Renamed As Boolean = False
'        ' Align leaf label
'        Private activeLeafColorSetID As String = "", activeLeafBKColorSetID As String = "", activeBranchColorSetID As String = ""
'        '
'        '			 * April 4, 2011; this hash will replace colorsetIDs NOTE: here
'        '			 * datasetID should be type + actualID, for example, a branch color
'        '			 * dataset with name 'set1' should be writen as BRANCHCOLORset1 this way
'        '			 

'        Private dataset2originalUserinputAsString As New Dictionary(Of String, String)()
'        Private dataset2opacity As New Dictionary(Of String, System.Nullable(Of Single))()
'        ' // April 8, 2011; dataset opacity
'        '
'        '			 * use this to store SVG elements such as lines/ texts/ and paths to
'        '			 * modified them later NOTE: all IDs are internal ID
'        '			 
'        '   Private leafID2OMNode As New Dictionary(Of String, OMNode)(), branchID2OMNode As New Dictionary(Of String, OMNode)(), bootstrapID2OMNode As New Dictionary(Of String, OMNode)(), leafID2LeafAlignOMNode As New Dictionary(Of String, OMNode)()
'        Dim rootXY As OMSVGPoint
'        Dim rootnode As PhyloNode
'        Dim m_phylotree As PhyloTree
'        Dim angle_start As Single
'        Dim pxPerHeight As Single
'        Dim pxPerWidthCurrentMode As Single

'        '
'        '        '		 * NOTE: here the 'extends' part could be SimplePanel, VP, HP and any other
'        '        '		 * panels derived from SimplePanel; jan 7, 2011
'        '        '		 


'        Private m_sessionid As String = ""
'        Private Shared demoproject As String = "DEMOS"
'        ' >>>>>>>>>>>>>> global variables >>>>>>>>>>>>>>>>
'        '
'        '		 * tree plot modes; phylogram/ cladogram; cladogram styles don't show branch
'        '		 * length Jan 9, 2011; currently RADIAL_CLADOGRAM mode not implemented
'        '		 

'        Private plotmode As TreePlotMode = TreePlotMode.RECT_CLADOGRAM
'        '
'        '		 * viewing options in the toolbar
'        '		 

'        Private showBrachLength As Boolean = False, showNodeLabel As Boolean = True, showBootStrap As Boolean = False, draw_branch_length_scale As Boolean = False, circular_tree_plot_in_clockwise_mode As Boolean = True, disable_all_dataset As Boolean = False, _
'            chart_plots_enabled As Boolean = True, leaf_font_italic As Boolean = False, bMouseWheelEnabled As Boolean = False, bootStrapFontItalic_Renamed As Boolean = False, branchLengthFontItalic_Renamed As Boolean = False
'        '
'        '		 * March 25, 2011; a global parameter to indicate whether all
'        '		 * datasets are disabled:
'        '		 
'        '
'        '		 * some global parameters for tree plot
'        '		 

'        Private pxPerWidthDefault As Single = 20.0F
'        ' -- April 23, 2012 --
'        Private pxPerHeight_Renamed As Single = 20.0F, pxPerBranchLengthScaling As Single = 5, pxPerBranchLength As Single = pxPerWidthDefault * pxPerBranchLengthScaling, margin_x As Single = 50.0F, margin_y As Single = 50.0F, _
'            default_line_width As Single = 1.0F, angle_per_vertical_level As Single = 0, angle_span As Single = 350.0F, scale_tree_branch_linewidth As Single = 1.5F
'        ' Nov 07, 2011; -  Oct 27, 2011: this should be float, instead of int
'        ' min and max allowed px perwidth
'        '			pxPerWidthMinAllowed = 0, pxPerWidthMaxAllowed = pxPerWidthDefault * 20,
'        ' min and max allowed px per height
'        '			pxPerHeightMinAllowed = 0, pxPerHeightMaxAllowed = pxPerHeight * 20, defaultPxPerWidth = 20, defaultPxPerHeight = 20,
'        ' Margins ...
'        '			margin_normal = 0.15f, margin_with_text = 0.2f,
'        ' default line width 
'        ' for circular  mode
'        ''' <summary>
'        ''' margins; May 5, 2013
'        ''' </summary>
'        Private ReadOnly margin_top As Integer = 10, margin_left As Integer = 10

'        '
'        '		 * ===== April 23, 2012 ===== -- improve 1: pxPerWidth changes according to
'        '		 * plot mode -- howto: by default, the default values for each mode will be
'        '		 * used; however if user changes the pxPerWidth for a plotmode, that value
'        '		 * will be always used (for the specific plot mode) and saved to server
'        '		 

'        Private ReadOnly sPxPerWidth As String = "horizScale4PlotMode"

'        ' -- improve 2: independent tranlatedXY for each plot mode --
'        Private ReadOnly sTranslateX As String = "translateX4PlotMode"
'        Private ReadOnly sTranslateY As String = "translateY4PlotMode"

'        ' <<<<< April 23, 2012 <<<<<
'        ' for CIRCULAR mode;
'        Private max_angle_span As Integer = 360, min_angle_span As Integer = 20
'        '
'        '		 * a variable to keep record of the most right position
'        '		 *
'        '		 * plotting order of Legends for subpanels tree skeleton / tree label / tree
'        '		 * label background piechart / other charts
'        '		 

'        Private currentMostRightPosition As Single = 0.0F
'        ' current most right position
'        '
'        '		 * canvas size/ svg canvas size
'        '		 
'        Private canvas_width As Integer = 800, canvas_height As Integer = 600
'        '
'        '		 * font sizes ...
'        '		 

'        Private default_font_size As Integer = 14, default_bootstrap_font_size As Integer = 10, default_branchlength_font_size As Integer = 10, default_tick_label_font_size As Integer = 8
'        Private ReadOnly default_font_family As String = "Arial"

'        '
'        '		 * some parameters for plots of datasets
'        '		 

'        Private ReadOnly space_between_datasets As Single = 10, space_between_columns_within_a_dataset As Single = 3, space_between_treeskeleton_and_leaflabels As Single = 5
'        Private treeID_Renamed As String = "", projectID_Renamed As String = ""

'        '
'        '		 * svg objects, including a svg canvas, and several layers of svg elements;
'        '		 * each layer is independent from others, meaning I can move/ zoom-in-out a
'        '		 * certain layer without affecting others.
'        '		 

'        ' May 10, 2012
'        '
'        '		 * manipulation of the svg_tree_layer, including zoom in/out, pan keep
'        '		 * records of mouse positions
'        '		 *
'        '		 
'        Private mouse_pos As OMSVGPoint, mouse_moved As OMSVGPoint
'        ' mouse position when mouse_down event happen; jan 14, 2011;
'        Private dragging As Boolean = False, resizable As Boolean = False, mousedown As Boolean = False, resizing As Boolean = False
'        ' May 12, 2011 -  May 12, 2011; -  May 12, 2011 -  mouse left button down and drag
'        Private mouse_client_x As Integer = 0, mouse_client_y As Integer = 0
'        ' May 12, 2011
'        Private Shared scale_factor As Single = 0.9F
'        ' scaling factor for mouse wheel events
'        '
'        '		 * use the following to store positions for various text-labels NOTE: all
'        '		 * IDs are internal ID
'        '		 
'        Private BranchLengthPositions As New Dictionary(Of String, APosition)(), LeafInternalID2NodePosisionts As New Dictionary(Of String, APosition)(), BootstrapPositions As New Dictionary(Of String, APosition)()
'        ' leaf internal id to positions
'        '
'        '
'        '		 * May 13, 2011; add the following variables to initiate svg canvas
'        '		 * transformation/ translation infor
'        '		 

'        Private svg_translate_x As Single = 0, svg_translate_y As Single = 0, svg_scale As Single = 1.0F
'        ' keep tracking of current scale (used for drawing tree scale);
'        Private db_serialID As Integer = 0
'        '
'        '		 * May 20, 2011: move some variables from Legends and Branch length scale
'        '		 

'        Private ReadOnly legend_start_pos_x As Integer = 30, legend_start_pos_y As Integer = 80, branchlength_scale_y1 As Integer = 70, default_legend_font_size As Integer = 12
'        Private ReadOnly space_between_legends As Single = 10.0F, space_between_legend_and_text As Single = 5.0F, space_between_legend_items As Single = 5.0F

'        Private ReadOnly delayShowMiliSec As Integer = 500, delayHideMiliSec As Integer = 200
'        Private tooltipx As Integer = 0, tooltipy As Integer = 0

'        ''' <summary>
'        ''' >>>> oct 25, 2013 : 
'        ''' </summary>
'        Private ReadOnly flexTableCellSpacing As Integer = 2
'        Dim layer_tree_skeleton As Drawing.Graphics

'        '
'        '			 * public methods
'        '			 

'        '
'        '			 * make tree skeleton and update positions for plotting branchlength,
'        '			 * bootstrap scores and leaf labels April 8, 2011; cleanup and add
'        '			 * opacity
'        '			 


'        Sub New(Tree As PhyloTree)
'            ' Nov 2, 2013
'            ' April 8, 2011; opacity 
'            Dim opacity As Single = If(Me.dataset2opacity.ContainsKey(Me.activeBranchColorSetID), dataset2opacity(Me.activeBranchColorSetID), 1)

'            rootnode = Tree.RootNode

'            Dim bmp As New Bitmap(1024, 1024)

'            layer_tree_skeleton = Graphics.FromImage(bmp)

'            ' change it
'            ' get branch color for root node; only used when plot-mode is RECT
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            Dim branch_stroke_color As String = If((Me.branchColorEnabled AndAlso Not disable_all_dataset AndAlso Not activeBranchColorSetID.Length = 0), rootnode.getBranchColorByColorsetID(Me.activeBranchColorSetID), "black")
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            If plotmode = TreePlotMode.RECT_CLADOGRAM OrElse plotmode = TreePlotMode.RECT_PHYLOGRAM Then
'                layer_tree_skeleton.DrawLine(Pens.Black, rootXY.X, rootXY.Y, If(plotmode = TreePlotMode.RECT_PHYLOGRAM, rootXY.X - pxPerBranchLength / 10 / 5, rootXY.X - pxPerWidthCurrentMode / 3), rootXY.Y)
'                Me.rect_mode(rootnode, layer_tree_skeleton, rootXY)
'            ElseIf plotmode = TreePlotMode.SLANTED_CLADOGRAM_RECT OrElse plotmode = TreePlotMode.SLANTED_CLADOGRAM_MIDDLE OrElse plotmode = TreePlotMode.SLANTED_CLADOGRAM_NORMAL Then
'                ' plotmodes other than slanted_normal are not supported at the moment; jan 20, 2011;
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim x_first_leaf As Single = rootnode.LevelHorizontal * pxPerWidthCurrentMode + rootXY.X
'                'float y_first_leaf = 1 * pxPerHeight + margin_normal * canvas_height;
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim y_first_leaf As Single = 1 * pxPerHeight + margin_y
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim atan As Double = Math.Atan((rootXY.Y - y_first_leaf) / (x_first_leaf - rootXY.X))
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Me.slanted_mode(rootnode, layer_tree_skeleton, rootXY, atan, x_first_leaf)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            ElseIf plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Me.circular_mode(rootnode, layer_tree_skeleton, rootXY)
'            End If
'        End Sub
'        ' makeTreeSkeleton
'        '
'        '			 * April 8, 2011; cleanup and add opacity including bootstrap scores,
'        '			 * branch length and tree leaf labels == April 23-24, 2012 == change the
'        '			 * orientation of leaf/branch-length/bootstrap score labels for better
'        '			 * readability
'        '			 

'        Public Overridable Sub makeLeafLabels()

'            ' clear all 
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            clearAllChildNodesFromSVGGElement(layer_tree_leaf_labels)
'            leafID2OMNode.Clear()

'            '
'            '				 * part 1, leaf label; update or make NOTE: all keys in hashes are
'            '				 * internal IDs March 20, 2011; use native javascript to calculate
'            '				 * pixel width and height; much more accurate March 20, 2011; add
'            '				 * align leaf lables; only availale if plotmode = PHYLOGRAM
'            '				 

'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            Dim bAlignleaflable As Boolean = If((Me.alignLeafLabels_Renamed = True AndAlso (plotmode.Equals(TreePlotMode.RECT_PHYLOGRAM) OrElse plotmode.Equals(TreePlotMode.CIRCULAR_PHYLOGRAM))), True, False)
'            ' April 8, 2011; opacity 
'            Dim opacity As Single = If(Me.dataset2opacity.ContainsKey(Me.activeLeafColorSetID), dataset2opacity(Me.activeLeafColorSetID), 1)
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            layer_tree_leaf_labels.attributeute("fill-opacity", Convert.ToString(opacity))
'            ' change it
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            layer_tree_leaf_labels.style.fontSizeize(default_font_size, Unit.PX)
'            ' Oct 26, 2011; user default font size at the beginning
'            '
'            '				 * March 20, 2011; keep tracking of the right most leaf label
'            '				 * positions if alignLeafLabels is true, add
'            '				 * layer_tree_align_leaf_labels to layer_tree, else remove it from
'            '				 * layer tree
'            '				 
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            Dim stringPixelWidthHeight As JsArrayInteger = JSFuncs.stringPixelWidthHeight("Anystring", default_font_family, default_font_size)
'            Dim fontPxHeight As Single = stringPixelWidthHeight.[get](1)

'            ' if alignLeafLabels, get the right most positions for treeskeleton and leaf labels
'            If bAlignleaflable Then
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(layer_tree_align_leaf_labels)
'            Else
'                'clearAllChildNodesFromSVGGElement(layer_tree_align_leaf_labels);
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                deleteChildNodeFromLayerTree(layer_tree_align_leaf_labels)
'            End If
'            ' if alignLeafLabels is true or false
'            ' 0 = treeskeleton; 1 = leaf
'            Dim mostRightPositions As List(Of System.Nullable(Of Single)) = Me.rightMostTreeAndLabelPosisions
'            Dim mostrightTree As Single = mostRightPositions(0)
'            ' Iterate leaf node positions
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            For Each entry As KeyValuePair(Of String, APosition) In LeafInternalID2NodePosisionts
'                Dim leaf_internal_id As String = entry.Key
'                Dim pos As APosition = entry.Value

'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim leafnode As PhyloNode = m_phylotree.getNodeByID(leaf_internal_id)
'                Dim leaf_id As String = leafnode.ID

'                ' if OMNode objects for leaf label not exist; make a SVGtext object and put it at (0,0)
'                Dim leaf_id_without_underline As String = JSFuncs.JsReplaceUnderlineWithSingleSpaceChar(leaf_id)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim txtLeafLabel As OMSVGTextElement = doc.createSVGTextElement(0, 0, OMSVGLength.SVG_LENGTHTYPE_PX, leaf_id_without_underline)
'                ' add it to leafID2OMNode as well as tree_leaf_node layer
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree_leaf_labels.appendChild(txtLeafLabel)
'                leafID2OMNode.Add(leaf_internal_id, txtLeafLabel)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree_leaf_labels.transform.baseVal.consolidate()

'                '  update position / angle; 
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim x As Single = If((bAlignleaflable), mostrightTree + space_between_treeskeleton_and_leaflabels, pos.x + space_between_treeskeleton_and_leaflabels)
'                Dim y As Single = pos.y + fontPxHeight / 4
'                Dim angle As Single = pos.angle

'                ' now move the text to intented area --
'                txtLeafLabel.attributeute("x", Convert.ToString(x))
'                txtLeafLabel.attributeute("y", Convert.ToString(y))

'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                If angle <> 0 OrElse plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then

'                    ' and then rotate
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim r As OMSVGTransform = svg.createSVGTransform()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    r.rotateate(180 - angle, rootXY.X, rootXY.Y)
'                    txtLeafLabel.transform.baseVal.appendItem(r)

'                    ' Apr 24, 2012
'                    betterReadabilityForSVGTextElem(txtLeafLabel, angle, x, y - fontPxHeight / 4, "end")
'                End If

'                ' update color
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim textfillcolor As String = If((leafColorEnabled AndAlso Not activeLeafColorSetID.Length = 0 AndAlso Not disable_all_dataset), leafnode.getLeafColorByColorsetID(activeLeafColorSetID), "black")
'                txtLeafLabel.attributeute("fill", textfillcolor)

'                ' March 20, 2011; Align leaf node 
'                If bAlignleaflable Then
'                    ' if OMNode objects for leaf label not exist; make a SVGtext object and put it at (0,0)
'                    If Not leafID2LeafAlignOMNode.ContainsKey(leaf_internal_id) Then
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        Dim EmptyLine = New OMSVGLineElement(pos.x + space_between_treeskeleton_and_leaflabels / 2, pos.y, mostrightTree + space_between_treeskeleton_and_leaflabels / 2, pos.y)
'                        ' add it to leafID2OMNode as well as tree_leaf_node layer
'                        leafID2LeafAlignOMNode.Add(leaf_internal_id, EmptyLine)
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        layer_tree_align_leaf_labels.appendChild(EmptyLine)
'                    End If
'                    '
'                    '						 * update position of aligned lines
'                    '						 

'                    Dim line As OMSVGLineElement = DirectCast(leafID2LeafAlignOMNode(leaf_internal_id), OMSVGLineElement)
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    line.attributeute("x1", Convert.ToString(pos.x + space_between_treeskeleton_and_leaflabels / 2))
'                    line.attributeute("y1", Convert.ToString(pos.y))
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    line.attributeute("x2", Convert.ToString(mostrightTree + space_between_treeskeleton_and_leaflabels / 2))
'                    line.attributeute("y2", Convert.ToString(pos.y))

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim liner As OMSVGTransform = svg.createSVGTransform()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    liner.rotateate(180 - angle, rootXY.X, rootXY.Y)

'                    line.transform.baseVal.clear()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    If angle <> 0 OrElse plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then
'                        line.transform.baseVal.appendItem(liner)
'                    End If
'                    ' if align leaf labels
'                End If
'            Next
'            ' part 1 leaf label ends here
'            '
'            '				 * part 2, bootstrap scores; update or make
'            '				 
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            If m_phylotree.hasBootstrapScores() Then
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                For Each entry As KeyValuePair(Of String, APosition) In BootstrapPositions
'                    Dim node_internal_id As String = entry.Key
'                    Dim pos As APosition = entry.Value
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim node As PhyloNode = m_phylotree.getNodeByID(node_internal_id)
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim bootstrapstring As String = formatterDeci4.format(node.BootStrap)
'                    ' if OMNode objects for leaf label not exist; make a SVGtext object and put it at (0,0)
'                    If Not Me.bootstrapID2OMNode.ContainsKey(node_internal_id) Then
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        Dim text As OMSVGTextElement = New OMSVGTextElement(0, 0, OMSVGLength.SVG_LENGTHTYPE_PX, bootstrapstring)
'                        ' add it to leafID2OMNode as well as tree_leaf_node layer
'                        Me.bootstrapID2OMNode.Add(node_internal_id, text)
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        layer_tree_bootstrap.appendChild(text)
'                    End If

'                    ' update position
'                    Dim txtBootstrapScore As OMSVGTextElement = DirectCast(Me.bootstrapID2OMNode(node_internal_id), OMSVGTextElement)
'                    ' March 20, 2011;
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim strPxWidthHeight As JsArrayInteger = JSFuncs.stringPixelWidthHeight(bootstrapstring, "arial", default_bootstrap_font_size)
'                    Dim x As Single = pos.x + 3
'                    Dim y As Single = pos.y + strPxWidthHeight.[get](1) / 3
'                    Dim angle As Single = pos.angle

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim r As OMSVGTransform = svg.createSVGTransform()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    r.rotateate(180 - angle, rootXY.X, rootXY.Y)

'                    txtBootstrapScore.attributeute("x", Convert.ToString(x))
'                    txtBootstrapScore.attributeute("y", Convert.ToString(y))

'                    txtBootstrapScore.transform.baseVal.clear()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    If angle <> 0 OrElse plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then
'                        txtBootstrapScore.transform.baseVal.appendItem(r)

'                        ' Apr 24, 2012
'                        betterReadabilityForSVGTextElem(txtBootstrapScore, angle, x, y - strPxWidthHeight.[get](1) / 3, "end")
'                    End If
'                    ' part 2, bootstrap scores end here; end of FOR
'                Next
'            End If
'            ' bootstrap scores
'            '
'            '				 * part 3, branch length part
'            '				 

'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            If m_phylotree.HasBranchLength() Then
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                For Each entry As KeyValuePair(Of String, APosition) In BranchLengthPositions
'                    Dim node_internal_id As String = entry.Key
'                    Dim pos As APosition = entry.Value
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim node As PhyloNode = m_phylotree.getNodeByID(node_internal_id)
'                    ' if OMNode objects for leaf label not exist; make a SVGtext object and put it at (0,0)
'                    If Not Me.branchID2OMNode.ContainsKey(node_internal_id) Then
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        Dim text As OMSVGTextElement = doc.createSVGTextElement(0, 0, OMSVGLength.SVG_LENGTHTYPE_PX, formatterDeci4.format(node.BranchLength))
'                        text.attributeute("text-anchor", "middle")
'                        ' April 14, 2011; Align center
'                        ' add it to leafID2OMNode as well as tree_leaf_node layer
'                        Me.branchID2OMNode.Add(node_internal_id, text)
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        layer_tree_branchlength.appendChild(text)
'                    End If
'                    ' update position
'                    Dim txtBranchLength As OMSVGTextElement = DirectCast(branchID2OMNode(node_internal_id), OMSVGTextElement)

'                    Dim x As Single = pos.x
'                    Dim y As Single = pos.y - 3
'                    Dim angle As Single = pos.angle

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim r As OMSVGTransform = svg.createSVGTransform()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    r.rotateate(180 - angle, rootXY.X, rootXY.Y)

'                    txtBranchLength.attributeute("x", Convert.ToString(x))
'                    txtBranchLength.attributeute("y", Convert.ToString(y))

'                    txtBranchLength.transform.baseVal.clear()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    If angle <> 0 OrElse plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then
'                        txtBranchLength.transform.baseVal.appendItem(r)

'                        betterReadabilityForSVGTextElem(txtBranchLength, angle, x, y + 3, "middle")
'                    End If
'                    ' part 3, branch length end here; end of FOR
'                Next
'            End If
'            ' if branch length exist
'        End Sub
'        ' makeOrUpdate Leaf Lables
'        '
'        '			 * March 21, 2011; April 8, 2011; cleanup and add opacity;
'        '			 

'        Public Overridable Sub makeLeafLabelBackground()
'            ' first of all, clear its contents
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            clearAllChildNodesFromSVGGElement(layer_tree_leaflabel_background)
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            Dim bAlignleaflable As Boolean = If((Me.alignLeafLabels_Renamed = True AndAlso (plotmode.Equals(TreePlotMode.RECT_PHYLOGRAM) OrElse plotmode.Equals(TreePlotMode.CIRCULAR_PHYLOGRAM))), True, False)
'            ' April 8, 2011; opacity
'            Dim opacity As Single = If(Me.dataset2opacity.ContainsKey(Me.activeLeafBKColorSetID), dataset2opacity(Me.activeLeafBKColorSetID), 0.5F)
'            ' by default, leaf background opacity is 0.5
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            layer_tree_leaflabel_background.attributeute("fill-opacity", Convert.ToString(opacity))
'            ' change it
'            ' background colors for leaf nodes
'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            If Me.leafBKColorEnabled AndAlso Not disable_all_dataset AndAlso Not Me.activeLeafBKColorSetID.Length = 0 Then
'                ' 0 = treeskeleton; 1 = leaf
'                Dim mostRightPositions As List(Of System.Nullable(Of Single)) = Me.rightMostTreeAndLabelPosisions
'                Dim mostrightTree As Single = mostRightPositions(0)
'                Dim mostrightLeaf As Single = mostRightPositions(1)
'                ' make background; March 21, 2011
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                For Each entry As KeyValuePair(Of String, APosition) In LeafInternalID2NodePosisionts
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim leafnode As PhyloNode = m_phylotree.getNodeByID(entry.Key)
'                    Dim startpos As APosition = entry.Value
'                    Dim angle As Single = startpos.angle

'                    Dim bkcolor As String = leafnode.getLeafBKColorByColorsetID(Me.activeLeafBKColorSetID)
'                    If Not bkcolor.ToUpper() = "white".ToUpper() Then
'                        ' if circular mode 
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        If plotmode.Equals(TreePlotMode.CIRCULAR_CLADOGRAM) OrElse plotmode.Equals(TreePlotMode.CIRCULAR_PHYLOGRAM) Then
'                            '
'                            '								 * 1. get four points that mark the fan-shape
'                            '								 * locations of the four points are: 1 ---> 2 | |
'                            '								 * |arc | arc | v 4 <--- 3
'                            '								 

'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim angleSpanPerLeafNode As Single = angle_span / m_phylotree.LeafNodes.size()
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim innerRadius As Single = If((bAlignleaflable), mostrightTree + space_between_treeskeleton_and_leaflabels / 2 - rootXY.X, startpos.x + space_between_treeskeleton_and_leaflabels / 2 - rootXY.X)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim outterRadius As Single = mostrightLeaf - rootXY.X + space_between_datasets / 3

'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim p1 As OMSVGPoint = getCoordinatesOnCircle(rootXY, innerRadius, 360 - angleSpanPerLeafNode / 2)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim p2 As OMSVGPoint = getCoordinatesOnCircle(rootXY, outterRadius, 360 - angleSpanPerLeafNode / 2)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim p3 As OMSVGPoint = getCoordinatesOnCircle(rootXY, outterRadius, angleSpanPerLeafNode / 2)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim p4 As OMSVGPoint = getCoordinatesOnCircle(rootXY, innerRadius, angleSpanPerLeafNode / 2)

'                            ' 2. make fan using path
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim fan As OMSVGPathElement = New OMSVGPathElement
'                            Dim segs As OMSVGPathSegList = New OMSVGPathSegList
'                            segs.appendItem(fan.createSVGPathSegMovetoAbs(p1.X, p1.Y))
'                            ' move to p1
'                            segs.appendItem(fan.createSVGPathSegLinetoAbs(p2.X, p2.Y))
'                            ' line from p1 to p2;
'                            segs.appendItem(fan.createSVGPathSegArcAbs(p3.X, p3.Y, outterRadius, outterRadius, 0, False, _
'                                False))
'                            ' arc from p2 to p3
'                            segs.appendItem(fan.createSVGPathSegLinetoAbs(p4.X, p4.Y))
'                            ' line from p3 to p4;
'                            segs.appendItem(fan.createSVGPathSegArcAbs(p1.X, p1.Y, innerRadius, innerRadius, 0, False, _
'                                True))
'                            ' arc from p4 to p1
'                            segs.appendItem(fan.createSVGPathSegClosePath())
'                            ' close path
'                            fan.attributeute("stroke", "none")
'                            fan.attributeute("fill", bkcolor)

'                            ' 3. rotate; Feb 12, 2012 --
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            If angle <> 0 OrElse plotmode = TreePlotMode.CIRCULAR_CLADOGRAM OrElse plotmode = TreePlotMode.CIRCULAR_PHYLOGRAM Then
'                                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                                Dim fanr As OMSVGTransform = svg.createSVGTransform()
'                                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                                fanr.rotateate(-angle, rootXY.X, rootXY.Y)
'                                ' March 21, 2011; this rotation is correct
'                                fan.transform.baseVal.appendItem(fanr)
'                            End If
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            layer_tree_leaflabel_background.appendChild(fan)
'                        Else
'                            ' non-circular mode
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim rectX1 As Single = If((bAlignleaflable), mostrightTree + space_between_treeskeleton_and_leaflabels / 2, startpos.x + space_between_treeskeleton_and_leaflabels / 2)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim rectX2 As Single = mostrightLeaf + space_between_datasets / 3
'                            Dim rectWidth As Single = rectX2 - rectX1
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            Dim rect As OMSVGRectElement = doc.createSVGRectElement(rectX1, startpos.y - pxPerHeight / 2, rectWidth, pxPerHeight, 0, 0)
'                            rect.attributeute("stroke", "none")
'                            rect.attributeute("fill", bkcolor)
'                            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                            layer_tree_leaflabel_background.appendChild(rect)
'                            ' plot mode is circular / non-circular
'                        End If
'                        ' if background color isn't white
'                    End If
'                    ' iterate hashPositionOfLeafNodes one more time
'                Next
'            End If
'            ' if leafBKColorEnabled
'        End Sub

'        ' April 4, 2011;
'        ' <<<<<<<<<<< -------------- branch colors -------------
'        '
'        '			 * NOTE: datasetID should be TYPE + actualldatasetID, for example the
'        '			 * name for a branch data named 'bd1' will be BRANCHCOLORbd1 therefore
'        '			 * this color dataset name is unique
'        '			 

'        Public Overridable Function getOriginalUserInputAsStringByID(datasetid As String) As String
'            Return If(Me.dataset2originalUserinputAsString.ContainsKey(datasetid), Me.dataset2originalUserinputAsString(datasetid), "")
'        End Function
'        ' April 4, 2011;
'        '
'        '			 * >>>>>>>>>>>>>>>> align leaf labels >>>>>>>>>>>>>>>
'        '			 

'        Public Overridable ReadOnly Property alignLeafLabels() As Boolean
'            Get
'                Return Me.alignLeafLabels_Renamed
'            End Get
'        End Property
'        ' March 20, 2011;
'        ' March 21, 2011; setAlignLeafLabels and return current status
'        ' Oct 26, 2011; fix a bug here
'        Public Overridable Function setAlignLeafLabels(alignleaflabel As Boolean) As Boolean
'            If Me.alignLeafLabels_Renamed <> alignleaflabel Then
'                Me.alignLeafLabels_Renamed = alignleaflabel

'                ' if current mode is phylogram, replot
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                If plotmode.Equals(TreePlotMode.RECT_PHYLOGRAM) OrElse plotmode.Equals(TreePlotMode.CIRCULAR_PHYLOGRAM) Then
'                    Me.makeLeafLabels()
'                    Me.makeLeafLabelBackground()
'                End If
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                ' May 14, 2011
'                updateTreeCanvasInfoOnServer("alignLeafLabel", If(alignLeafLabels_Renamed, 1.0F, 0.0F))
'            End If
'            Return alignLeafLabels_Renamed
'        End Function
'        ' setAlignLeafLabels;
'        ' <<<<<<<<<<<<<<<< align leaf labels <<<<<<<<<<<<<<<
'        '
'        '			 * private functions
'        '			 

'        '
'        '			 * March 21, 2011;
'        '			 * April 11, 2013; maxLenLeafLabel = maxLenLeafLabel + default_font_size
'        '			 

'        Private ReadOnly Property rightMostTreeAndLabelPosisions() As List(Of System.Nullable(Of Single))
'            Get
'                Dim posLeafLabels As Single = 0.0F, posTreeSkeleton As Single = 0.0F, maxLenLeafLabel As Single = 0.0F

'                ' iterate hashPositionOfLeafNodes
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                For Each entry As KeyValuePair(Of String, APosition) In LeafInternalID2NodePosisionts
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim leaf_id As String = m_phylotree.getNodeByID(entry.Key).ID
'                    Dim x As Single = entry.Value.x

'                    ' rightmostPositionsTreeSkeleton
'                    If x > posTreeSkeleton Then
'                        posTreeSkeleton = x
'                    End If
'                    ' march 20, 2011
'                    ' get rightmostPositionsLeafLabels
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim pxWidth As Single = JSFuncs.stringPixelWidthHeight(leaf_id, default_font_family, default_font_size).[get](0)

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    If (x + space_between_treeskeleton_and_leaflabels + pxWidth) > posLeafLabels Then
'                        'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                        posLeafLabels = x + space_between_treeskeleton_and_leaflabels + pxWidth
'                    End If
'                    ' march 21, 2011
'                    If pxWidth > maxLenLeafLabel Then
'                        maxLenLeafLabel = pxWidth
'                    End If
'                Next
'                ' iterate HashMap : hashPositionOfLeafNodes
'                If Me.alignLeafLabels_Renamed Then
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    posLeafLabels = posTreeSkeleton + maxLenLeafLabel + space_between_treeskeleton_and_leaflabels
'                End If
'                ' March 21, 2011;
'                Dim rightmostpos As New List(Of System.Nullable(Of Single))(2)
'                rightmostpos.Add(posTreeSkeleton)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                rightmostpos.Add(posLeafLabels + default_font_size)
'                ' April 11, 2013
'                Return rightmostpos
'            End Get
'        End Property

'        '
'        '			 * >>>>>>>>>> PLOT TREE in various modes >>>>>>>>>>>>>>> Jan 9, 2011;
'        '			 * add support to phylogram mode March 17, 2011; add branch color
'        '			 

'        Private Sub circular_mode(pnode As PhyloNode, layer_tree As OMSVGGElement, parentxy As OMSVGPoint)
'            Dim px As Single = parentxy.X
'            Dim py As Single = parentxy.Y

'            Dim vlevel As Single = pnode.LevelVertical
'            'float hlevel = pnode.getLevelHorizontal();

'            'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'            Dim pradius As Single = If((plotmode = TreePlotMode.CIRCULAR_CLADOGRAM), (rootnode.LevelHorizontal - pnode.LevelHorizontal) * pxPerWidthCurrentMode, pnode.BranchLengthToRoot * pxPerBranchLength)
'            ' iterate child nodes
'            For Each dnode As PhyloNode In pnode.Descendents
'                ' stroke color for current branch; march 17, 2011
'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final String branch_stroke_color = (this.branchColorEnabled && !disable_all_dataset && !this.activeBranchColorSetID.isEmpty()) ? dnode.getBranchColorByColorsetID(this.activeBranchColorSetID) : "black";
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim branch_stroke_color As String = If((Me.branchColorEnabled AndAlso Not disable_all_dataset AndAlso Not Me.activeBranchColorSetID.Length = 0), dnode.getBranchColorByColorsetID(Me.activeBranchColorSetID), "black")
'                '
'                '					 * for each descdent node
'                '					 

'                Dim vl As Single = dnode.LevelVertical
'                'float hl = dnode.getLevelHorizontal();
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim dradius As Single = If((plotmode = TreePlotMode.CIRCULAR_CLADOGRAM), (rootnode.LevelHorizontal - dnode.LevelHorizontal) * pxPerWidthCurrentMode, (dnode.BranchLengthToRoot * pxPerBranchLength))

'                ' Dec 22, 2011; fine-tune angle 
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim angle As Single = If(circular_tree_plot_in_clockwise_mode, -CSng(dnode.LevelVertical - 1.0F) * angle_per_vertical_level - 180.0F, CSng(dnode.LevelVertical - 1.0F) * angle_per_vertical_level - 180.0F)
'                ' angle = 360 - angle
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                angle += (360.0F + angle_start)
'                angle = angle Mod 360.0F
'                If angle < 0 Then
'                    angle += 360.0F
'                End If

'                ' calculate positions of the points on arc
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim dxy As OMSVGPoint = getCoordinatesOnCircle(rootXY, dradius, angle)
'                ' xy of current node at current circle
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim pxy As OMSVGPoint = getCoordinatesOnCircle(rootXY, pradius, angle)
'                ' xy of current node at parent circle
'                '
'                '					 * largeArcFlag: A value of 0 indicates that the arc's measure
'                '					 * is less than 180掳. A value of 1 indicates that the arc's
'                '					 * measure is greater than or equal to 180掳 sweepFlag: A value
'                '					 * of 0 indicates that the arc is to be drawn in the negative
'                '					 * angle direction (counterclockwise). A value of 1 indicates
'                '					 * that the arch is to be drawn in the positive angle direction
'                '					 * (clockwise).
'                '					 *
'                '					 * revised: Feb 28, 2011; add support to
'                '					 * circular_tree_plot_in_clockwise_mode
'                '					 

'                Dim sweepFlag As Boolean = If(vl < vlevel, False, True)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                If circular_tree_plot_in_clockwise_mode = False Then
'                    sweepFlag = Not sweepFlag
'                End If

'                ' boolean largeArcFlag = angle >= 180 ? true : false; 
'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGPathElement path = doc.createSVGPathElement();
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim path As OMSVGPathElement = doc.createSVGPathElement()
'                Dim segs As OMSVGPathSegList = path.pathSegList
'                segs.appendItem(path.createSVGPathSegMovetoAbs(px, py))
'                segs.appendItem(path.createSVGPathSegArcAbs(pxy.X, pxy.Y, pradius, pradius, 0, False, _
'                    sweepFlag))
'                segs.appendItem(path.createSVGPathSegLinetoAbs(dxy.X, dxy.Y))
'                path.attributeute("stroke", branch_stroke_color)
'                path.attributeute("fill", "none")

'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGCircleElement c = addCircleAndMouseEventToSVEPathElement(path, dxy, branch_stroke_color, dnode);
'                Dim c As OMSVGCircleElement = addCircleAndMouseEventToSVEPathElement(path, dxy, branch_stroke_color, dnode)
'                ' append g to layer tree
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(path)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(c)

'                ' <<<< =---====  Jan 09, 2012 ----===

'                ' current position for label, before rotation; jan 23, 2011;
'                Dim node_internal_id As String = dnode.InternalID
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim current_xy As OMSVGPoint = New Drawing.Point(rootXY.X + dradius, rootXY.Y)
'                If dnode.IsLeaf = False Then
'                    ' prepare a transform elements; used by bootstrap/ leaf label and branch length
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    Dim r As OMSVGTransform = svg.createSVGTransform()
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    r.rotateate(180.0F - angle, rootXY.X, rootXY.Y)

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    BootstrapPositions.Add(node_internal_id, New APosition(current_xy, angle))
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    circular_mode(dnode, layer_tree, dxy)
'                Else
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    LeafInternalID2NodePosisionts.Add(node_internal_id, New APosition(current_xy, angle))
'                End If
'                ' leaf node VS Internal Node
'                ' keep trakcing the position for branch length
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                BranchLengthPositions.Add(node_internal_id, New APosition(New OMSVGPoint(rootXY.X + (dradius + pradius) / 2, rootXY.Y), angle))
'            Next
'            ' for each descendant
'        End Sub

'        Private Function getCoordinatesOnCircle(xy As OMSVGPoint, radius As Single, angle As Single) As OMSVGPoint
'            ' angle cannot be higher than 360
'            angle = angle Mod CSng(360)
'            ' narrow angle within range of 0~90
'            Dim nangle As Single = angle Mod CSng(90.0)
'            ' translate angle to radians
'            Dim radians As Single = CSng(nangle * 2.0 * Math.PI / 360.0)
'            ' assume that the center is at (0,0)
'            Dim sin As Single = CSng(Math.Sin(radians))
'            Dim cos As Single = CSng(Math.Cos(radians))
'            Dim x As Single = cos * radius
'            Dim y As Single = sin * radius
'            ' adjust x,y according to original angle
'            Dim newx As Single = 0, newy As Single = 0
'            If angle >= 0 AndAlso angle < 90 Then
'                newx = -x
'                newy = y
'            ElseIf angle >= 90 AndAlso angle < 180 Then
'                newx = y
'                newy = x
'            ElseIf angle >= 180 AndAlso angle < 270 Then
'                newx = x
'                newy = -y
'            ElseIf angle >= 270 AndAlso angle < 360 Then
'                newx = -y
'                newy = -x
'            End If
'            ' make a SVGpoint based on newx, newy; and move it to xy
'            Dim newxy As OMSVGPoint = New Drawing.Point(newx, newy)
'            Return newxy
'        End Function


'        ' circular_cladogram_normal_plot
'        '
'        '			 * jan 8, 2011; version = 2; slanted MODE March 17, 2011; branch stoke
'        '			 * color
'        '			 

'        Private Sub slanted_mode(pnode As PhyloNode, layer_tree As OMSVGGElement, parentxy As OMSVGPoint, atan As Double, x_first_leaf As Single)
'            Dim px As Single = parentxy.X
'            Dim py As Single = parentxy.Y

'            Dim vlevelS As Single = pnode.LevelVerticalSlanted
'            Dim vlevel As Single = pnode.LevelVertical
'            Dim hlevel As Single = pnode.LevelHorizontal
'            ' iterate child nodes
'            For Each dnode As PhyloNode In pnode.Descendents
'                ' stroke color for current branch; march 17, 2011
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim branch_stroke_color As String = If((Me.branchColorEnabled AndAlso Not disable_all_dataset AndAlso Not Me.activeBranchColorSetID.Length = 0), dnode.getBranchColorByColorsetID(Me.activeBranchColorSetID), "black")
'                '
'                '					 * for each descdent node
'                '					 

'                Dim vlS As Single = dnode.LevelVerticalSlanted
'                Dim vl As Single = dnode.LevelVertical
'                Dim hl As Single = dnode.LevelHorizontal

'                Dim min_leaf_v As Single = dnode.minLeafVerticalLevel
'                Dim max_leaf_v As Single = dnode.maxLeafVerticalLevel

'                'float x_min_leaf = x_first_leaf;
'                'float y_min_leaf = min_leaf_v * pxPerHeight + margin_normal * canvas_height;
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim y_min_leaf As Single = min_leaf_v * pxPerHeight + margin_y

'                Dim dx As Single = 0, dy As Single = 0
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                If plotmode = TreePlotMode.SLANTED_CLADOGRAM_RECT Then
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dx = px + (hlevel - hl) * pxPerWidthCurrentMode
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dy = py + (vl - vlevel) * pxPerHeight
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                ElseIf plotmode = TreePlotMode.SLANTED_CLADOGRAM_MIDDLE Then
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dx = px + (hlevel - hl) * pxPerWidthCurrentMode
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dy = py + (vlS - vlevelS) * pxPerHeight
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                ElseIf plotmode = TreePlotMode.SLANTED_CLADOGRAM_NORMAL Then
'                    '
'                    '						 * jan 7, 2011; NOTE: in this mode, the position of internal
'                    '						 * node is determined by it's first leaf node and last leaf
'                    '						 * node. its XY-coordinates are: y = (Y of its first leaf
'                    '						 * node + Y of its last leaf node) / 2; x is a little
'                    '						 * complicated; here is howto:
'                    '						 *
'                    '						 * 1. suppose coordinateXY of root is : x1, y1, and the
'                    '						 * first leaf is: x2, y2; then we'll have a global angle for
'                    '						 * the whole plot: atan = Math.atan( (y2 - y1) / (x2 - x1)
'                    '						 * ); 2. for internal nodes, x = X of leaf node (all leaf
'                    '						 * nodes have the same X) - (Y of its last leaf node - Y of
'                    '						 * its first leaf node) / 2 / Math.tan(atan);
'                    '						 *
'                    '						 * the output is similar to SLANTED_CLADOGRAM in Dendroscope
'                    '						 

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dx = If(dnode.IsLeaf, x_first_leaf, x_first_leaf - ((max_leaf_v - min_leaf_v) / 2 * pxPerHeight) / CSng(Math.Tan(atan)))
'                    ' it's very strange here
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dy = y_min_leaf + (max_leaf_v - min_leaf_v) / 2 * pxPerHeight
'                End If

'                '                OMSVGLineElement sline = doc.createSVGLineElement(px, py, dx, dy);
'                '                sline.setAttribute("stroke", branch_stroke_color);
'                '                layer_tree.appendChild(sline);
'                '                
'                ' use path instead of two lines to make branches 
'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGPathElement path = doc.createSVGPathElement();
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim path As OMSVGPathElement = New OMSVGPathElement
'                Dim segs As OMSVGPathSegList = New OMSVGPathSegList
'                segs.appendItem(path.createSVGPathSegMovetoAbs(px, py))
'                segs.appendItem(path.createSVGPathSegLinetoAbs(dx, dy))
'                path.attributeute("stroke", branch_stroke_color)
'                path.attributeute("fill", "none")

'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGCircleElement c = addCircleAndMouseEventToSVEPathElement(path, svg.createSVGPoint(dx, dy), branch_stroke_color, dnode);
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim c As OMSVGCircleElement = addCircleAndMouseEventToSVEPathElement(path, New OMSVGPoint(dx, dy), branch_stroke_color, dnode)
'                ' append g to layer tree
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(path)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(c)
'                ' <<<< ==== Jan 09, 2012 ==== <<<<

'                ' check if current node is leaf
'                Dim node_internal_id As String = dnode.InternalID
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim current_xy As OMSVGPoint = New Drawing.Point(dx, dy)
'                If dnode.IsLeaf = False Then
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    BootstrapPositions.Add(node_internal_id, New APosition(current_xy, 0))
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    slanted_mode(dnode, layer_tree, current_xy, atan, x_first_leaf)
'                Else
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    LeafInternalID2NodePosisionts.Add(node_internal_id, New APosition(current_xy, 0))
'                End If
'                ' leaf or internal node
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                BranchLengthPositions.Add(node_internal_id, New APosition(New OMSVGPoint((px + dx) / 2, (py + dy) / 2), 0))
'            Next
'            ' for each descendent nodes
'        End Sub
'        ' similar to rect mode
'        '
'        '			 * jan 8, 2011; version = 2; RECT MODE * March 17, 2011; branch stoke
'        '			 * color April 8, 2011; clean up
'        '			 

'        Private Sub rect_mode(pnode As PhyloNode, layer_tree As Graphics, parentxy As OMSVGPoint)
'            Dim px As Single = parentxy.X
'            Dim py As Single = parentxy.Y

'            System.Console.WriteLine(vbTab & vbTab & "--> plot " & Convert.ToString(pnode.InternalID) & "; " & Convert.ToString(pnode.ID))

'            Dim vlevel As Single = pnode.LevelVertical
'            Dim hlevel As Single = pnode.LevelHorizontal
'            ' iterate child nodes
'            For Each dnode As PhyloNode In pnode.Descendents
'                ' stroke color for current branch; march 17, 2011
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim branch_stroke_color As String = If((Me.branchColorEnabled AndAlso Not disable_all_dataset AndAlso Not Me.activeBranchColorSetID.Length = 0), dnode.getBranchColorByColorsetID(Me.activeBranchColorSetID), "black")
'                '
'                '					 * for each descdent node
'                '					 

'                Dim vl As Single = dnode.LevelVertical
'                Dim hl As Single = dnode.LevelHorizontal
'                Dim hb As Single = dnode.BranchLength

'                Dim dx As Single = px
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                If plotmode = TreePlotMode.RECT_CLADOGRAM Then
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dx += (hlevel - hl) * pxPerWidthCurrentMode
'                Else
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    dx += hb * pxPerBranchLength
'                End If
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim dy As Single = py + (vl - vlevel) * pxPerHeight

'                ' use path instead of two lines to make branches 
'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGPathElement path = doc.createSVGPathElement();
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim path As OMSVGPathElement = New OMSVGPathElement
'                Dim segs As OMSVGPathSegList = New OMSVGPathSegList
'                segs.appendItem(path.createSVGPathSegMovetoAbs(px, py))
'                segs.appendItem(path.createSVGPathSegLinetoAbs(px, dy))
'                segs.appendItem(path.createSVGPathSegLinetoAbs(dx, dy))
'                path.attributeute("stroke", branch_stroke_color)
'                path.attributeute("fill", "none")

'                'JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
'                'ORIGINAL LINE: final org.vectomatic.dom.svg.OMSVGCircleElement c = addCircleAndMouseEventToSVEPathElement(path, svg.createSVGPoint(dx, dy), branch_stroke_color, dnode);
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim c As OMSVGCircleElement = addCircleAndMouseEventToSVEPathElement(path, New OMSVGPoint(dx, dy), branch_stroke_color, dnode)
'                ' append g to layer tree
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(path)
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                layer_tree.appendChild(c)
'                ' <<<< ==== Jan 09, 2012 ==== <<<<

'                ' check if is leaf
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                Dim current_xy As OMSVGPoint = New Drawing.Point(dx, dy)
'                Dim node_internal_id As String = dnode.InternalID
'                If dnode.IsLeaf Then
'                    '
'                    '						 * keep record of position of leaf label
'                    '						 

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    LeafInternalID2NodePosisionts.Add(node_internal_id, New APosition(current_xy, 0))
'                Else
'                    '
'                    '						 * if internal label, make text element for branch length
'                    '						 * (if any) and bootstrap score
'                    '						 

'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    BootstrapPositions.Add(node_internal_id, New APosition(current_xy, 0))
'                    'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                    rect_mode(dnode, layer_tree, current_xy)
'                End If
'                ' leaf or internal node
'                '
'                '					 * keep traking of positions for showing branch length
'                '					 
'                'JAVA TO C# CONVERTER TODO TASK: C# doesn't allow accessing outer class instance members within a nested class:
'                BranchLengthPositions.Add(node_internal_id, New APosition(New OMSVGPoint((px + dx) / 2, dy), 0))
'            Next
'            ' iterate child nodes
'        End Sub
'    End Class
'End Namespace
