#Region "Microsoft.VisualBasic::24c5b96129b929ab7b3ba111cdc69fd1, GCModeller\engine\IO\GCMarkupLanguage\GCML_Documents\XmlElements\Metabolism\Metabolite.vb"

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
    '    Code Lines: 44
    ' Comment Lines: 20
    '   Blank Lines: 9
    '     File Size: 3.03 KB


    '     Class Metabolite
    ' 
    '         Properties: BoundaryCondition, CommonName, Compartment, Identifier, InitialAmount
    '                     MetaboliteType, MolWeight, NumOfFluxAssociated
    '         Enum MetaboliteTypes
    ' 
    '             Compound, Polypeptide, ProteinComplexes, Transcript
    ' 
    ' 
    ' 
    '         Interface IDegradable
    ' 
    '             Properties: Lamda, locusId
    ' 
    '             Function: CastTo, ToString
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Serialization

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Metabolite
        Implements FLuxBalanceModel.IMetabolite
        Implements INamedValue

        ''' <summary>
        ''' UniqueID.(本目标对象的唯一标识符)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.IMetabolite.Key
        <DumpNode> <XmlElement("COMMON-NAME", Namespace:="http://code.google.com/p/genome-in-code/virtualcell_model/GCMarkupLanguage/")>
        Public Property CommonName As String
        <DumpNode> <XmlAttribute> Public Property Compartment As String
        <DumpNode> <XmlAttribute("InitialAmount")>
        Public Property InitialAmount As Double Implements FLuxBalanceModel.IMetabolite.InitializeAmount

        <DumpNode> <XmlAttribute("Boundary?")> Public Shadows Property BoundaryCondition As Boolean
        Public Property MolWeight As Double

        ''' <summary>
        ''' 与本代谢物相关的流对象的数目，计算规则：
        ''' 当处于不可逆反应的时候：处于产物边，计数为零，处于底物边，计数为1
        ''' 当处于可逆反应的时候：无论是处于产物边还是底物边，都被计数为0.5
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("n_FluxAssociated", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics")>
        Public Property NumOfFluxAssociated As Integer
        <XmlElement("MetaboliteType", Namespace:="http://code.google.com/p/genome-in-code/GCModeller/Dynamics/Mapping")>
        Public Property MetaboliteType As MetaboliteTypes

        Public Enum MetaboliteTypes
            Compound
            Polypeptide
            ProteinComplexes
            Transcript
        End Enum

        Public Overrides Function ToString() As String
            Return Identifier
        End Function

        Public Shared Function CastTo(e As Level2.Elements.Specie) As Metabolite
            Return New Metabolite With {
                .Identifier = e.ID,
                .CommonName = e.name,
                .BoundaryCondition = e.boundaryCondition,
                .Compartment = e.CompartmentID,
                .InitialAmount = e.InitialAmount
            }
        End Function

        Public Interface IDegradable
            Property locusId As String
            ''' <summary>
            ''' 降解系数，必须为0-1之间的数字，数字越小，降解越快
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Property Lamda As Double
        End Interface
    End Class
End Namespace
