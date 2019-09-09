#Region "Microsoft.VisualBasic::189001a801ecf4e5a67c57e49f53923a, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\base\paste.vb"

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

    '     Class paste0
    ' 
    '         Properties: collapse, x
    ' 
    '         Function: Func
    ' 
    '     Class paste
    ' 
    '         Properties: sep
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Abstract
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.Rtypes

Namespace SymbolBuilder.packages.base

    <RFunc("paste0")> Public Class paste0 : Inherits IRToken

        <Parameter("...", ValueTypes.List, False, True)>
        Public Property x As RExpression()
        Public Property collapse As RExpression = NULL

        Public Function Func(ParamArray x As String()) As String
            Dim action As paste0 = Me.ShadowsCopy
            action.x = x.Select(Function(s) New RExpression(s)).ToArray
            Return action.RScript
        End Function
    End Class

    <RFunc("paste")> Public Class paste : Inherits paste0

        Public Property sep As String = " "
    End Class
End Namespace
