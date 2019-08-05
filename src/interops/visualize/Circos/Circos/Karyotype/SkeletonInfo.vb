#Region "Microsoft.VisualBasic::584371b5cd4a5a12df90084462e6f039, visualize\Circos\Circos\Karyotype\SkeletonInfo.vb"

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

    '     Class SkeletonInfo
    ' 
    '         Properties: loopHole, size
    ' 
    '         Function: AddBands, Build, (+2 Overloads) Save
    ' 
    '         Sub: singleKaryotypeChromosome
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Circos.Configurations.ComponentModel

Namespace Karyotype

    ''' <summary>
    ''' The annotated genome skeleton information.
    ''' </summary>
    Public MustInherit Class SkeletonInfo : Inherits DynamicPropertyBase(Of Object)
        Implements ICircosDocument

        ''' <summary>
        ''' 基因组的大小，在这里默认是所有的染色体的总长度
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property size As Integer
            Get
                Return Aggregate karyo As Karyotype
                       In karyos
                       Let len As Integer = Math.Abs(karyo.end - karyo.start)
                       Into Sum(len)
            End Get
        End Property

        ''' <summary>
        ''' 缺口的大小，这个仅仅在单个染色体的基因组绘图模型之中有效
        ''' </summary>
        ''' <returns></returns>
        Public Property loopHole As Integer

        Protected karyos As List(Of Karyotype)
        Protected bands As List(Of Band)

        ''' <summary>
        ''' 枚举出当前的这个圈图内的所有的染色体的定义数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Iterator Property Karyotypes As IEnumerable(Of Karyotype)
            Get
                For Each karyo As Karyotype In karyos
                    Yield karyo
                Next
            End Get
        End Property

        ''' <summary>
        ''' 只有一个基因组的时候可以调用这个方法
        ''' </summary>
        Protected Sub singleKaryotypeChromosome(Optional color As String = "black")
            Me.karyos = New Karyotype With {
                .chrLabel = "1",
                .chrName = "chr1",
                .start = 1,
                .end = size,
                .color = color
            }
        End Sub

        Public Function AddBands(bands As IEnumerable(Of Band)) As SkeletonInfo
            Call Me.bands.AddRange(bands)
            Return Me
        End Function

        Public Function Build(IndentLevel As Integer, directory$) As String Implements ICircosDocNode.Build
            Dim sb As New StringBuilder

            For Each chr As IKaryotype In karyos
                Call sb.AppendLine(chr.GetData)
            Next
            For Each chr As IKaryotype In bands.SafeQuery
                Call sb.AppendLine(chr.GetData)
            Next

            Return sb.ToString
        End Function

        Public Function Save(Path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Return Build(Scan0, directory:=Path.ParentPath).SaveTo(Path, encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function
    End Class
End Namespace
