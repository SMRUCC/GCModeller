#Region "Microsoft.VisualBasic::29aa8fe80cbc2890031283ae030279f5, core\Bio.Assembly\Assembly\KEGG\Web\Map\MapIndex.vb"

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

    '     Class MapIndex
    ' 
    '         Properties: compoundIndex, index, KeyVector, KOIndex
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.WebServices

    Public Class MapIndex : Inherits Map
        Implements INamedValue

        <XmlElement("keys")>
        Public Property KeyVector As TermsVector
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New TermsVector With {
                    .terms = index.Objects
                }
            End Get
            Set(value As TermsVector)
                _index = New Index(Of String)(value.terms)
                _KOIndex = value _
                    .terms _
                    .Where(Function(id)
                               Return id.IsPattern("K\d+", RegexICSng)
                           End Function) _
                    .Indexing
                _compoundIndex = value _
                    .terms _
                    .Where(Function(id)
                               Return id.IsPattern("C\d+", RegexICSng)
                           End Function) _
                    .Indexing
            End Set
        End Property

        ''' <summary>
        ''' KO, compoundID, reactionID, etc.
        ''' </summary>
        ''' <returns></returns>
        <XmlIgnore> Public ReadOnly Property index As Index(Of String)
        <XmlIgnore> Public ReadOnly Property KOIndex As Index(Of String)
        <XmlIgnore> Public ReadOnly Property compoundIndex As Index(Of String)

        Public Overrides Function ToString() As String
            Return ID
        End Function
    End Class

End Namespace
