'Imports Microsoft.VisualBasic.Terminal.stdio  

'Namespace Builder

'    ''' <summary>
'    ''' 将通用蛋白质底物进行展开
'    ''' </summary>
'    ''' <remarks></remarks>
'    Public Class ExpendGeneralProtein : Inherits IBuilder

'        Sub New(MetaCyc As MetaCyc.File.FileSystem.DatabaseLoadder, Model As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.Model)
'            Call MyBase.New(MetaCyc, Model)
'        End Sub

'        Public Overrides Function Invoke() As Model
'            Dim originalCounts = Model.ProteinAssemblies.Count

'            For i As Integer = 0 To originalCounts - 1
'                Dim sourceRxn = Model.ProteinAssemblies(i)
'                Dim hwnd As Integer = ContainsGeneralSubstrate(sourceRxn)   '判断这个反应对象是否具有通用蛋白质底物对象
'                If hwnd > -1 Then
'                    Call Printf("GENERAL_RULE:: %s", sourceRxn.ToString)
'                    For Each protein In Model.Proteins
'                        Dim expendedRxn = Copy(sourceRxn)
'                        expendedRxn.Metabolites(hwnd).species = protein.UniqueId
'                        expendedRxn.UniqueID = String.Concat(New String() {protein.UniqueId, "-", expendedRxn.UniqueID})
'                        Call Model.ProteinAssemblies.Add(expendedRxn)
'                    Next
'                    '销毁源目标对象
'                    i -= 1
'                    Call Model.ProteinAssemblies.Remove(sourceRxn)
'                End If
'            Next

'            MyBase.Model.ProteinAssemblies = Model.ProteinAssemblies
'            Return MyBase.Model
'        End Function

'        ''' <summary>
'        ''' 按值复制对象
'        ''' </summary>
'        ''' <param name="rxn"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Shared Function Copy(rxn As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction
'            Dim CopyObject As LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction = New Elements.Reaction
'            CopyObject.BaseType = rxn.BaseType
'            ' CopyObject.EnzymaticRxn = rxn.EnzymaticRxn
'            CopyObject.Enzymes = rxn.Enzymes
'            CopyObject.Keq_1 = rxn.Keq_1
'            CopyObject.Keq_2 = rxn.Keq_2
'            CopyObject.LOWER_BOUND = rxn.LOWER_BOUND
'            CopyObject.Name = rxn.Name
'            CopyObject.ObjectiveCoefficient = rxn.ObjectiveCoefficient
'            CopyObject.Regulators = rxn.Regulators
'            CopyObject.Reversible = rxn.Reversible
'            CopyObject.UniqueID = rxn.UniqueID
'            CopyObject.UPPER_BOUND = New LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction.Parameter With {.Value = 10} ' rxn.UPPER_BOUND

'            Dim CopySpecieRef As System.Func(Of List(Of LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference), List(Of LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference)) =
'                Function(source As List(Of LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference)) _
'                    (From ref As LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference
'                     In source
'                     Select ref.CopyData()).ToList

'            CopyObject.Products = CopySpecieRef(rxn.Products)
'            CopyObject.Reactants = CopySpecieRef(rxn.Reactants)

'            Return CopyObject
'        End Function

'        ''' <summary>
'        ''' 仅判断通用的蛋白质底物的存在
'        ''' </summary>
'        ''' <param name="rxn"></param>
'        ''' <returns></returns>
'        ''' <remarks></remarks>
'        Public Shared Function ContainsGeneralSubstrate(rxn As Assembly.DocumentFormat.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.Reaction) As Integer
'            Dim Collection = rxn.Metabolites
'            Dim LQuery = (From hwnd As Integer In Collection.Sequence
'                          Let [sub] As LANS.SystemsBiology.Assembly.SBML.Level2.Elements.Reaction.speciesReference = Collection(hwnd)
'                          Where String.Equals([sub].species, "General-Protein-Substrates")
'                          Select hwnd).ToArray   '
'            If LQuery.IsNullOrEmpty Then
'                Return -1
'            Else
'                Return LQuery.First
'            End If
'        End Function
'    End Class
'End Namespace