Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Scripting.MathExpression
Imports SMRUCC.genomics.ComponentModel.Annotation
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.v2
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Kinetics
Imports BioCycKineticsFactor = SMRUCC.genomics.Data.BioCyc.KineticsFactor

''' <summary>
''' 由BioCyc数据库的 ``compounds.dat``/``reactions.dat``/``enzrxns.dat``/``pathways.dat``
''' 构建虚拟细胞模型之中的代谢网络结构
''' </summary>
Public Class ReactionNetworkCompiler

    ''' <summary>
    ''' 在BioCyc数据库没有给出反应的动力学参数的时候所使用的缺省最大反应速率
    ''' </summary>
    Public Const DefaultMaxVelocity As Double = 1000

    ReadOnly compiler As v2Compiler
    ReadOnly biocyc As Workspace

    ''' <summary>
    ''' 反应编号到催化该反应的酶的蛋白质编号的索引
    ''' </summary>
    ReadOnly enzymes As New Dictionary(Of String, List(Of String))
    ''' <summary>
    ''' 酶促反应编号到酶的蛋白质编号的索引
    ''' </summary>
    ReadOnly enzymeByEnzRxn As New Dictionary(Of String, String)

    Sub New(compiler As v2Compiler)
        Me.compiler = compiler
        Me.biocyc = compiler.biocyc

        For Each enz As enzrxns In biocyc.enzrxns.features
            If enz.enzyme.StringEmpty(, True) Then
                Continue For
            End If

            If Not enz.uniqueId.StringEmpty(, True) Then
                enzymeByEnzRxn(enz.uniqueId) = enz.enzyme
            End If

            For Each reaction As String In enz.reaction.SafeQuery
                If reaction.StringEmpty(, True) Then
                    Continue For
                End If

                Dim list As List(Of String) = enzymes.ComputeIfAbsent(reaction, Function() New List(Of String))

                If Not list.Contains(enz.enzyme) Then
                    Call list.Add(enz.enzyme)
                End If
            Next
        Next
    End Sub

    Public Function BuildModel() As MetabolismStructure
        Dim reactions As ReactionGroup = createReactions()

        Return New MetabolismStructure With {
            .reactions = reactions,
            .enzymes = createEnzyme().ToArray,
            .compounds = createCompounds().ToArray,
            .maps = New PathwayCompiler(biocyc).CreatePathwayMaps()
        }
    End Function

    Private Iterator Function createCompounds() As IEnumerable(Of Compound)
        For Each cpd As compounds In biocyc.compounds.features
            Yield New Compound With {
                .ID = cpd.uniqueId,
                .name = If(cpd.commonName, cpd.uniqueId),
                .formula = compounds.FormulaString(cpd),
                .smiles = cpd.SMILES,
                .inchi = cpd.InChI,
                .inchiKey = cpd.InChIKey,
                .mass = cpd.molecularWeight,
                .monoisotopicMass = cpd.exactMass,
                .charge = getCharge(cpd.atomCharges),
                .gibbs0 = cpd.Gibbs0,
                .classId = cpd.types.SafeQuery.FirstOrDefault,
                .synonyms = cpd.synonyms,
                .componentOf = cpd.componentOf,
                .locations = cpd.locations,
                .referenceIds = {cpd.uniqueId},
                .db_xrefs = cpd.dbLinks,
                .evidence = compiler.evidence.GetEvidenceIds(cpd.citations)
            }
        Next
    End Function

    ''' <summary>
    ''' 将BioCyc的原子电荷列表进行求和，得到化合物的净电荷
    ''' </summary>
    ''' <param name="charges"></param>
    ''' <returns></returns>
    Private Shared Function getCharge(charges As String()) As Double
        Dim value As Double = 0

        For Each charge As String In charges.SafeQuery
            Dim parsed As Double

            If Double.TryParse(charge, result:=parsed) Then
                value += parsed
            End If
        Next

        Return value
    End Function

    Private Iterator Function createEnzyme() As IEnumerable(Of Enzyme)
        Dim enzList = biocyc.enzrxns.features.GroupBy(Function(a) a.enzyme).ToArray

        For Each enz As IGrouping(Of String, enzrxns) In enzList
            Dim ecNumber As String = enz _
                .Select(Function(d) If(d.EC_number Is Nothing, Nothing, d.EC_number.ECNumberString)) _
                .Where(Function(id) Not id.StringEmpty(, True)) _
                .GroupBy(Function(id) id) _
                .OrderByDescending(Function(g) g.Count) _
                .Select(Function(g) g.Key) _
                .FirstOrDefault
            Dim kcat As BioCycKineticsFactor = enz _
                .Select(Function(a) a.Kcat.SafeQuery.FirstOrDefault) _
                .Where(Function(f) Not f Is Nothing) _
                .OrderByDescending(Function(f) f.Km) _
                .FirstOrDefault
            Dim km As BioCycKineticsFactor = enz _
                .Select(Function(a) a.Km.SafeQuery.FirstOrDefault) _
                .Where(Function(f) Not f Is Nothing) _
                .FirstOrDefault
            Dim vmax As Double = enz.Select(Function(a) a.Vmax).Where(Function(v) v > 0).DefaultIfEmpty(0).Max
            Dim cofactors As String() = enz _
                .Select(Function(a) a.cofactors) _
                .IteratesALL _
                .Distinct _
                .ToArray
            Dim catalysis As Catalysis() = enz _
                .Select(Function(a) createKinetics(a)) _
                .IteratesALL _
                .ToArray

            Yield New Enzyme With {
                .ECNumber = ecNumber,
                .proteinID = enz.Key,
                .KO = Nothing,
                .kcat = If(kcat Is Nothing, 0, kcat.Km),
                .km = If(km Is Nothing, 0, km.Km),
                .vmax = vmax,
                .kcatKm = getCatalyticEfficiency(kcat, km),
                .direction = getDirection(enz),
                .cofactors = cofactors,
                .catalysis = catalysis
            }
        Next
    End Function

    Private Shared Function getDirection(enz As IEnumerable(Of enzrxns)) As ReactionDirection
        Return MapDirection(enz.Select(Function(a) a.reactionDirection))
    End Function

    Private Shared Function getCatalyticEfficiency(kcat As BioCycKineticsFactor, km As BioCycKineticsFactor) As Double
        If kcat Is Nothing OrElse km Is Nothing Then
            Return 0
        End If
        If km.Km <= 0 Then
            Return 0
        End If

        Return kcat.Km / km.Km
    End Function

    Private Shared Function MapDirection(directions As IEnumerable(Of ReactionDirections)) As ReactionDirection
        If directions.Any(Function(d) d = ReactionDirections.Reversible) Then
            Return ReactionDirection.reversible
        ElseIf directions.Any(Function(d) d = ReactionDirections.IrreversibleLeftToRight) Then
            Return ReactionDirection.leftToRight
        ElseIf directions.Any(Function(d) d = ReactionDirections.IrreversibleRightToLeft) Then
            Return ReactionDirection.rightToLeft
        Else
            Return ReactionDirection.unknown
        End If
    End Function

    ''' <summary>
    ''' 为一个酶所催化的每一个反应分别构建动力学函数
    ''' </summary>
    ''' <param name="a"></param>
    ''' <returns></returns>
    Private Iterator Function createKinetics(a As enzrxns) As IEnumerable(Of Catalysis)
        Dim kcat As BioCycKineticsFactor = a.Kcat.SafeQuery.FirstOrDefault
        Dim km As BioCycKineticsFactor = a.Km.SafeQuery.FirstOrDefault
        Dim vmax As Double = If(a.Vmax > 0, a.Vmax, DefaultMaxVelocity)

        For Each reaction As String In a.reaction.SafeQuery
            If reaction.StringEmpty(, True) Then
                Continue For
            End If

            Dim substrate As String = If(kcat Is Nothing, Nothing, kcat.substrate)
            Dim parameters As KineticsParameter()
            Dim dynamics As FunctionElement

            If substrate.StringEmpty(, True) Then
                substrate = If(km Is Nothing, Nothing, km.substrate)
            End If

            If kcat Is Nothing OrElse km Is Nothing Then
                ' 仅仅知道米氏常数的时候，使用Vmax形式的动力学方程
                parameters = {
                    New KineticsParameter With {.name = "s", .target = substrate, .value = Double.NaN, .isModifier = False}
                }
                dynamics = New FunctionElement With {
                    .name = a.commonName,
                    .parameters = {"s"},
                    .lambda = $"({vmax} * s) / ({If(km Is Nothing, 1, km.Km)} + s)"
                }
            Else
                parameters = {
                    New KineticsParameter With {.name = "s", .target = substrate, .value = Double.NaN, .isModifier = False},
                    New KineticsParameter With {.name = "E", .target = a.enzyme, .value = Double.NaN, .isModifier = True}
                }
                dynamics = New FunctionElement With {
                    .name = a.commonName,
                    .parameters = {"E", "s"},
                    .lambda = $"(({kcat.Km} * E) * s) / ({km.Km} + s)"
                }
            End If

            Yield New Catalysis With {
                .characters = New EnzymeCharacteristics With {.pH_opt = If(a.PH = 0, 7, a.PH), .T_opt = If(a.temperature = 0, 23, a.temperature)},
                .reaction = reaction,
                .parameter = parameters,
                .formula = dynamics
            }
        Next
    End Function

    Private Shared Function CheckEnzymatic(rxn As reactions) As Boolean
        Return rxn.ec_number IsNot Nothing OrElse
            Not rxn.enzymaticReaction.IsNullOrEmpty
    End Function

    Private Function createReactions() As ReactionGroup
        Dim reactions = biocyc.reactions
        Dim enzymatic = reactions.features _
            .Where(Function(rxn) CheckEnzymatic(rxn)) _
            .Select(Function(r) enzymaticReaction(r)) _
            .Where(Function(r) Not r Is Nothing) _
            .ToArray
        Dim non_enzymatic = reactions.features _
            .Where(Function(rxn) Not CheckEnzymatic(rxn)) _
            .Select(Function(r) nonEnzymaticReaction(r)) _
            .Where(Function(r) Not r Is Nothing) _
            .ToArray
        Dim group As New ReactionGroup With {
            .none_enzymatic = non_enzymatic,
            .enzymatic = enzymatic
        }

        group.transportation = New TransportationCompiler(compiler.compartment) _
            .CreateTransportations(group.AsEnumerable)

        Return group
    End Function

    Private Function enzymaticReaction(rxn As reactions) As Reaction
        Dim model As Reaction = nonEnzymaticReaction(rxn)

        If Not model Is Nothing Then
            model.is_enzymatic = True
            model.ec_number = rxn.ec_number _
                .SafeQuery _
                .Where(Function(e) Not e Is Nothing) _
                .Select(Function(ec) ec.ECNumberString) _
                .ToArray
        End If

        Return model
    End Function

    Private Iterator Function CreateCompounds(list As IEnumerable(Of CompoundSpecieReference)) As IEnumerable(Of CompoundFactor)
        If list Is Nothing Then
            Return
        End If

        For Each cpd As CompoundSpecieReference In list
            Yield New CompoundFactor(cpd.Stoichiometry, cpd.ID, compiler.MapCompartId(cpd.Compartment))
        Next
    End Function

    Private Function nonEnzymaticReaction(rxn As reactions) As Reaction
        Dim left = CreateCompounds(rxn.left).ToArray
        Dim right = CreateCompounds(rxn.right).ToArray

        If left.IsNullOrEmpty OrElse right.IsNullOrEmpty Then
            Call $"Missing reactants or products from the reaction {rxn.uniqueId} ({rxn.commonName})!".warning
            Return Nothing
        End If

        Dim direction As ReactionDirection = MapDirection(rxn.reactionDirection)
        Dim bounds As Double() = GetBounds(direction)

        Return New Reaction With {
            .ID = rxn.uniqueId,
            .bounds = bounds,
            .direction = direction,
            .is_enzymatic = False,
            .spontaneous = rxn.spontaneous,
            .gibbs = rxn.gibbs0,
            .deltaG = rxn.gibbs0,
            .equilibriumConstant = rxn.equilibriumConstant,
            .stdReductionPotential = rxn.stdReductionPotential,
            .name = rxn.commonName,
            .substrate = left,
            .product = right,
            .compartment = compiler.MapCompartId(rxn.reactionLocations),
            .pathways = rxn.inPathway,
            .atomMappings = rxn.atomMappings,
            .enzymes = getEnzymes(rxn),
            .evidence = compiler.evidence.GetEvidenceIds(rxn.citations),
            .note = rxn.comment
        }
    End Function

    Private Function getEnzymes(rxn As reactions) As String()
        Dim result As New List(Of String)

        ' 反应对象的ENZYMATIC-REACTION槽位所指向的是酶促反应(Enzymatic-Reactions)对象，
        ' 需要先通过酶促反应对象索引到具体的酶分子
        For Each enzrxn As String In rxn.enzymaticReaction.SafeQuery
            Dim enzymeId As String = enzymeByEnzRxn.TryGetValue(enzrxn)

            If Not enzymeId.StringEmpty(, True) Then
                If Not result.Contains(enzymeId) Then
                    Call result.Add(enzymeId)
                End If
            End If
        Next

        ' 部分的反应对象直接使用反应编号作为索引
        Dim list As List(Of String) = enzymes.TryGetValue(rxn.uniqueId)

        For Each id As String In list.SafeQuery
            If Not result.Contains(id) Then
                Call result.Add(id)
            End If
        Next

        Return result.ToArray
    End Function

    Shared Function MapDirection(direction As ReactionDirections) As ReactionDirection
        Select Case direction
            Case ReactionDirections.Reversible
                Return ReactionDirection.reversible
            Case ReactionDirections.IrreversibleLeftToRight,
                 ReactionDirections.PhysiolLeftToRight
                Return ReactionDirection.leftToRight
            Case ReactionDirections.IrreversibleRightToLeft,
                 ReactionDirections.PhysiolRightToLeft
                Return ReactionDirection.rightToLeft
            Case Else
                Return ReactionDirection.unknown
        End Select
    End Function

    ''' <summary>
    ''' 根据反应的热力学方向给出反应速度的上下边界
    ''' </summary>
    ''' <param name="direction"></param>
    ''' <returns></returns>
    Public Shared Function GetBounds(direction As ReactionDirection) As Double()
        Select Case direction
            Case ReactionDirection.leftToRight
                Return {0, DefaultMaxVelocity}
            Case ReactionDirection.rightToLeft
                Return {-DefaultMaxVelocity, 0}
            Case Else
                Return {-DefaultMaxVelocity, DefaultMaxVelocity}
        End Select
    End Function

End Class
