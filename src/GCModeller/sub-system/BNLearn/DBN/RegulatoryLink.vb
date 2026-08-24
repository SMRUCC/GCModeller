#Region "Microsoft.VisualBasic::12aefb41c7e09f1842f306896d95ed3f, sub-system\BNLearn\DBN\RegulatoryLink.vb"

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

    '   Total Lines: 47
    '    Code Lines: 13 (27.66%)
    ' Comment Lines: 30 (63.83%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 4 (8.51%)
    '     File Size: 1.25 KB


    ' Class RegulatoryLink
    ' 
    '     Properties: effector, regulate_genes, target_operon, TF_family, TF_id
    '                 TFBS_id
    ' 
    ' Enum Effector
    ' 
    '     Activator, Inhibitor, Unknown
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region


''' <summary>
''' Gene regulatory network
''' </summary>
Public Class RegulatoryLink

    ''' <summary>
    ''' transcript factor protein/rna id
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_id As String
    ''' <summary>
    ''' family of the TF
    ''' </summary>
    ''' <returns></returns>
    Public Property TF_family As String
    ''' <summary>
    ''' motif id of the TFBS site
    ''' </summary>
    ''' <returns></returns>
    Public Property TFBS_id As String
    ''' <summary>
    ''' effector metabolite of this TF its regulation function
    ''' </summary>
    ''' <returns></returns>
    Public Property effector As Dictionary(Of String, Effector)
    ''' <summary>
    ''' target operon id that this TF regulates
    ''' </summary>
    ''' <returns></returns>
    Public Property target_operon As String
    ''' <summary>
    ''' the operon member genes, TF regulates this operon member genes theirs transcription.
    ''' </summary>
    ''' <returns></returns>
    Public Property regulate_genes As String()

End Class

''' <summary>
''' effects of the effector to the TF protein
''' </summary>
Public Enum Effector
    Unknown
    Activator
    Inhibitor
End Enum

