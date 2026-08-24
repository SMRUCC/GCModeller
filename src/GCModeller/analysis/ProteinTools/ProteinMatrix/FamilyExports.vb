#Region "Microsoft.VisualBasic::18ee5b57b875a36f41cb46a98ea944d5, analysis\ProteinTools\ProteinMatrix\FamilyExports.vb"

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

    '   Total Lines: 25
    '    Code Lines: 12 (48.00%)
    ' Comment Lines: 8 (32.00%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (20.00%)
    '     File Size: 616 B


    ' Class FamilyExports
    ' 
    '     Properties: family_id, members, rep_seq, representative
    ' 
    ' Class SequenceCluster
    ' 
    '     Properties: family_id, score, seq, seq_title
    ' 
    ' /********************************************************************************/

#End Region

Public Class FamilyExports

    Public Property family_id As String
    Public Property members As Integer
    ''' <summary>
    ''' representative sequence title
    ''' </summary>
    ''' <returns></returns>
    Public Property representative As String
    ''' <summary>
    ''' representative sequence
    ''' </summary>
    ''' <returns></returns>
    Public Property rep_seq As String

End Class

Public Class SequenceCluster

    Public Property seq_title As String
    Public Property family_id As String
    Public Property score As Double
    Public Property seq As String

End Class
