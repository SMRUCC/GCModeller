#Region "Microsoft.VisualBasic::e61df22310d9cd324848208aab086e17, GCModeller\core\Bio.Assembly\Assembly\DOOR\IO.vb"

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

    '   Total Lines: 75
    '    Code Lines: 55
    ' Comment Lines: 12
    '   Blank Lines: 8
    '     File Size: 2.57 KB


    '     Module DOOR_IO
    ' 
    '         Function: __lines, [Imports], Text
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.ComponentModel.Loci
Imports API = SMRUCC.genomics.Assembly.DOOR.DOOR_API

Namespace Assembly.DOOR

    ''' <summary>
    ''' Parser and writer
    ''' </summary>
    Public Module DOOR_IO

        ''' <summary>
        ''' 解析已经读取的文本行为DOOR操纵子集合对象
        ''' </summary>
        ''' <param name="data"></param>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function [Imports](data As String(), Optional path$ = Nothing, Optional header As Boolean = True) As DOOR
            Dim LQuery As OperonGene() = LinqAPI.Exec(Of OperonGene) <=
 _
                From line As String
                In data.Skip(If(header, 1, 0))
                Where Not String.IsNullOrEmpty(line)
                Select OperonGene.TryParse(line)

            Return New DOOR With {.Genes = LQuery}
        End Function

        ''' <summary>
        ''' 文本的标题行
        ''' </summary>
        Const DOOR_title As String = "OperonID	GI	Synonym	Start	End	Strand	Length	COG_number	Product"

        <ExportAPI("Doc.Create")>
        <Extension>
        Public Function Text(data As IEnumerable(Of Operon)) As String
            Dim lines$() = data _
                .Select(AddressOf __lines) _
                .IteratesALL _
                .ToArray
            Dim value$ = {
                DOOR_title
            }.Join(lines) _
            .JoinBy(ASCII.LF)

            Return value
        End Function

        <Extension>
        Private Function __lines(operon As Operon) As String()
            Return LinqAPI.Exec(Of String) <=
 _
                From gene As OperonGene
                In operon.Value
                Let strand = If(gene.Location.Strand = Strands.Forward, "+", "-")
                Let rowData = {
                    operon.Key,
                    gene.GI,
                    gene.Synonym,
                    CStr(gene.Location.Left),
                    CStr(gene.Location.Right),
                    strand,
                    CStr(gene.Location.FragmentSize),
                    gene.COG_number,
                    gene.Product
                }
                Select String.Join(vbTab, rowData)
        End Function
    End Module
End Namespace
