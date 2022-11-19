#Region "Microsoft.VisualBasic::b73cabd6b6325752da85854a1fe5aa08, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\BindReaction.vb"

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

    '   Total Lines: 44
    '    Code Lines: 19
    ' Comment Lines: 14
    '   Blank Lines: 11
    '     File Size: 2.21 KB


    '     Class BindReaction
    ' 
    '         Properties: Activators, Inhibitors, OfficialEC, Reactants, Table
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection
Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Binding reaction between proteins and DNA binding sites such as promoters
    ''' </summary>
    ''' <remarks></remarks>
    Public Class BindReaction : Inherits MetaCyc.File.DataFiles.Slots.Object

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites", "active", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Activators As List(Of String)

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites", "inhibit", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Inhibitors As List(Of String)

        <MetaCycField(name:="OFFICIAL-EC?", type:=MetaCycField.Types.String)> Public Property OfficialEC As String

        <ExternalKey("compounds,proteins,protligandcplxes,dnabindsites,promoters", "have reactant", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Reactants As List(Of String)

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.bindrxns
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As BindReaction
        '    Dim NewObj As BindReaction = New BindReaction

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of BindReaction) _
        '        (MetaCyc.File.AttributeValue.Object.Format(MetaCyc.File.DataFiles.BindRxns.AttributeList, e), NewObj)

        '    NewObj.Activators = StringQuery(NewObj.Object, "ACTIVATORS( \d+)?")
        '    NewObj.Inhibitors = StringQuery(NewObj.Object, "INHIBITORS( \d+)?")
        '    NewObj.Reactants = StringQuery(NewObj.Object, "REACTANTS( \d+)?")
        '    If NewObj.Object.ContainsKey("OFFICIAL-EC?") Then NewObj.OfficialEC = NewObj.Object("OFFICIAL-EC?") Else NewObj.OfficialEC = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace
