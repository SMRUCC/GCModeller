Imports Microsoft.VisualBasic.ComponentModel.DataStructures.Tree

Namespace Core.Cache

    Public Class FileNode : Inherits TreeNodeBase(Of FileNode)
        Implements ITreeNode(Of FileNode)

        Public ReadOnly Property File As CachedFile

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name">FileName</param>
        Public Sub New(name As String)
            MyBase.New(name)
        End Sub

        Public ReadOnly Property isDirectory As Boolean
            Get
                Return ChildNodes.Count > 0
            End Get
        End Property

        Public Overrides ReadOnly Property MySelf As FileNode
            Get
                Return Me
            End Get
        End Property
    End Class
End Namespace