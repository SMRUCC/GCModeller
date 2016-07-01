Imports System.Xml.Serialization
Imports SMRUCC.genomics.Model.SBML.Specifics.MetaCyc

Namespace Components

    Public MustInherit Class ModelBase : Inherits IPartsBase

        Public Overrides Function ToString() As String
            Return String.Format("<model id=""{0}"" name=""{1}"">", id, name)
        End Function
    End Class

    Public MustInherit Class IPartsBase

        <Escaped>
        <XmlAttribute> Public Overridable Property id As String
        <XmlAttribute> Public Overridable Property name As String
    End Class
End Namespace