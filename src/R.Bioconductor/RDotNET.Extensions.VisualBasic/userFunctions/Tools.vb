#Region "Microsoft.VisualBasic::ad3ab05c24278203e09e3425488fef45, RDotNET.Extensions.VisualBasic\userFunctions\Tools.vb"

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

    '     Module MyTools
    ' 
    '         Function: (+2 Overloads) RemovesRlistNULL, SingleColumn2Vector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports RDotNET.Extensions.VisualBasic.API

Namespace Custom

    Public Module MyTools

        ''' <summary>
        ''' ```R
        ''' singleColumndf2Vector &lt;- function(df) {
        '''
        '''     df = as.vector(df)
        '''	    df = as.vector(df[,1])
        '''
        '''     return (df)
        ''' }
        ''' ```
        ''' </summary>
        ''' <param name="df$">data frame type in R language</param>
        ''' <returns></returns>
        Public Function SingleColumn2Vector(df$) As String
            df = [as].vector(df)
            df = [as].vector(df)

            Return df
        End Function

        <Extension> Public Function RemovesRlistNULL(list As var) As String
            Return list.name.RemovesRlistNULL
        End Function

        ''' <summary>
        ''' 从R的list()对象之中删除NA名称以及NULL内容的slot元素
        ''' </summary>
        ''' <param name="list$"></param>
        ''' <returns></returns>
        <Extension> Public Function RemovesRlistNULL(list$) As String
            ' > head(raw.pos)
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            '
            ' $<NA>
            ' NULL
            Dim newList$ = base.list()

            SyncLock R
                With R

                    .call = $"for(i in 1:length({list})) {{

                                  name <- names({list}[i]);

                                  if (!is.na(name)) {{
                                      {newList}[[name]] <- {list}[[name]]
                                  }}

                              }}"

                End With
            End SyncLock

            Return newList
        End Function
    End Module
End Namespace
