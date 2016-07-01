Imports System.Xml.Serialization
Imports SMRUCC.genomics.Assembly.SBML
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic

Namespace GCML_Documents.XmlElements.Metabolism

    Public Class Metabolite
        Implements FLuxBalanceModel.IMetabolite
        Implements sIdEnumerable

        ''' <summary>
        ''' UniqueID.(本目标对象的唯一标识符)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> <XmlAttribute> Public Property Identifier As String Implements FLuxBalanceModel.IMetabolite.Identifier
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