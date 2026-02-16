#Region "Microsoft.VisualBasic::7296cd70935ccfc3f0f32e5c4ec5fe12, data\SABIO-RK\Dumps\Kinetics.vb"

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

    '   Total Lines: 187
    '    Code Lines: 135 (72.19%)
    ' Comment Lines: 35 (18.72%)
    '    - Xml Docs: 97.14%
    ' 
    '   Blank Lines: 17 (9.09%)
    '     File Size: 8.98 KB


    '     Class EnzymeCatalystKineticLaw
    ' 
    '         Properties: buffer, compartment, Ec_number, enzyme, fast
    '                     KEGGReactionId, lambda, parameters, PH, PubMed
    '                     reaction, reversible, rhea_id, temperature, uniprot_id
    '                     xref
    ' 
    '         Function: Create, ToString
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
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.xml.MathML
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.Xml.Models.KeyValuePair
Imports SMRUCC.genomics.Data.SABIORK.SBML

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

        ''' <summary>
        ''' list of compartment of current enzyme and kinetics
        ''' </summary>
        ''' <returns></returns>
        Public Property compartment As String()
        Public Property enzyme As Dictionary(Of String, String)
        Public Property reaction As String
        Public Property KEGGReactionId As String
        Public Property Ec_number As String
        Public Property fast As Boolean
        Public Property reversible As Boolean
        Public Property PH As Double
        Public Property temperature As Double
        Public Property buffer As String
        Public Property PubMed As String()
        Public Property parameters As Dictionary(Of String, String)
        Public Property lambda As String
        Public Property substrates As Dictionary(Of String, NamedCollection(Of String))
        Public Property products As Dictionary(Of String, NamedCollection(Of String))
        Public Property uniprot_id As String()
        Public Property rhea_id As String

        Public Overrides Function ToString() As String
            Return String.Format("({0}) {1}", enzyme.Values.GetJson, reaction)
        End Function

        Public Shared Function Create(rxn As SBMLReaction, math As LambdaExpression, doc As SBMLInternalIndexer) As EnzymeCatalystKineticLaw
            Dim experiment As experimentalConditions = rxn.kineticLaw.annotation.sabiork?.experimentalConditions
            Dim mathId As String = "KL_" & rxn.kineticLawID
            Dim exp As String = math.lambda.ToString
            Dim pubmeds As String() = rxn.kineticLaw.annotation.RDF.description _
                .Select(Function(part)
                            Return part.isDescribedBy.Select(Function(i) i.Bag.list)
                        End Function) _
                .IteratesALL _
                .IteratesALL _
                .Where(Function(li) Strings.InStr(li.resource, "pubmed") > 0) _
                .Select(Function(li) li.resource) _
                .ToArray
            Dim left As Dictionary(Of String, NamedCollection(Of String)) = Nothing
            Dim right As Dictionary(Of String, NamedCollection(Of String)) = Nothing
            Dim equation As String = doc.ToString(rxn, left, right)
            Dim enzymes = doc.getEnzymes(rxn).ToArray
            Dim args As New Dictionary(Of String, String)
            Dim ci As String() = rxn.kineticLaw.math.apply.ci _
                .Select(AddressOf Strings.Trim) _
                .Where(Function(a) a <> mathId) _
                .ToArray
            Dim locals = rxn.kineticLaw.listOfLocalParameters.ToDictionary(Function(l) l.id)
            Dim locations As String() = enzymes _
                .Select(Function(e) doc.getCompartmentName(e.compartmentId)) _
                .ToArray

            For i As Integer = 0 To ci.Length - 1
                Dim name As String = math.parameters(i)
                Dim ci_id As String = ci(i)

                If ci_id.StringEmpty Then
                    ci_id = ci.Where(Function(e) e.StartsWith("KL")).FirstOrDefault
                End If

                If Not args.ContainsKey(name) Then
                    If locals.ContainsKey(ci_id) Then
                        args.Add(name, locals(ci_id).value)
                    Else
                        args.Add(name, ci_id)
                    End If
                End If
            Next

            Return New EnzymeCatalystKineticLaw With {
                .SabiorkId = mathId,
                .buffer = If(experiment Is Nothing, "*", Strings.Trim(experiment.buffer)),
                .PH = If(experiment Is Nothing OrElse experiment.pHValue Is Nothing, 7.0, experiment.pHValue.startValuepH),
                .temperature = If(experiment Is Nothing OrElse experiment.temperature Is Nothing, 36, experiment.temperature.startValueTemperature),
                .lambda = exp,
                .fast = rxn.fast,
                .reversible = rxn.reversible,
                .PubMed = pubmeds.Select(Function(url) url.Split("/"c).Last).ToArray,
                .Ec_number = rxn.ec_number,
                .KEGGReactionId = SBMLInternalIndexer.GetKeggReactionId(rxn).FirstOrDefault,
                .reaction = equation,
                .parameters = args,
                .substrates = left,
                .products = right,
                .enzyme = enzymes.ToDictionary(Function(e) e.id, Function(e) e.name),
                .compartment = locations,
                .uniprot_id = enzymes _
                    .Select(Function(e)
                                Return e.db_xrefs.SafeQuery _
                                    .Where(Function(a) a.DBName.TextEquals("uniprot"))
                            End Function) _
                    .IteratesALL _
                    .Select(Function(a) a.entry) _
                    .Distinct _
                    .ToArray
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
