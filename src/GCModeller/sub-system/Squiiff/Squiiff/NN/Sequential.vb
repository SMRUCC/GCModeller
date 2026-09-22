Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace NN

    ''' <summary>
    ''' 顺序层容器：前向按注册顺序执行，反向按逆序执行。
    ''' 每层自行缓存反向所需的中间量，因此不需要容器额外保存激活值。
    ''' </summary>
    Public Class Sequential
        Inherits LayerModule

        Private ReadOnly _name As String
        Private ReadOnly _layers As New List(Of LayerModule)

        Public Sub New(name As String, ParamArray layers As LayerModule())
            Me._name = name
            If layers IsNot Nothing Then
                For Each layer In layers
                    If layer IsNot Nothing Then Me._layers.Add(layer)
                Next
            End If
        End Sub

        Public Overrides ReadOnly Property Name As String
            Get
                Return _name
            End Get
        End Property

        ''' <summary>容器内的层列表（追加顺序即前向顺序）。</summary>
        Public ReadOnly Property Layers As IReadOnlyList(Of LayerModule)
            Get
                Return _layers
            End Get
        End Property

        ''' <summary>追加一层并返回自身（便于链式构建）。</summary>
        Public Function Add(layer As LayerModule) As Sequential
            Me._layers.Add(layer)
            Return Me
        End Function

        Public Overrides ReadOnly Property Parameters As IEnumerable(Of Parameter)
            Get
                Dim bag As New List(Of Parameter)
                For Each layer In _layers
                    For Each p In layer.Parameters
                        bag.Add(p)
                    Next
                Next
                Return bag
            End Get
        End Property

        Public Overrides Function Forward(x As Tensor, training As Boolean) As Tensor
            Dim h = x
            For Each layer In _layers
                h = layer.Forward(h, training)
            Next
            Return h
        End Function

        Public Overrides Function Backward(dOut As Tensor) As Tensor
            Dim d = dOut
            For i As Integer = _layers.Count - 1 To 0 Step -1
                d = _layers(i).Backward(d)
            Next
            Return d
        End Function
    End Class
End Namespace
