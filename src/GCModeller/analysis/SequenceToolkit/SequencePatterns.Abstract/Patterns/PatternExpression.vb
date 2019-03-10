﻿#Region "Microsoft.VisualBasic::c1a2d19949878c30d0830320fe53a07f, analysis\SequenceToolkit\SequencePatterns.Abstract\Patterns\PatternExpression.vb"

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

    '     Class PatternExpression
    ' 
    '         Properties: Identifier, Motif, RangeExpr
    ' 
    '         Function: Match, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Motif.Patterns

    ''' <summary>
    ''' 使用正则表达式来表示序列的模式
    ''' </summary>
    Public Class PatternExpression
        Implements INamedValue

        Public Property RangeExpr As Token(Of Tokens)()

        Public Property Motif As Residue()
            Get
                Return __motif
            End Get
            Set(value As Residue())
                __motif = value
                __regex = New Regex(String.Join("", value.Select(Function(x) x.Regex).ToArray))
                __rc = New Regex(String.Join("", value.Reverse.Select(Function(x) x.GetComplement.Regex).ToArray))
            End Set
        End Property

        Public Property Identifier As String Implements INamedValue.Key

        Dim __motif As Residue()
        Dim __regex As Regex
        ''' <summary>
        ''' Regex complement reversed.
        ''' </summary>
        Dim __rc As Regex

        Public Function Match(seq As IPolymerSequenceModel) As SimpleSegment()
            Dim nt As String = seq.SequenceData.ToUpper
            Dim matches = __regex.Matches(nt).ToArray

            Throw New NotImplementedException
        End Function

        Public Overrides Function ToString() As String
            Return String.Join("", Motif.Select(Function(x) x.Raw).ToArray)
        End Function
    End Class
End Namespace
