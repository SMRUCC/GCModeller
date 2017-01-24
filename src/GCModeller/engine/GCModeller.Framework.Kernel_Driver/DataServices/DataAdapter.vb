#Region "Microsoft.VisualBasic::c3157b1062a8dadd4ede0c81bc7e123d, ..\GCModeller\engine\GCModeller.Framework.Kernel_Driver\DataServices\DataAdapter.vb"

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

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.LDM

Namespace Kernel

    ''' <summary>
    ''' The data reading adapter for the GCModeller calculation engine.(计算引擎的数据采集卡)
    ''' </summary>
    ''' <typeparam name="ModelType"></typeparam>
    ''' <remarks></remarks>
    Public MustInherit Class DataAdapter(Of ModelType As ModelBaseType)

        Protected Friend _kernelModule As Framework.Kernel_Driver.IterationMathEngine(Of ModelType)
        ''' <summary>
        ''' {RTime, {Handle, Value}}，每一个对象都是按照Handle排列的
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _innerBuffer As List(Of KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())) =
            New List(Of KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)()))

        Sub New(Kernel As Framework.Kernel_Driver.IterationMathEngine(Of ModelType))
            Me._kernelModule = Kernel
            Kernel.__runDataAdapter = Sub() Call Me.Tick()
        End Sub

        Public Overridable Function Tick() As Integer
            Call Me._innerBuffer.Add(GetDataPackage)
            Return 0
        End Function

        ''' <summary>
        ''' {RTime, {Handle, Value}}()，每一个元素都是按照Handle排列顺序的
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function GetDataPackage() As KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())
            Dim LQuery = (From Equation As Expression In Me._kernelModule.get_Expressions
                          Let Data0Expr = New KeyValuePair(Of Long, Double)(Equation.Handle, Equation.Value)
                          Select Data0Expr).ToArray
            Dim DeltaData = New KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())(_kernelModule.RuntimeTicks, LQuery)
            Return DeltaData
        End Function

        Public Overridable Function CreateDataPackage() As IO.File
            Dim CsvData As New IO.File
            Call CsvData.Add(New String() {"RTimeTicks"})
            Call CsvData.First.AddRange(__getHeaders)
            Dim LQuery = (From item In Me._innerBuffer.AsParallel
                          Select RowData = __createRow(item)
                          Order By RowData.First Ascending).ToArray
            Call CsvData.AppendRange(LQuery)

            Return CsvData
        End Function

        Private Shared Function __createRow(x As KeyValuePair(Of Integer, KeyValuePair(Of Long, Double)())) As IO.RowObject
            Dim Row As IO.RowObject = New String() {}
            Call Row.Add(x.Key)
            Call Row.AddRange((From it In x.Value Select CStr(it.Value)).ToArray)
            Return Row
        End Function

        ''' <summary>
        ''' 按照Handle顺序排列Headers字符串
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend MustOverride Function __getHeaders() As String()
    End Class
End Namespace
