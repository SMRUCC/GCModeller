#Region "Microsoft.VisualBasic::5cf624e2e76b5ce52e8ed4593a5398d5, RDotNET.Extensions.VisualBasic\API\base\with.vb"

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

    '     Module base
    ' 
    '         Function: [with]
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace API

    Partial Module base

        ''' <summary>
        ''' Evaluate an R expression in an environment constructed from data, possibly modifying (a copy of) the original data.
        ''' </summary>
        ''' <param name="data">data to use for constructing an environment. For the default with method this may be an environment, a list, a data frame, or an integer as in sys.call. For within, it can be a list or a data frame.</param>
        ''' <param name="expr">expression to evaluate.</param>
        ''' <param name="additionals">arguments to be passed to future methods.</param>
        ''' <returns>
        ''' with is a generic function that evaluates expr in a local environment constructed from data. The environment has the caller's environment as its parent. This is useful for simplifying calls to modeling functions. (Note: if data is already an environment then this is used with its existing parent.)
        ''' Note that assignments within expr take place In the constructed environment And Not In the user's workspace.
        ''' within Is similar, except that it examines the environment after the evaluation of expr And makes the corresponding modifications to a copy of data (this may fail in the data frame case if objects are created which cannot be stored in a data frame), And returns it. within can be used as an alternative to transform.
        ''' </returns>
        Public Function [with](data As String, expr As String, ParamArray additionals As String()) As String
            Dim tmp As String = RDotNetGC.Allocate

            Call $"{tmp} <- with({data}, {"{" & vbCrLf &
                                              expr & vbCrLf &
                                          "}"}, {additionals.params})".__call
            Return tmp
        End Function
    End Module
End Namespace
