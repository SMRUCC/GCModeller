#Region "Microsoft.VisualBasic::e51b4f08bfecd7ad5dcf26f9a010204e, GCModeller\data\SABIO-RK\Dumps\Kinetics.vb"

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

'   Total Lines: 151
'    Code Lines: 91
' Comment Lines: 44
'   Blank Lines: 16
'     File Size: 7.24 KB


'     Class EnzymeCatalystKineticLaw
' 
'         Properties: Buffer, Ec, Enzyme, Kcat, KEGGCompoundId
'                     KEGGReactionId, KineticRecord, Km, Metabolite, PH
'                     PubMed, Temperature, Uniprot
' 
'         Function: Copy, ToString
' 
'     Class ModifierKinetics
' 
' 
'         Enum ModifierTypes
' 
'             Activator, CoFactors, Inhibitor
' 
' 
' 
'  
' 
'     Properties: K, KEGGCompoundId, KineticsRecordId, Modifier, ModifierType
'                 ObjectId
' 
'     Function: ToString, TryGetType
' 
'     Class KineticLawModel
' 
'         Properties: Catalyst, Ec, Equation, Fast, KeggReaction
'                     KineticRecord, PubMed, Reaction, Taxonomy
' 
'         Function: CreateObject, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models.KeyValuePair
Imports SMRUCC.genomics.Data.SABIORK.SBML
Imports SMRUCC.genomics.Model.SBML.Level3

Namespace TabularDump

    ''' <summary>
    ''' Vmax 是指 最大反应速度。当 底物浓度 足够大时，体系中酶的活性中心达到饱和状态，其反应速度达到最大。由此可见，最大反应速度 Vmax 随 酶浓度 的变化而变化。
    ''' kcat 指反应常数(Catalytic Constant)，Kcat 可以由公式计算得到： kcat = Vmax/[E]
    '''    - [E] 指 酶浓度，由此可以说， kcat 表示了每单位时间内（秒）每摩尔的酶（或者说每摩尔的活性中心）能够把多少摩尔的底物转化成产物。
    '''
    ''' Km 俗称 米氏常数，以浓度做单位，米氏常数定义为 反应速度 达到 最大反应速度一半时 的底物浓度。 Km 可以反映出酶与底物的亲和力，Km越低，亲和力越大。
    ''' Kcat/Km 称为 催化效率，常常以此来比较 不同的酶而同一底物， 或者 不同底物而同一种酶。
    ''' </summary>
    ''' <remarks></remarks>
    Public Class EnzymeCatalystKineticLaw : Inherits SabiorkEntity

        Public Property enzyme As String()
        Public Property reaction As String
        Public Property KEGGReactionId As String
        Public Property Ec_number As String
        Public Property fast As Boolean
        Public Property reversible As Boolean
        Public Property PH As Double
        Public Property Temperature As Double
        Public Property Buffer As String
        Public Property PubMed As String()
        Public Property parameters As Dictionary(Of String, String)
        Public Property lambda As String
        Public Property xref As Dictionary(Of String, String())

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1}", enzyme, reaction)
        End Function

        Public Shared Function Create(rxn As SBMLReaction, math As LambdaExpression, doc As SBMLInternalIndexer) As EnzymeCatalystKineticLaw
            Dim experiment = rxn.kineticLaw.annotation.sabiork.experimentalConditions
            Dim exp As String = math.lambda.ToString
            Dim pubmeds As String() = rxn.kineticLaw.annotation.RDF.description.isDescribedBy _
                .Select(Function(i) i.Bag.list) _
                .IteratesALL _
                .Where(Function(li) Strings.InStr(li.resource, "pubmed") > 0) _
                .Select(Function(li) li.resource) _
                .ToArray
            Dim xrefs As Dictionary(Of String, String()) = Nothing
            Dim equation As String = doc.ToString(rxn, xrefs)
            Dim enzymes = doc.getEnzymes(rxn).ToArray
            Dim args As New Dictionary(Of String, String)

            Return New EnzymeCatalystKineticLaw With {
                .SabiorkId = rxn.kineticLaw.annotation.sabiork.kineticLawID,
                .Buffer = experiment.buffer,
                .PH = experiment.pHValue.startValuepH,
                .Temperature = experiment.temperature.startValueTemperature,
                .lambda = exp,
                .fast = rxn.fast,
                .reversible = rxn.reversible,
                .PubMed = pubmeds.Select(Function(url) url.Split("/"c).Last).ToArray,
                .Ec_number = rxn.ec_number,
                .KEGGReactionId = SBMLInternalIndexer.GetKeggReactionId(rxn).FirstOrDefault,
                .reaction = equation,
                .parameters = args,
                .xref = xrefs,
                .enzyme = enzymes.Select(Function(e) e.ToString).ToArray
            }
        End Function
    End Class

    ''' <summary>
    ''' 对酶分子的Modifier
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ModifierKinetics : Implements IKeyValuePair

        Public Enum ModifierTypes As Integer
            Inhibitor
            Activator
            CoFactors
        End Enum

        ''' <summary>
        ''' Sabio-rk Id
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property KineticsRecordId As String
        ''' <summary>
        ''' KEGG.Compound identifier string for <see cref="ModifierKinetics.Modifier">enzyme modifier</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Modifier(KEGG.Compound)")> Public Property KEGGCompoundId As String
        Public Property Modifier As String Implements IKeyValuePairObject(Of String, String).Key
        ''' <summary>
        ''' 具备附加属性的：Enzyme或者Reaction
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("RegulatedObject")> Public Property ObjectId As String Implements IKeyValuePairObject(Of String, String).Value
        <Column("ModifierType")> Public Property ModifierType As ModifierKinetics.ModifierTypes
        <Column("Kinetics")> Public Property K As Double

        Public Overrides Function ToString() As String
            Return String.Format("{0} -> {1};  k={2};", Modifier, ObjectId, K)
        End Function

        Public Shared Function TryGetType(strValue As String) As ModifierKinetics.ModifierTypes
            If String.Equals(strValue, "Modifier-Inhibitor") Then
                Return ModifierTypes.Inhibitor
            ElseIf String.Equals(strValue, "Modifier-Cofactor") Then
                Return ModifierTypes.CoFactors
            Else
                Return ModifierTypes.Activator
            End If
        End Function
    End Class

    Public Class KineticLawModel : Inherits SabiorkEntity

        <Column("EC-code")> Public Property Ec As String
        Public Property Equation As String
        <Column("sabiork.reaction")> Public Property Reaction As String
        Public Property Taxonomy As String
        <Column("fast")> Public Property Fast As Boolean
        <Column("kegg.reaction")> Public Property KeggReaction As String
        Public Property Catalyst As String()
        Public Property PubMed As String
        <Column("sabiork.kineticrecord")> Public Property KineticRecord As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
