#Region "Microsoft.VisualBasic::584371b5cd4a5a12df90084462e6f039, visualize\Circos\Circos\KaryotypeEntry\KaryotypeSkeleton.vb"

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

    '     Class KaryotypeSkeleton
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
    Public MustInherit Class KaryotypeSkeleton : Inherits DynamicPropertyBase(Of Object)
        Implements ICircosDocument

        ''' <summary>
        ''' 基因组的大小，在这里默认是所有的染色体的总长度
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property size As Integer
            Get
                ' 使用 Long 累加之后再转换为 Integer，避免真核生物基因组（> 2^31）求总和的时候溢出
                Dim genomeSize& = Aggregate karyo As KaryotypeEntry
                                  In karyos.SafeQuery
                                  Let len As Long = Math.Abs(CLng(karyo.end) - CLng(karyo.start))
                                  Into Sum(len)

                If genomeSize > Integer.MaxValue Then
                    Throw New OverflowException(
                        $"The genome size {genomeSize} nt is too large for the circos plot, " &
                        "please reduce the resolution of your data (e.g. use a larger window size).")
                End If

                Return CInt(genomeSize)
            End Get
        End Property

        ''' <summary>
        ''' 缺口的大小，这个仅仅在单个染色体的基因组绘图模型之中有效
        ''' </summary>
        ''' <returns></returns>
        Public Property loopHole As Integer

        Protected karyos As List(Of KaryotypeEntry)
        Protected bands As List(Of Band)

        ''' <summary>
        ''' 枚举出当前的这个圈图内的所有的染色体的定义数据
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Iterator Property Karyotypes As IEnumerable(Of KaryotypeEntry)
            Get
                For Each karyo As KaryotypeEntry In karyos
                    Yield karyo
                Next
            End Get
        End Property

        ''' <summary>
        ''' 枚举出当前的这个圈图内的所有的 cytogenetic band 定义数据
        ''' （用于内置的 GDI+ 绘图引擎绘制 band 色带）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BandData As Band()
            Get
                If bands Is Nothing Then
                    Return New Band() {}
                End If

                Return bands.ToArray()
            End Get
        End Property

        ''' <summary>
        ''' 只有一个基因组的时候可以调用这个方法
        ''' </summary>
        Protected Sub singleKaryotypeChromosome(Optional color As String = "black")
            Me.karyos = New KaryotypeEntry With {
                .chrLabel = "1",
                .chrName = "chr1",
                .start = 1,
                .end = size,
                .color = color
            }
        End Sub

        Public Function AddBands(bands As IEnumerable(Of Band)) As KaryotypeSkeleton
            Call Me.bands.AddRange(bands)
            Return Me
        End Function

        Public Function Build(IndentLevel As Integer, directory$) As String Implements ICircosDocNode.Build
            Dim sb As New StringBuilder

            For Each chr As IKaryotype In karyos.SafeQuery
                Call sb.AppendLine(chr.GetData)
            Next
            For Each chr As IKaryotype In bands.SafeQuery
                Call sb.AppendLine(chr.GetData)
            Next

            Return sb.ToString
        End Function

        Public Function Save(filePath As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Dim parent$ = System.IO.Path.GetDirectoryName(filePath.Replace("\"c, "/"c))

            If Not String.IsNullOrEmpty(parent) Then
                Call System.IO.Directory.CreateDirectory(parent)
            End If

            Return Build(Scan0, directory:=parent).SaveTo(filePath, encoding)
        End Function

        Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(path, encoding.CodePage)
        End Function

        Public Function Save(file As IO.Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
