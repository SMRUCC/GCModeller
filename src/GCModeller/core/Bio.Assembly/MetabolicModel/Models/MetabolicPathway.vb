#Region "Microsoft.VisualBasic::6ffc6cb0e11c7d267a00c3a84fe1a035, core\Bio.Assembly\MetabolicModel\Models\MetabolicPathway.vb"

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

    '   Total Lines: 18
    '    Code Lines: 13 (72.22%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 5 (27.78%)
    '     File Size: 607 B


    '     Class MetabolicPathway
    ' 
    '         Properties: ID, metabolicNetwork, metabolites, name
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetabolicModel

    Public Class MetabolicPathway : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property name As String
        Public Property metabolites As MetabolicCompound()
        Public Property metabolicNetwork As MetabolicReaction()

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
