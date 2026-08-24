Namespace DBN



    ' ==================== DBN Node ====================

    ''' <summary>
    ''' A node in the Dynamic Bayesian Network.
    ''' Represents a gene/operon, transcription factor, or effector metabolite.
    ''' </summary>
    Public Class DBNNode

        ''' <summary>Unique node identifier (matches TF_id, target_operon, or metabolite ID)</summary>
        Public Property NodeId As String

        ''' <summary>Type of this node (Gene, TranscriptionFactor, or EffectorMetabolite)</summary>
        Public Property NodeType As DBNNodeType

        ''' <summary>Discrete states (default: Low, Medium, High)</summary>
        Public Property States As New List(Of String) From {"Low", "Medium", "High"}

        ''' <summary>
        ''' Parent node IDs at time t that influence this node at time t+1.
        ''' For gene nodes: the TFs and effector metabolites that regulate this gene.
        ''' </summary>
        Public Property ParentIds As New List(Of String)

        ''' <summary>Conditional Probability Table for this node</summary>
        Public Property CPT As ConditionalProbabilityTable

        ''' <summary>
        ''' For TF nodes: effector metabolites and their effect types.
        ''' Key = metabolite ID, Value = effect type (Activator/Inhibitor/Unknown).
        ''' Populated from RegulatoryLink.effector.
        ''' </summary>
        Public Property EffectorMetabolites As New Dictionary(Of String, Effector)

        ''' <summary>
        ''' For TF nodes without effectors: default regulatory direction.
        ''' Used when a TF regulates a gene directly without an effector metabolite.
        ''' Default is Activator.
        ''' </summary>
        Public Property DefaultRegulatoryDirection As Effector = Effector.Activator

        ''' <summary>For gene nodes: list of TF IDs that regulate this gene</summary>
        Public Property RegulatorTFs As New List(Of String)

        ''' <summary>
        ''' For gene nodes: mapping from TF ID to its effector metabolite IDs.
        ''' Key = TF ID, Value = list of effector metabolite IDs for that TF.
        ''' Used during CPT initialization to compute activation scores.
        ''' </summary>
        Public Property TFEffectors As New Dictionary(Of String, List(Of String))


        Public Sub New(id As String, type As DBNNodeType)
            NodeId = id
            NodeType = type
            CPT = New ConditionalProbabilityTable()
        End Sub

    End Class


End Namespace