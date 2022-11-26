#Region "Microsoft.VisualBasic::1e31a375527932e89d741e136f5bfbda, GCModeller\core\Bio.Assembly\Metagenomics\BIOMTaxonomyParser.vb"

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

    '   Total Lines: 50
    '    Code Lines: 24
    ' Comment Lines: 21
    '   Blank Lines: 5
    '     File Size: 1.94 KB


    '     Class BIOMTaxonomyParser
    ' 
    '         Function: Parse, ToString, TryParse
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Scripting.Runtime

Namespace Metagenomics

    ''' <summary>
    ''' Parser and stringfier of <see cref="Taxonomy"/> object.
    ''' </summary>
    Public Class BIOMTaxonomyParser : Implements IParser

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">
        ''' Object value should be in data type <see cref="Taxonomy"/>
        ''' </param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overloads Function ToString(obj As Object) As String Implements IParser.ToString
            Return DirectCast(obj, Taxonomy).ToString(BIOMstyle:=True)
        End Function

        ''' <summary>
        ''' Create a <see cref="Taxonomy"/> object from parse taxonomy string
        ''' </summary>
        ''' <param name="content"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function TryParse(content As String) As Object Implements IParser.TryParse
            Return Parse(biomString:=content)
        End Function

        ''' <summary>
        ''' parse auto
        ''' </summary>
        ''' <param name="biomString"></param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Parse(biomString As String) As Taxonomy
            If biomString.StartsWith("superkingdom__") OrElse biomString.StartsWith("kingdom__") Then
                Return BIOMTaxonomy.TaxonomyParserAlt(biomString).AsTaxonomy
            ElseIf biomString.StartsWith("k__") Then
                Return BIOMTaxonomy.TaxonomyParser(biomString).AsTaxonomy
            Else
                Return BIOMTaxonomy.TaxonomyFromString(biomString)
            End If
        End Function
    End Class
End Namespace
