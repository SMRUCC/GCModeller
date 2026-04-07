#Region "Microsoft.VisualBasic::04ce9a74bac923d327e93df3fad90e65, analysis\Metagenome\MetaFunction\VFDB\VFDBInfo.vb"

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

    '   Total Lines: 21
    '    Code Lines: 15 (71.43%)
    ' Comment Lines: 3 (14.29%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 3 (14.29%)
    '     File Size: 621 B


    '     Class VFDBInfo
    ' 
    '         Properties: [Function], [Structure], Bacteria, Characteristics, Mechanism
    '                     Reference, VF_FullName, VF_Name, VFcategory, VFCID
    '                     VFID
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace VFDB

    ''' <summary>
    ''' 
    ''' </summary>
    Public Class VFDBInfo

        Public Property VFID As String
        Public Property VF_Name As String
        Public Property VF_FullName As String
        Public Property Bacteria As String
        Public Property VFCID As String
        Public Property VFcategory As String
        Public Property Characteristics As String
        Public Property [Structure] As String
        Public Property [Function] As String
        Public Property Mechanism As String
        Public Property Reference As String

    End Class
End Namespace
