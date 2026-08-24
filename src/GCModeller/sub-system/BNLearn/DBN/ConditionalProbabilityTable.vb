Namespace DBN


    ' ==================== Conditional Probability Table ====================

    ''' <summary>
    ''' Conditional Probability Table (CPT) for a DBN node.
    ''' Stores P(node_state | parent_states) for all combinations of parent states.
    ''' 
    ''' The table is indexed by a string key formed by joining parent state values
    ''' with the "|" separator. Each entry contains a probability distribution over
    ''' the child node's states.
    ''' </summary>
    Public Class ConditionalProbabilityTable

        ''' <summary>ID of the variable (child node) this CPT belongs to</summary>
        Public Property VariableId As String

        ''' <summary>Ordered list of parent node IDs (order matters for key construction)</summary>
        Public Property ParentIds As New List(Of String)

        ''' <summary>Discrete states of the child node (e.g., "Low", "Medium", "High")</summary>
        Public Property States As New List(Of String)

        ''' <summary>
        ''' Probability table: key = "|"-joined parent states, value = probability array.
        ''' The array is aligned with the States list.
        ''' </summary>
        Public Property Table As New Dictionary(Of String, Double())


        ''' <summary>Build the lookup key from an ordered list of parent state values</summary>
        Public Function GetKey(parentStates As List(Of String)) As String
            Return String.Join("|", parentStates)
        End Function


        ''' <summary>
        ''' Get the probability distribution over child states given a parent configuration.
        ''' Returns a cloned array (safe to modify).
        ''' If the configuration is not in the table, returns a uniform distribution.
        ''' </summary>
        Public Function GetDistribution(parentStates As List(Of String)) As Double()
            Dim key = GetKey(parentStates)
            If Table.ContainsKey(key) Then
                Return CType(Table(key).Clone(), Double())
            End If
            ' Fallback: uniform distribution
            Dim uniform(States.Count - 1) As Double
            For i = 0 To uniform.Length - 1
                uniform(i) = 1.0 / States.Count
            Next
            Return uniform
        End Function


        ''' <summary>Set the probability distribution for a parent configuration</summary>
        Public Sub SetDistribution(parentStates As List(Of String), distribution As Double())
            Dim key = GetKey(parentStates)
            Table(key) = CType(distribution.Clone(), Double())
        End Sub


        ''' <summary>
        ''' Enumerate all possible parent state combinations (Cartesian product).
        ''' Used for CPT initialization and parameter learning.
        ''' </summary>
        Public Function GetAllParentConfigurations(
            parentStatesMap As Dictionary(Of String, List(Of String))
        ) As List(Of List(Of String))

            Dim configs As New List(Of List(Of String))
            configs.Add(New List(Of String))  ' Start with one empty configuration

            For Each pid As String In ParentIds
                If Not parentStatesMap.ContainsKey(pid) Then Continue For
                Dim pStates = parentStatesMap(pid)
                Dim newConfigs As New List(Of List(Of String))
                For Each cfg In configs
                    For Each s In pStates
                        Dim newCfg = New List(Of String)(cfg)
                        newCfg.Add(s)
                        newConfigs.Add(newCfg)
                    Next
                Next
                configs = newConfigs
            Next

            Return configs
        End Function

    End Class

End Namespace