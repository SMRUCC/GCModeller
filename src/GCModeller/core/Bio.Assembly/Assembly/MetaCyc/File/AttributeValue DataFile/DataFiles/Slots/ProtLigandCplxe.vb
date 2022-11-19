#Region "Microsoft.VisualBasic::dc3d23797233a4d7790fe888bff1ad03, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\AttributeValue DataFile\DataFiles\Slots\ProtLigandCplxe.vb"

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

    '   Total Lines: 73
    '    Code Lines: 27
    ' Comment Lines: 32
    '   Blank Lines: 14
    '     File Size: 3.76 KB


    '     Class ProtLigandCplxe
    ' 
    '         Properties: Catalyzes, CommonName, ComponentOf, Components, ConsensusSequence
    '                     DNAFootprintSize, Identifier, Regulates, Table, Types
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Reflection
Imports SMRUCC.genomics.Assembly.MetaCyc.Schema.Reflection

Namespace Assembly.MetaCyc.File.DataFiles.Slots

    ''' <summary>
    ''' Protein-Small-Molecule-Complexes
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ProtLigandCplxe : Inherits [Object]
        Implements Slots.Protein.IEnzyme, Regulation.IRegulator

        ''' <summary>
        ''' The proteins and compounds that consists this complexes 
        ''' </summary>
        ''' <remarks></remarks>
        <ExternalKey("proteins,compounds,protligandcplxes", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Components As List(Of String) Implements Slots.Protein.IEnzyme.Components
        <MetaCycField(type:=MetaCycField.Types.TStr)> Public Shadows Property Types As List(Of String) Implements Regulation.IRegulator.Types

        <ExternalKey("proteins", "", ExternalKey.Directions.In)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property ComponentOf As List(Of String) Implements Slots.Protein.IEnzyme.ComponentOf, Slots.Regulation.IRegulator.ComponentOf

        ''' <summary>
        ''' A list of enzymatic reaction unique id that catalyzed by this protein.(本蛋白质所催化的酶促反应的UniqueId的列表)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <ExternalKey("reactions,enzrxns", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Catalyzes As List(Of String) Implements Slots.Protein.IEnzyme.Catalyze

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        <MetaCycField()> Public Property ConsensusSequence As String
        <MetaCycField()> Public Property DNAFootprintSize As String

        ''' <summary>
        ''' The reaction that this complex regulates
        ''' </summary>
        ''' <remarks></remarks>
        <ExternalKey("regulation", "", ExternalKey.Directions.Out)> <MetaCycField(Type:=MetaCycField.Types.TStr)>
        Public Property Regulates As List(Of String) Implements Regulation.IRegulator.Regulates

        <MetaCycField()> Public Shadows Property Identifier As String Implements Slots.Protein.IEnzyme.UniqueId, Regulation.IRegulator.UniqueId
        <MetaCycField()> <XmlAttribute> Public Shadows Property CommonName As String Implements Slots.Protein.IEnzyme.Name, Regulation.IRegulator.CommonName

        Public Overrides ReadOnly Property Table As [Object].Tables
            Get
                Return Tables.protligandcplxes
            End Get
        End Property

        'Public Shared Shadows Widening Operator CType(e As MetaCyc.File.AttributeValue.Object) As ProtLigandCplxe
        '    Dim NewObj As ProtLigandCplxe = New ProtLigandCplxe

        '    Call MetaCyc.File.DataFiles.Slots.[Object].TypeCast(Of MetaCyc.File.DataFiles.Slots.ProtLigandCplxe) _
        '        (MetaCyc.File.AttributeValue.Object.Format(ProtLigandCplxes.AttributeList, e), NewObj)

        '    NewObj.Components = StringQuery(NewObj.Object, "COMPONENTS( \d+)?")
        '    NewObj.Regulates = StringQuery(NewObj.Object, "REGULATES( \d+)?")

        '    If NewObj.Exists("CONSENSUS-SEQUENCE") Then NewObj.ConsensusSequence = NewObj.Object("CONSENSUS-SEQUENCE") Else NewObj.ConsensusSequence = String.Empty
        '    If NewObj.Exists("DNA-FOOTPRINT-SIZE") Then NewObj.DNAFootprintSize = NewObj.Object("DNA-FOOTPRINT-SIZE") Else NewObj.DNAFootprintSize = String.Empty

        '    Return NewObj
        'End Operator
    End Class
End Namespace
