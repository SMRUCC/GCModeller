#Region "Microsoft.VisualBasic::9aea69bd6efcd61aeb6f8ee5f8e13fd9, RDotNET.Extensions.VisualBasic\API\base\seq.vb"

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
    '         Function: seq, unique
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

Namespace API

    Partial Module base

        Public Function unique(ref As String) As String
            SyncLock R
                With R
                    Dim var$ = RDotNetGC.Allocate
                    .call = $"{var} <- unique({ref});"
                    Return var
                End With
            End SyncLock
        End Function

        ''' <summary>
        ''' Generate regular sequences. seq is a standard generic with a default method. seq.int is a primitive which can be much faster but has a few restrictions. seq_along and seq_len are very fast primitives for two common cases.
        ''' </summary>
        ''' <param name="from">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
        ''' <param name="[to]">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
        ''' <param name="by">number: increment of the sequence.</param>
        ''' <param name="lengthOut">desired length of the sequence. A non-negative number, which for seq and seq.int will be rounded up if fractional.</param>
        ''' <param name="alongWith">take the length from the length of this argument.</param>
        ''' <param name="additionals">arguments passed to or from methods.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Numerical inputs should all be finite (that is, not infinite, NaN or NA).
        ''' The interpretation Of the unnamed arguments Of seq And seq.int Is Not standard, And it Is recommended always To name the arguments When programming.
        ''' seq Is generic, And only the default method Is described here. Note that it dispatches on the class of the first argument irrespective of argument names. 
        ''' This can have unintended consequences if it Is called with just one argument intending this to be taken as along.with it Is much better to use seg_along in that case.
        ''' seq.int Is an internal generic which dispatches on methods for "seq" based on the class of the first supplied argument (before argument matching).
        ''' </remarks>
        Public Function seq(Optional from As Double = 1,
                            Optional [to] As Double = 1,
                            Optional by As String = NULL,
                            Optional lengthOut As String = NULL,
                            Optional alongWith As String = NULL,
                            Optional additionals As String() = Nothing) As String
            Dim tmp As var

            If alongWith <> NULL Then
                tmp = $"seq(along.with= {alongWith}  {additionals.params})"
            Else
                If lengthOut <> NULL Then
                    If by = NULL Then
                        tmp = $"seq(from = {from}, to = {[to]}, 
                                    by = ((to - from)/(length.out - 1)), 
                                    length.out = {lengthOut} 
                                    {additionals.params})"
                    Else
                        tmp = $"seq(from = {from}, to = {[to]}, length.out = {lengthOut} {additionals.params})"
                    End If
                Else
                    If by = NULL Then
                        tmp = $"seq(from = {from}, to = {[to]} {additionals.params})"
                    Else
                        tmp = $"seq(from = {from}, to = {[to]}, by = {by} {additionals.params})"
                    End If
                End If
            End If

            Return tmp
        End Function
    End Module
End Namespace
