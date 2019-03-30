Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Language

''' <summary>
''' 反应过程通道
''' </summary>
Public Class Channel

    Dim left As Variable()
    Dim right As Variable()

    Public Property Forward As Regulation
    Public Property Reverse As Regulation

    Public ReadOnly Property Direction As Directions
        Get
            If Forward > Reverse Then
                Return Directions.LeftToRight
            ElseIf Reverse > Forward Then
                Return Directions.RightToLeft
            Else
                Return Directions.Stop
            End If
        End Get
    End Property

    Public ReadOnly Property CoverLeft(Optional regulation As Double = 1) As Double
        Get
            Return minimalUnit(left, regulation)
        End Get
    End Property

    Public ReadOnly Property CoverRight(Optional regulation As Double = 1) As Double
        Get
            Return minimalUnit(right, regulation)
        End Get
    End Property

    Public Sub Transition(regulation As Double, dir As Directions)
        regulation = regulation * dir

        For Each mass In left
            mass.Mass.Value -= regulation * mass.Coefficient
        Next
        For Each mass In right
            mass.Mass.Value += regulation * mass.Coefficient
        Next
    End Sub

    ''' <summary>
    ''' 得到当前的物质内容所能够支撑的最小反应单位
    ''' </summary>
    ''' <param name="factors"></param>
    ''' <param name="regulation"></param>
    ''' <returns></returns>
    Private Shared Function minimalUnit(factors As IEnumerable(Of Variable), regulation As Double) As Double
        Return factors _
            .Select(Function(v)
                        Dim r = regulation * v.Coefficient

                        If r > v.Mass.Value Then
                            ' 消耗的已经超过了当前的容量
                            ' 则最小的反应单位是当前的物质容量

                            ' 如果某一个物质的容量是零，则表示没有反应物可以被利用了
                            ' 则计算出来的最小反应单位是零
                            ' 即此反应过程不可能会发生
                            Return v.Mass.Value / v.Coefficient
                        Else ' 能够正常的以当前的反应单位进行
                            Return regulation
                        End If
                    End Function) _
            .Min
    End Function

End Class

Public Enum Directions
    ''' <summary>
    ''' 反应过程将不会发生
    ''' </summary>
    [Stop] = 0
    LeftToRight = 1
    RightToLeft = -1
End Enum

''' <summary>
''' 对反应过程的某一个方向的控制效应
''' </summary>
Public Class Regulation

    Public Property Activation As Variable()
    ''' <summary>
    ''' 如果抑制的总量大于激活的总量，那么这个调控的反应过程将不会进行
    ''' </summary>
    ''' <returns></returns>
    Public Property Inhibition As Variable()

    ''' <summary>
    ''' 计算出当前的调控效应单位
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Coefficient As Double
        Get
            Dim i = Inhibition.Sum(Function(v) v.Coefficient * v.Mass.Value)
            Dim a = Activation.Sum(Function(v) v.Coefficient * v.Mass.Value)

            If i >= a Then
                ' 抑制的总量已经大于等于激活的总量的，则反应过程不会发生
                Return 0
            Else
                Return a - i
            End If
        End Get
    End Property

    Public Shared Operator >(a As Regulation, b As Regulation) As Boolean
        Return a.Coefficient > b.Coefficient
    End Operator

    Public Shared Operator <(a As Regulation, b As Regulation) As Boolean
        Return a.Coefficient < b.Coefficient
    End Operator

    Public Shared Operator =(a As Regulation, b As Regulation) As Boolean
        Return a.Coefficient = b.Coefficient
    End Operator

    Public Shared Operator <>(a As Regulation, b As Regulation) As Boolean
        Return a.Coefficient <> b.Coefficient
    End Operator

End Class

Public Class Variable

    ''' <summary>
    ''' 对反应容器之中的某一种物质的引用
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Mass As Factor
    ''' <summary>
    ''' 在反应过程之中的变异系数，每完成一个单位的反应过程，当前的<see cref="Mass"/>将会丢失或者增加这个系数相对应的数量的含量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Coefficient As Double

    Public Overrides Function ToString() As String
        Return Mass.ToString
    End Function

End Class

''' <summary>
''' 一个变量因子，这个对象主要是用于存储值
''' </summary>
Public Class Factor : Inherits Value(Of Double)
    Implements INamedValue

    Public Property ID As String Implements IKeyedEntity(Of String).Key

    Public Overrides Function ToString() As String
        Return $"{ID} ({Value} unit)"
    End Function
End Class

''' <summary>
''' 一个反应容器，也是一个微环境，这在这个反应容器之中包含有所有的反应过程
''' </summary>
Public Class Vessel

    ''' <summary>
    ''' 当前的这个微环境之中的所有的反应过程列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Channels As Channel()
    ''' <summary>
    ''' 当前的这个微环境之中的所有的物质列表
    ''' </summary>
    ''' <returns></returns>
    Public Property Mass As Factor()

    ''' <summary>
    ''' 当前的这个微环境的迭代器
    ''' </summary>
    Public Sub ContainerIterator()
        For Each reaction As Channel In Channels
            ' 不可以使用Where直接在for循环外进行筛选
            ' 因为环境是不断地变化的
            Select Case reaction.Direction
                Case Directions.LeftToRight
                    ' 消耗左边，产生右边
                    Dim regulate = reaction.Forward.Coefficient

                    If regulate > 0 Then
                        ' 当前是具有调控效应的
                        ' 接着计算最小的反应单位
                        regulate = reaction.CoverLeft(regulate)
                    End If
                    If regulate > 0 Then
                        ' 当前的过程是可以进行的
                        ' 则进行物质的转义的计算
                        Call reaction.Transition(regulate, Directions.LeftToRight)
                    End If
                Case Directions.RightToLeft
                    Dim regulate = reaction.Reverse.Coefficient

                    If regulate > 0 Then
                        regulate = reaction.CoverRight(regulate)
                    End If
                    If regulate > 0 Then
                        Call reaction.Transition(regulate, Directions.RightToLeft)
                    End If
                Case Else
                    ' no reaction will be happends
            End Select
        Next
    End Sub
End Class