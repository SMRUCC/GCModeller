' ============================================================================
'  Module 2 - PhylogenyReconstruction.vb
'  Phylogeny & ancestral state reconstruction
'
'  Implements a simplified GLOOME-style maximum-likelihood ancestral-state
'  reconstruction for binary (presence/absence) characters on a fixed tree.
'
'  Model: continuous-time Markov chain with two states (0 = absent,
'  1 = present) and rates alpha (0->1) and beta (1->0). The transition
'  matrix over branch length t is:
'
'      P_00(t) = beta + (alpha - beta) * exp(-(alpha+beta)*t)   / (alpha+beta)
'      P_01(t) = alpha * (1 - exp(-(alpha+beta)*t))             / (alpha+beta)
'      P_10(t) = beta  * (1 - exp(-(alpha+beta)*t))             / (alpha+beta)
'      P_11(t) = alpha + (beta  - alpha) * exp(-(alpha+beta)*t) / (alpha+beta)
'
'  Posterior probabilities of "gain" and "loss" events on each branch are
'  computed by Felsenstein's pruning algorithm (downward pass) followed by
'  a marginal reconstruction pass (upward).
'
'  In the full Traitar pipeline this module consumes the sequenced Tree of
'  Life (sTOL); for the demo we accept any Newick tree and any binary
'  character matrix.
' ============================================================================

