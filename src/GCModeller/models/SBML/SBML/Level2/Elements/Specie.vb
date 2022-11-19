#Region "Microsoft.VisualBasic::96397aa21a5300c906fa271c0e4c7ca6, GCModeller\models\SBML\SBML\Level2\Elements\Specie.vb"

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
    '    Code Lines: 27
    ' Comment Lines: 11
    '   Blank Lines: 5
    '     File Size: 1.47 KB


    '     Class Specie
    ' 
    '         Properties: Charge, compartmentId, ID, InitialAmount, Notes
    ' 
    '         Function: GetTrimmedId, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Components
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Level2.Elements

    <XmlType("species")>
    Public Class Specie : Inherits Components.Specie
        Implements IMetabolite

        ''' <summary>
        ''' UniqueID
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Escaped>
        <XmlAttribute("id")>
        Public Overrides Property ID As String Implements IMetabolite.Key
        <XmlAttribute("initialAmount")>
        Public Property InitialAmount As Double Implements IMetabolite.InitializeAmount
        <Escaped>
        <XmlAttribute("compartment")>
        Public Overrides Property compartmentId As String
        <XmlAttribute("charge")>
        Public Property Charge As Double
        <XmlElement("notes")> Public Property Notes As Notes

        ''' <summary>
        ''' 获取去除了位置编号的唯一标识符
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTrimmedId() As String
            Return ID.Replace("_CCO-IN", "").Replace("_CCO-OUT", "").ToUpper
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("[{0}]{1} [{2}]", ID, name, compartmentId)
        End Function
    End Class
End Namespace
