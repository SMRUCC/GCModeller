#Region "Microsoft.VisualBasic::79d63116d5ca4fe70a23ea906fea78e3, ..\interops\visualize\Circos\Circos\Karyotype\Abstract.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Circos.Configurations
Imports SMRUCC.genomics.AnalysisTools.DataVisualization.Interaction.Circos.TrackDatas
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Namespace Karyotype

    ''' <summary>
    ''' The annotated genome skeleton information.
    ''' </summary>
    Public MustInherit Class SkeletonInfo : Inherits ClassObject
        Implements ICircosDocument

        ''' <summary>
        ''' 基因组的大小，在这里默认是所有的染色体的总长度
        ''' </summary>
        ''' <returns></returns>
        Public Overridable ReadOnly Property Size As Integer
            Get
                Return __karyotypes.Select(Function(x) Math.Abs(x.end - x.start)).Sum
            End Get
        End Property

        Protected __karyotypes As List(Of Karyotype)
        Protected __bands As List(Of Band)

        Public ReadOnly Iterator Property Karyotypes As IEnumerable(Of Karyotype)
            Get
                For Each x As Karyotype In __karyotypes
                    Yield x
                Next
            End Get
        End Property

        ''' <summary>
        ''' 只有一个基因组的时候可以调用这个方法
        ''' </summary>
        Protected Sub __karyotype(Optional color As String = "black")
            Me.__karyotypes = New List(Of Karyotype) From {
                New Karyotype With {
                    .chrLabel = "1",
                    .chrName = "chr1",
                    .start = 1,
                    .end = Size,
                    .color = color
                }
            }
        End Sub

        Public Function GenerateDocument(IndentLevel As Integer) As String Implements ICircosDocNode.GenerateDocument
            Dim sb As New StringBuilder

            For Each x As IKaryotype In __karyotypes
                Call sb.AppendLine(x.GetData)
            Next
            For Each x As IKaryotype In __bands.SafeQuery
                Call sb.AppendLine(x.GetData)
            Next

            Return sb.ToString
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encoding = Nothing) As Boolean Implements ISaveHandle.Save
            Return GenerateDocument(Scan0).SaveTo(Path, encoding)
        End Function

        Public Function Save(Optional Path As String = "", Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
            Return Save(Path, encoding.GetEncodings)
        End Function
    End Class
End Namespace
