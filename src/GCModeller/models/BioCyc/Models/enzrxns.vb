#Region "Microsoft.VisualBasic::fe81c754d475c24a84afe7daba5a1de5, GCModeller\models\BioCyc\Models\enzrxns.vb"

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

    '   Total Lines: 43
    '    Code Lines: 37
    ' Comment Lines: 0
    '   Blank Lines: 6
    '     File Size: 1.43 KB


    ' Class enzrxns
    ' 
    '     Properties: alternativeSubstrates, cofactors, EC_number, enzyme, Kcat
    '                 Km, PH, reaction, reactionDirection, regulatedBy
    '                 specificActivity, temperature, Vmax
    ' 
    ' Class KineticsFactor
    ' 
    '     Properties: citations, Km, substrate
    ' 
    ' /********************************************************************************/

#End Region

Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism

<Xref("enzrxns.dat")>
Public Class enzrxns : Inherits Model

    <AttributeField("ALTERNATIVE-SUBSTRATES")>
    Public Property alternativeSubstrates As String()
    <AttributeField("ENZYME")>
    Public Property enzyme As String
    <AttributeField("REACTION")>
    Public Property reaction As String
    <AttributeField("REACTION-DIRECTION")>
    Public Property reactionDirection As ReactionDirections
    <AttributeField("COFACTORS")>
    Public Property cofactors As String()
    <AttributeField("KM")>
    Public Property Km As KineticsFactor()
    <AttributeField("KCAT")>
    Public Property Kcat As KineticsFactor()
    <AttributeField("PH-OPT")>
    Public Property PH As Double
    <AttributeField("TEMPERATURE-OPT")>
    Public Property temperature As Double
    <AttributeField("SPECIFIC-ACTIVITY")>
    Public Property specificActivity As Double
    <AttributeField("REGULATED-BY")>
    Public Property regulatedBy As String()
    <AttributeField("ENZRXN-EC-NUMBER")>
    Public Property EC_number As ECNumber
    <AttributeField("VMAX")>
    Public Property Vmax As Double

End Class

Public Class KineticsFactor

    Public Property Km As Double
    Public Property substrate As String
    Public Property citations As String()

End Class
