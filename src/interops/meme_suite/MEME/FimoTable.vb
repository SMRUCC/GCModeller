#Region "Microsoft.VisualBasic::7530399a81416f0d52b61dac8deee640, meme_suite\MEME\FimoTable.vb"

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

    ' Class FimoTable
    ' 
    '     Properties: [End], Matched, Motif, pValue, qValue
    '                 Start, Strand, Title
    ' 
    '     Function: MatchInterGeneLoci
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.csv.Extensions
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

''' <summary>
''' Motif在序列上面的搜索结果
''' </summary>
''' <remarks></remarks>
Public Class FimoTable

    Public Property Motif As String
    <Column("Sequence Name")> Public Property Title As String
    Public Property Strand As String
    Public Property Start As Integer
    Public Property [End] As Integer
    <Column("p-value")> Public Property pValue As Double
    <Column("q-value")> Public Property qValue As Double
    <Column("Matched Sequence")> Public Property Matched As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Table"></param>
    ''' <param name="anno"><see cref="SMRUCC.genomics.ComponentModel.Annotation.GeneTable"></see>Csv文件的文件路径</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function MatchInterGeneLoci(Table As String, anno As String) As FimoTable()
        Dim FiMOTable = Table.LoadCsv(Of FimoTable)(False).ToArray
        Throw New NotImplementedException
    End Function
End Class
