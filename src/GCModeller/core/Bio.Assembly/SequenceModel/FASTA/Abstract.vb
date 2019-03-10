#Region "Microsoft.VisualBasic::5ab50f0978609c8a6b573be8ea333c1d, Bio.Assembly\SequenceModel\FASTA\Abstract.vb"

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

'     Interface I_FastaProvider
' 
'         Properties: Headers, Title
' 
'     Interface IAbstractFastaToken
' 
'         Properties: Headers, Title
' 
'     Interface I_FastaObject
' 
'         Function: GetSequenceData
' 
' 
' /********************************************************************************/

#End Region

Namespace SequenceModel.FASTA

    Public Interface I_FastaProvider : Inherits IPolymerSequenceModel
        ReadOnly Property Title As String
        ReadOnly Property Headers As String()
    End Interface

    ''' <summary>
    ''' The fasta object is a sequence model object with a specific title to identify the sequence and a sequence data property to represents the specific molecule.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IAbstractFastaToken : Inherits IPolymerSequenceModel
        ''' <summary>
        ''' The title value which contains some brief information about this sequence.(这条序列数据的标题摘要信息)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ReadOnly Property Title As String
        ''' <summary>
        ''' The attribute header of this FASTA file. The fasta header usually have some format which can be parsed by some 
        ''' specific loader and gets some well organized information about the sequence. The format of the header is 
        ''' usually different between each biological database.(这个FASTA文件的属性头，标题的格式通常在不同的数据库之间是具有很大差异的)
        ''' </summary>
        ''' <returns></returns>
        Property Headers As String()
    End Interface

    Public Interface I_FastaObject
        Function GetSequenceData() As String
    End Interface
End Namespace
