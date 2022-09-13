#Region "Microsoft.VisualBasic::bc64394ba7dfc9fd1e3d02fee5afbe11, GCModeller\models\SBML\SBML\Level2\XmlFile.vb"

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

    '   Total Lines: 87
    '    Code Lines: 58
    ' Comment Lines: 15
    '   Blank Lines: 14
    '     File Size: 3.45 KB


    '     Class XmlFile
    ' 
    '         Properties: Height, level, MetabolismNetwork, Metabolites, Model
    '                     version, Width
    ' 
    '         Function: __loadXml, __processNamespace, Load, Save, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Model.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Model.SBML.Level2.Elements
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Level2

    ''' <summary>
    ''' The level2 sbml xml file.
    ''' </summary>
    <XmlRoot("sbml", Namespace:=API.Namespace)>
    Public Class XmlFile
        Implements I_FBAC2(Of speciesReference)

        <XmlElement("model")> Public Shadows Property Model As Level2.Elements.Model
        <XmlAttribute> Public Property level As Integer
        <XmlAttribute> Public Property version As Integer

        Public Overrides Function ToString() As String
            Return API.Namespace & "   " & Model.ToString
        End Function

        Public Shared Shadows Widening Operator CType(File As String) As XmlFile
            Return XmlFile.Load(File)
        End Operator

        Public Shared Function Load(File As String) As Level2.XmlFile
            Dim doc As XmlFile = __loadXml(File, Encoding.UTF8)
            Return doc.RevertEscapes(Escaping.DefaultEscapes)
        End Function

        Private Shared Function __loadXml(path As String, encoding As Encoding) As XmlFile
            Return path.LoadXml(Of XmlFile)(encoding, throwEx:=True, preprocess:=AddressOf __processNamespace)
        End Function

        Private Shared Function __processNamespace(doc As String) As String
            Dim sbr As StringBuilder = New StringBuilder(doc)
            Call sbr.Replace("<body xmlns=""http://www.w3.org/1999/xhtml"">", "<body>")
            Call sbr.Replace("<math xmlns=""http://www.w3.org/1998/Math/MathML"">", "<math>")

            Return sbr.ToString
        End Function

        Public Function Save(FilePath As String, Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.GetXml.SaveTo(FilePath, throwEx:=True, encoding:=Encoding)
        End Function

        ''' <summary>
        ''' n reactions
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Height As Integer Implements SBML.FLuxBalanceModel.I_FBAC2(Of speciesReference).Height
            Get
                Return Model.listOfReactions.Count()
            End Get
        End Property

        ''' <summary>
        ''' m metabolites
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Width As Integer Implements SBML.FLuxBalanceModel.I_FBAC2(Of speciesReference).Width
            Get
                Return Model.listOfSpecies.Count
            End Get
        End Property

        Public ReadOnly Property MetabolismNetwork As IEnumerable(Of FLuxBalanceModel.I_ReactionModel(Of speciesReference)) Implements SBML.FLuxBalanceModel.I_FBAC2(Of speciesReference).MetabolismNetwork
            Get
                Return Model.listOfReactions
            End Get
        End Property

        Public ReadOnly Property Metabolites As IEnumerable(Of IMetabolite) Implements SBML.FLuxBalanceModel.I_FBAC2(Of speciesReference).Metabolites
            Get
                Return Model.listOfSpecies
            End Get
        End Property
    End Class
End Namespace
