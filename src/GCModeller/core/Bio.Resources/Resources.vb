#Region "Microsoft.VisualBasic::d40c4e7dcb420869d1c9659f8ca1af02, ..\core\Bio.Resources\Resources.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.ComponentModel.Composition
Imports System.Resources

''' <summary>
''' External resource data EXPORT API
''' </summary>
<Export(GetType(ResourceManager))>
Public Module Resources

    ''' <summary>
    ''' Export the resource data to the assembly SMRUCC.genomics.Assembly.Components_v2.0_33.0.0.0__89845dcd8080cc91
    ''' </summary>
    ''' <returns></returns>
    <Export(GetType(ResourceManager))>
    Public ReadOnly Property Resources As ResourceManager
        Get
            Return My.Resources.ResourceManager
        End Get
    End Property
End Module
