#Region "Microsoft.VisualBasic::741234f8f3c4d2355894e25f569ba427, annotations\Bifrost\Prodigal\Models\PredictionResult.vb"

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

    '   Total Lines: 21
    '    Code Lines: 9 (42.86%)
    ' Comment Lines: 7 (33.33%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (23.81%)
    '     File Size: 540 B


    ' Class PredictionResult
    ' 
    '     Properties: Genes, Model, SeqId, SeqLength
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' 基因预测结果
''' </summary>
Public Class PredictionResult
    ''' <summary>序列ID</summary>
    Public Property SeqId As String

    ''' <summary>序列长度</summary>
    Public Property SeqLength As Integer

    ''' <summary>预测的基因列表</summary>
    Public Property Genes As List(Of PredictedGene)

    ''' <summary>使用的训练模型</summary>
    Public Property Model As TrainingModel

    Public Sub New()
        Genes = New List(Of PredictedGene)()
    End Sub
End Class
