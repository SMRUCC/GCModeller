Imports System.Xml.Serialization
Imports LANS.SystemsBiology.Assembly.SBML.Components
Imports LANS.SystemsBiology.Assembly.SBML.FLuxBalanceModel
Imports LANS.SystemsBiology.Assembly.SBML.Specifics.MetaCyc

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
        Public Overrides Property ID As String Implements IMetabolite.Identifier
        <XmlAttribute("initialAmount")>
        Public Property InitialAmount As Double Implements IMetabolite.InitializeAmount
        <Escaped>
        <XmlAttribute("compartment")>
        Public Overrides Property CompartmentID As String
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
            Return String.Format("[{0}]{1} [{2}]", ID, name, CompartmentID)
        End Function
    End Class
End Namespace