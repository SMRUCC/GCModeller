Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel
Imports SMRUCC.genomics.Assembly.SBML.FLuxBalanceModel
Imports SMRUCC.genomics.Assembly.SBML.Level2.Elements
Imports SMRUCC.genomics.Assembly.SBML.Specifics.MetaCyc

Namespace Level2

    ''' <summary>
    ''' The level2 sbml xml file.
    ''' </summary>
    <XmlRoot("sbml", Namespace:=API.Namespace)>
    Public Class XmlFile : Inherits ITextFile
        Implements I_FBAC2(Of speciesReference)

        <XmlElement("model")> Public Shadows Property Model As Model
        <XmlAttribute> Public Property level As Integer
        <XmlAttribute> Public Property version As Integer

        Public Overrides Function ToString() As String
            Return API.Namespace & "   " & Model.ToString
        End Function

        Public Shared Shadows Widening Operator CType(File As String) As XmlFile
            Return XmlFile.Load(File)
        End Operator

        Public Shared Function Load(File As String) As Level2.XmlFile
            Dim doc As XmlFile = File.LoadTextDoc(Of XmlFile)(parser:=AddressOf __loadXml)
            Return doc.RevertEscapes(Escaping.DefaultEscapes)
        End Function

        Private Shared Function __loadXml(path As String, encoding As Encoding) As XmlFile
            Return path.LoadXml(Of XmlFile)(encoding, ThrowEx:=True, preprocess:=AddressOf __processNamespace)
        End Function

        Private Shared Function __processNamespace(doc As String) As String
            Dim sbr As StringBuilder = New StringBuilder(doc)
            Call sbr.Replace("<body xmlns=""http://www.w3.org/1999/xhtml"">", "<body>")
            Call sbr.Replace("<math xmlns=""http://www.w3.org/1998/Math/MathML"">", "<math>")

            Return sbr.ToString
        End Function

        Public Overrides Function Save(Optional FilePath As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            Return Me.SaveAsXml(getPath(FilePath), throwEx:=True, encoding:=Encoding)
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