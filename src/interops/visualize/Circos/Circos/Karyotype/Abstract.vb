#Region "Microsoft.VisualBasic::686210fe64f8faae0ad29c5cc0b30fd2, visualize\Circos\Circos\Karyotype\Abstract.vb"

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
    '         Properties: Size
    ' 
    '         Function: Build, (+2 Overloads) Save
    ' 
    '         Sub: __karyotype
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Circos.Configurations

Namespace Karyotype

    ''' <summary>
    ''' The annotated genome skeleton information.
    ''' </summary>
    Public MustInherit Class SkeletonInfo : Inherits BaseClass
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

        Public Function Build(IndentLevel As Integer, directory$) As String Implements ICircosDocNode.Build
            Dim sb As New StringBuilder

            For Each x As IKaryotype In __karyotypes
                Call sb.AppendLine(x.GetData)
            Next
            For Each x As IKaryotype In __bands.SafeQuery
                Call sb.AppendLine(x.GetData)
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
