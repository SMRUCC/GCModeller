﻿#Region "Microsoft.VisualBasic::a4340f380698df28410faf9ff7a6a08f, Microsoft.VisualBasic.Core\Language\Linq\Indexer.vb"

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

    '     Module Indexer
    ' 
    '         Function: Indexing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq

Namespace Language

    Public Module Indexer

#Region "Default Public Overloads Property Item(args As Object) As T()"

        ''' <summary>
        ''' Generates the vector elements index collection.
        ''' </summary>
        ''' <param name="args"></param>
        ''' <returns></returns>
        Public Function Indexing(args As Object) As IEnumerable(Of Integer)
            Dim type As Type = args.GetType

            If type Is GetType(Integer) Then
                Return {DirectCast(args, Integer)}
            ElseIf type.ImplementInterface(GetType(IEnumerable(Of Integer))) Then
                Return DirectCast(args, IEnumerable(Of Integer))
            ElseIf type.ImplementInterface(GetType(IEnumerable(Of Boolean))) Then
                Return Which.IsTrue(DirectCast(args, IEnumerable(Of Boolean)))
            ElseIf type.ImplementInterface(GetType(IEnumerable(Of Object))) Then
                Dim array = DirectCast(args, IEnumerable(Of Object)).ToArray

                With array(Scan0).GetType
                    If .ByRef Is GetType(Boolean) Then
                        Return Which.IsTrue(array.Select(Function(o) CBool(o)))
                    ElseIf .ByRef Is GetType(Integer) Then
                        Return array.Select(Function(o) CInt(o))
                    Else
                        Throw New NotImplementedException
                    End If
                End With
            Else
                Throw New NotImplementedException
            End If
        End Function
#End Region
    End Module
End Namespace
