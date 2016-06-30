Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataStructures.BinaryTree

Namespace gplots

    Public Module DendrogramAPI

        <Extension>
        Public Function ClusterParts(Of T)(tree As BinaryTree(Of T)) As Dictionary(Of String, String())
            Return tree.ClusterParts(AddressOf IsLeaf, AddressOf IsLeafX, AddressOf GetEntities)
        End Function

        Public Function IsLeaf(Of T)(x As TreeNode(Of T)) As Boolean
            Throw New NotImplementedException
        End Function

        Public Function IsLeafX(Of T)(x As TreeNode(Of T)) As Boolean
            Throw New NotImplementedException
        End Function

        Public Function GetEntities(Of T)(x As TreeNode(Of T)) As String()
            Throw New NotImplementedException
        End Function
    End Module
End Namespace