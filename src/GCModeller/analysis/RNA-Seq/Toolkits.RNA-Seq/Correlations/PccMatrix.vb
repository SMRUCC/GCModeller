#Region "Microsoft.VisualBasic::97bd594c778fcea43953e16d71b6e10c, ..\GCModeller\analysis\RNA-Seq\Toolkits.RNA-Seq\Correlations\PccMatrix.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT

''' <summary>
''' Pearson correlation coefficient calculator.
''' (因为为了查找字典方便，所以里面的所有的编号都已经被转换为大写形式了，在查找的时候应该要注意)
''' </summary>
''' <remarks></remarks>
''' 
Public Class PccMatrix

    Implements IDisposable
    Implements IReadOnlyCollection(Of ExprSamples)
    Implements IEnumerable(Of ExprSamples)
    Implements IWeightPaired

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Dim _pccValues As Dictionary(Of String, ExprSamples)

    Public ReadOnly Property PccValues As ExprSamples()
        Get
            Return _pccValues.Values.ToArray
        End Get
    End Property

    ''' <summary>
    ''' 当前的这个矩阵对象是否为皮尔森系数和斯皮尔曼相关性系数的混合矩阵？
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PCC_SPCC_MixedType As Boolean = False

    ''' <summary>
    ''' 顺序是与<see cref="_pccValues"></see>之中的对象是一一对应的
    ''' </summary>
    ''' <returns></returns>
    Public Property lstGenes As String()
        Get
            Return __ordinalHash.Keys.ToArray
        End Get
        Protected Set(value As String())
            If value Is Nothing Then
                __ordinalHash = New Dictionary(Of String, Integer)
            Else
                __ordinalHash = value.ToArray(
                    Function(id, i) New KeyValuePair(Of String, Integer)(id, i)).ToDictionary
            End If
        End Set
    End Property

    Dim __ordinalHash As Dictionary(Of String, Integer)

    Protected Sub New()
    End Sub

    Sub New(samples As Dictionary(Of String, ExprSamples), lstId As String())
        _pccValues = samples
        lstGenes = lstId
    End Sub

    Sub New(Samples As Generic.IEnumerable(Of ExprSamples))
        Dim dict = Samples.ToDictionary(Function(sample) sample.locusId)

        _pccValues = dict
        lstGenes = dict.Keys.ToArray
    End Sub

    Public Function SaveTo(path As String) As Boolean
        Return Not MatrixAPI.SavePccMatrix(Me, SaveTo:=path) Is Nothing
    End Function

    Public Sub Filtering(IdList As String())
        Dim LQuery = (From item In Me._pccValues.AsParallel Where Array.IndexOf(IdList, item.Key) > -1 Select item).ToArray
        Dim NewDict As Dictionary(Of String, ExprSamples) = New Dictionary(Of String, ExprSamples)
        For Each PccItem In LQuery
            Call NewDict.Add(PccItem.Key, PccItem.Value)
        Next
        Me._pccValues = NewDict
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 检测冗余的调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO:  释放托管状态(托管对象)。
            End If

            ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
            ' TODO:  将大型字段设置为 null。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码是为了正确实现可处置模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    Public Iterator Function GetEnumerator() As IEnumerator(Of ExprSamples) Implements IEnumerable(Of ExprSamples).GetEnumerator
        For Each item As KeyValuePair(Of String, ExprSamples) In _pccValues
            Yield item.Value
        Next
    End Function

    Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of ExprSamples).Count
        Get
            Return Me._pccValues.Count
        End Get
    End Property

    Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function

    ''' <summary>
    ''' Get the pcc value between the specified two gene object.(获取任意两个基因之间的Pcc系数，请注意，所有的编号应该是大写的)
    ''' </summary>
    ''' <param name="Id1"></param>
    ''' <param name="Id2"></param>
    ''' <param name="Parallel">本参数无任何用处，仅仅是为了保持接口的统一性而设置的</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValue(Id1 As String, Id2 As String, Optional Parallel As Boolean = True) As Double Implements IWeightPaired.GetValue
        Dim key1 As String = Id1.ToUpper

        If Not _pccValues.ContainsKey(key1) Then
            Return 0
        End If

        If __ordinalHash.ContainsKey(Id2) Then
            Dim idx As Integer = __ordinalHash(Id2)
            Dim value As Double = _pccValues(key1).Values(idx)
            Return value
        Else
            Return 0
        End If
    End Function
End Class
