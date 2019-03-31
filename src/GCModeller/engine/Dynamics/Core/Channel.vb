Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

''' <summary>
''' 反应过程通道
''' </summary>
Public Class Channel : Implements INamedValue

    Friend left As Variable()
    Friend right As Variable()

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

    Public Function CoverLeft(shares As Dictionary(Of String, Double), regulation As Double) As Double
        Return minimalUnit(shares, left, regulation)
    End Function

    Public Function CoverRight(shares As Dictionary(Of String, Double), regulation As Double) As Double
        Return minimalUnit(shares, right, regulation)
    End Function

    Public Property ID As String Implements IKeyedEntity(Of String).Key

    Sub New(left As IEnumerable(Of Variable), right As IEnumerable(Of Variable))
        Me.left = left.ToArray
        Me.right = right.ToArray
    End Sub

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
    Private Shared Function minimalUnit(parallel As Dictionary(Of String, Double), factors As IEnumerable(Of Variable), regulation As Double) As Double
        Return factors _
            .Select(Function(v)
                        Dim r = regulation * v.Coefficient
                        Dim shares# = parallel(v.Mass.ID)
                        Dim massUnit = v.Mass.Value / shares

                        If r > massUnit Then
                            ' 消耗的已经超过了当前的容量
                            ' 则最小的反应单位是当前的物质容量

                            ' 如果某一个物质的容量是零，则表示没有反应物可以被利用了
                            ' 则计算出来的最小反应单位是零
                            ' 即此反应过程不可能会发生
                            Return massUnit / v.Coefficient
                        Else ' 能够正常的以当前的反应单位进行
                            Return regulation
                        End If
                    End Function) _
            .Min
    End Function

End Class