Namespace OwlDocument

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class ResourceCollectionAttribute : Inherits Attribute
        Public Shared ReadOnly Property attrTypeId As Type =
            GetType(ResourceCollectionAttribute)
    End Class
End Namespace