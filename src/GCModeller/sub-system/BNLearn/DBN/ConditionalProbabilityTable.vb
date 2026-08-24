#Region "Microsoft.VisualBasic::cc089f7c114d8920842d25f591fdda0b, sub-system\BNLearn\DBN\ConditionalProbabilityTable.vb"

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

    '   Total Lines: 94
    '    Code Lines: 46 (48.94%)
    ' Comment Lines: 28 (29.79%)
    '    - Xml Docs: 89.29%
    ' 
    '   Blank Lines: 20 (21.28%)
    '     File Size: 3.79 KB


    '     Class ConditionalProbabilityTable
    ' 
    '         Properties: ParentIds, States, Table, VariableId
    ' 
    '         Function: GetAllParentConfigurations, GetDistribution, GetKey
    ' 
    '         Sub: SetDistribution
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
