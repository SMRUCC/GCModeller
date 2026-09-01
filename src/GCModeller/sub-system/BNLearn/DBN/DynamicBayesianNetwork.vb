#Region "Microsoft.VisualBasic::92b6330268b82e3b1ab4dd055623c23f, sub-system\BNLearn\DBN\DynamicBayesianNetwork.vb"

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

    '   Total Lines: 995
    '    Code Lines: 591 (59.40%)
    ' Comment Lines: 256 (25.73%)
    '    - Xml Docs: 56.64%
    ' 
    '   Blank Lines: 148 (14.87%)
    '     File Size: 43.30 KB


    '     Class DynamicBayesianNetwork
    ' 
    '         Properties: Config
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: ComputeActivationScore, ComputeDefaultDistribution, ComputeExpectedRNARate, ComputeLogLikelihood, DiscretizeValue
    '                   GetAllNodes, GetGeneNodes, GetMarginalDistribution, GetMetaboliteNodes, GetNode
    '                   GetOperonGeneMapping, GetSummary, GetTFNodes, GetThresholds, PredictNextState
    '                   StateToScore
    ' 
    '         Sub: BuildFromTopology, InitializeCPT, LearnParameters, LoadFromFile, ResetToTopologyPrior
    '              SaveToFile, UpdateParametersOnline
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' File: DynamicBayesianNetwork.vb
'
' Dynamic Bayesian Network (DBN) for Gene Regulatory Network Simulation
' Part of the Virtual Cell Computational Engine
'
' Features:
' 1. Topology-based DBN construction from RegulatoryLink structures
'    (works without RNAseq data - uses biological heuristics for CPT initialization)
' 2. Parameter learning from RNAseq time-series data
'    (topology serves as Dirichlet prior, data refines the parameters)
' 3. Prediction interface for ODEs coupling
'    (metabolites + TF abundances -> gene expression states + RNA abundance changes)
' 4. Bidirectional coupling with metabolic network ODEs
' 5. No third-party dependencies (uses only .NET base class library)
'
' Architecture:
'   - 2-slice Temporal Bayesian Network (2TBN): gene[t+1] depends on TF[t] and metabolite[t]
'   - Discrete states: Low, Medium, High (configurable)
'   - Noisy-OR / Noisy-AND combination for multiple regulators
'   - Dirichlet prior for Bayesian parameter estimation
'
' Coupling Rules:
'   DBN -> ODEs: Predicted gene states map to RNA transcript abundance change rates
'                dR/dt = k_synthesis * E[transcription_rate] - k_degradation * R
'   ODEs -> DBN: Metabolite concentrations and TF abundances are discretized
'                and used as evidence for DBN inference
'
' Requirements:
'   - RegulatoryLink and Effector types must be accessible (same project or global namespace)
'   - .NET Framework 4.0+ or .NET Core/5+ (uses Tuple, LINQ)

Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace DBN

    ' ==================== Dynamic Bayesian Network (Main Class) ====================

    ''' <summary>
    ''' Dynamic Bayesian Network for gene regulatory network simulation.
    ''' 
    ''' This DBN implements a 2-slice temporal Bayesian network (2TBN) where:
    ''' - Gene/operon expression at time t+1 depends on TF and metabolite states at time t
    ''' - TF states are provided as evidence from the ODE solver (TF protein/RNA abundance)
    ''' - Metabolite concentrations are provided as evidence from the ODE solver
    ''' 
    ''' The DBN supports two modes:
    ''' 1. Topology-only mode: Uses RegulatoryLink topology to initialize CPTs based on
    '''    biological heuristics (activator/inhibitor effects via noisy-OR/AND gates).
    '''    No RNAseq data required.
    ''' 2. Data-fitting mode: Uses RNAseq time-series data to learn CPT parameters,
    '''    with the topology-based CPTs serving as a Dirichlet prior.
    ''' 
    ''' Coupling with metabolic network ODEs:
    ''' - DBN -> ODEs: Predicted gene states are mapped to RNA transcript abundance
    '''   change rates (expected transcription rate), which serve as transcription rate
    '''   terms in the ODEs: dR/dt = k_synthesis * rate - k_degradation * R
    ''' - ODEs -> DBN: Metabolite concentrations and TF abundances from the ODEs are
    '''   discretized (Low/Medium/High) and used as evidence for DBN inference.
    ''' </summary>
    Public Class DynamicBayesianNetwork

        ' Internal state
        Private _nodes As New Dictionary(Of String, DBNNode)
        Private _topologyLinks As RegulatoryLink()
        Private _operonGenes As New Dictionary(Of String, List(Of String))
        Private _rng As Random
        Private _config As DBNConfig


        ''' <summary>Configuration for the DBN (discretization, smoothing, rates, etc.)</summary>
        Public Property Config As DBNConfig
            Get
                Return _config
            End Get
            Set(value As DBNConfig)
                _config = value
                If _config IsNot Nothing Then
                    _rng = New Random(_config.Seed)
                End If
            End Set
        End Property

        Public ReadOnly Property topologySize As Integer
            Get
                Return _topologyLinks.Length
            End Get
        End Property

        ''' <summary>Default constructor with default configuration</summary>
        Public Sub New()
            _config = New DBNConfig()
            _rng = New Random(_config.Seed)
        End Sub


        ''' <summary>Constructor with custom configuration</summary>
        Public Sub New(config As DBNConfig)
            _config = config
            If _config Is Nothing Then _config = New DBNConfig()
            _rng = New Random(_config.Seed)
        End Sub


        ' ==================== Build from Topology ====================

        ''' <summary>
        ''' Build the DBN structure from a list of RegulatoryLink objects.
        ''' 
        ''' This method:
        ''' 1. Creates nodes for all TFs, effector metabolites, and target operons
        ''' 2. Sets up parent-child relationships (gene -> TF + effector parents)
        ''' 3. Initializes CPTs based on biological heuristics (noisy-OR/AND gates)
        ''' 
        ''' After calling this method, the DBN is ready for prediction (topology-only mode)
        ''' or parameter learning (data-fitting mode).
        ''' </summary>
        ''' <param name="links">List of regulatory links defining the network topology</param>
        Public Sub BuildFromTopology(links As IEnumerable(Of RegulatoryLink))
            If links Is Nothing Then
                Throw New ArgumentNullException("the gene expression regulator network should not be nothing!")
            Else
                _topologyLinks = links.ToArray
                _nodes.Clear()
                _operonGenes.Clear()
            End If

            ' --- Step 1: Create nodes for all TFs, effector metabolites, and target operons ---
            For Each link In _topologyLinks
                ' Create TF node if not exists
                If Not _nodes.ContainsKey(link.TF_id) Then
                    _nodes(link.TF_id) = New DBNNode(link.TF_id, DBNNodeType.TranscriptionFactor)
                End If

                ' Add effector metabolites to TF node and create metabolite nodes
                If link.effector IsNot Nothing Then
                    For Each kv In link.effector
                        _nodes(link.TF_id).EffectorMetabolites(kv.Key) = kv.Value
                        ' Create metabolite node if not exists
                        If Not _nodes.ContainsKey(kv.Key) Then
                            _nodes(kv.Key) = New DBNNode(kv.Key, DBNNodeType.EffectorMetabolite)
                        End If
                    Next
                End If

                ' Create operon (gene) node if not exists
                If Not _nodes.ContainsKey(link.target_operon) Then
                    _nodes(link.target_operon) = New DBNNode(link.target_operon, DBNNodeType.Gene)
                End If

                ' Store operon-gene mapping
                If link.regulate_genes IsNot Nothing AndAlso link.regulate_genes.Length > 0 Then
                    If Not _operonGenes.ContainsKey(link.target_operon) Then
                        _operonGenes(link.target_operon) = New List(Of String)()
                    End If
                    For Each g In link.regulate_genes
                        If Not _operonGenes(link.target_operon).Contains(g) Then
                            _operonGenes(link.target_operon).Add(g)
                        End If
                    Next
                End If
            Next

            ' --- Step 2: Set up parent-child relationships ---
            For Each link In _topologyLinks
                Dim geneNode = _nodes(link.target_operon)

                ' Add TF as parent (avoid duplicates for multi-effector TFs)
                If Not geneNode.ParentIds.Contains(link.TF_id) Then
                    geneNode.ParentIds.Add(link.TF_id)
                    geneNode.RegulatorTFs.Add(link.TF_id)
                End If

                ' Initialize TFEffectors entry if needed
                If Not geneNode.TFEffectors.ContainsKey(link.TF_id) Then
                    geneNode.TFEffectors(link.TF_id) = New List(Of String)()
                End If

                ' Add effector metabolites as parents and in TFEffectors mapping
                If link.effector IsNot Nothing Then
                    For Each kv In link.effector
                        ' Add metabolite as parent of the gene
                        If Not geneNode.ParentIds.Contains(kv.Key) Then
                            geneNode.ParentIds.Add(kv.Key)
                        End If
                        ' Track which effectors belong to this TF (for this gene)
                        If Not geneNode.TFEffectors(link.TF_id).Contains(kv.Key) Then
                            geneNode.TFEffectors(link.TF_id).Add(kv.Key)
                        End If
                    Next
                End If
            Next

            ' --- Step 3: Initialize CPTs for all nodes ---
            For Each node In _nodes.Values
                InitializeCPT(node)
            Next
        End Sub


        ' ==================== CPT Initialization (Topology-Based Prior) ====================

        ''' <summary>
        ''' Initialize the CPT for a node based on its type and parent relationships.
        ''' For gene nodes, uses biological heuristics (noisy-OR/AND combination of
        ''' activator and inhibitor effects). For other nodes, uses uniform distributions.
        ''' </summary>
        Private Sub InitializeCPT(node As DBNNode)
            node.CPT.VariableId = node.NodeId
            node.CPT.ParentIds = New List(Of String)(node.ParentIds)
            node.CPT.States = New List(Of String)(node.States)
            node.CPT.Table = New Dictionary(Of String, Double())

            If node.ParentIds.Count = 0 Then
                ' No parents: uniform prior
                Dim dist(node.States.Count - 1) As Double
                For i = 0 To dist.Length - 1
                    dist(i) = 1.0 / node.States.Count
                Next
                node.CPT.SetDistribution(New List(Of String), dist)
            Else
                ' Enumerate all parent configurations and compute default distributions
                Dim parentStatesMap As New Dictionary(Of String, List(Of String))
                For Each pid As String In node.ParentIds
                    parentStatesMap(pid) = _nodes(pid).States
                Next

                Dim configs = node.CPT.GetAllParentConfigurations(parentStatesMap)

                For Each cfg In configs
                    Dim dist = ComputeDefaultDistribution(node, cfg)
                    node.CPT.SetDistribution(cfg, dist)
                Next
            End If
        End Sub


        ''' <summary>
        ''' Compute the default probability distribution for a gene node given parent states.
        ''' Uses the activation score to determine the distribution shape:
        ''' - High activation score -> P(High) dominant
        ''' - Moderate score -> P(Medium) dominant
        ''' - Low score (inhibited) -> P(Low) dominant
        ''' </summary>
        Private Function ComputeDefaultDistribution(
            node As DBNNode,
            parentStates As List(Of String)
        ) As Double()

            Dim dist(node.States.Count - 1) As Double

            If node.NodeType = DBNNodeType.Gene AndAlso node.ParentIds.Count > 0 Then
                ' Compute activation score [0, 1]
                Dim score = ComputeActivationScore(node, parentStates)

                Dim idxHigh = node.States.IndexOf("High")
                Dim idxMed = node.States.IndexOf("Medium")
                Dim idxLow = node.States.IndexOf("Low")

                ' Map score to probability distribution using soft thresholds
                If score >= 0.66 Then
                    ' Strong activation
                    dist(idxHigh) = 0.7
                    dist(idxMed) = 0.2
                    dist(idxLow) = 0.1
                ElseIf score >= 0.34 Then
                    ' Moderate / basal expression
                    dist(idxHigh) = 0.25
                    dist(idxMed) = 0.5
                    dist(idxLow) = 0.25
                Else
                    ' Low expression (inhibited)
                    dist(idxHigh) = 0.1
                    dist(idxMed) = 0.2
                    dist(idxLow) = 0.7
                End If
            Else
                ' Non-gene nodes or genes without parents: uniform
                For i = 0 To dist.Length - 1
                    dist(i) = 1.0 / node.States.Count
                Next
            End If

            Return dist
        End Function


        ''' <summary>
        ''' Compute the activation score for a gene given parent (TF and metabolite) states.
        ''' 
        ''' Uses a noisy-OR / noisy-AND combination model:
        ''' - For activators: P(activation) = 1 - prod(1 - p_i)  [noisy-OR]
        ''' - For inhibitors: P(inhibition) = 1 - prod(1 - q_j)  [noisy-OR]
        ''' - Net score = activation - inhibition, mapped to [0, 1]
        ''' 
        ''' Interpretation of Effector enum (net effect on gene expression):
        ''' - Activator: high effector concentration promotes gene activation
        ''' - Inhibitor: high effector concentration promotes gene inhibition
        ''' - Unknown: neutral effect (no contribution)
        ''' 
        ''' For TFs without effectors, uses the TF's DefaultRegulatoryDirection.
        ''' </summary>
        Private Function ComputeActivationScore(
            node As DBNNode,
            parentStates As List(Of String)
        ) As Double

            Dim activationScore = 0.0  ' P(at least one activator is active)
            Dim inhibitionScore = 0.0  ' P(at least one inhibitor is active)
            Dim hasActivator = False
            Dim hasInhibitor = False

            For Each tfId In node.RegulatorTFs
                Dim tfIdx = node.ParentIds.IndexOf(tfId)
                If tfIdx < 0 Then Continue For

                Dim tfState = parentStates(tfIdx)
                Dim tfScore = StateToScore(tfState)  ' Low=0, Medium=0.5, High=1

                ' Get effectors for this TF (in the context of this gene)
                Dim effectorIds As List(Of String) = Nothing
                If node.TFEffectors.ContainsKey(tfId) Then
                    effectorIds = node.TFEffectors(tfId)
                End If

                If effectorIds Is Nothing OrElse effectorIds.Count = 0 Then
                    ' --- No effector: use TF's default regulatory direction ---
                    Dim tfNode = _nodes(tfId)
                    Dim direction = tfNode.DefaultRegulatoryDirection

                    If direction = Effector.Activator Then
                        hasActivator = True
                        ' Noisy-OR: combine with existing activation
                        activationScore = 1 - (1 - activationScore) * (1 - tfScore)
                    ElseIf direction = Effector.Inhibitor Then
                        hasInhibitor = True
                        inhibitionScore = 1 - (1 - inhibitionScore) * (1 - tfScore)
                    End If
                Else
                    ' --- Has effectors: combine TF and effector states ---
                    ' The TF-effector complex requires both TF and effector to be present.
                    ' complexScore = tfScore * effScore (both must be high for strong effect)
                    For Each effId In effectorIds
                        Dim effIdx = node.ParentIds.IndexOf(effId)
                        If effIdx < 0 Then Continue For

                        Dim effState = parentStates(effIdx)
                        Dim effScore = StateToScore(effState)

                        ' Complex formation: both TF and effector needed
                        Dim complexScore = tfScore * effScore

                        ' Get effector type from TF node
                        Dim effType = Effector.Unknown
                        If _nodes(tfId).EffectorMetabolites.ContainsKey(effId) Then
                            effType = _nodes(tfId).EffectorMetabolites(effId)
                        End If

                        Select Case effType
                            Case Effector.Activator
                                hasActivator = True
                                activationScore = 1 - (1 - activationScore) * (1 - complexScore)
                            Case Effector.Inhibitor
                                hasInhibitor = True
                                inhibitionScore = 1 - (1 - inhibitionScore) * (1 - complexScore)
                        End Select
                    Next
                End If
            Next

            ' Compute net score
            Dim netScore As Double
            If hasActivator AndAlso hasInhibitor Then
                ' Both activation and inhibition present: net = activation - inhibition
                netScore = activationScore - inhibitionScore
            ElseIf hasActivator Then
                netScore = activationScore
            ElseIf hasInhibitor Then
                netScore = -inhibitionScore
            Else
                ' No regulation: basal (netScore = 0 -> score = 0.5)
                netScore = 0.0
            End If

            ' Map to [0, 1]: 0.5 = basal, >0.5 = activated, <0.5 = inhibited
            Dim score = 0.5 + 0.5 * netScore
            Return Math.Max(0.0, Math.Min(1.0, score))
        End Function


        ''' <summary>Convert a state string to a numeric score (Low=0, Medium=0.5, High=1)</summary>
        Private Function StateToScore(state As String) As Double
            Select Case state
                Case "Low" : Return 0.0
                Case "Medium" : Return 0.5
                Case "High" : Return 1.0
                Case Else : Return 0.5
            End Select
        End Function


        ' ==================== Parameter Learning (Data-Fitting Mode) ====================

        ''' <summary>
        ''' Learn CPT parameters from RNAseq time-series data.
        ''' 
        ''' Uses Bayesian estimation with Dirichlet prior:
        '''   P(s|parents) = (count(s) + alpha * prior(s)) / (total + alpha)
        ''' where:
        '''   - count(s) is the observed count of state s given the parent configuration
        '''   - alpha is the smoothing parameter (Config.SmoothingAlpha)
        '''   - prior(s) is the topology-based probability (serves as base distribution)
        ''' 
        ''' When alpha is large, the prior (topology) dominates.
        ''' When alpha is small, the data dominates.
        ''' When no data is available for a configuration, the prior is retained.
        ''' </summary>
        ''' <param name="rnaSeqTimeSeries">
        ''' List of time points. Each time point is a dictionary mapping
        ''' node_id (gene/TF/metabolite) -> abundance value (continuous).
        ''' Values should be normalized to [0, 1] for default thresholds,
        ''' or custom thresholds should be provided.
        ''' </param>
        ''' <param name="customThresholds">
        ''' Optional per-node discretization thresholds.
        ''' Key = node ID, Value = Tuple(low_threshold, high_threshold).
        ''' If not provided, uses Config.LowThreshold and Config.HighThreshold.
        ''' </param>
        Public Sub LearnParameters(
            rnaSeqTimeSeries As List(Of Dictionary(Of String, Double)),
            Optional customThresholds As Dictionary(Of String, Tuple(Of Double, Double)) = Nothing
        )
            If rnaSeqTimeSeries Is Nothing OrElse rnaSeqTimeSeries.Count < 2 Then
                Throw New ArgumentException("Need at least 2 time points for parameter learning")
            End If

            ' --- Step 1: Discretize all continuous values ---
            Dim discreteSeries As New List(Of Dictionary(Of String, String))
            For Each tp In rnaSeqTimeSeries
                Dim d As New Dictionary(Of String, String)
                If tp IsNot Nothing Then
                    For Each kv In tp
                        If _nodes.ContainsKey(kv.Key) Then
                            Dim thresh = GetThresholds(kv.Key, customThresholds)
                            d(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                        End If
                    Next
                End If
                discreteSeries.Add(d)
            Next

            ' --- Step 2: Initialize count tables for all nodes with parents ---
            Dim counts As New Dictionary(Of String, Dictionary(Of String, Double()))
            For Each node In _nodes.Values
                If node.ParentIds.Count = 0 Then Continue For

                counts(node.NodeId) = New Dictionary(Of String, Double())
                Dim parentStatesMap As New Dictionary(Of String, List(Of String))
                For Each pid As String In node.ParentIds
                    parentStatesMap(pid) = _nodes(pid).States
                Next
                Dim configs = node.CPT.GetAllParentConfigurations(parentStatesMap)
                For Each cfg In configs
                    Dim key = String.Join("|", cfg)
                    counts(node.NodeId)(key) = New Double(node.States.Count - 1) {}
                Next
            Next

            ' --- Step 3: Count transitions (t -> t+1) ---
            For t = 0 To discreteSeries.Count - 2
                Dim cur = discreteSeries(t)
                Dim nxt = discreteSeries(t + 1)

                For Each node In _nodes.Values
                    If node.ParentIds.Count = 0 Then Continue For
                    If Not nxt.ContainsKey(node.NodeId) Then Continue For

                    ' Get parent states at time t
                    Dim parentStates As New List(Of String)
                    Dim allPresent = True
                    For Each pid As String In node.ParentIds
                        If Not cur.ContainsKey(pid) Then
                            allPresent = False
                            Exit For
                        End If
                        parentStates.Add(cur(pid))
                    Next
                    If Not allPresent Then Continue For

                    Dim key = String.Join("|", parentStates)
                    Dim childState = nxt(node.NodeId)
                    Dim childIdx = node.States.IndexOf(childState)

                    If childIdx >= 0 AndAlso counts(node.NodeId).ContainsKey(key) Then
                        counts(node.NodeId)(key)(childIdx) += 1
                    End If
                Next
            Next

            ' --- Step 4: Update CPTs with Dirichlet posterior ---
            ' P(s|parents) = (count(s) + alpha * prior(s)) / (total + alpha)
            For Each node In _nodes.Values
                If node.ParentIds.Count = 0 Then Continue For

                Dim parentStatesMap As New Dictionary(Of String, List(Of String))
                For Each pid As String In node.ParentIds
                    parentStatesMap(pid) = _nodes(pid).States
                Next
                Dim configs = node.CPT.GetAllParentConfigurations(parentStatesMap)

                For Each cfg In configs
                    Dim key = String.Join("|", cfg)
                    Dim c = counts(node.NodeId)(key)
                    Dim total = c.Sum()

                    ' Get prior distribution from topology-based CPT
                    Dim prior = node.CPT.GetDistribution(cfg)

                    ' Compute posterior
                    Dim newDist(node.States.Count - 1) As Double
                    Dim denom = total + _config.SmoothingAlpha
                    For i = 0 To newDist.Length - 1
                        newDist(i) = (c(i) + _config.SmoothingAlpha * prior(i)) / denom
                    Next

                    node.CPT.SetDistribution(cfg, newDist)
                Next
            Next
        End Sub


        ''' <summary>
        ''' Online parameter update using exponential moving average (EMA).
        ''' Useful for incremental learning during simulation.
        ''' 
        ''' For each transition (current -> next), updates the CPT entry:
        '''   P(s|parents) = (1 - lr) * P_old(s|parents) + lr * target(s)
        ''' where target(s) = 1 if s == observed_next_state, else 0.
        ''' </summary>
        ''' <param name="currentStates">Discrete states at time t (node_id -> state string)</param>
        ''' <param name="nextStates">Discrete states at time t+1 (node_id -> state string)</param>
        Public Sub UpdateParametersOnline(
            currentStates As Dictionary(Of String, String),
            nextStates As Dictionary(Of String, String)
        )
            If currentStates Is Nothing OrElse nextStates Is Nothing Then Return

            For Each node In _nodes.Values
                If node.ParentIds.Count = 0 Then Continue For
                If Not nextStates.ContainsKey(node.NodeId) Then Continue For

                ' Get parent states at time t
                Dim parentStates As New List(Of String)
                Dim allPresent = True
                For Each pid As String In node.ParentIds
                    If Not currentStates.ContainsKey(pid) Then
                        allPresent = False
                        Exit For
                    End If
                    parentStates.Add(currentStates(pid))
                Next
                If Not allPresent Then Continue For

                Dim childIdx = node.States.IndexOf(nextStates(node.NodeId))
                If childIdx < 0 Then Continue For

                ' EMA update
                Dim dist = node.CPT.GetDistribution(parentStates)
                Dim lr = _config.OnlineLearningRate

                For i = 0 To dist.Length - 1
                    Dim target = If(i = childIdx, 1.0, 0.0)
                    dist(i) = (1 - lr) * dist(i) + lr * target
                Next

                node.CPT.SetDistribution(parentStates, dist)
            Next
        End Sub


        ' ==================== Prediction / Inference (ODEs -> DBN) ====================

        ''' <summary>
        ''' Predict the next-state gene expression given current metabolite and TF abundances.
        ''' 
        ''' This is the main interface for ODEs -> DBN coupling.
        ''' 
        ''' Algorithm:
        ''' 1. Discretize metabolite concentrations and TF abundances into Low/Medium/High
        ''' 2. For each gene node, look up CPT: P(gene[t+1] | parents[t])
        ''' 3. Determine predicted state (argmax or sampled)
        ''' 4. Compute expected RNA transcription rate from the distribution
        ''' 
        ''' Usage:
        '''   Dim result = dbn.PredictNextState(metabolites, tfAbundances)
        '''   ' result.GeneStates: operon_id -> "Low"/"Medium"/"High"
        '''   ' result.RNAAbundanceChanges: operon_id -> transcription rate [0, 1]
        ''' </summary>
        ''' <param name="metaboliteAbundances">
        ''' Metabolite ID -> concentration (normalized to [0, 1] recommended).
        ''' These come from the metabolic network ODEs.
        ''' </param>
        ''' <param name="tfAbundances">
        ''' TF ID -> abundance (normalized to [0, 1] recommended).
        ''' These come from the TF protein/RNA ODEs.
        ''' </param>
        ''' <param name="currentGeneStates">
        ''' Optional: current gene states for parent nodes that lack evidence.
        ''' Used when a gene is also a TF (regulated by other TFs).
        ''' </param>
        ''' <returns>Prediction result with gene states, probabilities, and RNA abundance changes</returns>
        Public Function PredictNextState(
            metaboliteAbundances As Dictionary(Of String, Double),
            tfAbundances As Dictionary(Of String, Double),
            Optional currentGeneStates As Dictionary(Of String, String) = Nothing
        ) As DBNPredictionResult

            Dim result As New DBNPredictionResult()

            ' Copy operon-gene mapping to result
            For Each kv In _operonGenes
                result.OperonGeneMapping(kv.Key) = New List(Of String)(kv.Value)
            Next

            ' --- Step 1: Discretize evidence ---
            Dim evidence As New Dictionary(Of String, String)

            If metaboliteAbundances IsNot Nothing Then
                For Each kv In metaboliteAbundances
                    If _nodes.ContainsKey(kv.Key) Then
                        Dim thresh = GetThresholds(kv.Key, Nothing)
                        evidence(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                    End If
                Next
            End If

            If tfAbundances IsNot Nothing Then
                For Each kv In tfAbundances
                    If _nodes.ContainsKey(kv.Key) Then
                        Dim thresh = GetThresholds(kv.Key, Nothing)
                        evidence(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                    End If
                Next
            End If

            ' --- Step 2: For each gene node, compute P(gene[t+1] | parents[t]) ---
            For Each node In _nodes.Values
                If node.NodeType <> DBNNodeType.Gene Then Continue For

                ' Get parent states from evidence or defaults
                Dim parentStates As New List(Of String)
                For Each pid As String In node.ParentIds
                    If evidence.ContainsKey(pid) Then
                        parentStates.Add(evidence(pid))
                    ElseIf currentGeneStates IsNot Nothing AndAlso currentGeneStates.ContainsKey(pid) Then
                        parentStates.Add(currentGeneStates(pid))
                    Else
                        ' Default to Medium when no evidence available
                        parentStates.Add("Medium")
                    End If
                Next

                ' Get CPT distribution
                Dim dist = node.CPT.GetDistribution(parentStates)

                ' Determine predicted state
                Dim predictedState As String
                Dim predictedProb As Double

                If _config.UseMultinomialSampling Then
                    ' Stochastic: sample from distribution
                    Dim r = _rng.NextDouble()
                    Dim cum = 0.0
                    predictedState = node.States(node.States.Count - 1)
                    predictedProb = dist(dist.Length - 1)
                    For i = 0 To dist.Length - 1
                        cum += dist(i)
                        If r <= cum Then
                            predictedState = node.States(i)
                            predictedProb = dist(i)
                            Exit For
                        End If
                    Next
                Else
                    ' Deterministic: take most likely state (argmax)
                    Dim maxIdx = 0
                    For i = 1 To dist.Length - 1
                        If dist(i) > dist(maxIdx) Then maxIdx = i
                    Next
                    predictedState = node.States(maxIdx)
                    predictedProb = dist(maxIdx)
                End If

                ' Store results
                result.GeneStates(node.NodeId) = predictedState
                result.GeneProbabilities(node.NodeId) = CType(dist.Clone(), Double())
                result.GeneStateProbabilities(node.NodeId) = predictedProb
                result.RNAAbundanceChanges(node.NodeId) = ComputeExpectedRNARate(dist, node.States)
            Next

            Return result
        End Function


        ''' <summary>
        ''' Compute the expected RNA transcript abundance change rate from a distribution.
        ''' 
        ''' E[rate] = P(High) * k_high + P(Medium) * k_basal + P(Low) * k_low
        ''' 
        ''' This value represents the expected transcription rate and can be used
        ''' directly in ODE models:
        '''   dR/dt = k_synthesis * E[rate] - k_degradation * R
        ''' </summary>
        Private Function ComputeExpectedRNARate(dist As Double(), states As List(Of String)) As Double
            Dim rate = 0.0
            For i = 0 To dist.Length - 1
                Select Case states(i)
                    Case "High"
                        rate += dist(i) * _config.HighTranscriptionRate
                    Case "Medium"
                        rate += dist(i) * _config.BasalTranscriptionRate
                    Case "Low"
                        rate += dist(i) * _config.LowTranscriptionRate
                End Select
            Next
            Return rate
        End Function


        ' ==================== Model Evaluation ====================

        ''' <summary>
        ''' Compute the log-likelihood of the data given the current model.
        ''' Useful for model evaluation, comparison, and convergence checking.
        ''' </summary>
        Public Function ComputeLogLikelihood(
            rnaSeqTimeSeries As List(Of Dictionary(Of String, Double)),
            Optional customThresholds As Dictionary(Of String, Tuple(Of Double, Double)) = Nothing
        ) As Double

            If rnaSeqTimeSeries Is Nothing OrElse rnaSeqTimeSeries.Count < 2 Then Return 0.0

            ' Discretize
            Dim discreteSeries As New List(Of Dictionary(Of String, String))
            For Each tp In rnaSeqTimeSeries
                Dim d As New Dictionary(Of String, String)
                If tp IsNot Nothing Then
                    For Each kv In tp
                        If _nodes.ContainsKey(kv.Key) Then
                            Dim thresh = GetThresholds(kv.Key, customThresholds)
                            d(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                        End If
                    Next
                End If
                discreteSeries.Add(d)
            Next

            Dim logLik = 0.0
            For t = 0 To discreteSeries.Count - 2
                Dim cur = discreteSeries(t)
                Dim nxt = discreteSeries(t + 1)

                For Each node In _nodes.Values
                    If node.ParentIds.Count = 0 Then Continue For
                    If Not nxt.ContainsKey(node.NodeId) Then Continue For

                    Dim parentStates As New List(Of String)
                    Dim allPresent = True
                    For Each pid As String In node.ParentIds
                        If Not cur.ContainsKey(pid) Then
                            allPresent = False
                            Exit For
                        End If
                        parentStates.Add(cur(pid))
                    Next
                    If Not allPresent Then Continue For

                    Dim dist = node.CPT.GetDistribution(parentStates)
                    Dim childIdx = node.States.IndexOf(nxt(node.NodeId))
                    If childIdx >= 0 Then
                        Dim p = Math.Max(dist(childIdx), 0.001)  ' Avoid log(0)
                        logLik += Math.Log(p)
                    End If
                Next
            Next

            Return logLik
        End Function


        ' ==================== Utility Methods ====================

        ''' <summary>Get discretization thresholds for a node</summary>
        Private Function GetThresholds(
            nodeId As String,
            customThresholds As Dictionary(Of String, Tuple(Of Double, Double))
        ) As Tuple(Of Double, Double)
            If customThresholds IsNot Nothing AndAlso customThresholds.ContainsKey(nodeId) Then
                Return customThresholds(nodeId)
            End If
            Return New Tuple(Of Double, Double)(_config.LowThreshold, _config.HighThreshold)
        End Function


        ''' <summary>Discretize a continuous value into Low/Medium/High</summary>
        Private Function DiscretizeValue(
            value As Double,
            lowThreshold As Double,
            highThreshold As Double
        ) As String
            If value < lowThreshold Then Return "Low"
            If value < highThreshold Then Return "Medium"
            Return "High"
        End Function


        ''' <summary>Get a node by ID (returns Nothing if not found)</summary>
        Public Function GetNode(id As String) As DBNNode
            If _nodes.ContainsKey(id) Then Return _nodes(id)
            Return Nothing
        End Function


        ''' <summary>Get all nodes in the network</summary>
        Public Function GetAllNodes() As List(Of DBNNode)
            Return _nodes.Values.ToList()
        End Function


        ''' <summary>Get all gene/operon nodes</summary>
        Public Function GetGeneNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.Gene).ToList()
        End Function


        ''' <summary>Get all transcription factor nodes</summary>
        Public Function GetTFNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.TranscriptionFactor).ToList()
        End Function


        ''' <summary>Get all effector metabolite nodes</summary>
        Public Function GetMetaboliteNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.EffectorMetabolite).ToList()
        End Function


        ''' <summary>Get the operon-to-genes mapping</summary>
        Public Function GetOperonGeneMapping() As Dictionary(Of String, List(Of String))
            Return New Dictionary(Of String, List(Of String))(_operonGenes)
        End Function


        ''' <summary>
        ''' Compute the marginal distribution of a node (averaging over all parent configs).
        ''' Useful for initialization and debugging.
        ''' </summary>
        Public Function GetMarginalDistribution(nodeId As String) As Double()
            Dim node = GetNode(nodeId)
            If node Is Nothing Then Return Nothing

            If node.ParentIds.Count = 0 Then
                Return node.CPT.GetDistribution(New List(Of String))
            End If

            Dim marginal(node.States.Count - 1) As Double
            Dim parentStatesMap As New Dictionary(Of String, List(Of String))
            For Each pid As String In node.ParentIds
                parentStatesMap(pid) = _nodes(pid).States
            Next

            Dim configs = node.CPT.GetAllParentConfigurations(parentStatesMap)
            For Each cfg In configs
                Dim dist = node.CPT.GetDistribution(cfg)
                For i = 0 To marginal.Length - 1
                    marginal(i) += dist(i) / configs.Count
                Next
            Next

            Return marginal
        End Function


        ''' <summary>Reset all CPTs to topology-based defaults (discard learned parameters)</summary>
        Public Sub ResetToTopologyPrior()
            For Each node In _nodes.Values
                InitializeCPT(node)
            Next
        End Sub


        ''' <summary>Get a summary string of the DBN structure (for debugging)</summary>
        Public Function GetSummary() As String
            Dim sb As New StringBuilder()
            sb.AppendLine("=== Dynamic Bayesian Network Summary ===")
            sb.AppendLine(String.Format("Total nodes: {0}", _nodes.Count))
            sb.AppendLine(String.Format("  Genes/Operons: {0}", GetGeneNodes().Count))
            sb.AppendLine(String.Format("  Transcription Factors: {0}", GetTFNodes().Count))
            sb.AppendLine(String.Format("  Effector Metabolites: {0}", GetMetaboliteNodes().Count))
            sb.AppendLine()
            sb.AppendLine("Node Details:")
            For Each node In _nodes.Values
                sb.AppendLine(String.Format("  [{0}] {1} (parents: {2})",
                    node.NodeType.ToString(),
                    node.NodeId,
                    If(node.ParentIds.Count = 0, "none", String.Join(", ", node.ParentIds))))
            Next
            Return sb.ToString()
        End Function


        ' ==================== Persistence (Save / Load) ====================

        ''' <summary>
        ''' Save the DBN (structure + learned parameters) to a text file.
        ''' Format: simple pipe-delimited text, human-readable.
        ''' </summary>
        Public Sub SaveToFile(filePath As String)
            Using writer As New StreamWriter(filePath)
                writer.WriteLine("# Dynamic Bayesian Network")
                writer.WriteLine("# Format: DBN_V1")
                writer.WriteLine()

                ' Write nodes
                writer.WriteLine("NODES {0}", _nodes.Count)
                For Each node In _nodes.Values
                    writer.WriteLine("NODE|{0}|{1}|{2}|{3}",
                        node.NodeId,
                        CInt(node.NodeType),
                        String.Join(",", node.States),
                        String.Join(",", node.ParentIds))
                Next
                writer.WriteLine()

                ' Write CPTs
                writer.WriteLine("CPTS")
                For Each node In _nodes.Values
                    For Each kv In node.CPT.Table
                        writer.WriteLine("CPT|{0}|{1}|{2}",
                            node.NodeId,
                            kv.Key,
                            String.Join(",", kv.Value.Select(
                                Function(d) d.ToString("G6", CultureInfo.InvariantCulture))))
                    Next
                Next
            End Using
        End Sub


        ''' <summary>Load the DBN from a text file (saved by SaveToFile)</summary>
        Public Sub LoadFromFile(filePath As String)
            _nodes.Clear()
            _operonGenes.Clear()

            Dim lines = File.ReadAllLines(filePath)

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("#") Then Continue For

                Dim parts = line.Split("|"c)
                If parts.Length = 0 Then Continue For

                Select Case parts(0)
                    Case "NODE"
                        If parts.Length >= 5 Then
                            Dim id = parts(1)
                            Dim type = CType(Integer.Parse(parts(2)), DBNNodeType)
                            Dim states = parts(3).Split(","c).ToList()
                            Dim parents = If(String.IsNullOrEmpty(parts(4)),
                                New List(Of String)(),
                                parts(4).Split(","c).ToList())

                            Dim node As New DBNNode(id, type)
                            node.States = states
                            node.ParentIds = parents
                            node.CPT.VariableId = id
                            node.CPT.ParentIds = parents
                            node.CPT.States = states
                            _nodes(id) = node
                        End If

                    Case "CPT"
                        If parts.Length >= 4 Then
                            Dim nodeId = parts(1)
                            Dim key = parts(2)
                            Dim probs = parts.Last.Split(","c).Select(Function(s) Double.Parse(s, CultureInfo.InvariantCulture)).ToArray()

                            If _nodes.ContainsKey(nodeId) Then
                                _nodes(nodeId).CPT.Table(key) = probs
                            End If
                        End If
                End Select
            Next
        End Sub

    End Class


End Namespace

