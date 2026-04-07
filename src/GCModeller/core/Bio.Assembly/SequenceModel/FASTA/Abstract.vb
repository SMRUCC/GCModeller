#Region "Microsoft.VisualBasic::2dd51f8c2d9c3f0bb9897f28a7df53c2, core\Bio.Assembly\SequenceModel\FASTA\Abstract.vb"

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

    '   Total Lines: 53
    '    Code Lines: 13 (24.53%)
    ' Comment Lines: 31 (58.49%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 9 (16.98%)
    '     File Size: 2.00 KB


    '     Interface IFastaProvider
    ' 
    '         Properties: length, title
    ' 
    '     Interface IAbstractFastaToken
    ' 
    '         Properties: headers, title
    ' 
    '     Interface ISequenceProvider
    ' 
    '         Function: GetSequenceData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace SequenceModel.FASTA

    ''' <summary>
    ''' A fasta sequence data provider
    ''' </summary>
    Public Interface IFastaProvider : Inherits ISequenceProvider

        ''' <summary>
        ''' A readonly property for provides the title of the sequence 
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property title As String
        ReadOnly Property length As Integer

    End Interface

    ''' <summary>
    ''' The fasta object is a sequence model object with a specific title to identify 
    ''' the sequence and a sequence data property to represents the specific molecule.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IAbstractFastaToken : Inherits IPolymerSequenceModel

        ''' <summary>
        ''' The title value which contains some brief information about this sequence.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>(这条序列数据的标题摘要信息)</remarks>
        ReadOnly Property title As String
        ''' <summary>
        ''' The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
        ''' specific loader and gets some well organized information about the sequence. The format of the header is 
        ''' usually different between each biological database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' (这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
        ''' </remarks>
        Property headers As String()

    End Interface

    ''' <summary>
    ''' This object could provide a biological sequence data
    ''' through <see cref="ISequenceProvider.GetSequenceData()"/> function.
    ''' </summary>
    Public Interface ISequenceProvider

        Function GetSequenceData() As String

    End Interface
End Namespace
