#Region "Microsoft.VisualBasic::c6b2ebcd4bab9dbd40dbe0e39c9664f5, GCModeller\core\Bio.Assembly\Assembly\iGEM\rsbpml.vb"

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

    '   Total Lines: 43
    '    Code Lines: 27
    ' Comment Lines: 9
    '   Blank Lines: 7
    '     File Size: 1.57 KB


    '     Class rsbpml
    ' 
    '         Properties: part_list
    ' 
    '     Class Part
    ' 
    '         Properties: part_author, part_entered, part_id, part_name, part_nickname
    '                     part_rating, part_results, part_short_desc, part_short_name, part_type
    '                     part_url, release_status, sample_status, sequences
    ' 
    '     Class seq_data
    ' 
    '         Properties: SequenceData
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel

Namespace Assembly.iGEM

    Public Class rsbpml

        Public Property part_list As Part()

    End Class

    <XmlType("part")> Public Class Part
        Public Property part_id As String
        Public Property part_name As String
        Public Property part_short_name As String
        Public Property part_short_desc As String
        Public Property part_type As String
        Public Property release_status As String
        Public Property sample_status As String
        Public Property part_results As String
        Public Property part_nickname As String
        Public Property part_rating As String
        Public Property part_url As String
        Public Property part_entered As String
        Public Property part_author As String
        ' Public Property deep_subparts As String
        ' Public Property specified_subparts As String
        'Public Property specified_subscars As String
        Public Property sequences As seq_data
        'Public Property features As String
        'Public Property parameters As String
        'Public Property categories As String
        'Public Property samples As String
        'Public Property references As String
        'Public Property groups As String
    End Class

    Public Class seq_data : Implements IPolymerSequenceModel

        <XmlElement("seq_data")>
        Public Property SequenceData As String Implements IPolymerSequenceModel.SequenceData
    End Class
End Namespace