Namespace Modules

    ''' <summary>
    ''' A simple binary tree node used for ancestral-state reconstruction.
    ''' Branch lengths are stored on the edge to the parent.
    ''' </summary>
    Public Class PhyloNode
        Public Property Name As String = ""
        Public Property BranchLength As Double = 1.0R
        Public Property Parent As PhyloNode = Nothing
        Public Property Children As New List(Of PhyloNode)()
        Public Property IsLeaf As Boolean
        ' Per-character leaf state (0/1) for leaves; ancestral posterior (0..1) for internal nodes
        Public Property LeafState As Integer = -1
        Public Property PosteriorPresent As Double = 0.0R
        Public Property PosteriorAbsent As Double = 0.0R
        ' Per-branch gain/loss posterior (filled by the reconstruction pass)
        Public Property GainPosterior As Double = 0.0R
        Public Property LossPosterior As Double = 0.0R
    End Class

    Public Module PhylogenyReconstruction

        ' Default rate parameters (gain/loss rates per unit branch length)
        Public Const DefaultAlpha As Double = 0.1R   ' 0 -> 1 rate
        Public Const DefaultBeta As Double = 0.1R    ' 1 -> 0 rate

        ''' <summary>
        ''' Compute the 2x2 transition probability matrix over branch length t
        ''' under the binary continuous-time Markov chain.
        ''' Returns P[i,j] = P(state j at child | state i at parent, t).
        ''' </summary>
        Public Function TransitionMatrix(alpha As Double, beta As Double, t As Double) As Double(,)
            Dim sum As Double = alpha + beta
            Dim expTerm As Double = Math.Exp(-sum * t)
            Dim P(1, 1) As Double
            If sum = 0.0R Then
                P(0, 0) = 1.0R : P(0, 1) = 0.0R
                P(1, 0) = 0.0R : P(1, 1) = 1.0R
            Else
                P(0, 0) = (beta + (alpha - beta) * expTerm) / sum
                P(0, 1) = (alpha * (1.0R - expTerm)) / sum
                P(1, 0) = (beta * (1.0R - expTerm)) / sum
                P(1, 1) = (alpha + (beta - alpha) * expTerm) / sum
            End If
            Return P
        End Function

        ''' <summary>
        ''' Felsenstein pruning: compute the partial likelihood vector at each
        ''' node for a single binary character. partial[node] = (L0, L1) where
        ''' Lk = P(descendant leaves | node state = k).
        ''' </summary>
        Public Function DownwardPass(node As PhyloNode,
                                      alpha As Double,
                                      beta As Double) As Double()
            Dim [partial](1) As Double
            If node.IsLeaf Then
                If node.LeafState = 1 Then
                    [partial](0) = 0.0R : [partial](1) = 1.0R
                ElseIf node.LeafState = 0 Then
                    [partial](0) = 1.0R : [partial](1) = 0.0R
                Else
                    ' Missing data
                    [partial](0) = 1.0R : [partial](1) = 1.0R
                End If
                Return [partial]
            End If

            [partial](0) = 1.0R : [partial](1) = 1.0R
            For Each child As PhyloNode In node.Children
                Dim childPartial As Double() = DownwardPass(child, alpha, beta)
                Dim P(,) As Double = TransitionMatrix(alpha, beta, child.BranchLength)
                ' Marginalize over child's state
                Dim c0 As Double = P(0, 0) * childPartial(0) + P(0, 1) * childPartial(1)
                Dim c1 As Double = P(1, 0) * childPartial(0) + P(1, 1) * childPartial(1)
                [partial](0) *= c0
                [partial](1) *= c1
            Next
            Return [partial]
        End Function

        ''' <summary>
        ''' Marginal ancestral-state reconstruction: compute the posterior
        ''' probability that each internal node was in state 1 (present),
        ''' given the leaf states and the tree. Uses a uniform root prior.
        ''' </summary>
        Public Sub ReconstructAncestralStates(root As PhyloNode,
                                               alpha As Double,
                                               beta As Double)
            ' Downward pass: partial likelihoods at every node
            Dim partials As New Dictionary(Of PhyloNode, Double())()
            CollectPartials(root, alpha, beta, partials)

            ' Root posterior with uniform prior
            Dim rootPartial As Double() = partials(root)
            Dim sum As Double = rootPartial(0) + rootPartial(1)
            If sum = 0.0R Then
                root.PosteriorAbsent = 0.5R : root.PosteriorPresent = 0.5R
            Else
                root.PosteriorAbsent = rootPartial(0) / sum
                root.PosteriorPresent = rootPartial(1) / sum
            End If

            ' Upward pass: propagate posteriors from root to children
            PropagatePosteriors(root, alpha, beta, partials)
        End Sub

        Private Sub CollectPartials(node As PhyloNode,
                                     alpha As Double, beta As Double,
                                     partials As Dictionary(Of PhyloNode, Double()))
            partials(node) = DownwardPass(node, alpha, beta)
            For Each child As PhyloNode In node.Children
                CollectPartials(child, alpha, beta, partials)
            Next
        End Sub

        Private Sub PropagatePosteriors(node As PhyloNode,
                                         alpha As Double, beta As Double,
                                         partials As Dictionary(Of PhyloNode, Double()))
            For Each child As PhyloNode In node.Children
                ' P(child=k | parent=j) * P(parent=j) * P(siblings | parent=j)
                ' Marginalize over parent state j
                Dim P(,) As Double = TransitionMatrix(alpha, beta, child.BranchLength)
                Dim childPartial As Double() = partials(child)
                Dim p0 As Double = 0.0R, p1 As Double = 0.0R
                ' parent state 0
                p0 += node.PosteriorAbsent * P(0, 0) * childPartial(0)
                p1 += node.PosteriorAbsent * P(0, 1) * childPartial(1)
                ' parent state 1
                p0 += node.PosteriorPresent * P(1, 0) * childPartial(0)
                p1 += node.PosteriorPresent * P(1, 1) * childPartial(1)
                Dim sum As Double = p0 + p1
                If sum = 0.0R Then
                    child.PosteriorAbsent = 0.5R : child.PosteriorPresent = 0.5R
                Else
                    child.PosteriorAbsent = p0 / sum
                    child.PosteriorPresent = p1 / sum
                End If

                ' Branch gain/loss posteriors:
                '   gain  = P(parent=0, child=1 | data)
                '   loss  = P(parent=1, child=0 | data)
                Dim gain As Double = (node.PosteriorAbsent * P(0, 1) * childPartial(1)) / If(sum = 0.0R, 1.0R, sum)
                Dim loss As Double = (node.PosteriorPresent * P(1, 0) * childPartial(0)) / If(sum = 0.0R, 1.0R, sum)
                child.GainPosterior = gain
                child.LossPosterior = loss

                PropagatePosteriors(child, alpha, beta, partials)
            Next
        End Sub

        ''' <summary>
        ''' Collect all branch gain/loss posteriors from the tree as a flat
        ''' list of (node name, gain posterior, loss posterior).
        ''' </summary>
        Public Function CollectBranchEvents(root As PhyloNode) As List(Of Tuple(Of String, Double, Double))
            Dim result As New List(Of Tuple(Of String, Double, Double))()
            CollectBranchEventsRec(root, result)
            Return result
        End Function

        Private Sub CollectBranchEventsRec(node As PhyloNode,
                                            result As List(Of Tuple(Of String, Double, Double)))
            If node.Parent IsNot Nothing Then
                result.Add(Tuple.Create(node.Name, node.GainPosterior, node.LossPosterior))
            End If
            For Each child As PhyloNode In node.Children
                CollectBranchEventsRec(child, result)
            Next
        End Sub

        ''' <summary>
        ''' Build a simple balanced binary tree from a list of leaf names,
        ''' with unit branch lengths. Useful for the demo when no real sTOL
        ''' is available.
        ''' </summary>
        Public Function BuildBalancedTree(leafNames As List(Of String)) As PhyloNode
            If leafNames.Count = 0 Then Return Nothing
            Dim leaves As New List(Of PhyloNode)()
            For Each name As String In leafNames
                leaves.Add(New PhyloNode() With {.Name = name, .IsLeaf = True, .BranchLength = 1.0R})
            Next
            Do While leaves.Count > 1
                Dim parents As New List(Of PhyloNode)()
                Dim i As Integer = 0
                While i < leaves.Count
                    Dim p As New PhyloNode() With {.IsLeaf = False, .BranchLength = 1.0R}
                    p.Children.Add(leaves(i))
                    leaves(i).Parent = p
                    If i + 1 < leaves.Count Then
                        p.Children.Add(leaves(i + 1))
                        leaves(i + 1).Parent = p
                    End If
                    parents.Add(p)
                    i += 2
                End While
                leaves = parents
            Loop
            Return leaves(0)
        End Function

        ''' <summary>
        ''' Set the leaf state of a tree node by name (used to encode a
        ''' binary character - e.g. a Pfam presence/absence pattern - on
        ''' the leaves before ancestral reconstruction).
        ''' </summary>
        Public Sub SetLeafStates(root As PhyloNode, leafStates As Dictionary(Of String, Integer))
            SetLeafStatesRec(root, leafStates)
        End Sub

        Private Sub SetLeafStatesRec(node As PhyloNode, leafStates As Dictionary(Of String, Integer))
            If node.IsLeaf Then
                If leafStates.ContainsKey(node.Name) Then
                    node.LeafState = leafStates(node.Name)
                Else
                    node.LeafState = -1   ' missing
                End If
            Else
                For Each child As PhyloNode In node.Children
                    SetLeafStatesRec(child, leafStates)
                Next
            End If
        End Sub

    End Module

End Namespace
