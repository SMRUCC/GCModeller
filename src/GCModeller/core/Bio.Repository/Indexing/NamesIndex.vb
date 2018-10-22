Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree

''' <summary>
''' 对对象名称建立索引
''' </summary>
''' <typeparam name="T"></typeparam>
Public Class NamesIndex(Of T)

    ReadOnly tree As BinaryTree(Of String, T)

    Public Sub New(repository As NamesFactory(Of T))
        Dim tree As New AVLTree(Of String, T)(AddressOf String.Compare)

        For Each obj As T In repository.PopulateObjects
            For Each name As String In repository.GetNames(obj)
                Call tree.Add(name, obj, False)
            Next
        Next

        Me.tree = tree.root
    End Sub

    Public Function GetValueByName(name As String) As IEnumerable(Of T)
        Dim node As BinaryTree(Of String, T) = tree.Find(name, AddressOf String.Compare)

        If Not node Is Nothing Then
            Return node!values
        Else
            Return {}
        End If
    End Function
End Class
