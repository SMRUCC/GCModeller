#Region "Microsoft.VisualBasic::147a908405d6a53824eafd4644df6cc7, core\Bio.Assembly\MetabolicModel\Models\MetabolicPathway.vb"

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

    '   Total Lines: 34
    '    Code Lines: 22 (64.71%)
    ' Comment Lines: 5 (14.71%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 7 (20.59%)
    '     File Size: 1.29 KB


    '     Class MetabolicPathway
    ' 
    '         Properties: genes, ID, metabolicNetwork, metabolites, name
    ' 
    '         Function: CheckAllECNumberExists, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository

Namespace MetabolicModel

    ''' <summary>
    ''' 通路信息（用于通路级别汇总）
    ''' </summary>
    Public Class MetabolicPathway : Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key

        ''' <summary>通路名称</summary>
        Public Property name As String
        ''' <summary>通路包含的基因ID列表</summary>
        Public Property genes As String()
        Public Property metabolites As MetabolicCompound()
        Public Property metabolicNetwork As MetabolicReaction()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function CheckAllECNumberExists(ec_numbers As IEnumerable(Of String)) As Boolean
            Return ec_numbers _
                .All(Function(ec_number)
                         Return metabolicNetwork.Any(Function(rxn) rxn.ECNumbers.IndexOf(ec_number) > -1)
                     End Function)
        End Function

        Public Overrides Function ToString() As String
            Return name
        End Function

    End Class
End Namespace
