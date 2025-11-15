#Region "Microsoft.VisualBasic::5c9f4fcfa144885fac8e960c5c452f24, meme_suite\MEME.DocParser\ComponentModel\ISiteReader.vb"

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

    '     Interface ISiteReader
    ' 
    '         Properties: Distance, Family, gStart, gStop, ORF
    '                     Strand
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.SequenceModel

Namespace ComponentModel

    Public Interface ISiteReader : Inherits IPolymerSequenceModel
        ReadOnly Property Distance As Integer
        ReadOnly Property ORF As String
        ReadOnly Property Strand As String
        ReadOnly Property gStart As Integer
        ReadOnly Property gStop As Integer
        ReadOnly Property Family As String
    End Interface
End Namespace
