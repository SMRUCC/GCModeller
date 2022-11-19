#Region "Microsoft.VisualBasic::2b7249cb86a11e2dea23f45ff30c344d, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\Metabolites\Glycan.vb"

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

    '   Total Lines: 76
    '    Code Lines: 61
    ' Comment Lines: 4
    '   Blank Lines: 11
    '     File Size: 2.55 KB


    '     Class Glycan
    ' 
    '         Properties: Composition, CompoundId, Mass, Orthology
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: GetCompoundId, ToCompound
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    <XmlRoot("KEGG.Glycan", Namespace:="http://www.kegg.jp/dbget-bin/www_bget?gl:glycan_id")>
    Public Class Glycan : Inherits Compound

        Public Property Composition As String
        Public Property Mass As String
        Public Property Orthology As KeyValuePair()

        ''' <summary>
        ''' Glycan id to kegg compound id
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CompoundId As String()
            Get
                Return GetCompoundId(Me)
            End Get
        End Property

        Sub New()
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Sub New(links As DBLinks)
            MyBase._DBLinks = links
        End Sub

        Public Shared Function GetCompoundId(compound As Compound) As String()
            If compound.remarks.IsNullOrEmpty Then
                Return {}
            End If

            Dim sameAs$ = compound.remarks _
                .Select(Function(s)
                            Return s.GetTagValue(":"c, trim:=True)
                        End Function) _
                .Where(Function(t) t.Name = "Same as") _
                .FirstOrDefault _
                .Value

            If sameAs.StringEmpty Then
                Return {}
            Else
                Return sameAs.Split _
                    .Select(AddressOf Trim) _
                    .Where(Function(id) id.First = "C"c) _
                    .ToArray
            End If
        End Function

        Const URL = "http://www.kegg.jp/dbget-bin/www_bget?gl:{0}"

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ToCompound() As Compound
            Return New Compound With {
                .entry = entry,
                .commonNames = commonNames,
                .DbLinks = DbLinks,
                .formula = Me.Composition,
                .reactionId = reactionId,
                .Module = Me.Module,
                .molWeight = Val(Mass),
                .pathway = pathway,
                .enzyme = enzyme,
                .exactMass = .molWeight,
                .remarks = remarks
            }
        End Function
    End Class
End Namespace
